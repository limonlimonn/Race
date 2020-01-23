using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using HCR.Enums;

namespace HCR
{
	public class CarItem : MonoBehaviour
	{
		public Action<CarModel> OnCarClick;

		public Text price;
		public Image carImage;
		public Image carStatus;

		public CarTypeEnum type;
		public int level;

		public bool opened = false;

		public List<Sprite> carTypeImages = new List<Sprite>();

		public Sprite canBuySprite;
		public Sprite boughtSprite;
		public Sprite notOpenedSprite;

		public CarModel switchedCar;

		public Button showInfo;

		private DescriptionPopup popup;

		public void Init(CarModel car)
		{
			popup = GameObject.FindObjectOfType<DescriptionPopup>();
			showInfo.onClick.AddListener(ShowPopup);
			if (!opened)
			{
				carStatus.sprite = notOpenedSprite;
			}
			else
			{
				carStatus.sprite = canBuySprite;
			}


			var ca = Core.Instance.GetService<PlayerManager>().playerCars.Find(c => c.carType == car.CarType && c.carLevel == car.level);

			if (ca != null)
			{
				carStatus.sprite = boughtSprite;
			}


			switch (car.CarType)
			{
				
				case CarTypeEnum.Rally:
                    
                    type = CarTypeEnum.Rally;
					carImage.sprite = carTypeImages[car.level+8];
					break;
				case CarTypeEnum.Jeep:
					type = CarTypeEnum.Jeep;
					carImage.sprite = carTypeImages[car.level+4];
					break;
                case CarTypeEnum.Baggy:
                    
                    carImage.sprite = carTypeImages[car.level];
                    type = CarTypeEnum.Baggy;
                    break;
            }

			level = car.level;
			price.text = String.Format("{0}", car.price.gold);
			switchedCar = car;

		}

		public void UpdateStatus()
		{
			carStatus.sprite = canBuySprite;
		}


		void ShowPopup()
		{
			popup.Show(switchedCar.name, switchedCar.description);
		}

		public void ChooseForWatch()
		{
			if (OnCarClick != null)
			{
                OnCarClick(switchedCar);
			}
		}



	}
}