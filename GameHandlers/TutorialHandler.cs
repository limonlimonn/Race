using UnityEngine;
using HCR;
using HCR.Enums;
using HCR.Interfaces;
using System;
using UnityEngine.Assertions;
using HCR.Event;

namespace HCR.Gameplay.Tutorial
{
    public class TutorialHandler : ABaseRecordHandler, IGameHandler
    {

        private SafePlayerPrefs _safePlayerPrefs;

        private GameWindowTutorial _gameWindow;
        private PauseWindow _pauseWindow;
        private RetryWindow _retryWindow;
        private FinishWindow _finishWindow;
        public GameObject StartPosition = new GameObject();
        
        private TrigersTutorial _trigersTutorial;
        bool IsTryAgain = false;
        public new int TryesCount { get; protected set; }

        public void Destroy()
        {

            _audioService.StopGameMusic();

            UnsubscribeOnEvents();
            SafeStopCoroutine_StartTimer();

            _playerManager.PlayerCar = null;

           // if (_playerManager.tutorialStep != 2)
           /// {
                _playerCar.Destroy();
                
           /// }
        }

        public void Init(GameData data)
        {



            //pre-conditions
            Assert.AreNotEqual(null, data);

            _carConstructor = Core.Instance.GetService<CarConstructor>();
            Assert.AreNotEqual(null, _carConstructor);

            _gameManager = Core.Instance.GetService<GameManager>();
            Assert.AreNotEqual(null, _gameManager);

            _playerManager = Core.Instance.GetService<PlayerManager>();
            Assert.AreNotEqual(null, _playerManager);

            _safePlayerPrefs = Core.Instance.GetService<SafePlayerPrefs>();
            Assert.AreNotEqual(null, _safePlayerPrefs);

            //
            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);

            _gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_TUTORIAL) as GameWindowTutorial;
            Assert.AreNotEqual(null, _gameWindow);

            _pauseWindow = _uiManager.GetWindow(UIWindowEnum.PAUSE_TUTORIAL) as PauseWindow;
            Assert.AreNotEqual(null, _pauseWindow);

            _retryWindow = _uiManager.GetWindow(UIWindowEnum.RETRY_TUTORIAL) as RetryWindow;
            Assert.AreNotEqual(null, _retryWindow);

            _finishWindow = _uiManager.GetWindow(UIWindowEnum.FINISH_TUTORIAL) as FinishWindow;
            Assert.AreNotEqual(null, _finishWindow);

            _trigersTutorial = _uiManager.GetWindow(UIWindowEnum.TRIGERS_TUTORIAL) as TrigersTutorial;
            Assert.AreNotEqual(null, _trigersTutorial);

