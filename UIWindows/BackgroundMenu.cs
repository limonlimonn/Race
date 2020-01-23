using HCR.Interfaces;
using UnityEngine;
using UnityEngine.Assertions;
using HCR.GlobalWindow.MainMenu;
using HCR.Enums;

namespace HCR
{
    public class BackgroundMenu : MonoBehaviour, IUIWindow
    {

        [SerializeField]
        protected CanvasGroup _canvaseGroup;

        [SerializeField]
        private GameObject[] _windowEnumTutorial;
        [SerializeField]
        private GameObject[] _windowTextTurial;
        [SerializeField]
        private GameObject _HederBG;
        [SerializeField]
        private GameObject _IndexBG;

        [SerializeField]
        private GameObject TreeBG;

        private int index;
        private ChooseCarWindow _chooseCarWindow;
        private MultiplayerWindow _multiplayerWindow;
        private bool MultiButt = false;
        private UIManager _uiManager;
        private PlayerManager _playerManager;


        private NetworkManager _networkManager;

       

        public void Hide()
        {
            WindowViewHandler.Hide(_canvaseGroup);


        }

        public void Init()
        { 

            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);
            _chooseCarWindow = _uiManager.GetWindow(Enums.UIWindowEnum.CHOOSE_CAR) as ChooseCarWindow;
            Assert.AreNotEqual(null, _chooseCarWindow);
            _networkManager = Core.Instance.GetService<NetworkManager>();
            Assert.AreNotEqual(null, _networkManager);
            _multiplayerWindow = _uiManager.GetWindow(UIWindowEnum.MAIN_MULTIPLAYER) as MultiplayerWindow;
            Assert.AreNotEqual(null, _multiplayerWindow);

            _playerManager = Core.Instance.GetService<PlayerManager>();
            Assert.AreNotEqual(null, _playerManager);
        }

        public void Show()
        {

            

            _playerManager.clearPlayerCarAndUpgrades();
            _networkManager.LoadUserFromObjData(_networkManager.tutorialPlayerData);

            _chooseCarWindow.ShowTutorialOnBuyCar += ActionCarBuy;
            _chooseCarWindow.ShowTutorialCarUpdate += ActionUpdateBuy;
            WindowViewHandler.Show(_canvaseGroup);
            index = 0;
            _windowEnumTutorial[index].SetActive(true);
            _windowTextTurial[index].SetActive(true);
        }

        public void StandartNextButton(int ButtonIndex)
        {
            HideAllTextAndButton();
            if (_windowEnumTutorial[ButtonIndex])
            {
                _windowEnumTutorial[ButtonIndex].SetActive(true);
            }
            _windowTextTurial[ButtonIndex].SetActive(true);
        }

       

        public void OkButton()
        {
            HideAllTextAndButton();
            _IndexBG.SetActive(false);
        }

        public void ActionCarBuy()
        {
            _windowTextTurial[8].SetActive(true);
        }

        public void ActionUpdateBuy()
        {
            HideAllTextAndButton();
            index = 12;
            Debug.Log(index);
            _windowEnumTutorial[index].SetActive(true);
            _windowTextTurial[index].SetActive(true);
            _IndexBG.SetActive(true);

        }

        public void showMultiIntro(int ButtonIndex)
        {
            //_IndexBG.SetActive(false);
            HideAllTextAndButton();
            if (_windowEnumTutorial[ButtonIndex])
            {
                _windowEnumTutorial[ButtonIndex].SetActive(true);
            }
            _windowTextTurial[ButtonIndex].SetActive(true);
        }

        public void EndTutorial()
        {

            _chooseCarWindow.ShowTutorialOnBuyCar -= ActionCarBuy;
            _chooseCarWindow.ShowTutorialCarUpdate -= ActionUpdateBuy;

            HideAllTextAndButton();

            _uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
            
            _networkManager.EndTutorial(
                endTutorialSucces, endTutorialError
            );
            
        }

        private void endTutorialSucces()
        {
            _IndexBG.SetActive(false);
            _uiManager.ShowWindow(UIWindowEnum.MENUTUTORIALWINDOW, false);
            _uiManager.ShowWindow(UIWindowEnum.FIRSTTUTORIALWINDOW, false);
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
            _multiplayerWindow.OnClickButton_RandomGame();

        }
        private void endTutorialError()
        {
            _IndexBG.SetActive(false);
            _uiManager.ShowWindow(UIWindowEnum.MENUTUTORIALWINDOW, false);
            _uiManager.ShowWindow(UIWindowEnum.FIRSTTUTORIALWINDOW, false);
            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
        }

        private void HideAllTextAndButton()
        {
            for (var i = 0; i < _windowEnumTutorial.Length; i++)
            {
                _windowEnumTutorial[i].SetActive(false);
            }

            for (var i = 0; i < _windowTextTurial.Length; i++)
            {
                _windowTextTurial[i].SetActive(false);
            }
        }

    }
}
