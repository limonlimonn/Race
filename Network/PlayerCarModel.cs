using System.Collections.Generic;
using System;
using SimpleJSON;

using HCR.Enums;
using UnityEngine;

namespace HCR
{
	public class PlayerCarModel
	{
		public CarTypeEnum carType
		{
			get
			{
				_ct = (CarTypeEnum)Enum.Parse(typeof(CarTypeEnum), type.ToString());
				return _ct;
			}
			set //Only from test
			{
				_ct = value;
				type = (int)value;
			}
		}
		private CarTypeEnum _ct;
        
        //type of car
        public int type;
		public int carLevel = 0;
		public int car_upgrade_level
		{
			get
			{
				return _car_upgrade_level;
			}
			set
			{
				_car_upgrade_level = value;
			}
		}
		int _car_upgrade_level;

		public int current_color;
		public List<int> bought_colors = new List<int>();

		//type, order
		public Dictionary<UpgradeType, int> installedUpgrades = new Dictionary<UpgradeType, int>();

		public List<UpgradeItem> GetPlayerUpgrades()
		{
			var ups = new List<UpgradeItem>();
			if (installedUpgrades != null && installedUpgrades.Count > 0)
			{
				List<UpgradeItem> selectedCarUpgrades = DataModel.Instance.GetUpgradesByCar(carType, carLevel);
				foreach (var up in installedUpgrades)
				{
                    //Debug.Log("Key " + up.Key + " Val " + up.Value);
					ups.Add(selectedCarUpgrades.Find(u => u.UpgradeType == up.Key && u.upgradeOrder == up.Value));
				}

			}

			return ups;
		}

        public virtual string GetPrefabName(int upgradeLevel) //do not send USE type and level and up_level_0
        {
            return String.Format("{0}_{1}_{2}", carType, carLevel, upgradeLevel);
        }

        public Dictionary<string, float> GetPlayerParameters()
		{

			Dictionary<string, float> param = new Dictionary<string, float>();

			var carModel = DataModel.Instance.carsModels.Find(c => c.level == this.carLevel && c.CarType == this.carType);
			if (carModel != null)
			{
				param = carModel.parametersValues;
			}


			return param;
		}

		public override string ToString()
		{
			var res = "CarType: " + carType + " Level: " + carLevel + " UP_Level: " + car_upgrade_level;
			if (installedUpgrades != null && installedUpgrades.Count > 0)
			{
				foreach (var up in installedUpgrades)
				{
					res += "\n" + up.Key + " : " + up.Value;
				}
			}
			else
			{
				res += " EMPTY UPGRADES!!!";
			}

			return res;
		}

		public void Set(JObject obj)
		{
			for (int j = 0; j < obj.Count; j++)
			{
				bought_colors.Add(int.Parse(obj[j]));
			}
		}



	}
}