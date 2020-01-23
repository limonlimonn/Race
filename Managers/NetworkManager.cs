using UnityEngine;
using System;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Messages;
using GameSparks.Api;
using GameSparks.Api.Responses;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine.Assertions;

using System.IO;
using System.IO.Compression;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;

using HCR.Enums;
using HCR.Interfaces;
using HCR.GlobalWindow.MainMenu;
using UnityEngine.SceneManagement;
using HCR.Network;
using System.Globalization;

namespace HCR
{
    /// <summary>
    /// Класс - менеджер для связи с сервером "GameSparks" (через ACTIONS)
    /// </summary>

    public class NetworkManager : IService
    {
        // FIELDS

        private string _registrationTime;

        // dependences
        private PlayerManager _playerManager;
        private FacebookConnectRequest FC;
        private StampTimerManager _stampTimerManager;
        private SafePlayerPrefs _safePlayerPrefs;
        private UIManager _UImanager;
        private NotificationWindow _notificationWindow;
        private JObject tutorialUpgrades;
        private JObject tutorialCars;
        public JObject tutorialPlayerData;
        private StatisticsService _statisticsService;
        private GameManager _gameManager;
        private EventService _eventService;
        private AudioService _audioService;
        private EnternetWindow _enternetWindow;
        private ParsManager pars;

        public bool IsSaveGame = false;

        // METHODS

        public void Init()
        {
            _playerManager = Core.Instance.GetService<PlayerManager>();
            _stampTimerManager = Core.Instance.GetService<StampTimerManager>();
            _safePlayerPrefs = Core.Instance.GetService<SafePlayerPrefs>();
            _UImanager = Core.Instance.GetService<UIManager>();
            _notificationWindow = _UImanager.NotificationWindow;
            _statisticsService = Core.Instance.GetService<StatisticsService>();
            _gameManager = Core.Instance.GetService<GameManager>();
            _eventService = Core.Instance.GetService<EventService>();
            _audioService = Core.Instance.GetService<AudioService>();
            pars = new ParsManager();
        }

        private string GetRegistrationTime()
        {
            if (string.IsNullOrEmpty(_registrationTime))
            {
                _registrationTime = TimeZone.CurrentTimeZone.ToString();
            }
            return _registrationTime;
        }

        #region Auth and Registration

        public void AuthOrRegWithFB(string AuthToken, Action<string> OnAuthError, Action<string> OnAuthSuccess)
        {
            FC = new FacebookConnectRequest();
            FC.SetAccessToken(AuthToken);


            FC.SetDoNotLinkToCurrentPlayer(false);
            FC.Send((response) =>
            {
                if (!response.HasErrors)
                {
                    string authToken = response.AuthToken;
                    GSData response_data = response.ScriptData.GetGSData("response_Data");
                    var respString = response.ScriptData.JSON;
                    JObject respObj = JObject.Parse(respString);
                    JObject playerData = null;
                    playerData = respObj["response_Data"];
                    JObject colorData = null;
                    colorData = respObj["response_Data"];
                    JObject levelData = null;
                    levelData = respObj["response_Data"];
                    JObject tracksData = null;
                    tracksData = respObj["response_Data"];
                    JObject leaderData = null;
                    leaderData = respObj["response_Data"];
                    JObject shopData = null;
                    shopData = respObj["response_Data"];
                    JObject eventData = null;
                    eventData = respObj["response_Data"];
                    registerPushNotifications();
                    GetAllCars(() =>
                    {
                        LoadUserFromObjData(playerData["player_Data"]);
                        LoadColorFromObjData(colorData["colors"]);
                        LoadLevelFromObjData(levelData["xp_levels"]);
                        LoadTracksFromObjData(tracksData["trakcs"]);
                        LoadLeaderData(leaderData["tracks_leader"]);
                        LoadShop(shopData["shop"]);
                        LoadEvent(eventData["EventData"]);

                        OnAuthSuccess("Auth Success");
                    }, () =>
                    {
                        OnAuthError("Error Loading Cars Data...");
                    });

                }
                else
                {
                    Debug.LogError("Error Authenticating Player...");
                    OnAuthError("Error Authenticating Player...");
                }
            });
        }

