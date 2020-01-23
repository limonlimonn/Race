using HCR.Enums;
using HCR.Interfaces;
using UnityEngine;

namespace HCR
{
	public class CarConstructor : IService
	{
		public void Init()
		{

		}



        public CarBase CreatePlayerCar(PlayerCarModel model, int colorId)
        {
            CarBase playerCar;
            playerCar = CreateCar(model);
            playerCar.SetColorInGame(colorId);
            playerCar.transform.tag = "Player";
            playerCar.Transform.position = new Vector3(0f, 3f, 2f);
            return playerCar;
        }

        public CarBase CreateEnemyCar(PlayerCarModel model, int colorId)
        {
            CarBase _enemyCar = CreateCar(model);
            _enemyCar.Transform.position = new Vector3(0f, 3f, -2f);
            _enemyCar.SetColorInGame(colorId);
            _enemyCar.transform.tag = "Enemy";
            return _enemyCar;
        }

		private CarBase CreateCar(PlayerCarModel model)
		{
			if (model == null)
			{
				model = new PlayerCarModel() { carType = CarTypeEnum.Baggy, carLevel = 0, car_upgrade_level = 0 };
			}
			CarModel carModel = DataModel.Instance.GetCar(model.carType, model.carLevel);
			if (carModel == null)
			{
				carModel = new CarModel() { CarType = model.carType, level = model.carLevel };
			}
			//TODO  get car upgrades from model;

			//Load specific car from resources, cast to CarBase
			//Init physics parameters by car options


			var c = Resources.Load("RaceCarsPrefabs/" + carModel.GetPrefabName(model.car_upgrade_level));
			if (c != null)
			{
				var carGO = GameObject.Instantiate(c) as GameObject;
				var car = carGO.GetComponent<CarBase>();
				car.model = model;
				car.Init();
				return car;
			}
			else
			{
				c = Resources.Load("RaceCarsPrefabs/" + carModel.GetPrefabName(model.car_upgrade_level - 1));
				var carGO = GameObject.Instantiate(c) as GameObject;
				var car =  carGO.GetComponent<CarBase>();
				car.model = model;
				car.Init();
				return car;
			}
		}

		public void UpgradeCar(CarBase car/*, car options */)
		{
			car.ApplyCarPart();
		}

		public void ChangeCarSkin(CarBase car/*, car options */)
		{
			car.ApplyCarSkin();
		}



	}
}