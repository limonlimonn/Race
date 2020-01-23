using System;
using System.Collections.Generic;
using HCR.Interfaces;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HCR.Gameplay
{
	public abstract class ABaseRetryWindow : MonoBehaviour
	{
		// ACTION

		
       


        // FIELDS

        #region VARIALBLES

        // -------------------------------------------------
        //
        [SerializeField] protected CanvasGroup _canvasGroup;

		// -------------------------------------------------
		//
		[Header("BARREL_0")]
		[SerializeField] protected Image _barrel_0_ok;
		[SerializeField] protected Image _barrel_0_x;

		[Header("BARREL_1")]
		[SerializeField] protected Image _barrel_1_ok;
		[SerializeField] protected Image _barrel_1_x;

		[Header("BARREL_2")]
		[SerializeField] protected Image _barrel_2_ok;
		[SerializeField] protected Image _barrel_2_x;

		// -------------------------------------------------
		//
		protected List<Image> _barrelsFull;
		protected List<Image> _barrelsEmpty;

		// -------------------------------------------------
		//
		protected int _triesCount;

		#endregion

		// dependences
		protected UIManager _uiManager;



		// METHODS

		protected void InitVariables()
		{
			_uiManager = Core.Instance.GetService<UIManager>();
			Assert.AreNotEqual(null, _uiManager);

			//
			_barrelsFull = new List<Image>();

			_barrelsFull.Add(_barrel_0_ok);
			_barrelsFull.Add(_barrel_1_ok);
			_barrelsFull.Add(_barrel_2_ok);

			//
			_barrelsEmpty = new List<Image>();

			_barrelsEmpty.Add(_barrel_0_x);
			_barrelsEmpty.Add(_barrel_1_x);
			_barrelsEmpty.Add(_barrel_2_x);
		}

		protected void CheckBarrelsCount()
		{
            _triesCount = GameDataManager._init.GameData.GetTryes;

			switch (_triesCount)
			{
				case 2:
					HideBarrel(0);
					ShowBarrel(1);
					ShowBarrel(2);
					break;

				case 1:
					HideBarrel(0);
					HideBarrel(1);
					ShowBarrel(2);
					break;

				case 0:
					HideBarrel(0);
					HideBarrel(1);
					HideBarrel(2);
					break;

				default:
					#region DEBUG
#if UNITY_EDITOR
					Debug.Log("[ERROR] _triesCount <= 0 or > 3 !!!");
#endif
					#endregion
					break;
			}
		}

		protected void ShowBarrel(int id)
		{
			_barrelsFull[id].gameObject.SetActive(true);
			_barrelsEmpty[id].gameObject.SetActive(false);
		}

		protected void HideBarrel(int id)
		{
			_barrelsFull[id].gameObject.SetActive(false);
			_barrelsEmpty[id].gameObject.SetActive(true);
		}




	}
}