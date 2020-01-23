using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using HCR.Enums;
using HCR.Interfaces;
using System;
using System.Collections;

namespace HCR.Gameplay
{
	public abstract class ABaseFinishWindow : MonoBehaviour
	{
        // FIELDS
        public Action NExtTrack;
        #region VARIABLES

        // --------------------------------------------------
        //
        [SerializeField] protected CanvasGroup _canvaseGroup;
        [SerializeField]
        protected CanvasGroup _canvaseGroupQuestion;

        // --------------------------------------------------
        //
        [Header("PANEL NAME")]
		[SerializeField] protected Text _textPanelName;
        [SerializeField] protected Image _AccountGold;

		// --------------------------------------------------
		//
		[Header("STARS")]
		[SerializeField] protected Image _imageStar_0;
		[SerializeField] protected Image _imageStar_1;
		[SerializeField] protected Image _imageStar_2;

        [SerializeField]protected Animator bonus2Animator;
        [SerializeField]protected Animator bonus3Animator;
        [SerializeField]protected Animator bonus4Animator;

        [SerializeField]
        protected Image _imageStar_0Gold;
        [SerializeField]
        protected Image _imageStar_1Gold;
        [SerializeField]
        protected Image _imageStar_2Gold;

        protected List<Image> _stars;
        protected List<Image> _starsGold;

        [Space]
		[SerializeField] protected Sprite _spriteStarDark;
		[SerializeField] protected Sprite _spriteStarGold;

		// --------------------------------------------------
		//
		[Header("WIN ATTEMPST")]
		[SerializeField] protected Text _textLabelWinAttempts;        // X1 / X2 / X3 - выиграл с первого / второго / третьего раза

		protected string _defaultWinAttemptsLabel = "win attempts";

        [Header("JEWELS")]
        [SerializeField]
        protected Image _imageJewels;
        [SerializeField]
        protected Text _textJewels;

        [Header("GOLD")]
		[SerializeField] protected Text _textGoldForWinAttempts;    // бонусное серебро за победу - x1 / x2 / x3
		[SerializeField] protected Text _textGoldCollected;  
        [SerializeField] protected Text _bonusGold;
        [SerializeField] protected Text _bonusGoldText;// собрано серебра за заезд
		          // заработано серебра = собрано серебра за заезд + бонусное

		[Space]
		[SerializeField] protected Text _textGoldTotal;             // ВСЕГО серебра за заезд + то что было раньше у Игрока

		// --------------------------------------------------
		//
		[Header("STAMPS")]
		[SerializeField] protected Image _imageStampX2;
		[SerializeField] protected Image _imageStampX3;
		[SerializeField] protected Image _imageStampX4;


        [SerializeField]
        protected Sprite _SpriteStampX2;
        [SerializeField]
        protected Sprite _SpriteStampX3;
        [SerializeField]
        protected Sprite _SpriteStampX4;
        [Space]
		[SerializeField] protected Sprite _spriteStampGold;

		// --------------------------------------------------
		//
		[Header("EXP")]
		[SerializeField] protected Text _textTrickAirTime;
		[SerializeField] protected Text _textTrickFlips;
		[SerializeField] protected Text _textTrickHorseTime;
		[SerializeField] protected Text _textTrick90;
        [SerializeField] protected Text _textExpForTricks;
        [SerializeField] protected Text _textExpForWin;
        [SerializeField] protected Text _textExpPremium;
        [SerializeField] protected Text _textExpTotal; 
		[Space]
        [SerializeField]
        protected Text _TitleExpForWin;
        [SerializeField]
        protected Text _TitleExpPremium;
        [SerializeField] protected Image _imageExpProgrssBar;
        [SerializeField]
        protected Image _imageExpPemium;

        // --------------------------------------------------
        //
        [Header("SEND_RECORD")]
		[SerializeField] protected Button _buttonSendRecord;

		#endregion

		// dependences
		protected PlayerManager _playerManager;
		protected StampTimerManager _stampTimerManager;
		protected NetworkManager _networkManager;
		protected SafePlayerPrefs _safePlayerPrefs;
		protected UIAnimatorManager _uiAnimatorManager;

		protected UIManager _uiManager;
		protected MainScreenStateManager _mainScreenStateManager;

		// must be set from another class
		protected int _goldForWinAttempts;
        protected int _expForWinAttempts = 0;
        protected RaceResultEnum _raceResult;
        protected int  earnedGoldWithMult = 0;
        protected bool IsFiilGold = true;
        protected int  IsFinish = 0;

