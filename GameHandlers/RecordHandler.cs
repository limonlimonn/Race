using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using HCR;
using HCR.Enums;
using HCR.Interfaces;
using System;
using HCR.Event;

namespace HCR.Gameplay.AsyncMultiplayer
{
    /// <summary>
    /// Класс - хэндлер для записи заезда для другого игрока ("Игра: Мультиплеер")
    /// </summary>

    public class RecordHandler : ABaseRecordHandler, IGameHandler
    {
        // FIELDS

        private Coroutine _coroutineRecord;
        private Coroutine _coroutineGhost;

        // dependences
        private GameWindowAsyncMultiplayer _gameWindow;
        private ABaseGameWindow _aBaseGameWindow;
        private PauseWindow _pauseWindow;
        private RetryWindow _retryWindow;
        private CrashFinishWindow _crashFinishWindow;
        private RecordFinishPanel _recordFinishPanel;
        private StatisticsService _stasticService;
        private SafePlayerPrefs _safePlayerPrefs;
        private int TimeInfo = 10;
        private int Timer = 10;


        // I_HANDLER

        #region I_HANDLER_INTERFACES

        public void Init(GameData data)
        {
            GameDataManager._init.SetGameData(data);
            _gameData = GameDataManager._init.GameData;
            EventManager._init.Game.TrackEvent.GetEvent.Invoke_Awake();
        #region Init
            //pre-conditions
            Assert.AreNotEqual(null, data);

            _carConstructor = Core.Instance.GetService<CarConstructor>();
            Assert.AreNotEqual(null, _carConstructor);

            _gameManager = Core.Instance.GetService<GameManager>();
            Assert.AreNotEqual(null, _gameManager);

            _playerManager = Core.Instance.GetService<PlayerManager>();
            Assert.AreNotEqual(null, _playerManager);

            //
            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);

            _gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_ASYNC) as GameWindowAsyncMultiplayer;
            Assert.AreNotEqual(null, _gameWindow);

            _pauseWindow = _uiManager.GetWindow(UIWindowEnum.PAUSE_ASYNC) as PauseWindow;
            Assert.AreNotEqual(null, _pauseWindow);

            _retryWindow = _uiManager.GetWindow(UIWindowEnum.RETRY_ASYNC) as RetryWindow;
            Assert.AreNotEqual(null, _retryWindow);

            _crashFinishWindow = _uiManager.GetWindow(UIWindowEnum.CRASH_FINISH_ASYNC) as CrashFinishWindow;
            Assert.AreNotEqual(null, _crashFinishWindow);

            _recordFinishPanel = _uiManager.GetWindow(UIWindowEnum.RECORD_FINISH_ASYNC) as RecordFinishPanel;
            Assert.AreNotEqual(null, _recordFinishPanel);

            _recordObjectController = Core.Instance.GetService<RecordObjectController>();
            Assert.AreNotEqual(null, _recordObjectController);
            //
            _audioService = Core.Instance.GetService<AudioService>();
            Assert.AreNotEqual(null, _audioService);

            _stasticService = Core.Instance.GetService<StatisticsService>();
            Assert.AreNotEqual(null, _stasticService);

            _safePlayerPrefs = Core.Instance.GetService<SafePlayerPrefs>();
            Assert.AreNotEqual(null, _safePlayerPrefs);

            _replyObjectController = Core.Instance.GetService<ReplyObjectController>();



