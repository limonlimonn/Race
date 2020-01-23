using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HCR
{
	public abstract class ABaseUiPanel : MonoBehaviour
	{
		// FIELDS

		#region VARIABLES

		[Space]
		[SerializeField] protected List<CanvasGroup> nitroImages;

		[Space]
		[SerializeField] protected Text timerText;						// 1 - 2 - 3 in Race start

		[Space]
		[SerializeField] protected List<Image> triesSprite;

		[Space]
		[SerializeField] protected Sprite lostTrie;
		[SerializeField] protected Sprite trie;

		[SerializeField] protected Text meterCountLabel;
        [SerializeField] protected Text enemyCountMeter;
        [SerializeField] protected Image enemyCountImage;
        [SerializeField] protected Text currentSpeedLabel;


        [SerializeField] protected Sprite leaderSprite;
        [SerializeField] protected Sprite looserSprite;
        [SerializeField] protected Sprite crashedSprite;

        [SerializeField] protected Button pauseButton;

		[SerializeField] protected GameObject rotateCircleF;
		[SerializeField] protected GameObject rotateCircleB;
		[SerializeField] protected GameObject rotateCircleUP;
		[SerializeField] protected GameObject rotateCircleDwn;

		[SerializeField] protected Transform cursore;

		[SerializeField] protected Slider nitro;
		[SerializeField] protected CanvasGroup nitroButtonImage;
		[SerializeField] protected Image slider;					// nitro bar

		[Space]
		[SerializeField] protected List<CanvasGroup> panelsList;

		[Space]
		[SerializeField] protected GameObject podium;				// empty field in Inspector !

		[SerializeField] protected Text textRaceTimer;
		[SerializeField] protected Text textJewels;
		[SerializeField] protected Text textGold;
        [SerializeField] protected GameObject _ImageJewels;
        [SerializeField] protected GameObject _ImageGold;
        [SerializeField]
        protected Image _forward;
        [SerializeField] protected Sprite forwardOnSprite;
        [SerializeField] protected Sprite forwardOffSprite;
        #endregion



        // UNITY

        protected virtual void Awake()
		{
			AssertVariables();
		}




		// INTERFACES

		// =========================================================
		//
		public List<CanvasGroup> Get_NitroImages()
		{
			return nitroImages;
		}

		// =========================================================
		//
		public Text Get_TimerText()
		{
			return timerText;
		}

		// =========================================================
		//
		public List<Image> Get_TriesSprite()
		{
			return triesSprite;
		}

		// =========================================================
		//
		public Sprite Get_LostTrie()
		{
			return lostTrie;
		}

		public Sprite Get_Trie()
		{
			return trie;
		}

		// =========================================================
		//
		public Text Get_MeterCountLabel()
		{
			return meterCountLabel;
		}

        public Text Get_EnemyCountMeter()
        {
            return enemyCountMeter;
        }


        public Text Get_CurrentSpeedLabel()
		{
			return currentSpeedLabel;
		}

        public Image Get_EnemyCountImage()
        {
            return enemyCountImage;
        }
        

        public Sprite Get_LeaderSprite()
        {
            return leaderSprite;
        }

        public Sprite Get_LooserSprite()
        {
            return looserSprite;
        }

        public Sprite Get_CrashedSprite()
        {
            return crashedSprite;
        }
        // =========================================================
        //
        public Button Get_PauseButton()
		{
			return pauseButton;
		}

        public Image Get_ForwardButton()
        {
            return _forward;
        }

		// =========================================================
		//
		public void Set_RotateCircle_F(bool active)
		{
            rotateCircleF.SetActive(active);
		}

		public void Set_RotateCircle_B(bool active)
		{
			 rotateCircleB.SetActive(active);
        }

		public void Set_RotateCircle_Up(bool active)
		{
			 rotateCircleUP.SetActive(active);
        }

		public void Set_RotateCircle_Down(bool active)
		{
            rotateCircleDwn.SetActive(active);
		}

		// =========================================================
		//
		public Transform Get_Cursore()
		{
			return cursore;
		}

		// =========================================================
		//
		public Slider Get_Nitro()
		{
			return nitro;
		}

		public CanvasGroup Get_NitroButtonImage()
		{
			return nitroButtonImage;
		}

		public Image Get_Slider()
		{
			return slider;
		}

		// =========================================================
		//
		public List<CanvasGroup> Get_PanelsList()
		{
			return panelsList;
		}

		// =========================================================
		//
		public GameObject Get_Podium()
		{
			return podium;
		}

		// =========================================================
		//
		public Text Get_TextRaceTimer()
		{
			return textRaceTimer;
		}

		public Text Get_TextJewels()
		{
			return textJewels;
		}

		public Text Get_TextGold()
		{
			return textGold;
		}

        public GameObject Get_Jewels()
        {
            return _ImageJewels;
        }
        public GameObject Get_Gold()
        {
            return _ImageGold;
        }
        // =========================================================



        // METHODS

        private void AssertVariables()
		{
			//
			Assert.AreNotEqual(null, nitroImages);
			Assert.IsTrue(nitroImages.Count > 0);

			//
			Assert.AreNotEqual(null, timerText);

			//
			Assert.AreNotEqual(null, triesSprite);
			Assert.IsTrue(triesSprite.Count > 0);

			//
			Assert.AreNotEqual(null, lostTrie);
			Assert.AreNotEqual(null, trie);

			Assert.AreNotEqual(null, meterCountLabel);
            Assert.AreNotEqual(null, enemyCountMeter);
            Assert.AreNotEqual(null, currentSpeedLabel);

            Assert.AreNotEqual(null, enemyCountImage);
            Assert.AreNotEqual(null, looserSprite);
            Assert.AreNotEqual(null, leaderSprite);
            Assert.AreNotEqual(null, crashedSprite);

            Assert.AreNotEqual(null, pauseButton);

			Assert.AreNotEqual(null, rotateCircleF);
			Assert.AreNotEqual(null, rotateCircleB);
			Assert.AreNotEqual(null, rotateCircleUP);
			Assert.AreNotEqual(null, rotateCircleDwn);

			Assert.AreNotEqual(null, cursore);

			Assert.AreNotEqual(null, nitro);
			Assert.AreNotEqual(null, nitroButtonImage);
			Assert.AreNotEqual(null, slider);

			//
			Assert.AreNotEqual(null, panelsList);
			Assert.IsTrue(panelsList.Count > 0);

			//
			// --- podium --- ???
			Assert.AreNotEqual(null, textRaceTimer);
			Assert.AreNotEqual(null, textJewels);
			Assert.AreNotEqual(null, textGold);
		}

        public void NitroPressed()
        {
            _forward.sprite = forwardOnSprite;
        }

        public void NitroUnPressed()
        {
            _forward.sprite = forwardOffSprite;
        }

        public void ForwardPressed()
        {

        }

        public void ForawrdUnPressed()
        {

        }

    }
}