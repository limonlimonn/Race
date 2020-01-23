using System;
using System.Collections;

using HCR.Enums;
using HCR.GlobalWindow.MainMenu;
using HCR.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace HCR
{
	public class StampTimerManager : IService
	{
		public Header header;
		public int nowStamp;

		public int X2StartStamp;
		public int X2EndStamp;
		public int currentX2Minutes;

		public int X3StartStamp;
		public int X3EndStamp;
		public int currentX3Minutes;

		public int X4StartStamp;
		public int X4EndStamp;
		public int currentX4Minutes;

		//
		private bool _isX2_Active;

		private bool _isX3_Active;
		private bool _isX4_Active;

		//
		private UIManager _uim;



		public void Init()
		{
            //NotificationManager.CancelAll();
            _uim = Core.Instance.GetService<UIManager>();
            header = _uim.GetWindow(UIWindowEnum.HEADER) as Header;

            currentX2Minutes = (nowStamp - X2StartStamp) / 60;
            RegistrationNotification((X2EndStamp - nowStamp));
            currentX3Minutes = (nowStamp - X3StartStamp) / 60;
            RegistrationNotification((X3EndStamp - nowStamp));
            currentX4Minutes = (nowStamp - X4StartStamp) / 60;
            RegistrationNotification((X4EndStamp - nowStamp));
            SetOffAnim(GameObject.Find("CountImage2").GetComponent<Image>(), 2);
            Core.Instance.StartCor(StartX2Bonus());
            SetOffAnim(GameObject.Find("CountImage3").GetComponent<Image>(), 3);
            Core.Instance.StartCor(StartX3Bonus());
            SetOffAnim(GameObject.Find("CountImage4").GetComponent<Image>(), 4);
            Core.Instance.StartCor(StartX4Bonus());
        }

        public void ReInit()
        {
            Core.Instance.StopCoroutine(StartX2Bonus());
            Core.Instance.StopCoroutine(StartX3Bonus());
            Core.Instance.StopCoroutine(StartX4Bonus());
            SetOffAnim(GameObject.Find("CountImage2").GetComponent<Image>(), 2);
            SetOffAnim(GameObject.Find("CountImage3").GetComponent<Image>(), 3);
            SetOffAnim(GameObject.Find("CountImage4").GetComponent<Image>(), 4);
            Init();
        }

        public void RegistrationNotification(int CurrentTimer)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
           // if (CurrentTimer > 60)
            //NotificationManager.Send(TimeSpan.FromSeconds(CurrentTimer), "Score modifier is ready", "Go in game for obtain bonus", new Color(1, 0.3f, 0.15f));
#endif
        }

        IEnumerator StartX2Bonus()
		{
			header.X2Timer.text = TimerFor2XStamp();
			header.slider2X.fillAmount = (currentX2Minutes / ((X2EndStamp - X2StartStamp) / 60f));
			while (currentX2Minutes < ((X2EndStamp - X2StartStamp) / 60))
			{
				yield return new WaitForSecondsRealtime(59);
				currentX2Minutes++;
				header.slider2X.fillAmount = (currentX2Minutes / ((X2EndStamp - X2StartStamp) / 60f));
				header.X2Timer.text = TimerFor2XStamp();
			}
			//TODO some Action;
			SetCompleteAnim(GameObject.Find("CountImage2").GetComponent<Image>(), 2);
			header.slider2X.fillAmount = 1;

			_isX2_Active = true;

			//Debug.Log("TIMER 2 END");
		}

		IEnumerator StartX3Bonus()
		{
			header.slider3X.fillAmount = (currentX3Minutes / ((X3EndStamp - X3StartStamp) / 60f));
			header.X3Timer.text = TimerFor3XStamp();
			while (currentX3Minutes < ((X3EndStamp - X3StartStamp) / 60))
			{
				yield return new WaitForSecondsRealtime(59);
				currentX3Minutes++;
				header.slider3X.fillAmount = (currentX3Minutes / ((X3EndStamp - X3StartStamp) / 60f));

				header.X3Timer.text = TimerFor3XStamp();
			}

			//TODO some Action;
			SetCompleteAnim(GameObject.Find("CountImage3").GetComponent<Image>(), 3);
            header.slider3X.fillAmount = 1;
            _isX3_Active = true;
		}

		IEnumerator StartX4Bonus()
		{
            _isX4_Active = false;
            header.X4Timer.text = TimerFor4XStamp();
			header.slider4X.fillAmount = (currentX4Minutes / ((X4EndStamp - X4StartStamp) / 60f));
			while (currentX4Minutes < ((X4EndStamp - X4StartStamp) / 60))
			{
				yield return new WaitForSecondsRealtime(59);
				currentX4Minutes++;
				header.slider4X.fillAmount = (currentX4Minutes / ((X4EndStamp - X4StartStamp) / 60f));
				header.X4Timer.text = TimerFor4XStamp();
			}

			//TODO some Action;
			SetCompleteAnim(GameObject.Find("CountImage4").GetComponent<Image>(), 4);
            //Debug.LogError("TIMER 4 END");
            header.slider4X.fillAmount = 1;
            _isX4_Active = true;
		}

		public string TimerFor2XStamp()
		{
			string time = "";
			int hours = (((X2EndStamp - X2StartStamp) / 60) - currentX2Minutes) / 60;
			int min = (((X2EndStamp - X2StartStamp) / 60) - currentX2Minutes) % 60;

			if (min < 10)
			{
				if (hours < 10)
				{
					time = String.Format("0{0}:0{1}", hours > 0 ? hours : 00, min);
				}
				else
				{
					time = String.Format("{0}:0{1}", hours > 0 ? hours : 00, min);
				}

			}
			else
			{
				if (hours < 10)
				{
					time = String.Format("0{0}:{1}", hours > 0 ? hours : 00, min);
				}
				else
				{
					time = String.Format("{0}:{1}", hours > 0 ? hours : 00, min);
				}
			}

			return time;
		}

		public string TimerFor3XStamp()
		{
			string time = "";
			int hours = (((X3EndStamp - X3StartStamp) / 60) - currentX3Minutes) / 60;
			int min = (((X3EndStamp - X3StartStamp) / 60) - currentX3Minutes) % 60;


			if (min < 10)
			{
				if (hours < 10)
				{
					time = String.Format("0{0}:0{1}", hours > 0 ? hours : 00, min);
				}
				else
				{
					time = String.Format("{0}:0{1}", hours > 0 ? hours : 00, min);
				}

			}
			else
			{
				if (hours < 10)
				{
					time = String.Format("0{0}:{1}", hours > 0 ? hours : 00, min);
				}
				else
				{
					time = String.Format("{0}:{1}", hours > 0 ? hours : 00, min);
				}
			}

			return time;
		}

		public string TimerFor4XStamp()
		{
			string time = "";
			int hours = (((X4EndStamp - X4StartStamp) / 60) - currentX4Minutes) / 60;
			int min = (((X4EndStamp - X4StartStamp) / 60) - currentX4Minutes) % 60;

			if (min < 10)
			{
				if (hours < 10)
				{
					time = String.Format("0{0}:0{1}", hours > 0 ? hours : 00, min);
				}
				else
				{
					time = String.Format("{0}:0{1}", hours > 0 ? hours : 00, min);
				}

			}
			else
			{
				if (hours < 10)
				{
					time = String.Format("0{0}:{1}", hours > 0 ? hours : 00, min);
				}
				else
				{
					time = String.Format("{0}:{1}", hours > 0 ? hours : 00, min);
				}
			}

			return time;
		}

        public void SetOffAnim(Image image, int id)
        {
            //Debug.Log("SetOffAnim " + id);
            switch (id)
            {
                case 2:
                    header.bonusImage = image; // GameObject.Find("CountImage2").GetComponent<Image>();
                    header.bonusImage.sprite = header.inActiveBonusImage2;
                    header.bonusImage.rectTransform.sizeDelta = new Vector2(66, 75);
                    header.X2Timer.color = new Color32(255, 255, 255, 255);
                    header.bonus2Animator.enabled = false;
                    _isX2_Active = false;
                    break;
                case 3:
                    header.bonusImage = image; // GameObject.Find("CountImage2").GetComponent<Image>();
                    header.bonusImage.sprite = header.inActiveBonusImage3;
                    header.bonusImage.rectTransform.sizeDelta = new Vector2(66, 75);
                    header.X3Timer.color = new Color32(255, 255, 255, 255);
                    header.bonus3Animator.enabled = false;
                    _isX3_Active = false;
                    break;
                case 4:
                    header.bonusImage = image; // GameObject.Find("CountImage2").GetComponent<Image>();
                    header.bonusImage.sprite = header.inActiveBonusImage4;
                    header.bonusImage.rectTransform.sizeDelta = new Vector2(66, 75);
                    header.X4Timer.color = new Color32(255, 255, 255, 255);
                    header.bonus4Animator.enabled = false;
                    _isX4_Active = false;
                    break;
            }
        }


        public void SetCompleteAnim(Image image, int id)
		{
            //Debug.Log("SetCompleteAnim " + id);
			switch (id)
			{
				case 2:
					header.bonusImage = image; // GameObject.Find("CountImage2").GetComponent<Image>();
					header.bonusImage.sprite = header.activeBonusImage;
					header.bonusImage.rectTransform.sizeDelta = new Vector2(114, 120);
					header.X2Timer.text = "00:00";
					header.X2Timer.color = new Color32(255, 204, 0, 255);
					header.bonus2Animator.enabled = true;
					break;
				case 3:
					header.bonusImage = image; // GameObject.Find("CountImage2").GetComponent<Image>();
					header.bonusImage.sprite = header.activeBonusImage;
					header.bonusImage.rectTransform.sizeDelta = new Vector2(114, 120);
					header.X3Timer.text = "00:00";
					header.X3Timer.color = new Color32(255, 204, 0, 255);
					header.bonus3Animator.enabled = true;
					break;
				case 4:
					header.bonusImage = image; // GameObject.Find("CountImage2").GetComponent<Image>();
					header.bonusImage.sprite = header.activeBonusImage;
					header.bonusImage.rectTransform.sizeDelta = new Vector2(114, 120);
					header.X4Timer.text = "00:00";
					header.X4Timer.color = new Color32(255, 204, 0, 255);
					header.bonus4Animator.enabled = true;
					break;
			}
		}

		//
		public bool Get_IsX2_Active()
		{
			return _isX2_Active;
		}

		public bool Get_IsX3_Active()
		{
			return _isX3_Active;
		}

		public bool Get_IsX4_Active()
		{
			return _isX4_Active;
		}



	}
}