            _aBaseGameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_ASYNC) as ABaseGameWindow;

            #endregion
            if (_gameData.GetTryes == 0 && _gameData.GameType != GameTypeEnum.MultyNew)
            {
                int TryesCount = _safePlayerPrefs.GetTryse(_gameData.gameId);
                _safePlayerPrefs.ChangeTryse(TryesCount, _gameData.gameId);
            }

            
            _gameWindow.InitGameData(_gameData);
            _gameManager.LoadTrack(OnTrackLoaded);

        }

        public void Start()
        {

            //new 
            

            
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
            _gameWindow.Show();
            //
            _gameData.IsFinishedRecord = 0;

            _recordObjectController.Init();
            _gameWindow.DeActiveJewels();


            //InitTimeInfo();
            _gameWindow.DeActiveGold();




            if (!_playerManager.TrackOpen.Contains(Convert.ToInt32(_gameData.track_id) + 1))
            {
                //coroutineStartTimeInfo = Core.Instance.StartCor(СheckTimeInfo());
            }

            Debug.Log("Tryes in Start " + _gameData.GetTryes);
            _coroutineStartTimer = Core.Instance.StartCor(
                StartTimer(
                    _gameWindow,

                    () => {
                        if (_gameData.GetTryes <= 0)
                        {
                           
                            //_finishWindow.OnBuyTriesClick += BuyTries;

                            _playerCar.EnablePlayerControll(false);
                            CalculateResults();
                            if (_coroutineStartTimer != null)
                            {
                                Core.Instance.StopCor(_coroutineStartTimer);
                            }
                        }

                        _coroutineRecord = Core.Instance.StartCor(Record());
                        if (_replayData.Count > 0)
                            _coroutineGhost = Core.Instance.StartCor(MoveGhost());
                        EventManager._init.Game.TrackEvent.GetEvent.Invoke_Start();

                    }
                )
            );
        }

        public void OnPlayerCrash()
        {
            if (_gameData.GameType != GameTypeEnum.MultyNew)
                _safePlayerPrefs.ChangeTryse(_gameData.GetTryes, _gameData.gameId);
            SafeStopCoroutine_Record();
            SafeStopCoroutine_Ghost();
            if (_gameData.GetTryes < 3)
            {
                Zaglushka_CrashAndThenFinish();
                CheckWhatWindowShowOnCrash();
            }
            else
            {
                BuyTriesQuestion();
            }
        }

        public void Restart()
        {

            
            if (_gameData.GameType != GameTypeEnum.MultyNew)
                _safePlayerPrefs.ChangeTryse(_gameData.GetTryes, _gameData.gameId);
            //_gameWindow.InfoWindowPreTime.text = "";
            if (_gameData.GetTryes < 3)
            {
                CheckWhatDoingOnRestart();
            }
            else
            {
                BuyTriesQuestion();
            }
        }

        public void TryAgain()
        {
            //_gameWindow.InfoWindowPreTime.text = "";
            RestartWork_HaveTries();
        }


        public void OnPlayerFinish()
        {

            //----------
            int time = _gameManager.TrackTimeById[Convert.ToInt32(_gameData.track_id)].Time * 100;
           if (TimerRace._init.GetTime < time)
            {
                _gameData.IsDone = 1;
            }

            _audioService.StopGameMusic();

            Zaglushka_FinishAndThenCrash();
            

           EventManager._init.Game.CarEvent.Player.GetEvent.Finish -= OnPlayerFinish;

            _playerCar.EnablePlayerControll(false);

            _coroutineMovePlayerOnFinish = Core.Instance.StartCor(MovePlayerOnFinish());

            //_gameData.recordData = "";
            _gameData.IsFinishedRecord = 1;
            CalculateResults();
        }

        public void CalculateResults()
        {
            Debug.Log("CalculateResults");
            if (!_playerManager.TrackOpen.Contains(Convert.ToInt32(_gameData.track_id) + 1))
            {
                Core.Instance.StopCor(_coroutineStartTimeInfo);
            }

            FillingStatistic();
            //_gameWindow.ResetInfoWindowTime();
            // Delete Ghost Data
            _gameData.ghostData.Clear();
            _replayData.Clear();
            //
            _audioService.StopGameMusic();
            _recordFinishPanel.SetValue_GameData(_gameData);
            _recordFinishPanel.Show();
        }


        public void Destroy()
        {
            if (!_playerManager.TrackOpen.Contains(Convert.ToInt32(_gameData.track_id) + 1))
            {
                Core.Instance.StopCor(_coroutineStartTimeInfo);
            }
            Core.Instance.UnMute();
            _audioService.StopGameMusic();

            UnsubscribeOnEvents();

            SafeStopCoroutine_StartTimer();
            SafeStopCoroutine_Record();
            SafeStopCoroutine_Ghost();
            _playerManager.PlayerCar = null;
            if (_playerCar != null)
                _playerCar.Destroy();
        }

        #endregion



        // METHODS



        private void OnTrackLoaded()
        {
            EventManager._init.Game.TrackEvent.GetEvent.Invoke_OnLoad();

            HideSilverCoins();
            HideGoldCoins();

            _gameWindow.Hide();
            

            SetPlayerCarSettings(_gameWindow);
            _gameWindow.SetValue_PlayerCar();

            if (_gameData.ghostData.Count > 0)
            {
                _replayData = new List<Vector4>(_gameData.ghostData);
                SetShadowCarSettings();
                //GetReplayFrame(0);
                replayFrame = 0;
                _replyObjectController.Init();
                _replyObjectController.ObjectsReplayInfo = _gameData.ObjectsReplayGhost;

            }







            SubscribeOnEvents();

            Core.Instance.GetService<UIManager>().Get_MainScreenStateManager().
                SwitchState(MainScreenStatesEnum.GAME_ASYNC);

            if (_replyObjectController.IsFind && _gameData.ghostData.Count > 0)
            {

                _replyObjectController.SetObjectTransformDict(_replayData);
                _replyObjectController.DisablePhysics();
            }

            TimerRace timerRace = new TimerRace();
            Start();
        }

        private void FillingStatistic()
        {
            //SaveRecordData();

            

            var save = _safePlayerPrefs;
            float Crash = 12;
            if (_gameData.IsFinishedRecord != 1)
            {
                Crash = _gameWindow._playerCar.transform.position.x;
            }
            //_stasticService.Create_Record_data(_gameData.track_id, _gameWindow.triesCount, _playerCar.name, _playerCar.timer, save.GetFloat_ExpForTrick(), _gameData.IsFinishedRecord, Crash, _playerManager.PlayerId,_playerManager.PlayerDisplayName, _gameData, _playerManager.selectedCar);

            if (_gameData.GameType == GameTypeEnum.MultyNew)
            {
                _stasticService.UserStatistic.Clear();
            }

            _stasticService.CreateUserStatistic(save.GetInt_TrickFlipSum(), save.GetFloat_HorseTime(), save.GetFloat_90Time(), save.GetFloat_TrickAirTime(), save.GetMaxFlip, save.GetMaxBalance, save.GetMaxBlunt, save.GetMaxInAir, save.GetFloat_ExpForTrick(), _gameData);


           
        }

        protected override void OnPlayerFinishClick()
        {

            _crashFinishWindow.Hide();
            _crashFinishWindow.OnSendRecord -= OnPlayerFinishClick;

            base.OnPlayerFinishClick();
        }

        private IEnumerator Record()
        {
            YieldInstruction _fupd = new WaitForFixedUpdate();
            _rawData.Clear();
            Vector3 projected;
            float angle;
            float targetAngle;

            while (true)
            {
                projected = Vector3.ProjectOnPlane(_playerCar.Transform.up, Vector3.forward);
                angle = Vector3.Angle(projected, Vector3.up);
                targetAngle = ((Vector3.Angle(Vector3.right, projected) > 90f) ? angle : 360f - angle);

                _rawData.Add(new Vector4(_playerCar.Transform.position.x,
                    _playerCar.Transform.position.y, targetAngle, _playerCar.nitro.IsNitroPressed));

                if (_recordObjectController.IsFind)
                    _recordObjectController.RecordObjectInFrame();

                yield return _fupd;

            }



        }



        private IEnumerator MoveGhost()
        {
            YieldInstruction _fghost = new WaitForFixedUpdate();

            while (replayFrame < _replayData.Count)
            {
                GetReplayFrame(replayFrame);
                replayFrame++;

                yield return _fghost;
            }




            if (_ghostCar.IsFinish == false)
            {

                if (_ghostCar != null)
                    _ghostCar.ExploseEnemy();
                //if (_calcMetersToEnemy != null)
                //  Core.Instance.StopCor(_calcMetersToEnemy);
                //_gameWindow.enemyCountMeter.text = "crash";
            }
            else
            {
                var i = 0;
                while (i < 100)
                {
                    i++;
                    GetReplayFrame(_replayData.Count - 1);
                }
            }

            if (_replyObjectController.IsFind)
            {
                _replyObjectController.EnablePhysics();
            }


        }

        Vector4 replayFramePos;
        Vector3 replayFrameRot;
        private void GetReplayFrame(int frame)
        {
            try
            {
                _ghostCar.RotateEnemyWheels(
                    Mathf.Sign(replayFramePos.x
                    - _replayData[frame].x)
                    * 360f * (_replayData[frame] -
                    replayFramePos).magnitude /
                    (2 * Mathf.PI * _ghostCar.wheelRadius));
                replayFramePos = _replayData[frame];
                _ghostCar.Transform.position = new Vector3(replayFramePos.x, replayFramePos.y, -2f);
                _ghostCar.Transform.eulerAngles = new Vector3(replayFramePos.z, 270, 0);

                if (_replayData[frame].w == 1)
                {
                    _ghostCar.nitro.startParticle();
                }
                else
                {
                    _ghostCar.nitro.stopParticle();
                }
            }
            catch (ArgumentOutOfRangeException)
            {

                Debug.LogError("ArgumentOutOfRangeException");
                return;

            }

            Vector3 projected = Vector3.ProjectOnPlane(_playerCar.Transform.up, Vector3.forward);
            float angle = Vector3.Angle(projected, Vector3.up);
            float targetAngle = ((Vector3.Angle(Vector3.right, projected) > 90f) ? angle : 360f - angle);

            if (_replyObjectController.IsFind)
                _replyObjectController.AplyRecord(frame);

        }

        //      private IEnumerator RecordForVelocity()
        //{
        //	YieldInstruction _fupd = new WaitForFixedUpdate();

        //          _rawData.Clear();
        //          Vector3 projected;
        //          float angle;
        //          float targetAngle;

        //          while (true)
        //	{
        //              projected = Vector3.ProjectOnPlane(_playerCar.Transform.up, Vector3.forward);
        //              angle = Vector3.Angle(projected, Vector3.up);
        //              targetAngle = ((Vector3.Angle(Vector3.right, projected) > 90f) ? angle : 360f - angle);

        //              _rawData.Add(new RecordParameter(new Vector3(_playerCar.Transform.position.x,
        //                  _playerCar.Transform.position.y, targetAngle), _playerCar.Rigidbody.velocity,
        //                  _playerCar.Rigidbody.angularVelocity));

        //              yield return _fupd;

        //          }
        //}

        //private IEnumerator RecordForInput()
        //{
        //    YieldInstruction _fupd = new WaitForFixedUpdate();

        //    while (true)
        //    {

        //        InputData.Add(new InputParameter(_playerCar.controller.steerInput, _playerCar.controller.throttleInput,
        //       _playerCar.controller.brakeInput, _playerCar.controller.handbrakeInput, _playerCar.nitro.nitroPower,
        //       _playerCar.airController.rotateAndroid, _playerCar.nitro.Sin, _playerCar.nitro.Cos));


        //        yield return _fupd;
        //    }
        //}

        #region SHOW_WINDOW

        protected override void ShowWindow_Retry()
        {


            Core.Instance.StartCor(Wait(1.5f, _retryWindow.Show));
        }

        protected override void ShowWindow_CrashFinish()
        {

            FillingStatistic();
            //_gameWindow.ResetInfoWindowTime();
            _crashFinishWindow.SetValue_GameData(_gameData);

            Core.Instance.StartCor(Wait(1.5f, _crashFinishWindow.Show));
        }

        private void BuyTriesQuestion()
        {
            Core.Instance.StartCor(Wait(1f, () =>
            {
                EventManager._init.Game.PlayerEvent.Invoke_BuyTries();
                EventManager._init.Game.PlayerEvent.CancleBuyTries += CalculateResults;
            }));
        }

        IEnumerator Wait(float time, Action Function)
        {
            yield return new WaitForSeconds(1.5f);
            Function();
        }

        #endregion

        #region RESTART_WORK

        protected override void RestartWork_HaveTries()
        {
            if (_rawData != null && _rawData.Count > 0)
            {
                _gameData.ghostData = new List<Vector4>(_rawData);
            }

            if (_recordObjectController.ObjectsRecordInfo != null)
            {
                _gameData.ObjectsReplayGhost = new Dictionary<int, Dictionary<int, RecordableObjectModel>>(_recordObjectController.ObjectsRecordInfo);
            }

            //_gameWindow.OnTimeInfo += ShowTimeInfo;
            Destroy();
            _gameData.recordData = "";
            _gameManager.Restart(OnTrackLoaded);
        }

        protected override void RestartWork_HaveNoTriesLeft()
        {
            EventManager._init.Game.CarEvent.Player.GetEvent.Finish -= OnPlayerFinish;
            EventManager._init.Game.CarEvent.Player.GetEvent.Crash -= OnPlayerCrash;
            //_gameWindow.OnTimeInfo -= ShowTimeInfo;
            CalculateResults();
            SafeStopCoroutine_Record();
            SafeStopCoroutine_Ghost();
            SafeStopCoroutine_StartTimer();

        }

        #endregion

        #region SUBSCRIBE/UNSUBSCRIBE

        private void SubscribeOnEvents()
        {
            _recordFinishPanel.OnSendRecord += OnPlayerFinishClick;
            _crashFinishWindow.OnSendRecord += OnPlayerFinishClick;
            EventManager._init.Game.MenuButton.TryAgain += TryAgain;
            //_gameWindow.OnTimeInfo += ShowTimeInfo;
            EventManager._init.Game.MenuButton.Restart += Restart;
            EventManager._init.Game.CarEvent.Player.GetEvent.Finish += OnPlayerFinish;
            EventManager._init.Game.CarEvent.Player.GetEvent.Crash += OnPlayerCrash;
        }

        private void UnsubscribeOnEvents()
        {
            _recordFinishPanel.OnSendRecord -= OnPlayerFinishClick;
            _crashFinishWindow.OnSendRecord -= OnPlayerFinishClick;
            EventManager._init.Game.MenuButton.TryAgain -= TryAgain;
            //_gameWindow.OnTimeInfo -= ShowTimeInfo;
            EventManager._init.Game.MenuButton.Restart -= Restart;
            EventManager._init.Game.CarEvent.Player.GetEvent.Finish -= OnPlayerFinish;
            EventManager._init.Game.CarEvent.Player.GetEvent.Crash -= OnPlayerCrash;

            //_gameWindow.OnPauseClick -= Core.Instance.Mute;
            EventManager._init.Game.MenuButton.Resume -= Core.Instance.UnMute;
            //
            EventManager._init.Game.CarEvent.Player.GetEvent.Crash -= _playerCar.ExplosePlayer;
        }

        #endregion

        #region ZAGLUSHKI

        private void Zaglushka_CrashAndThenFinish()
        {
            // заглушка - чтоб если разбились возле финиша и тушка машины на него заехала
            // то не засчитывали победы в заезде

            EventManager._init.Game.CarEvent.Player.GetEvent.Finish -= OnPlayerFinish;
            _playerCar.ExplosePlayer();
        }

        private void Zaglushka_FinishAndThenCrash()
        {
            // заглушка - чтоб когда дошел до финица и взорвался
            // не показывалось окно "RetryWindow", но показывался взрыв

            EventManager._init.Game.CarEvent.Player.GetEvent.Crash -= OnPlayerCrash;
            EventManager._init.Game.CarEvent.Player.GetEvent.Crash += _playerCar.ExplosePlayer;
        }

        #endregion

        protected override void SafeStopCoroutine_Record()
        {
            if (_coroutineRecord != null)
            {
                Core.Instance.StopCor(_coroutineRecord);
            }

            _coroutineRecord = null;
        }

        private void SafeStopCoroutine_Ghost()
        {
            if (_coroutineGhost != null)
            {
                Core.Instance.StopCor(_coroutineGhost);
            }

            _coroutineGhost = null;
        }







    }
}