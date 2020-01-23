using UnityEngine;
using UnityEngine.Assertions;

using HCR.Gameplay.AsyncMultiplayer;
using HCR.GlobalWindow.MainMenu;
using HCR.Interfaces;
using HCR.Enums;
using HCR.Event;

namespace HCR
{
	public class GameAsyncState : IState
	{
        // FIELDS


        
		// dependences
		private GameManager _gameManager;
		private StatesManager _statesManager;

		private UIManager _uiManager;
		private MainScreenStateManager _mainScreenStateManager;
		private GameWindowAsyncMultiplayer _gameWindow;
		private PauseWindow _pauseWindow;
		private Header _header;
        protected AudioService _audioService;


        // I_STATES

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

			_gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_ASYNC) as GameWindowAsyncMultiplayer;
			Assert.AreNotEqual(null, _gameWindow);

			_pauseWindow = _uiManager.GetWindow(UIWindowEnum.PAUSE_ASYNC) as PauseWindow;
			Assert.AreNotEqual(null, _pauseWindow);

            _audioService = Core.Instance.GetService<AudioService>();
            Assert.AreNotEqual(null, _audioService);
            //

            Debug.Log("Enable GAME_ASYNC");

            EventManager._init.Game.MenuButton.Resume += Resume;
            _pauseWindow.OnMenuClick += Menu;

            _gameManager.CreateGameHandler();

            //
            
			Assert.AreNotEqual(null, _gameWindow._objBackground);

            _gameWindow._objBackground.SetActive(false);

            // -------------------------------------------
            //
            
			// -------------------------------------------

			//
			_uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
			_mainScreenStateManager.SwitchState(MainScreenStatesEnum.GAME_ASYNC);
		}

		public void Disable()
		{
            Debug.Log("Disable GAME_ASYNC");
			_gameWindow._objBackground.SetActive(true);
			_uiManager.ShowWindow(UIWindowEnum.SPLASH, true);

			_gameWindow.Hide();
			_pauseWindow.Hide();

            // -------------------------------------------
            //

            EventManager._init.Game.MenuButton.Resume -= Resume;
			_pauseWindow.OnMenuClick -= Menu;
			// -------------------------------------------

			_gameManager.Destroy();
		}



		// INTERFACES


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
            //_gameWindow.ResetInfoWindowTime();
            _audioService.StopGameMusic();
            _statesManager.SwitchState(StatesEnum.MAIN_MULTIPLAYER_WINDOW);

			Core.Instance.StartCor(
				_gameManager.LoadGameScene("EntryPoint",
					() => { _uiManager.ShowWindow(UIWindowEnum.SPLASH, false); }
				) );
		}



	}
}