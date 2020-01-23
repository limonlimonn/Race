using System;
using UnityEngine;

using HCR.Interfaces;
using HCR.Enums;
using System.Collections.Generic;
using SimpleJSON;
using System.IO;

namespace HCR
{
    /// <summary>
    /// Класс - обертка над PlayerPrefs для шифрования и дешифрования данных
    /// (завуалировать данные будем через Base64-преобразование)
    /// </summary>

    public class SafePlayerPrefs :IService
    {
        private PlayerManager _playerManager;
        private StatisticsService _statistic;

        private float ExpForTricks = 0;
        private int maxFlip = 0;
        private float maxBlunt = 0;
        private float maxInAir = 0;
        private float maxBalance = 0;
        private Dictionary<string, TGame> SaveGames = new Dictionary<string, TGame>();

        // I_SERVICE

        public void Init()
        {
            _playerManager = Core.Instance.GetService<PlayerManager>();
            _statistic = Core.Instance.GetService<StatisticsService>();
            //ResetAllData();
           // LoadSaveGame();
        }

        // LOAD GAME 
        /*
        private void LoadSaveGame()
        {
            if (PlayerPrefs.HasKey(PPKeys.GamesId))
            {
                string Games = PlayerPrefs.GetString(PPKeys.JSON);
                JObject respObj = JObject.Parse(Games);
                for (int i = 0; i < respObj.Count; i++)
                {
                    JObject gameData = new JObject();
                    gameData = respObj[Get_GamesID(i)];
                    string arr = gameData["gameId"];
                    Debug.Log("arr" + i + " " + arr);
                }

            }
        }

        private string Get_GamesID(int i)
        {
            if (PlayerPrefs.HasKey(PPKeys.GamesId + i.ToString()))
                return PlayerPrefs.GetString(PPKeys.GamesId + i.ToString());
            else { Debug.LogError("GAME_ID is no found"); return "0"; }
        }*/

        // INTERFACES

        #region PLAYER_XX
        public int GetMaxFlip
        {
            get { return maxFlip; }
        }
        public float GetMaxBlunt
        {
            get { return maxBlunt; }
        }
        public float GetMaxInAir
        {
            get { return maxInAir; }
        }
        public float GetMaxBalance
        {
            get { return maxBalance; }
        }
        private string MygameId; 


        // =================================================
        // Save_gameData

        // new
        

        public void SafeDataOnPlayClick(GameData gameData)
        {
            TGame Game = new TGame();



            Game.TrackId = gameData.track_id;
            //Remove elemnt by Key before add if exists
            if (SaveGames.ContainsKey(gameData.gameId))
            {
                SaveGames.Remove(gameData.gameId);
            }

            SaveGames.Add(gameData.gameId, Game);

            if (!PlayerPrefs.HasKey(PPKeys.JSON) || PlayerPrefs.GetString(PPKeys.JSON) == "")
                PlayerPrefs.SetString(PPKeys.JSON, ObjectToGSData(SaveGames).JSON.ToString());
            else
                PlayerPrefs.SetString(PPKeys.JSON, SaveNextGame(ObjectToGSData(SaveGames)));

            SaveGamesId(gameData.gameId);
            Debug.Log("JSON " + PlayerPrefs.GetString(PPKeys.JSON));
        }


        // CONVERT  
        #region CONVERT
        
        public string SaveNextGame(GameSparks.Core.GSRequestData obj) // SAVE GAME >= 1
        {
            string Game = obj.JSON;

            string Games = PlayerPrefs.GetString(PPKeys.JSON);

            Games = Games.Substring(0, Games.Length - 1);
            Games +=","+ Game.Remove(0, 1);
            Debug.Log("SaveNextGame" + Games);
            return Games;
            //.SetString(PPKeys.JSON, Games);
        }

