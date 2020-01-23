using UnityEngine.Assertions;

using HCR.Enums;
using HCR.Interfaces;
using UnityEngine;

namespace HCR.Gameplay.Singleplayer
{
	/// <summary>
	/// Класс - окно, когда разбились и еще остались попытки ("Игра: Одиночная игра")
	/// </summary>

	public class RetryWindow : ABaseRetryWindow, IUIWindow
	{
		// FIELDS

		private GameWindowSingleplayer _gameWindow;



		// I_UI_WINDOW

		public void Init()
		{
			base.InitVariables();

			_gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_SINGLE) as GameWindowSingleplayer;
			Assert.AreNotEqual(null, _gameWindow);
		}

		public void Show()
		{
			WindowViewHandler.Show(_canvasGroup);

			//
			CheckBarrelsCount();
			//_gameWindow.BlockPauseButton();
		}

		public void Hide()
		{
			WindowViewHandler.Hide(_canvasGroup);
		}



		// INTERFACES

		public void TryAgain()
		{
            Debug.Log("Single Retry");
            Hide();

			//if (OnTryAgainClick != null) {
				//OnTryAgainClick(); }
		}



	}
}