        // INTERFACES

        #region SET_VALUE (from another class)

        public void SetValue_GoldForWinAttempts(int gold)
		{
            _goldForWinAttempts = gold;
		}

        public void SetValue_IsFinish(int IsFinish)
        {
            this.IsFinish = IsFinish;
        }

        public void SetValue_XPForWinAttempts(int XP)
        {
            _expForWinAttempts = XP;
        }

        public void SetValue_RaceResult(RaceResultEnum raceResult)
		{
			_raceResult = raceResult;
		}

		#endregion



		// METHODS

		protected void InitVariables()
		{
			_playerManager = Core.Instance.GetService<PlayerManager>();
			Assert.AreNotEqual(null, _playerManager);

			_stampTimerManager = Core.Instance.GetService<StampTimerManager>();
			Assert.AreNotEqual(null, _stampTimerManager);

			_networkManager = Core.Instance.GetService<NetworkManager>();
			Assert.AreNotEqual(null, _networkManager);

			_safePlayerPrefs = Core.Instance.GetService<SafePlayerPrefs>();
			Assert.AreNotEqual(null, _safePlayerPrefs);

			_uiAnimatorManager = Core.Instance.GetService<UIAnimatorManager>();
			Assert.AreNotEqual(null, _uiAnimatorManager);

			//
			_uiManager = Core.Instance.GetService<UIManager>();
			Assert.AreNotEqual(null, _uiManager);

			_mainScreenStateManager = _uiManager.Get_MainScreenStateManager();
			Assert.AreNotEqual(null, _mainScreenStateManager);

			//
			_stars = new List<Image>();
			_stars.Add(_imageStar_0);
			_stars.Add(_imageStar_1);
			_stars.Add(_imageStar_2);

            _imageStar_0Gold.gameObject.SetActive(false);
            _imageStar_1Gold.gameObject.SetActive(false);
            _imageStar_2Gold.gameObject.SetActive(false);


            _starsGold = new List<Image>();
            _starsGold.Add(_imageStar_0Gold);
            _starsGold.Add(_imageStar_1Gold);
            _starsGold.Add(_imageStar_2Gold);

            
        }

		protected void CheckButtonsStampX()
		{
			DisableMultiplierButtons();

			if (_stampTimerManager.Get_IsX2_Active() && IsFinish == 1)
			{
				_imageStampX2.GetComponent<Button>().interactable = true;
            }else
            {
                _imageStampX2.GetComponent<Button>().interactable = false;
                _imageStampX2.sprite = _SpriteStampX2;
                _imageStampX2.SetNativeSize();

            }
			if (_stampTimerManager.Get_IsX3_Active() && IsFinish == 1)
			{
				_imageStampX3.GetComponent<Button>().interactable = true;
			}
            else
            {
                _imageStampX3.GetComponent<Button>().interactable = false;
                _imageStampX3.sprite = _SpriteStampX2;
                _imageStampX3.SetNativeSize();

            }
            if (_stampTimerManager.Get_IsX4_Active() && IsFinish == 1)
			{
				_imageStampX4.GetComponent<Button>().interactable = true;
			}
            else
            {
                _imageStampX4.GetComponent<Button>().interactable = false;
                _imageStampX4.sprite = _SpriteStampX4;
                _imageStampX4.SetNativeSize();
                

            }
        }

		protected bool IsCanClickStampX(int multiplier)
		{
			if ( multiplier == 2 && _stampTimerManager.Get_IsX2_Active() )
			{
				return true;
			}
			if ( multiplier == 3 && _stampTimerManager.Get_IsX3_Active() )
			{
				return true;
			}
			if ( multiplier == 4 && _stampTimerManager.Get_IsX4_Active() )
			{
				return true;
			}

			return false;
		}

		protected void ShowPanelName()
		{
			switch (_raceResult)
			{
				case RaceResultEnum.WIN:
					_textPanelName.text = "WIN";
					break;

				case RaceResultEnum.LOSING:
					_textPanelName.text = "LOSING";
					break;
                case RaceResultEnum.DRAW:
                    _textPanelName.text = "DRAW";
                    break;

				default:
					#region DEBUG
#if UNITY_EDITOR
					Debug.Log("[ERROR] _raceResult is incorrect = " + _raceResult);
#endif
					#endregion
					break;
			}
		}

