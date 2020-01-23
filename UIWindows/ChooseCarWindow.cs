using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

using HCR.Enums;
using HCR.Interfaces;
using DG.Tweening;
using HCR.Network;

namespace HCR.GlobalWindow.MainMenu
{
    /// <summary>
    /// Класс - окно "Гараж" (выбор машины)
    /// </summary>

    public class ChooseCarWindow : MonoBehaviour, IUIWindow
    {
        // FIELDS

        #region VARIABLES

        public CanvasGroup canvaseGroup;
        public Action ShowTutorialOnBuyCar;
        public Action ShowTutorialCarUpdate;

        //
        public List<GameObject> carsPrefabs = new List<GameObject>();
        public List<PlayerCarModel> playerCars;

        [HideInInspector]
        public List<GameObject> carUpsList = new List<GameObject>();

        public List<ColorButton> colorsButtons = new List<ColorButton>();

        public Text carNameToBuy;
        public Text carNameToSell;
        public Text carPriceJewel;

        public SpriteRenderer bg;



        public Button buyCarBtn;
        public Button buyCarBtnJewel;
        public Button cancelBuyBtn;

        public Button buyUpgradeBtn;
        public Button cancelBuyUpgradeBtn;

        public Button buyColorBtn;
        public Button cancelBuyColorBtn;

        public GameObject upgradesBuyConteiner;
        public GameObject carBuyConteiner;
        public GameObject cantBuyConteiner;
        public GameObject colorBuyConteiner;
        public GameObject BlockButton;

        public RectTransform maxSpeed;
        public RectTransform accelerator;
        public RectTransform grip;
        public RectTransform handing;
        public RectTransform nitro;

        private GameObject currentCar;


        public Text seedLabel;
        public Text accelLabel;
        public Text gripLabel;
        public Text handlingLabel;
        public Text nitroLabel;

        public Text carPriceToBuy;
        public Text carPriceToSell;

        public GameObject upgradeItemPrefab;

        public float rotateSpeed = 35f;

        private bool isChecked;
        private Vector3 startSwipePos;
        private int sign = 1;
        private float s = 1;

        // private int silverCount;
        private int goldCount;
        private int jewelCount;
        private int carPrice;

        private float sliderSize = 0;
        private PlayerCarModel pcmCandidate;
        public GameObject loadingCircle;
        public Transform parennt;
        public CarModel car;
        private List<UpgradeType> typesList = new List<UpgradeType>();
        private UpgradeItem _item;
        private GameObject _podium;
        private GameObject _podiumNew;
        public Transform carParent;
        private EnternetWindow _enternetWindow;

        // -------------------------------------
        //
        [Space]
        [SerializeField]
        private Text _textCantBuyPrice;
        [SerializeField]
        private Text _textCarPriceToBuy;
        [SerializeField]
        private Text _textCarPriceJewelToBuy;
        [SerializeField]
        private Text _textUpgradePriceToBuy;

        #endregion

        // dependences
        private PlayerManager _playerManager;
        private GameManager _gameManager;

        private BackgroundMenu _backgroundMenu;

        private UIManager _uiManager;
        private MainScreenStateManager _mainScreenStateManager;
        private TreeWindow _treeWindow;

        private Header _header;
        private bool IsUpgrade = true;
        [SerializeField]
        private Banner banner;



        // I_UI_WINDOW

        public void Init()
        {
            _playerManager = Core.Instance.GetService<PlayerManager>();
            _gameManager = Core.Instance.GetService<GameManager>();

            _uiManager = Core.Instance.GetService<UIManager>();
            _mainScreenStateManager = _uiManager.Get_MainScreenStateManager();
            _treeWindow = _uiManager.GetWindow(UIWindowEnum.TREE_UPGRADE) as TreeWindow;
            _header = _uiManager.GetWindow(UIWindowEnum.HEADER) as Header;
            _enternetWindow = _uiManager.GetWindow(UIWindowEnum.IS_ENTERNET) as EnternetWindow;

            _backgroundMenu = _uiManager.GetWindow(UIWindowEnum.FIRSTTUTORIALWINDOW) as BackgroundMenu;


            buyCarBtn.onClick.AddListener(BuyCar);
            buyCarBtnJewel.onClick.AddListener(BuyCar);
            cancelBuyBtn.onClick.AddListener(GoToTree);

            buyColorBtn.onClick.AddListener(BuyColor);
            cancelBuyColorBtn.onClick.AddListener(() => {

                colorBuyConteiner.SetActive(false);
                currentCar.GetComponent<CarBase>().SetColorInGame(pcmCandidate.current_color);
            });

            buyUpgradeBtn.onClick.AddListener(BuyUpgrade);
            cancelBuyUpgradeBtn.onClick.AddListener(() => { upgradesBuyConteiner.SetActive(false); });

            _podium = carParent.Find("Podium").gameObject;
            _podiumNew = carParent.Find("PodiumNew").gameObject;
        }

