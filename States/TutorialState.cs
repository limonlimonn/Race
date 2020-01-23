using UnityEngine;
using UnityEngine.Assertions;
using HCR.Interfaces;
using HCR.Enums;
using HCR.Gameplay.Tutorial;
using System;
using System.Collections;

namespace HCR.Gameplay.Tutorial
{
    public class TutorialState : IState
    {


        private Core _core;
        private NetworkManager _nm;
        private PlayerManager _playerManager;
        private UIManager _uim;

        private GameObject _objBackground;
        private GameManager _gameManager;
        private PauseWindow _pauseWindow;
        private GameWindowTutorial _gameWindowTutorial;
        private ChooseCarTutorialWindow _chooseCarTutorial;
        private FinishWindow _finishWindow;
        private MainScreenStateManager _mainScreenStateManager;
        private StatesManager _statesManager;
        private UIManager _uiManager;
        private TrigersTutorial _trigersTutorial;



        public void Disable()
        {

          
           
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);

       
            _pauseWindow.Hide();

            // -------------------------------------------
            //
            
            _gameWindowTutorial.OnPauseClick -= Pause;
            _pauseWindow.OnResumeClick -= Resume;
            _pauseWindow.OnMenuClick -= Menu;

            _gameWindowTutorial.OnPauseClick -= Core.Instance.Mute;
            _pauseWindow.OnResumeClick -= Core.Instance.UnMute;
            _pauseWindow.OnMenuClick -= Core.Instance.UnMute;
            _pauseWindow.OnCoiseAnyCar -= Core.Instance.UnMute;
            _pauseWindow.OnRestartClick -= Core.Instance.UnMute;

            // -------------------------------------------




        }

      

        public void Enable()
        {


            _gameManager = Core.Instance.GetService<GameManager>();
            Assert.AreNotEqual(null, _gameManager);

            _statesManager = Core.Instance.GetService<StatesManager>();
            Assert.AreNotEqual(null, _statesManager);

            //
            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);

            _mainScreenStateManager = _uiManager.Get_MainScreenStateManager();
            Assert.AreNotEqual(null, _mainScreenStateManager);

            _playerManager = Core.Instance.GetService<PlayerManager>();
            Assert.AreNotEqual(null, _playerManager);

            _gameWindowTutorial = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_TUTORIAL) as GameWindowTutorial;
              Assert.AreNotEqual(null, _gameWindowTutorial);
            
            _pauseWindow = _uiManager.GetWindow(UIWindowEnum.PAUSE_TUTORIAL) as PauseWindow;
              Assert.AreNotEqual(null, _pauseWindow);

              _chooseCarTutorial = _uiManager.GetWindow(UIWindowEnum.CHOOSE_CAR_TUTORIAL) as ChooseCarTutorialWindow;
              Assert.AreNotEqual(null, _chooseCarTutorial);

            _finishWindow = _uiManager.GetWindow(UIWindowEnum.FINISH_TUTORIAL) as FinishWindow;
            Assert.AreNotEqual(null, _finishWindow);

            _trigersTutorial = _uiManager.GetWindow(UIWindowEnum.TRIGERS_TUTORIAL) as TrigersTutorial;
            Assert.AreNotEqual(null, _trigersTutorial);
            //_chooseCarTutorial.Show();

            // -------------------------------------------
            //
            _mainScreenStateManager.SwitchState(MainScreenStatesEnum.GAME_TUTORIAL);

            
            _gameWindowTutorial.OnPauseClick += Pause;
            _pauseWindow.OnResumeClick += Resume;

            _gameWindowTutorial.OnPauseClick += Core.Instance.Mute;
            _pauseWindow.OnResumeClick += Core.Instance.UnMute;
            _pauseWindow.OnMenuClick += Core.Instance.UnMute;
            _pauseWindow.OnCoiseAnyCar += Core.Instance.UnMute;
            _pauseWindow.OnRestartClick += Core.Instance.UnMute;

            _chooseCarTutorial.OnClickBuggy += clickBuggy;
            _chooseCarTutorial.OnClickJeep += clickJeep;
            _chooseCarTutorial.OnClickRally += clickRally;


            _finishWindow.NExtTrack += NextTrack;
            _pauseWindow.OnMenuClick += Menu;
            // // -------------------------------------------

            // _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);


            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);

            
            _uiManager.ShowWindow(UIWindowEnum.HEADER, false);

            
            if (_playerManager.GetTutorialStep() < 3)
            {
                _trigersTutorial.Show();
                _chooseCarTutorial.Show();
            }
            else if(_playerManager.GetTutorialStep() == 3)
            {
               
                Menu();
            }


        }
        // Button Clik Choise Car

        //----
        public void NextTrack()
        {
            // Destroy();
            
            _playerManager.SetTutorialStep(2);
         
            _gameManager.Destroy();


            _gameManager.CreateTutorialGame(100);
          
            
             //_gameManager.LoadTrack(OnTrackLoaded);
        }
        //----

        private void DownloadChooseCar(int type, int color)
        {
           
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
            _chooseCarTutorial.Hide();


            //_gameWindowTutorial.Show();

          

            CarModel curentCar = null;

            curentCar = new CarModel() {
                type = type,
                level = 0,
                TutorialColor = color

            };
            _playerManager.SetCurrentCur(curentCar);

            





            GoToTutorial();
        }


        // INTERFACES
       public void GoToTutorial() {

            if (_playerManager.GetTutorialStep() == 1)
            {
                _gameManager.CreateTutorialGame(99);

            }else if(_playerManager.GetTutorialStep() == 2)
            {
               
                _gameManager.CreateTutorialGame(100);
           
            }
            else 
            {
                
                Debug.LogError("_playerManager.tutorial > 1 //" + _playerManager.GetTutorialStep());
            }
            



        }



        public void clickBuggy() {

            DownloadChooseCar(0,1);
        }

        public void clickJeep()
        {

            DownloadChooseCar(2,7);
        }

        public void clickRally()
        {

            DownloadChooseCar(1,8);

        }


        public void Pause()
        {
            _pauseWindow.Show();
        }

        public void Resume()
        {
            _pauseWindow.Hide();
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void Menu()
        {
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
            _playerManager.SetTutorialStep(3);
            _playerManager.PlayerCar = null;
            _playerManager.currentCar = null;
            _playerManager.selectedCar = null;
            _playerManager.playerCars.Clear();



             _statesManager.SwitchState(StatesEnum.MAIN_MULTIPLAYER_WINDOW);
           
            Core.Instance.StartCor(
                _gameManager.LoadGameScene("EntryPoint",
                    () => { Core.Instance.StartCor(Wait(1f,() => { _uiManager.ShowWindow(UIWindowEnum.SPLASH, false); })); }
                ));

            _uiManager.ShowWindow(UIWindowEnum.MENUTUTORIALWINDOW, true);
            _uiManager.ShowWindow(UIWindowEnum.FIRSTTUTORIALWINDOW, true);

        }

        private IEnumerator Wait(float time ,Action OnComplete)
        {
            yield return new WaitForSeconds(time);
            OnComplete();
        }

      
       


    }
}

