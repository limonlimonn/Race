using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Assertions;

using HCR.Interfaces;
using HCR.Enums;
using DG.Tweening;
using HCR.Event;

namespace HCR.Gameplay.AsyncMultiplayer
{
	/// <summary>
	/// Класс - окно, когда гонялись с "призраком" другого игрока и
	/// выиграли или проиграли
	/// </summary>

	public class FinishWindow : ABaseFinishWindow, IUIWindow
	{
		// ACTIONS

		public Action OnFinishClick;
        public Action OnBuyTriesClick;
        public Text PriceBuyTryes;
        // FIELDS
       
        #region VARIALBLES

        // --------------------------------------------------
        //
        [Header("PLAYER")]
		[SerializeField] private Text _textPlayerName;
		[SerializeField] private Text _textPlayerScore;

		[Header("ENEMY")]
		[SerializeField] private Text _textEnemyName;
		[SerializeField] private Text _textEnemyScore;
        public Image TimeBuy;

        

        
        #endregion

        // dependences
        private GameWindowAsyncMultiplayer _gameWindow;

		// must be set from another class
		private int _playerScore;
		private int _enemyScore;
		private string _enemyName;

        Tween timmerTween;


		// I_UI_WINDOW

		public void Init()
		{
			base.InitVariables();

			_gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_ASYNC) as GameWindowAsyncMultiplayer;
			Assert.AreNotEqual(null, _gameWindow);
            TimeBuy.DOFillAmount(1, 0f);
            //
            AssertVariables();
            
            

        }

        public void BuyTriesQuestion()
        {
            ShowQuestion(
                // End Time Question
                () =>{ EventManager._init.Game.PlayerEvent.Invoke_CancleBuyTries(); }
            );



        }

        public void ShowQuestion(Action OnComplete)
        {
            Debug.LogError("SHowQW");
            WindowViewHandler.Show(_canvaseGroupQuestion);
            timmerTween =  TimeBuy.DOFillAmount(0, 5f).OnComplete(() => {  HideQuestion(); OnComplete(); });
        }
        public void HideQuestion()
        {
            WindowViewHandler.Hide(_canvaseGroupQuestion);
            TimeBuy.DOFillAmount(1, 0f);
            
        }

        public void InitVideo()
        {
            AdMobManager.adMob.InitVideo(OnBuyTriesClick, CloseRewardVideo);
        }


        public void Show()
		{

            
            WindowViewHandler.Show(_canvaseGroup);

            //
            CheckValue();
            
            InitExpBar();
            CheckButtonsStampX();
			ShowPanelName();
			//ShowStars(_gameWindow);
			ShowPlayerAndEnemyNamesScores();
			ShowWinAttemptsX(_gameWindow);
			ShowStamps();
           
			ShowAllUIAnimations(_gameWindow);

			//_gameWindow.BlockPauseButton();
		}

		public void Hide()
		{
			WindowViewHandler.Hide(_canvaseGroup);
            _imageJewels.gameObject.SetActive(false);
        }

        private void CheckValue()
        {
            
            IsFiilGold = true;
            

            _imageExpPemium.fillAmount = 0f;
            _imageExpProgrssBar.fillAmount = 0f;

            _imageStar_0Gold.gameObject.SetActive(false);
            _imageStar_1Gold.gameObject.SetActive(false);
            _imageStar_2Gold.gameObject.SetActive(false);

            _imageJewels.gameObject.SetActive(false);
            if (_safePlayerPrefs.GetInt_EarnedJewels() != 0)
            {
                _imageJewels.gameObject.SetActive(true);
                _textJewels.text = "0";
            }
            if (_playerManager.accountType == 1)
            {
                _AccountGold.gameObject.SetActive(true);
                _TitleExpPremium.gameObject.SetActive(true);
                _bonusGold.gameObject.SetActive(true);
            }else
            {
                _AccountGold.gameObject.SetActive(false);
                _TitleExpPremium.gameObject.SetActive(false);
                _bonusGold.gameObject.SetActive(false);
            }
            if (_raceResult != RaceResultEnum.WIN)
            {
                _textLabelWinAttempts.gameObject.SetActive(false);
                _TitleExpForWin.gameObject.SetActive(false);
            }else
            {
                _textLabelWinAttempts.gameObject.SetActive(true);
                _TitleExpForWin.gameObject.SetActive(true);
            }
        }