        public void Show()
        {
             banner.ShowBaner();
            _podiumNew.SetActive(true);
            _uiManager.ShowCanvas(canvaseGroup);


            //canvaseGroup.alpha = 1;
            canvaseGroup.blocksRaycasts = true;

            //
            _textCantBuyPrice.text = "";
            _textCarPriceToBuy.text = "";
            _textUpgradePriceToBuy.text = "";

            //

            bg.gameObject.SetActive(false);

            _podium.SetActive(true);

            //silverCount = _playerManager.silver;
            goldCount = _playerManager.gold;
            jewelCount = _playerManager.jewels;

            playerCars = _playerManager.playerCars;

            if (playerCars.Count > 0)
            {
                isShow = true;
            }
            Debug.Log("car null " + (car == null) + "  curent car == null " + (_playerManager.currentCar == null));
            if (car == null && _playerManager.currentCar != null)
            {
                Debug.Log( "Car type " + _playerManager.currentCar.CarType + " lvl " + _playerManager.currentCar.level);
                car = DataModel.Instance.carsModels.Find(c => c.CarType == _playerManager.currentCar.CarType && c.level == _playerManager.currentCar.level);

                foreach (var item in DataModel.Instance.carsModels)
                {
                    Debug.Log("carsModels type " + item.CarType + " lvl " + item.level);

                }

                Debug.Log("car " + (car == null));

            }

            if (_playerManager.isTutorial != 1)
            {
                pcmCandidate = playerCars.Find(c => c.carType == car.CarType && c.carLevel == car.level);
            }
           
            if (car != null)
            {
                if (_playerManager.isTutorial != 1)
                {
                    pcmCandidate = playerCars.Find(c => c.carType == car.CarType && c.carLevel == car.level);
                }
                //else { Debug.Log(10); pcmCandidate = null; car.isOpened = true; }
                if (pcmCandidate != null)
                {
                    SetButtonsVisible(pcmCandidate);
                    ChangeCar(car, pcmCandidate.car_upgrade_level);
                    InitBtns();
                    _carIndicators = car.CalculateIndicators(pcmCandidate.GetPlayerUpgrades());
                }
                else
                {
                    if (!car.isOpened)
                    {
                        _carIndicators = car.CalculateIndicators();
                        cantBuyConteiner.SetActive(true);
                        carPrice = car.price.jewels;
                        Set_TextCarPriceJewelToBuy(carPrice);
                        if (carPrice > jewelCount)
                            buyCarBtnJewel.interactable = false;
                        else buyCarBtnJewel.interactable = true;
                        ChangeCar(car, 0);
                    }
                    else
                    {
                        carPrice = car.price.gold;
                        Set_TextCarPriceToBuy(carPrice);
                        carBuyConteiner.SetActive(true);
                        if (carPrice > goldCount)
                            buyCarBtn.interactable = false;
                        else buyCarBtn.interactable = true;
                        ChangeCar(car, 0);
                        _carIndicators = car.CalculateIndicators();
                    }
                }
            }
            else
            {
                if (_playerManager.currentCar != null)
                {
                    car = DataModel.Instance.carsModels.Find(c => c.level == _playerManager.currentCar.level && c.CarType == _playerManager.currentCar.CarType);
                    pcmCandidate = _playerManager.playerCars.Find(c => c.carLevel == car.level && c.carType == car.CarType);
                    ChangeCar(car, pcmCandidate.car_upgrade_level);
                    InitBtns();
                    SetButtonsVisible(pcmCandidate);
                    _carIndicators = car.CalculateIndicators(pcmCandidate.GetPlayerUpgrades());
                }
            }

            if (pcmCandidate != null)
            {
                //Debug.LogError(pcmCandidate.car_upgrade_level);
            }


            s = 1;
            sign = 1;

            UpdateUI();

            SetHexToButtons();

            _treeWindow.OnShowPanel += SwitchedCar;

            CheckCurrentCarInDB();
            // Debug.LogError(_pm.currentCar);
            if (pcmCandidate != null)
                SetButtonsVisible(pcmCandidate);

        }

        public void Hide()
        {
            banner.HideBaner();
            CheckCurrentCarInDB();

            //canvaseGroup.alpha = 0;
            _uiManager.HideCanvas(canvaseGroup);
            canvaseGroup.blocksRaycasts = false;

            //
            if (car != null)
            {
                // _playerManager.SetCurrentCur(car);

                _playerManager.ChooseCar(pcmCandidate);
            }




            ClearUpgradesPanel();
            cantBuyConteiner.SetActive(false);
            carBuyConteiner.SetActive(false);
            upgradesBuyConteiner.SetActive(false);

            if (currentCar != null)
            {
                Destroy(currentCar.gameObject);
            }



            //if(pcmCandidate!= null)
            //pcmCandidate.bought_colors.Clear();

            isChecked = false;
            isShow = false;
            pcmCandidate = null;

            foreach (var item in colorsButtons)
            {
                item.button.interactable = false;
                item.anim.enabled = false;
            }

            carsPrefabs.Clear();

            _podiumNew.SetActive(false);
            //_podium = GameObject.Find("Podium");
            _podium.SetActive(false);
        }

        // MY

        public void Set_TextCarPriceToBuy(int price)
        {
            _textCarPriceToBuy.text = string.Format(" {0}", price);
        }

        public void Set_TextCarPriceJewelToBuy(int price)
        {
            _textCarPriceJewelToBuy.text = price.ToString();
        }

        public void Set_TextCantBuyPrice(int price)
        {
            _textCantBuyPrice.text = string.Format(" {0}", price);
        }

        public void Set_TextUpgradePriceToBuy(int price)
        {
            _textUpgradePriceToBuy.text = string.Format(" {0}", price);
        }

        void SwitchedCar(CarModel car)
        {


            _treeWindow.OnShowPanel -= SwitchedCar;
            this.car = car;
            carsPrefabs.Add(Resources.Load("RaceCarsPrefabs/" + car.GetPrefabName(car.level)) as GameObject);
            pcmCandidate = playerCars.Find(c => c.carType == car.CarType && c.carLevel == car.level);

            if (pcmCandidate != null)
            {
                SetButtonsVisible(pcmCandidate);
            }
        }

