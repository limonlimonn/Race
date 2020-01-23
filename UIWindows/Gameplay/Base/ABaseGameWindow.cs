using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;
using UnityEngine.UI;

using HCR.Enums;
using HCR.GlobalWindow.MainMenu;
using HCR.Interfaces;
using HCR.Gameplay.Tutorial;
using DG.Tweening;
using HCR.Event;

namespace HCR.Gameplay
{
	/// <summary>
	/// Абстракный базовый класс - главных окон игр
	/// "Игра: Одиночная игра", "Игра: Асинхронный мулльтиплеер"
	/// </summary>

	public abstract class ABaseGameWindow : MonoBehaviour, IUIWindow
	{
		// FIELDS

		#region VARIABLES
		// ---------------------------------------------------
		//
		[Header("CANVAS_GROUP")]
		[SerializeField] protected CanvasGroup _canvasGroup;

		[Header("UI: OLD AND NEW")]
		[SerializeField] protected ABaseUiPanel _uiPanelOld;
		[SerializeField] protected ABaseUiPanel _uiPanelNew;

		// ---------------------------------------------------
		// fields in Inspector (для подмены UI)
		[HideInInspector] public List<CanvasGroup> nitroImages;
		[HideInInspector] public Text timerText;

		[HideInInspector] public List<Image> triesSprite = new List<Image>();

		[HideInInspector] public Sprite lostTrie;
		[HideInInspector] public Sprite trie;

		[HideInInspector] public Text meterCountLabel;
        [HideInInspector] public Text enemyCountMeter;
        [HideInInspector] public Text currentSpeedLabel;

        [SerializeField] public Image enemyCountImage;

        [SerializeField] public Sprite leaderSprite;
        [SerializeField] public Sprite looserSprite;
        [SerializeField] public Sprite crashedSprite;
        [SerializeField]
        public Sprite forwardOnSprite;
        [SerializeField]
        public Sprite forwardOffSprite;

        [HideInInspector] public Button pauseButton;
        [HideInInspector] public Image forwardButton;

		[HideInInspector] public Transform cursore;

        [HideInInspector] public Slider nitro;
		[HideInInspector] public CanvasGroup nitroButtonImage;
		[HideInInspector] public Image slider;					// nitro bar

		[HideInInspector] public List<CanvasGroup> panelsList = new List<CanvasGroup>();
        
		[HideInInspector] public GameObject podium;

		//
		[HideInInspector] public Text _textRaceTimer;
		[HideInInspector] public Text _textJewels;
		[HideInInspector] public Text _textGold;
        [HideInInspector] public GameObject _ImageJewels;
        [HideInInspector]
        public GameObject _ImageGold;

        // ---------------------------------------------------
        // public misc
        [HideInInspector] public bool isNitroPressed = false;
		[HideInInspector] public int triesCount;

		// ---------------------------------------------------
		//
		protected float _sliderSize;
		protected int _speed;
        protected float _cursorSpeed;
        // controls
        protected bool updateUI = false;

		protected bool isFroward = false;
        private bool isSmoke = true;
        protected bool isBack = false;

		protected bool isUp = false;
		protected bool isDown = false;

		#endregion

		// dependences
		public CarBase _playerCar;
		protected SafePlayerPrefs _safePlayerPrefs;
		protected PlayerManager _playerManager;
		protected GameManager _gameManager;
		protected UIManager _uiManager;
		protected Header _header;
        private AudioService _audioService;
        protected CarConstructor _carConstructor;

        private float TimeGoldLost = 1;
        string audioPath = "event:/UI Sounds/button_click";



        // UNITY