        public int XPForStars(IGameWindow gameWindow)
        {
            int triesCount = gameWindow.Get_TriesCount();

            if (triesCount == 3)
                return  400;
            else if(triesCount == 2)
                return  200;
            else if (triesCount == 1)
                return  100;

            else return  0;

            

        }

		protected void ShowStars(IGameWindow gameWindow, Action OnComplete)
		{
			
			int triesCount = gameWindow.Get_TriesCount();

			if (triesCount >= 3) {
				triesCount = 3; }

			if (_raceResult == RaceResultEnum.WIN)
			{
                StartCoroutine(Wait(0.5f, 
                    () => {
                        ShowAnimStar(triesCount, 0, OnComplete);
                          }));
                
            }else
            {
                OnComplete();
            }
			
		}

        private IEnumerator Wait(float time, Action OnComplete)
        {
            yield return new WaitForSeconds(time);
            OnComplete();
        }

        private void ShowAnimStar(int star,int index, Action OnComplete)
        {
            if(index > 3) { return; }


            if (index < star)
            {
                
                
                _starsGold[index].gameObject.SetActive(true);

                

                _starsGold[index].transform.DOScale(1, 0.2f).OnComplete(() =>
                {
                    
                    _starsGold[index].transform.DOPunchScale(new Vector3(0.65f, 0.65f), 0.35f, 1).OnComplete(() => { ShowAnimStar(star, ++index, OnComplete); });
                });
                    
            }else
            {
                OnComplete();
            }
           
        }

		protected void ShowWinAttemptsX(IGameWindow gameWindow)
		{
			if (_raceResult != RaceResultEnum.WIN && _safePlayerPrefs.GetInt_EarnedGold() > 0)
			{
				_textLabelWinAttempts.text = string.Format("{0}:", _defaultWinAttemptsLabel);
                _TitleExpForWin.text = string.Format("{0}:", "stars");

                return;
			}

			int triesCount = gameWindow.Get_TriesCount();
			string multiplier = "";

			switch (triesCount)
			{
				case 3:
					multiplier = "X1";
					break;
				case 2:
					multiplier = "X2";
					break;
				case 1:
					multiplier = "X3";
					break;
				default:
					#region DEBUG
#if UNITY_EDITOR
					Debug.Log("[ERROR] triesCount (must be 1 or 2 or 3), but it is = " + triesCount);
#endif
					#endregion
					break;
			}

			_textLabelWinAttempts.text = string.Format("{0} {1}:", _defaultWinAttemptsLabel, multiplier);
            _TitleExpForWin.text =  string.Format("{0} {1}:", "stars", multiplier);

        }

		#region STAMPS

		protected void ShowStamps()
		{
			bool isStampX2 = _stampTimerManager.Get_IsX2_Active();
			bool isStampX3 = _stampTimerManager.Get_IsX3_Active();
			bool isStampX4 = _stampTimerManager.Get_IsX4_Active();

			if (isStampX2)
			{
				ShowStampGoldSprite(_imageStampX2);
                bonus2Animator.enabled = true;
			}
			if (isStampX3)
			{
				ShowStampGoldSprite(_imageStampX3);
                bonus3Animator.enabled = true;

            }
			if (isStampX4)
			{
				ShowStampGoldSprite(_imageStampX4);
                bonus4Animator.enabled = true;

            }
		}

		protected void ShowStampGoldSprite(Image imageStamp)
		{
			imageStamp.sprite = _spriteStampGold;
			imageStamp.SetNativeSize();
		}

       
        #endregion

        #region CLICK_BUTTON_MULTIPLIER

        // callback !
        protected void OnGoldWasMultiplied()
		{
			// TODO --- hide loader !

			EnableSendRecordButton();
		}

		protected void DisableMultiplierButtons()
		{
			_imageStampX2.GetComponent<Button>().interactable = false;
			_imageStampX3.GetComponent<Button>().interactable = false;
			_imageStampX4.GetComponent<Button>().interactable = false;
            bonus2Animator.enabled = false;
            bonus3Animator.enabled = false;
            bonus4Animator.enabled = false;
        }

		protected void EnableSendRecordButton()
		{
			_buttonSendRecord.interactable = true;
		}

		protected void DisableSendRecordButton()
		{
			_buttonSendRecord.interactable = false;
		}

		#endregion

		protected void ShowGoldWithChoosenMultiplier(int multiplier)
		{
            
            earnedGoldWithMult = _safePlayerPrefs.GetInt_EarnedGold() * multiplier;
            if(!IsFiilGold)
            Callback_GoldTotalMulti(earnedGoldWithMult);
        }