        public void GoToTree()
        {
           

            if (currentCar != null)
            {
                rot = currentCar.transform.rotation;
                if(_playerManager.isTutorial != 1 && pcmCandidate!= null)
                currentCar.GetComponent<CarBase>().SetColorInGame(pcmCandidate.current_color);
            }

            upgradesBuyConteiner.SetActive(false);
            colorBuyConteiner.SetActive(false);
            

            _mainScreenStateManager.SwitchState(
                MainScreenStatesEnum.TREE);
        }

        private void DrawSliders(Dictionary<UIIndicatorsEnum, float> indicators)
        {

            foreach (var item in indicators.Keys)
            {
                switch (item)
                {

                    case UIIndicatorsEnum.MaxSpeed:
                        sliderSize = (372 / 100) * indicators[item];
                        maxSpeed.sizeDelta = new Vector2(sliderSize, 25);
                        seedLabel.text = String.Format("{0} MPH", (Mathf.Round(indicators[item] * 100f) / 100f));
                        break;

                    case UIIndicatorsEnum.Accelerator:
                        sliderSize = (372 / 70) * indicators[item];
                        accelerator.sizeDelta = new Vector2(sliderSize * 10, 25);
                        accelLabel.text = ((Mathf.Round(indicators[item] * 100f) / 100f)).ToString();
                        break;

                    case UIIndicatorsEnum.Grip:
                        sliderSize = (372 / 15) * indicators[item];
                        grip.sizeDelta = new Vector2(sliderSize * 10, 25);
                        gripLabel.text = ((Mathf.Round(indicators[item] * 100f) / 100f)).ToString();
                        break;

                    case UIIndicatorsEnum.Handling:
                        sliderSize = (372 / 60) * indicators[item];
                        handing.sizeDelta = new Vector2(sliderSize, 25);
                        handlingLabel.text = (Mathf.Round(indicators[item] * 100f) / 100f).ToString();
                        break;

                    case UIIndicatorsEnum.Nitro:
                        sliderSize = (372 / 50) * indicators[item];
                        nitro.sizeDelta = new Vector2(sliderSize, 25);
                        nitroLabel.text = (Mathf.Round(indicators[item] * 100f) / 100f).ToString();
                        break;
                }
            }
        }

        bool isShow = false;

        List<int> counts = new List<int>();

        void InitBtns()
        {
            if (isShow)
            {
                Dictionary<UpgradeType, List<UpgradeItem>> ups_dict = new Dictionary<UpgradeType, List<UpgradeItem>>();
                Dictionary<UpgradeType, List<UpgradeItem>> ups_result_dict = new Dictionary<UpgradeType, List<UpgradeItem>>();
                Dictionary<UpgradeType, Dictionary<int, int>> ups_levels_count = new Dictionary<UpgradeType, Dictionary<int, int>>();

                foreach (var up in selectedCarUpgrades)
                {
                    SetUpgradeItemState(up, pcmCandidate);

                    if (!ups_dict.ContainsKey(up.UpgradeType))
                    {
                        ups_dict.Add(up.UpgradeType, new List<UpgradeItem>());
                        ups_levels_count.Add(up.UpgradeType, new Dictionary<int, int>());
                    }
                    ups_dict[up.UpgradeType].Add(up);



                    if (!ups_levels_count[up.UpgradeType].ContainsKey(up.upgradeLevel))
                    {
                        ups_levels_count[up.UpgradeType].Add(up.upgradeLevel, 1);
                    }
                    else
                    {
                        ups_levels_count[up.UpgradeType][up.upgradeLevel]++;
                    }
                }

                foreach (var ups in ups_dict)
                {
                    ups.Value.Sort(delegate (UpgradeItem a, UpgradeItem b)
                    {
                        return (b.upgradeOrder).CompareTo(a.upgradeOrder);
                    });
                }

                float xOffset = 100 / (1920f / Screen.width);
                Vector2 posVec = Vector2.zero;
                int up_lvl = 3;
                float posDelta = 0f;

                Dictionary<UpgradeItemButton, float> btnsList = new Dictionary<UpgradeItemButton, float>();
                List<UpgradeItemButton> btns;

                ///Ebanutay Sortirovka
                UpgradeType[] stringArray = new UpgradeType[3];
                int[] intArray = new int[3];
                int ix = 0;
                foreach (var ups in ups_dict)
                {
                    intArray[ix] = ups.Value.Count;
                    stringArray.SetValue(ups.Key, ix);
                    ix++;
                }

                int max_count = Mathf.Max(intArray);
                int min_count = Mathf.Max(intArray);
                bool min_saved = false;
                bool mid_saved = false;

                foreach (var ups in ups_dict)
                {
                    if (ups.Value.Count == max_count)
                    {
                        stringArray.SetValue(ups.Key, 1);
                    }

                    else if (ups.Value.Count == min_count)
                    {
                        if (min_saved)
                        {
                            stringArray.SetValue(ups.Key, 0);
                        }
                        else
                        {
                            stringArray.SetValue(ups.Key, 2);
                            min_saved = true;
                        }
                    }

                    else
                    {
                        if (mid_saved)
                        {
                            stringArray.SetValue(ups.Key, 2);
                        }
                        else
                        {
                            stringArray.SetValue(ups.Key, 0);
                            mid_saved = true;
                        }
                    }
                }

                for (int i = 0; i < stringArray.Length; i++)
                {
                    ups_result_dict[stringArray[i]] = ups_dict[stringArray[i]];

                }

                //End of ebanutaya sortirovka
                foreach (var ups in ups_result_dict)
                {
                    btnsList.Clear();
                    for (int i = ups.Value.Count - 1; i >= 0; i--)
                    {
                        if (up_lvl != ups.Value[i].upgradeLevel)
                        {
                            up_lvl = ups.Value[i].upgradeLevel;
                            float butpos = ups_levels_count[ups.Key][up_lvl];
                            if ((int)butpos == 1)
                            {
                                switch (up_lvl)
                                {
                                    case 0: butpos = 1.25f; break;
                                    case 1: butpos = 1.5f; break;
                                    case 2: butpos = 1.5f; break;
                                }

                            }
                            else if ((int)butpos == 2)
                            {
                                switch (up_lvl)
                                {
                                    case 0: butpos -= 0.1f; break;
                                    case 1: butpos += 0.25f; break;
                                    case 2: butpos += 0.2f; break;
                                }
                            }
                            else if ((int)butpos == 3)
                            {
                                switch (up_lvl)
                                {
                                    // case 0: butpos; break;
                                    // case 1: butpos += 0.25f; break;
                                    //case 2: butpos += 0.2f; break;
                                }
                            }





                                posDelta = (GetPosYByUpLevel(up_lvl) / butpos);
                            posVec = new Vector2(xOffset, GetLevelOffset(up_lvl) + 5f / (1080f / Screen.height));
                        }
                        
                        posVec = new Vector2(xOffset, posVec.y + posDelta);
                        btnsList.Add(InstantiateUpgradeButton(posVec, ups.Value[i]), posVec.y);
                    }


                    btns = new List<UpgradeItemButton>(btnsList.Keys);

                    for (int i = btns.Count - 2; i >= 0; i--)
                    {
                        btns[i].SetArrowHeight((btnsList[btns[i + 1]] - btnsList[btns[i]]) - (100f / (1920f / Screen.height)));
                    }

                    xOffset += 180 / (1920f / Screen.width);
                }
            }
        }

