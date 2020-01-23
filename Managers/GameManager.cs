using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using HCR.Gameplay.AsyncMultiplayer;
using HCR.Gameplay.Singleplayer;
using HCR.Gameplay.Tutorial;
using HCR.Loading;
using HCR.Enums;
using HCR.GlobalWindow.MainMenu;
using HCR.Interfaces;
using HCR.Network;
using HCR.Event;

namespace HCR
{
    /// <summary>
    /// * your summary text *
    /// </summary>

    public class GameManager : IService
    {
        // ACTIONS

        public Action<List<GameData>> OnGamesUpdated;
        public Action<List<GameData>> OnGamesMostRecentUpdated;
        public Action ChooseTrackNow;
        public Action tracksSettingsLoaded;
        public Action<Dictionary<string, string>> OnFriendsUpdate;
        public bool ButtAsyncOn = false;

        // FIELDS

        // -------------------------------------------------
        //
        public GameTypeEnum GameType { get; private set; }
        public bool IsEntryPointLoad { get; private set; }
        public List<GameData> Games { get; private set; }

        //TODO Move to shop manager, add cars data as value
        public Dictionary<int, string> carsNamesById { get; private set; }

        //Track's diplay names
        public Dictionary<int, string> TrackNamesById = new Dictionary<int, string>();
        public List<TrackTimePrice> TrackTimeById = new List<TrackTimePrice>();                                // int[]{ 1 - gold , 2 - jewels , 3 - time}


        public GameData gameData { get; private set; }

        // -------------------------------------------------
        // dependences
        private IGameHandler _handler;
        private NetworkManager _networkManager;
        private PlayerManager _playerManager;
        private StatesManager _statesManager;
        private Header _header;

        private UIManager _uiManager;
        private MainScreenStateManager _mainScreenStateManager;
        private SplashScreen _splashScreen;
        private MultiplayerWindow _multiplayerWindow;
        private TutorialState _tutorialState;
        private SafePlayerPrefs _safePlayerPrefs;
        private ChooseTrackWindow _chooseTrackWindow;
        private EnternetWindow _enternetWindow;
        private GameObject _podiumNew;
        private Transform _carParent;
        // I_SERVICE

        public void Init()
        {
            Games = new List<GameData>();

            //
            _networkManager = Core.Instance.GetService<NetworkManager>();
            Assert.AreNotEqual(null, _networkManager);

            _playerManager = Core.Instance.GetService<PlayerManager>();
            Assert.AreNotEqual(null, _playerManager);

            _statesManager = Core.Instance.GetService<StatesManager>();
            Assert.AreNotEqual(null, _statesManager);

            //
            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);

            _chooseTrackWindow = _uiManager.GetWindow(UIWindowEnum.CHOOSE_TRACK) as ChooseTrackWindow;
                Assert.AreNotEqual(null, _chooseTrackWindow);

            _mainScreenStateManager = _uiManager.Get_MainScreenStateManager();
            Assert.AreNotEqual(null, _mainScreenStateManager);

            _splashScreen = _uiManager.GetWindow(UIWindowEnum.SPLASH) as SplashScreen;
            Assert.AreNotEqual(null, _splashScreen);

            _multiplayerWindow = _uiManager.GetWindow(UIWindowEnum.MAIN_MULTIPLAYER) as MultiplayerWindow;
            Assert.AreNotEqual(null, _multiplayerWindow);

            

            _safePlayerPrefs = Core.Instance.GetService<SafePlayerPrefs>();
            Assert.AreNotEqual(null, _safePlayerPrefs);

            _header = _uiManager.GetWindow(UIWindowEnum.HEADER) as Header;
            Assert.AreNotEqual(null, _header);

            _enternetWindow = _uiManager.GetWindow(UIWindowEnum.IS_ENTERNET) as EnternetWindow;
            Assert.AreNotEqual(null, _enternetWindow);

            _carParent = GameObject.Find("CarParent").transform;
            _podiumNew = _carParent.Find("PodiumNew").gameObject;

            

