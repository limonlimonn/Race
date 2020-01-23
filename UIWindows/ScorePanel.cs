using DG.Tweening;
using HCR.Enums;
using HCR.Interfaces;
using HCR.Loading;
using HCR.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HCR.GlobalWindow.MainMenu
{
    public class ScorePanel : MonoBehaviour, IUIWindow
    {
        [SerializeField]
        private SpriteRenderer _background;
        public CanvasGroup _canvasGroup;
        private UIManager _uiManager;
        private StatisticPanel _statisticPanel;
        private MainScreenStateManager _mainScreenStateManager;
        private UIAnimatorManager _uiAnimatorManager;
        private NetworkManager _networkManager;
        private Header _header;
        private EventService _eventService;
        private EnternetWindow _enternetWindow;
        private SplashScreen _splashScreen;

        public Dictionary<int, Dictionary<string, int>> Shop = new Dictionary<int, Dictionary<string, int>>();

        public Text[] ArrHeader;
        public Text[] ArrBottom;
        public Image Thank;
        public Image BuyMore;
        public Image Random;
        public Image Bonus;
        private PlayerManager _playerManager;
        public Sprite[] ImageRandom;
        public Text RandomText;
        public Text WaitText;
        public Image HideRandomImage;
        private Coroutine StartTimeWait;
        public Button RandomButt;
        public Scrollbar _scroll;
        public GameObject BlockPanel;
        int index = 0; // 0 - 2
        float kase = 20;
        int CountBonus = 0;
        int TypeBonus = 0;
        int stampEndWait = 0;
        public void Hide()
        {
            //_canvasGroup.alpha = 0;
            _uiManager.HideCanvas(_canvasGroup);
            _canvasGroup.blocksRaycasts = false;
            _background.gameObject.SetActive(false);
        }

        public void Init()
        {   _playerManager = Core.Instance.GetService<PlayerManager>();
            Assert.AreNotEqual(null, _playerManager);

            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);

            _mainScreenStateManager = _uiManager.Get_MainScreenStateManager();
            Assert.AreNotEqual(null, _mainScreenStateManager);

            _networkManager = Core.Instance.GetService<NetworkManager>();
            Assert.AreNotEqual(null, _networkManager);

            _header = _uiManager.GetWindow(Enums.UIWindowEnum.HEADER ) as Header;
            Assert.AreNotEqual(null, _header);

            _statisticPanel = _uiManager.GetWindow(UIWindowEnum.STATISTIC) as StatisticPanel;
            Assert.AreNotEqual(null, _statisticPanel);

            _uiAnimatorManager = Core.Instance.GetService<UIAnimatorManager>();
            Assert.AreNotEqual(null, _uiAnimatorManager);

            _eventService = Core.Instance.GetService<EventService>();
            Assert.AreNotEqual(null, _eventService);

            _enternetWindow = _uiManager.GetWindow(UIWindowEnum.IS_ENTERNET) as EnternetWindow;
            Assert.AreNotEqual(null, _enternetWindow);

            _splashScreen = _uiManager.GetWindow(UIWindowEnum.SPLASH) as SplashScreen;
            Assert.AreNotEqual(null, _splashScreen);

        }
        
        public void Show()
        {
            AdMobManager.adMob.InitVideo(GetRandomReward, () => { });
            StartWait();
            //_canvasGroup.alpha = 1;
            _uiManager.ShowCanvas(_canvasGroup);
            _canvasGroup.blocksRaycasts = true;
            _scroll.value = 1;
            _background.gameObject.SetActive(true);
            _background.sprite = _uiManager.ChangeBG(UIWindowEnum.SETTINGS);
            //Debug.LogError("DeleteKey  JSON");
            //PlayerPrefs.DeleteKey(PPKeys.JSON);

            BlockPanel.SetActive(false);
        }
        
        public void FillShop()
        {
            
            Shop = _eventService.Shop;

            foreach(var block in Shop)
            {
                if (block.Key > 2)
                    ArrHeader[block.Key].text = "X " + block.Value["jewels"].ToString();
                else
                    ArrHeader[block.Key].text = "X " +  block.Value["gold"].ToString();

                ArrBottom[block.Key].text = block.Value["price"].ToString();
                if (block.Key > 2 && block.Key < 6 || block.Key == 9)
                    ArrBottom[block.Key].text = "$ " + ArrBottom[block.Key].text;
            }
            ArrHeader[6].text = "Day";
            ArrHeader[7].text = "Week";
            ArrHeader[8].text = "Month";
            ArrHeader[9].text = "Gold: " + Shop[9]["gold"].ToString() +
                " Jewels: " + Shop[9]["jewels"].ToString()
                + " Week";
            
        }

        #region BUTTON
        public void BuyButton(int id)
        {
            if (((id < 4 || id > 6)&& id != 10) && Shop[id -1]["price"] <= _playerManager.jewels)
            {
                BlockPanel.SetActive(true);
                BuyInNetwork(id);
            }
            else if (id > 3 && id < 7 || id  == 10 )
            {
                BlockPanel.SetActive(true);
                BuyInNetwork(id);
            }
            else
            {
                BlockPanel.SetActive(true);
                StartCoroutine(ShowImg(BuyMore, () => { BlockPanel.SetActive(false); }));
            }
        }

        private void BuyInNetwork(int id)
        {
            _splashScreen.ShowLoadingPanel();
            int accountType = _playerManager.accountType;
            _networkManager.BuyInScore(id, (msg) =>
            {
                _splashScreen.HideLoadingPanel();
                if (msg == 1)
                {
                    if (id > 6)
                    {
                        _header.GoldChanged();
                        _header.JewelChanged();
                        _statisticPanel.Init(); _header.UpdateAccoutSprite(() =>
                        {
                            StartCoroutine(ShowImg(Thank,() => { BlockPanel.SetActive(false); }));
                        });
                    }
                    else
                    {
                        StartCoroutine(ShowImg(Thank, () => { BlockPanel.SetActive(false); }));
                        _header.GoldChanged();
                        _header.JewelChanged();
                    }
                }
            } , (err)=> {
                if(err == "timeout")
                {
                    BlockPanel.SetActive(false);
                    _splashScreen.HideLoadingPanel();
                    _enternetWindow.ShowErrorEnternet();
                }
                else
                {
                    _splashScreen.HideLoadingPanel();
                    BlockPanel.SetActive(false);
                    Debug.LogError("BuyInNetwork err");
                }
                
            });
        }

        private void ShowReward()
        {
            AdMobManager.adMob.ShowVideo();
        }

        public void RandomClick()
        {
            ShowReward();
        }


        private void GetRandomReward()
        {
            BlockPanel.SetActive(true);
            _splashScreen.ShowLoadingPanel();
            Debug.LogError("GetRandomReward");

            index = 0;
            kase = 3;
            _networkManager.RandomInShop((type) => { TypeBonus = type; }, (count) => {
                _splashScreen.HideLoadingPanel();
                CountBonus = count; AnimationRandom();
            },
                (err) => {
                    if (err == "timeout")
                    {
                        _splashScreen.HideLoadingPanel();
                        BlockPanel.SetActive(false);
                        Debug.Log("RandomClick err");
                        _enternetWindow.ShowErrorEnternet();

                    }
                    else
                    {
                        _splashScreen.HideLoadingPanel();
                        BlockPanel.SetActive(false);
                        Debug.LogError("RandomClick err");
                    }


                });

        }
        #endregion


        #region random
        private void AnimationRandom()
        {
            Random.DOFade(0f, kase / 10).OnComplete(() => {
                Random.sprite = ImageRandom[index];
                index++;
                if (index == 3)
                {
                        kase--;
                    if (kase == 0)
                    { Random.sprite = ImageRandom[TypeBonus]; Random.DOFade(1f, 1f).OnComplete(ShowBonus); }
                    index = 0;
                }
                if (kase != 0)
                    Random.DOFade(1f, kase /10).OnComplete(AnimationRandom);

                    
                
               

            });
        }

        private void ShowBonus()
        {
            switch (TypeBonus)
            {
                case 0: ShowJewels();
                    break;
                case 1:
                    ShowGold();
                    break;
                case 2:
                    ShowAccount();
                    break;
                default:
                    break;
            }
            
        }

        private void ShowGold() {
            Bonus.sprite = ImageRandom[1];
            Bonus.gameObject.SetActive(true);
            _uiAnimatorManager.ShowTextFieldUpdateAniamtion(RandomText, (int)0, (int)CountBonus, () => {
               Core.Instance.StartCor(Wait(1f, () =>
                {
                    _uiAnimatorManager.ShowTextFieldUpdateAniamtion(RandomText, (int)CountBonus, (int)0);
                    _uiAnimatorManager.ShowTextFieldUpdateAniamtion(_header.gold, (int)_playerManager.gold - CountBonus, _playerManager.gold, () =>
                    {
                        Core.Instance.StartCor(Wait(1f, HideRandom));
                    });
                }));
            });
        }

        private void ShowJewels() {
            Bonus.sprite = ImageRandom[0];
            Bonus.gameObject.SetActive(true);
            _uiAnimatorManager.ShowTextFieldUpdateAniamtion(RandomText, (int)0, CountBonus, () => {
                Core.Instance.StartCor(Wait(1f, () =>
                {
                    _uiAnimatorManager.ShowTextFieldUpdateAniamtion(RandomText, (int)CountBonus, (int)0);
                    _uiAnimatorManager.ShowTextFieldUpdateAniamtion(_header.jewels, (int)_playerManager.jewels - CountBonus, (int)_playerManager.jewels, () =>
                    {
                        Core.Instance.StartCor(Wait(1f, HideRandom));
                    });
                }));
            });
        }

        private void ShowAccount() {
            RandomText.text = "1 Day";
            if (CountBonus == 0) { _header.UpdateAccoutSprite(() => { Core.Instance.StartCor(Wait(1f, HideRandom)); }); }
        }

        private void HideRandom()
        {
            Random.sprite = ImageRandom[3];
            RandomText.text = "Free";
            Bonus.gameObject.SetActive(false);
            BlockPanel.SetActive(false);
            StartWait();
        }

        private void StartWait()
        {
            if (!_playerManager.IsOnRandom && stampEndWait == 0)
            {
                RandomButt.interactable = false;
                HideRandomImage.gameObject.SetActive(true);
                stampEndWait = (_playerManager.stampRandom - _playerManager.stamp_now_user) / 60;
                StartTimeWait = Core.Instance.StartCor(StartTimeWaitRandom());
            }
            else if(_playerManager.IsOnRandom)
            {
                HideRandomImage.gameObject.SetActive(false);
                RandomButt.interactable = true;
            }
            }
        #endregion


        private IEnumerator StartTimeWaitRandom()
        {
            WaitText.text = Timer();
            while (stampEndWait > 0)
            {
                yield return new WaitForSecondsRealtime(59);
                stampEndWait--;
                WaitText.text = Timer();
            }
            if(StartTimeWait != null)
            Core.Instance.StopCor(StartTimeWait);
            HideRandomImage.gameObject.SetActive(true);
        }

        public string Timer()
        {
            string time = "";

            int hours = stampEndWait / 60;
            int min = stampEndWait - hours * 60;

            time = "Hours:" + (hours > 9 ? hours.ToString() : "0" + hours.ToString()) + " Min:" + (min > 9 ? min.ToString() : "0" + min.ToString());
            return time;
        }

        private IEnumerator Wait(float time, Action OnComplete )
        {
            yield return new WaitForSecondsRealtime(time);
            OnComplete();
        }

       private IEnumerator ShowImg(Image img,Action OnComplete)
        {
            //Debug.Log("img" + img.name);
            img.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            img.gameObject.SetActive(false);
            //Debug.Log("img set false"  );
            OnComplete();
        }

    }
}