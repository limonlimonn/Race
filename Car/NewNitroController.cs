using UnityEngine;
using System;
using FMODUnity;
using System.Collections;
using UnityEngine.Assertions;
using HCR.Event;
using HCR.Event.UIControl;

namespace HCR
{
	public class NewNitroController : MonoBehaviour
	{

		public KeyCode key = KeyCode.N;
        //[HideInInspector]
        //public Slider nitroSlider;
        private float emitterNitro;

        public float maxVelocity = 30.0f;
		public float nitroPower = 5000f;

		public float NitroDuration = 1f;
		public float NitroRecovery = 10f;
		public float NitroDelay;

		Rigidbody m_rigidbody;
		private float nitroPlus = 1f;
		[HideInInspector]
		public float maxNitro;
		private float nitroMinus = 1f;
		[HideInInspector]
		public float nitroCurrent;

		private float nitroDelayCurrent = 0f;
		private float nitroDelayMax;

        private float groundModificator;
        [HideInInspector]
		public bool nitroEnable = true;
        public int IsNitroPressed = 0;

        private AudioService _audioService;

        //private GameWindow _gameView;

        // Use this for initialization

        private CarBase _playerCar;
        private ButtonControl buttonControl;

		public void Init (CarBase car)
		{
            _audioService = Core.Instance.GetService<AudioService>();
            Assert.AreNotEqual(null, _audioService);

            #region createing nitro audio emitter

            _audioService.CreateEmitterNitro();
            #endregion
            _audioService.CreateEmitterNitroEmpty();

            _playerCar = car;

            maxNitro = (float)Math.Round(NitroDuration / Time.fixedDeltaTime, 0);
            nitroPlus = (float)Math.Round(nitroMinus / NitroRecovery, 2);

            nitroDelayMax = (float)Math.Round(NitroDelay / Time.fixedDeltaTime, 0);

            m_rigidbody = GetComponent<Rigidbody>();

            nitroCurrent = maxNitro;

            
        }

        private void InitButtonEvent()
        {
            buttonControl = EventManager._init.Game.ButtonControl;
            buttonControl.OnNitroPressed += PressNitro;
            buttonControl.OnNitroUnPressed += UnPressNitro;
        }

        private void PressNitro()
        {
            PressNitro(true);
        }

        private void UnPressNitro()
        {
            PressNitro(false);
        }

		private void PressNitro(bool pressed)
		{
			uiNitroPressed = pressed;
		}



		bool uiNitroPressed = false;

		bool CheckInput()
		{
			return Input.GetKey(key) || uiNitroPressed;
		}

		void FixedUpdate()
		{

            if (nitroEnable)
			{
               
                emitterNitro = ( 1 - nitroCurrent / maxNitro) * 100;
                //Debug.Log(emitterNitro);
                _audioService.EmitterSetParameter("nitro",  (int)emitterNitro);

                if (nitroCurrent < 0)
				{
					nitroEnable = false;
                    IsNitroPressed = 0;
                    nitroDelayCurrent = 0;

					stopParticle();

				}

				else if (nitroCurrent > 0 && CheckInput() )
				{
                    AddSpeed();
                    IsNitroPressed = 1;
                    nitroCurrent = nitroCurrent - nitroMinus;

					//nitroSlider.value = nitroCurrent;

					startParticle();

				}

				else if (nitroCurrent < maxNitro)
				{
                    nitroCurrent = nitroCurrent + nitroPlus;
                    //nitroSlider.value = nitroCurrent;
                    IsNitroPressed = 0;
                    stopParticle();
                }
                else
                {
                    IsNitroPressed = 0;
                }
			}
			else
			{
                stopParticle();
                IsNitroPressed = 0;
                if (nitroCurrent > 0)
                {
                    nitroEnable = true;
                }

                if (nitroDelayCurrent < nitroDelayMax)
				{
					nitroDelayCurrent += nitroMinus;
				}
				else if(nitroDelayCurrent >= nitroDelayMax)
				{
					nitroEnable = true;
					nitroCurrent = 1;
				}

			}

		}

		public void AddSpeed()
		{
            if (_playerCar.controller.wheelData[0].collider.isGrounded
               && _playerCar.controller.wheelData[3].collider.isGrounded
                )
            {
                groundModificator = 1;
            }
            else
            {
                groundModificator = 0.8f;
            }

			_playerCar.controller.throttleInput = 1f;
            int sign = -1;
            float gradus = m_rigidbody.transform.eulerAngles.x;
			double radian = gradus * Math.PI / 180;

       

            if (Mathf.Abs(_playerCar.transform.rotation.x) < 0.5f){
                sign = -1;
            } else {
                sign = 1;
            }
			double sin = Math.Sin(radian);
			double cos = Math.Cos(radian);
			float nitroY = nitroPower * (float)sin;
			float nitroX = nitroPower * (float)cos;

            var vector3 = new Vector3(sign*nitroX, -nitroY, 0);

			m_rigidbody.AddForce(vector3 * groundModificator, ForceMode.Force); // instant with mass
		}

		public void startParticle()
		{
            if (!_audioService.emitter.IsPlaying())
            {
                if (_playerCar.isPlayer)
                {
                    _audioService.EmitterNitroPlay();
                }
            }
            for (int i = 0; i < _playerCar.nitroParticles.Count; i++)
			{
				_playerCar.nitroParticles[i].enableEmission = true;
			}
		}

		public void stopParticle() {

            if (_playerCar.nitroParticles[0].enableEmission == false) return;

            if (_playerCar.isPlayer)
            {
                _audioService.EmitterNitroStop();
            }
            if (nitroCurrent < 0)
            {


           _audioService.EmitterNitroEmptyPlay();


            }

            for (int i = 0; i < _playerCar.nitroParticles.Count; i++)
			{
				_playerCar.nitroParticles[i].enableEmission = false;
			}
		}

        IEnumerator playNitroEmpty()
        {
            yield return new WaitForSeconds(1);
            
        }

        public void stopSound()
        {
            nitroEnable = false;
            stopParticle();
            _audioService.EmitterNitroStop();
            _audioService.EmitterNitroEmptyStop();
        }

        public void destroySound()
        {

            stopSound();
            nitroEnable = false;
           // Destroy(emitter);
            //Destroy(emitterNitroEmpty);
        }
    }
}