            // =======================================================
            //
            /*
            TrackNamesById = new Dictionary<int, string>()
            {
                {0,     ""    },
                {1,     ""     },
                {2,     ""    },
                {3,     ""      },
                {4,     ""    },
                {5,     "West_Indian_1"     },
                {6,     ""     },
                {7,     ""      },
                {8,     "West _Town_2"      },
                {9,     ""     },
                {10,    "West_Shahta_1"     },
                {11,    "West _Canyon_1"    },
                {12,    "West_Tornado"      },
                {13,    "Island"            },
                {14,    "West_Tunnel_3"     },
                {15,    "West_Mountains_2"   },
                {16,    "West _Koleso_2"   },
                {17,    "West _Canyon_2"   },


                {99,    "Tutorial1"   },
                {100,    "Tutorial2"   },
            };
            */



            //
            /*
            TrackTimeById = new Dictionary<int, TrackTimePrice>()
            {
                {0,    new TrackTimePrice{Time = 120 , Price= 20  } },
                {1,    new TrackTimePrice{Time = 120 , Price = 21  } },
                {2,    new TrackTimePrice{Time = 120 , Price = 22  } },
                {3,    new TrackTimePrice{Time = 120 , Price = 23  } },
                {4,    new TrackTimePrice{Time = 120 , Price = 24  } },
                {5,    new TrackTimePrice{Time = 120 , Price = 25  } },
                {6,    new TrackTimePrice{Time = 120 , Price = 26  } },
                {7,    new TrackTimePrice{Time = 120 , Price= 27  } },
                {8,    new TrackTimePrice{Time = 120 , Price = 28  } },
                {9,    new TrackTimePrice{Time = 120 , Price= 29  } },
                {10,   new TrackTimePrice{Time = 120 , Price = 30  } },
                {11,   new TrackTimePrice{Time = 100 , Price = 31  } },
                {12,   new TrackTimePrice{Time = 72 , Price = 32  } },
                {13,   new TrackTimePrice{Time = 93 , Price = 33  } },
                {14,   new TrackTimePrice{Time = 74 , Price = 34  } },
                {15,   new TrackTimePrice{Time = 95 , Price = 35  } },
                {16,   new TrackTimePrice{Time = 76 , Price = 36  } },
                {17,   new TrackTimePrice{Time = 87 , Price = 37  } },

            };
            */
            // =======================================================

            _multiplayerWindow.OnSingleNew += CreateNewSingleplayerGame;
            _multiplayerWindow.OnNewGame += CreateNewGame;
            _multiplayerWindow.OnNewGameForFB += CreateNewGameForFB;
            IsEntryPointLoad = true;
        }

        
       
        
        public void LoadGames(Action OnComplete = null)
        {
            Debug.Log("ButtAsyncOn" + ButtAsyncOn);
            if (!ButtAsyncOn)
            {
                _splashScreen.ShowLoadingPanel();
                Debug.Log("LoadGames");
                ButtAsyncOn = true;
                _networkManager.LoadGames(
                    (data) =>
                    {
                        Debug.Log("LoadGames ok");
                        Games.Clear();
                        Games = data;
                        _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
                        if (OnGamesUpdated != null)
                        {
                            OnGamesUpdated(Games);
                            if(_playerManager.IsDone != -1) { OnComplete(); }
                            
                            //_uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
                        }
                        ButtAsyncOn = false;
                    }, 
                    (err)=> {
                        if (err == "timeout")
                        {
                            ButtAsyncOn = false;
                            Debug.Log("LoadGames err "  + _multiplayerWindow.canvaseGroup.alpha + " " );

                            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
                            //_uiManager.
                            if (_multiplayerWindow.canvaseGroup.alpha != 1)
                            {
                                Debug.Log("_multiplayerWindow.canvaseGroup.alpha != 1 " + _multiplayerWindow.canvaseGroup.alpha);
                                _uiManager.ShowWindow(UIWindowEnum.MAIN_MULTIPLAYER, true);
                            }

                            _enternetWindow.ShowErrorEnternet();
                        }
                        else { Debug.LogError("LoadGames err");
                            ButtAsyncOn = false;
                        }
                    },
                    (ErrorLoad)=> {
                        if (ErrorLoad == "timeout")
                        {
                            ButtAsyncOn = false;
                        Debug.Log("LoadGames err " + _multiplayerWindow.canvaseGroup.alpha + " ");

                        _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
                        _enternetWindow.ShowErrorEnternet();
                    }
                        else {
                    Debug.LogError("LoadGames err");
                    ButtAsyncOn = false;
                }

            });
                //Debug.Log("posle");
                
            }
            //Debug.Log("posle");
        }

        #region GET_FREE_GAME

