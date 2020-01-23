using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

using HCR.Enums;
using HCR.GlobalWindow.MainMenu;
using HCR.Interfaces;

using _Tutorial =  HCR.Gameplay.Tutorial;
using _AsyncMulty = HCR.Gameplay.AsyncMultiplayer;
using _Single = HCR.Gameplay.Singleplayer;

namespace HCR
{
	/// <summary>
	/// Класс - стей-менеджер "Главного Меню"
	/// (для переключения с него на "Настройки", "Гараж", "Дерево Апгрейдов" и т.д.)
	/// </summary>

	public class MainScreenStateManager : MonoBehaviour
	{
		// FIELDS

		private MainScreenStatesEnum _currentState;

		private Dictionary<MainScreenStatesEnum, IUIWindow> _dictionaryStates;
		private List<IUIWindow> _windows;

		// dependences
		private UIManager _uiManager;

		private MultiplayerWindow _multiplayerWindow;
		private ChooseCarWindow _chooseCarWindow;
        private StatisticPanel _statisticPanel;
        private ScorePanel _scorePanel;

        private TreeWindow _treeWindow;
		private SettingsWindow _settingsWindow;
		private ChooseTrackWindow _chooseTrackWindow;
        private EventWindow _eventWindow;

		private _AsyncMulty.GameWindowAsyncMultiplayer _gameWindowAsync;
		private _Single.GameWindowSingleplayer _gameWindowSingleplayer;
        private _Tutorial.GameWindowTutorial _gameWindowTutorial;
        // I_SERVICE

        public void Init()
		{

			InitVariables();
			InitDictionaryStates();
			_windows = _dictionaryStates.Values.ToList();

			
		}

		public MainScreenStatesEnum getCurrentState()
		{
			return _currentState;
		}

		// INTERFACES

		public void SwitchState(MainScreenStatesEnum state)
		{
            Core.Instance.UnMute();
			if (_currentState == state) {
				return; }

			IUIWindow targetWindow = _dictionaryStates[state];
			HideAllWindowsExceptTarget(state);
            
            targetWindow.Show();
            
            _currentState = state;
            Debug.Log("_currentState  " + _currentState.ToString());
		}



		// METHODS

		private void InitVariables()
		{
			_uiManager = Core.Instance.GetService<UIManager>();
			Assert.AreNotEqual(null, _uiManager);

			_multiplayerWindow = _uiManager.GetWindow(UIWindowEnum.MAIN_MULTIPLAYER) as MultiplayerWindow;
			Assert.AreNotEqual(null, _multiplayerWindow);

			_chooseCarWindow = _uiManager.GetWindow(UIWindowEnum.CHOOSE_CAR) as ChooseCarWindow;
			Assert.AreNotEqual(null, _chooseCarWindow);

			_treeWindow = _uiManager.GetWindow(UIWindowEnum.TREE_UPGRADE) as TreeWindow;
			Assert.AreNotEqual(null, _treeWindow);

			_settingsWindow = _uiManager.GetWindow(UIWindowEnum.SETTINGS) as SettingsWindow;
			Assert.AreNotEqual(null, _settingsWindow);

			_chooseTrackWindow = _uiManager.GetWindow(UIWindowEnum.CHOOSE_TRACK) as ChooseTrackWindow;
			Assert.AreNotEqual(null, _chooseTrackWindow);

			//
			_gameWindowAsync = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_ASYNC) as _AsyncMulty.GameWindowAsyncMultiplayer;
			Assert.AreNotEqual(null, _gameWindowAsync);

			_gameWindowSingleplayer = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_SINGLE) as _Single.GameWindowSingleplayer;
			Assert.AreNotEqual(null, _gameWindowSingleplayer);

            // tutorial 

            _gameWindowTutorial = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_TUTORIAL) as _Tutorial.GameWindowTutorial;
            Assert.AreNotEqual(null, _gameWindowTutorial);

            _statisticPanel = _uiManager.GetWindow(UIWindowEnum.STATISTIC) as StatisticPanel;
            Assert.AreNotEqual(null, _statisticPanel);

            _scorePanel = _uiManager.GetWindow(UIWindowEnum.SCORE) as ScorePanel;
            Assert.AreNotEqual(null, _scorePanel);

          _eventWindow = _uiManager.GetWindow(UIWindowEnum.EVENT) as EventWindow;
           Assert.AreNotEqual(null, _eventWindow);
        }

		private void InitDictionaryStates()
		{
			_dictionaryStates = new Dictionary<MainScreenStatesEnum, IUIWindow>();

			_dictionaryStates.Add( MainScreenStatesEnum.MAIN_MULTIPLAYER, 	_multiplayerWindow			);
			_dictionaryStates.Add( MainScreenStatesEnum.CHOOSE_CAR, 		_chooseCarWindow			);
            
			_dictionaryStates.Add( MainScreenStatesEnum.TREE, 				_treeWindow					);
			_dictionaryStates.Add( MainScreenStatesEnum.SETTINGS, 			_settingsWindow				);
            _dictionaryStates.Add( MainScreenStatesEnum.STATISTIC,          _statisticPanel             );
            _dictionaryStates.Add( MainScreenStatesEnum.SCORE,              _scorePanel                 );
            _dictionaryStates.Add(MainScreenStatesEnum.EVENT,               _eventWindow                );
            _dictionaryStates.Add( MainScreenStatesEnum.CHOOSE_TRACK, 		_chooseTrackWindow			);

			_dictionaryStates.Add( MainScreenStatesEnum.GAME_ASYNC,         _gameWindowAsync    		);
			_dictionaryStates.Add( MainScreenStatesEnum.GAME_SINGLE, 		_gameWindowSingleplayer		);
            _dictionaryStates.Add(MainScreenStatesEnum.GAME_TUTORIAL,       _gameWindowTutorial         );
        }

		private void HideAllWindowsExceptTarget(MainScreenStatesEnum state)
		{
            
            IUIWindow targetWindow = _dictionaryStates[state];
           

          
            for (int i = 0; i < _windows.Count; i++)
			{
               
                IUIWindow tempWindow = _windows[i];

				if (tempWindow != targetWindow) {
                   

                   tempWindow.Hide();
                }
			}
		}



	}
}