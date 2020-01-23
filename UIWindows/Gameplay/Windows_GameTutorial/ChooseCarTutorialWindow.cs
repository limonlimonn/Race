using System;
using HCR.Enums;
using HCR.Interfaces;
using UnityEngine.Assertions;
using HCR;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using EVP;
using UnityEngine.Assertions;

using HCR.Enums;
using HCR.Gameplay.Tutorial;
using HCR.GlobalWindow.MainMenu;



namespace HCR.Gameplay.Tutorial
{
    /// <summary>
    /// Класс - окно "Пауза" ("Игра: Tutorial")
    /// </summary>

    public class ChooseCarTutorialWindow : MonoBehaviour, IUIWindow
    {


        private UIManager _uiManager;
        private MainScreenStateManager _mainScreenStateManager;
        private GameManager _gameManager;
        private PlayerManager _playerManager;
        // FIELDS
        public CanvasGroup _canvasGroup;
        private GameWindowTutorial _gameWindow;
        public Action OnClickRally;
        public Action OnClickJeep;
        public Action OnClickBuggy;


        // I_UI_WINDOW

        public void Init()
        {


            _gameManager = Core.Instance.GetService<GameManager>();
            Assert.AreNotEqual(null, _gameManager);

            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);

            _playerManager = Core.Instance.GetService<PlayerManager>();
            Assert.AreNotEqual(null, _playerManager);

            _mainScreenStateManager = _uiManager.Get_MainScreenStateManager();
            Assert.AreNotEqual(null, _mainScreenStateManager);

            _gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_TUTORIAL) as GameWindowTutorial;
            Assert.AreNotEqual(null, _gameWindow);

            //
            AssertSerializedFields();
        }

        public void Show()
        {
          
          
            WindowViewHandler.Show(_canvasGroup);
      
        }

        public void Hide()
        {
            WindowViewHandler.Hide(_canvasGroup);

        }



        // INTERFACES

        #region ON_CLICK_BUTTON

        // used on button !
        public void OnClicButtonRally()
        {
            

            if (OnClickRally != null)
            {
                OnClickRally();
            }
        }

        public void OnClicButtonJeep()
        {


            if (OnClickJeep != null)
            {
                OnClickJeep();
            }
        }


        public void OnClicButtonBuggy()
        {


            if (OnClickBuggy != null)
            {
                OnClickBuggy();
            }
        }


        #endregion



        // METHODS

        private void AssertSerializedFields()
        {
            Assert.AreNotEqual(null, _canvasGroup);
        }



    }
}

 
 