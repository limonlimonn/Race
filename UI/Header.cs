using UnityEngine;
using UnityEngine.UI;
using System;
using HCR.Enums;
using HCR.Interfaces;
using UnityEngine.Assertions;
using DG.Tweening;


namespace HCR.GlobalWindow.MainMenu
{
	/// <summary>
	/// Класс - окно "Хедер" в Главном Меню (самый верх)
	/// тут отображается: имя Игрока, уровень, Х2/Х3/Х4, серебро, золото
	/// </summary>

	public class Header : MonoBehaviour, IUIWindow
	{
		// FIELDS

		#region VARIABLES

		//
		public CanvasGroup canvaseGroup;

		public Text X2Timer;
		public Text X3Timer;
		public Text X4Timer;

		public Sprite inActiveBonusImage2;
		public Sprite inActiveBonusImage3;
		public Sprite inActiveBonusImage4;
		public Sprite activeBonusImage;
		public Image bonusImage;				// не задан в Инспекторе !

		public Animator bonus2Animator;
		public Animator bonus3Animator;
		public Animator bonus4Animator;

		public Image slider2X;
		public Image slider3X;
		public Image slider4X;

		public Text playerXP;

		public Text jewels;
		public Text gold;
		public Text playerLevel;
		public Text playerName;

		public Image XPSlider;
        public Image AccountType;
        public Sprite AccountType0;
        public Sprite AccountType1;

        //
        private int _jewelsCashed;
		private int _goldCashed;

		#endregion

		// dependences
		private PlayerManager _playerManager;
		private UIAnimatorManager _uiAnimatorManager;
        private StatisticPanel _statisticPanel;
		private UIManager _uiManager;
		private MainScreenStateManager _mainScreenStateManager;
        private ChooseCarWindow _chooseCarWindow;
        private NotificationWindow _notificationWindow;


        // I_UI_WINDOW

        public void Init()
		{
			_playerManager = Core.Instance.GetService<PlayerManager>();
			Assert.AreNotEqual(null, _playerManager);

			_uiAnimatorManager = Core.Instance.GetService<UIAnimatorManager>();
			Assert.AreNotEqual(null, _uiAnimatorManager);

			_uiManager = Core.Instance.GetService<UIManager>();
			Assert.AreNotEqual(null, _uiManager);

			_mainScreenStateManager = _uiManager.Get_MainScreenStateManager();
			Assert.AreNotEqual(null, _mainScreenStateManager);

            //
            _notificationWindow = _uiManager.NotificationWindow;

            _statisticPanel = _uiManager.GetWindow(UIWindowEnum.STATISTIC) as StatisticPanel;
            Assert.AreNotEqual(null, _statisticPanel);

            _chooseCarWindow = _uiManager.GetWindow(UIWindowEnum.CHOOSE_CAR) as ChooseCarWindow;
            AssertVariables();
            ShowAccType();

        }

		public void Show()
		{
			canvaseGroup.alpha = 1;
			canvaseGroup.blocksRaycasts = true;
            
            
        }

		public void Hide()
		{
			canvaseGroup.alpha = 0;
			canvaseGroup.blocksRaycasts = false;
		}



		// INTERFACES

		// ==================================================
		/// <summary>
        /// *
        /// </summary>
       
        private void ShowAccType()
        {
            switch (_playerManager.accountType)
            {
                case 0:
                    AccountType.sprite = AccountType0; break;
                case 1:
                    AccountType.sprite = AccountType1; break;
                

                default:
                    break;
            }
            
        }

        public void UpdateAccoutSprite(Action OnComplete)
        {
            
            AccountType.transform.DOScale(1.3f, 0.5f).OnComplete(() => {
                AccountType.sprite = AccountType1;
                AccountType.transform.DOScale(1f, 0.5f).OnComplete(() => { AccountType.transform.localScale.Set(1, 1, 0); Debug.Log("Complete1"); OnComplete(); });
            });
        }
        
		public void UpdateUI()
		{
            ShowAccType();

            //ShowLevelProgress();
            ShowTextFieldsData();

		}

		public void GoldChanged(Action OnComplete = null)
		{
			CheckGoldChanged(OnComplete);
		}

		public void JewelChanged(Action OnComplete = null)
		{
			// TODO --- gold update text

			CheckJewelChanged(OnComplete);
		}

		// ==================================================
		//

		// used on button !
		public void ChooseCarClick()
		{
           if (_playerManager.selectedCar != null)
                _chooseCarWindow.car = DataModel.Instance.carsModels.Find(c => c.CarType == _playerManager.selectedCar.carType && c.level == _playerManager.selectedCar.carLevel); 
            
                _mainScreenStateManager.SwitchState(MainScreenStatesEnum.CHOOSE_CAR);
		}

