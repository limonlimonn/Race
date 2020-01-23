using UnityEngine.Assertions;
using HCR.Enums;
using HCR.Interfaces;
using UnityEngine;
using System;

namespace HCR.Gameplay.Tutorial
{
    /// <summary>
    /// Класс - окно, когда разбились и еще остались попытки ("Игра: Одиночная игра")
    /// </summary>

    public class RetryWindow : MonoBehaviour, IUIWindow
	{
		// FIELDS

		private GameWindowTutorial _gameWindow;
        public Action OnTryAgainClick;
        public Action OnCoiseAnyCar;

        [SerializeField]
        public CanvasGroup _canvasGroup;
        private UIManager _uiManager;


        // I_UI_WINDOW

        public void Init()
		{
            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);

            _gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_TUTORIAL) as GameWindowTutorial;
			Assert.AreNotEqual(null, _gameWindow);
		}

		public void Show()
		{
			WindowViewHandler.Show(_canvasGroup);

			//
			
			//_gameWindow.BlockPauseButton();
		}

		public void Hide()
		{
			WindowViewHandler.Hide(_canvasGroup);
		}



		// INTERFACES

		public void TryAgain()
		{
			Hide();

			if (OnTryAgainClick != null) {
				OnTryAgainClick(); }
		}

        public void ChoiseEnyCar()
        {
            Hide();

            if (OnCoiseAnyCar != null)
            {
                OnCoiseAnyCar();
            }
        }



    }
}