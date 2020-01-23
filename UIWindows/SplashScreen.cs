using UnityEngine;
using UnityEngine.Assertions;

using HCR.Enums;
using HCR.GlobalWindow.MainMenu;
using HCR.Interfaces;
using UnityEngine.UI;

namespace HCR.Loading
{
	/// <summary>
	/// Класс - окно "ЗАГРУЗКА" (при коннекте к серверу)
	/// </summary>

	public class SplashScreen : MonoBehaviour, IUIWindow
	{
		// FIELDS

		//
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private GameObject _loadingPanel;
        [SerializeField] private Text _loadingText;
        public string loadingText
        {
            get
            {
                return _loadingText.text;
            }
            set
            {
                _loadingText.text = value;
            }
        }

        // dependences
        private UIManager _uiManger;
		private Header _header;



		// I_UI_WINDOW

		public void Init()
		{
			_uiManger = Core.Instance.GetService<UIManager>();
			Assert.AreNotEqual(null, _uiManger);

			_header = _uiManger.GetWindow(UIWindowEnum.HEADER) as Header;
			Assert.AreNotEqual(null, _header);

			//
			AssertVariables();
		}

		public void Show()
		{
			_canvasGroup.alpha = 1;
			_canvasGroup.blocksRaycasts = true;

			//
			//_header.Hide();
			ShowLoadingPanel();
		}

		public void Hide()
		{
			_canvasGroup.alpha = 0;
			_canvasGroup.blocksRaycasts = false;

			//
			//_header.Show();
			HideLoadingPanel();
		}

        


		// INTERFACES

		public void ShowLoadingPanel()
		{
			_loadingPanel.SetActive(true);
		}

		public void HideLoadingPanel()
		{
            _loadingPanel.SetActive(false);
		}



		// METHODS

		private void AssertVariables()
		{
			Assert.AreNotEqual(null, _canvasGroup);
			Assert.AreNotEqual(null, _loadingPanel);
		}



	}
}