        #region EXP progress bar

        protected void InitExpBar()
        {
            float Min = 0;
            Debug.Log("_playerManager.level " + _playerManager.level);
            float xpNeeded = GetXpNeeded(_playerManager.level);

            if (_playerManager.level > 1)
                Min = GetXpNeeded(_playerManager.level - 1);

            _imageExpProgrssBar.fillAmount = (xpNeeded - Min  + (_playerManager.xp - xpNeeded)) / (xpNeeded - Min);

            Debug.Log("Fill " + _imageExpProgrssBar.fillAmount);

            _textExpTotal.text = string.Format("{0} XP", _playerManager.xp.ToString());



        }

		protected void ShowLevelProgress()
		{

            float xpTotal = _playerManager.xp + (int)_safePlayerPrefs.GetFloat_EarnedExp() + _expForWinAttempts;
            float xpNeeded = GetXpNeeded(_playerManager.level);
            float Min = 0;
            if (_playerManager.level > 1)
                Min = GetXpNeeded(_playerManager.level - 1);

            Debug.Log("level " + _playerManager.level);
            float expBarValue = (xpNeeded - Min + (xpTotal - xpNeeded)) / Math.Abs(xpNeeded - Min);
            Debug.Log("expBarValue " + expBarValue + " xpTotal " + xpTotal + " xpNeeded " + xpNeeded + " Min " + Min);
            _imageExpProgrssBar.DOFillAmount(expBarValue, 1f).OnComplete(
                () =>
                      {
                          Debug.Log("expBarValue 1 " + expBarValue);
                          if (expBarValue > 1f)
                          {
                              Debug.Log("expBarValue " + expBarValue);
                              ShowlvlProgressUP(_imageExpProgrssBar, xpTotal, xpNeeded, (val, lvl) =>
                              {
                                  Debug.Log("Callback_XpBonus(val, lvl); ");
                                  Callback_XpBonus(val, lvl);


                              });
                          }else
                          {
                              Callback_XpBonus(_imageExpPemium.fillAmount, _playerManager.level);
                          }
                      }
                      );
		}

        private void ShowLevelProgressPremium(float Val , int lvl, int bonus)
        {
            if (bonus == 0) return;
            Debug.Log("ShowLevelProgressPremium");
            _imageExpPemium.fillAmount = Val;
            
            float xpTotal = _playerManager.xp + (int)_safePlayerPrefs.GetFloat_EarnedExp() + bonus + _expForWinAttempts;
            float xpNeeded = GetXpNeeded(lvl);
            float Min = 0;
            if (lvl > 1)
                Min = GetXpNeeded(lvl - 1);
            Debug.Log("level " + _playerManager.level);
            float expBarValue = (xpNeeded - Min + (xpTotal - xpNeeded)) / Math.Abs(xpNeeded - Min);
            Debug.Log("expBarValue " + expBarValue + " xpTotal " + xpTotal + " xpNeeded " + xpNeeded + " Min " + Min);
            _imageExpPemium.DOFillAmount(expBarValue, 1f).OnComplete(
                () =>
                {
                    if (expBarValue < 1f) return; 
                    _imageExpProgrssBar.fillAmount = 0f;
                    ShowlvlProgressUP(_imageExpPemium, xpTotal, xpNeeded);
                    
                });

        }

        private void ShowlvlProgressUP(Image img, float xpTotal, float xpNeeded,  Action<float, int> Complete = null)
        {
            
            if (xpTotal > xpNeeded)
            {
                img.fillAmount = 0f;
                Debug.Log("level " + _playerManager.level);
                int level = _playerManager.level;
                float Min = _playerManager.allLevelsXP[level-1].maxValue;
                float Max = _playerManager.allLevelsXP[level].maxValue;

                float expBarValue = (Max - Min + (xpTotal - Max)) / Math.Abs(Max - Min);
                Debug.Log("expBarValue " + expBarValue + " xpTotal " + xpTotal + " xpNeeded " + Max + " Min " + Min);
                _playerManager.level += 1;
                img.DOFillAmount(expBarValue, 1f).OnComplete(
                    ()=> {
                            if (xpTotal > Max) {
                                ShowlvlProgressUP(img, xpTotal, Max, Complete);
                                }else
                    {
                        Complete(expBarValue, _playerManager.level);
                    }
                    
                });
            }

        }