        public GameSparks.Core.GSRequestData ObjectToGSData(Dictionary<string, TGame> obj)
        {
            GameSparks.Core.GSRequestData gsData = new GameSparks.Core.GSRequestData();

            foreach (var item in obj)
            {
                Debug.Log("item key  " + item.Key);
                GameSparks.Core.GSRequestData child = new GameSparks.Core.GSRequestData();
                foreach (var field in item.Value.GetType().GetFields())
                {
                        Debug.Log("other  " + field.FieldType + " name " + field.Name + " " + field.GetValue(item.Value).ToString());
                        child.AddString(field.Name, field.GetValue(item.Value).ToString());
                    Debug.Log("JSON child  " + child.JSON.ToString());
                }

                gsData.AddObject(item.Key, child);
            }
            Debug.Log("JSON " + gsData.JSON.ToString());
            return gsData;

        }

        public GameSparks.Core.GSRequestData StringToGSData(string gameID)
        {
            GameSparks.Core.GSRequestData gsData = new GameSparks.Core.GSRequestData();
            gsData.AddString(gameID, gameID);
            Debug.Log("StringToGSData " + gsData.JSON.ToString());
            return gsData;

        }

        private void SaveGamesId(string gameId)
        {
            string newGame = StringToGSData(gameId).JSON;
            if (PlayerPrefs.HasKey(PPKeys.GamesId))
            {
                string oldGame = PlayerPrefs.GetString(PPKeys.GamesId);
                newGame = oldGame.Substring(0, oldGame.Length - 1) + ","+ newGame.Remove(0, 1);
            }
            PlayerPrefs.SetString(PPKeys.GamesId, newGame);
            Debug.Log("GamesId" + newGame);
        }

        public void GSDataToObject()
        {/*
            Debug.LogWarning(gsData.JSON);

            Type objType = Type.GetType(gsData.GetString("type"));
            object obj = Activator.CreateInstance(objType);
            foreach (var typeField in objType.GetFields())
            {
                if (!typeField.IsNotSerialized && gsData.ContainsKey(typeField.Name))

                {
                    // check our types here
                }
            }

            return obj;*/
        }

        #endregion

        // CHANGE Trise
        public void ChangeTryse(int tryes, string gameId)
        {
            string Game = PlayerPrefs.GetString(PPKeys.JSON);
            JObject respObj = JObject.Parse(Game);
            Debug.Log("respObj " + respObj);
            if (respObj != null)
            {
                respObj[gameId]["TriesCount"] = (tryes + 1).ToString();

                PlayerPrefs.SetString(PPKeys.JSON, respObj.ToString());
            }
        }

        public void SaveSattistic(string gameId, string name, Dictionary<string, string> Game)
        {
            var myStopwatch = new System.Diagnostics.Stopwatch();
            myStopwatch.Start();
            //Debug.LogError("SaveSattistic Start");


            if (Game == null) { Debug.LogError("Statistic == null"); return; }

            string GameJSON = PlayerPrefs.GetString(PPKeys.JSON);
            JObject respObj = JObject.Parse(GameJSON);
            respObj[gameId].Add(name,new object());
            foreach (var str in Game)
            {
                
                //Debug.Log("Key " + str.Key + " Value " + str.Value.ToString());
                respObj[gameId][name].Add(str.Key, str.Value.ToString());
            }

            if (name == "StatisticReplay")
            {
                respObj[gameId]["ReplayDone"] = true.ToString();
                respObj[gameId]["TriesCount"] = "0";
            }
            else if (name == "StatisticRecord")
                respObj[gameId]["RecordDone"] = true.ToString();
            Debug.Log("JSON" + respObj.ToString());
            PlayerPrefs.SetString(PPKeys.JSON, respObj.ToString());

            myStopwatch.Stop();
            //Debug.LogError("SaveSattistic" + myStopwatch.Elapsed.Seconds + "  " + myStopwatch.Elapsed.Milliseconds);
        }
        