        protected void FixedUpdate()
		{
			if (!updateUI) {
				return; }

			if (_playerCar != null)
			{
				nitro.maxValue = _playerCar.nitro.maxNitro;
				nitro.value = _playerCar.nitro.nitroCurrent;
				nitroButtonImage.alpha = Mathf.Clamp( (nitro.value / 10f), 0.3f, 1f);
				SetVisibleNitro(nitro.value, nitro.maxValue);

				meterCountLabel.text = String.Format("{0}m", _playerCar.currentMeters);

				_speed = (int)(Mathf.Clamp(
					(_playerCar.controller.speed * 2.237f), 0,
					(int)_playerCar.controller.speed * 2.237f));

				currentSpeedLabel.text = _speed.ToString();
                if (isFroward && _speed < 15)
                {
                    
                    if (isSmoke)
                    {
                        for (int i = 0; i < _playerCar.smokeParticles.Count; i++)
                        {
                            _playerCar.smokeParticles[i].emissionRate = 400;

                        }
                        isSmoke = false;
                    }

                }
                else if (isFroward) {
                    if (!isSmoke)
                    {
                        for (int i = 0; i < _playerCar.smokeParticles.Count; i++)
                        {
                            _playerCar.smokeParticles[i].emissionRate = 200;

                        }
                        isSmoke = true;
                    }
                    

                }


            }
            
		}



		// I_UI_WINDOW

		public virtual void Init()
		{
			_safePlayerPrefs = Core.Instance.GetService<SafePlayerPrefs>();
			Assert.AreNotEqual(null, _safePlayerPrefs);

			_playerManager = Core.Instance.GetService<PlayerManager>();
			Assert.AreNotEqual(null, _playerManager);

			_gameManager = Core.Instance.GetService<GameManager>();
			Assert.AreNotEqual(null, _gameManager);

			//
			_uiManager = Core.Instance.GetService<UIManager>();
			Assert.AreNotEqual(null, _uiManager);

			_header = _uiManager.GetWindow(UIWindowEnum.HEADER) as Header;
			Assert.AreNotEqual(null, _header);

            _audioService = Core.Instance.GetService<AudioService>();
            Assert.AreNotEqual(null, _audioService);

        }

		public abstract void Show();

		public abstract void Hide();



		// INTERFACES

		public void SetValue_PlayerCar()
		{
			// pre-condition
			Assert.AreNotEqual(null, _playerManager.PlayerCar);

			_playerCar = _playerManager.PlayerCar;
            EventManager._init.Game.CarEvent.Player.GetEvent.Crash += CheckExplGold;
            for (int i = 0; i < _playerCar.smokeParticles.Count; i++)
            {
                _playerCar.smokeParticles[i].emissionRate = 40;
            }
            Assert.AreNotEqual(null, _playerCar);
            //


        }

        private void CheckExplGold()
        { if (_safePlayerPrefs.GetInt_EarnedGold() > 0) ExplGold(); }


        private void ExplGold()
        {
            
             _textGold.transform.DOPunchScale(new Vector3(0.08f, 0.08f), 1/TimeGoldLost, 1, 1).OnComplete(() => {
                 if (_textGold.text!= "0") {
                     _audioService.RM_PlayOneShot(audioPath);
                     _textGold.text = (int.Parse(_textGold.text) - 1).ToString();
                     ExplGold();
                 }
                 else
                 {
                     _textGold.transform.DOPunchScale(new Vector3(0.6f, 0.6f), 1.3f, 1, 1);
                     if (!_playerCar.IsFinish)
                     {
                         _safePlayerPrefs.SaveEarnedGold(0);
                         _safePlayerPrefs.SavePlayerGold(0);
                     }

                 }



             });

        }


        public void InitUIType()
		{
			CheckSavedUISettings();
		}

		public void UpdateTextGold()
        {
             
            DOTween.Kill("gold",true);
            _textGold.transform.DOPunchScale(new Vector3(0.5f, 0.5f), 0.4f, 5, 1).SetId("gold");
            _textGold.text = _safePlayerPrefs.GetInt_EarnedGold().ToString();
            TimeGoldLost++;

        }
        
		public void UpdateTextJewels()
		{
            //DOTween.KillAll(true);
            _textJewels.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.4f, 5, 1);

            _textJewels.text = _safePlayerPrefs.GetInt_EarnedJewels().ToString();
        }

