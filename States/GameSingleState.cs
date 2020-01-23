using UnityEngine;
using UnityEngine.Assertions;

using HCR.GlobalWindow.MainMenu;
using HCR.Interfaces;
using HCR.Enums;

namespace HCR.Gameplay.Singleplayer
{
	public class GameSingleState : IState
	{
		// FIELDS

		private GameObject _objBackground;

		// dependences
		private GameManager _gameManager;
		private StatesManager _statesManager;

		private UIManager _uiManager;
		private MainScreenStateManager _mainScreenStateManager;
		private GameWindowSingleplayer _gameWindowSingleplayer;
		private PauseWindow _pauseWindow;
		private Header _header;



		// I_STATE

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

			_gameWindowSingleplayer = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_SINGLE) as GameWindowSingleplayer;
			Assert.AreNotEqual(null, _gameWindowSingleplayer);

			_pauseWindow = _uiManager.GetWindow(UIWindowEnum.PAUSE_SINGLE) as PauseWindow;
			Assert.AreNotEqual(null, _pauseWindow);

			//
			_gameManager.CreateGameHandler();

			//
			_objBackground = GameObject.FindGameObjectWithTag("BG");
			Assert.AreNotEqual(null, _objBackground);

			_objBackground.SetActive(false);

			// -------------------------------------------
			//
			_gameWindowSingleplayer.OnPauseClick += Pause;
			//_pauseWindow.OnResumeClick += Resume;
			///_pauseWindow.OnMenuClick += Menu;
			// -------------------------------------------

			_uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
			_mainScreenStateManager.SwitchState(MainScreenStatesEnum.GAME_SINGLE);
		}

		public void Disable()
		{
			_objBackground.SetActive(true);
			_uiManager.ShowWindow(UIWindowEnum.SPLASH, true);

			_gameWindowSingleplayer.Hide();
			_pauseWindow.Hide();

			// -------------------------------------------
			//
			_gameWindowSingleplayer.OnPauseClick -= Pause;
			//_pauseWindow.OnResumeClick -= Resume;
			//_pauseWindow.OnMenuClick -= Menu;
			// -------------------------------------------

			_gameManager.Destroy();
		}



	 	// INTERFACES

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
			_statesManager.SwitchState(StatesEnum.MAIN_MULTIPLAYER_WINDOW);

			Core.Instance.StartCor(
				_gameManager.LoadGameScene("EntryPoint",
					() => { _uiManager.ShowWindow(UIWindowEnum.SPLASH, false); }
				) );
		}



	}
}