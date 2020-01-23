using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;

using HCR.Enums;
using HCR.Interfaces;
using UnityEngine.UI;
using DG.Tweening;

namespace HCR.GlobalWindow.MainMenu
{
	/// <summary>
	/// Класс - окно "Покупка апгрейдов"
	/// </summary>

	public class TreeWindow : MonoBehaviour, IUIWindow
	{
		// ACTIONS

		public Action<CarModel> OnShowPanel;

		// FIELDS

		#region VARIABLES

		// ----------------------------------------
		//
		public CanvasGroup canvaseGroup;

		// ----------------------------------------
		//
		public GameObject carItemPrefab;

		public SpriteRenderer bg;

		public Transform rallyParent;
		public Transform buggyParent;
		public Transform jeepParent;

		public List<CarItem> carItems = new List<CarItem>();

		public GameObject fade;

		//
		private Transform _carParent;
		private GameObject currentCar;

		// ----------------------------------------
		//
		[Header("SCROLL SIZE")]
		[SerializeField] private RectTransform _objScrollContent;
		[SerializeField] private Transform _objRally;
		[SerializeField] private Transform _prefabCarItem;
        [SerializeField] private Scrollbar ScrollTree;

        #endregion

        // dependences
        private PlayerManager _playerManager;

		private UIManager _uiManager;
		private MainScreenStateManager _mainScreenStateManager;
		private ChooseCarWindow _chooseCarWindow;
		private Header _header;
        private float Size;
        [SerializeField]
        private Banner Banner;

		// I_UI_WINDOW

		public void Init()
		{
			_playerManager = Core.Instance.GetService<PlayerManager>();
			_uiManager = Core.Instance.GetService<UIManager>();
			_mainScreenStateManager = _uiManager.Get_MainScreenStateManager();
			_chooseCarWindow = _uiManager.GetWindow(UIWindowEnum.CHOOSE_CAR) as ChooseCarWindow;
			_header = _uiManager.GetWindow(UIWindowEnum.HEADER) as Header;
            //Size = ScrollTree.size;


            AssertVariables();
		}

		public void Show()
		{
            Banner.ShowBaner();
            //canvaseGroup.alpha = 1;
            _uiManager.ShowCanvas(canvaseGroup);
            canvaseGroup.blocksRaycasts = true;
            ScrollTree.value = 0;
            //ScrollTree.size = Size;
            //
            bg.gameObject.SetActive(true);
            

            bg.sprite = _uiManager.ChangeBG(UIWindowEnum.TREE_UPGRADE);
			_header.UpdateUI();

			if (rallyParent.childCount == 0 || buggyParent.childCount == 0 || jeepParent.childCount == 0)
			{
				ClearTree();
				SwitchTypeAndInstantiateCars();
			}

            /*
			_carParent = GameObject.Find("CarParent").transform;

			if (_chooseCarWindow.car != null)
			{
				var ca = _playerManager.playerCars.Find(c => c.carType == _chooseCarWindow.car.CarType && c.carLevel == _chooseCarWindow.car.level);
				if (ca != null)
				{
					fade.SetActive(true);
					SetCarToBG(_chooseCarWindow.car, ca.car_upgrade_level);
				}
				else
				{
					if (_playerManager.currentCar != null)
					{
						fade.SetActive(true);
						ca = _playerManager.playerCars.Find(c => c.carType == _playerManager.currentCar.CarType && c.carLevel == _playerManager.currentCar.level);
						SetCarToBG(_playerManager.currentCar, ca.car_upgrade_level);
					}
				}

			}
			else
			{
				if (_playerManager.currentCar != null)
				{
					fade.SetActive(true);
					var ca = _playerManager.playerCars.Find(c => c.carType == _playerManager.currentCar.CarType && c.carLevel == _playerManager.currentCar.level);
					SetCarToBG(_playerManager.currentCar, ca.car_upgrade_level);
				}
			}
            */

			//
			AutoAdjustScrollSize();
		}

		public void Hide()
		{
            Banner.HideBaner();
            bg.gameObject.SetActive(false);
            //canvaseGroup.alpha = 0;
			canvaseGroup.blocksRaycasts = false;
            _uiManager.HideCanvas(canvaseGroup);

            //
            Destroy(currentCar);
			currentCar = null;
		}



		// INTERFACES

		public void GoToGarage()
		{
			_mainScreenStateManager.SwitchState(
				MainScreenStatesEnum.CHOOSE_CAR);
		}



		// METHODS

		private void AssertVariables()
		{
			Assert.AreNotEqual(null, _objScrollContent);
			Assert.AreNotEqual(null, _objRally);
			Assert.AreNotEqual(null, _prefabCarItem);
		}

		private void SetCarToBG(CarModel car, int up_lvl)
		{
            return;
			Quaternion rot = Quaternion.Euler(0.6f, -109f, -10f);

			var go = Resources.Load("RaceCarsPrefabs/" + car.GetPrefabName(up_lvl)) as GameObject;
			if (go != null)
			{
				currentCar = GameObject.Instantiate(go, _carParent.position, rot) as GameObject;
				currentCar.transform.SetParent(_carParent);
				currentCar.GetComponent<CarBase>().SetColorInGame(_playerManager.selectedCar.current_color);
				currentCar.transform.localPosition = new Vector3(3.32f, -1.2f, 0.18f);

				currentCar.GetComponent<Rigidbody>().isKinematic = true;
			}
			else
			{
				go = Resources.Load("RaceCarsPrefabs/" + car.GetPrefabName(up_lvl - 1)) as GameObject;
				currentCar = GameObject.Instantiate(go, _carParent.position, rot) as GameObject;
				currentCar.transform.SetParent(_carParent);
				currentCar.transform.localPosition = new Vector3(3.32f, -1.2f, 0.18f);
                

				currentCar.GetComponent<Rigidbody>().isKinematic = true;
			}

		}

