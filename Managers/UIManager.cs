using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

using HCR.Enums;
using HCR.Gameplay;
using HCR.GlobalWindow;
using HCR.GlobalWindow.MainMenu;
using HCR.Interfaces;
using HCR.Loading;

using _AsyncMulty = HCR.Gameplay.AsyncMultiplayer;
using _Single = HCR.Gameplay.Singleplayer;
using _Tutorial = HCR.Gameplay.Tutorial;
using DG.Tweening;
using HCR.Network;

namespace HCR
{
	public class UIManager : MonoBehaviour, IService
	{
		// FIELDS

		#region VARIABLES

		// ---------------------------------------------------
		//
		[SerializeField] private MainScreenStateManager _mainScreenStateManager;
		[SerializeField] private SmilesContainer _smilesContainer;
        [SerializeField] public NotificationWindow NotificationWindow;

        // ---------------------------------------------------
        //
        private Dictionary<UIWindowEnum, IUIWindow> _dictWindows;
		public List<Sprite> _backgrounds = new List<Sprite>();
     
		// ---------------------------------------------------
		// windows --- Global
		private AuthentificationWindow _authentificationWindow;

		private MultiplayerWindow _multiplayerWindow;
		private Header _header;
		private ChooseCarWindow _chooseCarWindow;
		private ChooseTrackWindow _chooseTrackWindow;
		private TreeWindow _treeWindow;
        private SettingsWindow _settingsWindow;
        private StatisticPanel _statisticPanel;
        private SplashScreen _splashScreen;
        private ScorePanel _scorePanel;
        private EventWindow _eventWindow;
        private EnternetWindow _enternetWindow;
        // ---------------------------------------------------
        // windows --- Gameplay: Async Multiplayer
        private _AsyncMulty.GameWindowAsyncMultiplayer _gameWindow;

		private _AsyncMulty.PauseWindow _pauseWindow;
		private _AsyncMulty.FinishWindow _finishWindow;
		private _AsyncMulty.RecordFinishPanel _recordFinishPanel;
		private _AsyncMulty.RetryWindow _retryWindow;
		private _AsyncMulty.CrashFinishWindow _crashFinishWindow;

		// ---------------------------------------------------
		// windows --- Gameplay: Singleplayer
		private _Single.GameWindowSingleplayer _gameWindowSingleplayer;

		private _Single.PauseWindow _pauseWindowSingle;
		private _Single.FinishWindow _finishWindowSingle;
		private _Single.RetryWindow _retryWindowSingle;

        // window Tutorial gameplayer
        private _Tutorial.GameWindowTutorial _gameWindowsTutorial;
        private _Tutorial.PauseWindow _pauseWindowTutorial;
        private _Tutorial.FinishWindow _finishWindowTutorial;
        private _Tutorial.RetryWindow _retryWindowTutorial;
        private _Tutorial.ChooseCarTutorialWindow _chooseCarTutorial;
        private _Tutorial.TrigersTutorial _trigersTutorial;
       
        #endregion

        //window tutorial

        private IndexMenuTutorial _indexMenuTutorial;
        private BackgroundMenu _backgroundMenu;
        // I_SERVICE

        public void Init()
		{
			AssertVariables();
            

            InitVariables();
			InitDictionaryWindows();
			Invoke_InitMethod_InWindows();
            

            //
            _mainScreenStateManager.Init();
			_smilesContainer.Init();
		}



		// INTERFACES

		#region GETTERS/SETTERS

		public MainScreenStateManager Get_MainScreenStateManager()
		{
			// pre-conditions
			Assert.AreNotEqual(null, _mainScreenStateManager);

			return _mainScreenStateManager;
		}

		public SmilesContainer Get_SmilesContainer()
		{
			// pre-conditions
			Assert.AreNotEqual(null, _smilesContainer);

			return _smilesContainer;
		}

		#endregion 
        