        public void SaveUserSattistic(GameData gameData, string name, Dictionary<string, float> User)
        {
            
            if (User == null) {Debug.LogError("SaveUserSattistic == null"); return; }

            string GameJSON = PlayerPrefs.GetString(PPKeys.JSON);
            JObject respObj = JObject.Parse(GameJSON);
            respObj[gameData.gameId].Add(name, new object());
            foreach (var str in User)
            {
                respObj[gameData.gameId][name].Add(str.Key, str.Value.ToString());
                Debug.Log("SUS " + str.Key + " " + str.Value);
            }

            if (gameData.GameType != GameTypeEnum.MultyRecord)
            {
                PlayerPrefs.SetString(PPKeys.JSON, respObj.ToString());
            }else
            {
                SaveRecordData(gameData, _playerManager.PlayerCar.model , respObj);
                    
            }
            
        }

        public void SaveTrackRecord(string gameId,int trackId)
        {
            string GameJSON = PlayerPrefs.GetString(PPKeys.JSON);
            JObject respObj = JObject.Parse(GameJSON);
            if(_playerManager.isTutorial == 1) { return; }
            respObj[gameId]["TrackId"] = trackId.ToString();
            respObj[gameId]["IsRecord"] = true.ToString();
            Debug.Log("IsRecord " + respObj[gameId]["IsRecord"].ToString());
            PlayerPrefs.SetString(PPKeys.JSON, respObj.ToString());
        }

        public void DeleteGame(string gameId)
        {
            string GameJSON = PlayerPrefs.GetString(PPKeys.JSON);
            JObject gameObj = JObject.Parse(GameJSON);
            if (_playerManager.isTutorial == 1) { return; }

            PlayerPrefs.DeleteKey(PPKeys.multiplier);
            PlayerPrefs.DeleteKey(PPKeys.goldEarned);


            gameObj.Remove(gameId);
            Debug.Log("JSON DELETE " + gameId);
            Debug.Log("  " + gameObj.ToString());

            string GameID = PlayerPrefs.GetString(PPKeys.GamesId);
            JObject gameID = JObject.Parse(GameID);

            gameID.Remove(gameId);
            PlayerPrefs.SetString(PPKeys.GamesId, gameID.ToString());
            Debug.Log("GamesId" + PlayerPrefs.GetString(PPKeys.GamesId));
            PlayerPrefs.SetString(PPKeys.JSON, gameObj.ToString());
            if (gameObj.ToString() == "{}" || gameID.ToString() == "{}")
            {
                PlayerPrefs.DeleteKey(PPKeys.JSON);
                PlayerPrefs.DeleteKey(PPKeys.GamesId);
            }

            PlayerPrefs.DeleteKey(PPKeys.ReplayJSON);
        }

        public void DeleteAll()
        {
            PlayerPrefs.DeleteKey(PPKeys.JSON);
            PlayerPrefs.DeleteKey(PPKeys.GamesId);
            PlayerPrefs.DeleteKey(PPKeys.ReplayJSON);
        }

        public int GetTryse(string gameId)
        {
            string Game = PlayerPrefs.GetString(PPKeys.JSON);
            JObject respObj = JObject.Parse(Game);
            if (Game != "" && respObj[gameId]["TriesCount"] != null)
                return int.Parse(respObj[gameId]["TriesCount"]);
            else return 0;
        }

        public void FillRecordStatistic(string gameId)
        {
            _statistic.StatisticRecord = new Dictionary<string, string>();
            _statistic.StatisticRecord = GetGameStatistic("StatisticRecord", gameId);
        }

        public void FillReplayStatistic(string gameId)
        {
            _statistic.StatisticReplay = new Dictionary<string, string>();
            _statistic.StatisticReplay = GetGameStatistic("StatisticReplay", gameId);
        }

        public void FillUserStatistic(string gameId)
        {
            _statistic.UserStatistic = new Dictionary<string, float>();
            _statistic.UserStatistic = GetUserStatistic("StatisticUser", gameId);
        }

