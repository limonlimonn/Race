using System.Collections.Generic;
using HCR.Enums;

namespace HCR
{
	public class UpgradesData
	{


		//CarType send as byte, UpgradeType send as byte, byte - upgradeLevel
		public Dictionary<CarTypeEnum, Dictionary<byte, Dictionary<UpgradeType, UpgradeItem>>> Upgrades;


	}
}