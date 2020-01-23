using HCR.Enums;
using UnityEngine.Assertions;

using HCR.Interfaces;
using UnityEngine;
using System;

namespace HCR.Gameplay.Tutorial
{
	/// <summary>
	/// Класс - окно "Пауза" ("Tutorial")
	/// </summary>

	public class PauseWindow : MonoBehaviour,  IUIWindow
	{
        // FIELDS
        public Action OnResumeClick;
        public Action OnRestartClick;
        public Action OnCoiseAnyCar;
        public Action OnMenuClick;

        private GameWindowTutorial _gameWindow;
        
        public CanvasGroup _canvasGroup;
        private UIManager _uiManager;

        // I_UI_WINDOW

        public void Init()
		{
            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);

            _gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_TUTORIAL) as GameWindowTutorial;
			Assert.AreNotEqual(null, _gameWindow);

			//
			AssertSerializedFields();
		}

		public void Show()
		{
			WindowViewHandler.Show(_canvasGroup);

			//
			
			SetPauseOn();
		}

		public void Hide()
		{
			WindowViewHandler.Hide(_canvasGroup);

			//
			SetPauseOff();
		}
        public void SetPauseOn()
        {
            Time.timeScale = 0;
        }
        public void SetPauseOff()
        {
            Time.timeScale = 1;
        }

        // INTERFACES

        #region ON_CLICK_BUTTON

        // used on button !

        public void ChoiseEnyCar()
        {
            Hide();

            if (OnCoiseAnyCar != null)
            {
                OnCoiseAnyCar();
            }
        }





        public void OnButtonClick_Resume()
		{
			Debug.Log("CLICK Resume (SINGLE)");

			if (OnResumeClick != null) {
				OnResumeClick(); }
		}

		// used on button !
		public void OnButtonClick_Restart()
		{
			Debug.Log("(Pause Window) >>> Restart()");

			
			Hide();

			if (OnRestartClick != null) {
				OnRestartClick(); }
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