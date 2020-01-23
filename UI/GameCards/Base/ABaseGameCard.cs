using System.Collections.Generic;
using HCR.Gameplay;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HCR
{
	public abstract class ABaseGameCard : MonoBehaviour
	{
		// FIELDS

		protected string _gameID;
        protected GameData _gameData;

		// ------------------------------------------
		//
		[Header("SMILES")]
		[SerializeField] protected Image _imageSmile_0;
		[SerializeField] protected Image _imageSmile_1;
		[SerializeField] protected Image _imageSmile_2;

		protected List<Image> _smilesMessage;

		// dependences
		protected UIManager _uiManager;
		protected SmilesContainer _smilesContainer;



		// INTERFACES

		public void Set_GameID(GameData gameData)
		{
			// pre-conditions
			Assert.IsTrue(gameData.gameId != "");

			_gameID = gameData.gameId;
            _gameData = gameData;
        }

		public void FillMessageWithSmiles(List<int> idSmiles)
		{
			_uiManager = Core.Instance.GetService<UIManager>();
			_smilesContainer = _uiManager.Get_SmilesContainer();

			//
			_smilesMessage = new List<Image>();
			_smilesMessage.Add(_imageSmile_0);
			_smilesMessage.Add(_imageSmile_1);
			_smilesMessage.Add(_imageSmile_2);

			FillSmiles(idSmiles);
		}



		// METHODS

		private void FillSmiles(List<int> idSmiles)
		{
			for (int i = 0; i < idSmiles.Count; i++)
			{
				Sprite spriteSmile = _smilesContainer.GetSmileSprite(idSmiles[i]);
				_smilesMessage[i].sprite = spriteSmile;
				_smilesMessage[i].SetNativeSize();
			}
		}



	}
}