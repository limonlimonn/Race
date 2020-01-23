using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;

using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;
using SimpleJSON;

using HCR.Enums;
using HCR.Interfaces;
using HCR.Loading;
using DG.Tweening;
using HCR.Network;
using GoogleMobileAds.Api;

namespace HCR.GlobalWindow.MainMenu
{
	/// <summary>
	/// Класс - Главное Окно (показывается после логина)
	/// в "Header" кнопка называется - "Multiplayer"
	/// </summary>

	public class MultiplayerWindow : MonoBehaviour, IUIWindow
	{
		// ACTIONS

		public Action OnSingleNew;
		public Action OnNewGame;
        public Action<string,string> OnNewGameForFB; 
        public Action OnRandomGame;

		//public Action OnRefreshGame;
		public Action<string> OnPlay;

		// FIELDS

		#region VARIABLES

		// ------------------------------------------------
		//
		[Header("BUTTONS")]
		[SerializeField] private Button _buttonSingleplayer;
		[SerializeField] private Button _buttonNewGame;
		[SerializeField] private Button _buttonRandomGame;
		[SerializeField] private Button _buttonMyGames;
		[SerializeField] private Button _buttonMostRecentGames;
        [SerializeField] private Button _buttonFriends;
        [SerializeField] private Button _buttonInviteMoreFriends;
        [Header("COUNTERS")]
        [SerializeField] private CanvasGroup _counterMyGamesCanvas;
        [SerializeField] private Text _counterMyGamesText;
        private int _counterMyGamesValue = 0;
        // ------------------------------------------------
        //
        [Space]
		public SpriteRenderer bg;
		public CanvasGroup canvaseGroup;
		public GameObject fade;

		private GameObject currentCar;
		private Transform _carParent;

		[Header("MY GAMES")]
		[SerializeField] private GameObject _objScrollMyGames;
		public Transform parent;
		private RectTransform rt = new RectTransform();
		public GameObject card;

		private List<GameCard> cardsList;

		// ------------------------------------------------
		//
		[Header("MOST RECENT GAMES")]
		[SerializeField] private GameObject _objScrollMostRecentGames;
		[SerializeField] private Transform _objParentForMostRecentCards;
		[SerializeField] private GameObject _prefabGameCardMostRecent;
        public Text ButtonJoin;
		private RectTransform rtMost = new RectTransform();
		private List<GameCardMostRecent> cardsMostRecentList;

        [Header("FRIENDS")]
        [SerializeField]
        private GameObject _objScrollFriends;
        [SerializeField]
        private Transform _objParentForFriend;
        [SerializeField]
        private GameObject _prefabGameCardFriends;
        private RectTransform rtFriend = new RectTransform();
        private List<FriendCard> cardsFriendsList;
        private Dictionary<string, string> FriendsDict;

        #endregion

        // dependences
        private GameManager _gameManager;
		private PlayerManager _playerManager;
		private StampTimerManager _stampTimerManager;
        private MainScreenStateManager _mainScreenStateManager;
        private UIManager _uiManager;
		private SplashScreen _splashScreen;
		private ChooseCarWindow _chooseCarWindow;
		private ChooseTrackWindow _chooseTrackWindow;
		private Header _header;
        private SafePlayerPrefs _safePlayerPrefs;
        private EnternetWindow _enternetWindow;

        public Transform carParent;
        private GameObject _podium;
        private GameObject _podiumNew;
        float s = 1;
        private int sign = 1;
        private bool isChecked;
        private Vector3 startSwipePos;
        public float rotateSpeed = 25f;
        private Dictionary<string,int> previousGameUpdates = new Dictionary<string, int>();
        [SerializeField]
        private Banner banner;
        // I_UI_WINDOW

