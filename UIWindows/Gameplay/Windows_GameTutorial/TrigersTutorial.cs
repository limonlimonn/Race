using UnityEngine.Assertions;
using HCR.Enums;
using HCR.Interfaces;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using System.Collections;
using System;

namespace HCR.Gameplay.Tutorial
{
    public class TrigersTutorial : MonoBehaviour, IUIWindow
    {
        // dependences
        private GameManager _gameManager;


        private GameWindowTutorial _gameWindow;

        private UIManager _uiManager;
        [SerializeField]
        public GameObject[] TrigerScen;
        public Image[] TrigerImageButton;
        public Tween imageTween;
        [SerializeField]
        public CanvasGroup _canvasGroupMenu;
        private float[] StartPosition = new float[3];
        public Action SetPosition;

        private PauseWindow _pauseWindow;

        private bool executeButton = false;
        private int indexCurrent = 0;
        private string nameButton;

        public bool onPlay = true;



        public void Init()
        {

            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);

            _gameManager = Core.Instance.GetService<GameManager>();
            Assert.AreNotEqual(null, _gameManager);

            _gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_TUTORIAL) as GameWindowTutorial;
            Assert.AreNotEqual(null, _gameWindow);

            _pauseWindow = _uiManager.GetWindow(UIWindowEnum.PAUSE_TUTORIAL) as PauseWindow;
            Assert.AreNotEqual(null, _pauseWindow);
        }

        public void ShowTutorialHint(string nameButt, int triggerOrder)
        {
            _canvasGroupMenu.blocksRaycasts = false;
            onPlay = false;

            indexCurrent = triggerOrder;

            TrigerScen[indexCurrent].SetActive(true);

            fadeInButtonImage(TrigerImageButton[indexCurrent]);

            executeButton = true;
            Time.timeScale = 0;

            Core.Instance.Mute();

            nameButton = nameButt;
           

        }

        public void Hide()
        {
            WindowViewHandler.Hide(_canvasGroupMenu);
        }

        public void Show()
        {
        
            WindowViewHandler.Show(_canvasGroupMenu);
        }
        public void SetStartPosition(float x,float y,float z )
        {
            
            StartPosition[0] = x;
            StartPosition[1] = y;
            StartPosition[2] = z;

        }
        public float[] GetStartPosition()
        {
            return StartPosition;
        }
        public void SetPauseOff(string nameButt)
        {
            if (nameButton == nameButt || nameButt == "anykey"|| nameButton == "anykey")
            {
                if (Time.timeScale == 0 && _pauseWindow._canvasGroup.alpha == 0)
                {


                    fadeOutButtonImage();
                    Time.timeScale = 1;
                    TrigerScen[indexCurrent].SetActive(false);
                       

                    onPlay = true;
                    Core.Instance.UnMute();
                }
            }

           
        }

        public void fadeInButtonImage(Image image)
        {

            imageTween = image.DOFade(255, 200).OnKill(() => image.DOFade(0, 1));
        }
        public void fadeOutButtonImage()
        {

            imageTween.Kill();
        }
        void Update()
        {
            
            if (onPlay)
            {
                if (imageTween != null)
                {
                    imageTween.Kill();
                }
            }
        }
    }
}
