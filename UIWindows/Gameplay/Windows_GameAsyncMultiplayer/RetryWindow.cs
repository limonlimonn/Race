using UnityEngine.Assertions;

using HCR.Enums;
using HCR.Interfaces;
using UnityEngine;
using HCR.Event;

namespace HCR.Gameplay.AsyncMultiplayer
{
	/// <summary>
	/// Класс - окно, когда мы разбились в заезде
	/// Префаб для этого класса называется "CrashMenu" !!!
	/// </summary>

	public class RetryWindow : ABaseRetryWindow, IUIWindow
	{
		// FIELDS

		
        


		// I_UI_WINDOW

		public void Init()
		{
			base.InitVariables();

			
		}

		public void Show()
		{
			WindowViewHandler.Show(_canvasGroup);
			//
			CheckBarrelsCount();
			
		}

		public void Hide()
		{
			WindowViewHandler.Hide(_canvasGroup);
		}



		// INTERFACES

		// used on button !
		public void TryAgain()
		{
            Debug.Log("Multi Retry");
			Hide();

            EventManager._init.Game.MenuButton.Invoke_TryAgain();
		}



	}
}