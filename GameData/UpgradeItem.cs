using UnityEngine;
using System.Collections.Generic;
using System;

using HCR.Enums;

namespace HCR
{
	public class UpgradeItem
	{
		public int carType;
		private CarTypeEnum _ct;

		public CarTypeEnum CarType
		{
			get
			{
				_ct = (CarTypeEnum)Enum.Parse(typeof(CarTypeEnum), carType.ToString());
				return _ct;
			}
			set //Only from test
			{
				_ct = value;
				upgradeType = (int)value;
				Debug.LogError(value);
			}
		} //send as int

		public int carLevel;
		public int upgradeLevel;
		public Price price;

		public bool isInteractable;
		public bool isButton;


		private UpgradeType _ut;

		public UpgradeType UpgradeType
		{
			get
			{
				_ut = (UpgradeType)Enum.Parse(typeof(UpgradeType), upgradeType.ToString());
				return _ut;
			}
			set //Only from test
			{
				_ut = value;
				upgradeType = (int)value;
				Debug.LogError(value);
			}
		}

		public int upgradeType;
		public int upgradeOrder;

		public Dictionary<string, float> upgradeValues = new Dictionary<string, float>();



	}
}