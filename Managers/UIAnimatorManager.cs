using System;
using System.Collections;
using DG.Tweening;
using HCR.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace HCR
{
	public class UIAnimatorManager : MonoBehaviour, IService
	{
        // FIELDS
        private AudioService _audioService;
        public string audioPath;
        public int audioDevider = 3;

		#region VARIABLES

		private bool _isUpdateAnimation;
		private bool _isTapDuringUpdateAnimation;

        private object IdScaleInt;
        private object IdScaleFloat;

        #endregion


        void PlayAudioOn()
        {
            Init();
            _audioService.RM_PlayOneShot(audioPath);
        }
        // UNITY

        private void Update()
		{
			CheckTapDuringUpdateAnimation();
		}



		// I_SERVICE

		public void Init()
		{
            _audioService = Core.Instance.GetService<AudioService>();
		}



		// INTERFACES

		public void ShowTextFieldUpdateAniamtion(Text textField, int oldValue, int newValue, Action OnComplete = null)
		{
//		Debug.Log("=============================================");
//		Debug.Log(" TEXT FIELD >> " + textField + " | " +
//		          "  oldValue = " + oldValue + " | " +
//		          "newValue = " + newValue);

			_isUpdateAnimation = true;

			Vector2 textFieldOriginalScale = textField.rectTransform.localScale;
			Vector2 textFieldNewScale = new Vector2(0.9f, 1.2f);

            // tweener - change silver text size
            var seq = DOTween.Sequence();
            seq.Append(textField.transform.DOScale(textFieldNewScale, 0.04f)
                .SetEase(Ease.Linear)
                .SetLoops(9999, LoopType.Yoyo));
            seq.SetId("IdScaleInt" + textField.GetHashCode());
            // animation textSilver change
            Coroutine corut = Core.Instance.StartCor(
				UpdateTextField( textField, textFieldOriginalScale,
					oldValue, newValue, OnComplete) );

            //Debug.Log("GetHashCode : " + corut.GetHashCode());

        }

		// float !!!
		public void ShowTextFieldUpdateAniamtion(Text textField, float oldValue, float newValue, Action OnComplete = null)
		{
//		Debug.Log("=============================================");
//		Debug.Log(" TEXT FIELD >> " + textField + " | " +
//		          "  oldValue = " + oldValue + " | " +
//		          "newValue = " + newValue);

			_isUpdateAnimation = true;
            
            Vector2 textFieldOriginalScale = textField.rectTransform.localScale;
			Vector2 textFieldNewScale = new Vector2(0.9f, 1.2f);

            // tweener - change silver text size
            var seq = DOTween.Sequence();
            seq.Append(textField.transform.DOScale(textFieldNewScale, 0.04f)
                .SetEase(Ease.Linear)
                .SetLoops(9999, LoopType.Yoyo));
            seq.SetId("IdScaleFloat" + textField.GetHashCode());
            // animation textSilver change
           Coroutine corut = Core.Instance.StartCor(
				UpdateTextField( textField, textFieldOriginalScale,
					oldValue, newValue, OnComplete) );
		}





		// METHODS

		#region TEXT_FIELDS_ANIMATIONS

		private IEnumerator UpdateTextField(Text textField, Vector2 textFieldOriginalScale,
			int oldValue, int newValue, Action OnComplete = null)
		{
            
            float animationDuration = 45f;

			if (newValue == 0)
			{
				OnCompleteUpdateAnimation(textField, textFieldOriginalScale, newValue, OnComplete);
				yield return null;
			}

			// ------------------------------------------------------------------------------------
			//
			if (oldValue > newValue)
			{
                

                float delta = oldValue - newValue;
				int subtraction = (int) Mathf.Round(delta / animationDuration);

				if (subtraction <= 0) {
					subtraction = 1; }

				int resultIterator = 0;

				while (delta > (resultIterator * subtraction) )
				{
					resultIterator++;
					textField.text = (oldValue - resultIterator * subtraction).ToString();

					if (_isTapDuringUpdateAnimation) {
						OnStopAnimationOnTap(textField, textFieldOriginalScale, newValue); }
                   
                    yield return new WaitForFixedUpdate();
                    if (resultIterator % audioDevider == 0)
                    {
                        PlayAudioOn();
                    }
                }

				OnCompleteUpdateAnimation(textField, textFieldOriginalScale, newValue, OnComplete);
			}

			// ------------------------------------------------------------------------------------
			//
			if (oldValue < newValue)
			{
				float delta = newValue - oldValue;
				int adding = (int) Mathf.Round(delta / animationDuration);

				if (adding <= 0) {
					adding = 1; }

				int resultIterator = 0;

				while (delta > (resultIterator * adding))
				{
					resultIterator++;
					textField.text = (oldValue + resultIterator * adding).ToString();

					if (_isTapDuringUpdateAnimation) {
						OnStopAnimationOnTap(textField, textFieldOriginalScale, newValue); }
                    
                   
                    yield return new WaitForFixedUpdate();
                    
                    if(resultIterator % audioDevider == 0)
                    {
                        PlayAudioOn();
                    }
                    
                }

				OnCompleteUpdateAnimation(textField, textFieldOriginalScale, newValue, OnComplete);
			}

			// ------------------------------------------------------------------------------------

		}

		// float !!!
		private IEnumerator UpdateTextField(Text textField, Vector2 textFieldOriginalScale,
			float oldValue, float newValue, Action OnComplete = null)
		{
			float animationDuration = 45f;

			if (newValue == 0)
			{
				OnCompleteUpdateAnimation(textField, textFieldOriginalScale, newValue, OnComplete);
				yield return null;
			}

			// ------------------------------------------------------------------------------------
			//
			if (oldValue > newValue)
			{
				float delta = oldValue - newValue;
				int subtraction = (int) Mathf.Round(delta / animationDuration);

				if (subtraction <= 0) {
					subtraction = 1; }

				int resultIterator = 0;

				while (delta > (resultIterator * subtraction) )
				{
					resultIterator++;
					textField.text = (oldValue - resultIterator * subtraction).ToString("f1");

					if (_isTapDuringUpdateAnimation) {
						OnStopAnimationOnTap(textField, textFieldOriginalScale, newValue); }

					yield return new WaitForFixedUpdate();
				}

				OnCompleteUpdateAnimation(textField, textFieldOriginalScale, newValue, OnComplete);
			}

			// ------------------------------------------------------------------------------------
			//
			if (oldValue < newValue)
			{
				float delta = newValue - oldValue;
				int adding = (int) Mathf.Round(delta / animationDuration);

				if (adding <= 0) {
					adding = 1; }

				int resultIterator = 0;

				while (delta > (resultIterator * adding))
				{
					resultIterator++;
					textField.text = (oldValue + resultIterator * adding).ToString("f1");

					if (_isTapDuringUpdateAnimation) {
						OnStopAnimationOnTap(textField, textFieldOriginalScale, newValue); }

					yield return new WaitForFixedUpdate();
				}

				OnCompleteUpdateAnimation(textField, textFieldOriginalScale, newValue, OnComplete);
			}

			// ------------------------------------------------------------------------------------

		}



		private void CheckTapDuringUpdateAnimation()
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (_isUpdateAnimation) {
					_isTapDuringUpdateAnimation = true; }
			}
		}




		private void OnCompleteUpdateAnimation(Text textField, Vector2 textFieldOriginalScale, int newValue, Action OnComplete = null)
		{
			StopUpdateAnimation(textField, textFieldOriginalScale, newValue, OnComplete);
		}

		// float !!!
		private void OnCompleteUpdateAnimation(Text textField, Vector2 textFieldOriginalScale, float newValue, Action OnComplete = null)
		{
			StopUpdateAnimation(textField, textFieldOriginalScale, newValue, OnComplete);
		}




		private void OnStopAnimationOnTap(Text textField, Vector2 textFieldOriginalScale, int newValue)
		{
			StopUpdateAnimation(textField, textFieldOriginalScale, newValue);
		}

		// float !!!
		private void OnStopAnimationOnTap(Text textField, Vector2 textFieldOriginalScale, float newValue)
		{
			StopUpdateAnimation(textField, textFieldOriginalScale, newValue);
		}



		private void StopUpdateAnimation(Text textField, Vector2 textFieldOriginalScale, int newValue, Action OnComplete = null)
		{
			textField.text = newValue.ToString();
            _isUpdateAnimation = false;
			_isTapDuringUpdateAnimation = false;
            //DOTween.KillAll();
            //Debug.Log(textField.name + " 1: " + textField.GetHashCode());
            DOTween.Kill("IdScaleInt"+ textField.GetHashCode(), true);
            textField.rectTransform.localScale = textFieldOriginalScale;
            //DOTween.Play("gold");
            StopAllCoroutines();

			// ------------------------------------------------
			//
			if (OnComplete != null) {
				OnComplete(); }
		}

		// float !!!
		private void StopUpdateAnimation(Text textField, Vector2 textFieldOriginalScale, float newValue, Action OnComplete = null)
		{
			textField.text = newValue.ToString("f1");
			_isUpdateAnimation = false;
			_isTapDuringUpdateAnimation = false;
            //DOTween.KillAll();
            //Debug.Log(textField.name + " 2: " + textField.GetHashCode());
            DOTween.Kill("IdScaleFloat" + textField.GetHashCode(), true);
            textField.rectTransform.localScale = textFieldOriginalScale;
            //DOTween.Play("gold");
            StopAllCoroutines();

			// ------------------------------------------------
			//
			if (OnComplete != null) {
				OnComplete(); }
		}

		#endregion



	}
}