        float YScreenCoef = (Screen.height / 1080f);

        float GetPosYByUpLevel(int up_lvl)
        {
            if (up_lvl == 2)
            {
                return 330f * YScreenCoef;
            }
            if (up_lvl == 1)
            {
                return 290f * YScreenCoef;
            }
            else// (up_lvl == 0)
            {
                return 200f * YScreenCoef;
            }
        }

        float GetLevelOffset(int up_lvl)
        {
            if (up_lvl == 2)
            {
                return (840f - 325f) * YScreenCoef;
            }
            if (up_lvl == 1)
            {
                return (840f - 325f - 265f) * YScreenCoef;
            }
            else// (up_lvl == 0)
            {
                return 0;
            }
        }

        [Space]
        public Transform UpgradesParent;

        UpgradeItemButton InstantiateUpgradeButton(Vector2 position, UpgradeItem item)
        {

            var go = Instantiate(upgradeItemPrefab, position, Quaternion.identity) as GameObject;
            go.transform.SetParent(UpgradesParent);
            go.transform.localScale = Vector3.one;
            var btn = go.GetComponent<UpgradeItemButton>();
            btn.Init(item);
            go.transform.name = UpgradesParent.name + "Upgrade" + item.upgradeOrder;
            carUpsList.Add(go);
            btn.OnClick += UpgradeClick;

            if (!item.isButton)
            {
                btn.OnClick -= UpgradeClick;
            }

            return btn;
        }

        #region UPGRADES

        private void SetUpgradeItemState(UpgradeItem item, PlayerCarModel pcm)
        {

            // isButtom --- false => green / true => orange
            // isInteractable --- false => can't click / true => can click



            //		Debug.Log( (UpgradeType) item.upgradeType + " | " +
            //		           "order: " + item.upgradeOrder + " | " +
            //		           "(Level: " + (item.upgradeLevel + 1) + ")");


            // ============================================================================================================
            if (IsCarHasInstalledUpgrade(item, pcm))
            {
                // УЖЕ КУПЛЕНО
                if (item.upgradeOrder <= GetCarInstalledUpgradeOrder(item, pcm))
                {
                    SetItemState_AlreadyBuyed(item);
                }

                // МОЖНО КУПИТЬ
                else if (item.upgradeOrder == GetCarInstalledUpgradeOrder(item, pcm) + 1)
                {
                    SetItemState_CanBuy(item);
                }

                // НЕЛЬЗЯ КУПИТЬ
                else
                {
                    SetItemState_CanNotBuy(item);
                }

            }
            // ============================================================================================================
            else
            {
                // МОЖНО КУПИТЬ
                if (item.upgradeOrder == 0)
                {
                    SetItemState_CanBuy(item);
                }

                // НЕЛЬЗЯ КУПИТЬ
                else
                {
                    SetItemState_CanNotBuy(item);
                }
            }
            // ============================================================================================================

            //Debug.Log("=================================================");

        }

        private bool IsCarHasInstalledUpgrade(UpgradeItem item, PlayerCarModel pcm)
        {
            bool isOK = pcm.installedUpgrades.ContainsKey(item.UpgradeType);

            return isOK;
        }

        private int GetCarInstalledUpgradeOrder(UpgradeItem item, PlayerCarModel pcm)
        {
            int order = pcm.installedUpgrades[item.UpgradeType];

            return order;
        }

        private void SetItemState_CanBuy(UpgradeItem item)
        {
            item.isButton = true;
            item.isInteractable = true;
        }

        private void SetItemState_CanNotBuy(UpgradeItem item)
        {
            item.isButton = true;
            item.isInteractable = false;
        }

        private void SetItemState_AlreadyBuyed(UpgradeItem item)
        {
            item.isButton = false;
            item.isInteractable = true;
        }

        #endregion

        void UpdateUI()
        {
            //_header.UpdateUI();
            carPriceToBuy.text = String.Format("Buy $ {0}", carPrice);
            carPriceToSell.text = String.Format("To sell $ {0}", carPrice);
            DrawSliders(_carIndicators);
        }

        private Button upButton;

