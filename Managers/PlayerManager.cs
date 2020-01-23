using UnityEngine;
using System.Collections.Generic;
using System;
using HCR.Interfaces;

namespace HCR
{
	public class PlayerManager : IService
	{
		public Action OnUserLoaded;

		public string PlayerId { get; private set; }

		public string PlayerDisplayName { get; private set; }

        //public int SelectedCarId { get; private set; }
        #region USER STATISTIC
        public float AverageStars;
        public int MaxGold;
        public int TotalGold;
        public int MaxJewels;
        public int TotalJewels;
        public int MaxXP;
        public float AverageXP;
        public float MaxBalance;
        public float TotalBalance;
        public int MaxFlip;
        public int TotalFlip;
        public float MaxInAir;
        public float TotalInAir;
        public float MaxBlunt;
        public int TotalBlunt;
        public int Win;
        public int Draw;
        public int Losing;
        public int InGame;



        #endregion

        public int level = 0;
        public int IsDone = -1;
		public int xp = 0;
        public string UserMale;
        public string UserOld;
		public int jewels = 0;

		public int Jewels
        {
			get
			{
				return jewels;
			}
			set
			{
                jewels = value;
			}
		}

		public int accountType = 0;
        public int accountTime = 0;
        public int now_stamp = 0;
        public int stamp_now_user = 0;
		public int gold = 0;
        public bool IsOnRandom;
        public int stampRandom = 0;
      
        public int isTutorial;
        public int tutorialStep;
        public int TutorialIns {
            get
            {
                return isTutorial;
            }
            set
            {
                isTutorial = value;
            }
        } 
        
		public Color32 selectedColor;

		public PlayerCarModel selectedCar;
        public CarModel currentCar;
        

		public CarBase PlayerCar;
        public List<int> TrackOpen = new List<int>();
        public List<int> TrackBestTime = new List<int>();
        #region LEADER BORD
        public Dictionary<int,Dictionary<string, Leader>> LeaderBoard = new Dictionary<int, Dictionary<string, Leader>>();
          public class Leader
        {
            public string name;
            public int time;
        };
        #endregion

        public List<PlayerCarModel> playerCars = new List<PlayerCarModel>();

		public List<CarModel> stockCarsList;


		public List<CarColor> allColors = new List<CarColor>();

		private DataModel _model;

		public List<LevelsXP> allLevelsXP = new List<LevelsXP>();

		public void Init()
		{

		}

		public void Init(string playerId, string playerDisplayName, int tutorial)
		{
			PlayerId = playerId;
			PlayerDisplayName = playerDisplayName;
            this.isTutorial = tutorial;
            if (Core.Instance.ingoreTutorial)
            {
                this.isTutorial = 0;
            }
            //SelectedCarId = 0;

            // Core.Instance.GetService<NetworkManager>().GetAllColors(OnUserLoaded, () => { Debug.LogError("ERROR LOAD COLORS"); });
            //Core.Instance.GetService<NetworkManager>().GetAllLevelsXP(OnUserLoaded, () => { Debug.LogError("ERROR LOAD XP"); });
            SetPlayerCars(playerCars);


			if (OnUserLoaded != null)
			{
				OnUserLoaded();

			}

			if (_model == null)
			{
				_model = DataModel.Instance;
			}

			stockCarsList = new List<CarModel>();
			foreach (var car_m in _model.carsModels)
			{
				stockCarsList.Add(car_m);
			}
		}

		public void SetCurrentCur(CarModel car)
		{
            //Debug.LogError("SetCurrentCur");
			currentCar = car;
		}

		public void SetPlayerCars(List<PlayerCarModel> cars)
		{
			playerCars = cars;
		}

		public void AddCarToPlayerCarsList(PlayerCarModel car)
		{
			if (!playerCars.Contains(car))
			{
				playerCars.Add(car);
			}
		}

		public void ChooseCar(PlayerCarModel car)
		{
           
            if (playerCars.Contains(car))
			{
				selectedCar = car;
			}
			else
			{
				#region DEBUG
#if UNITY_EDITOR 
				//Debug.LogError("User don't have car: " + car);
#endif
				#endregion
			}
		}

		public void UpgradeCar()
		{

		}

        public int GetTutorialStep()
        {
            tutorialStep = PlayerPrefs.GetInt( PPKeys.Tutorial_step, 1 );

            return tutorialStep;
        }

        public void SetTutorialStep(int step)
        {
            PlayerPrefs.SetInt(PPKeys.Tutorial_step, step);
            tutorialStep = step;
        }

        public void clearPlayerCarAndUpgrades()
        {
            playerCars = new List<PlayerCarModel>();
            stockCarsList = new List<CarModel>();
            DataModel.Instance.openedCars = new List<CarModel>();
        
        }

	}
}