        public void Init()
		{
			AssertSerializedFields();

			//
			_gameManager = Core.Instance.GetService<GameManager>();
			Assert.AreNotEqual(null, _gameManager);

			_playerManager = Core.Instance.GetService<PlayerManager>();
			Assert.AreNotEqual(null, _playerManager);

			_stampTimerManager = Core.Instance.GetService<StampTimerManager>();
			Assert.AreNotEqual(null, _stampTimerManager);

			//
			_uiManager = Core.Instance.GetService<UIManager>();
			Assert.AreNotEqual(null, _uiManager);

			_splashScreen = _uiManager.GetWindow(UIWindowEnum.SPLASH) as SplashScreen;
			Assert.AreNotEqual(null, _splashScreen);

			_chooseCarWindow = _uiManager.GetWindow(UIWindowEnum.CHOOSE_CAR) as ChooseCarWindow;
			Assert.AreNotEqual(null, _chooseCarWindow);

			_chooseTrackWindow = _uiManager.GetWindow(UIWindowEnum.CHOOSE_TRACK) as ChooseTrackWindow;
			Assert.AreNotEqual(null, _chooseTrackWindow);

			_header = _uiManager.GetWindow(UIWindowEnum.HEADER) as Header;
			Assert.AreNotEqual(null, _header);

            _mainScreenStateManager = _uiManager.Get_MainScreenStateManager();
            Assert.AreNotEqual(null, _mainScreenStateManager);

            _safePlayerPrefs = Core.Instance.GetService<SafePlayerPrefs>();
            Assert.AreNotEqual(null, _safePlayerPrefs);

            _enternetWindow = _uiManager.GetWindow(UIWindowEnum.IS_ENTERNET) as EnternetWindow;
            Assert.AreNotEqual(null, _enternetWindow);
            //
            rt = parent.GetComponent(typeof(RectTransform)) as RectTransform;
			rtMost = _objParentForMostRecentCards.GetComponent(typeof(RectTransform)) as RectTransform;
            rtFriend = _objParentForFriend.GetComponent(typeof(RectTransform)) as RectTransform;
            cardsList = new List<GameCard>();
			cardsMostRecentList = new List<GameCardMostRecent>();
            cardsFriendsList = new List<FriendCard>();

        }

		public void Show()
		{
            ShowBaner();



            Debug.Log("MultiplayerWindow Show");
            _uiManager.ShowCanvas(canvaseGroup);
			canvaseGroup.blocksRaycasts = true;

            
            s = 1;
            sign = 1;
            _carParent = GameObject.Find("CarParent").transform;
			CheckShowCarBG();
            //CheckEnternet();
            ShowPodium();

			bg.gameObject.SetActive(false);
			
			_header.UpdateUI();


			SetPanelState_MyGames();
            /*
            if (_gameManager.IsEntryPointLoad)
            {
                _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
            }
            */
           // Debug.Log("MultiplayerWindow");
           // _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
        }

        private void ShowBaner()
        {
            banner.ShowBaner();
        }

		public void Hide()
		{

            banner.HideBaner();
            Debug.Log("MultiplayerWindow Hide");
            _uiManager.HideCanvas(canvaseGroup);
            canvaseGroup.blocksRaycasts = false;
            isChecked = false;
            
            Destroy(currentCar);
			currentCar = null;

            bg.gameObject.SetActive(true);
        }



		// INTERFACES

		// used on button !
//	public void RefreshGames()
//	{
//		RemoveFromList();
//		_gameManager.LoadGames();
//
//	}

        private void ShowPodium()
        {
            _podium = _carParent.Find("Podium").gameObject;
            _podiumNew = _carParent.Find("PodiumNew").gameObject;
            _podiumNew.transform.rotation = _podium.transform.rotation;
            _podiumNew.SetActive(true);
            _podium.SetActive(true);
        }
        
        void InstantiateCar(GameObject go, Quaternion rot)//, CarModel model)
        {
            //Quaternion rot;


            currentCar = GameObject.Instantiate(go,
                new Vector3(
                    _carParent.transform.position.x,
                    _carParent.transform.position.y - 0.1374124f,
                    _carParent.transform.position.z
                    )
                , _chooseCarWindow.rot) as GameObject;

            currentCar.transform.SetParent(_carParent);

            // currentCar.AddComponent<SphereCollider>();
            //currentCar.GetComponent<SphereCollider>().radius = 5f;

            //currentCar.GetComponent<Rigidbody>().useGravity = false;
            currentCar.GetComponent<Rigidbody>().useGravity = true;
            currentCar.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
            currentCar.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
            currentCar.GetComponent<CarBase>().controller.enabled = true;
            currentCar.GetComponent<CarBase>().SetColorInGame(_playerManager.selectedCar.current_color);



            /*
            var selectedCarUpgrades = DataModel.Instance.GetUpgradesByCar(model.CarType, model.level);

            foreach (var value in Enum.GetValues(typeof(UpgradeType)))
            {
                if (selectedCarUpgrades.Find(up => up.UpgradeType == (UpgradeType)value) != null)
                {
                    typesList.Add(((UpgradeType)value));
                }
            }

            carNameToBuy.text = model.name;
            carNameToSell.text = model.name;
            */
            /*
            if (pcmCandidate != null)
            {
                currentCar.GetComponent<CarBase>().SetColorInGame(pcmCandidate.current_color);
                currentCar.GetComponent<CarBase>().ApplyCarUpgradeValues(pcmCandidate.GetPlayerUpgrades());
            }

          */
        }