        void UpgradeClick(UpgradeItem item, Button button)
        {
            goldCount = _playerManager.gold;
            currentCar.GetComponent<CarBase>().SetColorInGame(pcmCandidate.current_color);
            upgradesBuyConteiner.SetActive(true);
            colorBuyConteiner.SetActive(false);
            if (item.price.gold > goldCount)
                buyUpgradeBtn.interactable = false;
            else buyUpgradeBtn.interactable = true;

            // Debug.LogError(item.UpgradeType + " : " + item.upgradeOrder);
            if (!pcmCandidate.installedUpgrades.ContainsKey(item.UpgradeType))
            {
                if (item.price.gold > goldCount)
                {
                    this._item = null;
                    upButton = null;
                    return;
                }
                else
                {
                    if (item.upgradeOrder > 0)
                    {
                        this._item = null;
                        upButton = null;
                        return;
                    }
                    else
                    {
                        
                        this._item = item;
                        upButton = button;
                    }
                }
            }
            else
            {
                if (item.upgradeOrder > pcmCandidate.installedUpgrades[item.UpgradeType] + 1)
                {
                    this._item = null;
                    upButton = null;
                    return;

                }
                else
                {
                    upgradesBuyConteiner.SetActive(true);
                    colorBuyConteiner.SetActive(false);
                    this._item = item;
                    upButton = button;
                }
            }
        }

        void BuyColor()
        {
           ConfirmBuyColor(colorToBuy);
        }

        void ConfirmBuyColor(int id)
        {
            if (50 > _playerManager.gold)

            {
                return;
            }
            BlockButton.SetActive(true);
            loadingCircle.SetActive(true);
            if (car != null)
            {
                Core.Instance.GetService<NetworkManager>().BuyColor(car.CarType, car.level, id, (msg) =>
                {
                    loadingCircle.SetActive(false);
                    pcmCandidate.bought_colors.Add(id);
                    pcmCandidate.current_color = id;
                    SetButtonsVisible(pcmCandidate);
                    colorBuyConteiner.SetActive(false);
                    Core.Instance.GetService<NetworkManager>().SetColor(car.level, car.CarType, id, (m) =>
                    {
                        SelectColor(id);
                    });
                    //currentCar.GetComponent<CarBase>().colorMat.color = HexToColor(hex);
                    //carMaterial.color = HexToColor(hex);
                    //_playerManager.selectedColor = carMaterial.color;

                    SetButtonsVisible(pcmCandidate);
                    UpdateUI();
                    _header.GoldChanged(() => {
                        BlockButton.SetActive(false);
                    });


                }, (err)=> 
                {
                    if(err == "timeout")
                    {
                        BlockButton.SetActive(false);
                        loadingCircle.SetActive(false);
                        _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
                        _enternetWindow.ShowErrorEnternet();
                    }
                    else
                    {
                        BlockButton.SetActive(false);
                        loadingCircle.SetActive(false);
                        Debug.LogError("PlayGame err");
                    }
                });

            }
            else
            {
                Core.Instance.GetService<NetworkManager>().BuyColor(pcmCandidate.carType, pcmCandidate.carLevel, id, (msg) =>
                {
                    //BlockButton.SetActive(false);
                    loadingCircle.SetActive(false);
                    pcmCandidate.bought_colors.Add(id);
                    pcmCandidate.current_color = id;
                    SetButtonsVisible(pcmCandidate);
                    colorBuyConteiner.SetActive(false);
                    Core.Instance.GetService<NetworkManager>().SetColor(pcmCandidate.carLevel, pcmCandidate.carType, id, (m) =>
                    {
                        //Debug.LogError(id);
                        //Debug.LogError(m);
                        SelectColor(id);
                    });
                    // carMaterial.color = HexToColor(hex);
                    //_playerManager.selectedColor = carMaterial.color;

                    SetButtonsVisible(pcmCandidate);
                    UpdateUI();
                    _header.GoldChanged(() => {
                        Debug.Log("GoldChanged");
                        BlockButton.SetActive(false);
                    });
                    Debug.LogError("_playerManager.gold " + _playerManager.gold);

                }, (err) =>
                {
                    if (err == "timeout")
                    {
                        BlockButton.SetActive(false);
                        loadingCircle.SetActive(false);
                        _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
                        _enternetWindow.ShowErrorEnternet();
                    }
                    else
                    {
                        BlockButton.SetActive(false);
                        loadingCircle.SetActive(false);
                        Debug.LogError("PlayGame err");
                    }
                });
            }
            
            

        }

        void BuyUpgrade()
        {
                    ConfirmBuyUpgrade(_item);
                    if (_playerManager.isTutorial == 1 && ShowTutorialCarUpdate != null)
                    {
                        ShowTutorialCarUpdate();

                    }
        }