		protected int GetXpNeeded(int level)
		{
			for (int i = 0; i < _playerManager.allLevelsXP.Count; i++)
			{
				if (level == _playerManager.allLevelsXP[i].ID)
				{
					return _playerManager.allLevelsXP[i].maxValue;
				}
			}

			#region DEBUG
#if UNITY_EDITOR
			Debug.Log("[ERROR] can't get xp needed! Level = " + _playerManager.level);
#endif
			#endregion

			return 0;
		}

		#endregion

		#region UI_ANIMATIONS

		// ==========================================================================
		// Gold
        private void SetZeroTricks()
        {
            _textTrickAirTime.text = _textTrickFlips.text = _textTrickHorseTime.text = _textTrick90.text =  
                _textGoldCollected.text = _textGoldForWinAttempts.text  = _textGoldTotal.text = _textExpForTricks.text = _textExpForWin.text = _textExpPremium.text = "0";
            earnedGoldWithMult = 0;                                                             
            //_imageExpProgrssBar.DOFillAmount(0, 0f);                                          
        }

		protected void ShowAllUIAnimations(IGameWindow gameWindow)
		{
            
            SetZeroTricks();
            ShowStars(gameWindow , ()=> {
                Callback_GoldForWinAttempts();
                Callback_GoldCollected();
                Callback_GoldBonus();
                
                Callback_GoldTotal(() => {

                    Callback_JewelsTotal();
                    Callback_TrickAirTime();
                    Callback_TrickFlips();
                    Callback_TrickHorse();
                    Callback_Trick90Earned();
                    Callback_ExpForWin();
                    Callback_ExpForTrick();
                    Callback_ExpTotal();
                });
            });
        }

		protected void Callback_GoldForWinAttempts()
		{
            //Debug.Log("CALLBACK - Gold for win attempts !!!");
            if (_goldForWinAttempts != 0)
            {
                _uiAnimatorManager.ShowTextFieldUpdateAniamtion(
                    _textGoldForWinAttempts, 0,
                    _goldForWinAttempts
                    );
            }
		}

		protected void Callback_GoldCollected()
		{
            //Debug.Log("CALLBACK - Gold collected !!!");
            _uiAnimatorManager.ShowTextFieldUpdateAniamtion(
                _textGoldCollected, 0,
                _safePlayerPrefs.GetInt_EarnedGold()
               );

           
		}

        protected void Callback_GoldBonus()
        {if (_playerManager.accountType == 1)
            {
                _bonusGold.gameObject.SetActive(true);
                
                _uiAnimatorManager.ShowTextFieldUpdateAniamtion(
                _bonusGoldText, 0,
                (int)((_safePlayerPrefs.GetInt_EarnedGold() + _goldForWinAttempts )* 0.5f)
                );  
            }
            
        }
          
        protected void Callback_GoldTotal(Action OnComplete)
        {
           
            float bonus = (_playerManager.accountType == 1 )? _safePlayerPrefs.GetInt_PlayerGold() * 0.5f : 0;

            Debug.Log("_safePlayerPrefs.GetInt_PlayerGold() " + _safePlayerPrefs.GetInt_PlayerGold() + " " + bonus);

            _uiAnimatorManager.ShowTextFieldUpdateAniamtion(
                _textGoldTotal, 0,
                (int)(_safePlayerPrefs.GetInt_PlayerGold() + bonus) , () =>
                {
                    if (earnedGoldWithMult != 0)
                    {
                        StartCoroutine(Wait(0.2f, () => {
                            Callback_GoldTotalMulti(earnedGoldWithMult, OnComplete);
                        }));
                        
                    }
                    else
                    {
                        IsFiilGold = false;
                        OnComplete();
                    }
                }
                );
            
        }
        
        protected void Callback_GoldTotalMulti(int earnedGoldWithMult, Action OnComplete = null)
		{
            Debug.Log("__ _safePlayerPrefs.GetInt_PlayerGold() !!! = " + _safePlayerPrefs.GetInt_PlayerGold() + " " + earnedGoldWithMult + " " + _safePlayerPrefs.GetInt_EarnedGold());
            float bonus = (_playerManager.accountType == 1) ? _safePlayerPrefs.GetInt_PlayerGold() * 0.5f : 0;
            //Debug.Log("CALLBACK - Gold total !!!");
            if (earnedGoldWithMult != 0)
            {
                _uiAnimatorManager.ShowTextFieldUpdateAniamtion(
                _textGoldTotal,
                int.Parse(_textGoldTotal.text), _safePlayerPrefs.GetInt_PlayerGold() + (earnedGoldWithMult - _safePlayerPrefs.GetInt_EarnedGold() + (int)bonus), OnComplete);
                
            }
			
		}
        