        public void ActiveGold()
        {
            _ImageGold.SetActive(true);
        }

        public void DeActiveGold()
        {
            _ImageGold.SetActive(false);
        }

        public void ActiveJewels()
        {
            _ImageJewels.SetActive(true);
        }

        public void DeActiveJewels()
        {
            _ImageJewels.SetActive(false);
        }
		

		#region BLOCK/UNBLOCK PAUSE BUTTON

		

		#endregion

        public void ShowEnemyMeters()
        {
            enemyCountMeter.gameObject.SetActive(true);
            enemyCountImage.sprite = leaderSprite;
            enemyCountMeter.text = "0m";
        }

        public void HideEnemyMeters()
        {
            enemyCountImage.sprite = leaderSprite;
            enemyCountMeter.text = "0m";
            enemyCountMeter.gameObject.SetActive(false);
        }

        public void LeaderEnemyMeter(string metersToEnemy)
        {
            enemyCountImage.sprite = leaderSprite;
            enemyCountMeter.text = "+" + metersToEnemy + "m";
        }

        public void LooseEnemyMeter(string metersToEnemy)
        {
            enemyCountImage.sprite = looserSprite;
            enemyCountMeter.text = metersToEnemy + "m";
        }

        // METHODS

        protected void WorkOnShow()
		{
			//Debug.Log("__ WORK ON SHOW");

			InitUIType();

			_textJewels.text = "0";
			_textGold.text = "0";

			//BlockPauseButton();

			// =============================================================
			//
			podium = GameObject.Find("Podium");
			if (podium != null) {
				podium.SetActive(false); }

			_header.Hide();

			cursore.rotation = Quaternion.Euler(0, 180, 0);

			isNitroPressed = false;
			updateUI = true;
			//meterCountLabel.text = String.Format("meters: {0}", 0);

			StartCoroutine( CursoreMove() );
		}

		protected void WorkOnHide()
		{
			isNitroPressed = false;
			updateUI = false;
			_playerCar = null;

			//
			HideUiPanel(_uiPanelOld);
			HideUiPanel(_uiPanelNew);
		}

		private void SetVisibleNitro(float currentValue, float maxValue)
		{
			slider.fillAmount = currentValue / maxValue;
		}

		private IEnumerator CursoreMove()
		{


			while (true && _playerCar)
			{
               
                _cursorSpeed = Mathf.Abs(_playerCar.controller.speed / _playerCar.controller.maxSpeedForward) * 160;

                if (_cursorSpeed >= 110)
                {
                    _cursorSpeed = 110;
                }

                
                cursore.rotation = Quaternion.Euler(
					Vector3.Slerp(cursore.rotation.eulerAngles,
						new Vector3(0, 
                        180,
                        _cursorSpeed
                        ),
						Time.deltaTime));
                
              

                 yield return new WaitForEndOfFrame();
			}
		}

		#region CHECK_UI_SAVED

		private void CheckSavedUISettings()
		{
			bool isUiTypeWasSaved = PlayerPrefs.HasKey(PPKeys.uiType);

			if (isUiTypeWasSaved)
			{
				InitUiPanel();
			}
			else
			{
				SaveDefaultUISettings();
				InitUiPanel();
			}
		}

		private void SaveDefaultUISettings()
		{
			int defaultUI = (int) UITypeEnum.OLD_UI;
			PlayerPrefs.SetInt(PPKeys.uiType, defaultUI);
		}

		#endregion

		#region SHOW_UI_TYPE

		protected void InitUiPanel()
		{
			UITypeEnum uiType = (UITypeEnum) PlayerPrefs.GetInt(PPKeys.uiType);

			switch (uiType)
			{
				case UITypeEnum.OLD_UI:
					ShowUiPanel(_uiPanelOld);
					HideUiPanel(_uiPanelNew);
					InitUiPanelOld();
					break;

				case UITypeEnum.NEW_UI:
					ShowUiPanel(_uiPanelNew);
					HideUiPanel(_uiPanelOld);
					InitUiPanelNew();
					break;

				default:
					#region DEBUG
#if UNITY_EDITOR
					Debug.Log("[ERROR] wrong uiType = " + uiType);
#endif
					#endregion
					break;
			}
		}

