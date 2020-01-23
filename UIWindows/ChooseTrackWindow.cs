using UnityEngine;
using System.Collections.Generic;
using HCR.Enums;
using HCR.Interfaces;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;

namespace HCR.GlobalWindow.MainMenu
{
    /// <summary>
    /// Класс - окно "Выбор трассы"
    /// </summary>

    public class ChooseTrackWindow : MonoBehaviour, IUIWindow
    {
        // FIELDS

        public CanvasGroup canvaseGroup;

    
        //
        public List<Sprite> trackList;
        public List<Sprite> newTrackList;
        //[HideInInspector]
        public List<string> trackNames;
        
        public Button BuyTrack;
        public Text trackName;
        public Text trackNumber;
        public Text BestTime;
        public Image ImBestTime;
        public Image parent;
        public Image ClosedTrack;

        public Text NotHaveMoney;
        public Text trackTimeText;
        public Text trackBuyText;
        public Button buyTrackBtn;

        private int silverCount;
        private int goldCount;
        private int carPrice;

        public Button nextBtn;
        public Button prevBtn;
        public Button backBtn;
        public Button applyBtn;

        public Text First;
        public Text Second;
        public Text Third;

        private int iterator;
        public  string IsChooseTrack = "";

        // dependences
        private GameManager _gameManager;
        private PlayerManager _playerManager;
        private UIManager _uiManager;
        private MainScreenStateManager _mainScreenStateManager;
        private MultiplayerWindow _multiplayerWindow;
        private Header _header;



        // I_UI_WINDOW

        public void Init()
        {
            _gameManager = Core.Instance.GetService<GameManager>();
            Assert.AreNotEqual(null, _gameManager);

            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);

            _mainScreenStateManager = _uiManager.Get_MainScreenStateManager();
            Assert.AreNotEqual(null, _mainScreenStateManager);

            _multiplayerWindow = _uiManager.GetWindow(UIWindowEnum.MAIN_MULTIPLAYER) as MultiplayerWindow;
            Assert.AreNotEqual(null, _multiplayerWindow);

            _header = _uiManager.GetWindow(UIWindowEnum.HEADER) as Header;
            Assert.AreNotEqual(null, _header);

            _playerManager = Core.Instance.GetService<PlayerManager>();
            Assert.AreNotEqual(null, _playerManager);

            nextBtn.onClick.AddListener(NextTrack);
            prevBtn.onClick.AddListener(PreviousTrack);

            applyBtn.onClick.AddListener(Apply);
            buyTrackBtn.onClick.AddListener(BuyTrackClick);
            BestTime.text = "";
            ImBestTime.gameObject.SetActive(false);

            _gameManager.tracksSettingsLoaded += LoadSetting;



        }
        public void LoadSetting()
        {
            _gameManager.tracksSettingsLoaded -= LoadSetting;

            newTrackList = new List<Sprite>();


            foreach (var name in _gameManager.TrackNamesById)
            {
                foreach (Sprite item in trackList)
                {
                    if (item.name == name.Value)
                    {
                        newTrackList.Add(item);
                    }
                }
            }

            trackList = newTrackList;

            trackNames = new List<string>();
            
            foreach (var name in _gameManager.TrackTimeById)
            {
                trackNames.Add(name.nameTrack);
            }

        }

        public void Show()
        {
            //Debug.LogError("Show CTW ");
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
            //canvaseGroup.alpha = 1;
            _uiManager.ShowCanvas(canvaseGroup);
            canvaseGroup.blocksRaycasts = true;
            iterator = 0;
            for (int track = 0; track < trackList.Count ; track++)
            {
                Debug.Log(_playerManager.TrackOpen.Count + "  " + trackList.Count+ " " + track);
                if (_playerManager.TrackOpen.Contains(track))
                {
                    iterator = track;
                }
            }
         

            parent.sprite = trackList[iterator];
            trackName.text = trackNames[iterator];
            trackNumber.text = iterator + 1 + "/" + trackList.Count;
            
            BuyTrack.gameObject.SetActive(false);
            applyBtn.gameObject.SetActive(true);
            ClosedTrack.gameObject.SetActive(false);
            CheckBestTime();
            UpdateLeader();
            

        }

