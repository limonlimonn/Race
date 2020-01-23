using UnityEngine;
using System;
using HCR.Enums;
using HCR.GlobalWindow.MainMenu;
using HCR.Interfaces;
using UnityEngine.Assertions;
using UnityEngine.UI;

using HCR.Loading;
using System.Collections.Generic;

namespace HCR.GlobalWindow
{
	/// <summary>
	/// Класс - окно Логина/Регистрации
	/// </summary>

	public class AuthentificationWindow : MonoBehaviour, IUIWindow
	{
		// FIELDS

		//
		public Action<string, string> OnLogin;
		public Action<string, string, string, string> OnRegistered;

		#region VARIABLES

		// -----------------------------------
		//
		public CanvasGroup canvaseGroup;

 
        // -----------------------------------
        //
        public RectTransform registrationPanel;
        public Button RegistrationButton;
        public RectTransform loginPanel;

        public Button LoginButton;
       

        public Text errorMessage;
        public Text errorName;
        public Text errorPass;
        public Text errorMale;

        public InputField autUserName;
		public InputField autPass;
        public InputField regUserName;
        public Dropdown regUserDays;
        public Dropdown regUserMonth;
        public Dropdown regUserYears;
        public Text SelectMonth;
        public Text SelectDay;
        public Text SelectYear;
        public InputField regPass;
        public Toggle Male;
        public Toggle Female;
        private string UserMale = "Female";

        private List<string> Month = new List<string>(){ "January","February","March","April","May","June","July","August","September","October","November","December" };
        private List<string> Day = new List<string>();
        private List<string> Years = new List<string>();
        #endregion

        // dependences
        private UIManager _uiManager;
		private SplashScreen _splashScreen;
		private Header _header;



		// I_UI_WINDOW

		public void Init()
		{
			_uiManager = Core.Instance.GetService<UIManager>();
			Assert.AreNotEqual(null, _uiManager);

			_splashScreen = _uiManager.GetWindow(UIWindowEnum.SPLASH) as SplashScreen;
			Assert.AreNotEqual(null, _splashScreen);

			_header = _uiManager.GetWindow(UIWindowEnum.HEADER) as Header;
			Assert.AreNotEqual(null, _header);

            InitDropdown();







        }

        private void InitDropdown()
        {//clear
            regUserDays.ClearOptions();
            regUserMonth.ClearOptions();
            regUserYears.ClearOptions();
            //Years
            for (int i = 2017; i > 1883; i--)
                Years.Add(i.ToString());

            regUserYears.AddOptions(Years);
            //Month
            regUserMonth.AddOptions(Month);
            //Day
            for (int i = 1; i <= DateTime.DaysInMonth(2017, 1); i++)
                Day.Add(i.ToString());

            regUserDays.AddOptions(Day);
        }

        public void CheckDay()
        {
            
           
            int month =  Month.IndexOf(SelectMonth.text) + 1;
            Debug.Log("month" + month );
            int years = int.Parse(SelectYear.text);
            Day.Clear();
            for (int i = 1; i <= DateTime.DaysInMonth(years, month); i++)
                Day.Add(i.ToString());

            regUserDays.ClearOptions();
            regUserDays.AddOptions(Day);
            
        }

        public void CheckMale(string male)
        {
            if (male == "Male")
            {
                Female.isOn = !Male.isOn;
            }
            else if (male == "Female")
            {
                Male.isOn = !Female.isOn;
            }
            /*
            if (Male.isOn)
            {
                Female.isOn = false;
                UserMale = "Male";
            }
            else if (Female.isOn)
            {
                Male.isOn = false;
                UserMale = "Female";

            }
            */
        }

        public void Show()
		{
			canvaseGroup.alpha = 1;
			canvaseGroup.blocksRaycasts = true;

			//
			_header.Hide();

			//
			errorMessage.text = "";
			autUserName.text = "";
			autPass.text = "";
			regUserName.text = "";
			regPass.text = "";
            
            ShowLoginPanel();
		}

		public void Hide()
		{
			canvaseGroup.alpha = 0;
			canvaseGroup.blocksRaycasts = false;
		}



		// INTERFACES

		#region LOGIN

		public void ShowLoginPanel()
		{
            LoginButton.interactable = false;
            RegistrationButton.interactable = true;
            registrationPanel.localScale = new Vector3(1f, 0, 1f);
			loginPanel.localScale = new Vector3(1f, 1f, 1f);
		}

		// used on button !
		public void ApplyLogin()
		{
            if (OnLogin != null) {
				OnLogin(autUserName.text, autPass.text); }
		}

		public void LoginErrorMessage(string errorMessage)
		{
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
            this.errorMessage.text = errorMessage;
		}

		#endregion

		#region REGISTRATION

		public void Registration()
		{
            LoginButton.interactable = true;
            RegistrationButton.interactable = false;
            registrationPanel.localScale = new Vector3(1f, 1f, 1f);
			loginPanel.localScale = new Vector3(1f, 0, 1f);
		}

		// used on button !
		public void ApplyRegistration()
		{
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
            bool IsError = false;
            errorPass.gameObject.SetActive(false);
            errorName.gameObject.SetActive(false);
            errorMale.gameObject.SetActive(false);
            int month = Month.IndexOf(SelectMonth.text) + 1;
            string Date = SelectDay.text + " " + month.ToString() + " " + SelectYear.text;

            if(Female.isOn == Male.isOn)
            {
                _splashScreen.Hide();
                errorMale.gameObject.SetActive(true);
                IsError = true;
            }

            if(regUserName.text.Length < 3)
            {
                _splashScreen.Hide();
                errorName.gameObject.SetActive(true);
                IsError = true;
            }
            if(regPass.text.Length < 6)
            {
                _splashScreen.Hide();
                errorPass.gameObject.SetActive(true);
                IsError = true;
            }

            if (OnRegistered != null && !IsError)
            {
                OnRegistered(regUserName.text, regPass.text, Date, UserMale);
            }
        }

		public void RegistrationErrorMessage(string errorMessage)
		{
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
            this.errorMessage.text = errorMessage;
		}

		#endregion

		// used on input field !
		public void OnInputField_ValueChanged()
		{
			if (errorMessage.text != "")
			{
				errorMessage.text = "";
			}
		}



	}
}