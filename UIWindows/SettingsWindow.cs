using HCR.Enums;
using HCR.Interfaces;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameSparks.Api;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;
using System.Collections.Generic;
using System;
using DG.Tweening;

namespace HCR.GlobalWindow.MainMenu
{
    /// <summary>
    /// Класс - окно настроек, вызываемое из Header
    /// </summary>

    public class SettingsWindow : MonoBehaviour, IUIWindow
    {
        // FIELDS

        #region VARIABLES

        [SerializeField]
        private SpriteRenderer _background;

        [Header("PANELS")]
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [Header("CHOOSE UI")]
        [SerializeField]
        private Toggle _toggleOldUI;
        [SerializeField]
        private Toggle _toggleNewUI;

        [Header("QUALITY SETTINGS")]
        [SerializeField]
        private Toggle _toggleQualityShadows;
        [SerializeField]
        private Toggle _toggleVibration;

        [SerializeField]
        private Toggle _toggleMusic;

        [SerializeField]
        private Text lbl_isLog;
        public Slider VolumeMusic;
        public Slider VolumeSounds;

        [SerializeField]
        private Image LoginFB;


        private FacebookConnectRequest FC;

        private string fb_id;
        private string fb_token;


        #endregion

        // dependences
        private UIManager _uiManager;
        private MainScreenStateManager _mainScreenStateManager;
        private NetworkManager _networkManager;
        private AudioService _audioService;
        private SafePlayerPrefs _safePlayerPrefs;


        // I_UI_WINDOW

        public void Init()
        {
            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);

            _mainScreenStateManager = _uiManager.Get_MainScreenStateManager();
            Assert.AreNotEqual(null, _mainScreenStateManager);

            _networkManager = Core.Instance.GetService<NetworkManager>();
            Assert.AreNotEqual(null, _networkManager);
            _audioService = Core.Instance.GetService<AudioService>();
            Assert.AreNotEqual(null, _audioService);
            //
            _safePlayerPrefs = Core.Instance.GetService<SafePlayerPrefs>();
            Assert.AreNotEqual(null, _safePlayerPrefs);
            AssertSerializedFields();

            CheckUiTogglesOnInit();
            CheckQualityShadowsOnInit();
            CheckVibrationOnInit();
            CheckVolumeMusic();
            CheckMusicOnInit();



        }

        public void Show()
        {
           // _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _uiManager.ShowCanvas(_canvasGroup);

            

            _background.gameObject.SetActive(true);
            _background.sprite = _uiManager.ChangeBG(UIWindowEnum.SETTINGS);
        }

        public void Hide()
        {
            //_canvasGroup.alpha = 0;
            _uiManager.HideCanvas(_canvasGroup);
            _canvasGroup.blocksRaycasts = false;

            _background.gameObject.SetActive(false);
        }



        // INTERFACES

        // used on button !
        public void OnClickButton_OldUI()
        {
            bool state = _toggleOldUI.isOn;
            _toggleNewUI.isOn = !state;

            UITypeEnum chosenUI = GetSelectedUI();
            PlayerPrefs.SetInt(PPKeys.uiType, (int)chosenUI);
            PlayerPrefs.Save();
        }

        // used on button !
        public void OnClickButton_NewUI()
        {
            bool state = _toggleNewUI.isOn;
            _toggleOldUI.isOn = !state;

            UITypeEnum chosenUI = GetSelectedUI();
            PlayerPrefs.SetInt(PPKeys.uiType, (int)chosenUI);
            PlayerPrefs.Save();
        }

        public void OnClickAudioVibration()
        {
           
            VibrationEnum chosenVibration = GetSelectedVibration();
            PlayerPrefs.SetInt(PPKeys.vibration, (int)chosenVibration);
            SetVibration();
            PlayerPrefs.Save();
        }
       

       public void OnClicMusic()
        {
           
            if (_toggleMusic.isOn)
            {
                _audioService.UserChoiseMusic = 1;
                _audioService.StartMenuMusic();
            }
            else
            {
                _audioService.UserChoiseMusic = 0;
                _audioService.StopMenuMusic();
            }
            
            MusicEnum chosenMusic = GetSelectedMusic();
          
            PlayerPrefs.SetInt(PPKeys.music, (int)chosenMusic);
            PlayerPrefs.Save();
        }

