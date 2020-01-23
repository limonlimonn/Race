using System;
using HCR.Enums;
using HCR.Interfaces;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

namespace HCR.Gameplay.Tutorial
{
    /// <summary>
    /// Tutorial 
    /// </summary>

    public class GameWindowTutorial : ABaseGameWindow, IGameWindow
	{
        // ACTIONS
        
        public Action OnPauseClick;

        
        
		// I_UI_WINDOW

		public override void Init()
		{

			base.Init();
			AssertSerializeFields();
           

        }

		public override void Show()
		{
           

            WindowViewHandler.Show(_canvasGroup);
            

            //Debug.Log("SHOW");
            //
            WorkOnShow();

        }

        public override void Hide()
		{
			WindowViewHandler.Hide(_canvasGroup);

			//
			WorkOnHide();
		}

        // Button 
       


        // I_GAME_WINDOW

        #region I_GAME_WINDOW_INTERFAECS

        public Text Get_TimerText()
		{
			return timerText;
		}

		public void Set_TimerText(string value)
		{
			timerText.text = value;
		}

		public Button Get_PauseButton()
		{
			return pauseButton;
		}

		public int Get_TriesCount()
		{
			return triesCount;
		}

		public void Set_TriesCount(int value)
		{
			triesCount = value;
		}

		public void Set_TriesSpriteListValue(int id, Sprite sprite)
		{
			triesSprite[id].sprite = sprite;
		}

		public Sprite Get_SpriteLostTrie()
		{
			return lostTrie;
		}

		public Sprite Get_SpriteTrie()
		{
			return trie;
		}

		public Text Get_TextRaceTimer()
		{
			return _textRaceTimer;
		}

		#endregion



		// INTERFACES

		// used on button !
		public void Pause()
		{
            if( Time.timeScale == 0 )
            {
                return;
            }

            if (!IsCanClickPause()) {
				return; }

			//
			if (OnPauseClick != null) {
				OnPauseClick(); }
		}

        // METHODS

        private void AssertSerializeFields()
		{
			//
			Assert.AreNotEqual(null, _uiPanelOld);
			Assert.IsTrue(_uiPanelOld is UiPanelOld);

			//
			Assert.AreNotEqual(null, _uiPanelNew);
			Assert.IsTrue(_uiPanelNew is UiPanelNew);
		}

		private bool IsCanClickPause()
		{
			return
				(_gameManager.gameData.GameType == GameTypeEnum.TutorialGame);
		}



	}
}