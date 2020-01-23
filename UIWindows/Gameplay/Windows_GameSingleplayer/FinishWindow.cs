using UnityEngine.Assertions;

using HCR.Enums;
using HCR.Interfaces;
using UnityEngine;

namespace HCR.Gameplay.Singleplayer
{
	/// <summary>
	/// Класс - окно при удачном финише ("Игра: Одиночная игра")
	/// с начислением Silver, Exp
	/// </summary>

	public class FinishWindow : ABaseFinishWindow, IUIWindow
	{
		// dependences
		private GameManager _gameManager;

		private GameWindowSingleplayer _gameWindow;



		// I_UI_WINDOW

		public void Init()
		{
			//base.InitVariables();

			//_gameManager = Core.Instance.GetService<GameManager>();
			//Assert.AreNotEqual(null, _gameManager);

			//_gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_SINGLE) as GameWindowSingleplayer;
			//Assert.AreNotEqual(null, _gameWindow);

			//
			//AssertVariables();
		}

		public void Show()
		{
			WindowViewHandler.Show(_canvaseGroup);

			//
            

			CheckButtonsStampX();
			ShowPanelName();
			//ShowStars(_gameWindow);
			ShowWinAttemptsX(_gameWindow);
			ShowStamps();
			//ShowAllUIAnimations();

			//_gameWindow.BlockPauseButton();
		}

		public void Hide()
		{
			WindowViewHandler.Hide(_canvaseGroup);
		}



		// INTERFACES

		// used on button !
		public void OnClickButton_StampX(int multiplier)
		{
			//
		}

		#region RECORD

		// used on button !
		public void OnClickButton_Record()
		{
			_uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
			_networkManager.SaveSingleplayer(Callback_RecordOK, Callback_RecordError);
		}

		private void Callback_RecordOK()
		{
			Hide();
			_uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
			_gameManager.Finish();
		}

		private void Callback_RecordError()
		{
			_uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
			Debug.Log("Error!");
		}

		#endregion



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
			//Assert.AreNotEqual(null, _textTrick90);
			Assert.AreNotEqual(null, _textExpTotal);

			Assert.AreNotEqual(null, _imageExpProgrssBar);

			// button - send record
			Assert.AreNotEqual(null, _buttonSendRecord);

			#endregion
		}



	}
}