        private Dictionary<string, string> GetGameStatistic(string name, string gameId)
        {
            string GameJSON = PlayerPrefs.GetString(PPKeys.JSON);
            JObject respObj = JObject.Parse(GameJSON);
                if(GameJSON == "") Debug.LogError("JSON == null GetGameStatistic");              
            Dictionary<string, string> Statistic = new Dictionary<string, string>();
            string[] nameStat = new string[] { };
            if (name == "StatisticReplay")
                nameStat = _statistic.nameStatReplay;
            else if (name == "StatisticRecord")
                nameStat = _statistic.nameStatRecord;

            for(int i = 0; i < respObj[gameId][name].Count; i++)
            {
                Statistic.Add(nameStat[i], respObj[gameId][name][i].Value);

                
            }
          
            return Statistic;
        }

        private Dictionary<string, float> GetUserStatistic(string name, string gameId)
        {
            string GameJSON = PlayerPrefs.GetString(PPKeys.JSON);
            JObject respObj = JObject.Parse(GameJSON);
            if (GameJSON == "") Debug.LogError("JSON == null GetUserStatistic");
            Dictionary<string, float> Statistic = new Dictionary<string, float>();
            string[] nameStat = new string[] { };
            nameStat = _statistic.nameStatUser;
            Debug.Log("Fill StatisU " + respObj[gameId][name].ToString());
            Debug.Log("respObj " + respObj.ToString());

            for (int i = 0; i < respObj[gameId][name].Count; i++)
            {
                
                Statistic.Add(nameStat[i], float.Parse(respObj[gameId][name][i].Value));
            }
            
            return Statistic;
        }

        public void SaveRecordData(GameData gameData, PlayerCarModel pcm, JObject respObj)
        {   //
            respObj[gameData.gameId].Add("replayData", "crash");
            respObj[gameData.gameId].Add("smile", new object());
            if (gameData.smiles != null)
            {
                foreach (var str in gameData.smiles)
                {
                    respObj[gameData.gameId]["smile"].Add(str.ToString());
                }
            }
            else
            {
                respObj[gameData.gameId]["smile"].Add("1");
                respObj[gameData.gameId]["smile"].Add("1");
                respObj[gameData.gameId]["smile"].Add("1");
            }
            

           
            respObj[gameData.gameId].Add("IsFinishedRecord", gameData.IsFinishedRecord);
            respObj[gameData.gameId].Add("IsDone", gameData.IsDone);
            respObj[gameData.gameId].Add("track_id", gameData.track_id);
            respObj[gameData.gameId].Add("gameResultScore", gameData.gameResultScore);
            

            respObj[gameData.gameId].Add("current_color", pcm.current_color);
            respObj[gameData.gameId].Add("carLevel", pcm.carLevel);
            respObj[gameData.gameId].Add("type", pcm.type);
            respObj[gameData.gameId].Add("car_upgrade_level", pcm.car_upgrade_level);

            
            
            
            string respons = respObj.ToString();
            
            PlayerPrefs.SetString(PPKeys.JSON, respons);

        }

        public void SaveReplayData(GameData gameData)
        {
             PlayerPrefs.SetString(PPKeys.ReplayJSON, gameData.replayData);
        }


