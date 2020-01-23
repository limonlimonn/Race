using HCR.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace HCR.Network
{
    public class EnternetWindow : MonoBehaviour, IUIWindow
    {
        public CanvasGroup canvasGroup;
        private UIManager _uiManager;
        
        public GameObject ErrorSend;
        public GameObject ErrorEnternet;

        public Action ClickTry;
        public Action ClickLater;
        public Action ClickOk;

        public Text nameSecondButt;
        public Text nameOkButt;

        public void Hide()
        {
            _uiManager.HideCanvas(canvasGroup);
            canvasGroup.blocksRaycasts = false;
        }

        public void Init()
        {
            _uiManager = Core.Instance.GetService<UIManager>();
            ErrorSend.SetActive(false);
            ErrorEnternet.SetActive(false);
        }

        public void Show()
        {
            _uiManager.ShowCanvas(canvasGroup);
            canvasGroup.blocksRaycasts = true;
        }

        #region BUTTON

        public void OnClickOk()
        {
            if (ClickOk != null)
            {
                ClickOk();
            }
            Hide();
        }

        public void OnClickTry()
        {
            ClickTry();
            Hide();
        }

        public void OnClickLater()
        {
            ClickLater();
            Hide();
        }
        #endregion

        public void ShowErrorSend(Action ClickTry,string SecondButtName , Action ClickLater)
        {
            nameSecondButt.text = SecondButtName;
            Show();
            _uiManager.ShowWindow(Enums.UIWindowEnum.SPLASH, false);
            ErrorSend.SetActive(true);
            ErrorEnternet.SetActive(false);
            this.ClickTry = ClickTry;
            this.ClickLater = ClickLater;
        }

        public void ShowErrorEnternet(Action ClickTry = null, string name = null)
        {
            nameOkButt.text = "Ok";
            if (name != null)
                nameOkButt.text = name;

            Show();
            ErrorEnternet.SetActive(true);
            ErrorSend.SetActive(false);
                ClickOk = ClickTry;
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
