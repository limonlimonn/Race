using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

using HCR.Gameplay;
using HCR.Interfaces;

namespace HCR
{
	/// <summary>
	/// Абстрактный базовый класс - для финишных окон, где есть отправка смайлов
	/// </summary>

	public abstract class ABaseFinishRecordRaceWithSmiles : MonoBehaviour
	{
		// FIELDS

		#region VARIABLES

		// ----------------------------------------------------
		//
		[SerializeField] protected CanvasGroup _canvasGroup;
		[SerializeField] protected Text _textRaceTime;

		// ----------------------------------------------------
		//
		[Header("SMILES_BIG")]
		[SerializeField] protected Image _imageSmileBig_0;
		[SerializeField] protected Image _imageSmileBig_1;
		[SerializeField] protected Image _imageSmileBig_2;

		protected List<Image> _smilesBigImages;

		// ----------------------------------------------------
		//
		protected List<int> _idSmilesToSend;

		#endregion

		// dependences
		protected GameData _gameData;

		protected UIManager _uiManager;
		protected SmilesContainer _smilesContainer;



		// METHODS

		protected void InitVariables()
		{
			_uiManager = Core.Instance.GetService<UIManager>();
			Assert.AreNotEqual(null, _uiManager);

			_smilesContainer = _uiManager.Get_SmilesContainer();
			Assert.AreNotEqual(null, _smilesContainer);

			//
			_smilesBigImages = new List<Image>();
			_smilesBigImages.Add(_imageSmileBig_0);
			_smilesBigImages.Add(_imageSmileBig_1);
			_smilesBigImages.Add(_imageSmileBig_2);

			//
			_idSmilesToSend = new List<int>();
		}

		protected void ShowSmilesBig()
		{
			ShowSmileBigById(0);
			ShowSmileBigById(1);
			ShowSmileBigById(2);
		}

		private void ShowSmileBigById(int id)
		{
			int idSprite = _idSmilesToSend[id];
			Sprite smileBig = _smilesContainer.GetSmileSprite(idSprite);

			_smilesBigImages[id].sprite = smileBig;
			_smilesBigImages[id].SetNativeSize();
		}

		// abstract methods
		protected abstract void ShowDefaultSmiles();

		protected void ShowRaceTime(IGameWindow gameWindow)
		{
			_textRaceTime.text = gameWindow.Get_TextRaceTimer().text;
		}



	}
}