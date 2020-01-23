using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HCR
{
	public class GameCard : ABaseGameCard
	{
		// ACTIONS

		public Action<string> OnPlayClick;

		// FIELDS

		[Header("CARD")]
		public Text enemyName;
		public Text buttonStatus;
		public Text youScoreLabel;
		public Text enemyScoreLabel;

		public GameObject playButton;
        public GameObject ContinueButton;
        public GameObject waitButton;

		// --------------------------------
		//
		[SerializeField] private Text _textMapName;
		[SerializeField] private Text _textRaceDate;



        // UNITY

        private void Start()
		{
			AssertSerialiedFields();
            //ContinueButton.SetActive(false);

        }



		// INTERFACES

		#region GETTERS/SETTERS

		public void Set_MapName(string mapName)
		{
			// pre-conditions
			Assert.IsTrue(mapName != "");

			_textMapName.text = mapName;
		}

		public void Set_TextRaceDate(string raceDate)
		{
			// pre-conditions
			Assert.IsTrue(raceDate != "");

			_textRaceDate.text = raceDate;
		}

		#endregion

		public void SetRecordView()
		{
			playButton.SetActive(false);
			waitButton.SetActive(true);
            ContinueButton.SetActive(false);
            buttonStatus.text = "WAIT FOR TURN";
		}

		public void SetReplayView()
		{
			playButton.SetActive(true);
			waitButton.SetActive(false);
            ContinueButton.SetActive(false);
            buttonStatus.text = "PLAY";
		}

		// used on button !
		public void PlayGame()
		{
			// pre-condition
			Assert.IsTrue(_gameID != "");

			if (OnPlayClick != null) {
				OnPlayClick(_gameID); }
		}

        public void ContinueGame()
        {
            // pre-condition
            Assert.IsTrue(_gameID != "");

            if (OnPlayClick != null)
            {
                OnPlayClick(_gameID);
            }
        }

        public void Set_TextButtonPlay(int but, string text)
        { 
            if(but == 2)
            {
                playButton.SetActive(false);
                ContinueButton.SetActive(true);
                waitButton.SetActive(false);
                buttonStatus.text = text;
            }
            
        }




        // METHODS

        private void AssertSerialiedFields()
		{
			Assert.AreNotEqual(null, _textMapName);
			Assert.AreNotEqual(null, _textRaceDate);
		}



	}
}