        public void Authentication(string name, string pass, Action<string, string> OnAuthError, Action<string> OnAuthSuccess)
        {
            Debug.Log("Authentication " + name+ "  " + pass);
            new GameSparks.Api.Requests.AuthenticationRequest()
                .SetUserName(name)
                .SetPassword(pass)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {

                        Debug.Log("Authentication  OK ");
                        GSData response_data = response.ScriptData.GetGSData("response_Data");
                        var respString = response.ScriptData.JSON;
                        JObject respObj = JObject.Parse(respString);
                        JObject playerData = null;
                        playerData = respObj["response_Data"];
                        JObject colorData = null;
                        colorData = respObj["response_Data"];
                        JObject levelData = null;
                        levelData = respObj["response_Data"];
                        JObject tracksData = null;
                        tracksData = respObj["response_Data"];
                        JObject leaderData = null;
                        leaderData = respObj["response_Data"];
                        JObject eventData = null;
                        eventData = respObj["response_Data"];
                        JObject shopData = null;
                        shopData = respObj["response_Data"];
                        registerPushNotifications();
                        Debug.Log("Authentication  OK 2 ");
                        SavePlayerCredantials(name, pass);



                        if (_safePlayerPrefs.IsFinishGame())
                        {


                            var _sPP = _safePlayerPrefs;

                            string id = _sPP.LoadFinishGame();
                            _sPP.FillGameData(id);
                            SendSaveRecord(id, _sPP.GetCar(id),() =>
                             {
                                 Debug.Log("Duo Authentication");
                                 Authentication(name, pass, OnAuthError, OnAuthSuccess);
                             }, () => { Debug.Log("DeleteGame Error2"); _sPP.DeleteGame(id); });
                        }
                        else
                        {

                            GetAllCars(() =>
                            {
                                //LoadUser(OnAuthError, response);
                                LoadUserFromObjData(playerData["player_Data"]);
                                LoadColorFromObjData(colorData["colors"]);
                                LoadLevelFromObjData(levelData["xp_levels"]);
                                LoadTracksFromObjData(tracksData["trakcs"]);
                                LoadLeaderData(leaderData["tracks_leader"]);
                                LoadShop(shopData["shop"]);
                                LoadEvent(eventData["EventData"]);

                                _safePlayerPrefs.SavePlayerName(name);
                                _safePlayerPrefs.SavePlayerPass(pass);
                                Debug.Log("no errors in auth");
                                OnAuthSuccess("Auth Success");
                            }, () =>
                            {
                                OnAuthError("Error Loading Cars Data...", "");

                            });
                        }
                    }
                    else
                    {
                        var error = response.Errors.JSON;
                        JObject errorObj = JObject.Parse(error);
                        Debug.Log("errorObj" + errorObj["error"]);                       
                        //Debug.LogError("Error Authenticating Player...");
                        OnAuthError("Error Authenticating Player...", errorObj["error"]);
                    }
                });
        }

        public void Registration(string name, string displayName, string age, string pass, string male, Action<string, string> OnAuthError, Action<string> OnRegistrationSuccess)
        {
            var timeZone = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalHours.ToString();
            GSRequestData segments = new GSRequestData();
            segments.Add("UserAge", age);
            segments.Add("UserMale", male);
            segments.Add("UTC", timeZone);




            new GameSparks.Api.Requests.RegistrationRequest()
                .SetUserName(name)
                .SetPassword(pass)
                .SetDisplayName(displayName)
                .SetSegments(segments)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        SavePlayerCredantials(name, pass);

                        GSData response_data = response.ScriptData.GetGSData("response_Data");
                        var respString = response.ScriptData.JSON;
                        JObject respObj = JObject.Parse(respString);
                        JObject playerData = null;
                        playerData = respObj["response_Data"];
                        JObject colorData = null;
                        colorData = respObj["response_Data"];
                        JObject levelData = null;
                        levelData = respObj["response_Data"];
                        JObject tracksData = null;
                        tracksData = respObj["response_Data"];
                        JObject leaderData = null;
                        leaderData = respObj["response_Data"];
                        JObject shopData = null;
                        shopData = respObj["response_Data"];
                        JObject eventData = null;
                        eventData = respObj["response_Data"];

                        registerPushNotifications();


                        GetAllCars(() =>
                        {
                            LoadUserFromObjData(playerData["player_Data"]);
                            LoadColorFromObjData(colorData["colors"]);
                            LoadLevelFromObjData(levelData["xp_levels"]);
                            LoadTracksFromObjData(tracksData["trakcs"]);
                            LoadLeaderData(leaderData["tracks_leader"]);
                            LoadShop(shopData["shop"]);
                            LoadEvent(eventData["EventData"]);
                            _safePlayerPrefs.SavePlayerName(name);
                            _safePlayerPrefs.SavePlayerPass(pass);
                            OnRegistrationSuccess("Registration success");
                        }, () =>
                        {
                            OnAuthError("Error Loading Cars Data...","");
                        });


                    }
                    else
                    {
                        var Error = response.Errors.JSON;
                        JObject errorObj = JObject.Parse(Error);
                        Debug.Log(errorObj["error"]);



                        string error = "";
                        for (int i = 1; i < response.Errors.JSON.Length - 1; i++)
                        {
                            error += response.Errors.JSON[i].ToString();
                        }
                        if (error == "\"USERNAME\":\"TAKEN\"")
                            OnAuthError("Error : User Is Present", errorObj["error"]);
                        else OnAuthError("Error Registration", errorObj["error"]);
                    }
                });



        }

        public int MaxBuggy = 0;
        public int MaxRally = 0;
        public int MaxJeep = 0;

        public void GetMessages(ScriptMessage message)
        {


            if (message.ExtCode == "challengeAnswer")
            {
                GSData data = message.Data;

                _notificationWindow.ShowGameAnswer(data.GetString("body"));
                
                _UImanager.UpdateMyGamesCounter();
            }
        }



        public void registerPushNotifications() { }

        /*
        #region PushNotifications

        public void registerPushNotifications()
        {

            Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
            dependencyStatus = Firebase.FirebaseApp.CheckDependencies();
            if (dependencyStatus != Firebase.DependencyStatus.Available)
            {
                Firebase.FirebaseApp.FixDependenciesAsync().ContinueWith(task =>
                {
                    dependencyStatus = Firebase.FirebaseApp.CheckDependencies();
                    if (dependencyStatus == Firebase.DependencyStatus.Available)
                    {
                        InitializeFirebase();
                    }
                    else
                    {
                        Debug.LogError(
                            "Could not resolve all Firebase dependencies: " + dependencyStatus);
                    }
                });
            }
            else
            {
                InitializeFirebase();
            }
        }

        void InitializeFirebase()
        {

            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.Subscribe("topik");

        }

        public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        {

            Debug.Log("OnTokenReceived");
            new PushRegistrationRequest()
               .SetDeviceOS("FCM")
               .SetPushId(token.Token)
               .Send((response) =>
               {
                   Debug.Log("PushRegistrationRequest");
                   string registrationId = response.RegistrationId;
                   Debug.Log("Registration id: " + registrationId);
                   GSData scriptData = response.ScriptData;
               });

            //GUILayout.TextField(token.Token);
        }

        public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            Debug.Log("Received a new message");
            var notification = e.Message.Notification;
            if (notification != null)
            {
                Debug.Log("title: " + notification.Title);
                Debug.Log("body: " + notification.Body);
            }
            if (e.Message.From.Length > 0)
                Debug.Log("from: " + e.Message.From);
            if (e.Message.Data.Count > 0)
            {
                Debug.Log("data:");
                foreach (System.Collections.Generic.KeyValuePair<string, string> iter in
                         e.Message.Data)
                {
                    Debug.Log("  " + iter.Key + ": " + iter.Value);
                }
            }
        }

        #endregion
        */

        public void LoadColorFromObjData(JObject colorData)
        {

            if (colorData != null)
            {
                CarColor col = null;
                for (int i = 0; i < colorData.Count; i++)
                {
                    col = new CarColor()
                    {
                        ID = int.Parse(colorData[i]["id"]),
                        hex = colorData[i]["value"],
                        price = int.Parse(colorData[i]["gold"])
                    };
                    _playerManager.allColors.Add(col);
                }
            }
            else
            {
                Debug.LogError("Error Loading colorData From JObject...");
            }
        }

        public void LoadLevelFromObjData(JObject levelData)
        {
            if (levelData != null)
            {
                LevelsXP level = null;

                for (int i = 0; i < levelData.Count; i++)
                {
                    level = new LevelsXP()
                    {
                        ID = int.Parse(levelData[i]["id"]),
                        maxValue = int.Parse(levelData[i]["value"])
                    };
                    _playerManager.allLevelsXP.Add(level);

                }
            }
            else
            {
                Debug.LogError("Error Loading levelData From JObject...");
            }

        }

        public void LoadTracksFromObjData(JObject trackData)
        {
            if (trackData != null)
            {
                Debug.Log("ALLLLL");
                TrackTimePrice Setting;
                for (int i = 0; i < trackData.Count; i++)
                {

                    _gameManager.TrackNamesById.Add(i, trackData[i]["scene"]);

                    Setting = new TrackTimePrice()
                    {


                        PriceGold = int.Parse(trackData[i]["gold"]),
                        PriceJewels = int.Parse(trackData[i]["jewels"]),
                        Time = int.Parse(trackData[i]["time"]),
                        nameTrack = trackData[i]["name"],


                    };
                    _gameManager.TrackTimeById.Add(Setting);
                }
                _gameManager.TrackNamesById.Add(99, "Tutorial1");
                _gameManager.TrackNamesById.Add(100, "Tutorial2");
                _gameManager.tracksSettingsLoaded();

            }
            else
            {
                Debug.LogError("Error Loading trackData From JObject...");
            }
        }

        public void LoadUserStatistic(JObject respObj)
        {

            _playerManager.AverageStars =   pars.ToFloat(respObj["game"]["averageStars"]);
            _playerManager.Win =            int.Parse(respObj["game"]["Win"]);
            _playerManager.Draw =           int.Parse(respObj["game"]["Draw"]);
            _playerManager.InGame =         int.Parse(respObj["game"]["InGame"]);
            _playerManager.Losing =         int.Parse(respObj["game"]["Losing"]);
            _playerManager.MaxXP =          int.Parse(respObj["xp_money_inTrack"]["xp"]["max"]);
            _playerManager.MaxGold =        int.Parse(respObj["xp_money_inTrack"]["gold"]["max"]);
            _playerManager.MaxJewels =      int.Parse(respObj["xp_money_inTrack"]["jewels"]["max"]);
            _playerManager.AverageXP =      pars.ToFloat(respObj["xp_money_inTrack"]["xp"]["average"]);
            _playerManager.TotalGold =      int.Parse(respObj["xp_money_inTrack"]["gold"]["total"]);
            _playerManager.TotalJewels =    int.Parse(respObj["xp_money_inTrack"]["jewels"]["total"]);
            _playerManager.MaxBalance =     pars.ToFloat(respObj["tricks"]["balance"]["max"]);
            _playerManager.MaxBlunt =       pars.ToFloat(respObj["tricks"]["Blunt"]["max"]);
            _playerManager.MaxFlip =        int.Parse(respObj["tricks"]["flip"]["max"]);
            _playerManager.MaxInAir =       pars.ToFloat(respObj["tricks"]["InAir"]["max"]);
            _playerManager.TotalBalance =   pars.ToFloat(respObj["tricks"]["balance"]["total"]);
            _playerManager.TotalBlunt =     int.Parse(respObj["tricks"]["Blunt"]["total"]);
            _playerManager.TotalFlip =      int.Parse(respObj["tricks"]["flip"]["total"]);
            _playerManager.TotalInAir =     pars.ToFloat(respObj["tricks"]["InAir"]["total"]);

        }

        public void LoadLeaderData(JObject leaderData)
        {
            _playerManager.LeaderBoard = new Dictionary<int, Dictionary<string, PlayerManager.Leader>>();
            PlayerManager.Leader leader;
            string[] item = new string[] { "first", "second", "third" };
            Dictionary<string, PlayerManager.Leader> numLeader;
            //Debug.Log("count " + leaderData.Count);
            for (int i = 0; i < leaderData.Count; i++)
            {
                numLeader = new Dictionary<string, PlayerManager.Leader>();

                for (int j = 0; j < 3; j++)
                {

                    leader = new PlayerManager.Leader();
                    if (leaderData[i][item[j]]["display_name"] == null)
                    {
                        leader.name = "Null";
                        leader.time = 0;
                    }
                    else
                    {
                        leader.name = leaderData[i][item[j]]["display_name"];
                        leader.time =
                            int.Parse(leaderData[i][item[j]]["time"]);
                    }
                    numLeader.Add(item[j], leader);
                    //Debug.Log(item[j] + " "+"leader " + leader.name + " "+ leader.time);
                }
                _playerManager.LeaderBoard.Add(i, numLeader);
            }
        }

        public void LoadEvent(JObject eventData)
        {

            Debug.Log("LoadEvent");
            _eventService.Events.Clear();
            _eventService.Events = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> Event;

            JObject Day = null;
            Day = eventData["Daily"];
            JObject Week = null;
            Week = eventData["Week"];
            //_______DAILY_________


            for (int i = 0; i < Day["DayEventData"]["countEvent"].Count; i++)
            {
                Event = new Dictionary<string, string>();
                Event.Add("description", Day["DayEventData"]["countEvent"][i]["description"]);
                Event.Add("status", eventData["UserDaily"][i][0]); // status
                Event.Add("Complete", eventData["UserDaily"][i][1]); // Complete
                Event.Add("collect", eventData["UserDaily"][i][2]);//collect
                Event.Add("end_stamp", Day["end_stamp"]);
                Event.Add("now_stamp", Day["now_stamp"]);
                Event.Add("gold", Day["DayEventData"]["countEvent"][i]["collect"]["gold"]);
                Event.Add("jewel", Day["DayEventData"]["countEvent"][i]["collect"]["jewel"]);

                _eventService.Events.Add(Day["DayEventData"]["type"] + i.ToString(), Event);
                /*
                foreach (var item in Event)
                {
                    Debug.Log("key : " + item.Key + "item : " + item.Value);
                }*/
            }

            //___________

            Event = new Dictionary<string, string>();
            //___WEEK___
            Event.Add("description", Week["WeekEventData"]["description"]);
            Event.Add("status", eventData["UserWeek"][0]);      // status
            Event.Add("Complete", eventData["UserWeek"][1]);      // Complete
            Event.Add("collect", eventData["UserWeek"][2]);//collect
            Event.Add("end_stamp", Week["end_stamp"]);
            Event.Add("now_stamp", Week["now_stamp"]);
            Event.Add("gold", Week["WeekEventData"]["collect"]["gold"]);
            Event.Add("jewel", Week["WeekEventData"]["collect"]["jewel"]);

            _eventService.Events.Add(Week["WeekEventData"]["type"], Event);


            _eventService.Init();




        }

        public void LoadShop(JObject shopData)
        {
            _eventService.Shop = new Dictionary<int, Dictionary<string, int>>();

            for (int i = 0; i < shopData.Count; i++)
            {
                Dictionary<string, int> Price = new Dictionary<string, int>();
                Price.Add("price", int.Parse(shopData[i]["price"]));
                Price.Add("gold", int.Parse(shopData[i]["gold"]));
                Price.Add("jewels", int.Parse(shopData[i]["jewels"]));
                _eventService.Shop.Add(i, Price);
            }
        }

        public void LoadUserFromObjData(JObject playerData)
        {

            if (playerData != null)
            {
                //if user data exists, start receive messages
                ScriptMessage.Listener += GetMessages;

                _playerManager.Init(playerData["id"], playerData["displayName"], int.Parse(playerData["is_tutorial"]));

                _stampTimerManager.X2StartStamp = int.Parse(playerData["x2_start_stamp"]);
                _stampTimerManager.X2EndStamp = int.Parse(playerData["x2_end_stamp"]);
                _stampTimerManager.X3StartStamp = int.Parse(playerData["x3_start_stamp"]);
                _stampTimerManager.X3EndStamp = int.Parse(playerData["x3_end_stamp"]);
                _stampTimerManager.X4StartStamp = int.Parse(playerData["x4_start_stamp"]);
                _stampTimerManager.X4EndStamp = int.Parse(playerData["x4_end_stamp"]);
                _stampTimerManager.nowStamp = int.Parse(playerData["now_stamp"]);

                _stampTimerManager.Init();

                JObject respObj = playerData;
                _playerManager.now_stamp = int.Parse(playerData["now_stamp"]);
                _playerManager.accountTime = int.Parse(respObj["timeAccount"]);
                _playerManager.level = int.Parse(respObj["level"]);
                Debug.Log("_playerManager.level  " + _playerManager.level);
                _playerManager.xp = int.Parse(respObj["xp"]);
                _playerManager.accountType = int.Parse(respObj["accountType"]);
                _playerManager.jewels = int.Parse(respObj["jewels"]);
                _playerManager.gold = int.Parse(respObj["gold"]);
                _playerManager.IsOnRandom = bool.Parse(respObj["randomBonus"]["IsOn"]);
                _playerManager.stampRandom = int.Parse(respObj["randomBonus"]["stamp"]);
                _playerManager.stamp_now_user = int.Parse(respObj["stamp_now_user"]);



                _playerManager.TrackBestTime.Clear();
                _playerManager.TrackOpen.Clear();
                for (int i = 0; i < respObj["open_tracks"].Count; i++)
                {
                    _playerManager.TrackOpen.Add(int.Parse(respObj["open_tracks"][i]));
                }
                for (int i = 0; i < respObj["best_time_track"].Count; i++)
                {
                    _playerManager.TrackBestTime.Add(int.Parse(respObj["best_time_track"][i]));
                }

                LoadUserStatistic(respObj);




                JObject curCar = null;
                curCar = respObj["current_car"];
                if (curCar != null)
                {
                    CarModel currentCar = null;
                    currentCar = new CarModel()
                    {
                        type = int.Parse(curCar["type"]),
                        level = int.Parse(curCar["level"])
                    };
                    _playerManager.SetCurrentCur(currentCar);

                }

                if (_playerManager.isTutorial == 1 && _playerManager.tutorialStep < 3)
                {
                    tutorialPlayerData = playerData;

                    CarModel currentCar = null;
                    currentCar = new CarModel()
                    {
                        type = 2,
                        level = 0
                    };
                    _playerManager.SetCurrentCur(currentCar);
                }


                JObject carsData = null;
                carsData = respObj["bought_cars"];
                //Debug.LogError(carsData);
                if (_playerManager.isTutorial == 1 && _playerManager.tutorialStep < 3)
                {
                    carsData = tutorialCars;

                }

                PlayerCarModel pcm = null;
                for (int i = 0; i < carsData.Count; i++)
                {



                    JObject colData = null;
                    colData = carsData[i]["bought_colors"];
                    if (_playerManager.isTutorial == 1 && _playerManager.tutorialStep < 3)
                    {

                        pcm = new PlayerCarModel()
                        {
                            type = int.Parse(carsData[i]["type"]),
                            carLevel = int.Parse(carsData[i]["level"]),
                            current_color = 0,
                        };
                    }
                    else
                    {
                        pcm = new PlayerCarModel()
                        {
                            type = int.Parse(carsData[i]["type"]),
                            carLevel = int.Parse(carsData[i]["level"]),
                            current_color = int.Parse(carsData[i]["current_color"]),
                        };
                    }



                    pcm.Set(colData);
                    _playerManager.playerCars.Add(pcm);
                }

                JObject upsData = null;
                upsData = respObj["bought_upgrades"];
                if (_playerManager.isTutorial == 1 && _playerManager.tutorialStep < 3)
                {
                    upsData = tutorialUpgrades;

                }

                if (upsData != null)
                {
                    //Debug.LogError(upsData);
                    for (int i = 0; i < upsData.Count; i++)
                    {
                        pcm = _playerManager.playerCars.Find(c => c.carLevel == int.Parse(upsData[i]["carLevel"]) &&
                                                                  c.type == int.Parse(upsData[i]["carType"]));
                        var up_type = (UpgradeType)Enum.Parse(typeof(UpgradeType), upsData[i]["upgradeType"]);
                        if (!pcm.installedUpgrades.ContainsKey(up_type))
                        {
                            pcm.installedUpgrades.Add(up_type, int.Parse(upsData[i]["upgradeOrder"]));
                        }
                        else
                        {
                            pcm.installedUpgrades[up_type] = int.Parse(upsData[i]["upgradeOrder"]);
                        }
                    }
                }

                for (int i = 0; i < _playerManager.playerCars.Count; i++)
                {
                    _playerManager.playerCars[i].car_upgrade_level = CheckForCarPrefabUpgrade(_playerManager.playerCars[i],
                        DataModel.Instance.GetUpgradesByCar(_playerManager.playerCars[i].carType,
                            _playerManager.playerCars[i].carLevel));
                    //Debug.LogError(_pm.playerCars[i]);

                    switch (_playerManager.playerCars[i].carType)
                    {
                        case CarTypeEnum.Baggy:
                            MaxBuggy = GetOpenedCar(_playerManager.playerCars[i].carType, _playerManager.playerCars[i].carLevel);
                            //Debug.LogError(MaxBuggy);
                            break;
                        case CarTypeEnum.Rally:
                            MaxRally = GetOpenedCar(_playerManager.playerCars[i].carType, _playerManager.playerCars[i].carLevel);
                            // Debug.LogError(MaxRally);
                            break;
                        case CarTypeEnum.Jeep:
                            MaxJeep = GetOpenedCar(_playerManager.playerCars[i].carType, _playerManager.playerCars[i].carLevel);
                            //Debug.LogError(MaxJeep);
                            break;
                    }

                    foreach (var car in DataModel.Instance.carsModels)
                    {
                        if (car.level == (_playerManager.playerCars[i].carLevel + 1) &&
                            car.CarType == _playerManager.playerCars[i].carType && car != null)
                        {
                            if (_playerManager.playerCars[i] != null && _playerManager.playerCars[i].installedUpgrades.Count > 0)
                            {
                                switch (_playerManager.playerCars[i].carType)
                                {
                                    case CarTypeEnum.Baggy:
                                        if (_playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Tires) &&
                                            _playerManager.playerCars[i].installedUpgrades[UpgradeType.Tires] == MaxBuggy ||
                                            _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Nitro) &&
                                            _playerManager.playerCars[i].installedUpgrades[UpgradeType.Nitro] == MaxBuggy ||
                                            _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Motor) &&
                                            _playerManager.playerCars[i].installedUpgrades[UpgradeType.Motor] == MaxBuggy ||
                                            _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Gearbox) &&
                                            _playerManager.playerCars[i].installedUpgrades[UpgradeType.Gearbox] == MaxBuggy ||
                                            _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Chassis) &&
                                            _playerManager.playerCars[i].installedUpgrades[UpgradeType.Chassis] == MaxBuggy)
                                        {
                                            DataModel.Instance.openedCars.Add(car);
                                        }

                                        break;
                                    case CarTypeEnum.Jeep:
                                        if (_playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Tires) &&
                                            _playerManager.playerCars[i].installedUpgrades[UpgradeType.Tires] == MaxJeep ||
                                            _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Nitro) &&
                                            _playerManager.playerCars[i].installedUpgrades[UpgradeType.Nitro] == MaxJeep ||
                                            _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Motor) &&
                                            _playerManager.playerCars[i].installedUpgrades[UpgradeType.Motor] == MaxJeep ||
                                            _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Gearbox) && _playerManager
                                                .playerCars[i]
                                                .installedUpgrades[UpgradeType.Gearbox] == MaxJeep ||
                                            _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Chassis) && _playerManager
                                                .playerCars[i]
                                                .installedUpgrades[UpgradeType.Chassis] == MaxJeep)
                                        {
                                            DataModel.Instance.openedCars.Add(car);
                                        }

                                        break;
                                    case CarTypeEnum.Rally:
                                        if (_playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Tires) &&
                                            _playerManager.playerCars[i].installedUpgrades[UpgradeType.Tires] == MaxRally ||
                                            _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Nitro) &&
                                            _playerManager.playerCars[i].installedUpgrades[UpgradeType.Nitro] == MaxRally ||
                                            _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Motor) &&
                                            _playerManager.playerCars[i].installedUpgrades[UpgradeType.Motor] == MaxRally ||
                                            _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Gearbox) &&
                                            _playerManager.playerCars[i].installedUpgrades[UpgradeType.Gearbox] == MaxRally ||
                                            _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Chassis) &&
                                            _playerManager.playerCars[i].installedUpgrades[UpgradeType.Chassis] == MaxRally)
                                        {
                                            DataModel.Instance.openedCars.Add(car);
                                        }

                                        break;
                                }
                            }
                        }
                        else
                        {
                            car.isOpened = false;
                        }

                        if (car.level == 0)
                        {
                            DataModel.Instance.openedCars.Add(car);
                        }
                    }
                }

                if (_playerManager.playerCars.Count == 0)
                {
                    foreach (var car in DataModel.Instance.carsModels)
                    {
                        if (car.level == 0)
                        {
                            DataModel.Instance.openedCars.Add(car);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Error Loading Player Data From JObject...");
            }

        }

        /*

        public void LoadUser(Action<string> OnAuthError, GameSparks.Api.Responses.AuthenticationResponse resp)
        {
            DataModel.Instance.openedCars.Clear();
            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("LOAD_USER")
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        GSData data = response.ScriptData.GetGSData("user_Data");
                        if (data != null)
                        {
                            _playerManager.Init(resp.UserId, resp.DisplayName, resp["is_tutorial"]));

                            var respString = response.ScriptData.JSON;

                            JObject respObj = JObject.Parse(respString);

                            _playerManager.level = int.Parse(respObj["user_Data"]["level"]);
                            _playerManager.xp = int.Parse(respObj["user_Data"]["xp"]);
                            _playerManager.accountType = int.Parse(respObj["user_Data"]["accountType"]);
                            _playerManager.silver = int.Parse(respObj["user_Data"]["silver"]);
                            _playerManager.gold = int.Parse(respObj["user_Data"]["gold"]);

                            JObject curCar = null;
                            curCar = respObj["user_Data"]["current_car"];
                            if (curCar != null)
                            {
                                CarModel currentCar = null;
                                currentCar = new CarModel()
                                {
                                    type = int.Parse(curCar["type"]),
                                    level = int.Parse(curCar["level"])
                                };
                                _playerManager.SetCurrentCur(currentCar);

                            }

                            JObject carsData = null;
                            carsData = respObj["user_Data"]["bought_cars"];
                            //Debug.LogError(carsData);

                            PlayerCarModel pcm = null;
                            for (int i = 0; i < carsData.Count; i++)
                            {
                                JObject colData = null;
                                colData = carsData[i]["bought_colors"];
                                pcm = new PlayerCarModel()
                                {
                                    type = int.Parse(carsData[i]["type"]),
                                    carLevel = int.Parse(carsData[i]["level"]),
                                    current_color = int.Parse(carsData[i]["current_color"]),
                                };

                                pcm.Set(colData);
                                _playerManager.playerCars.Add(pcm);
                            }

                            JObject upsData = null;
                            upsData = respObj["user_Data"]["bought_upgrades"];
                            if (upsData != null)
                            {
                                //Debug.LogError(upsData);
                                for (int i = 0; i < upsData.Count; i++)
                                {
                                    pcm = _playerManager.playerCars.Find(c => c.carLevel == int.Parse(upsData[i]["carLevel"]) &&
                                                                              c.type == int.Parse(upsData[i]["carType"]));
                                    var up_type = (UpgradeType)Enum.Parse(typeof(UpgradeType), upsData[i]["upgradeType"]);
                                    if (!pcm.installedUpgrades.ContainsKey(up_type))
                                    {
                                        pcm.installedUpgrades.Add(up_type, int.Parse(upsData[i]["upgradeOrder"]));
                                    }
                                    else
                                    {
                                        pcm.installedUpgrades[up_type] = int.Parse(upsData[i]["upgradeOrder"]);
                                    }
                                }
                            }

                            for (int i = 0; i < _playerManager.playerCars.Count; i++)
                            {
                                _playerManager.playerCars[i].car_upgrade_level = CheckForCarPrefabUpgrade(_playerManager.playerCars[i],
                                    DataModel.Instance.GetUpgradesByCar(_playerManager.playerCars[i].carType,
                                        _playerManager.playerCars[i].carLevel));
                                //Debug.LogError(_pm.playerCars[i]);

                                switch (_playerManager.playerCars[i].carType)
                                {
                                    case CarTypeEnum.Baggy:
                                        MaxBuggy = GetOpenedCar(_playerManager.playerCars[i].carType, _playerManager.playerCars[i].carLevel);
                                        //Debug.LogError(MaxBuggy);
                                        break;
                                    case CarTypeEnum.Rally:
                                        MaxRally = GetOpenedCar(_playerManager.playerCars[i].carType, _playerManager.playerCars[i].carLevel);
                                        // Debug.LogError(MaxRally);
                                        break;
                                    case CarTypeEnum.Jeep:
                                        MaxJeep = GetOpenedCar(_playerManager.playerCars[i].carType, _playerManager.playerCars[i].carLevel);
                                        //Debug.LogError(MaxJeep);
                                        break;
                                }

                                foreach (var car in DataModel.Instance.carsModels)
                                {
                                    if (car.level == (_playerManager.playerCars[i].carLevel + 1) &&
                                        car.CarType == _playerManager.playerCars[i].carType && car != null)
                                    {
                                        if (_playerManager.playerCars[i] != null && _playerManager.playerCars[i].installedUpgrades.Count > 0)
                                        {
                                            switch (_playerManager.playerCars[i].carType)
                                            {
                                                case CarTypeEnum.Baggy:
                                                    if (_playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Tires) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Tires] == MaxBuggy ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Nitro) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Nitro] == MaxBuggy ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Motor) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Motor] == MaxBuggy ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Gearbox) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Gearbox] == MaxBuggy ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Chassis) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Chassis] == MaxBuggy)
                                                    {
                                                        DataModel.Instance.openedCars.Add(car);
                                                    }

                                                    break;
                                                case CarTypeEnum.Jeep:
                                                    if (_playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Tires) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Tires] == MaxJeep ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Nitro) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Nitro] == MaxJeep ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Motor) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Motor] == MaxJeep ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Gearbox) && _playerManager
                                                            .playerCars[i]
                                                            .installedUpgrades[UpgradeType.Gearbox] == MaxJeep ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Chassis) && _playerManager
                                                            .playerCars[i]
                                                            .installedUpgrades[UpgradeType.Chassis] == MaxJeep)
                                                    {
                                                        DataModel.Instance.openedCars.Add(car);
                                                    }

                                                    break;
                                                case CarTypeEnum.Rally:
                                                    if (_playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Tires) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Tires] == MaxRally ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Nitro) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Nitro] == MaxRally ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Motor) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Motor] == MaxRally ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Gearbox) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Gearbox] == MaxRally ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Chassis) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Chassis] == MaxRally)
                                                    {
                                                        DataModel.Instance.openedCars.Add(car);
                                                    }

                                                    break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        car.isOpened = false;
                                    }

                                    if (car.level == 0)
                                    {
                                        DataModel.Instance.openedCars.Add(car);
                                    }
                                }
                            }

                            if (_playerManager.playerCars.Count == 0)
                            {
                                foreach (var car in DataModel.Instance.carsModels)
                                {
                                    if (car.level == 0)
                                    {
                                        DataModel.Instance.openedCars.Add(car);
                                    }
                                }
                            }
                        }
                        else
                        {
                            OnAuthError("Error Loading Player Data...");
                        }
                    }
                    else
                    {
                        OnAuthError("Error Loading Player Data...");
                    }
                });
        }

        void LoadUserAfterRegistration(Action<string> OnAuthError, GameSparks.Api.Responses.RegistrationResponse resp)
        {
            DataModel.Instance.openedCars.Clear();
            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("LOAD_USER")
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        GSData data = response.ScriptData.GetGSData("user_Data");
                        if (data != null)
                        {
                            _playerManager.Init(resp.UserId, resp.DisplayName, int.Parse(playerData["is_tutorial"]));

                            var respString = response.ScriptData.JSON;

                            JObject respObj = JObject.Parse(respString);

                            _playerManager.level = int.Parse(respObj["user_Data"]["level"]);
                            _playerManager.xp = int.Parse(respObj["user_Data"]["xp"]);
                            _playerManager.accountType = int.Parse(respObj["user_Data"]["accountType"]);
                            _playerManager.silver = int.Parse(respObj["user_Data"]["silver"]);
                            _playerManager.gold = int.Parse(respObj["user_Data"]["gold"]);

                            JObject curCar = null;
                            curCar = respObj["user_Data"]["current_car"];
                            if (curCar != null)
                            {
                                CarModel currentCar = null;
                                currentCar = new CarModel()
                                {
                                    type = int.Parse(curCar["type"]),
                                    level = int.Parse(curCar["level"])
                                };
                                _playerManager.SetCurrentCur(currentCar);

                            }

                            JObject carsData = null;
                            carsData = respObj["user_Data"]["bought_cars"];
                            //Debug.LogError(carsData);

                            PlayerCarModel pcm = null;
                            for (int i = 0; i < carsData.Count; i++)
                            {
                                JObject colData = null;
                                colData = carsData[i]["bought_colors"];
                                pcm = new PlayerCarModel()
                                {
                                    type = int.Parse(carsData[i]["type"]),
                                    carLevel = int.Parse(carsData[i]["level"]),
                                    current_color = int.Parse(carsData[i]["current_color"]),
                                };

                                pcm.Set(colData);
                                _playerManager.playerCars.Add(pcm);
                            }

                            JObject upsData = null;
                            upsData = respObj["user_Data"]["bought_upgrades"];
                            if (upsData != null)
                            {
                                //Debug.LogError(upsData);
                                for (int i = 0; i < upsData.Count; i++)
                                {
                                    pcm = _playerManager.playerCars.Find(c => c.carLevel == int.Parse(upsData[i]["carLevel"]) &&
                                                                              c.type == int.Parse(upsData[i]["carType"]));
                                    var up_type = (UpgradeType)Enum.Parse(typeof(UpgradeType), upsData[i]["upgradeType"]);
                                    if (!pcm.installedUpgrades.ContainsKey(up_type))
                                    {
                                        pcm.installedUpgrades.Add(up_type, int.Parse(upsData[i]["upgradeOrder"]));
                                    }
                                    else
                                    {
                                        pcm.installedUpgrades[up_type] = int.Parse(upsData[i]["upgradeOrder"]);
                                    }
                                }
                            }

                            for (int i = 0; i < _playerManager.playerCars.Count; i++)
                            {
                                _playerManager.playerCars[i].car_upgrade_level = CheckForCarPrefabUpgrade(_playerManager.playerCars[i],
                                    DataModel.Instance.GetUpgradesByCar(_playerManager.playerCars[i].carType,
                                        _playerManager.playerCars[i].carLevel));
                                Debug.LogError(_playerManager.playerCars[i]);

                                switch (_playerManager.playerCars[i].carType)
                                {
                                    case CarTypeEnum.Baggy:
                                        MaxBuggy = GetOpenedCar(_playerManager.playerCars[i].carType, _playerManager.playerCars[i].carLevel);
                                        Debug.LogError(MaxBuggy);
                                        break;
                                    case CarTypeEnum.Rally:
                                        MaxRally = GetOpenedCar(_playerManager.playerCars[i].carType, _playerManager.playerCars[i].carLevel);
                                        Debug.LogError(MaxRally);
                                        break;
                                    case CarTypeEnum.Jeep:
                                        MaxJeep = GetOpenedCar(_playerManager.playerCars[i].carType, _playerManager.playerCars[i].carLevel);
                                        Debug.LogError(MaxJeep);
                                        break;
                                }

                                foreach (var car in DataModel.Instance.carsModels)
                                {
                                    if (car.level == (_playerManager.playerCars[i].carLevel + 1) &&
                                        car.CarType == _playerManager.playerCars[i].carType && car != null)
                                    {
                                        if (_playerManager.playerCars[i] != null && _playerManager.playerCars[i].installedUpgrades.Count > 0)
                                        {
                                            switch (_playerManager.playerCars[i].carType)
                                            {
                                                case CarTypeEnum.Baggy:
                                                    if (_playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Tires) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Tires] == MaxBuggy ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Nitro) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Nitro] == MaxBuggy ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Motor) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Motor] == MaxBuggy ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Gearbox) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Gearbox] == MaxBuggy ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Chassis) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Chassis] == MaxBuggy)
                                                    {
                                                        DataModel.Instance.openedCars.Add(car);
                                                    }

                                                    break;
                                                case CarTypeEnum.Jeep:
                                                    if (_playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Tires) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Tires] == MaxJeep ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Nitro) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Nitro] == MaxJeep ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Motor) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Motor] == MaxJeep ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Gearbox) && _playerManager
                                                            .playerCars[i]
                                                            .installedUpgrades[UpgradeType.Gearbox] == MaxJeep ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Chassis) && _playerManager
                                                            .playerCars[i]
                                                            .installedUpgrades[UpgradeType.Chassis] == MaxJeep)
                                                    {
                                                        DataModel.Instance.openedCars.Add(car);
                                                    }

                                                    break;
                                                case CarTypeEnum.Rally:
                                                    if (_playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Tires) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Tires] == MaxRally ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Nitro) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Nitro] == MaxRally ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Motor) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Motor] == MaxRally ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Gearbox) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Gearbox] == MaxRally ||
                                                        _playerManager.playerCars[i].installedUpgrades.ContainsKey(UpgradeType.Chassis) &&
                                                        _playerManager.playerCars[i].installedUpgrades[UpgradeType.Chassis] == MaxRally)
                                                    {
                                                        DataModel.Instance.openedCars.Add(car);
                                                    }

                                                    break;
                                            }

                                        }

                                    }
                                    else
                                    {
                                        car.isOpened = false;
                                    }

                                    if (car.level == 0)
                                    {
                                        DataModel.Instance.openedCars.Add(car);
                                    }
                                }
                            }

                            if (_playerManager.playerCars.Count == 0)
                            {
                                foreach (var car in DataModel.Instance.carsModels)
                                {
                                    if (car.level == 0)
                                    {
                                        DataModel.Instance.openedCars.Add(car);
                                    }
                                }
                            }
                        }
                        else
                        {
                            OnAuthError("Error Loading Player Data...");
                        }
                    }
                    else
                    {
                        OnAuthError("Error Loading Player Data...");
                    }
                });

        }

           */

        public int GetOpenedCar(CarTypeEnum type, int carlevel)
        {
            List<UpgradeItem> counts = new List<UpgradeItem>();
            var list = DataModel.Instance.upgrades;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].CarType == type && list[i].carLevel == carlevel)
                    counts.Add(list[i]);
            }
            int max = 0;
            for (int i = 0; i < counts.Count; i++)
            {
                max = Mathf.Max(max, counts[i].upgradeOrder);
            }

            return max;
        }

        public int CheckForCarPrefabUpgrade(PlayerCarModel pcm, List<UpgradeItem> car_ups)
        {
            Dictionary<int, int> ups_CarList = new Dictionary<int, int>();
            List<UpgradeType> typesList = new List<UpgradeType>();
            List<UpgradeType> CartypesList = new List<UpgradeType>() {
                UpgradeType.Motor,
                UpgradeType.Gearbox,
                UpgradeType.Chassis,
                UpgradeType.Tires,
                UpgradeType.Nitro,
        };

            car_ups = DataModel.Instance.GetUpgradesByCar(pcm.carType, pcm.carLevel);

            foreach (var value in Enum.GetValues(typeof(UpgradeType)))
            {
                if (car_ups.Find(up => up.UpgradeType == (UpgradeType)value) != null)
                {
                    typesList.Add(((UpgradeType)value));
                }
            }


            if (pcm.installedUpgrades != null && pcm.installedUpgrades.Count > 0)
            {
                int newUpLevel = int.MaxValue;
                int UpLevel = int.MaxValue;
                foreach (var t in typesList)
                {
                    ups_CarList.Clear();
                    int MaxUpdate = 0;

                    foreach (var ups in car_ups)
                    {

                        if (t.ToString() == CartypesList[ups.upgradeType].ToString())
                        {

                            ups_CarList.Add(MaxUpdate, ups.upgradeLevel);


                            MaxUpdate++;
                        }
                    }

                    var itemsOfType = car_ups.FindAll(u => u.UpgradeType == t);
                    if (itemsOfType == null && itemsOfType.Count == 0)
                        continue;
                    if (!pcm.installedUpgrades.ContainsKey(t))
                    {

                        return 0;
                    }

                    else
                    {
                        if (itemsOfType.Find(u => u.upgradeLevel == pcm.car_upgrade_level && u.upgradeOrder > pcm.installedUpgrades[t]) ==
                            null)
                        {
                            foreach (var ups in car_ups)
                            {



                                if (t.ToString() == CartypesList[ups.upgradeType].ToString())
                                {
                                    if ((MaxUpdate - 1) == pcm.installedUpgrades[t])
                                    {
                                        newUpLevel = 3;
                                        UpLevel = Math.Min(newUpLevel, UpLevel);
                                    }
                                    else if (ups_CarList[pcm.installedUpgrades[t]] != ups_CarList[pcm.installedUpgrades[t] + 1])
                                    {
                                        newUpLevel = ups_CarList[pcm.installedUpgrades[t]];
                                        newUpLevel++;
                                        UpLevel = Math.Min(newUpLevel, UpLevel);
                                    }
                                    else if (ups_CarList[pcm.installedUpgrades[t]] == ups_CarList[pcm.installedUpgrades[t] + 1])
                                    {
                                        newUpLevel = ups_CarList[pcm.installedUpgrades[t]];
                                        UpLevel = Math.Min(newUpLevel, UpLevel);
                                    }
                                    ups_CarList.Clear();
                                    break;

                                }

                            }




                        }
                        else
                        {
                            UpLevel = 0; //Mathf.Min(newUpLevel, pcm.installedUpgrades[t]);
                            continue;
                        }
                    }
                }

                if (UpLevel >= pcm.car_upgrade_level)
                {
                    return UpLevel;
                }
                return 0;
            }
            return 0;
        }

        void SaveStatistics(Action<string> OnAuthError, Action OnComplete)
        {
            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("STATISTICS")
                .SetEventAttribute("LAST_LOGIN_TIME", DateTime.Now.ToString())
                .SetEventAttribute("REGISTRATION_TIME", _registrationTime)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        OnComplete();
                    }
                    else
                    {
                        OnAuthError("Error Saving Player Data...");
                    }
                });
        }

        void SavePlayerCredantials(string name, string pass)
        {
            // TODO --- delete PlayerPrefs data !!!

            var savedName = PlayerPrefs.GetString(PPKeys.name);
            if (string.IsNullOrEmpty(name) || savedName != name)
            {
                PlayerPrefs.SetString(PPKeys.name, name);
                PlayerPrefs.SetString(PPKeys.pass, pass);
                PlayerPrefs.Save();
            }
        }

        #endregion

        #region Games

        public void Join_Play(string gameId, PlayerCarModel car, Action OnComplete, Action<string> OnError)
        {
            GSRequestData jsonData = new GSRequestData();
            jsonData.AddString("color_id", car.current_color.ToString());
            jsonData.AddString("carType", car.type.ToString());
            jsonData.AddString("carLevel", car.carLevel.ToString());
            jsonData.AddString("upgradeLevel", car.car_upgrade_level.ToString());

            new GameSparks.Api.Requests.LogEventRequest()
                    .SetEventKey("JOIN_PLAY")
                    .SetEventAttribute("GAME_ID", gameId)
                    .SetEventAttribute("CAR", jsonData)
                    .Send((response) =>
                    {
                        if (!response.HasErrors)
                            OnComplete();
                        else
                        {
                            Debug.Log("Error Saving Player Data..." + response.Errors);
                            var error = response.Errors.JSON;
                            JObject errorObj = JObject.Parse(error);
                            Debug.Log(errorObj["error"]);
                            OnError(errorObj["error"]);
                        }
                    });
        }
        
        private void CheckIsSaveGame(Action<string> Error, Action OnComplete )
        {
            

            if (IsSaveGame)
            {
                if (_safePlayerPrefs.IsFinishGame())
                {

                    


                    
                    var _sPP = _safePlayerPrefs;

                    string id = _sPP.LoadFinishGame();
                    _sPP.FillGameData(id);
                   
                            IsSaveGame = false;
                            SaveRecord(id, _sPP.GetCar(id), OnComplete, (err) => {
                                if (err == "timeout")
                                {
                                    
                                   // Debug.Log("IsFinishGame() timeout");
                                   // _UImanager.ShowWindow(UIWindowEnum.SPLASH, false);
                                //    _enternetWindow = Core.Instance.GetService<UIManager>().GetWindow////(UIWindowEnum.IS_ENTERNET) as EnternetWindow;
                                  //  _enternetWindow.ShowErrorEnternet();
                                    
                                    IsSaveGame = true;
                                    Error("timeout");
                                    


                                }
                                else
                                {
                                    Debug.LogError("DeleteGame Error1");
                                    
                                    _sPP.DeleteGame(id);
                                    OnComplete();
                                }
                            });
                }
                else
                {
                    OnComplete();
                }
            }
            else
            {
                
                OnComplete();
            }
        }
        
        public void LoadGames(Action<List<GameData>> OnGamesLoaded, Action<string> OnErrorSave, Action <string>ErrorLoad = null)
        {

            CheckIsSaveGame(OnErrorSave, () => { 

            new LogEventRequest()
                .SetEventKey("LOAD_GAMES")
                .Send((response) =>
                {
                if (!response.HasErrors)
                {
                        
                        

                            var respString = response.ScriptData.JSON;
                            JObject respObj = JObject.Parse(respString);

                            // -------------------------------------------------------------
                            //
                            var gamesData = respObj["response_Data"]["player_games"];
                            Assert.AreNotEqual(null, gamesData);

                            var cur_player_id = respObj["response_Data"]["current_player_id"];
                            var games = new List<GameData>();

                            for (int i = 0; i < gamesData.Count; i++)
                            {
                                var car1_data = gamesData[i]["player1_car"];
                                var car2_data = gamesData[i]["player2_car"];



                                games.Add(new GameData()
                                {
                                    gameId = gamesData[i]["_id"]["$oid"].Value,

                                    track_id = gamesData[i]["track_id"],

                                    player1_Id = gamesData[i]["player1"],

                                    player1_car = new PlayerCarModel()
                                    {
                                        type = int.Parse(car1_data["carType"]),
                                        carLevel = int.Parse(car1_data["carLevel"]),
                                        car_upgrade_level = int.Parse(car1_data["upgradeLevel"]),
                                        current_color = int.Parse(car1_data["color_id"])
                                    },
                                    player1_name = gamesData[i]["player1_name"],
                                    player1_score = int.Parse(gamesData[i]["player1_score"]),

                                    player2_id = gamesData[i]["player2"],
                                    player2_car = new PlayerCarModel()
                                    {
                                        type = int.Parse(car2_data["carType"]),
                                        carLevel = int.Parse(car2_data["carLevel"]),
                                        car_upgrade_level = int.Parse(car2_data["upgradeLevel"]),
                                        current_color = int.Parse(car2_data["color_id"])
                                    },
                                    player2_name = gamesData[i]["player2_name"],

                                    player2_score = int.Parse(gamesData[i]["player2_score"]),


                                    last_player = gamesData[i]["last_player"],
                                    last_update = gamesData[i]["last_update"],
                                    replay_id = gamesData[i]["replay_id"],
                                    IsFinishedEnemy = int.Parse(gamesData[i]["is_finished"]),

                                    // message smiles
                                    smiles = new List<int>()
                                {
                                    gamesData[i]["message"][0].AsInt,
                                    gamesData[i]["message"][1].AsInt,
                                    gamesData[i]["message"][2].AsInt
                                },

                                    GameType = gamesData[i]["status"].Value == "onplay" 
                                        ? GetGameTypeByStatus(gamesData[i]["last_player"], cur_player_id)
                                        : GameTypeEnum.MultyWait
                                });


                            }


                            // -------------------------------------------------------------
                            //
                            var recentData = respObj["response_Data"]["recent_games"];
                            Assert.AreNotEqual(null, recentData);

                            for (int i = 0; i < recentData.Count; i++)
                            {
                                var car1_data = recentData[i]["player1_car"];


                                games.Add(new GameData()
                                {
                                    gameId = recentData[i]["_id"]["$oid"].Value,
                                    track_id = recentData[i]["track_id"],
                                    player1_Id = recentData[i]["player1"],

                                    player1_car = new PlayerCarModel()
                                    {

                                        type = int.Parse(car1_data["carType"]),
                                        carLevel = int.Parse(car1_data["carLevel"]),
                                        car_upgrade_level = int.Parse(car1_data["upgradeLevel"]),
                                        current_color = int.Parse(car1_data["color_id"])
                                    },

                                    player1_name = recentData[i]["player1_name"],
                                    last_player = recentData[i]["last_player"],
                                    last_update = recentData[i]["last_update"],
                                    replay_id = recentData[i]["replay_id"],
                                    IsFinishedEnemy = int.Parse(recentData[i]["is_finished"]),
                                    // message smiles
                                    smiles = new List<int>()
                                {
                                    recentData[i]["message"][0].AsInt,
                                    recentData[i]["message"][1].AsInt,
                                    recentData[i]["message"][2].AsInt
                                },

                                    GameType = GameTypeEnum.MultyJoin

                                });

                            }

                        // -------------------------------------------------------------
                        OnGamesLoaded(games);
                        
                    }
                    else
                    {
                        
                        Debug.LogError("Error Loading games list...");
                        var error = response.Errors.JSON;
                        JObject errorObj = JObject.Parse(error);
                        Debug.Log(errorObj["error"]);
                        ErrorLoad(errorObj["error"]);
                        var games = new List<GameData>();
                        OnGamesLoaded(games);
                    }
                });

            });

        }

        GameTypeEnum GetGameTypeByStatus(string last_player, string current_player)
        {
            if (current_player == last_player)
            {
                return GameTypeEnum.MultyRecord;
            }
            else
            {
                return GameTypeEnum.MultyReplay;
            }
        }

        public void FindOpenGame(Action<GameData> OnFreeGameFinded, Action<string> OnError)
        {
            CheckIsSaveGame(OnError, () =>
            {
                new LogEventRequest()
                .SetEventKey("FIND_OPEN_GAME")
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        GSData response_data = response.ScriptData.GetGSData("response_Data");
                        var respString = response.ScriptData.JSON;
                        JObject respObj = JObject.Parse(respString);
                        JObject gamesData = null;
                        gamesData = respObj["response_Data"]["finded_game"];

                        if (gamesData.ToString() == "\"null\"")
                        {
                            OnError("");
                        }
                        else
                        {
                            string finded_game_id = gamesData["_id"]["$oid"].Value;
                            LoadReplay(finded_game_id, () =>
                            {
                                var gm = Core.Instance.GetService<GameManager>();
                                var gData = gm.GetGameDataByID(finded_game_id);

                                gData.player1_Id = gamesData["player1"].Value;
                                gData.player1_name = gamesData["player1_name"].Value;

                                var car_data = gamesData["player1_car"];
                                gData.player1_car = new PlayerCarModel();
                                gData.player1_car.type = int.Parse(gamesData["player1_car"]["carType"]);
                                gData.player1_car.carLevel = int.Parse(gamesData["player1_car"]["carLevel"]);
                                gData.player1_car.car_upgrade_level = int.Parse(gamesData["player1_car"]["upgradeLevel"]);
                                gData.player1_car.current_color = int.Parse(gamesData["player1_car"]["color_id"]);


                                gData.track_id = gamesData["track_id"].Value;
                                gData.last_player = gamesData["last_player"].Value;

                                OnFreeGameFinded(gData);


                            }, OnError);
                        }
                    }
                    else
                    {
                        Debug.Log("Error Loading Player Data...");
                        var error = response.Errors.JSON;
                        JObject errorObj = JObject.Parse(error);
                        Debug.Log(errorObj["error"]);
                        OnError(errorObj["error"]);


                    }
                });

            });
        }

        public void LoadReplay(string gameId, Action OnComplete, Action<string> OnError)
        {
            Debug.Log("LoadReplay");
            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("GET_REPLAY_BY_ID")
                .SetEventAttribute("GAME_ID", gameId)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        GSData response_data = response.ScriptData.GetGSData("response_Data");
                        string replay_data = response_data.GetString("replay_Data");
                        Debug.LogError("replay_data " + replay_data);
                        if (replay_data == null)
                        {
                            Debug.LogError("Error loading replay...");
                            OnError("");
                        }
                        else
                        {
                            var gm = Core.Instance.GetService<GameManager>();
                            GameData gData = gm.GetGameDataByID(gameId);
                            gData.replayData = UnzipString(replay_data);
                            gData.GameType = GameTypeEnum.MultyReplay;
                            OnComplete();

                        }
                    }
                    else
                    {
                        Debug.LogError("Error loading replay...");
                        var error = response.Errors.JSON;
                        JObject errorObj = JObject.Parse(error);
                        Debug.Log(errorObj["error"]);
                        OnError(errorObj["error"]);
                    }
                });
        }

        public void LoadReplayMostRecentGame(string gameId, Action OnComplete, Action OnError)
        {
            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("GET_REPLAY_BY_ID")
                .SetEventAttribute("GAME_ID", gameId)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        GSData response_data = response.ScriptData.GetGSData("response_Data");
                        string replay_data = response_data.GetString("replay_Data");
                        if (replay_data == null)
                        {
                            Debug.LogError("Error loading replay...");
                            OnError();
                        }
                        else
                        {
                            var gm = Core.Instance.GetService<GameManager>();
                            var gData = gm.GetGameDataByID(gameId);
                            gData.replayData = replay_data;
                            gData.GameType = GameTypeEnum.MultyReplay;
                            OnComplete();

                        }
                    }
                    else
                    {
                        Debug.LogError("Error loading replay...");
                        OnError();
                    }
                });
        }

        public void SaveFirstRecord(string gameId, PlayerCarModel pcm, Action OnComplete, Action<string> OnError)
        {

            //Debug.Log("SaveFirstRecord : ");
            GSRequestData jsonData = new GSRequestData();
            jsonData.AddString("color_id", pcm.current_color.ToString());
            jsonData.AddString("carType", pcm.type.ToString());
            jsonData.AddString("carLevel", pcm.carLevel.ToString());
            jsonData.AddString("upgradeLevel", pcm.car_upgrade_level.ToString());
            #region Statistic

            GSRequestData StatisticData = new GSRequestData();
            foreach (var item in _statisticsService.StatisticRecord)
            {
                StatisticData.Add(item.Key, item.Value);
            }
            foreach (var record in StatisticData.BaseData)
            {
                Debug.Log("StatisticData  " + record.Key + " " + record.Value);
            }

            GSRequestData UserStatistic = new GSRequestData();
            foreach (var item in _statisticsService.UserStatistic)
            {
                UserStatistic.Add(item.Key, item.Value);
            }
            foreach (var record in UserStatistic.BaseData)
            {
                Debug.Log("UserStatistic  " + record.Key + " " + record.Value);
            }
            



            #endregion

            var gm = Core.Instance.GetService<GameManager>();
            var data = gm.GetGameDataByID(gameId);
            if (data.recordData != null)
            {

                Debug.Log("data.recordData  " + data.recordData);




                GSRequestData smiles = new GSRequestData();

                smiles.AddNumberList("MESSAGE", data.smiles);

                Debug.LogError("Full record length: " + data.recordData.Length);
                var compressReplayData = ZipString(data.recordData);
                Debug.LogError("Compress record length: " + compressReplayData.Length);

                new GameSparks.Api.Requests.LogEventRequest()
                    .SetEventKey("INSERT_FIRST_REPLAY")
                    .SetEventAttribute("REPLAY_DATA", compressReplayData )
                    .SetEventAttribute("TRACKID", data.track_id)
                    .SetEventAttribute("CAR", jsonData)
                    .SetEventAttribute("MSG", smiles.GetIntList("MESSAGE"))
                    .SetEventAttribute("IS_FINISHED", data.IsFinishedRecord)
                    .SetEventAttribute("IS_DONE", data.IsDone)
                    .SetEventAttribute("USER_STATISTIC", UserStatistic)
                    .SetEventAttribute("STAT_RECORD", StatisticData)
                    .Send((response) =>
                    {
                        if (!response.HasErrors)
                        {
                            GSData response_data = response.ScriptData.GetGSData("response_Data");
                            var respString = response.ScriptData.JSON;
                            JObject respObj = JObject.Parse(respString);
                            JObject playerData = null;
                            JObject leaderData = null;
                            JObject EventData = null;
                            playerData = respObj["response_Data"];
                            leaderData = playerData["leader_tracks"];
                            respObj = playerData["player_Data"];
                            EventData = playerData["EventData"];
                            if (playerData == null)
                            {
                                Debug.LogError("Error loading player Data...");
                                OnError("");
                            }
                            else
                            {

                                _playerManager.level = int.Parse(playerData["player_Data"]["level"]);
                                _playerManager.xp = int.Parse(playerData["player_Data"]["xp"]);
                                _playerManager.jewels = int.Parse(playerData["player_Data"]["jewels"]);
                                _playerManager.gold = int.Parse(playerData["player_Data"]["gold"]);
                                // Debug.LogError(playerData);
                                _playerManager.TrackBestTime.Clear();
                                _playerManager.TrackOpen.Clear();
                                _playerManager.IsDone = int.Parse(respObj["is_done"]);
                                for (int i = 0; i < respObj["open_tracks"].Count; i++)
                                {
                                    _playerManager.TrackOpen.Add(int.Parse(respObj["open_tracks"][i]));
                                }
                                for (int i = 0; i < respObj["best_time_track"].Count; i++)
                                {
                                    _playerManager.TrackBestTime.Add(int.Parse(respObj["best_time_track"][i]));
                                }
                                LoadLeaderData(leaderData);
                                LoadUserStatistic(respObj);
                                LoadEvent(EventData);

                            }

                            Debug.Log("Save new game succes...");
                            OnComplete();


                        }
                        else
                        {
                           
                            Debug.Log("Error Saving Player Data..." + "\n" );

                            var error = response.Errors.JSON;
                            JObject errorObj = JObject.Parse(error);
                            Debug.Log(errorObj["error"]);
                            OnError(errorObj["error"]);

                            
                        }
                    });
            }
            else
            {
                Debug.Log("Error Saving Player Data... data.recordData == null");
                OnError("");
            }
        }

        public void SaveFirstRecordForFB(string gameId, PlayerCarModel pcm, Action OnComplete, Action OnError)
        {
            GSRequestData jsonData = new GSRequestData();
            jsonData.AddString("color_id", pcm.current_color.ToString());
            jsonData.AddString("carType", pcm.type.ToString());
            jsonData.AddString("carLevel", pcm.carLevel.ToString());
            jsonData.AddString("upgradeLevel", pcm.car_upgrade_level.ToString());

            GSRequestData StatisticData = new GSRequestData();
            StatisticData.Add("type", _statisticsService.StatisticRecord["type"]);
            StatisticData.Add("track_id", _statisticsService.StatisticRecord["track_id"]);
            StatisticData.Add("player_id", _statisticsService.StatisticRecord["player_id"]);
            StatisticData.Add("player_name", _statisticsService.StatisticRecord["player_name"]);
            StatisticData.Add("tryes_count", _statisticsService.StatisticRecord["tryes_count"]);
            StatisticData.Add("car", _statisticsService.StatisticRecord["car"]);
            StatisticData.Add("time", _statisticsService.StatisticRecord["time"]);
            StatisticData.Add("IsFinished", _statisticsService.StatisticRecord["IsFinished"]);
            if (_statisticsService.StatisticRecord["IsFinished"] != "1")
            {
                StatisticData.Add("CrashX", _statisticsService.StatisticRecord["CrashX"]);
            }

            GSRequestData UserStatistic = new GSRequestData();
            UserStatistic.Add("flip", _statisticsService.UserStatistic["flip"]);
            UserStatistic.Add("balance", _statisticsService.UserStatistic["balance"]);
            UserStatistic.Add("Blunt", _statisticsService.UserStatistic["Blunt"]);
            UserStatistic.Add("inAir", _statisticsService.UserStatistic["inAir"]);
            UserStatistic.Add("maxFlip", _statisticsService.UserStatistic["maxFlip"]);
            UserStatistic.Add("maxBalance", _statisticsService.UserStatistic["maxBalance"]);
            UserStatistic.Add("maxBlunt", _statisticsService.UserStatistic["maxBlunt"]);
            UserStatistic.Add("maxInAir", _statisticsService.UserStatistic["maxInAir"]);
            UserStatistic.Add("maxXP", _statisticsService.UserStatistic["maxXP"]);
            _statisticsService.NewCreateUserStatistic(); // ТАК НАДО!!

            var gm = Core.Instance.GetService<GameManager>();
            var data = gm.GetGameDataByID(gameId);
            if (data.recordData != null)
            {
                GSRequestData smiles = new GSRequestData();

                smiles.AddNumberList("MESSAGE", data.smiles);

                Debug.LogError("Full record length: " + data.recordData.Length);
                var compressReplayData = ZipString(data.recordData);
                Debug.LogError("Compress record length: " + compressReplayData.Length);

                new GameSparks.Api.Requests.LogEventRequest()
                    .SetEventKey("INSERT_FIRST_REPLAY_FOR_FB")
                    .SetEventAttribute("STAT_RECORD", StatisticData)
                    .SetEventAttribute("REPLAY_DATA", compressReplayData )
                    .SetEventAttribute("TRACKID", data.track_id)
                    .SetEventAttribute("PLAYER2_ID", data.player2_id)
                    .SetEventAttribute("PLAYER2_NAME", data.player2_name)
                    .SetEventAttribute("CAR", jsonData)
                    .SetEventAttribute("USER_STATISTIC", UserStatistic)
                    .SetEventAttribute("MSG", smiles.GetIntList("MESSAGE"))
                    .SetEventAttribute("IS_FINISHED", data.IsFinishedRecord)
                    .Send((response) =>
                    {

                        if (!response.HasErrors)
                        {
                            GSData response_data = response.ScriptData.GetGSData("response_Data");
                            var respString = response.ScriptData.JSON;
                            JObject respObj = JObject.Parse(respString);
                            JObject playerData = null;
                            playerData = respObj["response_Data"];

                            if (playerData == null)
                            {
                                Debug.LogError("Error loading player Data...");
                                OnError();
                            }
                            else
                            {
                                _playerManager.level = int.Parse(playerData["player_Data"]["level"]);
                                Debug.Log("_playerManager.level  " + _playerManager.level);
                                _playerManager.xp = int.Parse(playerData["player_Data"]["xp"]);
                                _playerManager.jewels = int.Parse(playerData["player_Data"]["jewels"]);
                                _playerManager.gold = int.Parse(playerData["player_Data"]["gold"]);
                                // Debug.LogError(playerData);

                                _playerManager.TrackBestTime.Clear();
                                _playerManager.TrackOpen.Clear();
                                for (int i = 0; i < respObj["open_tracks"].Count; i++)
                                {
                                    _playerManager.TrackOpen.Add(int.Parse(respObj["open_tracks"][i]));
                                }
                                for (int i = 0; i < respObj["best_time_track"].Count; i++)
                                {
                                    _playerManager.TrackBestTime.Add(int.Parse(respObj["best_time_track"][i]));
                                }
                                LoadUserStatistic(respObj);
                            }

                            Debug.Log("Save new FB game succes...");
                            OnComplete();


                        }
                        else
                        {
                            Debug.Log("Error Saving FB Player Data...");
                            OnError();
                        }
                    });
            }
            else
            {
                Debug.Log("Error Saving FB Player Data...");
                OnError();
            }
        }

        public void SendSaveRecord(string gameId, PlayerCarModel pcm, Action OnComplete, Action OnError = null)
        {
            GSRequestData jsonData = new GSRequestData();
            jsonData.AddString("color_id", pcm.current_color.ToString());
            jsonData.AddString("carType", pcm.type.ToString());
            jsonData.AddString("carLevel", pcm.carLevel.ToString());
            jsonData.AddString("upgradeLevel", pcm.car_upgrade_level.ToString());
            //
            foreach (var record in jsonData.BaseData)
            {
                Debug.Log("jsonData  " + record.Key + " " + record.Value);
            }
            //
            GSRequestData StatisticDataRecord = new GSRequestData();
            foreach (var record in _statisticsService.StatisticRecord)
            {
                StatisticDataRecord.Add(record.Key, record.Value);
            }
            //
            foreach (var record in StatisticDataRecord.BaseData)
            {
                Debug.Log("StatisticDataRecord  " + record.Key + " " + record.Value);
            }
            //
            GSRequestData UserStatistic = new GSRequestData();
            foreach (var item in _statisticsService.UserStatistic)
            {
                UserStatistic.Add(item.Key, item.Value);
            }
            _statisticsService.NewCreateUserStatistic(); // ТАК НАДО!!
            foreach (var record in UserStatistic.BaseData)
            {
                Debug.Log("UserStatistic  " + record.Key + " " + record.Value);
            }


            GSRequestData StatisticDataReplay = new GSRequestData();
            foreach (var replay in _statisticsService.StatisticReplay)
            {
                StatisticDataReplay.Add(replay.Key, replay.Value);
            }

            foreach (var record in StatisticDataReplay.BaseData)
            {
                Debug.Log("StatisticDataReplay  " + record.Key + " " + record.Value);
            }

           

            //-------
            var gm = Core.Instance.GetService<GameManager>();
            GameData data = new GameData();

            Debug.Log("Current Game_id" + gameId);
            foreach (var game in gm.Games)
            {
                Debug.Log("game-id" + game.gameId);
                if (game.recordData != null)
                {
                    Debug.LogError("Send record length" + game.recordData.Length);
                }
                else
                {
                    Debug.Log("record null");
                }



                // if (gameData == null)
                data = gm.GetGameDataByID(gameId);
                // else  data = gameData;
                

                Debug.Log("GOLD" + _safePlayerPrefs.GetInt_EarnedGold());

                if (data.recordData != null)
                {
                    Debug.LogError("Full record length: " + data.recordData.Length);
                    var compressReplayData = ZipString(data.recordData);
                    Debug.LogError("Compress record length: " + compressReplayData.Length);

                    GSRequestData smiles = new GSRequestData();
                    smiles.AddNumberList("MESSAGE", data.smiles);
                    new GameSparks.Api.Requests.LogEventRequest()
                        .SetMaxResponseTimeInMillis(15000)
                        .SetEventKey("UPDATE_REPLAY")
                        .SetEventAttribute("REPLAY_DATA", compressReplayData)
                        .SetEventAttribute("STAT_RECORD", StatisticDataRecord)
                        .SetEventAttribute("STAT_REPLAY", StatisticDataReplay)
                        .SetEventAttribute("USER_STATISTIC", UserStatistic)
                        .SetEventAttribute("GAME_ID", gameId)
                        .SetEventAttribute("TRACKID", data.track_id)
                        .SetEventAttribute("SCORE", data.gameResultScore)
                        .SetEventAttribute("XP", (int)_safePlayerPrefs.GetFloat_PlayerExp())
                        .SetEventAttribute("GOLD", _safePlayerPrefs.GetInt_PlayerGold())
                        .SetEventAttribute("SILVER", _safePlayerPrefs.GetInt_EarnedJewels())
                        .SetEventAttribute("JEWELS", _safePlayerPrefs.GetInt_EarnedJewels())
                        .SetEventAttribute("CAR", jsonData)
                        .SetEventAttribute("MSG", smiles.GetIntList("MESSAGE"))
                        .SetEventAttribute("IS_FINISHED", data.IsFinishedRecord)
                        .SetEventAttribute("IS_DONE", data.IsDone)
                        .SetEventAttribute("GOLD_EARNED", _safePlayerPrefs.GetGolgEarnedWithMultiplier())
                        .SetEventAttribute("MULTIPLIER", _safePlayerPrefs.GetMultiplier())
                        .Send((response) =>
                        {
                            Debug.Log("response");

                            if (!response.HasErrors)
                            {

                                _safePlayerPrefs.DeleteGame(gameId);
                                OnComplete();
                            }
                            else
                            {
                                
                                Debug.Log("Error Saving Player Data..." + "\n" );
                            //_safePlayerPrefs.SaveRecordData(data, pcm);
                            OnError();
                            }
                        });
                }
                else
                {
                    Debug.LogError("Error Send Player Data...");
                }

            }
        }

        public void SaveRecord(string gameId, PlayerCarModel pcm, Action OnComplete, Action<string> OnError)
        {
            #region DATA STATISTIC
            GSRequestData jsonData = new GSRequestData();
            jsonData.AddString("color_id", pcm.current_color.ToString());
            jsonData.AddString("carType", pcm.type.ToString());
            jsonData.AddString("carLevel", pcm.carLevel.ToString());
            jsonData.AddString("upgradeLevel", pcm.car_upgrade_level.ToString());
            //
            /*
            foreach (var record in jsonData.BaseData)
            {
                Debug.Log("jsonData  " + record.Key + " " + record.Value);
            }
            //
            */
            GSRequestData StatisticDataRecord = new GSRequestData();
            foreach (var record in _statisticsService.StatisticRecord)
            {
                StatisticDataRecord.Add(record.Key, record.Value);
            }
            //
            
            foreach (var record in StatisticDataRecord.BaseData)
            {
                Debug.Log("StatisticDataRecord  " + record.Key + " " + record.Value);
            }
            //
            
            GSRequestData UserStatistic = new GSRequestData();
            foreach(var item in _statisticsService.UserStatistic)
            {
                UserStatistic.Add(item.Key, item.Value);
            }
            _statisticsService.NewCreateUserStatistic(); // ТАК НАДО!!
            
            foreach (var record in UserStatistic.BaseData)
            {
                Debug.Log("UserStatistic  " + record.Key + " " + record.Value);
            }
            

            GSRequestData StatisticDataReplay = new GSRequestData();
            foreach(var replay in _statisticsService.StatisticReplay)
            {
                StatisticDataReplay.Add(replay.Key, replay.Value);
            }
            
            foreach (var record in StatisticDataReplay.BaseData)
            {
                Debug.Log("StatisticDataReplay  " + record.Key + " " + record.Value);
            }
            
            

            //-------
            var gm = Core.Instance.GetService<GameManager>();
            GameData data = new GameData();

            Debug.Log("Current Game_id" + gameId);
            foreach (var game in  gm.Games )
            {
                Debug.Log("game-id" + game.gameId);
                if (game.recordData != null) {
                    Debug.LogError("Save record length: " + game.recordData.Length);
                }else
                {
                    Debug.Log("record null");
                }
                
            }

           
            // if (gameData == null)
            data = gm.GetGameDataByID(gameId);
            // else  data = gameData;

           

            Debug.Log("GOLD" + _safePlayerPrefs.GetInt_EarnedGold());

            if (data.recordData != null)
            {
                Debug.LogError("Full record length: " + data.recordData.Length);
                var compressReplayData = ZipString(data.recordData);
                Debug.LogError("Compress record length: " + compressReplayData.Length);

                GSRequestData smiles = new GSRequestData();
                smiles.AddNumberList("MESSAGE", data.smiles);
                new GameSparks.Api.Requests.LogEventRequest()
                    .SetEventKey("UPDATE_REPLAY")
                    .SetMaxResponseTimeInMillis(15000)
                    .SetEventAttribute("REPLAY_DATA", compressReplayData )
                    .SetEventAttribute("STAT_RECORD", StatisticDataRecord)
                    .SetEventAttribute("STAT_REPLAY", StatisticDataReplay)
                    .SetEventAttribute("USER_STATISTIC", UserStatistic)
                    .SetEventAttribute("GAME_ID", gameId)
                    .SetEventAttribute("TRACKID", data.track_id)
                    .SetEventAttribute("SCORE", data.gameResultScore)
                    .SetEventAttribute("XP", (int)_safePlayerPrefs.GetFloat_PlayerExp())
                    .SetEventAttribute("GOLD",    _safePlayerPrefs.GetInt_PlayerGold())
                    .SetEventAttribute("SILVER",  _safePlayerPrefs.GetInt_EarnedJewels())
                    .SetEventAttribute("JEWELS",  _safePlayerPrefs.GetInt_EarnedJewels())
                    .SetEventAttribute("CAR", jsonData)
                    .SetEventAttribute("MSG", smiles.GetIntList("MESSAGE"))
                    .SetEventAttribute("IS_FINISHED", data.IsFinishedRecord)
                    .SetEventAttribute("IS_DONE", data.IsDone)
                    .SetEventAttribute("GOLD_EARNED", _safePlayerPrefs.GetGolgEarnedWithMultiplier())
                    .SetEventAttribute("MULTIPLIER", _safePlayerPrefs.GetMultiplier())

                    .Send((response) =>
                    {


                        if (!response.HasErrors)
                        {
                            
                            GSData response_data = response.ScriptData.GetGSData("response_Data");
                            var respString = response.ScriptData.JSON;
                            JObject respObj = JObject.Parse(respString);
                            
                            JObject playerData = null;
                            playerData = respObj["response_Data"];
                            respObj = playerData["player_Data"];
                            JObject EventData = null;
                            EventData = playerData["EventData"];
                            if (playerData == null)
                            {
                                Debug.LogError("Error loading player Data...");
                                OnError("");
                            }
                            else
                            {
                                _playerManager.level = int.Parse(playerData["player_Data"]["level"]);
                                _playerManager.xp = int.Parse(playerData["player_Data"]["xp"]);
                                _playerManager.jewels = int.Parse(playerData["player_Data"]["jewels"]);
                                _playerManager.gold = int.Parse(playerData["player_Data"]["gold"]);
                                _playerManager.TrackBestTime.Clear();
                                _playerManager.TrackOpen.Clear();
                                _playerManager.IsDone = int.Parse(respObj["is_done"]);
                                for (int i = 0; i < respObj["open_tracks"].Count; i++)
                                {
                                    _playerManager.TrackOpen.Add(int.Parse(respObj["open_tracks"][i]));
                                }
                                for (int i = 0; i < respObj["best_time_track"].Count; i++)
                                {
                                    _playerManager.TrackBestTime.Add(int.Parse(respObj["best_time_track"][i]));
                                }

                                LoadUserStatistic(respObj);
                                LoadEvent(EventData);
                            }
                            SaveGoldWithMultiplier(playerData);
                            Debug.Log("Replay update succes...");
                            _safePlayerPrefs.DeleteGame(gameId);
                            OnComplete();
                            

                        }
                        else
                        {
                            
                            Debug.Log("Error Saving Player Data..." + "\n" );

                            var error = response.Errors.JSON;
                            JObject errorObj = JObject.Parse(error);
                            Debug.Log(errorObj["error"]);
                            //_safePlayerPrefs.SaveRecordData(data, pcm);
                            OnError(errorObj["error"]);
                        }
                    });
            }
            else
            {
                Debug.Log("Data record == null");
                OnError("");

            }
        #endregion
        }

        

        public void BuyAttempt(int jewel, Action<int> OnResponse)
        {
            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("BUY_ATTEMPT")
                .SetEventAttribute("JEWELS", jewel.ToString())
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        GSData response_data = response.ScriptData.GetGSData("response_Data");
                        var respString = response.ScriptData.JSON;
                        JObject respObj = JObject.Parse(respString);
                        JObject playerData = null;
                        playerData = respObj["response_Data"];
                        JObject respObj1 = playerData["player_Data"];
                        if (int.Parse(respObj1["jewels"]) < _playerManager.jewels)
                        {
                            _playerManager.jewels = int.Parse(respObj1["jewels"]);
                            OnResponse(1);
                        }
                        else { OnResponse(0);  }

                    }
                    else
                    {
                        Debug.LogError("response.HasErrors in BuyInScore");
                        OnResponse(0);
                    }
                });
        }

        #endregion

        #region Garage

        public void BuyCar(CarTypeEnum car_type, int car_lvl,string BuyJewel, Action<string> OnResponse , Action<string> OnError)
        {
            Debug.Log("BuyJewel" + BuyJewel +" "+ car_lvl.ToString() + ((int)car_type).ToString());
            GSRequestData jsonData = new GSRequestData();
            jsonData.AddString("level", car_lvl.ToString());
            jsonData.AddString("type", ((int)car_type).ToString());
            

            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("BUY_CAR")
                .SetEventAttribute("JSN", jsonData)
                .SetEventAttribute("BUY_JEWEL", BuyJewel)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        GSData response_data = response.ScriptData.GetGSData("response_Data");
                        var respString = response.ScriptData.JSON;
                        JObject respObj = JObject.Parse(respString);
                        JObject playerData = null;
                        playerData = respObj["response_Data"];
                        JObject respObj1 = playerData["player_Data"];
                        _playerManager.jewels = int.Parse(respObj1["jewels"]);
                        _playerManager.gold = int.Parse(respObj1["gold"]);

                        OnResponse(GetErrorMessage(0));
                    }
                    else
                    {
                        var error = response.Errors.JSON;
                        JObject errorObj = JObject.Parse(error);
                        Debug.Log(errorObj["error"]);
                        OnError(errorObj["error"]);
                      
                    }
                });
        }

        public void BuyTrack(int track_id, Action<int> OnResponse)
        {


            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("BUY_TRACK")
                .SetEventAttribute("TRACK_ID", track_id)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        GSData response_data = response.ScriptData.GetGSData("response_Data");
                        var respString = response.ScriptData.JSON;
                        JObject respObj = JObject.Parse(respString);
                        JObject playerData = null;
                        playerData = respObj["response_Data"];
                        JObject respObj1 = playerData["player_Data"];
                        _playerManager.jewels = int.Parse(respObj1["jewels"]);
                        _playerManager.gold = int.Parse(respObj1["gold"]);
                        // _playerManager.TrackBestTime.Clear();
                        _playerManager.TrackOpen.Clear();
                        for (int i = 0; i < respObj1["open_tracks"].Count; i++)
                        {
                            _playerManager.TrackOpen.Add(int.Parse(respObj1["open_tracks"][i]));
                        }

                        OnResponse(int.Parse(playerData["result"]));


                    }
                    else
                    {
                        Debug.LogError("response.HasErrors in BuyTrack");
                    }
                });


        }

        public void BuyUpgrade(CarTypeEnum car_type, int car_lvl, UpgradeType up_type, int up_order,
            Action<string> OnResponse, Action<string> OnError)
        {
            GSRequestData jsonData = new GSRequestData();
            jsonData.AddString("carLevel", car_lvl.ToString());
            jsonData.AddString("carType", ((int)car_type).ToString());
            jsonData.AddString("upgradeType", ((int)up_type).ToString());
            jsonData.AddString("upgradeOrder", up_order.ToString());


            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("BUY_UPGRADE")
                .SetEventAttribute("JSN", jsonData)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        OnResponse(GetErrorMessage(0));
                    }
                    else
                    {
                        var error = response.Errors.JSON;
                        JObject errorObj = JObject.Parse(error);
                        Debug.Log(errorObj["error"]);
                        OnError(errorObj["error"]);
                    }
                });
        }

        public void RandomInShop(Action<int> Type, Action<int> Count, Action<string> OnError)
        {
            new GameSparks.Api.Requests.LogEventRequest()
               .SetEventKey("RANDOM_BUTTON")
               .Send((response) =>
               {
                   if (!response.HasErrors)
                   {
                      
                       GSData response_data = response.ScriptData.GetGSData("response_Data");
                       var respString = response.ScriptData.JSON;
                       JObject respObj = JObject.Parse(respString);
                       JObject playerData = null;
                       playerData = respObj["response_Data"];
                       respObj = playerData["player_Data"];
                       _playerManager.gold = int.Parse(respObj["gold"]);
                       _playerManager.jewels = int.Parse(respObj["jewels"]);
                       _playerManager.accountType = int.Parse(respObj["accountType"]);
                       _playerManager.IsOnRandom = bool.Parse(respObj["randomBonus"]["IsOn"]);
                       _playerManager.stampRandom = int.Parse(respObj["randomBonus"]["stamp"]);
                       _playerManager.stamp_now_user = int.Parse(respObj["stamp_now_user"]);

                       Count(int.Parse(playerData["countBonus"]));
                       Type(int.Parse(playerData["typeRand"]));
                   }
                   else
                   {
                       var error = response.Errors.JSON;
                       JObject errorObj = JObject.Parse(error);
                       Debug.Log(errorObj["error"]);
                       OnError(errorObj["error"]);
                   }
                   
               });
        }

        public void BuyInScore(int id, Action<int> OnResponse, Action<string> OnError)
        {
            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("BUY_IN_SCORE")
                .SetEventAttribute("ID_Product", id.ToString())
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        GSData response_data = response.ScriptData.GetGSData("response_Data");
                        var respString = response.ScriptData.JSON;
                        JObject respObj = JObject.Parse(respString);
                        JObject playerData = null;
                        playerData = respObj["response_Data"];
                        JObject respObj1 = playerData["player_Data"];

                        _playerManager.now_stamp = int.Parse(respObj1["now_stamp"]);
                        _playerManager.accountTime = int.Parse(respObj1["timeAccount"]);
                        _playerManager.jewels = int.Parse(respObj1["jewels"]);
                        _playerManager.gold = int.Parse(respObj1["gold"]);
                        _playerManager.accountType = int.Parse(respObj1["accountType"]);
                        OnResponse(int.Parse(respObj1["isBuy"]));
                    }
                    else
                    {
                        Debug.LogError("response.HasErrors in BuyInScore");
                        var error = response.Errors.JSON;
                        JObject errorObj = JObject.Parse(error);
                        Debug.Log(errorObj["error"]);
                        OnError(errorObj["error"]);
                    }
                });
        }

        public void BuyColor(CarTypeEnum car_type, int car_lvl, int color_id, Action<string> OnResponse, Action<string> OnError)
        {
            GSRequestData jsonData = new GSRequestData();
            jsonData.AddString("level", car_lvl.ToString());
            jsonData.AddString("type", ((int)car_type).ToString());
            jsonData.AddString("id", color_id.ToString());

            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("BUY_COLOR")
                .SetEventAttribute("JSN", jsonData)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        GSData response_data = response.ScriptData.GetGSData("response_Data");
                        var respString = response.ScriptData.JSON;
                        JObject respObj = JObject.Parse(respString);
                        JObject playerData = null;
                        playerData = respObj["response_Data"];

                        _playerManager.gold = int.Parse(playerData["new_gold"]);

                        OnResponse(GetErrorMessage(0));

                    }
                    else
                    {
                        var error = response.Errors.JSON;
                        JObject errorObj = JObject.Parse(error);
                        Debug.Log(errorObj["error"]);
                        OnError(errorObj["error"]);
                    }
                });
        }

        public void SetColor(int level, CarTypeEnum car_type, int color_id, Action<string> OnResponse)
        {
            GSRequestData jsonData = new GSRequestData();
            jsonData.AddString("level", level.ToString());
            jsonData.AddString("type", ((int)car_type).ToString());
            jsonData.AddString("id", color_id.ToString());

            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("SET_COLOR")
                .SetEventAttribute("JSN", jsonData)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        OnResponse(GetErrorMessage(0));
                    }
                    else
                    {
                        OnResponse(GetErrorMessage(1));
                    }
                });
        }

        string GetErrorMessage(int error_code)
        {
            //Debug.LogError(error_code);
            switch (error_code)
            {
                case 0: return "Success";
                case 4: return "Already bought";
                default: return "Unknown error";
            }
        }

        #endregion

        #region Game model data

        public void GetAllCars(Action OnComplete, Action OnError)
        {
            Debug.Log("GetAllCars");
            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("GET_ALL_MODELS")
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        Debug.Log("GetAllCars OK ");
                        GSData response_data = response.ScriptData.GetGSData("response_Data");
                        var respString = response.ScriptData.JSON;
                        JObject respObj = JObject.Parse(respString);
                        JObject carsData = null;
                        carsData = respObj["response_Data"]["finded_models"];

                       
                        if (carsData == null)
                        {
                            Debug.LogError("Error loading all models...");
                            OnError();
                        }
                        else
                        {
                            //Debug.LogError(carsData);
                            tutorialCars = carsData;
                            var models = new List<CarModel>();

                            CarModel cm = null;
                            for (int i = 0; i < carsData.Count; i++)
                            {
                                cm = new CarModel()
                                {
                                    type = int.Parse(carsData[i]["type"]),
                                    level = int.Parse(carsData[i]["level"]),
                                    name = carsData[i]["name"],
                                    description = carsData[i]["description"],

                                    price = new Price()
                                    {
                                        gold = int.Parse(carsData[i]["price"]["gold"]),
                                        jewels = (carsData[i]["price"]["silver"] == null) ? 0 : int.Parse(carsData[i]["price"]["silver"])
                                    }
                                };


                                cm.upgradeLevels = new Dictionary<int, string>();

                                for (int up = 0; up < carsData[i]["up_levels"].Count; up++)
                                {
                                    cm.upgradeLevels.Add(int.Parse(carsData[i]["up_levels"][up]["upgradeLevel"]),
                                        carsData[i]["up_levels"][up]["name"]);
                                    //Debug.LogError(cm.type + " uplvl: " + int.Parse(carsData[i]["up_levels"][up]["upgradeLevel"]) + " name: " + carsData[i]["up_levels"][up]["name"]);
                                }


                                

                                

                                cm.parametersValues = new Dictionary<string, float>();
                                string nameCarParam = "";
                                float valueCarParam = 0;
                                for (int par = 0; par < carsData[i]["parameters"].Count; par++)
                                {
                                    ///Debug.Log(carsData[i]["parameters"][par].ToString());
                                    if (!cm.parametersValues.ContainsKey(carsData[i]["parameters"][par]["name"]))
                                    {
                                        
                                        nameCarParam = carsData[i]["parameters"][par]["name"];
                                        string valueStr = carsData[i]["parameters"][par]["value"].Value;
                                        //Debug.Log(valueStr);
                                        
                                       
                                        bool isParse = float.TryParse(valueStr,out valueCarParam);
                                        if (!isParse)
                                            isParse =  float.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out valueCarParam);

                                        if(isParse)
                                        cm.parametersValues.Add(nameCarParam, valueCarParam);
                                        else { }
                                            //Debug.LogError("Error Pars all cars " + carsData[i]["parameters"][par].ToString());
                                        //Debug.LogError("name: " + carsData[i]["parameters"][par]["name"] + " value: " + float.Parse(carsData[i]["parameters"][par]["value"]));
                                    }
                                }

                                models.Add(cm);

                            }
                            DataModel.Instance.SetCars(models);
                            GetAllUpgrades(OnComplete, OnError);
                        }
                    }
                    else
                    {
                        Debug.LogError("Error loading cars data...");
                        OnError();
                    }
                });
        }

        public void GetAllUpgrades(Action OnComplete, Action OnError)
        {
            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("GET_ALL_UPGRADES")
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        GSData response_data = response.ScriptData.GetGSData("response_Data");
                        var respString = response.ScriptData.JSON;
                        JObject respObj = JObject.Parse(respString);
                        JObject upgradesData = null;
                        upgradesData = respObj["response_Data"]["finded_upgrades"];

                        if (upgradesData == null)
                        {
                            Debug.LogError("Error loading all models...");
                            OnError();
                        }
                        else
                        {
                            tutorialUpgrades = upgradesData;
                            //Debug.LogError(upgradesData);

                            var upgrades = new List<UpgradeItem>();

                            UpgradeItem upItem = null;
                            for (int i = 0; i < upgradesData.Count; i++)
                            {



                                upItem = new UpgradeItem()
                                {

                                    carType = int.Parse(upgradesData[i]["carType"]),
                                    carLevel = int.Parse(upgradesData[i]["carLevel"]),
                                    upgradeLevel = int.Parse(upgradesData[i]["upgradeLevel"]),

                                    price = new Price()
                                    {
                                        gold = int.Parse(upgradesData[i]["price"]["gold"]),
                                        jewels = (upgradesData[i]["price"]["jewels"] == null) ? 0 : int.Parse(upgradesData[i]["price"]["jewels"])
                                    },

                                    upgradeType = int.Parse(upgradesData[i]["upgradeType"]),
                                    upgradeOrder = int.Parse(upgradesData[i]["upgradeOrder"])

                                };

                                upItem.upgradeValues = new Dictionary<string, float>();
                                string nameUpgradeParam = "";
                                float valueUpgradeParam = 0;
                                for (int par = 0; par < upgradesData[i]["upgradeParameters"].Count; par++)
                                {
                                    if (!upItem.upgradeValues.ContainsKey(upgradesData[i]["upgradeParameters"][par]["name"]))
                                    {
                                        nameUpgradeParam = upgradesData[i]["upgradeParameters"][par]["name"];
                                        string valueStr = upgradesData[i]["upgradeParameters"][par]["value"];
                                        bool isPars = float.TryParse(valueStr,out valueUpgradeParam);
                                        if(!isPars)
                                            isPars = float.TryParse(valueStr,NumberStyles.Float, CultureInfo.InvariantCulture, out valueUpgradeParam);

                                        if (isPars)
                                            upItem.upgradeValues.Add(nameUpgradeParam, valueUpgradeParam);
                                        else
                                        Debug.LogError("name: " + upgradesData[i]["upgradeParameters"][par]["name"] + " value: " + float.Parse(upgradesData[i]["upgradeParameters"][par]["value"]));
                                    }
                                }

                                upgrades.Add(upItem);
                            }
                            DataModel.Instance.SetUpgrades(upgrades);
                            OnComplete();
                        }
                    }
                    else
                    {
                        Debug.LogError("Error loading cars data...");
                        OnError();
                    }
                });
        }

        #endregion
        /*
        public void GetAllColors(Action OnComplete, Action OnError)
        {
            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("GET_ALL_COLORS")
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        GSData response_data = response.ScriptData.GetGSData("response_Data");
                        var respString = response.ScriptData.JSON;
                        JObject respObj = JObject.Parse(respString);
                        JObject colorsData = null;
                        colorsData = respObj["response_Data"];

                        if (colorsData == null)
                        {
                            Debug.LogError("Error loading all colors...");
                            OnError();
                        }
                        else
                        {
                            CarColor col = null;
                            for (int i = 0; i < colorsData.Count; i++)
                            {
                                col = new CarColor()
                                {
                                    ID = int.Parse(colorsData[i]["id"]),
                                    hex = colorsData[i]["value"],
                                    price = int.Parse(colorsData[i]["gold"])
                                };
                                _playerManager.allColors.Add(col);
                            }
                        }
                    }
                });
        }
        */
        public void SET_CURRENT_CAR(CarTypeEnum car_type, int car_lvl, Action<string> OnResponse)
        {
            GSRequestData jsonData = new GSRequestData();
            jsonData.AddString("level", car_lvl.ToString());
            jsonData.AddString("type", ((int)car_type).ToString());

            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("SET_CURRENT_CAR")
                .SetEventAttribute("JSN", jsonData)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        OnResponse(GetErrorMessage(0));
                    }
                    else
                    {
                        OnResponse(GetErrorMessage(1));
                    }
                });

        }
        /*
        public void GetAllLevelsXP(Action OnComplete, Action OnError)
        {
            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("GET_ALL_LEVELS_XP")
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        GSData response_data = response.ScriptData.GetGSData("response_Data");
                        var respString = response.ScriptData.JSON;
                        JObject respObj = JObject.Parse(respString);
                        JObject XPData = null;
                        XPData = respObj["response_Data"];

                        if (XPData == null)
                        {
                            Debug.LogError("Error loading all colors...");
                            OnError();
                        }
                        else
                        {
                            LevelsXP level = null;
                            for (int i = 0; i < XPData.Count; i++)
                            {
                                level = new LevelsXP()
                                {
                                    ID = int.Parse(XPData[i]["id"]),
                                    maxValue = int.Parse(XPData[i]["value"])
                                };
                                _playerManager.allLevelsXP.Add(level);
                                //Debug.LogError(level.ID + " " + level.maxValue);
                            }
                        }
                    }
                });
        }
                */
        //
        public void SaveGoldWithMultiplier(JObject responseObject)
        {

                            JObject data = null;
                            data = responseObject;

                        // --------------------------------------------
                        if (data != null)
                        {
                            
                                

                                _stampTimerManager.X2StartStamp = int.Parse(data["player_Data"]["x2_start_stamp"]);
                                _stampTimerManager.X2EndStamp = int.Parse(data["player_Data"]["x2_end_stamp"]);

                                _stampTimerManager.X3StartStamp = int.Parse(data["player_Data"]["x3_start_stamp"]);
                                _stampTimerManager.X3EndStamp = int.Parse(data["player_Data"]["x3_end_stamp"]);

                                _stampTimerManager.X4StartStamp = int.Parse(data["player_Data"]["x4_start_stamp"]);
                                _stampTimerManager.X4EndStamp = int.Parse(data["player_Data"]["x4_end_stamp"]);

                                _stampTimerManager.nowStamp = int.Parse(data["player_Data"]["now_stamp"]);

                                _stampTimerManager.RegistrationNotification(_stampTimerManager.X2EndStamp - _stampTimerManager.nowStamp);
                                _stampTimerManager.RegistrationNotification(_stampTimerManager.X3EndStamp - _stampTimerManager.nowStamp);
                                _stampTimerManager.RegistrationNotification(_stampTimerManager.X4EndStamp - _stampTimerManager.nowStamp);
                                //
                                //_safePlayerPrefs.SaveEarnedGold(0); // ?????
                                _stampTimerManager.ReInit();


            
                            
                            
                            
                        }
                        // --------------------------------------------
                        else
                        {
                            Debug.LogError("[ERROR] response_Data is null !");
                            
                        }
                        // --------------------------------------------
            }
                    
        

        //

        public void SaveSingleplayer(Action OnComplete, Action OnError)
        {
            #region DEBUG
#if UNITY_EDITOR
            Debug.Log("SaveSingleplayer >>> " +
                      "gold = " + _safePlayerPrefs.GetInt_EarnedGold() + " | " +
                      "jewels = " + _safePlayerPrefs.GetInt_EarnedJewels() + " | " +
                      "exp = " + (int)_safePlayerPrefs.GetFloat_EarnedExp());
#endif
            #endregion

            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("UPDATE_SIGNLE_GAME")
                .SetEventAttribute("GOLD", _safePlayerPrefs.GetInt_EarnedGold())
                .SetEventAttribute("JEWELS", _safePlayerPrefs.GetInt_EarnedJewels())
                .SetEventAttribute("XP", (int)_safePlayerPrefs.GetFloat_EarnedExp())
                .Send((response) =>
                {
                    // ------------------------------------------------------------------------
                    if (!response.HasErrors)
                    {
                        GSData response_data = response.ScriptData.GetGSData("response_Data");
                        var respString = response.ScriptData.JSON;
                        JObject respObj = JObject.Parse(respString);

                        JObject playerData = null;
                        playerData = respObj["response_Data"];

                        if (playerData != null)
                        {
                            _playerManager.gold = int.Parse(playerData["player_Data"]["gold"]);
                            _playerManager.jewels = int.Parse(playerData["player_Data"]["jewels"]);
                            _playerManager.xp = int.Parse(playerData["player_Data"]["xp"]);
                        }
                        else
                        {
                            Debug.LogError("Error loading player Data...");
                            OnError();
                        }

                        Debug.Log("Update Singleplayer game succes...");
                        OnComplete();
                    }
                    // ------------------------------------------------------------------------
                    else
                    {
                        Debug.Log("Error Saving Player Data...");
                        OnError();
                    }
                    // ------------------------------------------------------------------------
                });
        }

        public void InsertFBData(string fb_id, string fb_token, Action OnComplete, Action OnError)
        {
            GSRequestData jsonData = new GSRequestData();
            jsonData.AddString("fb_id", fb_id);
            jsonData.AddString("fb_token", fb_token);

            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("INSERT_FB_DATA")
                .SetEventAttribute("JSN", jsonData)
                .Send((response) =>
                {
                    // ------------------------------------------------------------------------
                    if (!response.HasErrors)
                    {
                        GSData response_data = response.ScriptData.GetGSData("response_Data");
                        var respString = response.ScriptData.JSON;
                        JObject respObj = JObject.Parse(respString);

                        JObject playerData = null;
                        playerData = respObj["response_Data"];

                        if (playerData != null)
                        {
                            Debug.Log("Update FB_Data succes...");
                            OnComplete();
                        }
                        else
                        {
                            Debug.Log("Error Insert FB_Data...");
                            OnError();
                        }

                    }
                    // ------------------------------------------------------------------------
                    else
                    {
                        Debug.Log("Error Saving FB_Data...");
                        OnError();
                    }
                    // ------------------------------------------------------------------------
                });
        }

        public void DeleteFBData()
        {

            new GameSparks.Api.Requests.LogEventRequest()
               .SetEventKey("DELETE_FB_DATA")
               .Send((response) =>
               {
                   // ------------------------------------------------------------------------
                   if (!response.HasErrors)
                   {
                       Debug.Log("Delete FB_Data succes...");



                   }
                   // ------------------------------------------------------------------------
                   else
                   {
                       Debug.Log("Error Delete FB_Data...");

                   }
                   // ------------------------------------------------------------------------
               });
        }

        public void EndTutorial(Action onSucces, Action onError)
        {

            new GameSparks.Api.Requests.LogEventRequest()
               .SetEventKey("END_TUTORIAL")
               .Send((response) =>
               {
                   // ------------------------------------------------------------------------
                   if (!response.HasErrors)
                   {
                       onSucces();
                       Debug.Log("END Tutorial succes...");
                       _playerManager.isTutorial = 0;
                       
                   }
                   // ------------------------------------------------------------------------
                   else
                   {
                       onError();
                       Debug.Log("Error END Tutorial ...");

                   }
                   // ------------------------------------------------------------------------
               });
        }

        public void TimeEventIsOver()
        {
            new GameSparks.Api.Requests.LogEventRequest()
                .SetEventKey("TIME_EVENT_IS_OVER")
                 .Send((response) =>
                   {
                       // ------------------------------------------------------------------------
                       if (!response.HasErrors)
                       {
                           GSData response_data = response.ScriptData.GetGSData("response_Data");
                           var respString = response.ScriptData.JSON;
                           JObject respObj = JObject.Parse(respString);
                           JObject eventData = null;
                           eventData = respObj["response_Data"];
                           LoadEvent(eventData["EventData"]);

                       }
                       // ------------------------------------------------------------------------
                       else
                       {
                           Debug.LogError("Error TimeEventIsOver...");

                       }
                       // ------------------------------------------------------------------------
                   });
        }


        public static string ZipString(string sBuffer)
        {
            MemoryStream m_msBZip2 = null;
            BZip2OutputStream m_osBZip2 = null;
            string result;
            try
            {
                m_msBZip2 = new MemoryStream();
                Int32 size = sBuffer.Length;
                // Prepend the compressed data with the length of the uncompressed data (firs 4 bytes)
                //
                using (BinaryWriter writer = new BinaryWriter(m_msBZip2, System.Text.Encoding.ASCII))
                {
                    writer.Write(size);

                    m_osBZip2 = new BZip2OutputStream(m_msBZip2);
                    m_osBZip2.Write(Encoding.ASCII.GetBytes(sBuffer), 0, sBuffer.Length);

                    m_osBZip2.Close();
                    result = Convert.ToBase64String(m_msBZip2.ToArray());
                    m_msBZip2.Close();

                    writer.Close();
                }
            }
            finally
            {
                if (m_osBZip2 != null)
                {
                    m_osBZip2.Dispose();
                }
                if (m_msBZip2 != null)
                {
                    m_msBZip2.Dispose();
                }
            }
            return result;
        }

        public static string UnzipString(string compbytes)
        {
            Debug.LogError("byte " + compbytes);
            string result;
            MemoryStream m_msBZip2 = null;
            BZip2InputStream m_isBZip2 = null;
            try
            {
                m_msBZip2 = new MemoryStream(Convert.FromBase64String(compbytes));
                // read final uncompressed string size stored in first 4 bytes
                //
                using (BinaryReader reader = new BinaryReader(m_msBZip2, System.Text.Encoding.ASCII))
                {
                    Int32 size = reader.ReadInt32();

                    m_isBZip2 = new BZip2InputStream(m_msBZip2);
                    byte[] bytesUncompressed = new byte[size];
                    m_isBZip2.Read(bytesUncompressed, 0, bytesUncompressed.Length);
                    m_isBZip2.Close();
                    m_msBZip2.Close();

                    result = Encoding.ASCII.GetString(bytesUncompressed);

                    reader.Close();
                }
            }
            finally
            {
                if (m_isBZip2 != null)
                {
                    m_isBZip2.Dispose();
                }
                if (m_msBZip2 != null)
                {
                    m_msBZip2.Dispose();
                }
            }
            return result;
        }

    }
}