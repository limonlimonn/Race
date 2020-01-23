using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using HCR.Enums;
using HCR.Interfaces;
using UnityEngine.Assertions;
using HCR.Event;

namespace HCR.Gameplay.AsyncMultiplayer
{
    /// <summary>
    /// Класс - хэндлер для заезда с "призраком" ("Игра: Мультиплеер")
    /// </summary>

    public class ReplayHandler : IGameHandler
    {
        // PROPERTIES

        public int TryesCount { get; private set; }

        // FILELDS

        #region VARIABLES
        private Dictionary<int, Dictionary<int, RecordableObjectModel>> ObjectTransformDict;
        private CarBase _playerCar;
        private EnemyCar _enemyCar;

        private string[] exp;
        private Coroutine _moveEnemyCor;
        private Coroutine _startTimer;
       
        private Coroutine _movePlayerOnFinish;
        #endregion

        // dependences
        private SafePlayerPrefs _safePlayerPrefs;
        private CarConstructor _carConstructor;
        private GameManager _gameManager;
        private PlayerManager _playerManager;
        
        private UIManager _uiManager;
        private FinishWindow _finishWindow;
        private PauseWindow _pauseWindow;
        private GameWindowAsyncMultiplayer _gameWindow;
        private RetryWindow _retryWindow;
        private AudioService _audioService;
        private StatisticsService _statisticsService;
        // must be set from another script
        private GameData _gameData;
        private double metersToEnemy;
        int replayFrame = 0;
        bool isEnemyFinish = false;
        float lastVelocity;
        private float previousSecondCounter;
        private int CountTriesBuy = 1;
        private string audioCounterPath = "event:/UI Sounds/film_countdown";
        private bool IsBuyTries = false;
        private bool IsPlayerCrash = false;

        // METHODS

        public void Init(GameData data)
        {

            GameDataManager._init.SetGameData( data);
            _gameData = GameDataManager._init.GameData;
            
            if (TryesCount == 0 && _gameData.GameType != GameTypeEnum.MultyNew)
            {
                TryesCount = _safePlayerPrefs.GetTryse(_gameData.gameId);
                _safePlayerPrefs.ChangeTryse(TryesCount, _gameData.gameId);
            }
            //Debug.Log("TryesCount Finity " + TryesCount);
            _gameWindow.InitGameData(_gameData);
            _gameManager.LoadTrack(OnTrackLoaded);
            


            //


        }

        // CONSTRUCTOR

        public ReplayHandler()
        {
            _safePlayerPrefs = Core.Instance.GetService<SafePlayerPrefs>();
            _carConstructor = Core.Instance.GetService<CarConstructor>();
            _gameManager = Core.Instance.GetService<GameManager>();
            _playerManager = Core.Instance.GetService<PlayerManager>();
            _uiManager = Core.Instance.GetService<UIManager>();
            _finishWindow = _uiManager.GetWindow(UIWindowEnum.FINISH_ASYNC) as FinishWindow;
            _pauseWindow = _uiManager.GetWindow(UIWindowEnum.PAUSE_ASYNC) as PauseWindow;
            _gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_ASYNC) as GameWindowAsyncMultiplayer;
            _retryWindow = _uiManager.GetWindow(UIWindowEnum.RETRY_ASYNC) as RetryWindow;
           

            _audioService = Core.Instance.GetService<AudioService>();
            Assert.AreNotEqual(null, _audioService);

            _statisticsService = Core.Instance.GetService<StatisticsService>();
            Assert.AreNotEqual(null, _statisticsService);

            
        }


        private void ResetEarnedValues()
        {
            _gameData.IsFinishedReplay = 0;
            _gameWindow.DeActiveJewels();
            _safePlayerPrefs.SaveEarnedJewels(0);
            _safePlayerPrefs.SaveEarnedGold(0);
            _safePlayerPrefs.SaveEarnedExp(0);
            _safePlayerPrefs.SavePlayerGold(0);
            _safePlayerPrefs.SaveTrickHorseTime(0f);
            _safePlayerPrefs.SaveTrickFlipSum(0);
            _safePlayerPrefs.SaveTrickAirTime(0f);
            _safePlayerPrefs.SaveTrick90Time(0);
            _safePlayerPrefs.SaveEarnedExpForTrick(0);
        }

