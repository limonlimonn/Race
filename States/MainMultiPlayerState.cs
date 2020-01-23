using UnityEngine;
using System.Collections.Generic;

using HCR.Enums;
using HCR.GlobalWindow.MainMenu;
using HCR.Interfaces;
using HCR.Network;

namespace HCR
{
	/// <summary>
	/// * your summary text *
	/// </summary>

	public class MainMultiPlayerState : IState
	{
		// FIELDS

		// dependences
		private Core _core;
		private GameManager _gameManager;
		private PlayerManager _playerManager;

		private UIManager _uiManager;
		private MultiplayerWindow _multiplayerWindow;
		private Header _header;
		private MainScreenStateManager _mainScreenStateManager;
        private SafePlayerPrefs _sPlayerP;
        private EventService _eventSecvice;
        private NotificationWindow _notificationWindow;
        private AudioService _audioService;
        private EnternetWindow _enternetWindow;

        public void Enable()
		{
            Debug.Log("MAIN_MULTIPLAYER_WINDOW  Enable");
            _audioService = Core.Instance.GetService<AudioService>();
            
            _uiManager = Core.Instance.GetService<UIManager>();
			_header = _uiManager.GetWindow(UIWindowEnum.HEADER) as Header;
			_header.Show();
            _sPlayerP = Core.Instance.GetService<SafePlayerPrefs>();

            _mainScreenStateManager = _uiManager.Get_MainScreenStateManager();

            _eventSecvice = Core.Instance.GetService<EventService>();

            _notificationWindow = _uiManager.NotificationWindow;

            _enternetWindow = _uiManager.GetWindow(UIWindowEnum.IS_ENTERNET) as EnternetWindow;

            if (_core == null)
			{
				_core = Core.Instance;
			}
            _audioService.StartMenuMusic();

            _gameManager = _core.GetService<GameManager>();
			_playerManager = _core.GetService<PlayerManager>();

			_multiplayerWindow = (MultiplayerWindow)_uiManager.GetWindow(UIWindowEnum.MAIN_MULTIPLAYER);
			//chooseCarWindow = (ChooseCarWindow)_uim.GetWindow(UIWindowEnum.CHOOSECAR);

			_multiplayerWindow.OnPlay += OnPlayGame;
			_multiplayerWindow.OnRandomGame +=  OnRandomGame;

			//
			_gameManager.OnGamesUpdated  += OnUpdateGamesList;
            _gameManager.OnFriendsUpdate += OnUpdateFriendList;
            

            if (_eventSecvice.IsEventComplete()){
                Debug.Log("IsEventComplete");
                _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
                _eventSecvice.ShowCompleteAnim(()=> { 
                    if(_playerManager.IsDone != -1)
                    {
                        _gameManager.LoadGames(() => { _notificationWindow.ShowYouOpenTrack(_playerManager.IsDone); _playerManager.IsDone = -1; });
                    }
                    else
                    {
                        _gameManager.LoadGames();
                    }
                });

            }else
            {
                
                if (_playerManager.IsDone != -1)
                {
                    _gameManager.LoadGames(()=> { _notificationWindow.ShowYouOpenTrack(_playerManager.IsDone); _playerManager.IsDone = -1; });
                }
                else
                {
                    _gameManager.LoadGames();
                }
            }


        }

        void OnUpdateFriendList(Dictionary<string,string> friends_name)
        {
            _multiplayerWindow.AddFriendsToList(friends_name);
        }

		void OnUpdateGamesList(List<GameData> gamesList)
		{
            if (_multiplayerWindow.canvaseGroup.alpha != 1)
            {
                _mainScreenStateManager.SwitchState(MainScreenStatesEnum.MAIN_MULTIPLAYER);
            }
            _multiplayerWindow.AddGamesToList(gamesList);
		}



        void OnPlayGame(string gameId)
        {
            _multiplayerWindow.OnPlay -= OnPlayGame;
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);

            Core.Instance.GetService<NetworkManager>().Join_Play(gameId, _playerManager.selectedCar, () =>
        {
            if (!_sPlayerP.IsGameId(gameId))
            {
                Debug.Log("NEW GAME");
                _sPlayerP.SafeDataOnPlayClick(_gameManager.GetGameDataByID(gameId));
                _gameManager.StartGame(gameId);
            }
            else if (_sPlayerP.IsRecord(gameId))
            {
                Debug.Log("RECORD GAME");
                _gameManager.ApplyTrack(_sPlayerP.GetTrack(gameId), gameId);
                _sPlayerP.FillReplayStatistic(gameId);
                _sPlayerP.FillUserStatistic(gameId);
            }
            else
            {
                Debug.Log("REPLAY GAME");
                _gameManager.StartGame(gameId);
            }
        }, (err) =>
        {
            if (err == "timeout")
            {
                _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
                _enternetWindow.ShowErrorEnternet();
                _multiplayerWindow.OnPlay += OnPlayGame;
            }else
            {
                Debug.LogError("OnPlayGame err");
            }
        });

        }

		void OnRandomGame()
		{
                    _gameManager.GetFreeGame();
		}

		public void Disable()
		{
            Debug.Log("MAIN_MULTIPLAYER_WINDOW  Disable");
            _audioService.StopMenuMusic();

            _multiplayerWindow.OnPlay -= OnPlayGame;
			_gameManager.OnGamesUpdated -= OnUpdateGamesList;
			_multiplayerWindow.OnRandomGame -= OnRandomGame;
			_multiplayerWindow.Hide();

			_multiplayerWindow.RemoveFromList();
			_multiplayerWindow.RemoveFromMostRecentGamesList();
		}


	}
}