        public void GetFreeGame()
        {
            Debug.Log("GetFreeGame");
            _networkManager.FindOpenGame(OnFreeGame, 
                (err)=> {
                    if (err != "timeout")
                    {
                        Debug.Log("CreateNewGame");
                        CreateNewGame();
                    }
                    else
                    {
                        Debug.Log("LoadGames err");
                        _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
                        _enternetWindow.ShowErrorEnternet();
                    }
                     });
        }

        void OnFreeGame(GameData gd)
        {
            Debug.Log("OnFreeGame");
            _networkManager.Join_Play(gd.gameId, _playerManager.selectedCar,()=> {
                Debug.Log("Join_Play ok");
                _safePlayerPrefs.SafeDataOnPlayClick(gd);
                _podiumNew.SetActive(false);
                StartGame(gd.gameId);
            },
            (err)=>
                {
                    if(err == "timeout")
                    {
                        Debug.Log("Join_Play err");
                        _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
                        _enternetWindow.ShowErrorEnternet();
                    }
                    else
                        Debug.LogError("Join_Play err");
                });
            //_uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
        }

        void CreateNewGame()
        {
            gameData = CreateGameData();
            _mainScreenStateManager.SwitchState(MainScreenStatesEnum.CHOOSE_TRACK);
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
        }

        void CreateNewGameForFB(string player2_id, string player2_name)
        {
            gameData = CreateGameDataForFB(player2_id, player2_name);
            _mainScreenStateManager.SwitchState(MainScreenStatesEnum.CHOOSE_TRACK);
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
        }

        public void CreateTutorialGame(int trackId)
        {

            Games.Clear();
            gameData = CreateTutorialGameData();


            ApplyTrack(trackId);

        }
        #endregion

        public void ApplyTrack(int trackId, string gameId = null)
        {
            if (gameData == null|| gameId != null)
            {
                _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
                //Debug.Log("gameData == null");
                gameData = new GameData();
                gameData = GetGameDataByID(gameId);
                gameData.GameType = GameTypeEnum.MultyRecord;
                gameData.track_id = trackId.ToString();
                _safePlayerPrefs.SaveTrackRecord(gameData.gameId, trackId);
                Core.Instance.GetService<StatesManager>().SwitchState(StatesEnum.GAME_ASYNC);
                return;
            }
            if(gameData.GameType != GameTypeEnum.MultyNew)
                _safePlayerPrefs.SaveTrackRecord(gameData.gameId, trackId);


            /*
             if (gameData == null)
             {
                 Debug.Log("FillingGameData  ApplyTrack");
                 _safePlayerPrefs.FillingGameData(gameData, _playerManager.selectedCar);
                 _safePlayerPrefs.Delte_gameData();
             }
             gameData.AppyTrack = 1;
             gameData.track_id = trackId.ToString();*/
            //Debug.Log("GameType" + gameData.GameType + " trackId " + trackId);
            gameData.track_id = trackId.ToString();
            
            // ----------------------------------------------------------------------------
            // SINGLEPLAYER
            if (gameData.GameType == GameTypeEnum.SingleNew)
            {
                StartSingleplayerGame(gameData.gameId);
            }
            // ----------------------------------------------------------------------------
            // ASYNC MULTI - NEW
            else if (gameData.GameType == GameTypeEnum.MultyNew)
            {
                StartGame(gameData.gameId);
            }
            // ----------------------------------------------------------------------------
            //
            // ----------------------------------------------------------------------------
            // ASYNC MULTI - NEW
            else if (gameData.GameType == GameTypeEnum.MultyNewFB)
            {
                StartGame(gameData.gameId);
            }

            else if (gameData.GameType == GameTypeEnum.TutorialGame)
            {
                StartTutorialGame(gameData.gameId);
            }
            // ----------------------------------------------------------------------------
            //
            else
            {
                _mainScreenStateManager.SwitchState(MainScreenStatesEnum.GAME_ASYNC);
                Finish();
            }
            // ----------------------------------------------------------------------------
        }

        //New game
        GameData CreateGameData()
        {
            var gd = Games.Find(g => g.gameId == "new");
            if (gd != null)
            {
                return gd;
            }
            else
            {
                gd = new GameData();
                gd.gameId = "new";
                gd.GameType = GameTypeEnum.MultyNew;
                gd.player1_Id = _playerManager.PlayerId;
                gd.player1_name = _playerManager.PlayerDisplayName;

                if (_playerManager.selectedCar == null)
                {
                    if (_playerManager.playerCars[0] == null)
                    {
                        throw new Exception();
                    }

                    _playerManager.selectedCar = _playerManager.playerCars[0];
                }

                gd.player1_car = _playerManager.selectedCar;
                Games.Add(gd);

                return gd;
            }

        }

