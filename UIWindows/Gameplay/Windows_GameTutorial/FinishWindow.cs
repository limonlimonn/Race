using UnityEngine.Assertions;

using HCR.Enums;
using HCR.Interfaces;
using UnityEngine;
using System;

namespace HCR.Gameplay.Tutorial
{
	/// <summary>
	/// Класс - окно при удачном финише ("Игра: Одиночная игра")
	/// с начислением Silver, Exp
	/// </summary>

	public class FinishWindow : MonoBehaviour ,  IUIWindow
	{
        // dependences
        public Action NExtTrack;

        [SerializeField]
        public CanvasGroup _canvaseGroupMenu;
        [SerializeField]
        public CanvasGroup _canvaseGroup;

        private UIManager _uiManager;
        private GameWindowTutorial _gameWindowTutorial;
       
        private GameManager _gameManager;
        private PlayerManager _playerManager;
        private NetworkManager _networkManager;
        private RaceResultEnum _raceResult;



        // I_UI_WINDOW

        public void Init()
		{

            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);

            _gameManager = Core.Instance.GetService<GameManager>();
			Assert.AreNotEqual(null, _gameManager);

            _gameWindowTutorial = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_TUTORIAL) as GameWindowTutorial;
			Assert.AreNotEqual(null, _gameWindowTutorial);

            _playerManager = Core.Instance.GetService<PlayerManager>();
            Assert.AreNotEqual(null, _playerManager);

            _networkManager = Core.Instance.GetService<NetworkManager>();
            Assert.AreNotEqual(null, _networkManager);



            //
            AssertVariables();
		}



        

        public void Show()
		{
           


           if (_playerManager.GetTutorialStep() == 1)
            {
                
                WindowViewHandler.Show(_canvaseGroup);
                
            }
            else if (_playerManager.GetTutorialStep() == 2)
           {
               
                WindowViewHandler.Show(_canvaseGroupMenu);
               
           
            }


            //_gameWindowTutorial.BlockPauseButton();
		}

		public void Hide()
		{
			WindowViewHandler.Hide(_canvaseGroup);
		}



        // INTERFACES

        // used on button !

        public void SetValue_RaceResult(RaceResultEnum raceResult)
        {
            _raceResult = raceResult;
        }


        public void OnClickButton_StampX(int multiplier)
		{
			//
		}
        public void OnNExtTrack()
        {
            Hide();

            if (NExtTrack != null)
            {
                NExtTrack();
            }
        }



        #region RECORD

        // used on button !
        public void OnClickButton_Record()
		{
			_uiManager.ShowWindow(UIWindowEnum.SPLASH, true);
			_networkManager.SaveSingleplayer(Callback_RecordOK, Callback_RecordError);
		}

		private void Callback_RecordOK()
		{
			Hide();
			_uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
			_gameManager.Finish();
		}

		private void Callback_RecordError()
		{
			_uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
			Debug.Log("Error!");
		}

		#endregion



		// METHODS

		private void AssertVariables()
		{
			#region ASSERT

			Assert.AreNotEqual(null, _canvaseGroup);

			#endregion
		}



	}
}