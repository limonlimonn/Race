using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using HCR.Enums;
using HCR.GlobalWindow.MainMenu;

namespace HCR
{
	/// <summary>
	/// * your summary text *
	/// </summary>

	public class UpgradeItemButton : MonoBehaviour
	{
		public Action<UpgradeItem, Button> OnClick;
		public List<Sprite> itemSprites = new List<Sprite>();
		public List<Sprite> boughtSprites = new List<Sprite>();

		public UpgradeType type;

		public int order;
		public int price;

		public UpgradeItem item;

		[HideInInspector]
		public Button thisButton;

		public RectTransform arrowImage;

		public Image upImage;
		public Text priceLabel;

		// dependences
		private UIManager _uiManager;
		private ChooseCarWindow _chooseCarWindow;



		public void Init(UpgradeItem item)
		{
			_uiManager = Core.Instance.GetService<UIManager>();
			_chooseCarWindow = _uiManager.GetWindow(UIWindowEnum.CHOOSE_CAR) as ChooseCarWindow;

			//
			thisButton = gameObject.GetComponent<Button>();
			thisButton.interactable = true;
			this.item = item;
			this.type = item.UpgradeType;
			this.order = item.upgradeOrder;
            this.price = item.price.gold;

			priceLabel.text = String.Format("{0}", price);

			SetStatus();
		}

		public void Click()
		{
			_chooseCarWindow.Set_TextUpgradePriceToBuy(item.price.gold);

			if (OnClick != null) {
				OnClick(item, thisButton); }
		}

		public void SetStatus()
		{
			switch (type)
			{
				case UpgradeType.Tires:
					upImage.sprite = itemSprites[0];
					if (!item.isButton)
					{
						upImage.sprite = boughtSprites[0];
					}
					if (item.isInteractable == false)
					{
						thisButton.interactable = false;
					}
					break;
				case UpgradeType.Nitro:
					upImage.sprite = itemSprites[1];
					if (!item.isButton)
					{
						upImage.sprite = boughtSprites[1];
					}
					if (item.isInteractable == false)
					{
						thisButton.interactable = false;
					}
					break;
				case UpgradeType.Motor:
					upImage.sprite = itemSprites[2];
					if (!item.isButton)
					{
						upImage.sprite = boughtSprites[2];
					}
					if (item.isInteractable == false)
					{
						thisButton.interactable = false;
					}
					break;
				case UpgradeType.Gearbox:
					upImage.sprite = itemSprites[3];
					if (!item.isButton)
					{
						upImage.sprite = boughtSprites[3];
					}
					if (item.isInteractable == false)
					{
						thisButton.interactable = false;
					}
					break;
				case UpgradeType.Chassis:
					upImage.sprite = itemSprites[4];
					if (!item.isButton)
					{
						upImage.sprite = boughtSprites[4];
					}
					if (item.isInteractable == false)
					{
						thisButton.interactable = false;
					}
					break;

				default:
					break;
			}
		}

		public void SetArrowHeight(float height)
		{
			if (height == 0)
			{
				arrowImage.gameObject.SetActive(false);
			}
			else
			{
				arrowImage.gameObject.SetActive(true);
				arrowImage.sizeDelta = new Vector2(15, height);
			}
		}



	}
}