        //TODO get start positions from track

       

        void OnTrackLoaded()
        {
            EventManager._init.Game.TrackEvent.GetEvent.Invoke_OnLoad();
            //_gameData.TryesCount = TryesCount;

            IsPlayerCrash = false;
            //_safePlayerPrefs.SaveEarnedJewels(0);
            
            _gameWindow.Hide();
            
            _finishWindow.OnFinishClick += OnPlayerFinishClick;
            EventManager._init.Game.MenuButton.Restart += Restart;
            EventManager._init.Game.MenuButton.TryAgain += Restart;
            
           


            //gameWindow.triesCount.text = Mathf.Abs((TryesCount - 3)).ToString();

            InitPlayerCar();
            InitEnemyCar();
            _gameWindow.SetValue_PlayerCar();

            EventManager._init.Game.CarEvent.Player.GetEvent.Finish += OnPlayerFinish;
            EventManager._init.Game.CarEvent.Player.GetEvent.Crash += OnPlayerCrash;
            EventManager._init.Game.CarEvent.Enemy.GetEvent.Finish += EnemyFinish;

            //_gameWindow.OnPauseClick += Core.Instance.Mute;
            EventManager._init.Game.MenuButton.Resume += Core.Instance.UnMute;

            _playerManager.PlayerCar = _playerCar;

            

            

            
            replayFrame = 0;
            _enemyCar.carBase.controller.isEnemy = false;
            _enemyCar.carBase.controller.enabled = false;
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
            TimerRace timerRace = new TimerRace();
            Start();
        }


        private void EnemyFinish()
        {
            isEnemyFinish = true;
        }


        private void InitPlayerCar()
        {
            _playerCar = _carConstructor.CreatePlayerCar(_playerManager.selectedCar, _playerManager.selectedCar.current_color);
            _playerManager.PlayerCar = _playerCar;
        }

        private void InitEnemyCar()
        {
            if (_gameData.replayData != null)
            {
                _enemyCar = new EnemyCar();
                var _enemy_model = _gameData.player1_Id == _playerManager.PlayerId ?
                    _gameData.player2_car : _gameData.player1_car;
                CarBase car = _carConstructor.CreateEnemyCar(_enemy_model, _enemy_model.current_color);
                _enemyCar.InitCar(car);
                _enemyCar.Init();
                
                if (_gameData.replayData != "crash")
                {
                    _enemyCar.GetReplayFrame(0);
                }
            }
        }

       
        //List<RecordParameter> _replayData = new List<RecordParameter>();
        //List<InputParameter> _replayData = new List<InputParameter>();

        

        Vector4 replayFramePos;
        Vector3 replayFrameRot;
        private Coroutine _calcMetersToEnemy = null;

       

        public void Start()
        {
            //TODO Coroutine hide
            Core.Instance.GetService<UIManager>().ShowWindow(UIWindowEnum.SPLASH, false);

            ResetEarnedValues();
            
        

            if (TryesCount > 2)
            {
                _gameWindow.timerText.text = "";
                _playerCar.EnablePlayerControll(false);
                _enemyCar.carBase.EnableEnemyControll(false);
                _gameWindow.Show();
                CalculateResults();
            }
            else
            {
                var enemyName = (_playerManager.PlayerId == _gameData.player1_Id) ?
                  (_gameData.player2_name) : (_gameData.player1_name);
                        //_gameWindow.textInfoWindow("Racing versus " + enemyName);

                _gameWindow.ShowEnemyMeters();
                _gameWindow.ActiveGold();
                _gameWindow.Show();

                _startTimer = Core.Instance.StartCor(StartTimer());
                EventManager._init.Game.TrackEvent.GetEvent.Invoke_Start();
            }


          
           
        }