		public void ShowWindow(UIWindowEnum windowId, bool isShowWindow)
		{
            
			if (isShowWindow)
			{
               // Debug.Log("Show " + windowId.ToString());
                _dictWindows[windowId].Show();
			}
			else
			{
                //Debug.Log("Hide " + windowId.ToString());
                _dictWindows[windowId].Hide();
			}
		}

		public IUIWindow GetWindow(UIWindowEnum window)
		{
			return _dictWindows[window];
		}

		public Sprite ChangeBG(UIWindowEnum window)
		{
			switch (window)
			{
				case UIWindowEnum.MAIN_MULTIPLAYER:
					return _backgrounds[1];

				case UIWindowEnum.CHOOSE_CAR:
					return _backgrounds[2];

				case UIWindowEnum.TREE_UPGRADE:
					return _backgrounds[3];

				case UIWindowEnum.SETTINGS:
					return _backgrounds[1];

				default:
					return _backgrounds[0];
			}
		}



		// METHODS

		private void AssertVariables()
		{
			Assert.AreNotEqual(null, _mainScreenStateManager);
			Assert.AreNotEqual(null, _smilesContainer);

			Assert.AreNotEqual(null, _backgrounds);
			Assert.IsTrue(_backgrounds.Count > 0);
		}

		#region INITIALIZE

		private void InitVariables()
		{
			Init_GlobalWindows();
			Init_Gameplay_AsyncMultilpayer();
			Init_Gameplay_Singleplayer();
            Init_Tutorial_Game();
            Init_Tutorial_Menu();

        }

		private void Init_GlobalWindows()
		{
			_authentificationWindow = GetComponentInChildren<AuthentificationWindow>();
			Assert.AreNotEqual(null, _authentificationWindow);

			//
			_multiplayerWindow = GetComponentInChildren<MultiplayerWindow>();
			Assert.AreNotEqual(null, _multiplayerWindow);

			_header = GetComponentInChildren<Header>();
			Assert.AreNotEqual(null, _header);

			_chooseCarWindow = GetComponentInChildren<ChooseCarWindow>();
			Assert.AreNotEqual(null, _chooseCarWindow);

			_chooseTrackWindow = GetComponentInChildren<ChooseTrackWindow>();
			Assert.AreNotEqual(null, _chooseTrackWindow);

			_treeWindow = GetComponentInChildren<TreeWindow>();
			Assert.AreNotEqual(null, _treeWindow);

			_settingsWindow = GetComponentInChildren<SettingsWindow>();
			Assert.AreNotEqual(null, _settingsWindow);

            _statisticPanel = GetComponentInChildren<StatisticPanel>();
            Assert.AreNotEqual(null, _statisticPanel);
            //
            _splashScreen = GetComponentInChildren<SplashScreen>();
			Assert.AreNotEqual(null, _splashScreen);

            _scorePanel = GetComponentInChildren<ScorePanel>();
            Assert.AreNotEqual(null, _scorePanel);

            _eventWindow = GetComponentInChildren<EventWindow>();
            Assert.AreNotEqual(null, _eventWindow);

            _enternetWindow = GetComponentInChildren<EnternetWindow>();
            Assert.AreNotEqual(null, _enternetWindow);

        }

		private void Init_Gameplay_AsyncMultilpayer()
		{
			_gameWindow = GetComponentInChildren<_AsyncMulty.GameWindowAsyncMultiplayer>();
			Assert.AreNotEqual(null, _gameWindow);

			//
			_pauseWindow = GetComponentInChildren<_AsyncMulty.PauseWindow>();
			Assert.AreNotEqual(null, _pauseWindow);

			_finishWindow = GetComponentInChildren<_AsyncMulty.FinishWindow>();
			Assert.AreNotEqual(null, _finishWindow);

			_retryWindow = GetComponentInChildren<_AsyncMulty.RetryWindow>();
			Assert.AreNotEqual(null, _retryWindow);

			_recordFinishPanel = GetComponentInChildren<_AsyncMulty.RecordFinishPanel>();
			Assert.AreNotEqual(null, _recordFinishPanel);

			_crashFinishWindow = GetComponentInChildren<_AsyncMulty.CrashFinishWindow>();
			Assert.AreNotEqual(null, _crashFinishWindow);
		}

