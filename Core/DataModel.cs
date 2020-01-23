using System.Collections.Generic;

using HCR.Enums;

namespace HCR
{
	public class DataModel
	{
		public static DataModel Instance {
			get {
				if (_instance == null) {
					_instance = new DataModel(); }
				return _instance;
			}
		}

		private static DataModel _instance;

		public List<CarModel> carsModels = new List<CarModel>();

		public List<UpgradeItem> upgrades = new List<UpgradeItem>();

		public List<CarModel> openedCars = new List<CarModel>();

		public DataModel()
		{

		}

		public void Test()
		{
			//Debug.LogError("TEST!!!!!");
			#region Cars
			List<CarModel> tempCarModels = new List<CarModel>()
			{
				#region Buggy
				new CarModel()
				{
					CarType = CarTypeEnum.Baggy,
					level = 0,
					price = new Price()
					{
						gold = 0, jewels = 0
					},
					upgradeLevels = new Dictionary<int, string>()
					{
						{0, "Buggy_0_up_0"},
						{1, "Buggy_0_up_1"},
						{2, "Buggy_0_up_2"}
					},
					parametersValues = new Dictionary<string, float>()
				},
				#endregion
				#region Jeep
				new CarModel()
				{
					CarType = CarTypeEnum.Jeep,
					level = 0,
					price = new Price()
					{
						gold = 0, jewels = 0
					},
					upgradeLevels = new Dictionary<int, string>()
					{
						{0, "Jeep0_up_0"},
						{1, "Jeep0_up_1"},
						{2, "Jeep0_up_2"}
					},
					parametersValues = new Dictionary<string, float>()
				},
				#endregion
				#region Rally
				new CarModel()
				{
					CarType = CarTypeEnum.Rally,
					level = 0,
					price = new Price()
					{
						gold = 0, jewels = 0
					},
					upgradeLevels = new Dictionary<int, string>()
					{
						{0, "Rally0_up_0"},
						{1, "Rally0_up_1"},
						{2, "Rally0_up_2"}
					},
					parametersValues = new Dictionary<string, float>()
				}
				#endregion
			};

			SetCars(tempCarModels);
			#endregion

		}

        public void DestroySelf()
        {
            _instance = null;
        }

        public void SetCars(List<CarModel> carsModels)
		{
			this.carsModels = carsModels;
		}

		public CarModel GetCar(CarTypeEnum branch, int carLevel)
		{
			CarModel result = carsModels.Find(car => car.CarType == branch && car.level == carLevel);

			return result;
		}

		public void SetUpgrades(List<UpgradeItem> data)
		{
			upgrades = data;
		}

		public List<UpgradeItem> GetUpgradesByCar(CarTypeEnum carType, int carLevel)
		{
			List<UpgradeItem> res = new List<UpgradeItem>();
			foreach (var up in upgrades)
			{
				if (up.CarType == carType && up.carLevel == carLevel)
				{
					res.Add(up);
				}
			}

			return res;
		}


	}


}