        public void CheckMyGamesCounter(List<GameData> gamesList, int i) {
            if (previousGameUpdates.ContainsKey(gamesList[i].gameId))
            {
                if (
                    previousGameUpdates[gamesList[i].gameId] < Convert.ToInt32(gamesList[i].last_update)
                    && gamesList[i].GameType == GameTypeEnum.MultyReplay
                    )
                {
                    _counterMyGamesCanvas.alpha = 1;
                    _counterMyGamesValue += 1;
                    _counterMyGamesText.text = _counterMyGamesValue.ToString();

                }
            }
            previousGameUpdates[gamesList[i].gameId] = Convert.ToInt32(gamesList[i].last_update);

        }

        public void UpdateMyGamesCounter() {
            _counterMyGamesCanvas.alpha = 1;
            _counterMyGamesValue += 1;
            _counterMyGamesText.text = _counterMyGamesValue.ToString();
        }

        public void AddGamesToList(List<GameData> gamesList)
		{
            _counterMyGamesCanvas.alpha = 0;
            _counterMyGamesValue = 0;

            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 0);
			rtMost.sizeDelta = new Vector2(rt.sizeDelta.x, 0);
			for (int i = 0; i < gamesList.Count; i++)
			{
                CheckMyGamesCounter(gamesList, i);
                CreateCardForMyGames(gamesList, i);
				CreateCardForMostRecentGames(gamesList, i);

                

            }
            //Debug.Log("AddGamesToList");
		}

        public void AddFriendsToList(Dictionary<string,string> FriendsDict)
        {
            rtFriend.sizeDelta = new Vector2(rt.sizeDelta.x, 0);
            List<string> KeysList = new List<string>(FriendsDict.Keys);
            
            for (int i = 0; i < FriendsDict.Count; i++)
            {
                CreateCardForFriends(FriendsDict, i, KeysList[i]);
            }
        }

        public void RemoveFromList()
		{
			for (int i = 0; i < cardsList.Count; i++)
			{
				cardsList[i].OnPlayClick -= PlayGame;
			}

			cardsList.Clear();

			for (int i = 0; i < parent.childCount; i++)
			{
				Destroy(parent.GetChild(i).gameObject);
			}
		}

        public void RemoveFromFriendDict()
        {
            FriendsDict.Clear();

            for (int i = 0; i < _objParentForFriend.childCount; i++)
            {
                Destroy(_objParentForFriend.GetChild(i).gameObject);
            }
        }

        #region CARDS - MY GAMES