        IEnumerator StartTimer()
        {
            _audioService.StartGameMusic();

            float seconds = 4;
            _gameWindow.timerText.GetComponent<CanvasGroup>().alpha = 1;

            Rigidbody _prb = null;
            if (_playerCar != null)
            {
                _prb = _playerCar.GetComponent<Rigidbody>();
            }

            Rigidbody _erb = null;
            if (_enemyCar != null)
            {
                _erb = _enemyCar.carBase.GetComponent<Rigidbody>();
            }

            while (seconds > 1)
            {
                _gameWindow.timerText.text = (seconds - (seconds % 1)).ToString();
                if (previousSecondCounter != (seconds - (seconds % 1)) && seconds != 4)
                {
                    previousSecondCounter = (seconds - (seconds % 1));
                    _audioService.RM_PlayOneShot(audioCounterPath);
                }
                yield return new WaitForEndOfFrame();
                seconds -= Time.deltaTime;
                if (_prb != null)
                {
                    _prb.velocity = new Vector3(0, _prb.velocity.y, _prb.velocity.z);
                }

                if (_erb != null)
                {
                    _erb.velocity = new Vector3(0, _erb.velocity.y, _erb.velocity.z);
                }

                _playerCar.transform.position = new Vector3(_playerCar.transform.position.x, _playerCar.transform.position.y, 2);
            }
            IsBuyTries = false;
            EndOfCountdownAtRaceStart();

            _playerCar.EnablePlayerControll(true);
            _enemyCar.carBase.EnableEnemyControll(true);


            if (_gameData.replayData == "crash")
            {
                //_enemyCar.ExploseEnemy();
                _gameWindow.enemyCountMeter.text = "crash";
            }

            else if (_gameData.replayData != null)
            {



               
                _calcMetersToEnemy = Core.Instance.StartCor(calcMetersToEnemy());
            }
            else {
               // _enemyCar.ExploseEnemy();
                _gameWindow.enemyCountMeter.text = "crash";
            }
           

            
        }

        public void Restart()
        {
            EventManager._init.Game.CarEvent.Enemy.GetEvent.Finish -= EnemyFinish;
            isEnemyFinish = false;
            
            if (_calcMetersToEnemy != null)
            Core.Instance.StopCor(_calcMetersToEnemy);
            IsPlayerCrash = true;
           
            TryesCount++;
            _playerCar.EnablePlayerControll(false);
            _enemyCar.carBase.EnableEnemyControll(false);
            _gameWindow.DeActiveJewels();
            ResetEarnedValues();
            if (_gameData.GameType != GameTypeEnum.MultyNew)
            _safePlayerPrefs.ChangeTryse(TryesCount, _gameData.gameId);
            if (TryesCount < 3)
            {
                Destroy();
                _gameManager.Restart(OnTrackLoaded);

            }
            else if (TryesCount == 3)
            {
                //Forced lose
               // _finishWindow.OnBuyTriesClick += BuyTries;
                EventManager._init.Game.CarEvent.Player.GetEvent.Crash -= OnPlayerCrash;
                if (_gameData.IsFinishedEnemy == 1)
                {
                    _gameData.gameResultScore = 0;
                }
                else
                {
                    _gameData.gameResultScore = 2;
                }


                _safePlayerPrefs.SaveEarnedGold(0);
                BuyTriesQuestion();
                if (_moveEnemyCor != null)
                {
                    Core.Instance.StopCor(_moveEnemyCor);
                }
                _moveEnemyCor = null;
                if (_startTimer != null)
                {
                    Core.Instance.StopCor(_startTimer);
                }
                _startTimer = null;
            }
        }

        private void BuyTriesQuestion()
        {
            Core.Instance.StartCor(Wait(1f, () =>
            {
                EventManager._init.Game.PlayerEvent.Invoke_BuyTries();
                EventManager._init.Game.PlayerEvent.CancleBuyTries += CalculateResults;
            }));
        }

