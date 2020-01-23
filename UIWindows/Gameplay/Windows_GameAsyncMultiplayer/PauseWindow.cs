using HCR.Enums;
using UnityEngine.Assertions;

using HCR.Interfaces;
using UnityEngine;
using HCR.Event;

namespace HCR.Gameplay.AsyncMultiplayer
{
	/// <summary>
	/// Класс - окно "Пауза" ("Игра: Асинхронный мультиплеер")
	/// </summary>

	public class PauseWindow : ABasePauseWindow, IUIWindow
	{
		// FIELDS

		private GameWindowAsyncMultiplayer _gameWindow;



		// I_UI_WINDOW

		public void Init()
		{
			base.InitDependences();
            
			//
			AssertSerializedFields();
		}

		public void Show()
		{
			WindowViewHandler.Show(_canvasGroup);
			SetPauseOn();
		}

		public void Hide()
		{
			WindowViewHandler.Hide(_canvasGroup);
			SetPauseOff();
		}

		// INTERFACES

		#region ON_CLICK_BUTTON

		
		public void OnButtonClick_Resume()
		{
			Debug.Log("CLICK Resume (ASYNC)");

            EventManager._init.Game.MenuButton.Invoke_Resume();
        }

		
		public void OnButtonClick_Restart()
		{
			
			Hide();

            EventManager._init.Game.MenuButton.Invoke_Restart();
            
        }

		
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