		private void ClearTree()
		{
			foreach (var item in carItems)
			{
                item.OnCarClick -= ShowPanelWithCar;
				Destroy(item.gameObject);
			}

			carItems.Clear();
		}

		public void SwitchTypeAndInstantiateCars()
		{
            //Debug.Log(DataModel.Instance.carsModels.Count);
			for (int i = 0; i < DataModel.Instance.carsModels.Count; i++)// Заглушка 9 машин 
			{
                //Debug.Log(i);
                switch (DataModel.Instance.carsModels[i].CarType)
				{

					case CarTypeEnum.Baggy:
						var go = Instantiate(carItemPrefab, buggyParent.position, Quaternion.identity) as GameObject;
						go.transform.SetParent(buggyParent);
						go.transform.localScale = Vector2.one;
						var ci = go.GetComponent<CarItem>();
						var c = DataModel.Instance.openedCars.Find(q => q.level == DataModel.Instance.carsModels[i].level && q.CarType == DataModel.Instance.carsModels[i].CarType);
                        //Debug.Log("c Baggy = " + c);
						if (c != null)
						{
                            //Debug.Log(c.CarType + "  " + c.level);
                            DataModel.Instance.carsModels[i].isOpened = true;
							ci.opened = true;
						}
                        //Debug.Log(DataModel.Instance.carsModels[i].isOpened);
                        ci.Init(DataModel.Instance.carsModels[i]);
						carItems.Add(ci);

						break;

					case CarTypeEnum.Rally:
						go = Instantiate(carItemPrefab, rallyParent.position, Quaternion.identity) as GameObject;
						go.transform.SetParent(rallyParent);
						go.transform.localScale = Vector2.one;
						ci = go.GetComponent<CarItem>();
						c = DataModel.Instance.openedCars.Find(q => q.level == DataModel.Instance.carsModels[i].level && q.CarType == DataModel.Instance.carsModels[i].CarType);
                       // Debug.Log("c Rally = " + c);
                        if (c != null)
						{
                           // Debug.Log(c.CarType + "  " + c.level);
                            DataModel.Instance.carsModels[i].isOpened = true;
							ci.opened = true;
						}
                        //Debug.Log(DataModel.Instance.carsModels[i].isOpened);
                        ci.Init(DataModel.Instance.carsModels[i]);
						carItems.Add(ci);

						break;

					case CarTypeEnum.Jeep:
						go = Instantiate(carItemPrefab, jeepParent.position, Quaternion.identity) as GameObject;
						go.transform.SetParent(jeepParent);
						go.transform.localScale = Vector2.one;
						ci = go.GetComponent<CarItem>();
						c = DataModel.Instance.openedCars.Find(q => q.level == DataModel.Instance.carsModels[i].level && q.CarType == CarTypeEnum.Jeep);
                        //Debug.Log("c Jeep = " + c);
                        if (c != null)
						{
                            //Debug.Log(c.CarType +"  "+ c.level);
                            DataModel.Instance.carsModels[i].isOpened = true;
							ci.opened = true;
							ci.Init(c);
						}
						else
						{
							ci.Init(DataModel.Instance.carsModels[i]);
						}
                       // Debug.Log(DataModel.Instance.carsModels[i].isOpened);

                        carItems.Add(ci);
						break;

				}

				carItems[i].OnCarClick += ShowPanelWithCar;
			}
		}

		public void ShowPanelWithCar(CarModel car)
		{
			if (OnShowPanel != null)
			{
				OnShowPanel(car);
				GoToGarage();
				_chooseCarWindow.Set_TextCarPriceToBuy(car.price.gold);
			}
		}

		#region AUTO_SCROLL_SIZE

		private void AutoAdjustScrollSize()
		{
			// Алгоритм: умножаем количество иконок машин на высоту одной иконки,
			// прибавляем к этому небольшой процент сверху для красоты

			int numberOfElements = _objRally.transform.childCount;
			if (numberOfElements <= 0)
			{
				#region DEBUG
#if UNITY_EDITOR
				Debug.Log("[ERROR] rally prefab has no children !");
#endif
				#endregion
				return;
			}

			Transform bg = _prefabCarItem.GetChild(0);
			if (bg.name != "bg")
			{
				#region DEBUG
#if UNITY_EDITOR
				Debug.Log("[ERROR] can't find [bg] child element from prefab CarItem !");
#endif
				#endregion
			}

			RectTransform rtBG = bg.GetComponent<RectTransform>();
			Assert.AreNotEqual(null, rtBG);

			float summHeight = rtBG.rect.height * numberOfElements;

			// нужно добавить + 15% к общей высоте столбца
			const float coeff = 0.15f;
			float additionalHeight = rtBG.rect.height * coeff;
			float finalHeight = summHeight + additionalHeight;

			_objScrollContent.sizeDelta = new Vector2(
				_objScrollContent.rect.width,
				finalHeight);
		}

		#endregion



	}
}