        protected void Callback_JewelsTotal()
        {
            _uiAnimatorManager.ShowTextFieldUpdateAniamtion(
               _textJewels,
               0, _safePlayerPrefs.GetInt_EarnedJewels());
        }

        // ==========================================================================
        // EXP
        protected void Callback_TrickAirTime()
		{
            //Debug.Log("CALLBACK - trick AirTime");
            _uiAnimatorManager.ShowTextFieldUpdateAniamtion(
                _textTrickAirTime, 0,
                _safePlayerPrefs.GetFloat_TrickAirTime()
                );
		}

		protected void Callback_TrickFlips()
		{
            //Debug.Log("CALLBACK - trick Flips");
            _uiAnimatorManager.ShowTextFieldUpdateAniamtion(
                _textTrickFlips, 0,
                _safePlayerPrefs.GetInt_TrickFlipSum()
                );
		}

		protected void Callback_TrickHorse()
		{
            //Debug.Log("CALLBACK - trick House ");
            _uiAnimatorManager.ShowTextFieldUpdateAniamtion(
                _textTrickHorseTime, 0,
                _safePlayerPrefs.GetFloat_HorseTime()
                );
		}

		protected void Callback_Trick90Earned()
		{
            //Debug.Log("CALLBACK - exp earned ");
            _uiAnimatorManager.ShowTextFieldUpdateAniamtion(
                _textTrick90, 0,
                (int)_safePlayerPrefs.GetFloat_90Time()
                );


            
		}

        protected void Callback_ExpForWin()
        {
            if (_expForWinAttempts != 0)
            {
                Debug.Log("_expForWinAttempts " + _expForWinAttempts);
                _uiAnimatorManager.ShowTextFieldUpdateAniamtion(
                    _textExpForWin, 0, _expForWinAttempts,
                    () =>
                    {
                        _textExpForWin.text = string.Format("{0} XP", _textExpForWin.text);
                    });
            }
        }

        protected void Callback_ExpForTrick()
        {
            if ((int)_safePlayerPrefs.GetFloat_EarnedExp() != 0)
            {
                Debug.Log("_textExpForTricks " + (int)_safePlayerPrefs.GetFloat_EarnedExp());
                _uiAnimatorManager.ShowTextFieldUpdateAniamtion(
                    _textExpForTricks, 0,
                    
                      (int)_safePlayerPrefs.GetFloat_EarnedExp(),
                    () =>
                    {
                        _textExpForTricks.text = string.Format("{0} XP", _textExpForTricks.text);
                    });
            }
        }

        protected void Callback_ExpTotal()
		{
            if ((int)_safePlayerPrefs.GetFloat_EarnedExp() != 0 || _expForWinAttempts != 0)
            {
                ShowLevelProgress();
                Debug.Log("_textExpTotal " + (int)_safePlayerPrefs.GetFloat_PlayerExp() + " " + _playerManager.xp);
                _uiAnimatorManager.ShowTextFieldUpdateAniamtion(
                    _textExpTotal, _playerManager.xp,
                    _playerManager.xp +
                      (int)_safePlayerPrefs.GetFloat_PlayerExp(),
                    () =>
                    {
                        _textExpTotal.text = string.Format("{0} XP", _textExpTotal.text);
                    });
            }
		}

        protected void Callback_XpBonus(float Val, int lvl)
        {
            if (_playerManager.accountType == 1 && _safePlayerPrefs.GetFloat_EarnedExp() > 0)
            {
                Debug.Log("Callback_XpBonus " );

                int bonus = (int)(_safePlayerPrefs.GetFloat_EarnedExp() * 0.5f);
                ShowLevelProgressPremium(Val, lvl, bonus);
                _uiAnimatorManager.ShowTextFieldUpdateAniamtion(_textExpPremium, 0, bonus, () =>
                {
                    _textExpPremium.text = string.Format("{0} XP", _textExpPremium.text);
                });

                _uiAnimatorManager.ShowTextFieldUpdateAniamtion(
                _textExpTotal, _playerManager.xp +
                  (int)_safePlayerPrefs.GetFloat_EarnedExp(),
                _playerManager.xp +
                  (int)_safePlayerPrefs.GetFloat_PlayerExp() + bonus, () =>
                  {
                      _textExpTotal.text = string.Format("{0} XP", _textExpTotal.text);
                  });
            }

        }
        #endregion



    }
}