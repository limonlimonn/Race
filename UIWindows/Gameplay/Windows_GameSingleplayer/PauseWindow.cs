using HCR.Enums;
using UnityEngine.Assertions;

using HCR.Interfaces;
using UnityEngine;
using HCR.Event;

namespace HCR.Gameplay.Singleplayer
{
	/// <summary>
	/// Класс - окно "Пауза" ("Игра: Синглплеер")
	/// </summary>

	public class PauseWindow : ABasePauseWindow, IUIWindow
	{
		// FIELDS

		// I_UI_WINDOW

		public void Init()
		{
			base.InitDependences();

			//_gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_SINGLE) as GameWindowSingleplayer;
			//Assert.AreNotEqual(null, _gameWindow);

			//
			AssertSerializedFields();
		}

		public void Show()
		{
			WindowViewHandler.Show(_canvasGroup);

			//
			// HideGameWindowTimer(_gameWindow);
			SetPauseOn();
		}

		public void Hide()
		{
			WindowViewHandler.Hide(_canvasGroup);

			//
			SetPauseOff();
		}



		// INTERFACES

		#region ON_CLICK_BUTTON

		// used on button !
		public void OnButtonClick_Resume()
		{
			Debug.Log("CLICK Resume (SINGLE)");

            EventManager._init.Game.MenuButton.Invoke_Resume();
		}

		// used on button !
		public void OnButtonClick_Restart()
		{
			Debug.Log("(Pause Window) >>> Restart()");
			Hide();

            EventManager._init.Game.MenuButton.Invoke_Restart();
            // ShowGameWindowTimer(_gameWindow);

        }

        // used on button !
        public void OnButtonClick_Menu()
		{
			if (OnMenuClick != null) {
				OnMenuClick(); }
		}

		#endregion



		// METHODS

		private void AssertSerializedFields()
		{
			Assert.AreNotEqual(null, _canvasGroup);
		}



	}
}