        //New game for FB

        GameData CreateGameDataForFB(string player2_id, string player2_name)
        {
            var gd = new GameData();
            gd.gameId = "new";
            gd.GameType = GameTypeEnum.MultyNewFB;
            gd.player1_Id = _playerManager.PlayerId;
            gd.player1_name = _playerManager.PlayerDisplayName;

            gd.player2_id = player2_id;
            gd.player2_name = player2_name;

            if (_playerManager.selectedCar == null)
            {
                if (_playerManager.playerCars[0] == null)
                {
                    throw new Exception();
                }

                _playerManager.selectedCar = _playerManager.playerCars[0];
            }

            gd.player1_car = _playerManager.selectedCar;
            Games.Add(gd);

            return gd;
        }


        //TutorialGame

        GameData CreateTutorialGameData()
        {
            var gd = new GameData();
            gd.gameId = "new";
            gd.GameType = GameTypeEnum.TutorialGame;
            gd.player1_Id = _playerManager.PlayerId;
            gd.player1_name = _playerManager.PlayerDisplayName;

            _playerManager.selectedCar = _playerManager.playerCars[_playerManager.currentCar.type];
            _playerManager.selectedCar.current_color = _playerManager.currentCar.TutorialColor;

            gd.player1_car = _playerManager.selectedCar;
            Games.Add(gd);

            return gd;
        }
        

        public void StartGame(string gameId)
        {
            gameData = GetGameDataByID(gameId);
            if (!IsCanStartReplayGame()) { Debug.Log("++IsCanStartReplayGame: "); Core.Instance.GetService<StatesManager>().SwitchState(StatesEnum.GAME_ASYNC); return; }

                if (!_safePlayerPrefs.IsRecord(gameId) && _safePlayerPrefs.IsReplayDone(gameId) && _safePlayerPrefs.IsReplay(gameId))
            {
                _header.canvaseGroup.interactable = false;
                //Debug.Log("CHOOSE_TRACK");
                _chooseTrackWindow.IsChooseTrack = gameId;
                _mainScreenStateManager.SwitchState(MainScreenStatesEnum.CHOOSE_TRACK);
                return;
            }
            // --------------------------------------------------------------------------------------------------
            //
            if (gameData != null)
            {
                // -----------------------------------------------
                if (IsCanStartReplayGame())
                {
                    //Debug.LogError("LoadReplay " + gameId);
                    _networkManager.LoadReplay(gameId,
                        () => { Debug.Log("StartGame ok"); Core.Instance.GetService<StatesManager>().SwitchState(StatesEnum.GAME_ASYNC); },
                        (err) => { Debug.LogError("Can't load replay: " + gameId);
                            if(err == "timeout")
                            {
                                Debug.Log("StartGame err");
                                _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
                                _enternetWindow.ShowErrorEnternet();
                            }else
                            {
                                Debug.LogError("StartGame err");
                            }
                        }
                    );
                }
                else
                {

                    Core.Instance.GetService<StatesManager>().SwitchState(StatesEnum.GAME_ASYNC);
                }
                // -----------------------------------------------
            }
            // --------------------------------------------------------------------------------------------------
            //
            else
            {
                Debug.LogError("GAME DATA IS NULL!");
                _mainScreenStateManager.SwitchState(MainScreenStatesEnum.MAIN_MULTIPLAYER);
            }
            // --------------------------------------------------------------------------------------------------

        }

        private bool IsCanStartReplayGame()
        {
            return (
                (gameData.GameType == GameTypeEnum.MultyReplay
                 || gameData.GameType == GameTypeEnum.MultyJoin)

                && gameData.replayData == null);
        }