        public void FillGameData(string gameId)
        {
            string Game = PlayerPrefs.GetString(PPKeys.JSON);
            JObject gameObj = JObject.Parse(Game);
            GameData gameData = new GameData();
            gameData.gameId = gameId;
            if (PlayerPrefs.HasKey(PPKeys.ReplayJSON))
            {
                gameData.recordData = PlayerPrefs.GetString(PPKeys.ReplayJSON);
            }
            else
            {
                gameData.recordData = gameObj[gameId]["replayData"];
            }
            gameData.track_id = gameObj[gameId]["track_id"];
            gameData.gameResultScore = int.Parse(gameObj[gameId]["gameResultScore"]);
            gameData.IsFinishedRecord = int.Parse(gameObj[gameId]["IsFinishedRecord"]);
            gameData.IsDone = int.Parse(gameObj[gameId]["IsDone"]);
            gameData.smiles = new List<int>();
            foreach (var str in gameObj[gameId]["smile"].Childs)
            {
                gameData.smiles.Add(int.Parse(str.Value));
            }

            var gm = Core.Instance.GetService<GameManager>();
            //gm.Games.Remove(gm.GetGameDataByID(gameId));
            GameData gD = new GameData();
                foreach (var item in gm.Games)
            {
                if(item.gameId == gameData.gameId)
                {
                    gD =  item;
                }
            }
            if(gD != null)
            {
                gm.Games.Remove(gD);
            }
            gm.Games.Add(gameData);

            
        }

        public PlayerCarModel GetCar(string gameId)
        {
            string Game = PlayerPrefs.GetString(PPKeys.JSON);
            JObject gameObj = JObject.Parse(Game);
            PlayerCarModel car = new PlayerCarModel()
            {
                car_upgrade_level = int.Parse(gameObj[gameId]["car_upgrade_level"]),
                current_color = int.Parse(gameObj[gameId]["current_color"]),
                carLevel = int.Parse(gameObj[gameId]["carLevel"]),
                type = int.Parse(gameObj[gameId]["type"])
            };

            return car;
        }

        public bool IsFinishGame()
        {
           

            string GameID = PlayerPrefs.GetString(PPKeys.GamesId);
            JObject respObj = JObject.Parse(GameID);
            
            string Game = PlayerPrefs.GetString(PPKeys.JSON);
            
            
            JObject gameObj = JObject.Parse(Game);
            
            if (GameID == ""|| Game == "") return false;

           
            


            foreach (var id in respObj.Childs)
            {
                string f = id;
                
                if (gameObj[id]["RecordDone"].ToString() == "\"True\"")
                {
                    MygameId = id;
                    Debug.Log("gameId FinishGame : " + MygameId); return true;
                }
            }
            
            return false;
        }

        public string LoadFinishGame()
        {
            FillRecordStatistic(MygameId);
            FillReplayStatistic(MygameId);
            FillUserStatistic  (MygameId);
            Debug.Log("MygameId ret " + MygameId);
            return MygameId;
        }

        /*
        public void SaveSendLater(string gameId)
        {
            string Game = PlayerPrefs.GetString(PPKeys.JSON);
            JObject gameObj = JObject.Parse(Game);
            gameObj[gameId]["Later"] = "1";

            PlayerPrefs.SetString(PPKeys.JSON, gameObj.ToString());

        }
        */

        //____________________________


        public void Safe_SaveGoldWithMultiplier(int gold, int multiplier , Action OnComplete)
        {
            PlayerPrefs.SetInt(PPKeys.goldEarned, gold);
            PlayerPrefs.SetInt(PPKeys.multiplier, multiplier);
            OnComplete();
        }

        public int GetGolgEarnedWithMultiplier()
        {
            if (PlayerPrefs.HasKey(PPKeys.goldEarned))
                return PlayerPrefs.GetInt(PPKeys.goldEarned);
            else return 0;
        }

             public int GetMultiplier()
        {
            if (PlayerPrefs.HasKey(PPKeys.multiplier))
                return PlayerPrefs.GetInt(PPKeys.multiplier);
            else return 0;



        }


        //   NEW 
        public bool IsGameId(string gameId)
        {
            string Game = PlayerPrefs.GetString(PPKeys.JSON);
            JObject respObj = JObject.Parse(Game);
            if (Game != "" && respObj[gameId] != null)
            {
                Debug.Log(respObj.ToString());
            }
            if (Game !="" && respObj[gameId] != null )
                return true;
            else return false;

        }