        private void CreateCardForMyGames(List<GameData> gamesList, int i)
		{
			if (IsMyGames(gamesList, i))
			{
				GameObject go = Instantiate(card) as GameObject;

				var gCard = go.GetComponent<GameCard>();
				Assert.AreNotEqual(null, gCard);

				cardsList.Add(gCard);

				go.transform.SetParent(parent);
				go.transform.localScale = Vector3.one;

				//
				FillCardDataFoMyGames(gCard, gamesList[i]);

				rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y + 130);
				rt.transform.localPosition = new Vector3(rt.transform.localPosition.x, rt.rect.y, rt.transform.localPosition.z);
			}
		}

		private void FillCardDataFoMyGames(GameCard gCard, GameData gData)
		{
			//
			gCard.OnPlayClick += PlayGame;

			// game ID
			gCard.Set_GameID( gData );

			// enemy name
			var enemyName =
				(_playerManager.PlayerId == gData.player1_Id)
					? (gData.player2_name) : (gData.player1_name);

			gCard.enemyName.text = enemyName;

			// player score
			gCard.youScoreLabel.text =
				(_playerManager.PlayerId == gData.player1_Id)
					? (gData.player1_score.ToString() ) : (gData.player2_score.ToString() );

			// enemy score
			gCard.enemyScoreLabel.text =
				(_playerManager.PlayerId == gData.player1_Id)
					? (gData.player2_score.ToString() ) : (gData.player1_score.ToString() );

			// map name
			string trackIdString = gData.track_id;
			int trackId = int.Parse(trackIdString);
			string trackName = _chooseTrackWindow.trackNames[trackId];

			gCard.Set_MapName(trackName);

			// race date
			int lastUpdate = int.Parse(gData.last_update);
			string correctedRaceTime = GetCorrectedRaceTime(lastUpdate);

			gCard.Set_TextRaceDate(correctedRaceTime);

           

            switch (gData.GameType)
			{
				case GameTypeEnum.MultyRecord:
                    if(!_safePlayerPrefs.IsGameId(gData.gameId))
					gCard.SetRecordView();
					break;

				case GameTypeEnum.MultyReplay:
					gCard.SetReplayView();
					// message smiles
					List<int> idSmiles = gData.smiles;
					if (idSmiles != null) {
						gCard.FillMessageWithSmiles(idSmiles); }
					break;
			}
            // continue or not 
            int buttonShow = 1;
            string buttonText = "PLAY";

            if (_safePlayerPrefs.IsGameId( gData.gameId))
            {
                buttonShow = 2;
                buttonText = "CONTINUE";
            }
            gCard.Set_TextButtonPlay(buttonShow, buttonText);
        }

		#endregion

		#region CARDS - MOST RECENT GAMES

		private void CreateCardForMostRecentGames(List<GameData> gamesList, int i)
		{
			if (gamesList[i].GameType == GameTypeEnum.MultyJoin)
			{
				GameObject go = Instantiate(_prefabGameCardMostRecent) as GameObject;

				var mostRecentCard = go.GetComponent<GameCardMostRecent>();
				Assert.AreNotEqual(null, mostRecentCard);

				cardsMostRecentList.Add(mostRecentCard);

				go.transform.SetParent(_objParentForMostRecentCards);
				go.transform.localScale = Vector3.one;

				//
				FillCardDataForMostRecentGames(mostRecentCard, gamesList[i]);
				rtMost.sizeDelta = new Vector2(rtMost.sizeDelta.x, rtMost.sizeDelta.y + 130);
				rtMost.transform.localPosition = new Vector3(rtMost.transform.localPosition.x, rtMost.rect.y, rtMost.transform.localPosition.z);
			}
		}

		private void FillCardDataForMostRecentGames(GameCardMostRecent gCard, GameData gData)
		{
			//
			//cardsMostRecentList[i].OnPlayClick += PlayGame;

			// game ID
			gCard.Set_GameID(gData);

			// enemy mame
			string enemyName =
				(_playerManager.PlayerId == gData.player1_Id)
					? (gData.player2_name) : (gData.player1_name);

			gCard.Set_TetxEnemyName(enemyName);

			// map name
			string trackIdString = gData.track_id;
			int trackId = int.Parse(trackIdString);
			string trackName = _chooseTrackWindow.trackNames[trackId];

			gCard.Set_MapName(trackName);

			// race date
			int lastUpdate = int.Parse(gData.last_update);
			string correctedRaceTime = GetCorrectedRaceTime(lastUpdate);

			gCard.Set_TextRaceDate(correctedRaceTime);

            // continue or not
            string buttonName = "JOIN";
            if (_safePlayerPrefs.IsGameId(gData.gameId))
                buttonName = "CONTINUE";
            gCard.Set_TextButtonJoin(buttonName);

			// ------------------------------------------
			switch (gData.GameType)
			{
				//
				case GameTypeEnum.MultyRecord:
					//cardsList[i].SetRecordView();
					break;

				//
				case GameTypeEnum.MultyJoin:
					//cardsList[i].SetReplayView();
					// message smiles
					List<int> idSmiles = gData.smiles;
					if (idSmiles != null) {
						gCard.FillMessageWithSmiles(idSmiles); }
					break;
			}
			// ------------------------------------------
		}

		public void RemoveFromMostRecentGamesList()
		{
			cardsMostRecentList.Clear();

			for (int i = 0; i < _objParentForMostRecentCards.childCount; i++)
			{
				GameObject child = _objParentForMostRecentCards.GetChild(i).gameObject;
				Destroy(child);
			}
		}

		private string GetCorrectedRaceTime(int lastUpdate)
		{
			float rawTime = (_stampTimerManager.nowStamp - lastUpdate) / 60f;
			TimeSpan parsedTime = TimeSpan.FromSeconds(_stampTimerManager.nowStamp - lastUpdate);
			string result = "";

			if (rawTime > 1f)
			{
				result = string.Format("{0:D2}h:{1:D2}m ago", parsedTime.Hours, parsedTime.Minutes);
			}
			else
			{
				result = string.Format("00h:01m ago");
			}

			return result;
		}

        #endregion

        #region CARDS - FRIENDS

        private void CreateCardForFriends(Dictionary<string,string> FriendsDict, int i, string key)
        {


            GameObject go = Instantiate(_prefabGameCardFriends) as GameObject;

            var fCard = go.GetComponent<FriendCard>();
            Assert.AreNotEqual(null, fCard);

            cardsFriendsList.Add(fCard);

            go.transform.SetParent(_objParentForFriend);
            go.transform.localScale = Vector3.one;

            FillCardDataForFriends(fCard, FriendsDict[key], key);
            rtFriend.sizeDelta = new Vector2(rtFriend.sizeDelta.x, rtFriend.sizeDelta.y + 130);
            rtFriend.transform.localPosition = new Vector3(rtFriend.transform.localPosition.x, rtFriend.rect.y, rtFriend.transform.localPosition.z);

        }

        private void FillCardDataForFriends(FriendCard fCard, string Friend_name, string key)
        {
            //
            fCard.OnPlayClick += OnClickButton_NewGameForFB;
     
            fCard.FriendName.text = Friend_name;
            fCard.FriendID.text = key;
     
        }

        #endregion

        public void Back()
		{
			Core.Instance.GetService<StatesManager>().SwitchState(StatesEnum.Auth);
		}

		public void ExitGame()
		{
			//Debug.Log("EXIT");
			Application.Quit();
		}

		#region BUTTONS

		// used on button !
        public void OnClick_Event()
        {
            _mainScreenStateManager.SwitchState(MainScreenStatesEnum.EVENT);
        }

		public void OnClickButton_SilglepNew()
		{
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);

            if (OnSingleNew != null) {
				OnSingleNew(); }
		}

		// used on button !
		public void OnClickButton_NewGame()
		{
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);

			if (OnNewGame != null) {
				OnNewGame(); }
		}

        public void OnClickButton_NewGameForFB(string player2_id, string player2_name)
        {
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
            if (OnNewGameForFB != null)
            {
                OnNewGameForFB(player2_id, player2_name);
            }
        }

        // used on button !
        public void OnClickButton_RandomGame()
		{
            
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
            if (OnRandomGame != null) {
				OnRandomGame(); }
		}

        // --------------------------------
        public void PlayGame(string selectedGameId)
        {
            Debug.Log("PlayGame ");
            if (OnPlay != null)
            {
                OnPlay(selectedGameId);
            }
        }

        // used on button !
        public void OnClickButton_MyGames()
		{
            
            if (!_gameManager.ButtAsyncOn)
            {
                if (_mainScreenStateManager.getCurrentState().ToString() != "GAME_ASYNC")
                {
                    _splashScreen.ShowLoadingPanel();

                    Debug.Log("OnClickButton_MyGames ");
                    SetPanelState_MyGames();
                    RemoveFromList();
                    RemoveFromMostRecentGamesList();
                    _gameManager.LoadGames();
                }
            }
        }

		// used on button !
		public void OnClickButton_MostRecentGames()
        {
            if (!_gameManager.ButtAsyncOn)
            {
                if (_mainScreenStateManager.getCurrentState().ToString() != "GAME_ASYNC")
                {
                    _splashScreen.ShowLoadingPanel();

                    Debug.Log("OnClickButton_MyGames ");
                    SetPanelState_MostRecentGames();
                    RemoveFromList();
                    RemoveFromMostRecentGamesList();
                    _gameManager.LoadGames();
                }
               
            }
            
		}

        public void OnClickButton_Friends()
        {
            FriendsDict = new Dictionary<string, string>();

            new ListGameFriendsRequest().Send((response) =>
            {
                GSEnumerable<ListGameFriendsResponse._Player> friends = response.Friends;
                GSData scriptData = response.ScriptData;


                foreach (var i in friends)
                {
                    FriendsDict.Add(i.Id, i.DisplayName);
                }

                _gameManager.OnFriendsUpdate(FriendsDict);
                
                SetPanelState_Friends();
                

            });

            RemoveFromFriendDict();

        }
        
        void Update()
        {
            if (currentCar != null && _podiumNew.activeSelf
                 )
            {
                _podiumNew.transform.rotation = currentCar.transform.rotation;
                _chooseCarWindow.rot = currentCar.transform.rotation;
                _podiumNew.transform.localPosition = new Vector3(
                    currentCar.transform.localPosition.x,
                    _podiumNew.transform.localPosition.y,
                    currentCar.transform.localPosition.z
                    );
            }
            if (canvaseGroup.alpha > 0)
            {
                UserInput();
            }
            else
            {
                return;
            }
        }

         void UserInput()
        {
            if (!isChecked)
            {
                if (s > 1)
                {
                    s -= rotateSpeed / 100;
                }

                if (currentCar != null)
                {
                    currentCar.transform.Rotate(0, rotateSpeed * Time.deltaTime * sign * s, 0);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.transform.gameObject.tag == "Garage")
                    {
                        isChecked = true;
                        startSwipePos = Input.mousePosition;
                    }
                }
            }

            if (Input.GetMouseButton(0) && isChecked)
            {
                if ((startSwipePos.x - Input.mousePosition.x) == 0)
                {
                    return;
                }

                currentCar.transform.Rotate(0, rotateSpeed / 2 * Time.deltaTime * (startSwipePos.x - Input.mousePosition.x), 0);
                sign = (startSwipePos.x - Input.mousePosition.x) > 0 ? 1 : -1;
                s = Mathf.Clamp((Mathf.Abs(startSwipePos.x - Input.mousePosition.x)), 5, 20);
                startSwipePos = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isChecked = false;
            }


        }
        
        public void OnClickButton_InviteMoreFriends()
        {
            //FB.Mobile.AppInvite(new Uri("https://fb.me/" + FB.AppId.ToString()));
        }

        #endregion
        // METHODS

        private void AssertSerializedFields()
		{
			Assert.AreNotEqual(null, _buttonSingleplayer);
			Assert.AreNotEqual(null, _buttonNewGame);
			Assert.AreNotEqual(null, _buttonRandomGame);
			Assert.AreNotEqual(null, _buttonMyGames);
			Assert.AreNotEqual(null, _buttonMostRecentGames);

			Assert.AreNotEqual(null, _objScrollMyGames);
			Assert.AreNotEqual(null, parent);
			Assert.AreNotEqual(null, card);

			Assert.AreNotEqual(null, _objScrollMostRecentGames);
			Assert.AreNotEqual(null, _objParentForMostRecentCards);
			Assert.AreNotEqual(null, _prefabGameCardMostRecent);
		}

		private void CheckShowCarBG()
		{
            // -------------------------------------------------------------------------------------
           
            if (_chooseCarWindow.car != null)
			{
				var ca = _playerManager.playerCars.Find(c => c.carType == _chooseCarWindow.car.CarType && c.carLevel == _chooseCarWindow.car.level);

                if (ca != null)
                {
                    fade.SetActive(true);
                    SetCarToBG(ca, ca.car_upgrade_level);
					UnblockAllButtons();
				}
				else
				{
					if (_playerManager.selectedCar != null)
					{
                        
                        fade.SetActive(true);
						ca = _playerManager.playerCars.Find(c => c.carType == _playerManager.selectedCar.carType && c.carLevel == _playerManager.selectedCar.carLevel);
						SetCarToBG(ca, ca.car_upgrade_level);
						_playerManager.selectedCar = ca;
                        Debug.LogError("SetCurrentCur");
                        _playerManager.currentCar = _playerManager.stockCarsList.Find(c => c.CarType == _playerManager.selectedCar.carType && c.level == _playerManager.selectedCar.carLevel);

                        UnblockAllButtons();
					}
					else
					{
						BlockAllButtons();
					}
				}
			}
			// -------------------------------------------------------------------------------------
			//
			else
			{
				if (_playerManager.currentCar != null)
				{
                    fade.SetActive(true);
					var ca = _playerManager.playerCars.Find(c => c.carType == _playerManager.currentCar.CarType && c.carLevel == _playerManager.currentCar.level);
                    //ca.car_upgrade_level = _playerManager.selectedCar.car_upgrade_level;
                    
                    _playerManager.selectedCar = ca;
					 
                    try
                    {
                        SetCarToBG(ca, ca.car_upgrade_level);
                    }
                    catch (Exception)
                    {
                        Debug.Log("Exception");
                        throw;
                    }
					UnblockAllButtons();
				}
				else
				{
					fade.SetActive(false);
					BlockAllButtons();
				}
			}
            // -------------------------------------------------------------------------------------
        }
        /*
        private void CheckEnternet()
        {
            Core.Instance.IsNetwork((Done) =>
            {
                if (!Done)
                {
                    BlockAllButtons(); 
                }
            });
        }
        */

        private void BlockAllButtons()
		{
			_buttonRandomGame.interactable = false;
			_buttonNewGame.interactable = false;
			_buttonMyGames.interactable = false;
			_buttonMostRecentGames.interactable = false;
		}

		private void UnblockAllButtons()
		{
			_buttonRandomGame.interactable = true;
			_buttonNewGame.interactable = true;
			_buttonMyGames.interactable = true;
			_buttonMostRecentGames.interactable = true;
		}

        private void SetCarToBG(PlayerCarModel car, int up_lvl)
		{
			Quaternion rot = Quaternion.Euler(3f,-133f,-2f);
            
			var go = Resources.Load("RaceCarsPrefabs/" + car.GetPrefabName(up_lvl)) as GameObject;
			if (go != null)
			{
                InstantiateCar(go, rot);
                /*
				currentCar = GameObject.Instantiate(go, _carParent.position, rot) as GameObject;
                var pcm = _playerManager.playerCars.Find(c => c.carLevel == car.carLevel && c.carType == car.carType);
				currentCar.GetComponent<CarBase>().SetColorInGame(_playerManager.selectedCar.current_color);
				currentCar.transform.SetParent(_carParent);
				currentCar.transform.localPosition = new Vector3(-0.9f, -1f, 1.15f);
				currentCar.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
				currentCar.GetComponent<Rigidbody>().isKinematic = true;
                */
            }
			else
			{
				go = Resources.Load("RaceCarsPrefabs/" + car.GetPrefabName(up_lvl - 1)) as GameObject;
                InstantiateCar(go, rot);
                /*
				currentCar = GameObject.Instantiate(go, _carParent.position, rot) as GameObject;
				currentCar.transform.SetParent(_carParent);
				currentCar.transform.localPosition = new Vector3(-0.9f, -1f, 1.15f);
				currentCar.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
				currentCar.GetComponent<Rigidbody>().isKinematic = true;
                */
            }


		}

		private bool IsMyGames(List<GameData> gamesList, int i)
		{
			return 	(gamesList[i].GameType == GameTypeEnum.MultyRecord ||
			       	 gamesList[i].GameType == GameTypeEnum.MultyReplay ||
			       	 gamesList[i].GameType == GameTypeEnum.MultyWait);
		}

		private void SetPanelState_MyGames()
		{
			_objScrollMyGames.SetActive(true);
			_objScrollMostRecentGames.SetActive(false);
            _objScrollFriends.SetActive(false);
            _buttonInviteMoreFriends.gameObject.SetActive(false);

            this.parent.gameObject.SetActive(true);
			_objParentForMostRecentCards.gameObject.SetActive(false);
		}

		private void SetPanelState_MostRecentGames()
		{
			_objScrollMostRecentGames.SetActive(true);
			_objScrollMyGames.SetActive(false);
            _objScrollFriends.SetActive(false);
            _buttonInviteMoreFriends.gameObject.SetActive(false);

            _objParentForMostRecentCards.gameObject.SetActive(true);
			this.parent.gameObject.SetActive(false);
		}

        private void SetPanelState_Friends()
        {
            _objScrollMostRecentGames.SetActive(false);
            _objScrollMyGames.SetActive(false);
            _objScrollFriends.SetActive(true);
            _buttonInviteMoreFriends.gameObject.SetActive(true);

            _objParentForFriend.gameObject.SetActive(true);
            this.parent.gameObject.SetActive(false);
        }

    }
}