        public void Finish()
        {
            IsEntryPointLoad = false;
            //Send stat
            switch (gameData.GameType)
            {
                // -----------------------------------------------------------------------------------------------
                //
                case GameTypeEnum.MultyNew:
                    SendSaveFirstRecord();
                    break;

                // -----------------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------------
                //
                case GameTypeEnum.MultyNewFB:
                    _networkManager.SaveFirstRecordForFB(gameData.gameId,
                        Core.Instance.GetService<PlayerManager>().PlayerCar.model,
                        () =>
                        {
                            Core.Instance.GetService<StatesManager>().SwitchState(StatesEnum.MAIN_MULTIPLAYER_WINDOW);

                            Core.Instance.StartCor(
                                LoadGameScene("EntryPoint",
                                () => { Core.Instance.GetService<UIManager>().ShowWindow(UIWindowEnum.SPLASH, false); }
                            ));
                        },
                        () => { Debug.LogError("Error send first FB statistics"); }
                    );
                    break;

                // -----------------------------------------------------------------------------------------------
                //
                case GameTypeEnum.MultyRecord:
                    Debug.Log("MultyRecord");
                    SendSaveRecord();



                    break;

                // -----------------------------------------------------------------------------------------------
                //
                case GameTypeEnum.MultyReplay:
                    gameData.GameType = GameTypeEnum.MultyRecord;
                    if(_handler != null)
                    _handler.Destroy();
                    CreateGameHandler();
                    break;
                // -----------------------------------------------------------------------------------------------
                //
                case GameTypeEnum.SingleNew:
                    _handler.Destroy();
                    _statesManager.SwitchState(StatesEnum.MAIN_MULTIPLAYER_WINDOW);

                    Core.Instance.StartCor(
                        LoadGameScene("EntryPoint",
                        () => { Core.Instance.GetService<UIManager>().ShowWindow(UIWindowEnum.SPLASH, false); }
                    ));
                    break;

                    // -----------------------------------------------------------------------------------------------
            }
        }

        private void SendSaveRecord()
        {
             _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
            
                    _splashScreen.loadingText = "LOADING";
                    _networkManager.SaveRecord(gameData.gameId,
                                            Core.Instance.GetService<PlayerManager>().PlayerCar.model, () =>
                                            {

                                                

                                                Core.Instance.StartCor(
                                                    LoadGameScene("EntryPoint",
                                                    () =>
                                                    {
                                                        Core.Instance.GetService<StatesManager>().SwitchState(StatesEnum.MAIN_MULTIPLAYER_WINDOW);
                                                        //Core.Instance.GetService<UIManager>().ShowWindow(UIWindowEnum.SPLASH, false);

                                                    }
                                                ));
                                            },
                                            (err) =>
                                            {
                                                //open try again tryAgainButton += sendAgain(gameData.gameId,dasds);
                                                if (err == "timeout")
                                                {
                                                    Debug.LogError("Timeout error");
                                                    _enternetWindow.ShowErrorSend(SendSaveRecord, "Send later", () =>
                                                     {
                                                         _safePlayerPrefs.SaveReplayData(gameData);
                                                         _networkManager.IsSaveGame = true;
                                                         Core.Instance.GetService<UIManager>().ShowWindow(UIWindowEnum.SPLASH, true);

                                                         Core.Instance.StartCor(
                                                             LoadGameScene("EntryPoint",
                                                             () =>
                                                             {
                                                                 Core.Instance.GetService<StatesManager>().SwitchState(StatesEnum.MAIN_MULTIPLAYER_WINDOW);
                                                                 //Core.Instance.GetService<UIManager>().ShowWindow(UIWindowEnum.SPLASH, false);
                                                             }));
                                                     });
                                                } else
                                                {
                                                    Debug.LogError("Unknown error: "+ err);
                                                }
                                                
                                            }
                                        );
        }

       
        private void SendSaveFirstRecord()
        {
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
            
                    _splashScreen.loadingText = "LOADING";
                    _networkManager.SaveFirstRecord(gameData.gameId,
                       Core.Instance.GetService<PlayerManager>().PlayerCar.model,
                       () =>
                       {
                           
                           Core.Instance.StartCor(
                               LoadGameScene("EntryPoint",
                               () => {
                                   Core.Instance.GetService<StatesManager>().SwitchState(StatesEnum.MAIN_MULTIPLAYER_WINDOW);
                                   //Core.Instance.GetService<UIManager>().ShowWindow(UIWindowEnum.SPLASH, false);
                               }));
                       },
                       (err) => {
                           if (err == "timeout")
                           {
                               _enternetWindow.ShowErrorSend(SendSaveFirstRecord, "Go to Menu",() => {

                                   Core.Instance.GetService<UIManager>().ShowWindow(UIWindowEnum.SPLASH, true);
                                   Core.Instance.StartCor(
                                       LoadGameScene("EntryPoint",
                                       () =>
                                       {
                                           Core.Instance.GetService<StatesManager>().SwitchState(StatesEnum.MAIN_MULTIPLAYER_WINDOW);
                                       }));
                               });

                           }
                           else
                               Debug.LogError("Error send first statistics"); }
                   );
        }