        public void ConfirmBuyUpgrade(UpgradeItem item)
        {
            if (item.price.gold > _playerManager.gold)
            {
                return;
            }
            BlockButton.SetActive(true);
            loadingCircle.SetActive(true);
            Core.Instance.GetService<NetworkManager>().BuyUpgrade(pcmCandidate.carType, pcmCandidate.carLevel, item.UpgradeType, item.upgradeOrder, (msg) =>
            {
                goldCount -= item.price.gold;
                _playerManager.gold = goldCount;
                SetButtonsVisible(pcmCandidate);
                pcmCandidate.installedUpgrades[item.UpgradeType] = item.upgradeOrder;
                TryGetUpgradedPrefab();
                InitBtns();
                upgradesBuyConteiner.SetActive(false);

                if (_playerManager.currentCar != null)
                {
                    _carIndicators = car.CalculateIndicators(pcmCandidate.GetPlayerUpgrades());
                }
                if (_treeWindow.carItems.Count == 0)
                {
                    _treeWindow.SwitchTypeAndInstantiateCars();
                }


                switch (car.CarType)
                {
                    case CarTypeEnum.Baggy:

                        if (item.upgradeOrder >= Core.Instance.GetService<NetworkManager>().MaxBuggy)
                        {
                            var c = DataModel.Instance.carsModels.Find(q => q.CarType == car.CarType && q.level == (car.level + 1));
                            if (c != null && !DataModel.Instance.openedCars.Contains(c))
                            {
                                car.isOpened = true;
                                DataModel.Instance.openedCars.Add(car);
                                var ci = _treeWindow.carItems.Find(q => q.level == car.level + 1 && q.type == car.CarType);



                                if (ci == null)
                                {
                                    ci = _treeWindow.carItems.Find(q => q.level == car.level && q.type == car.CarType);

                                    //Debug.Log("ci : " + ci.type + ci.level);
                                }
                                if (ci != null)
                                {
                                    ci.opened = true;
                                    c.isOpened = true;
                                    ci.UpdateStatus();
                                }
                            }
                        }
                        break;

                    case CarTypeEnum.Rally:
                        if (item.upgradeOrder >= Core.Instance.GetService<NetworkManager>().MaxRally)
                        {
                            var c = DataModel.Instance.carsModels.Find(q => q.CarType == car.CarType && q.level == (car.level + 1));
                            if (c != null && !DataModel.Instance.openedCars.Contains(c))
                            {
                                car.isOpened = true;
                                DataModel.Instance.openedCars.Add(car);
                                var ci = _treeWindow.carItems.Find(q => q.level == car.level + 1 && q.type == car.CarType);
                                if (ci == null)
                                {
                                    ci = _treeWindow.carItems.Find(q => q.level == car.level && q.type == car.CarType);
                                    //Debug.Log("ci : " + ci.type + ci.level);
                                }
                                if (ci != null)
                                {
                                    ci.opened = true;
                                    c.isOpened = true;
                                    ci.UpdateStatus();
                                }
                            }

                        }
                        break;

                    case CarTypeEnum.Jeep:
                        if (item.upgradeOrder >= Core.Instance.GetService<NetworkManager>().MaxJeep)
                        {
                            var c = DataModel.Instance.carsModels.Find(q => q.CarType == car.CarType && q.level == (car.level + 1));
                            if (c != null && !DataModel.Instance.openedCars.Contains(c))
                            {
                                car.isOpened = true;
                                DataModel.Instance.openedCars.Add(car);
                                var ci = _treeWindow.carItems.Find(q => q.level == car.level + 1 && q.type == car.CarType);
                                if (ci == null)
                                {
                                    ci = _treeWindow.carItems.Find(q => q.level == car.level && q.type == car.CarType);
                                }
                                if (ci != null)
                                {
                                    ci.opened = true;
                                    c.isOpened = true;
                                    ci.UpdateStatus();
                                }
                            }
                        }
                        break;
                }

                loadingCircle.SetActive(false);
                 UpdateUI();
                _header.GoldChanged(()=> {BlockButton.SetActive(false); });
                currentCar.GetComponent<CarBase>().ApplyCarUpgradeValues(pcmCandidate.GetPlayerUpgrades());
            } ,
            (err)=> 
            {
                if (err == "timeout")
                {
                    BlockButton.SetActive(false);
                    loadingCircle.SetActive(false);
                    _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
                    _enternetWindow.ShowErrorEnternet();
                }
                else
                    Debug.LogError("Undefined Error");
            });
        }

        void TryGetUpgradedPrefab()
        {
            if (pcmCandidate != null)
            {
                if (CheckForCarPrefabUpgrade(pcmCandidate))
                {
                    int oldLvl = pcmCandidate.car_upgrade_level;
                    var _m = _playerManager.stockCarsList.Find(c => c.level == pcmCandidate.carLevel && c.CarType == pcmCandidate.carType);
                    if (_m.upgradeLevels.ContainsKey(oldLvl++) && _m.upgradeLevels.Count > oldLvl)
                    {
                        pcmCandidate.car_upgrade_level++;
                        _m = DataModel.Instance.GetCar(pcmCandidate.carType, pcmCandidate.carLevel);


                        GameObject.DestroyObject(currentCar);
                        IsUpgrade = true;
                        ChangeCar(_m, pcmCandidate.car_upgrade_level);
                        //Debug.Log("+ CAR UPGRADE LEVEL " + pcmCandidate);
                    }
                }
            }
            else
            {
                pcmCandidate = _playerManager.playerCars.Find(c => c.carLevel == _playerManager.currentCar.level && c.carType == _playerManager.currentCar.CarType);
                if (CheckForCarPrefabUpgrade(pcmCandidate))
                {
                    int oldLvl = pcmCandidate.car_upgrade_level;
                    var _m = _playerManager.currentCar;
                    if (_m.upgradeLevels.ContainsKey(oldLvl++) && _m.upgradeLevels.Count > oldLvl)
                    {
                        pcmCandidate.car_upgrade_level++;
                        _m = DataModel.Instance.GetCar(pcmCandidate.carType, pcmCandidate.carLevel);
                        IsUpgrade = true;
                        ChangeCar(_m, pcmCandidate.car_upgrade_level);
                        //Debug.Log("+ CAR UPGRADE LEVEL " + pcmCandidate);
                    }
                }
            }

        }

        void ClearUpgradesPanel()
        {
            foreach (var item in carUpsList)
            {
                item.GetComponent<UpgradeItemButton>().OnClick -= UpgradeClick;
                Destroy(item);
            }

            carUpsList.Clear();
        }