        public bool IsRecordDone(string gameId)
        {
            string GameJSON = PlayerPrefs.GetString(PPKeys.JSON);
            JObject respObj = JObject.Parse(GameJSON);
            Debug.Log(GameJSON.ToString());
            if (GameJSON != "" && respObj[gameId]["RecordDone"].ToString() == "\"True\"")
                return true;
            else return false;
        }

        public bool IsReplayDone(string gameId)
        {
            string GameJSON = PlayerPrefs.GetString(PPKeys.JSON);
            JObject respObj = JObject.Parse(GameJSON);
            Debug.Log(GameJSON.ToString());
            if (GameJSON != "" && respObj[gameId]["ReplayDone"].ToString() == "\"True\"")
                return true;
            else return false;
        }

        public bool IsReplay(string gameId)
        {
            string GameJSON = PlayerPrefs.GetString(PPKeys.JSON);
            JObject respObj = JObject.Parse(GameJSON);
            Debug.Log(GameJSON.ToString());
            if (GameJSON != "" && respObj[gameId]["IsReplay"].ToString() == "\"True\"")
                return true;
            else return false;
        }

        public bool IsRecord(string gameId)
        {
            string GameJSON = PlayerPrefs.GetString(PPKeys.JSON);
            JObject respObj = JObject.Parse(GameJSON);
            Debug.Log(GameJSON.ToString());
            if (GameJSON != "" && respObj[gameId]["IsRecord"].ToString() == "\"True\"")
                return true;
            else return false;
        }

        public int GetTrack(string gameId)
        {
            string GameJSON = PlayerPrefs.GetString(PPKeys.JSON);
            JObject respObj = JObject.Parse(GameJSON);
            if (GameJSON != "" && respObj[gameId]["TrackId"] != null)
                return int.Parse(respObj[gameId]["TrackId"]);
            else { Debug.LogError("Error GetTrack"); return 0; }
        }

        //

        // =================================================
        // Player_Authentification

        public void SavePlayerName(string name)
        {
            PlayerPrefs.SetString(PPKeys.name, name);
        }

        public void SavePlayerPass(string pass)
        {
            PlayerPrefs.SetString(PPKeys.pass, pass);
        }


        // =================================================
        // PLAYER_Jewels
        public void SavePlayerJewels(int Jewels)
        {
            PlayerPrefs.SetInt(PPKeys.playerJewels,
                Jewels);
        }

        public void AddPlayerJewels(int Jewels)
        {
            SavePlayerJewels(
                PlayerPrefs.GetInt(PPKeys.playerJewels)
                + Jewels);
        }

        public int GetInt_PlayerJewels()
        {
            return 
                PlayerPrefs.GetInt(PPKeys.playerJewels);
        }

        // =================================================
        // PLAYER_GOLD
        public void SavePlayerGold(int gold)
        {
            PlayerPrefs.SetInt(PPKeys.playerGold,
                gold);
        }

        public void AddPlayerGold(int gold)
        {
            SavePlayerGold(
                PlayerPrefs.GetInt(PPKeys.playerGold)
                + gold);
        }

        public int GetInt_PlayerGold()
        {
            return 
                PlayerPrefs.GetInt(PPKeys.playerGold);
        }

        // =================================================
        // PLAYER_EXP
        public void SavePlayerExp(float exp)
        {
            PlayerPrefs.SetFloat(PPKeys.playerExp,
                exp);
        }

        public void AddPlayerExp(float exp)
        {
            SavePlayerExp(
                PlayerPrefs.GetFloat(PPKeys.playerExp)
                + exp);
        }

        public float GetFloat_PlayerExp()
        {
            return 
                PlayerPrefs.GetFloat(PPKeys.playerExp);
        }

        #endregion

        #region EARNED_XX

        public void SaveGoldWithMultiplier(int multiplier , Action OnComplete)
        {

        }