        private void UpdateLeader()
        {   string name = _playerManager.LeaderBoard[iterator]["first"].name;
            int time = _playerManager.LeaderBoard[iterator]["first"].time;
           
            First.text = name + ": " + new parz().result(time);

            name = _playerManager.LeaderBoard[iterator]["second"].name;
           time = _playerManager.LeaderBoard[iterator]["second"].time;

            Second.text = name + ": " + new parz().result(time);

            name = _playerManager.LeaderBoard[iterator]["third"].name;
           time = _playerManager.LeaderBoard[iterator]["third"].time;

            Third.text = name + ": " + new parz().result(time);
        }

        private void CheckBestTime()
        {
            
           
            if (_playerManager.TrackBestTime[iterator] != 90000)
            {
                ImBestTime.gameObject.SetActive(true);
                int time = _playerManager.TrackBestTime[iterator];
                
                BestTime.text = "Best time : " + new parz().result(time);
            }
        }

        public void Hide()
        {
            //Debug.Log("Hide CTW ");
            //canvaseGroup.alpha = 0;
            _uiManager.HideCanvas(canvaseGroup);
            canvaseGroup.blocksRaycasts = false;
        }

        private void ShowCloseTrack()
        {
            BestTime.text = "";
            ImBestTime.gameObject.SetActive(false);
            if (!_playerManager.TrackOpen.Contains(iterator))
            {
                applyBtn.gameObject.SetActive(false);
                BuyTrack.gameObject.SetActive(true);
                if (_playerManager.gold < _gameManager.TrackTimeById[iterator].PriceGold)
                {
                    BuyTrack.interactable = false;
                }
                else { BuyTrack.interactable = true; }

                ClosedTrack.gameObject.SetActive(true);
                NotHaveMoney.gameObject.SetActive(false);

               int time =  _gameManager.TrackTimeById[iterator - 1].Time;
                trackTimeText.text = new parz().result(time * 100);//TimeSpan.FromSeconds(time * 60).ToString();
               int priceGold =_gameManager.TrackTimeById[iterator].PriceGold;
                trackBuyText.text = priceGold.ToString();
                
            }
            else
            {
                CheckBestTime();
                BuyTrack.gameObject.SetActive(false);
                applyBtn.gameObject.SetActive(true);
                ClosedTrack.gameObject.SetActive(false);

            }
            
        }

        public void NextTrack()
        {


            iterator++;

            if (iterator > trackList.Count - 1)
            {
                iterator = 0;
            }

            trackNumber.text = iterator + 1 + "/" + trackList.Count;
            parent.sprite = trackList[iterator];
            ShowCloseTrack();
            trackName.text = trackNames[iterator];
            UpdateLeader();


        }


        public void PreviousTrack()
        {

            iterator--;
            if (iterator < 0)
            {
                iterator = trackList.Count + iterator;
            }

            trackNumber.text = iterator + 1 + "/" + trackList.Count;
            parent.sprite = trackList[iterator];
            ShowCloseTrack();
            trackName.text = trackNames[iterator];
            UpdateLeader();


        }


        public void Back()
        {

        }


        public void Apply()
        {
            
            //this.Hide();
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
            _header.canvaseGroup.interactable = true;
            Debug.Log("IsChooseTrack" + IsChooseTrack);
            if (IsChooseTrack != "")
            {
                _gameManager.ApplyTrack(iterator, IsChooseTrack);
                IsChooseTrack = "";
            }
            else
                _gameManager.ApplyTrack(iterator);



        }
        private IEnumerator MesgMoney()
        {

           NotHaveMoney.gameObject.SetActive(true);
            

            yield return new WaitForSecondsRealtime(3);
           

            NotHaveMoney.gameObject.SetActive(false);

        }
        private  void MesgMoney2()
        { NotHaveMoney.gameObject.SetActive(false); }

        private void BuyTrackClick()
        {
           
            Core.Instance.GetService<NetworkManager>().BuyTrack(iterator, (msg) =>
            {
                switch (msg)
                {
                    case 0:
                        //_playerManager.TrackOpen.Add(iterator);
                        BuyTrack.gameObject.SetActive(false);
                        applyBtn.gameObject.SetActive(true);
                        ClosedTrack.gameObject.SetActive(false);
                        _header.UpdateUI();
                        ;
                        break;
                    case 2:
                        Core.Instance.StartCor(MesgMoney());
                        break;
					case 4:
						 //_playerManager.TrackOpen.Add(iterator);
                        BuyTrack.gameObject.SetActive(false);
                        applyBtn.gameObject.SetActive(true);
                        ClosedTrack.gameObject.SetActive(false);
                        ;
                        break;
                    default:
                        break;
                }
                
                

            });



        }

    }
}