        bool CheckForCarPrefabUpgrade(PlayerCarModel pcm)
        {
            var newUpLevel = Core.Instance.GetService<NetworkManager>().CheckForCarPrefabUpgrade(pcm, selectedCarUpgrades);

            if (newUpLevel > pcm.car_upgrade_level)
            {
                return true;
            }

            return false;
        }

        void Update()
        {
           
           
            if (canvaseGroup.alpha > 0)
            {
                UserInput();
                if (_podiumNew.activeSelf && currentCar != null)
                {
                    _podiumNew.transform.rotation = currentCar.transform.rotation;
                    rot = currentCar.transform.rotation;
                    _podiumNew.transform.localPosition = new Vector3(
                        currentCar.transform.localPosition.x,
                        _podiumNew.transform.localPosition.y,
                        currentCar.transform.localPosition.z
                        );
                }
            }
            else
            {
                return;
            }
        }

        List<UpgradeItem> selectedCarUpgrades = new List<UpgradeItem>();
        public Quaternion rot;
        public void ChangeCar(CarModel car, int up_lvl)
        {
            _playerManager = Core.Instance.GetService<PlayerManager>();

            var go = Resources.Load("RaceCarsPrefabs/" + car.GetPrefabName(up_lvl)) as GameObject;

            if (go != null)
            {
                InstantiateCar(go, car);
                //_carIndicators = car.CalculateIndicators(pcmCandidate.GetPlayerUpgrades());
            }
            else
            {
                go = Resources.Load("RaceCarsPrefabs/" + car.GetPrefabName(up_lvl - 1)) as GameObject;
                InstantiateCar(go, car);
                _carIndicators = car.CalculateIndicators(pcmCandidate.GetPlayerUpgrades());
            }
        }

        void RemoveCar()
        {

        }

        void InstantiateCar(GameObject go, CarModel model)
        {
            float height = 1.5f;
            if (_playerManager.selectedCar != null)
            {
                var level = _playerManager.selectedCar.carLevel;
                var type = _playerManager.selectedCar.type;


                if (level == model.level && model.type == type && IsUpgrade == false)
                {
                    height = -0.1374124f;
                    //Debug.Log("Одинаковые");

                }
            }
            IsUpgrade = false;
            //Debug.Log("rot " + rot);
            currentCar = GameObject.Instantiate(go,
                new Vector3(
                    carParent.transform.position.x,
                    carParent.transform.position.y + height,
                    carParent.transform.position.z
                    )
                , rot) as GameObject;

            currentCar.transform.SetParent(carParent);

            // currentCar.AddComponent<SphereCollider>();
            //currentCar.GetComponent<SphereCollider>().radius = 5f;

            //currentCar.GetComponent<Rigidbody>().useGravity = false;
            currentCar.GetComponent<Rigidbody>().useGravity = true;
            currentCar.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
            currentCar.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
            currentCar.GetComponent<CarBase>().controller.enabled = true;

            s = 1;


            selectedCarUpgrades = DataModel.Instance.GetUpgradesByCar(model.CarType, model.level);

            foreach (var value in Enum.GetValues(typeof(UpgradeType)))
            {
                if (selectedCarUpgrades.Find(up => up.UpgradeType == (UpgradeType)value) != null)
                {
                    typesList.Add(((UpgradeType)value));
                }
            }

            carNameToBuy.text = model.name;
            carNameToSell.text = model.name;

            if (pcmCandidate != null)
            {
                currentCar.GetComponent<CarBase>().SetColorInGame(pcmCandidate.current_color);
                currentCar.GetComponent<CarBase>().ApplyCarUpgradeValues(pcmCandidate.GetPlayerUpgrades());
            }

            UpdateUI();
        }

        Dictionary<UIIndicatorsEnum, float> _carIndicators = new Dictionary<UIIndicatorsEnum, float>();


