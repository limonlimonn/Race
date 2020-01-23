using System.Collections.Generic;

namespace HCR.Gameplay
{
	/// <summary>
	/// Класс-помощник для добавления больших смайлов в поле для отправки записи сыгранной игры
	/// </summary>

	class SmileSwapHandler
	{
		// FIELDS

		private List<int> _smiles;
		private const int _idSmileNone = 999;

		private const int _maxSmilesCount = 3;
		private int _counterOfSmiles;



		// CONSTRUCTOR

		public SmileSwapHandler()
		{
			SetSmilesDefaultValues();
		}



		// INTERFACES

		public List<int> GetDefaultSmiles()
		{
			SetSmilesDefaultValues();

			return _smiles;
		}

		public List<int> GetSmiles()
		{
			return _smiles;
		}

		public List<int> AddSmilesToList(int id)
		{
			if (_counterOfSmiles < _maxSmilesCount)
			{
				AddElement(id);
			}
			else
			{
				AddNewElementAndRemoveFirstElement(id);
			}

			return _smiles;
		}



		// METHODS

		private void SetSmilesDefaultValues()
		{
			_smiles = new List<int>();

			_smiles.Add(_idSmileNone);
			_smiles.Add(_idSmileNone);
			_smiles.Add(_idSmileNone);
		}

		private void AddElement(int id)
		{
			_smiles[_counterOfSmiles] = id;
			_counterOfSmiles++;
		}

		private void AddNewElementAndRemoveFirstElement(int id)
		{
			int tempElement_1 = _smiles[1];
			int tempElement_2 = _smiles[2];

			_smiles[0] = tempElement_1;
			_smiles[1] = tempElement_2;
			_smiles[2] = id;
		}



	}
}