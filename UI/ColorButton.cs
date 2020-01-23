using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;

namespace HCR
{
	public class ColorButton : MonoBehaviour
	{
		public Action<int> OnPress;

		public int ID;
		public int price;
		public string hex;

		public Button button;

		public Animator anim;

		public void Init(int id, int price, string hex)
		{
			ID = id;
			this.price = price;

		}

		public void Buy()
		{
			if (OnPress != null)
			{
				OnPress(ID);
			}
		}



	}
}