        private IEnumerator Wait(float time, Action onComplete)
        {
            //Debug.Log("Wait");
            yield return new WaitForSecondsRealtime(time);
            onComplete();
        }

        public void CreateGameHandler()
        {
            EventManager._init.Game.Invoke_InitAll();
            switch (gameData.GameType)
            {
                case GameTypeEnum.MultyReplay:
                    _handler = new ReplayHandler();
                    break;

                case GameTypeEnum.MultyJoin:
                    _handler = new ReplayHandler();
                    break;

                case GameTypeEnum.MultyNew:
                    _handler = new RecordHandler();
                    break;
                case GameTypeEnum.MultyNewFB:
                    _handler = new RecordHandler();
                    break;

                case GameTypeEnum.MultyRecord:
                    _handler = new RecordHandler();
                    break;

                case GameTypeEnum.SingleNew:
                    _handler = new SingleRecordHandler();
                    break;

                case GameTypeEnum.TutorialGame:
                    _handler = new TutorialHandler();
                    break;


                default:
                    _handler = null;
                    #region DEBUG
#if UNITY_EDITOR
                    Debug.Log("[ERROR] wrong GameTypeEnum = " + gameData.GameType + " !");
#endif
                    #endregion
                    break;
            }

            _handler.Init(gameData);
        }

        public void LoadTrack(Action OnComplete)
        {


            if (string.IsNullOrEmpty(gameData.track_id))
            {
                gameData.track_id = "0";
                Debug.Log("IsNullOrEmpty gameData.track_id");

            }

            Core.Instance.StartCor(LoadGameScene(
                TrackNamesById[int.Parse(gameData.track_id)],
                OnComplete));
        }

        public void Restart(Action OnComplete)
        {
            EventManager._init.Game.TrackEvent.GetEvent.Invoke_Restart();
            LoadTrack(OnComplete);
        }

        public IEnumerator LoadGameScene(string sceneName, Action OnComplete)
        {
            //Debug.Log("LoadGameScene");

            var async = SceneManager.LoadSceneAsync(sceneName);
            yield return async;


            //Debug.Log("LoadGameScene completed");
            OnComplete();
        }

        //Loaded game
        public GameData GetGameDataByID(string gameId)
        {
            var gData = Games.Find(g => g.gameId == gameId);

            if (gData == null)
            {
                gData = CreateGameData();
                gData.gameId = gameId;
            }

            return gData;
        }

        public void Destroy()
        {
            if(_handler != null)
            _handler.Destroy();
            _handler = null;
            gameData = null;
        }

        // ===========================================================

        #region SINGLE_NEW_GAME

        private void CreateNewSingleplayerGame()
        {
            gameData = CreateSingleplayerGameData();
            _mainScreenStateManager.SwitchState(MainScreenStatesEnum.CHOOSE_TRACK);
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
        }

        private GameData CreateSingleplayerGameData()
        {
            GameData gd = new GameData();
            gd.gameId = "new";
            gd.GameType = GameTypeEnum.SingleNew;
            gd.player1_Id = _playerManager.PlayerId;
            gd.player1_name = _playerManager.PlayerDisplayName;

            if (_playerManager.selectedCar == null)
            {
                if (_playerManager.playerCars[0] == null)
                {
                    throw new Exception();
                }

                _playerManager.selectedCar = _playerManager.playerCars[0];
            }

            gd.player1_car = _playerManager.selectedCar;
            Games.Add(gd);

            return gd;
        }

        private void StartSingleplayerGame(string gameId)
        {
            gameData = GetGameDataByID(gameId);
            Assert.AreNotEqual(null, gameData);

            _statesManager.SwitchState(StatesEnum.GAME_SINGLE);
        }



        #endregion

        #region TUTORIAL GAME

        private void StartTutorialGame(string gameId)
        {
            gameData = GetGameDataByID(gameId);
            Assert.AreNotEqual(null, gameData);
            CreateGameHandler();

        }

        #endregion

    }
    public struct TrackTimePrice
    {
        public int Time;
        public int PriceGold;
        public int PriceJewels;
        public string nameTrack;
    }


}