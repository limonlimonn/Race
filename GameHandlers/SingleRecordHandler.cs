using UnityEngine;
using UnityEngine.Assertions;

using HCR.Enums;
using HCR.Interfaces;
using HCR.Event;

namespace HCR.Gameplay.Singleplayer
{
	/// <summary>
	/// Класс - хэндлер одиночного заезда ("Игра: Синглплеер")
	/// </summary>

	public class SingleRecordHandler : ABaseRecordHandler, IGameHandler
	{
		// FIELDS

		// dependences
		private SafePlayerPrefs _safePlayerPrefs;

		private GameWindowSingleplayer _gameWindow;
		private PauseWindow _pauseWindow;
		private RetryWindow _retryWindow;
		private FinishWindow _finishWindow;

		// I_HANDLER

		#region I_HANDLER_INTERFACES

		public void Init(GameData data)
		{
			Debug.Log("INIT >>> SINGLE HANDLER");

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

			_gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_SINGLE) as GameWindowSingleplayer;
			Assert.AreNotEqual(null, _gameWindow);

			_pauseWindow = _uiManager.GetWindow(UIWindowEnum.PAUSE_SINGLE) as PauseWindow;
			Assert.AreNotEqual(null, _pauseWindow);

			_retryWindow = _uiManager.GetWindow(UIWindowEnum.RETRY_SINGLE) as RetryWindow;
			Assert.AreNotEqual(null, _retryWindow);

			_finishWindow = _uiManager.GetWindow(UIWindowEnum.FINISH_SINGLE) as FinishWindow;
			Assert.AreNotEqual(null, _finishWindow);

			//
			_gameData = data;
			_gameManager.LoadTrack(OnTrackLoaded);
		}

		public void Start()
		{
			_uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
			_gameWindow.Show();
			_coroutineStartTimer = Core.Instance.StartCor( StartTimer(_gameWindow) );
		}

		public void Restart()						// !!!
		{
			CheckWhatDoingOnRestart();
		}

        public void TryAgain()
        {
            RestartWork_HaveTries();
        }

        public void OnPlayerCrash()					// !!!
		{
			Zaglushka_CrashAndThenFinish();
			CheckWhatWindowShowOnCrash();
		}

		public void OnPlayerFinish()				// !!!
		{
			Zaglushka_FinishAndThenCrash();

            EventManager._init.Game.CarEvent.Player.GetEvent.Finish -= OnPlayerFinish;
			_playerCar.EnablePlayerControll(false);

			_coroutineMovePlayerOnFinish = Core.Instance.StartCor( MovePlayerOnFinish() );
			_gameData.recordData = "";

			ShowWindow_WinFinish();
		}

		public void CalculateResults()
		{
			// none
		}

		public void Destroy()
		{
			UnsubscribeOnEvents();
			SafeStopCoroutine_StartTimer();

			_playerManager.PlayerCar = null;
			_playerCar.Destroy();
		}

		#endregion

		// METHODS

		private void OnTrackLoaded()
		{
			_gameWindow.Hide();
			
			SetPlayerCarSettings(_gameWindow);
			_gameWindow.SetValue_PlayerCar();
			SubscribeOnEvents();

			Start();
		}

		#region RESTART_WORK

		protected override void RestartWork_HaveTries()
		{
			Debug.Log("(SingleHandler) HAVE TRIES");

			Destroy();
			_gameData.recordData = "";					// ??? do I need it ???
			_gameManager.LoadTrack(OnTrackLoaded);
		}

		protected override void RestartWork_HaveNoTriesLeft()
		{
			Debug.Log("(SingleHandler) HAVE NO TRIES");

			SafeStopCoroutine_StartTimer();
			ShowWindow_CrashFinish();
		}

		#endregion

		#region SHOW_WINDOW

		protected override void ShowWindow_Retry()
		{
			_retryWindow.Show();
		}

		protected override void ShowWindow_CrashFinish()
		{
			//Debug.Log("(loose) TRIES = " + TryesCount);

			_finishWindow.SetValue_RaceResult(RaceResultEnum.LOSING);
			_finishWindow.SetValue_GoldForWinAttempts(0);

            _finishWindow.SetValue_XPForWinAttempts(0);

			CalculateEarnedGold(0);
			//CalculateEarnedGold(0);
			CalculateEarnedExp(0);

			_finishWindow.Show();
		}

		private void ShowWindow_WinFinish()
		{
			//Debug.Log("(win) TRIES = " + TryesCount);

			_finishWindow.SetValue_RaceResult(RaceResultEnum.WIN);
			int bonusGold = CalculateWinAttemptsGold();
			_finishWindow.SetValue_GoldForWinAttempts(bonusGold);
            int bonusXP = _finishWindow.XPForStars(_gameWindow);
            _finishWindow.SetValue_XPForWinAttempts(bonusXP);

			CalculateEarnedGold(bonusGold);
			//CalculateEarnedGold(0);
			CalculateEarnedExp(0);

			_finishWindow.Show();
		}

		#endregion

		#region SUBSCRIBE/UNSUBSCRIBE

		private void SubscribeOnEvents()
		{
            EventManager._init.Game.MenuButton.TryAgain += TryAgain;
			//_pauseWindow.OnRestartClick += Restart;

            EventManager._init.Game.CarEvent.Player.GetEvent.Finish += OnPlayerFinish;

            EventManager._init.Game.CarEvent.Player.GetEvent.Crash += OnPlayerCrash;
		}

		private void UnsubscribeOnEvents()
		{
            EventManager._init.Game.MenuButton.TryAgain -= TryAgain;
			//_pauseWindow.OnRestartClick -= Restart;

            EventManager._init.Game.CarEvent.Player.GetEvent.Finish -= OnPlayerFinish;

            EventManager._init.Game.CarEvent.Player.GetEvent.Crash -= OnPlayerCrash;

            //
            EventManager._init.Game.CarEvent.Player.GetEvent.Crash -= _playerCar.ExplosePlayer;
		}

		#endregion

		#region WIN/LOOSE

		private int CalculateWinAttemptsGold()
		{
			int bonusGold = 0;

			int triesCount = _gameWindow.triesCount;
            //_gameData.tryesCount = _gameWindow.triesCount;

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
				+ bonusGold );
		}

		private void CalculateEarnedExp(float bonus)
		{
			// TODO --- узнать как будет рассчитываться !

			_safePlayerPrefs.AddEarnedExp(bonus);
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

        protected override void SafeStopCoroutine_Record()
        {

        }

        #endregion




    }
}