		private void Init_Gameplay_Singleplayer()
		{
			_gameWindowSingleplayer = GetComponentInChildren<_Single.GameWindowSingleplayer>();
			Assert.AreNotEqual(null, _gameWindowSingleplayer);

			//
			_pauseWindowSingle = GetComponentInChildren<_Single.PauseWindow>();
			Assert.AreNotEqual(null, _pauseWindowSingle);

			_finishWindowSingle = GetComponentInChildren<_Single.FinishWindow>();
			Assert.AreNotEqual(null, _finishWindowSingle);

			_retryWindowSingle = GetComponentInChildren<_Single.RetryWindow>();
			Assert.AreNotEqual(null, _retryWindowSingle);
		}

        private void Init_Tutorial_Game()
        {
            _gameWindowsTutorial = GetComponentInChildren<_Tutorial.GameWindowTutorial>();
            Assert.AreNotEqual(null, _gameWindowsTutorial);

            _pauseWindowTutorial = GetComponentInChildren<_Tutorial.PauseWindow>();
            Assert.AreNotEqual(null, _pauseWindowTutorial);

            _finishWindowTutorial = GetComponentInChildren<_Tutorial.FinishWindow>();
            Assert.AreNotEqual(null, _finishWindowTutorial);

            _retryWindowTutorial = GetComponentInChildren<_Tutorial.RetryWindow>();
            Assert.AreNotEqual(null, _retryWindowTutorial);

            _chooseCarTutorial = GetComponentInChildren<_Tutorial.ChooseCarTutorialWindow>();
            Assert.AreNotEqual(null, _chooseCarTutorial);

            _trigersTutorial = GetComponentInChildren<_Tutorial.TrigersTutorial>();
            Assert.AreNotEqual(null, _trigersTutorial);

           
        }


        private void Init_Tutorial_Menu()
        {
            _indexMenuTutorial = GetComponentInChildren<IndexMenuTutorial>();
            Assert.AreNotEqual(null, _indexMenuTutorial);

            _backgroundMenu = GetComponentInChildren<BackgroundMenu>();
            Assert.AreNotEqual(null, _backgroundMenu);
        }

        #endregion