        public void OnPlayerCrash()
        {
            //Debug.Log("OnPlayerCrash");
            // -----------------------------------------------------------------
            // заглушка - чтоб если разбились возле финиша и тушка машины на него заехала
            // то не засчитывали победы в заезде
            if(_calcMetersToEnemy != null)
            Core.Instance.StopCor(_calcMetersToEnemy);
            
            IsPlayerCrash = true;
            
            _playerCar.EnablePlayerControll(false);
            EventManager._init.Game.CarEvent.Player.GetEvent.Finish -= OnPlayerFinish;
            //_finishWindow.OnBuyTriesClick += BuyTries;
            _playerCar.ExplosePlayer();
            _gameData.IsFinishedReplay = 0;
            _gameWindow.DeActiveJewels();
            _safePlayerPrefs.SaveEarnedJewels(0);
            
            TryesCount++;
            TryesCount--;
           
            if (_gameData.GameType != GameTypeEnum.MultyNew)
            _safePlayerPrefs.ChangeTryse(TryesCount, _gameData.gameId);
            // -----------------------------------------------------------------

            if (TryesCount < 2)
            {
                Core.Instance.StartCor(Wait(1.5f, _retryWindow.Show));
                
            }
            else
            {
                if (_gameData.IsFinishedEnemy == 1)
                {
                    _gameData.gameResultScore = 0;
                }
                else
                {

                    _gameData.gameResultScore = 2;
                }
                _gameWindow.DeActiveGold();
                Core.Instance.StartCor(Wait(1f, () => {
                    _finishWindow.PriceBuyTryes.text = CountTriesBuy.ToString();
                    _finishWindow.ShowQuestion(()=> { if (!IsBuyTries) { _finishWindow.OnBuyTriesClick -= BuyTries; Core.Instance.StartCor(Wait(0.2f, CalculateResults));  } });
                }));
                
            }

        }

        IEnumerator Wait(float time, Action Function)
        {
            yield return new WaitForSeconds(1.5f);
            Function();
        }

        private void BuyTries()
        {
            //Debug.Log("BuyTries Click");
            if (IsBuyTries) return;
           // _finishWindow.OnBuyTriesClick -= BuyTries;
            //Debug.Log("BuyTries " + CountTriesBuy);
            if (_playerManager.jewels >= CountTriesBuy)
            {
                IsBuyTries = true;
                Core.Instance.GetService<NetworkManager>().BuyAttempt(CountTriesBuy, (msg) =>
                {
                    if (msg != 0)
                    {
                        IsBuyTries = true;
                        //Debug.Log("jewels2 " + _playerManager.jewels + msg);
                        
                       
                        CountTriesBuy++;
                        TryesCount = 2;
                        _finishWindow.HideQuestion();
                        Core.Instance.StopAllCoroutines();
                        _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
                        _playerCar.EnablePlayerControll(false);
                        _enemyCar.carBase.EnableEnemyControll(false);
                        Destroy();
                        _gameData.recordData = "";// ??? do I need it ???  YEs Do It !!!
                        Core.Instance.StartCor(Wait(0.3f, () => { _gameManager.LoadTrack(OnTrackLoaded); }));
                    }
                });
            }
        }

        private void setMetersToEnemy()
        {   
            metersToEnemy = Math.Round((double)(_enemyCar.carBase.transform.position.x - _playerCar.transform.position.x), 1);

            if (metersToEnemy > 0)
            {
                _gameWindow.LeaderEnemyMeter(metersToEnemy.ToString());
            }
            else if (metersToEnemy < 0)
            {
                _gameWindow.LooseEnemyMeter(metersToEnemy.ToString());
            }

        }

        

        IEnumerator calcMetersToEnemy() {
            YieldInstruction _fupd = new WaitForFixedUpdate();
            while (true)
            {
                yield return _fupd;
                setMetersToEnemy();
            }
        }

       

        IEnumerator MovePlayerOnFinish()
        {
            Camera.main.GetComponent<WorkShopCamTest>().enabled = false;
            _playerCar.Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            _playerCar.controller.enabled = true;
            _playerCar.controller.throttleInput = 0f;
            _playerCar.controller.brakeInput = 1f;
            _playerCar.Rigidbody.drag = 0.5f;

            YieldInstruction _wait = new WaitForSeconds(2f);
            yield return _wait;
            if (_playerCar != null)
            {
                _playerCar.Rigidbody.drag = 1.5f;
            }


        }

       

        