		protected void InitUiPanelOld()
		{
			nitroImages = _uiPanelOld.Get_NitroImages();
			timerText = _uiPanelOld.Get_TimerText();

			triesSprite = _uiPanelOld.Get_TriesSprite();

			lostTrie = _uiPanelOld.Get_LostTrie();
			trie = _uiPanelOld.Get_Trie();

			meterCountLabel = _uiPanelOld.Get_MeterCountLabel();
            

            currentSpeedLabel = _uiPanelOld.Get_CurrentSpeedLabel();

            enemyCountMeter = _uiPanelOld.Get_EnemyCountMeter();
            enemyCountImage = _uiPanelOld.Get_EnemyCountImage();
            leaderSprite = _uiPanelOld.Get_LeaderSprite();
            looserSprite = _uiPanelOld.Get_LooserSprite();
            crashedSprite = _uiPanelOld.Get_CrashedSprite();

            pauseButton = _uiPanelOld.Get_PauseButton();
            forwardButton = _uiPanelOld.Get_ForwardButton();

			

			cursore = _uiPanelOld.Get_Cursore();

			nitro = _uiPanelOld.Get_Nitro();
			nitroButtonImage = _uiPanelOld.Get_NitroButtonImage();
			slider = _uiPanelOld.Get_Slider();

			panelsList = _uiPanelOld.Get_PanelsList();

			podium = _uiPanelOld.Get_Podium();

			_textRaceTimer = _uiPanelOld.Get_TextRaceTimer();
			_textJewels = _uiPanelOld.Get_TextJewels();
			_textGold = _uiPanelOld.Get_TextGold();
            _ImageJewels = _uiPanelOld.Get_Jewels();
            _ImageGold = _uiPanelOld.Get_Gold();

        }

		private void InitUiPanelNew()
		{
			nitroImages = _uiPanelNew.Get_NitroImages();
			timerText = _uiPanelNew.Get_TimerText();

			triesSprite = _uiPanelNew.Get_TriesSprite();

			lostTrie = _uiPanelNew.Get_LostTrie();
			trie = _uiPanelNew.Get_Trie();

			meterCountLabel = _uiPanelNew.Get_MeterCountLabel();
            enemyCountMeter = _uiPanelNew.Get_EnemyCountMeter();
            currentSpeedLabel = _uiPanelNew.Get_CurrentSpeedLabel();

			pauseButton = _uiPanelNew.Get_PauseButton();

			

			cursore = _uiPanelNew.Get_Cursore();

			nitro = _uiPanelNew.Get_Nitro();
			nitroButtonImage = _uiPanelNew.Get_NitroButtonImage();
			slider = _uiPanelNew.Get_Slider();

			panelsList = _uiPanelNew.Get_PanelsList();

			podium = _uiPanelNew.Get_Podium();

			_textRaceTimer = _uiPanelNew.Get_TextRaceTimer();
			_textJewels = _uiPanelNew.Get_TextJewels();
			_textGold = _uiPanelNew.Get_TextGold();
            _ImageJewels = _uiPanelOld.Get_Jewels();
        }

		protected void ShowUiPanel(ABaseUiPanel uiPanel)
		{
			CanvasGroup panelCanvasGroup = uiPanel.GetComponent<CanvasGroup>();
			Assert.AreNotEqual(null, panelCanvasGroup);

			panelCanvasGroup.alpha = 1;
			panelCanvasGroup.blocksRaycasts = true;
		}

		protected void HideUiPanel(ABaseUiPanel uiPanel)
		{
			CanvasGroup panelCanvasGroup = uiPanel.GetComponent<CanvasGroup>();
			Assert.AreNotEqual(null, panelCanvasGroup);

			panelCanvasGroup.alpha = 0;
			panelCanvasGroup.blocksRaycasts = false;
		}

		#endregion



	}
}