        private void BuyCar()
        {

            if (car.price.gold > _playerManager.gold)
            {
                if (car.isOpened || car.price.jewels > _playerManager.jewels)
                    return;
            }
            BlockButton.SetActive(true);
            loadingCircle.SetActive(true);

            Core.Instance.GetService<NetworkManager>().BuyCar(car.CarType, car.level, (!car.isOpened).ToString(), (msg) =>
            {
                var _pcm = new PlayerCarModel();
                _pcm.carType = car.CarType;
                _pcm.carLevel = car.level;

                _pcm.car_upgrade_level = 0;
                _pcm.installedUpgrades = new Dictionary<UpgradeType, int>();

                _playerManager.AddCarToPlayerCarsList(_pcm);

                pcmCandidate = _pcm;
                _carIndicators = car.CalculateIndicators(pcmCandidate.GetPlayerUpgrades());
                goldCount = _playerManager.gold;
                jewelCount = _playerManager.jewels;

                if (_playerManager.isTutorial == 1 && ShowTutorialOnBuyCar != null)
                {
                    ShowTutorialOnBuyCar();
                }
                isShow = true;
                
                InitBtns();
                var ca = _treeWindow.carItems.Find(c => c.type == car.CarType && c.level == car.level);
                ca.carStatus.sprite = ca.boughtSprite;
                carBuyConteiner.SetActive(false);
                loadingCircle.SetActive(false);


                if (car != null)
                {
                    var p = _playerManager.playerCars.Find(q => q.carLevel == car.level && q.carType == car.CarType);
                    if (p != null)
                    {
                        Core.Instance.GetService<NetworkManager>().SET_CURRENT_CAR(car.CarType, car.level, (m) =>
                        {
                                Debug.LogError(m);
                            });
                    }
                }

                switch (car.CarType)
                {
                    case CarTypeEnum.Baggy:
                        Core.Instance.GetService<NetworkManager>().MaxBuggy =
                            Core.Instance.GetService<NetworkManager>().GetOpenedCar(car.CarType, car.level);
                        break;

                    case CarTypeEnum.Rally:
                        Core.Instance.GetService<NetworkManager>().MaxRally =
                            Core.Instance.GetService<NetworkManager>().GetOpenedCar(car.CarType, car.level);
                        break;

                    case CarTypeEnum.Jeep:
                        Core.Instance.GetService<NetworkManager>().MaxJeep =
                            Core.Instance.GetService<NetworkManager>().GetOpenedCar(car.CarType, car.level);
                        break;

                    default:
                            #region DEBUG
#if UNITY_EDITOR
                            Debug.Log("[ERROR] unknown car type = " + car.CarType + " !");
#endif
                            #endregion
                            break;
                }
                SetButtonsVisible(pcmCandidate);
                UpdateUI();
                _header.GoldChanged(()=> { BlockButton.SetActive(false); });
                _header.JewelChanged(()=> { BlockButton.SetActive(false); });
                cantBuyConteiner.SetActive(false);
                //Debug.Log("car  " + car.isOpened + " !");

            }, (err)=> 
            {
                if (err == "timeout")
                {
                    BlockButton.SetActive(false);
                    loadingCircle.SetActive(false);
                    _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
                    _enternetWindow.ShowErrorEnternet();
                }
                else
                {
                    Debug.LogError("BuyCar err");
                }
            });
        }

     
        public void Select()
        {
            //Debug.Log("Select");
            Hide();

            if (_gameManager.gameData != null && _gameManager.gameData.GameType == GameTypeEnum.MultyReplay)
            {
                _uiManager.ShowWindow(UIWindowEnum.FINISH_ASYNC, true);
                _uiManager.ShowWindow(UIWindowEnum.GAMEWINDOW_ASYNC, true);
            }
            else
            {
                Core.Instance.GetService<StatesManager>().SwitchState(StatesEnum.MAIN_MULTIPLAYER_WINDOW);
            }
        }

        int colorToBuy;
        void SelectColor(int id)
        {
            if (pcmCandidate.bought_colors.Contains(id))
            {
                //Change Color and return
                pcmCandidate.current_color = id;
                //TODO SetColor in base
                Core.Instance.GetService<NetworkManager>().SetColor(pcmCandidate.carLevel, pcmCandidate.carType, id, (msg) =>
                {
                    upgradesBuyConteiner.SetActive(false);
                    colorBuyConteiner.SetActive(false);
                });

                currentCar.GetComponent<CarBase>().SetColorInGame(id);
                return;
            }
            else
            {
                currentCar.GetComponent<CarBase>().SetColorInGame(id);
                colorBuyConteiner.SetActive(true);
                upgradesBuyConteiner.SetActive(false);
                colorToBuy = id;
            }
        }

        void SetButtonsVisible(PlayerCarModel pcm)
        {
            //Debug.Log("pcm.bought_colors.Count " + pcm.bought_colors.Count);
            if (pcm.bought_colors.Count > 0)
            {
                foreach (var item in colorsButtons)
                {
                    if (pcm.bought_colors.Contains(item.ID))
                    {
                        item.button.interactable = true;
                        item.anim.enabled = false;
                    }
                    else
                    {
                        //Debug.Log("goldCount " + goldCount);
                        if (item.price <= goldCount)
                        {
                            item.button.interactable = true;
                            item.anim.enabled = true;
                        }
                        else
                        {
                            item.button.interactable = false;
                            item.anim.enabled = false;
                        }

                    }
                }
            }
            else
            {
                foreach (var item in colorsButtons)
                {
                    if (item.price <= goldCount)
                    {
                        item.button.interactable = true;
                        item.anim.enabled = true;
                    }
                    else
                    {
                        item.button.interactable = false;
                        item.anim.enabled = false;
                    }
                }
            }

        }

        public Color32 HexToColor(string hex)
        {
            hex = hex.Replace("0x", "");
            hex = hex.Replace("#", "");

            byte a = 255;
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            }

            return new Color32(r, g, b, a);
        }

        void SetHexToButtons()
        {
            for (int i = 0; i < colorsButtons.Count; i++)
            {
                colorsButtons[i].Init(_playerManager.allColors[i].ID, _playerManager.allColors[i].price, _playerManager.allColors[i].hex);
                colorsButtons[i].OnPress = SelectColor;
            }
        }

        public void UserInput()
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

        public void CheckCurrentCarInDB()
        {
            if (_playerManager.currentCar != null && pcmCandidate != null)
            {

                if ((_playerManager.currentCar.CarType != pcmCandidate.carType) ||
                    (_playerManager.currentCar.level != pcmCandidate.carLevel)
                )
                {
                    var tempPcm = pcmCandidate;
                    Core.Instance.GetService<NetworkManager>().SET_CURRENT_CAR(pcmCandidate.carType, pcmCandidate.carLevel, (msg) =>
                    {
                        _playerManager.ChooseCar(tempPcm);

                    });
                }
            }
        }

        public void GoToMultyplayerState()
        {
            if (car != null)
            {
                var p = _playerManager.playerCars.Find(q => q.carLevel == car.level && q.carType == car.CarType);
                if (p != null)
                {
                    Core.Instance.GetService<NetworkManager>().SET_CURRENT_CAR(car.CarType, car.level, (msg) =>
                    {
                        //Debug.LogError(msg);
                    });
                }
            }

            Hide();
            var multy = Core.Instance.GetService<UIManager>().GetWindow(UIWindowEnum.MAIN_MULTIPLAYER);
            multy.Show();

        }



    }
}