        public void OnClickVideo()
        {
            
            QualityShadowsEnum chosenQualityShadows = GetSelectedQualityShadows();
            PlayerPrefs.SetInt(PPKeys.qualityShadows, (int)chosenQualityShadows);
            SetQualityShadows();
            PlayerPrefs.Save();
        }
       
        // used on button !
        public void OnClickButton_FBLog()
        {
            var perms = new List<string>() { "public_profile", "email", "user_friends" };
            
        }

        // used on button !
        public void OnClickButton_LogOut()
        {
            _safePlayerPrefs.DeleteDataForLogOut();
            _audioService.StopMenuMusicLG();
           // PlayerPrefs.DeleteAll();
            
            
            DataModel.Instance.DestroySelf();
            Core.Instance.DestroySelf();
            GameSparks.Core.GS.Disconnect();
            SceneManager.LoadScene("EntryPoint");
        }

        public void OnClickButton_VolumeMusic()
        {
            _audioService.SetVolumeMusic = VolumeMusic.value;
            PlayerPrefs.SetFloat(PPKeys.volumeMusic, VolumeMusic.value);
            _audioService.UpdateVolumMusic();


        }
        public void CheckVolumeMusic()
        {
            bool isVolumeSave = PlayerPrefs.HasKey(PPKeys.volumeMusic);
            if (!isVolumeSave)
            {
                PlayerPrefs.SetFloat(PPKeys.volumeMusic, 1f);
            }

            VolumeMusic.value = PlayerPrefs.GetFloat(PPKeys.volumeMusic);
            _audioService.SetVolumeMusic = VolumeMusic.value;

        }

        public void OnClickButton_VolumeSounds()
        {
            _audioService.SetVolumeSounds = VolumeSounds.value;
            PlayerPrefs.SetFloat(PPKeys.volumeSounds, VolumeSounds.value);

        }
        public void CheckVolumeSounds()
        {
            bool isVolumeSave = PlayerPrefs.HasKey(PPKeys.volumeSounds);
            if (!isVolumeSave)
            {
                PlayerPrefs.SetFloat(PPKeys.volumeSounds, 1f);
               
            }
            VolumeSounds.value = PlayerPrefs.GetFloat(PPKeys.volumeSounds);
            _audioService.SetVolumeSounds = VolumeSounds.value;
        }






        // METHODS

        private void AssertSerializedFields()
        {
            Assert.AreNotEqual(null, _background);

            Assert.AreNotEqual(null, _canvasGroup);

            Assert.AreNotEqual(null, _toggleOldUI);
            Assert.AreNotEqual(null, _toggleNewUI);

            Assert.AreNotEqual(null, _toggleQualityShadows);
        }

        #region UI_SETTINGS

        private void CheckUiTogglesOnInit()
        {
            bool isUiTypeWasSaved = PlayerPrefs.HasKey(PPKeys.uiType);

            if (isUiTypeWasSaved)
            {
                SetUiTogglesView();
            }
            else
            {
                SetTogglesDefaultView();
                SaveUiTogglesDefaultSettings();
            }
        }

        private void SetUiTogglesView()
        {
            UITypeEnum uiType = (UITypeEnum)PlayerPrefs.GetInt(PPKeys.uiType);

            switch (uiType)
            {
                case UITypeEnum.OLD_UI:
                    _toggleOldUI.isOn = true;
                    _toggleNewUI.isOn = false;
                    break;

                case UITypeEnum.NEW_UI:
                    _toggleNewUI.isOn = true;
                    _toggleOldUI.isOn = false;
                    break;

                default:
                    #region DEBUG
#if UNITY_EDITOR
                    Debug.Log("[ERROR] uiType is wrong = " + uiType + " !");
#endif
                    #endregion
                    break;
            }
        }

        private void SetTogglesDefaultView()
        {
            _toggleOldUI.isOn = true;
            _toggleNewUI.isOn = false;
        }

        private void SaveUiTogglesDefaultSettings()
        {
            int defaultUI = (int)UITypeEnum.OLD_UI;

            PlayerPrefs.SetInt(PPKeys.uiType, defaultUI);
            PlayerPrefs.Save();
        }

        #endregion

        #region QUALITY_SETTINGS