        // =================================================
        // EARNED_Jewels
        public void SaveEarnedJewels(int jewels)
        {
            PlayerPrefs.SetInt(PPKeys.earnedJewels,
                jewels);
        }

        public void AddEarnedJewels(int jewels)
        {
            SaveEarnedJewels(
                PlayerPrefs.GetInt(PPKeys.earnedJewels)
                + jewels);
        }

        public int GetInt_EarnedJewels()
        {
            return 
                PlayerPrefs.GetInt(PPKeys.earnedJewels);
        }

        // =================================================
        // EARNED_GOLD
        public void SaveEarnedGold(int gold)
        {
            PlayerPrefs.SetInt(PPKeys.earnedGold,
                gold);
        }

        public void AddEarnedGold(int gold)
        {
            SaveEarnedGold(
               PlayerPrefs.GetInt(PPKeys.earnedGold)
                + gold);
        }

        public int GetInt_EarnedGold()
        {
            return (
                PlayerPrefs.GetInt(PPKeys.earnedGold));
        }

        // =================================================
        // EARNED_EXP
        public void SaveEarnedExp(float exp)
        {
            PlayerPrefs.SetFloat(PPKeys.earnedExp,
                exp);
        }

        public void AddEarnedExp(float exp)
        {
            SaveEarnedExp(
                PlayerPrefs.GetFloat(PPKeys.earnedExp)
                + exp);
        }

        public float GetFloat_EarnedExp()
        {
            return PlayerPrefs.GetFloat(PPKeys.earnedExp);
        }

        public void AddEarnedExpForTrick(float exp)
        {
            ExpForTricks += exp;
        }

        public void SaveEarnedExpForTrick(float exp)
        {
            ExpForTricks = exp;
        }

        public float GetFloat_ExpForTrick()
        {
            return ExpForTricks;
        }

        #endregion

        #region TRICKS_XX

        // =================================================
        // TRICK_HORSE
        public void SaveTrickHorseTime(float time)
        {
            PlayerPrefs.SetFloat(PPKeys.trickHorseTime,
                time);
        }

        public void AddTrickHorseTime(float time)
        {
            SaveTrickHorseTime(
                PlayerPrefs.GetFloat(PPKeys.trickHorseTime)
                + time);
            maxBalance = Math.Max(maxBalance, time);
        }

        public float GetFloat_HorseTime()
        {
            return 
            PlayerPrefs.GetFloat(PPKeys.trickHorseTime);
        }

        // =================================================
        // TRICK_90
        public void SaveTrick90Time(float sum)
        {
            PlayerPrefs.SetFloat(PPKeys.trick90Time,
                sum);
        }

        public void AddTrick90Time(float sum)
        {
            SaveTrick90Time(
                PlayerPrefs.GetFloat(PPKeys.trick90Time)
                + sum);

            maxBlunt = Math.Max(maxBlunt, sum);

        }

        public float GetFloat_90Time()
        {

            return PlayerPrefs.GetFloat(PPKeys.trick90Time);
        }

        // =================================================
        // TRICK_AIR
        public void SaveTrickAirTime(float time)
        {
            PlayerPrefs.SetFloat(PPKeys.trickAirTime,
                time);
        }

        public void AddTrickAirTime(float time)
        {
            SaveTrickAirTime(
               PlayerPrefs.GetFloat(PPKeys.trickAirTime)
                + time);
            //Debug.Log("maxInAir" + time);
            maxInAir = Math.Max(maxInAir, time);
        }

        public float GetFloat_TrickAirTime()
        {

            return 
            PlayerPrefs.GetFloat(PPKeys.trickAirTime);
        }

        // =================================================
        // TRICK_FLIP
        public void SaveTrickFlipSum(int sum)
        {
            PlayerPrefs.SetInt(PPKeys.trickFlipSum,
                sum);
        }

