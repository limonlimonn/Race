using System;
using HCR.Interfaces;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HCR.Gameplay
{
	/// <summary>
	/// Абстрактный базовый класс - "Окно пауза" с кнопками: Продолжить / Заново (-1) / В Меню
	/// </summary>

	public abstract class ABasePauseWindow : MonoBehaviour
	{
		// ACTIONS

		
		
		public Action OnCoiseAnyCar;
        public Action OnMenuClick;

        // FIELDS

        [SerializeField] protected CanvasGroup _canvasGroup;

		// dependences

		// METHODS

		protected void InitDependences()
		{
			
		}

		#region PAUSE

		protected void SetPauseOn()
		{
			Time.timeScale = 0;
            Core.Instance.Mute();
		}

		protected void SetPauseOff()
		{
			Time.timeScale = 1;
            Core.Instance.UnMute();
        }

		#endregion



	}
}