        private void InitDictionaryWindows()
		{
			_dictWindows = new Dictionary<UIWindowEnum, IUIWindow>();

			// ---------------------------------------------------
			// windows --- global
			_dictWindows.Add( UIWindowEnum.AUTHENTIFICATION, 		_authentificationWindow		);

			_dictWindows.Add( UIWindowEnum.MAIN_MULTIPLAYER, 		_multiplayerWindow			);
			_dictWindows.Add( UIWindowEnum.HEADER, 					_header						);
			_dictWindows.Add( UIWindowEnum.CHOOSE_CAR, 				_chooseCarWindow			);
			_dictWindows.Add( UIWindowEnum.CHOOSE_TRACK, 			_chooseTrackWindow			);
			_dictWindows.Add( UIWindowEnum.TREE_UPGRADE, 			_treeWindow					);
			_dictWindows.Add( UIWindowEnum.SETTINGS, 				_settingsWindow				);
            _dictWindows.Add( UIWindowEnum.STATISTIC,               _statisticPanel             );
            _dictWindows.Add( UIWindowEnum.SPLASH, 					_splashScreen				);
            _dictWindows.Add( UIWindowEnum.SCORE,                   _scorePanel                 );
            _dictWindows.Add( UIWindowEnum.EVENT,                   _eventWindow                );
            _dictWindows.Add( UIWindowEnum.IS_ENTERNET,             _enternetWindow             );

            // ---------------------------------------------------
            // windows --- Gameplay: Async Multiplayer
            _dictWindows.Add( UIWindowEnum.GAMEWINDOW_ASYNC, 		_gameWindow					);

			_dictWindows.Add( UIWindowEnum.PAUSE_ASYNC, 			_pauseWindow				);
			_dictWindows.Add( UIWindowEnum.FINISH_ASYNC, 			_finishWindow				);
			_dictWindows.Add( UIWindowEnum.RECORD_FINISH_ASYNC, 	_recordFinishPanel			);
			_dictWindows.Add( UIWindowEnum.RETRY_ASYNC, 			_retryWindow				);
			_dictWindows.Add( UIWindowEnum.CRASH_FINISH_ASYNC, 		_crashFinishWindow			);

			// ---------------------------------------------------
			// windows --- Gameplay: Singleplayer
			_dictWindows.Add( UIWindowEnum.GAMEWINDOW_SINGLE, 		_gameWindowSingleplayer		);
			_dictWindows.Add( UIWindowEnum.PAUSE_SINGLE, 			_pauseWindowSingle			);
			_dictWindows.Add( UIWindowEnum.FINISH_SINGLE, 			_finishWindowSingle			);
			_dictWindows.Add( UIWindowEnum.RETRY_SINGLE, 			_retryWindowSingle			);

            // ---------------------------------------------------
            //Window tutorial

            _dictWindows.Add(UIWindowEnum.GAMEWINDOW_TUTORIAL,       _gameWindowsTutorial        );
            _dictWindows.Add(UIWindowEnum.PAUSE_TUTORIAL,            _pauseWindowTutorial        );
            _dictWindows.Add(UIWindowEnum.FINISH_TUTORIAL,           _finishWindowTutorial       );
            _dictWindows.Add(UIWindowEnum.RETRY_TUTORIAL,            _retryWindowTutorial        );
            _dictWindows.Add(UIWindowEnum.CHOOSE_CAR_TUTORIAL,       _chooseCarTutorial          );
            _dictWindows.Add(UIWindowEnum.TRIGERS_TUTORIAL,          _trigersTutorial            );
          


            // ---------------------------------------------------
            //Window tutorialMenu
            _dictWindows.Add(UIWindowEnum.MENUTUTORIALWINDOW,        _indexMenuTutorial  );
            _dictWindows.Add(UIWindowEnum.FIRSTTUTORIALWINDOW,       _backgroundMenu     );

        }

        private void Invoke_InitMethod_InWindows()
		{
			// ---------------------------------------------------
			// windows --- global
			_authentificationWindow.Init();
			_multiplayerWindow.Init();
			_header.Init();
			_chooseCarWindow.Init();
			_chooseTrackWindow.Init();
			_treeWindow.Init();
			_settingsWindow.Init();
            _statisticPanel.Init();
            _eventWindow.Init();
            _splashScreen.Init();
            _scorePanel.Init();
            _enternetWindow.Init();

            // ---------------------------------------------------
            // windows --- Gameplay: Async Multiplayer
            _gameWindow.Init();

			_pauseWindow.Init();
			_finishWindow.Init();
			_recordFinishPanel.Init();
			_retryWindow.Init();
			_crashFinishWindow.Init();

			// ---------------------------------------------------
			// windows --- Gameplay: Singleplayer
			_gameWindowSingleplayer.Init();

			_pauseWindowSingle.Init();
			_finishWindowSingle.Init();
			_retryWindowSingle.Init();
            //_chooseCarTutorial.Init();

            // windows --- Gameplay: Tutorial

            _gameWindowsTutorial.Init();

            _pauseWindowTutorial.Init();
            _finishWindowTutorial.Init();
            _retryWindowTutorial.Init();
            _chooseCarTutorial.Init();
            _trigersTutorial.Init();
         
            // ---------------------------------------------------
            _indexMenuTutorial.Init();
            _backgroundMenu.Init();
        }


        public void UpdateMyGamesCounter() {
            
            _multiplayerWindow.OnClickButton_MyGames();
            _multiplayerWindow.UpdateMyGamesCounter();
        }

        public void HideCanvas(CanvasGroup canvas)
        {
            canvas.DOFade(0, 0.2f);
        }
        public void ShowCanvas(CanvasGroup canvas)
        {
            canvas.DOFade(1, 0.2f);//.SetDelay<Tweener>(0.25f);
        }
    }
}