        public void CheckMusicOnInit()
        {
            bool isMusicSaved = PlayerPrefs.HasKey(PPKeys.music);

            if (isMusicSaved)
            {
                _audioService.UserChoiseMusic = PlayerPrefs.GetInt(PPKeys.music);
                switch (_audioService.UserChoiseMusic)
                {
                    case 0:
                        _toggleMusic.isOn = false;
                        break;
                    case 1:
                        _toggleMusic.isOn = true;
                        break;
                }
            }
            else
            {
               
               _audioService.UserChoiseMusic = 1;
                _toggleMusic.isOn = true;
            }
        }



        private void CheckQualityShadowsOnInit()
        {
            bool isQualityShadowSaved = PlayerPrefs.HasKey(PPKeys.qualityShadows);

            if (isQualityShadowSaved)
            {
                SetQualityShadows();
            }
            else
            {
                SaveDefaultQualityShadowsSettings();
            }
        }

        private void CheckVibrationOnInit()
        {
            bool isVibrationSaved = PlayerPrefs.HasKey(PPKeys.vibration);

            if (isVibrationSaved)
            {
                SetVibration();
            }
            else
            {
                SaveDefaultVibrationSettings();
            }
        }

        private void SetQualityShadows()
        {
            QualityShadowsEnum qualityShadows = (QualityShadowsEnum)PlayerPrefs.GetInt(PPKeys.qualityShadows);

            switch (qualityShadows)
            {
                case QualityShadowsEnum.ON:
                    SetUnitySettings_ShadowsOn();
                    break;

                case QualityShadowsEnum.OFF:
                    SetUnitySettings_ShadowsOff();
                    break;

                default:
                    #region DEBUG
#if UNITY_EDITOR
                    Debug.Log("[ERROR] qualityShadows is wrong = " + qualityShadows + " !");
#endif
                    #endregion
                    break;
            }
        }

        private void SetVibration()
        {
            VibrationEnum vibration = (VibrationEnum)PlayerPrefs.GetInt(PPKeys.vibration);

            switch (vibration)
            {
                case VibrationEnum.ON:
                    SetUnitySettings_VibrationOn();
                    break;

                case VibrationEnum.OFF:
                    SetUnitySettings_VibrationOff();
                    break;

                default:
                    #region DEBUG
#if UNITY_EDITOR
                    Debug.Log("[ERROR] vibration is wrong  !");
#endif
                    #endregion
                    break;
            }
        }

        private void SaveDefaultQualityShadowsSettings()
        {
            SetUnitySettings_ShadowsOn();
        }

        private void SaveDefaultVibrationSettings()
        {
            SetUnitySettings_VibrationOff();
        }

        private void SetUnitySettings_ShadowsOn()
        {
            _toggleQualityShadows.isOn = true;
            QualitySettings.shadows = ShadowQuality.HardOnly;
        }

        private void SetUnitySettings_ShadowsOff()
        {
            _toggleQualityShadows.isOn = false;
            QualitySettings.shadows = ShadowQuality.Disable;
        }

        private void SetUnitySettings_VibrationOn()
        {
            _toggleVibration.isOn = true;
            Vibration.IsOn = true;
        }

        private void SetUnitySettings_VibrationOff()
        {
            _toggleVibration.isOn = false;
            Vibration.IsOn = false;
        }

        #endregion

        #region SAVE_SETTINGS !!!



        private UITypeEnum GetSelectedUI()
        {
            if (_toggleOldUI.isOn && !_toggleNewUI.isOn)
            {
                return UITypeEnum.OLD_UI;
            }
            if (_toggleNewUI.isOn && !_toggleOldUI.isOn)
            {
                return UITypeEnum.NEW_UI;
            }

            #region DEBUG
#if UNITY_EDITOR
            Debug.Log("[ERROR] toggles is in wrong states : " +
                      "Old = " + _toggleOldUI.isOn + " | " +
                      "New = " + _toggleNewUI.isOn);
#endif
            #endregion
            return UITypeEnum.OLD_UI;
        }
        private MusicEnum GetSelectedMusic()
        {
            return (_toggleMusic.isOn) ?
                (MusicEnum.ON) : (MusicEnum.OFF);
        }

        private QualityShadowsEnum GetSelectedQualityShadows()
        {
            return (_toggleQualityShadows.isOn) ?
                (QualityShadowsEnum.ON) : (QualityShadowsEnum.OFF);
        }

        private VibrationEnum GetSelectedVibration()
        {
            return (_toggleVibration.isOn) ?
                (VibrationEnum.ON) : (VibrationEnum.OFF);
        }

        #endregion



       

    }
}