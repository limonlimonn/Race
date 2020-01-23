using System;
using UnityEngine.Assertions;

using HCR.Enums;
using HCR.Interfaces;

namespace HCR.Gameplay.AsyncMultiplayer
{
	/// <summary>
	/// Класс - окно, когда разбились три раза подряд и больше нет попыток
	/// </summary>

	public class CrashFinishWindow : ABaseFinishRecordRaceWithSmiles, IUIWindow
	{
		// ACTIONS

		public Action OnSendRecord;

		// FIELDS

		private GameWindowAsyncMultiplayer _gameWindow;

		// dependences
		private SmileSwapHandler _smileSwapHandler;



		// I_UI_WINDOW

		public void Init()
		{
			base.InitVariables();

			_gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_ASYNC) as GameWindowAsyncMultiplayer;
			Assert.AreNotEqual(null, _gameWindow);

			_smileSwapHandler = new SmileSwapHandler();

			//
			AssertSerializedFields();
		}

		public void Show()
		{
			WindowViewHandler.Show(_canvasGroup);

			//
			ShowDefaultSmiles();
			ShowRaceTime(_gameWindow);
			//_gameWindow.BlockPauseButton();
		}

		public void Hide()
		{
			WindowViewHandler.Hide(_canvasGroup);
		}



		// INTERFACES

		public void SetValue_GameData(GameData gameData)
		{
			_gameData = gameData;
		}

		// used on button !
		public void SendRecord()
		{
			_idSmilesToSend = _smileSwapHandler.GetSmiles();
			_gameData.smiles = _idSmilesToSend;

			Hide();

			if (OnSendRecord != null) {
				OnSendRecord(); }
		}

		// used on button !
		public void OnClickButtonSmileSmall(int id)
		{
			_idSmilesToSend = _smileSwapHandler.AddSmilesToList(id);
			ShowSmilesBig();
		}



		// METHODS

		protected override void ShowDefaultSmiles()
		{
			_idSmilesToSend = _smileSwapHandler.GetDefaultSmiles();
			ShowSmilesBig();
		}

		private void AssertSerializedFields()
		{
			Assert.AreNotEqual(null, _canvasGroup);
			Assert.AreNotEqual(null, _textRaceTime);

			Assert.AreNotEqual(null, _imageSmileBig_0);
			Assert.AreNotEqual(null, _imageSmileBig_1);
			Assert.AreNotEqual(null, _imageSmileBig_2);

			Assert.AreNotEqual(null, _smilesBigImages);
			Assert.IsTrue(_smilesBigImages.Count > 0);

			Assert.AreNotEqual(null, _idSmilesToSend);
		}



	}
}