            _audioService = Core.Instance.GetService<AudioService>();
            Assert.AreNotEqual(null, _audioService);

            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);

            //
            _gameData = data;
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
            _gameManager.LoadTrack(OnTrackLoaded);

           
        }

        private void OnTrackLoaded()
        {



            _playerCar = _carConstructor.CreatePlayerCar(_playerManager.selectedCar, _playerManager.selectedCar.current_color);
            Assert.AreNotEqual(null, _playerCar);

           // _playerCar.SetColorInGame(_playerManager.selectedCar.current_color);


            //TODO Move to constants

            
            _playerCar.transform.tag = "Player";
            _playerCar.Transform.position = new Vector3(0f, 3f, 2f);
            //_playerCar.SetValue_RaceTimer(_gameWindow._textRaceTimer);

            _playerManager.PlayerCar = _playerCar;
          
            
            _gameWindow.SetValue_PlayerCar();
            SubscribeOnEvents();

            _gameWindow._playerCar = _playerManager.PlayerCar;
            _gameWindow.Show();
            EventManager._init.Game.CarEvent.Player.GetEvent.Finish += OnPlayerFinish;

            EventManager._init.Game.CarEvent.Player.GetEvent.Crash += OnPlayerCrash;

            if (IsTryAgain)
            {
                var s = _trigersTutorial.GetStartPosition();
                _playerCar.Transform.position = new Vector3(s[0], s[1], s[2]);
            }
            Start();
        }

        public void OnPlayerCrash()
        {
            


            Zaglushka_CrashAndThenFinish();
            //CheckWhatWindowShowOnCrash();
            ShowWindow_Retry();
        }

        public void OnPlayerFinish()
        {
            
            Zaglushka_FinishAndThenCrash();

            
            _playerCar.EnablePlayerControll(false);


            _coroutineMovePlayerOnFinish = Core.Instance.StartCor(MovePlayerOnFinish());
            EventManager._init.Game.CarEvent.Player.GetEvent.Finish -= OnPlayerFinish;

            _gameData.recordData = "";

            _audioService.StopGameMusic();

            ShowWindow_WinFinish();
        }
       

        public void Restart()
        {
             IsTryAgain = false;
            RestartWork_HaveTries();
        }

        public void TryAgain()
        {
            IsTryAgain = true;   
            RestartWork_HaveTries();
        }

        public void EnyChoiseCar()
        { 
            IsTryAgain = false;
            RestartWork_ChoiseCar();
        }

        public void RestartWork_ChoiseCar()
        {
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
            _gameData.recordData = "";
            Destroy();
            _uiManager.ShowWindow(UIWindowEnum.CHOOSE_CAR_TUTORIAL, true);
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);

        }


        // Use this for initialization
        public void Start()
        {

            if(!IsTryAgain)
            _coroutineStartTimer = Core.Instance.StartCor(StartTimer(_gameWindow));
            else {
                _gameWindow.Get_TimerText().GetComponent<CanvasGroup>().alpha = 0;
                _playerCar.EnablePlayerControll(true);
                _gameWindow.Get_PauseButton().interactable = true;
            }
           
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
           
        }

        // Update is called once per frame

        protected override void ShowWindow_Retry()
        {
         
            _retryWindow.Show();
        }

        private void Zaglushka_FinishAndThenCrash()
        {

            // заглушка - чтоб когда дошел до финица и взорвался
            // не показывалось окно "RetryWindow", но показывался взрыв

            EventManager._init.Game.CarEvent.Player.GetEvent.Crash -= OnPlayerCrash;
            EventManager._init.Game.CarEvent.Player.GetEvent.Crash += _playerCar.ExplosePlayer;
        }

        protected override void ShowWindow_CrashFinish()
        {
           
          //
        }

        protected override void RestartWork_HaveTries()
        {
            
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
            Destroy();
            _gameData.recordData = "";                  // ??? do I need it ???  YEs Do It !!!
            _gameManager.LoadTrack(OnTrackLoaded);
            _audioService.StartGameMusic();
        }

        protected override void RestartWork_HaveNoTriesLeft()
        {
            

            SafeStopCoroutine_StartTimer();
            ShowWindow_CrashFinish();
        }

        protected override void SafeStopCoroutine_Record()
        {

        }

        private void SubscribeOnEvents()
        {
            //_trigersTutorial.SetPosition += () => { StartPosition.transform.position = _trigersTutorial.GetStartPosition(); };
            _retryWindow.OnTryAgainClick += TryAgain;
            _retryWindow.OnCoiseAnyCar += EnyChoiseCar;
            _pauseWindow.OnCoiseAnyCar += EnyChoiseCar;

            _pauseWindow.OnRestartClick += Restart;


            EventManager._init.Game.CarEvent.Player.GetEvent.Finish += OnPlayerFinish;

            EventManager._init.Game.CarEvent.Player.GetEvent.Crash += OnPlayerCrash;
        }
        

        private void OnStartTutorial()
        {

         
        }

        private void ShowWindow_WinFinish()
        {

            _audioService.StopGameMusic();
            _finishWindow.SetValue_RaceResult(RaceResultEnum.WIN);
            _finishWindow.Show();
        }


    

        private void UnsubscribeOnEvents()
        {

            _retryWindow.OnTryAgainClick -= TryAgain;
            _retryWindow.OnCoiseAnyCar -= EnyChoiseCar;
            _pauseWindow.OnCoiseAnyCar -= EnyChoiseCar;
            _pauseWindow.OnRestartClick -= Restart;

            EventManager._init.Game.CarEvent.Player.GetEvent.Finish -= OnPlayerFinish;

            EventManager._init.Game.CarEvent.Player.GetEvent.Crash -= OnPlayerCrash;

            //
            EventManager._init.Game.CarEvent.Player.GetEvent.Crash -= _playerCar.ExplosePlayer;
        }

        private void Zaglushka_CrashAndThenFinish()
        {

            // заглушка - чтоб если разбились возле финиша и тушка машины на него заехала
            // то не засчитывали победы в заезде

            EventManager._init.Game.CarEvent.Player.GetEvent.Finish -= OnPlayerFinish;
            _playerCar.ExplosePlayer();
        }

        public void CalculateResults()
        {
            //throw new NotImplementedException();
        }


        private int CalculateWinAttemptsGold()
        {
            int bonusGold = 0;

            int triesCount = _gameWindow.triesCount;
            switch (triesCount)
            {
                case 3:
                    bonusGold = 60;
                    break;
                case 2:
                    bonusGold = 30;
                    break;
                case 1:
                    bonusGold = 15;
                    break;
                default:
                    #region DEBUG
#if UNITY_EDITOR
                    Debug.Log("[ERROR] triesCount (must be 1 or 2 or 3), but it is = " + triesCount);
#endif
                    #endregion
                    break;
            }

            return bonusGold;
        }


        private void CalculateEarnedGold(int bonusGold)
        {
            _safePlayerPrefs.AddEarnedGold(
                _safePlayerPrefs.GetInt_PlayerGold()
                + bonusGold);
        }


        private void CalculateEarnedExp(float bonus)
        {
            // TODO --- узнать как будет рассчитываться !

            _safePlayerPrefs.AddEarnedExp(
                _safePlayerPrefs.GetFloat_PlayerExp()
                + bonus);
        }


        #region TRIGGERS

        public void StartTrigger()
        {


        }

        #endregion

    }

}
