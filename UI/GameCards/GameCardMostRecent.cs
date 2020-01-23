using HCR.Enums;
using HCR.Network;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HCR
{
	public class GameCardMostRecent : ABaseGameCard
	{
		// FIELDS

		[Header("CARD")]
		[SerializeField] private Text _textEnemyName;
		[SerializeField] private Text _textMapName;
		[SerializeField] private Text _textRaceDate;
        [SerializeField] private Text _textButtonJoin;

		// dependences
		private GameManager _gameManager;
        private SafePlayerPrefs _safePlayerPrefs;
        private EnternetWindow _enternetWindow;



        // UNITY

        private void Start()
		{
			AssertSerialiedFields();

			//
			_gameManager = Core.Instance.GetService<GameManager>();
			Assert.AreNotEqual(null, _gameManager);

            _safePlayerPrefs = Core.Instance.GetService<SafePlayerPrefs>();
            Assert.AreNotEqual(null, _safePlayerPrefs);

            _enternetWindow = Core.Instance.GetService<UIManager>().GetWindow(UIWindowEnum.IS_ENTERNET) as EnternetWindow;

        }



		// INTERFACES

		#region GETTERS/SETTERS

		public void Set_TetxEnemyName(string enemyName)
		{
			// pre-conditions
			Assert.IsTrue(enemyName != "");

			_textEnemyName.text = enemyName;
		}

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

        public void Set_TextButtonJoin(string text)
        {
            Assert.IsTrue(text != "");
            _textButtonJoin.text = text;
        }
		#endregion

		#region ON_BUTTON_CLICK

		// used on button !
		public void PlayGame()
		{
			// pre-condition
			Assert.IsTrue(_gameID != "");
            
                    Core.Instance.GetService<NetworkManager>().Join_Play(_gameID, Core.Instance.GetService<PlayerManager>().selectedCar, () =>
                    {
                        _safePlayerPrefs.SafeDataOnPlayClick(_gameData);
                        _gameManager.StartGame(_gameID);
                    },
                    (err)=>
                    {
                        if (err == "timeout")
                        {
                            _uiManager.ShowWindow(UIWindowEnum.SPLASH, false);
                            _enternetWindow.ShowErrorEnternet();
                        }
                        else
                        {
                            Debug.LogError("PlayGame err");
                        }
                    });
		}

		#endregion



		// METHODS

		private void AssertSerialiedFields()
		{
			Assert.AreNotEqual(null, _textEnemyName);
			Assert.AreNotEqual(null, _textMapName);
			Assert.AreNotEqual(null, _textRaceDate);
		}




	}
}