        private void FillingStatistic()
        {
            var save = _safePlayerPrefs;
            float Crash = 12;
            if(_gameData.IsFinishedReplay != 1) {
                Crash = _gameWindow._playerCar.transform.position.x;
            }
           // _statisticsService.Create_Replay_data(_gameData.track_id, _gameWindow.triesCount, _playerCar.name, _playerCar.timer, save.GetInt_EarnedGold(), CalculateWinAttemptsGold(), save.GetFloat_ExpForTrick(), save.GetInt_EarnedJewels(), _gameData.IsFinishedReplay, Crash, _playerManager.PlayerId, _playerManager.PlayerDisplayName , _gameData.gameId);

            _statisticsService.UserStatistic.Clear();

           // _statisticsService.CreateUserStatistic(save.GetInt_TrickFlipSum(), save.GetFloat_HorseTime(), save.GetFloat_90Time(), save.GetFloat_TrickAirTime(), save.GetMaxFlip, save.GetMaxBalance, save.GetMaxBlunt, save.GetMaxInAir, save.GetFloat_ExpForTrick(), _gameData);

        }

        public void OnPlayerFinish()
        {


            if (_calcMetersToEnemy != null)
                Core.Instance.StopCor(_calcMetersToEnemy);
            // -----------------------------------------------------------------
            // заглушка - чтоб когда дошел до финица и взорвался
            // не показывалось окно "RetryWindow", но показывался взрыв
            EventManager._init.Game.CarEvent.Player.GetEvent.Crash -= OnPlayerCrash;
            EventManager._init.Game.CarEvent.Player.GetEvent.Crash += _playerCar.ExplosePlayer;
            // -----------------------------------------------------------------


            EventManager._init.Game.CarEvent.Player.GetEvent.Finish -= OnPlayerFinish;
            _playerCar.EnablePlayerControll(false);
            _enemyCar.carBase.EnableEnemyControll(false);

            _movePlayerOnFinish = Core.Instance.StartCor(MovePlayerOnFinish());

            //        if (_enemyCar != null)
            //		{
            //            //_enemyCar.OnFinish -= OnEnemyFinish;
            //            //_enemyCar.StopEnemyRigidbody();
            //        }

            if (!isEnemyFinish)
            {
                _gameData.gameResultScore = 1;
            }
            else
            {
                _gameData.gameResultScore = 0;
            }
            _gameData.IsFinishedReplay = 1;
            _finishWindow.SetValue_IsFinish(1);

            CalculateResults();
        }

        void OnPlayerFinishClick()
        {
           
            _finishWindow.Hide();

           

           

            _gameManager.Finish();
        }

        public void CalculateResults()
        {
            // STATISTIC
            _gameData.IsDoneReplay = 1;
            
            
            _audioService.StopGameMusic();
            
            var enemyName = (_playerManager.PlayerId == _gameData.player1_Id) ?
                (_gameData.player2_name) : (_gameData.player1_name);

            var playerScore = (_playerManager.PlayerId == _gameData.player1_Id) ?
                (_gameData.player1_score) : (_gameData.player2_score);

            var enemyScore = (_playerManager.PlayerId == _gameData.player1_Id) ?
                (_gameData.player2_score) : (_gameData.player1_score);


            if (_gameData.gameResultScore == 1)
            {
                FinishWin(playerScore, enemyScore, enemyName);

                _statisticsService.IsWin = 1;
            }
            else if (_gameData.gameResultScore == 0)
            {
                FinishLosing(playerScore, enemyScore, enemyName);
                _statisticsService.IsLoss = 1;
            }
            else
            {
                FinishDraw(playerScore, enemyScore, enemyName);
                _statisticsService.IsDraw = 1;
            }

            FillingStatistic();

            _finishWindow.Show();
        }

