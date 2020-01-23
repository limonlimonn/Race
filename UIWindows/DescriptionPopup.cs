using UnityEngine;
using UnityEngine.UI;

namespace HCR
{
	public class DescriptionPopup : MonoBehaviour
	{
		public Text carName;
		public Text description;

		public Button closePopupBtn;

		[SerializeField]
		private CanvasGroup _group;

		public void Show(string name, string description)
		{
			closePopupBtn.onClick.AddListener(Hide);
			carName.text = name;
			this.description.text = description;

			_group.alpha = 1;
			_group.blocksRaycasts = true;
		}

		public void Hide()
		{
			_group.alpha = 0;
			_group.blocksRaycasts = false;
		}



	}
}