        // INTERFACES

        #region SET_VALUE (from another class)

        public void SetValue_PlayerScore(int score)
		{
			_playerScore = score;
		}

		public void SetValue_EnemyScore(int score)
		{
			_enemyScore = score;
		}

		public void SetValue_EnemyName(string enemyName)
		{
			_enemyName = enemyName;
		}

		#endregion

		// used on button !
		public void OnClickButton_StampX(int multiplier)
		{
			// pre-condition
			#region ASSERTS
			Assert.IsTrue(multiplier == 2 || multiplier == 3 || multiplier == 4);
			#endregion

			if (!IsCanClickStampX(multiplier)) {
				return; }

			DisableMultiplierButtons();
			DisableSendRecordButton();
			ShowGoldWithChoosenMultiplier(multiplier);
            
            //ShowAllUIAnimations();

            // TODO --- show loader !

            Debug.Log("__ OnClickButtonStampX !!! = " + multiplier);

            _safePlayerPrefs.Safe_SaveGoldWithMultiplier(_safePlayerPrefs.GetInt_EarnedGold(), multiplier, OnGoldWasMultiplied);

            /*
			_networkManager.SaveGoldWithMultiplier(multiplier, OnGoldWasMultiplied,
				() => Debug.Log("[ERROR] SaveGoldWithMultiplier !") );
            */
        }

		// used on button !
		public void OnClickButton_RecordYourTurn()
		{
            
			WindowViewHandler.Hide(_canvaseGroup);
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
            _mainScreenStateManager.SwitchState(MainScreenStatesEnum.CHOOSE_TRACK);
            _AccountGold.gameObject.SetActive(false);
            _bonusGold.gameObject.SetActive(false);
            
        }

        public void OnClickButton_BuyTries()
        {if (OnBuyTriesClick != null)
            {
#if UNITY_EDITOR
                 OnBuyTriesClick();
#endif
#if UNITY_ANDROID

                timmerTween.Pause();
                    AdMobManager.adMob.ShowVideo();
#endif
                //Debug.LogError("UNITY_EDITOR");
                // CloseRewardVideo();
            }
        }

        private void CloseRewardVideo()
        {
            OnBuyTriesClick = null;
            timmerTween.Play();
        }

		// METHODS

		private void AssertVariables()
		{
#region ASSERT

			Assert.AreNotEqual(null, _canvaseGroup);

			// panel name
			Assert.AreNotEqual(null, _textPanelName);

			// stars
			Assert.AreNotEqual(null, _imageStar_0);
			Assert.AreNotEqual(null, _imageStar_1);
			Assert.AreNotEqual(null, _imageStar_2);

			Assert.AreNotEqual(null, _stars);
			Assert.IsTrue(_stars.Count > 0);

			Assert.AreNotEqual(null, _spriteStarDark);
			Assert.AreNotEqual(null, _spriteStarGold);

			// player/enemy names and scores
			Assert.AreNotEqual(null, _textPlayerName);
			Assert.AreNotEqual(null, _textPlayerScore);

			Assert.AreNotEqual(null, _textEnemyName);
			Assert.AreNotEqual(null, _textEnemyScore);

			// win attempts
			Assert.AreNotEqual(null, _textLabelWinAttempts);

			// silver
			Assert.AreNotEqual(null, _textGoldForWinAttempts);
			Assert.AreNotEqual(null, _textGoldCollected);
			Assert.AreNotEqual(null, _textGoldTotal);

			// exp
			Assert.AreNotEqual(null, _textTrickAirTime);
			Assert.AreNotEqual(null, _textTrickFlips);
			Assert.AreNotEqual(null, _textTrickHorseTime);
			Assert.AreNotEqual(null, _textTrick90);
			Assert.AreNotEqual(null, _textExpTotal);

			Assert.AreNotEqual(null, _imageExpProgrssBar);

			// button - send record
			Assert.AreNotEqual(null, _buttonSendRecord);

#endregion
		}

		private void ShowPlayerAndEnemyNamesScores()
		{
			_textPlayerName.text = "you";
			_textPlayerScore.text =_playerScore.ToString();

			_textEnemyName.text = _enemyName;
			_textEnemyScore.text = _enemyScore.ToString();
		}



	}
}