        public void Destroy()
        {
            Core.Instance.UnMute();
            _audioService.StopGameMusic();

            _gameWindow.HideEnemyMeters();

            _finishWindow.OnFinishClick -= OnPlayerFinishClick;
            EventManager._init.Game.MenuButton.Restart -= Restart;
            EventManager._init.Game.MenuButton.TryAgain -= Restart;
            EventManager._init.Game.CarEvent.Player.GetEvent.Finish -= OnPlayerFinish;
            EventManager._init.Game.CarEvent.Player.GetEvent.Crash -= OnPlayerCrash;


            EventManager._init.Game.MenuButton.Resume -= Core.Instance.UnMute;

            EventManager._init.Game.CarEvent.Player.GetEvent.Crash -= _playerCar.ExplosePlayer;

            if (_moveEnemyCor != null)
            {
                Core.Instance.StopCor(_moveEnemyCor);
            }
            _moveEnemyCor = null;

            if (_startTimer != null)
            {
                Core.Instance.StopCor(_startTimer);
            }
            _startTimer = null;

            _playerManager.PlayerCar = null;
            if(_playerCar != null)
            _playerCar.Destroy();

            if (_enemyCar != null)
            {
                _enemyCar.carBase.Destroy();
            }
        }

        private void FinishWin(int playerScore, int enemyScore, string enemyName)
        {
            _finishWindow.SetValue_PlayerScore(playerScore + 1);
            _finishWindow.SetValue_EnemyScore(enemyScore);
            _finishWindow.SetValue_EnemyName(enemyName);
            _finishWindow.SetValue_RaceResult(RaceResultEnum.WIN);

            int bonusGold = CalculateWinAttemptsGold();
            int bonusXP = _finishWindow.XPForStars(_gameWindow);
            _finishWindow.SetValue_GoldForWinAttempts(bonusGold);
            _finishWindow.SetValue_XPForWinAttempts(bonusXP);

            //
            CalculateEarnedExp();
            CalculatePlayerGold(bonusGold);
            CalculatePlayerXP(bonusXP);
            //CalculateEarnedGold(0);     // !!!
                  // !!!
        }

        private void FinishLosing(int playerScore, int enemyScore, string enemyName)
        {
            _finishWindow.SetValue_PlayerScore(playerScore);
            _finishWindow.SetValue_EnemyScore(enemyScore + 1);
            _finishWindow.SetValue_EnemyName(enemyName);
            _finishWindow.SetValue_RaceResult(RaceResultEnum.LOSING);

            int bonusGold = 0;
            int bonusXP = 0;
            _finishWindow.SetValue_GoldForWinAttempts(bonusGold);
            _finishWindow.SetValue_XPForWinAttempts(bonusXP);

            //
            CalculateEarnedExp();
            CalculatePlayerGold(bonusGold);
            CalculatePlayerXP(bonusXP);
            // CalculateEarnedGold(0);     // !!!
                 // !!!
        }

        private void FinishDraw(int playerScore, int enemyScore, string enemyName)
        {
            _finishWindow.SetValue_PlayerScore(playerScore);
            _finishWindow.SetValue_EnemyScore(enemyScore);
            _finishWindow.SetValue_EnemyName(enemyName);
            _finishWindow.SetValue_RaceResult(RaceResultEnum.DRAW);

            int bonusGold = 0;
            int bonusXP = 0;
            _finishWindow.SetValue_GoldForWinAttempts(bonusGold);
            _finishWindow.SetValue_XPForWinAttempts(bonusXP);

            //
            CalculateEarnedExp();
            CalculatePlayerGold(bonusGold);
            CalculatePlayerXP(bonusXP);
            // CalculateEarnedGold(0);     // !!!
               // !!!
        }

        private int CalculateWinAttemptsGold()
        {
            int bonusGold = 0;

            int triesCount = _gameData.GetTryes;
            
            switch (triesCount)
            {
                case 3:
                    bonusGold = 60 + _playerManager.level;
                    break;
                case 2:
                    bonusGold = 30 + _playerManager.level;
                    break;
                case 1:
                    bonusGold = 15 + _playerManager.level;
                    break;
                default:
                    bonusGold = 0;
                    break;
            }

            return bonusGold;
        }

        private void CalculatePlayerGold(int bonusGold)
        {

            _safePlayerPrefs.AddPlayerGold(_safePlayerPrefs.GetInt_EarnedGold() + bonusGold);
        }

        private void CalculatePlayerXP(int bonusXP)
        {
            //Debug.Log("GetFloat_EarnedExp " + _safePlayerPrefs.GetFloat_EarnedExp() + "  bonusXP  " + bonusXP);
            _safePlayerPrefs.SavePlayerExp(_safePlayerPrefs.GetFloat_EarnedExp() + bonusXP);
        }