		// used on button !
		public void GoToMultyplayerState()
		{
            //        if (chooseWindow.car != null)
            //        {
            //            var p = _playerManager.playerCars.Find(q => q.carLevel == chooseWindow.car.level && q.carType == chooseWindow.car.CarType);
            //            if (p != null)
            //            {
            //                Core.Instance.GetService<NetworkManager>().SET_CURRENT_CAR(chooseWindow.car.CarType,
            //	                chooseWindow.car.level,
            //	                (msg) =>
            //					{
            //						//Debug.LogError(msg);
            //					});
            //            }
            //        }
            
			_mainScreenStateManager.SwitchState(MainScreenStatesEnum.MAIN_MULTIPLAYER);
		}

		// used on button !
		public void OnClickButton_Settings()
		{
			_mainScreenStateManager.SwitchState(MainScreenStatesEnum.SETTINGS);
		}

        public void OnClickButton_Shop()

        {
            //_notificationWindow.ShowGameAnswer("Работай");
           _mainScreenStateManager.SwitchState(MainScreenStatesEnum.SCORE);
        }



		// METHODS

		private void AssertVariables()
		{
			Assert.AreNotEqual(null, canvaseGroup);

			Assert.AreNotEqual(null, X2Timer);
			Assert.AreNotEqual(null, X3Timer);
			Assert.AreNotEqual(null, X4Timer);

			Assert.AreNotEqual(null, inActiveBonusImage2);
			Assert.AreNotEqual(null, inActiveBonusImage3);
			Assert.AreNotEqual(null, inActiveBonusImage4);
			Assert.AreNotEqual(null, activeBonusImage);
			//Assert.AreNotEqual(null, bonusImage);

			Assert.AreNotEqual(null, bonus2Animator);
			Assert.AreNotEqual(null, bonus3Animator);
			Assert.AreNotEqual(null, bonus4Animator);

			Assert.AreNotEqual(null, slider2X);
			Assert.AreNotEqual(null, slider3X);
			Assert.AreNotEqual(null, slider4X);

			Assert.AreNotEqual(null, playerXP);

			Assert.AreNotEqual(null, jewels);
			Assert.AreNotEqual(null, gold);
			Assert.AreNotEqual(null, playerLevel);
			Assert.AreNotEqual(null, playerName);

			Assert.AreNotEqual(null, XPSlider);
		}

		private void ShowTextFieldsData()
		{
			playerName.text = _playerManager.PlayerDisplayName;
			playerLevel.text = String.Format("Level {0}", _playerManager.level);
			playerXP.text = String.Format("{0}", _playerManager.xp);

            //
            jewels.text = _playerManager.jewels.ToString();
			gold.text = _playerManager.gold.ToString();

            //
            _jewelsCashed = _playerManager.jewels;
			_goldCashed = _playerManager.gold;
		}
        /*
		private void ShowLevelProgress()
		{
			float xpPlayer = GetXpNeeded();

            //Debug.Log("xpPlayer" + xpPlayer +" "+ _playerManager.xp);
            XPSlider.fillAmount = 1 + (xpPlayer / 1000);
		}
        */
        /*
		private float GetXpNeeded()
		{
			for (int i = 0; i < _playerManager.allLevelsXP.Count; i++)
			{
				if (_playerManager.xp < _playerManager.allLevelsXP[i].maxValue)
				{
                    _playerManager.level = _playerManager.allLevelsXP[i].ID;
                    
                    return (float)(_playerManager.xp - _playerManager.allLevelsXP[i].maxValue);

                }
			}

			#region DEBUG
#if UNITY_EDITOR
			Debug.Log("[ERROR] can't get xp needed! Level = " + _playerManager.level);
#endif
			#endregion

			return _playerManager.xp;
		}
        */
		//
		private void CheckGoldChanged(Action OnComplete = null)
		{
			if (_goldCashed == _playerManager.gold) {
				return; }

			_uiAnimatorManager.ShowTextFieldUpdateAniamtion(
				gold, _goldCashed, _playerManager.gold, OnComplete);

			_goldCashed = _playerManager.gold;
		}

        private void CheckJewelChanged(Action OnComplete = null)
        {
            if (_jewelsCashed == _playerManager.jewels)
            {
                return;
            }

            _uiAnimatorManager.ShowTextFieldUpdateAniamtion(
                jewels, _jewelsCashed, _playerManager.jewels, OnComplete);

            _jewelsCashed = _playerManager.jewels;
        }
        public void OnClickXP()
        {
            _mainScreenStateManager.SwitchState(MainScreenStatesEnum.STATISTIC);
        }

        private void CheckJewelsChanged()
        {
            if (_jewelsCashed == _playerManager.jewels)
            {
                return;
            }

            _uiAnimatorManager.ShowTextFieldUpdateAniamtion(
                jewels, _jewelsCashed, _playerManager.jewels);

            _jewelsCashed = _playerManager.jewels;
        }



    }
}