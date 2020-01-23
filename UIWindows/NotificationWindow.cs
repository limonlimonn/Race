using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using DG.Tweening;
using System;
using HCR.GlobalWindow.MainMenu;
using HCR.Enums;

namespace HCR
{

    public class NotificationWindow : MonoBehaviour 
    {

        
        public CanvasGroup NotificationsCanvas;
        public RectTransform NotificationRect;
        public RectTransform GameRect;
        public CanvasGroup GameAnswerCanvas;
        public Text GameAnswerText;
        public GameObject Congratulations;
        public Text CongText;
        public Image CongImage;
        private ChooseTrackWindow _chooseTrackWindow;
        private UIManager _uiManager;

        private bool AnswerShowed = false;
        private bool AnswerHide = false;

        // Use this for initialization
        void Start()
        {
            _uiManager = Core.Instance.GetService<UIManager>();
            _chooseTrackWindow = _uiManager.GetWindow(UIWindowEnum.CHOOSE_TRACK) as ChooseTrackWindow;
            Assert.AreNotEqual(null, NotificationsCanvas);
            Assert.AreNotEqual(null, GameAnswerCanvas);
            Assert.AreNotEqual(null, GameAnswerText);

        }

        public void ShowGameAnswer(string Text) {

            if (AnswerShowed) {
                return;
            }
            AnswerShowed = true;
            NotificationsCanvas.alpha = 1;
            GameAnswerCanvas.alpha = 1;
            GameAnswerText.text = Text;

            GameRect.transform.DOJump(
                new Vector3(
                    GameRect.transform.position.x, NotificationRect.position.y, GameRect.transform.position.z
                ), 1F, 1, 1F);

            AnswerHide = true;

            StartCoroutine(checkAnswerHide());
        }

        public void HideGameAnswer()
        {
            GameRect.transform.DOJump(
                new Vector3(
                    GameRect.transform.position.x, -100f, GameRect.transform.position.z
                    ),
                1F, 1, 1F);


            StartCoroutine( AlphaTimeout() );

        }

        IEnumerator HideTimeout() {
            yield return new WaitForSeconds(1f);
            HideGameAnswer();
        }

        IEnumerator AlphaTimeout() {
            yield return new WaitForSeconds(2);
            AnswerHide = false;
            AnswerShowed = false;
            NotificationsCanvas.alpha = 0;
            GameAnswerCanvas.alpha = 0;
            GameAnswerText.text = "";
        }

        IEnumerator checkAnswerHide()
        {

            yield return new WaitForEndOfFrame();
            if ((Input.touchCount > 0 || Input.anyKey) && AnswerHide)
            {
                StartCoroutine( HideTimeout() );
            }
            else {
                StartCoroutine( checkAnswerHide() );
            }
        }

        #region OpenTrack
        public void ShowYouOpenTrack(int track_id)
        {
            if (_chooseTrackWindow.trackNames.Count > track_id)
            {
                NotificationsCanvas.alpha = 1;
                NotificationsCanvas.interactable = true;
                NotificationsCanvas.blocksRaycasts = true;
                Congratulations.SetActive(true);
                CongText.text = _chooseTrackWindow.trackNames[track_id+1];
                CongImage.sprite = _chooseTrackWindow.trackList[track_id+1];
            }
            
        }

        public void HideNotifications(GameObject Window) {
            NotificationsCanvas.alpha = 0;
            NotificationsCanvas.interactable = false;
            NotificationsCanvas.blocksRaycasts = false;
            Window.SetActive(false);
        }   

        IEnumerator HideWindow(GameObject Window, float time)
        {
            yield return new WaitForSeconds(time);
            Window.SetActive(false);
        }

        #endregion


    }

}