        //  private void CalculateEarnedGold(int bonus)
        //  {
        // TODO --- узнать как будет рассчитываться !

        //      _safePlayerPrefs.AddEarnedGold(
        //          _safePlayerPrefs.GetInt_PlayerGold()
        //         + bonus);
        //  }

        private void CalculateEarnedExp()
        {
            // TODO --- узнать как будет рассчитываться !
           // Debug.Log(" CalculateEarnedExp : " + _safePlayerPrefs.GetFloat_ExpForTrick());

            _safePlayerPrefs.SaveEarnedExp(_safePlayerPrefs.GetFloat_ExpForTrick());
        }

        private void EndOfCountdownAtRaceStart()
        {
            _gameWindow.timerText.GetComponent<CanvasGroup>().alpha = 0;
            
        }

        //      private void GetReplayFrameForVelocity(int frame)
        //{
        //          _enemyCar.Rigidbody.velocity = _replayData[frame].VelocityParam;
        //          _enemyCar.Rigidbody.angularVelocity = _replayData[frame].AngularVelocityParam;

        //      }

        //private void GetReplayFrameForInput(int frame)
        //{


        //    _enemyCar.controller.steerInput = _replayData[frame].steerInput;
        //    _enemyCar.controller.throttleInput = _replayData[frame].throttleInput;
        //    _enemyCar.controller.brakeInput = _replayData[frame].brakeInput;
        //    _enemyCar.controller.handbrakeInput = _replayData[frame].handbrakeInput;
        //    _enemyCar.nitro.nitroPower = _replayData[frame].NitroPower;
        //    _enemyCar.airController.rotateAndroid = _replayData[frame].RotateAndroid;
        //    _enemyCar.nitro.Sin = _replayData[frame].Sin;
        //    _enemyCar.nitro.Cos = _replayData[frame].Cos;

        //    Debug.Log("steerInput   " + _replayData[frame].steerInput);
        //    Debug.Log("throttleInput   " + _replayData[frame].throttleInput);
        //    Debug.Log("brakeInput    " + _replayData[frame].brakeInput);
        //    Debug.Log("handbrakeInput   " + _replayData[frame].handbrakeInput);


        //}        


        //private void GetReplayDataForVelocity()
        //{
        //    _replayData.Clear();



        //    var rawData = Convert.FromBase64String(_gameData.replayData);


        //    //Parse
        //    var offset = 0;
        //    int length = rawData.Length / 36;
        //    for (int i = 0; i < length; i++)
        //    {
        //        offset = i * 36;
        //        _replayData.Add( new RecordParameter (new Vector3 (BitConverter.ToSingle(rawData, offset), BitConverter.ToSingle(rawData, offset + 4),
        //         BitConverter.ToSingle(rawData, offset + 8)), new Vector3 (BitConverter.ToSingle(rawData, offset + 12),
        //         BitConverter.ToSingle(rawData, offset + 16), BitConverter.ToSingle(rawData, offset + 20)),
        //         new Vector3 (BitConverter.ToSingle(rawData, offset + 24), BitConverter.ToSingle(rawData, offset + 28),
        //         BitConverter.ToSingle(rawData, offset + 32))));
        //    }
        //}

        //      private void GetReplayDataForInput()
        //{
        //	_replayData.Clear();



        //	var rawData = Convert.FromBase64String(_gameData.replayData);


        //	//Parse
        //	var offset = 0;
        //	int length = rawData.Length / 40;
        //	for (int i = 0; i < length; i++)
        //	{
        //              offset = i * 40;
        //              _replayData.Add(new InputParameter(BitConverter.ToSingle(rawData, offset), BitConverter.ToSingle(rawData, offset + 4),
        //               BitConverter.ToSingle(rawData, offset + 8), BitConverter.ToSingle(rawData, offset + 12), BitConverter.ToSingle(rawData, offset + 16),
        //               BitConverter.ToSingle(rawData, offset + 20), BitConverter.ToSingle(rawData, offset + 28), BitConverter.ToSingle(rawData, offset + 36)));
        //          }
        //      }
    }
}