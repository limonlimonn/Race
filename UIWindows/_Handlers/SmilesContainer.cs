using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace HCR.Gameplay
{
	/// <summary>
	/// Класс-контайнер для хранения смайлов, которые будут использованы в поле
	/// для отправки в конце заезда
	/// </summary>

	public class SmilesContainer : MonoBehaviour
	{
		// FIELDS

		#region VARIABLES

		// ----------------------------------------
		//
		[Header("SMILES")]
		[SerializeField] private Sprite _smile_0;
		[SerializeField] private Sprite _smile_1;
		[SerializeField] private Sprite _smile_2;
		[SerializeField] private Sprite _smile_3;
		[SerializeField] private Sprite _smile_4;
		[SerializeField] private Sprite _smile_5;
		[SerializeField] private Sprite _smile_6;
		[SerializeField] private Sprite _smile_7;

		[SerializeField] private Sprite _smileNone;

		// ----------------------------------------
		//
		private Dictionary<int, Sprite> _dictSmiles;

		#endregion



		public void Init()
		{
			AssertVariables();
			InitDictionarySmiles();
		}



		// INTERFACES

		public Sprite GetSmileSprite(int id)
		{
			// pre-conditions
			Assert.IsTrue( (id >= 0 && id < _dictSmiles.Count) || (id == 999),
				"[ERROR] id must be ( >= 0 && < _dictSmiles.Count ) || id = 999 !!!");

			Sprite smile = _dictSmiles[id];

			return smile;
		}

		public Sprite GetSmileSpriteNone()
		{
			return _smileNone;
		}



		// METHODS

		private void AssertVariables()
		{
			Assert.AreNotEqual(null, _smile_0);
			Assert.AreNotEqual(null, _smile_1);
			Assert.AreNotEqual(null, _smile_2);
			Assert.AreNotEqual(null, _smile_3);
			Assert.AreNotEqual(null, _smile_4);
			Assert.AreNotEqual(null, _smile_5);
			Assert.AreNotEqual(null, _smile_6);
			Assert.AreNotEqual(null, _smile_7);

			Assert.AreNotEqual(null, _smileNone);
		}

		private void InitDictionarySmiles()
		{
			_dictSmiles = new Dictionary<int, Sprite>();

			_dictSmiles.Add(0, _smile_0);
			_dictSmiles.Add(1, _smile_1);
			_dictSmiles.Add(2, _smile_2);
			_dictSmiles.Add(3, _smile_3);
			_dictSmiles.Add(4, _smile_4);
			_dictSmiles.Add(5, _smile_5);
			_dictSmiles.Add(6, _smile_6);
			_dictSmiles.Add(7, _smile_7);

			_dictSmiles.Add(999, _smileNone);
		}



	}
}