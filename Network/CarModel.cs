using UnityEngine;
using System;
using System.Collections.Generic;

using HCR.Enums;

namespace HCR
{
	public class CarModel
	{
		public int type;
        public int TutorialColor;
		public CarTypeEnum CarType
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
				//Debug.LogError(value);
			}
		}
		private CarTypeEnum _ct;
		public int level;
		public Price price;
		public string name;
		public string description;

		public bool isOpened = false;

		//public byte upgradeLevel;
		//public string name;


		public Dictionary<int, string> upgradeLevels;

		public Dictionary<string, float> parametersValues;

		public string GetCarName(int upgradeLevel)
		{
			return upgradeLevels[upgradeLevel];
		}

		public virtual string GetPrefabName(int upgradeLevel) //do not send USE type and level and up_level_0
		{
			return String.Format("{0}_{1}_{2}", CarType, level, upgradeLevel);
		}

		//ToDo wait for formulas
		public virtual Dictionary<UIIndicatorsEnum, float> CalculateIndicators()
		{
			return CalculateIndicators(new List<UpgradeItem>());
		}

		public virtual Dictionary<UIIndicatorsEnum, float> CalculateIndicators(List<UpgradeItem> ups)
		{
			if (parametersValues != null && parametersValues.Count > 0)
			{
				Dictionary<string, float> paramVals = new Dictionary<string, float>(parametersValues);

				foreach (var item in ups)
				{
					if (item.upgradeValues != null)
					{
						foreach (var value in item.upgradeValues)
						{
							if (!paramVals.ContainsKey(value.Key))
								paramVals.Add(value.Key, Mathf.Max(parametersValues[value.Key], value.Value));

							paramVals[value.Key] = Mathf.Max(parametersValues[value.Key], value.Value);

							//  Debug.LogError(value.Key + " " + paramVals[value.Key]);
						}
					}
				}



				Dictionary<UIIndicatorsEnum, float> upgrades = new Dictionary<UIIndicatorsEnum, float>()
				{


					//{UIIndicatorsEnum.MaxSpeed, paramVals.ContainsKey("MaxSpeedForward") ? paramVals["MaxSpeedForward"] : 17 },
					//{UIIndicatorsEnum.Accelerator, ((paramVals.ContainsKey("MaxDriveForce")? paramVals["MaxDriveForce"] : 0) / (paramVals.ContainsKey("Mass")? paramVals["Mass"] : 1) * (paramVals.ContainsKey("ForceCurveShape")? paramVals["ForceCurveShape"] : 0)) / 7 },
					//{UIIndicatorsEnum.Grip, ((paramVals.ContainsKey("MaxBrakeForce") ? paramVals["MaxBrakeForce"] : 0 )/ 10000 + (paramVals.ContainsKey("TireFriction")? paramVals["TireFriction"] : 0)) / 4 },
					//{UIIndicatorsEnum.Handling, (paramVals.ContainsKey("Spring") ? paramVals["Spring"] : 0) *( paramVals.ContainsKey("Damper") ? paramVals["Damper"]:0) /(paramVals.ContainsKey("Mass")? paramVals["Mass"]:0) /(paramVals.ContainsKey("SuspensionDistance") ? paramVals["SuspensionDistance"] :0)/ 40000 },
					//{UIIndicatorsEnum.Nitro, paramVals.ContainsKey("NitroDuration") ? paramVals["NitroDuration"] : 0 }

					{UIIndicatorsEnum.MaxSpeed, Mathf.Round(paramVals["MaxSpeedForward"]*2.24f) },
					{UIIndicatorsEnum.Accelerator, paramVals["MaxDriveForce"]/paramVals["Mass"] * paramVals["ForceCurveShape"] },
					{UIIndicatorsEnum.Grip, paramVals["TireFriction"] },
					{UIIndicatorsEnum.Handling, paramVals["Spring"]/paramVals["Mass"] },
					{UIIndicatorsEnum.Nitro, paramVals["NitroPower"]*paramVals["NitroDuration"]/paramVals["Mass"] }
				};
				return upgrades;
			}
			else
			{
				Dictionary<UIIndicatorsEnum, float> upgrades = new Dictionary<UIIndicatorsEnum, float>()
				{
					{UIIndicatorsEnum.MaxSpeed, 17 },
					{UIIndicatorsEnum.Accelerator, 0},
					{UIIndicatorsEnum.Grip, 0 },
					{UIIndicatorsEnum.Handling, 0 },
					{UIIndicatorsEnum.Nitro,0 }
				};
				return upgrades;
			}
		}



	}
}