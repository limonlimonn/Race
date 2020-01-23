using UnityEngine;
using UnityEngine.Assertions;

using HCR.Enums;
using HCR.Gameplay;

using _AsyncMulty = HCR.Gameplay.AsyncMultiplayer;
using _Single = HCR.Gameplay.Singleplayer;

namespace HCR
{
	public class MoneyControler : MonoBehaviour
	{
		// FIELDS
		public string moneyName;
		public float rotation = 100f;
		public float frequency = 10f;
		public float amplitude = 1f;

		private Renderer render;
		private Vector3 basePos;
		private bool hitOrder = true;

		// dependences
		private SafePlayerPrefs _safePlayerPrefs;
		private GameManager _gameManager;

		private UIManager _uiManager;
		private ABaseGameWindow _gameWindow;
        private AudioService _audioService;

        public string AudioPath = "event:/Action/Coin_take";

        // UNITY

        private void Start()
		{
            basePos = transform.position;
            render = GetComponent<Renderer>() as Renderer;
            InitVariables();
            if (Core.Instance.isTestCore) { return; }

			
			ResetPlayerSiverAndGold();
            
            //
            
		}

		private void Update()
		{
			if (render.isVisible)
			{
				float curPosY = basePos.y + Mathf.PingPong(Time.time, amplitude);
				Vector3 targetPos = new Vector3(basePos.x, curPosY, basePos.z);
				transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * frequency);
				transform.Rotate(Vector3.up * Time.deltaTime * rotation);
			}
		}

		private void OnTriggerStay (Collider hit)
		{
			if ( hitOrder &&
			     (hit.tag == "BodyCollider" || hit.tag == "RoofCollider") )
			{
				hitOrder = false;
				CheckCollectedCoin();
                _audioService.RM_PlayOneShot(AudioPath);

                Destroy(gameObject, 0);
			}
		}



		// METHODS

		private void InitVariables()
		{
			_safePlayerPrefs = Core.Instance.GetService<SafePlayerPrefs>();
			Assert.AreNotEqual(null, _safePlayerPrefs);

			_gameManager = Core.Instance.GetService<GameManager>();
			Assert.AreNotEqual(null, _gameManager);

			_uiManager = Core.Instance.GetService<UIManager>();
			Assert.AreNotEqual(null, _uiManager);

            _audioService = Core.Instance.GetService<AudioService>();

            //
            var gameWindowHandler = new GameWindowResolver();
			_gameWindow = gameWindowHandler.GetGameWindow();
		}

		private void ResetPlayerSiverAndGold()
		{
			_safePlayerPrefs.SaveEarnedJewels(0);
			_safePlayerPrefs.SaveEarnedGold(0);
            _gameWindow.DeActiveJewels();
        }

		private void CheckCollectedCoin()
		{
            if (Core.Instance.isTestCore) { return; }
            if (moneyName == "PlrJewels")
			{
                _gameWindow.ActiveJewels();
                _safePlayerPrefs.AddEarnedJewels(1);
				_gameWindow.UpdateTextJewels();
			}
			else if (moneyName == "PlrGold")
			{
				_safePlayerPrefs.AddEarnedGold(1);
				_gameWindow.UpdateTextGold();
			}
			else
			{
				#region DEBUG
#if UNITY_EDITOR
				Debug.Log("[ERROR] collected coin with strange name = " + moneyName);
#endif
				#endregion
			}
		}

    }
}