        public void AddTrickFlipSum(int sum)
        {
            SaveTrickFlipSum(
                PlayerPrefs.GetInt(PPKeys.trickFlipSum)
                + sum);
            maxFlip = Math.Max(maxFlip, sum);
        }

        public int GetInt_TrickFlipSum()
        {

            return 
            PlayerPrefs.GetInt(PPKeys.trickFlipSum);
        }

        #endregion



        // METHODS

        private void ResetAllData()
        {
            SavePlayerJewels(0);
            SavePlayerGold(0);
            SavePlayerExp(0);

            SaveEarnedJewels(0);
            SaveEarnedGold(0);
            SaveEarnedExp(0);

            SaveTrickHorseTime(0f);
            SaveTrickAirTime(0f);
            SaveTrickFlipSum(0);
            SaveTrick90Time(0);
        }

        public void DeleteDataForLogOut()
        {
            PlayerPrefs.DeleteKey(PPKeys.name);
            PlayerPrefs.DeleteKey(PPKeys.pass);
            PlayerPrefs.DeleteKey(PPKeys.playerJewels);
            PlayerPrefs.DeleteKey(PPKeys.playerGold);
            PlayerPrefs.DeleteKey(PPKeys.playerExp);
            PlayerPrefs.DeleteKey(PPKeys.earnedJewels);
            PlayerPrefs.DeleteKey(PPKeys.earnedGold);
            PlayerPrefs.DeleteKey(PPKeys.earnedExp);
            PlayerPrefs.DeleteKey(PPKeys.trickHorseTime);
            PlayerPrefs.DeleteKey(PPKeys.trickAirTime);
            PlayerPrefs.DeleteKey(PPKeys.trickFlipSum);
            PlayerPrefs.DeleteKey(PPKeys.trick90Time);
            PlayerPrefs.DeleteKey(PPKeys.uiType);
            PlayerPrefs.DeleteKey(PPKeys.qualityShadows);
            PlayerPrefs.DeleteKey(PPKeys.vibration);
            PlayerPrefs.DeleteKey(PPKeys.music);
            PlayerPrefs.DeleteKey(PPKeys.volumeMusic);
            PlayerPrefs.DeleteKey(PPKeys.Tutorial_step);
            PlayerPrefs.DeleteKey(PPKeys.goldEarned);
            PlayerPrefs.DeleteKey(PPKeys.multiplier);
            DeleteAll();


        }

        #region CONVERTERS
        /*
        // =================================================
        //
        public int ConvertStringToInt(string stringValue)
        {
            int numValue = 0;
            bool isParseOK = Int32.TryParse(stringValue, out numValue);

            if (isParseOK)
            {
                return numValue;
            }
            else
            {
                #region DEBUG
#if UNITY_EDITOR
                Debug.Log("[ERROR] parse to int failed !");
#endif
                #endregion
            }

            return 0;
        }

        private float ConvertStringToFloat(string stringValue)
        {
            Debug.Log("ConvertStringToFloat" + stringValue);
            return float.Parse(stringValue.Replace(',', '.'));
        }

        // =================================================
        //
        private string ConvertIntToBase64(int intValue)
        {
            return Base64Helper.Encode(intValue.ToString());
        }

        private int ConvertBase64ToInt(string base64Value)
        {
            return ConvertStringToInt(Base64Helper.Decode(base64Value));
        }

        // =================================================
        //
        private string ConvertFloatToBase64(float floatValue)
        {
            return Base64Helper.Encode(floatValue.ToString());
        }

        private float ConvertBase64ToFloat(string base64Value)
        {
            return ConvertStringToFloat(Base64Helper.Decode(base64Value));
        }

        

        */
        #endregion

    }

    public class TGame
    {
        
        public int TriesCount = 0;
        public bool IsRecord = false;
        public bool IsReplay = true;
        public bool RecordDone = false;
        public bool ReplayDone = false;
        public string TrackId;

        

        

        

    }
}