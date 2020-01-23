using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace HCR
{
	public class WindowViewHandler
	{



		// INTERFACES

		public static void Show(CanvasGroup canvasGroup)
		{
            // pre-conditions
            Assert.AreNotEqual(null, canvasGroup);

			canvasGroup.alpha = 1;
			canvasGroup.blocksRaycasts = true;
		}

		public static void Hide(CanvasGroup canvasGroup)
		{
			// pre-conditions
			Assert.AreNotEqual(null, canvasGroup);
            try
            {
                canvasGroup.alpha = 0;
                canvasGroup.blocksRaycasts = false;
            }
            catch (Exception ex)
            {

            }
		}



	}
}