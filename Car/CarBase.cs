using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using EVP;
using UnityEngine.Assertions;

using HCR.Enums;

using FMODUnity;
using HCR.Gameplay.AsyncMultiplayer;
using HCR.Event;
using HCR.Event.UIControl;

namespace HCR
{
    public class CarBase : MonoBehaviour
    {
        // ACTIONS
        
        //public Action OnFinish;
       //public Action OnCrash;

        // FIELDS
        public bool IsFinish = false;
        public CarTypeEnum CarType { get; private set; }
        public string CarId { get; private set; }
        public Transform Transform { get { return _tr; } }
        private Transform _tr;
        public PlayerCarModel model;
        public Rigidbody Rigidbody { get { if (_rb == null) { _rb = GetComponent<Rigidbody>(); } return _rb; } }
        private Rigidbody _rb;
        public GameObject stayCube;

        
        private int meters;
        [HideInInspector]
        public int currentMeters;
        



        public NewNitroController nitro;
        public VehicleController controller;
        public VehicleStandardInput input;
        public InAirController airController;
        public DynamicSuspension suspension;
        public TricksController tricks;
        public Explosion explosion;
        public VehicleTireEffects tireEffects;

       





        public string engineEmitterName = "";
        public string engineParam1 = "";
        public string engineParam2 = "";


        [HideInInspector]
        public float wheelRadius;
        private bool _isSkidParticlesEnabled = true;
        //Color vars
        private Material mainMat;
        private Material colorMat;


        private List<MeshRenderer> colorMeshs = new List<MeshRenderer>();
        public bool enemyDown = false;
        [HideInInspector]
        public List<ParticleSystem> skidParticles = new List<ParticleSystem>();

        public List<ParticleSystem> nitroParticles = new List<ParticleSystem>();
        public List<ParticleSystem> smokeParticles = new List<ParticleSystem>();

        private float MassDiver;
        private WheelHit F_WH, R_WH;
        private WheelCollider F_WC, R_WC;
        public bool isPlayer = false;

        private Quaternion BaseQuan;
        private AudioService _audioService;
        private GameWindowAsyncMultiplayer _gameWindow;
        private UIManager _uiManager;
        private ButtonControl buttonControl;



        private float speedPecent;
        private float RPM;
        private float Load;
        private float gearPercent;
        private bool gearInAir = true;
        private float speedAirPercent;

        private int fixedUpdates = 66;
        private int fixedCount = 0;

        public void Awake()
        {
            Debug.Log("Awake BaseCar");
            _audioService = Core.Instance.GetService<AudioService>();
            Assert.AreNotEqual(null, _audioService);
        
            //Auto init particles
            #region
            if (transform.Find("NitroParticles"))
            {
                var tempNtiroParticles = transform.Find("NitroParticles").GetComponentsInChildren(typeof(ParticleSystem), true);
                if (tempNtiroParticles.Length > 0)
                {
                    foreach (ParticleSystem tempPart in tempNtiroParticles)
                    {
                        nitroParticles.Add(tempPart);
                    }
                }
            }

            if (transform.Find("SmokeParticles"))
            {
                var tempSmokeParticles = transform.Find("SmokeParticles").GetComponentsInChildren(typeof(ParticleSystem), true);
                if (tempSmokeParticles.Length > 0)
                {
                    foreach (ParticleSystem tempPart in tempSmokeParticles)
                    {
                        smokeParticles.Add(tempPart);
                    }
                }
            }
            #endregion

              

            //Color Block
            #region
            var colorMeshsTemp = transform.GetComponentsInChildren(typeof(MeshRenderer), true);

            foreach (MeshRenderer colorMesh in colorMeshsTemp)
            {
                if (colorMesh.materials.Length > 1)
                {
                    colorMeshs.Add(colorMesh);
                }

            }


            if (colorMeshs[0].materials[0].name.Replace(" (Instance)", "") == "Car_Cuzov")
            {


                colorMat = new Material(colorMeshs[0].materials[0]);
                mainMat = new Material(colorMeshs[0].materials[1]);


            }
            else if (colorMeshs[0].materials[1].name.Replace(" (Instance)", "") == "Car_Cuzov")
            {

                colorMat = new Material(colorMeshs[0].materials[1]);
                mainMat = new Material(colorMeshs[0].materials[0]);

            }
            #endregion


            #region createing cardrop audio emitter

            _audioService.CreateCarDropEmitter();



            #endregion

            #region createing engine audio emitter
            _audioService.CreateCarEngineEmitter(engineEmitterName, engineParam1, engineParam2);
            #endregion

            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);
            _gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_ASYNC) as GameWindowAsyncMultiplayer;
            Assert.AreNotEqual(null, _gameWindow);

        }

        public virtual void Init()
        {



            
            BaseQuan = controller.wheels[0].wheelTransform.transform.rotation;
            //_gameView = (GameWindow)Core.Instance.GetService<UIManager>().GetWindow(UIWindowEnum.GAME);
            //_gameView.meterCountLabel.text = String.Format("meters: {0}", 0);
            _tr = transform;
            _rb = GetComponent<Rigidbody>();

            //_rb.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
            //Debug.Log(_rb.constraints);


            input.enabled = false;


            EnableSkidParticles(false);
            ApplyCarParameters(model.GetPlayerParameters());
            
            ApplyCarUpgradeValues(model.GetPlayerUpgrades());

            //
            ///CheckInitRaceTimerDefault();

            //suspension.enabled = true;

            wheelRadius = controller.wheels[0].wheelCollider.radius;
            nitro.Init(this);

            carDrag = _rb.drag;
            _rb.drag = 1;


            F_WC = controller.wheels[0].wheelCollider;
            R_WC = controller.wheels[3].wheelCollider;
           // InitEventReference();
            
        }




       // protected abstract void InitEventReference();


        #region BUTTON EVENT


        private void SetParticlesAcceleration()
        {
            for (int i = 0; i < smokeParticles.Count; i++)
            {
                smokeParticles[i].emissionRate = 200;

            }
        }

        private void SetParticlesStoped()
        {
            for (int i = 0; i < smokeParticles.Count; i++)
            {
                smokeParticles[i].emissionRate = 40;
            }
        }

        private void SetThrottleForward()
        {
            throttleInput = 1;
        }

        private void SetThrottleBack()
        {
            throttleInput = -1;
        }

        private void SetThrottleStop()
        {
            throttleInput = 0;
        }

        private void SetUpVertical()
        {
            vertical = 1;
        }

        private void SetDownVertical()
        {
            vertical = -1;
        }

        private void SetDefaultVertical()
        {
            vertical = 0;
        }

        #endregion

        float carDrag = 0f;

        public virtual void EnablePlayerControll(bool enable)
        {
            nitro.enabled = enable;
            controller.enabled = enable;


            //resetTime = Time.time;
            for (int i = 0; i < smokeParticles.Count; i++)
                smokeParticles[i].Play();
            input.enabled = enable;
            airController.enabled = enable;
            tricks.enabled = enable;

            if (enable)
            {
                _audioService.EngineEmitterPlay();
            }
            else
            {
                _audioService.CarDropEmitterStop();
                nitro.stopParticle();
                _audioService.EngineEmitterStop();
            }

            _rb.drag = carDrag;

            isPlayer = enable;

            MassDiver = controller.wheels[0].wheelCollider.suspensionSpring.spring / 40;



        }
        public void SetColorInGame(int id)
        {

            Material ApplyMat = new Material(colorMat);

            var colorData = Core.Instance.GetService<PlayerManager>().allColors.Find(c => c.ID == id);

            if (colorData != null)
                ApplyMat.color = HexToColor(colorData.hex);

            foreach (MeshRenderer mesh in colorMeshs)
            {

                List<Material> mats = new List<Material>();
                if (mesh.materials[0].name.Replace(" (Instance)", "") == "Car_Cuzov")
                {
                    mats.Add(ApplyMat);
                    mats.Add(mainMat);
                    mesh.materials = mats.ToArray();
                }
                else if (mesh.materials[1].name.Replace(" (Instance)", "") == "Car_Cuzov")
                {
                    mats.Add(mainMat);
                    mats.Add(ApplyMat);
                    mesh.materials = mats.ToArray();
                }

            }
        }

        public Color32 HexToColor(string hex)
        {
            hex = hex.Replace("0x", "");
            hex = hex.Replace("#", "");

            byte a = 255;
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            }

            return new Color32(r, g, b, a);
        }

        public virtual void EnableEnemyControll(bool enable)
        {
            Instantiate(stayCube, transform);
            _rb.isKinematic = true;

            suspension.enabled = true;
            controller.enabled = enable;
            controller.isEnemy = true;
            controller.wheels[0].wheelCollider.gameObject.layer = 0;
            controller.wheels[1].wheelCollider.gameObject.layer = 0;
            controller.wheels[2].wheelCollider.gameObject.layer = 0;
            controller.wheels[3].wheelCollider.gameObject.layer = 0;

            input.enabled = false;
            //resetTime = Time.time;

            for (int i = 0; i < smokeParticles.Count; i++)
            {
                smokeParticles[i].Play();
            }

            var roofColliderTransform = transform.Find("Colliders/RoofCollider");

            if (roofColliderTransform != null)
            {
                roofColliderTransform.gameObject.SetActive(false);
            }

            _rb.drag = carDrag;
        }

        public float throttleInput;
        public float vertical;



        void Update()
        {
            if (isPlayer && Vibration.IsOn && Time.timeScale == 1)
            {
                if (R_WC.GetGroundHit(out R_WH))
                {
                    GetVibro(R_WH);
                }

                if (F_WC.GetGroundHit(out F_WH))
                {
                    GetVibro(F_WH);
                }
            }


            //wheelRPM = (controller.wheelData[0].angularVelocity + controller.wheelData[3].angularVelocity) / 2 * 80;

            if (controller.enabled && isPlayer)
            {
                #region calculating speed percent
                if (controller.wheelData[0].grounded || controller.wheelData[3].grounded)
                {
                    speedPecent = controller.speed / controller.maxSpeedForward * 100;
                    gearInAir = true;
                }
                else
                {
                    if (gearInAir)
                    {
                        speedAirPercent = speedPecent;
                        gearInAir = false;
                    }
                    if (speedPecent < speedAirPercent * 0.8f)
                    {
                        speedPecent = speedAirPercent * 0.8f;
                    }
                    else
                    {
                        speedPecent = Mathf.Abs(controller.speed) / controller.maxSpeedForward * 100;
                    }

                }

                #endregion

                #region calculating gear

                if (speedPecent < 20f)
                {
                    gearPercent = speedPecent / 20f;
                    RPM = 3000 * gearPercent;
                }
                else if (speedPecent >= 20f && speedPecent < 40f)
                {

                    gearPercent = speedPecent / 40f;
                    RPM = 3500 * gearPercent;
                }
                else if (speedPecent >= 40f && speedPecent < 60f)
                {
                    gearPercent = speedPecent / 60f;
                    RPM = 4000 * gearPercent;
                }

                else if (speedPecent >= 60f && speedPecent < 80f)
                {
                    gearPercent = speedPecent / 80f;
                    RPM = 5000 * gearPercent;
                }
                else if (speedPecent >= 80f)
                {
                    gearPercent = speedPecent / 100f;
                    RPM = 5500 * gearPercent;
                }

                // reverse gear
                if (speedPecent < 0f)
                {
                    gearPercent = speedPecent / 100f;
                    RPM = 5500 * Mathf.Abs(gearPercent);
                }

                #endregion

                Load = (gearPercent + Input.GetAxis("Horizontal")) / 2 * 100;

                _audioService.EngineEmitterSetParameter("RPM", RPM);
                _audioService.EngineEmitterSetParameter("Load", Load);

                //Debug.Log(RPM);
            }

        }
        
        public void FixedUpdate()
        {


            if (controller.enabled && input.enabled && isPlayer)
            {
               // for (int i = 0; i < controller.wheelData.Length; i++)
               // {
                   // if (!controller.wheelData[i].collider.isGrounded)
                   // {
                        //start coroutine for add bonuses per metr
                        //Debug.LogError("Letim");
                    //}
                //}

                meters = (int)(Math.Abs(Math.Round(Transform.position.x)));

                if (currentMeters < meters)
                {
                    currentMeters = meters;
                }

                //Debug.Log("currMeters = " + currentMeters + " | " + "meters = " + meters);

           

                
#if !UNITY_EDITOR
            controller.throttleInput = throttleInput;
            airController.rotateAndroid = vertical;
#endif
            }
        }

        Vector3 localRot;
        float addAngle = 0f;
        public void RotateEnemyWheels(float angle)
        {
            addAngle += angle;

            for (int i = 0; i < controller.wheels.Length; i++)
            {
                controller.wheels[i].wheelTransform.transform.localRotation = Quaternion.Euler(addAngle, localRot.y, localRot.z);

            }
        }

        public virtual void EnableSkidParticles(bool enable)
        {
            if (_isSkidParticlesEnabled != enable && skidParticles != null && skidParticles.Count > 0)
            {
                _isSkidParticlesEnabled = enable;

                if (enable)
                {
                    foreach (var p in skidParticles)
                    {
                        p.Play();
                    }
                }
                else
                {
                    foreach (var p in skidParticles)
                    {
                        p.Stop();
                    }
                }
            }
        }

        public void ApplyCarParameters(Dictionary<string, float> param)
        {
            foreach (var value in param)
            {
                UpgradeValue(value.Key, value.Value);
            }
        }
        public void ApplyCarUpgradeValues(List<UpgradeItem> upgrades)
        {
            foreach (var item in upgrades)
            {
                
                foreach (var value in item.upgradeValues)
                {
                    UpgradeValue(value.Key, value.Value);
                }
            }
        }

        private void UpgradeValue(string valueName, float value)
        {

            switch (valueName)
            {
                //TODO add mass
                case "Mass":
                    Rigidbody.mass = value;
                    break;
                case "MaxSpeedForward":
                    controller.maxSpeedForward = value;
                    break;
                case "MaxDriveForce":
                    controller.maxDriveForce = value;
                    break;
                case "TireFriction":
                    controller.tireFriction = value;
                    break;
                case "MaxBrakeForce":
                    controller.maxBrakeForce = value;
                    break;
                case "Spring":
                    foreach (var item in controller.wheels)
                    {
                        JointSpring suspension = item.wheelCollider.suspensionSpring;
                        suspension.spring = value;
                        item.wheelCollider.suspensionSpring = suspension;
                    }
                    break;

                case "Damper":
                    foreach (var item in controller.wheels)
                    {

                        JointSpring suspension = item.wheelCollider.suspensionSpring;
                        suspension.damper = value;
                        item.wheelCollider.suspensionSpring = suspension;
                    }
                    break;

                case "ForceCurveShape":
                    controller.forceCurveShape = value;
                    break;
                case "SuspensionDistance":
                    foreach (var item in controller.wheels)
                    {
                        item.wheelCollider.suspensionDistance = value;
                    }
                    break;
                case "MaxVelocity":
                    nitro.maxVelocity = value;
                    break;
                case "NitroPower":
                    nitro.nitroPower = value;
                    break;
                case "NitroDuration":
                    nitro.NitroDuration = value;
                    break;
                case "InAirSpeed":
                    airController.InAirSpeed = value;
                    break;
                default:
                    Debug.Log(valueName + ": " + value);
                    break;
            }
        }
        public virtual void ApplyCarPart()
        {

        }

        public virtual void ApplyCarSkin()
        {

        }

        public virtual void StopEnemyRigidbody()
        {

        }

        public virtual void StopPlayerRigidbody()
        {

        }

        public void Destroy()
        {
            Debug.LogError("Destroy " + name);
            nitro.destroySound();

            _audioService.EngineEmitterStop();
            // Destroy(_audioService.engineEmitter);
            _audioService.CarDropEmitterStop();
            //Destroy(carDropEmitter);



            Destroy(transform.gameObject);
        }



        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Finish")
            {
                #region DEBUG
#if UNITY_EDITOR

#endif
                #endregion
                EventManager._init.Game.CarEvent.Player.GetEvent.Invoke_Finish();
                //OnFinish();
                IsFinish = true;
            }
        }

        bool _isCrashed = false;
        void OnCollisionEnter(Collision col)
        {
            ContactPoint contact = col.contacts[0];

            if (isPlayer && Vibration.IsOn)
                StartCoroutine(Vibration.WaitAndVibro(50, 0.03f, (int)col.relativeVelocity.magnitude / 4));

            if (!_isCrashed &&
                ((contact.thisCollider.tag == "RoofCollider" && contact.otherCollider.tag != "Barrier") || col.collider.tag == "BrokeCollider")
            )
            {

                _isCrashed = true;
               
                   _audioService.EngineEmitterStop();
                    _audioService.CarDropEmitterStop();
                    nitro.stopSound();
                if(tag == "Player")
                    EventManager._init.Game.CarEvent.Player.GetEvent.Invoke_Crash();
                else
                    EventManager._init.Game.CarEvent.Enemy.GetEvent.Invoke_Crash();
                //OnCrash();


            }
        }


        public void ExplosePlayer()
        {
            suspension.enabled = false;
            EnablePlayerControll(false);
            // AudioService.playExplose();
            explosion.DestroyCar("player");

            nitro.stopParticle();

            for (int i = 0; i < controller.wheels.Length; i++)
            {
                controller.wheels[i].wheelCollider.enabled = false;
            }
            //EventManager._init.Game.CarEvent.Player.Invoke_Crash();
        }

        public void ExploseEnemy()
        {
            EnableEnemyControll(false);


            for (int i = 0; i < smokeParticles.Count; i++)
            {
                smokeParticles[i].Stop();
            }


            for (int i = 0; i < controller.wheels.Length; i++)
            {
                controller.wheels[i].wheelCollider.enabled = false;
            }
            explosion.DestroyCar("enemy");
            //EventManager._init.Game.CarEvent.Enemy.Invoke_Crash();
        }

        private void GetVibro(WheelHit _wh)
        {
           

            if ((_wh.force / MassDiver) > 12 && (_wh.force / MassDiver) < 15)
            {
                Vibration.Vibrate(10);
                playDropAudio(10);

            }

            else if ((_wh.force / MassDiver) > 15 && (_wh.force / MassDiver) < 20)
            {
                Vibration.Vibrate(15);
                playDropAudio(30);

            }

            else if ((_wh.force / MassDiver) > 20 && (_wh.force / MassDiver) < 25)
            {
                StartCoroutine(Vibration.WaitAndVibro(15, 0.009f, 2));
                Vibration.Vibrate(15);

                playDropAudio(60);

            }

            else if ((_wh.force / MassDiver) > 25 && (_wh.force / MassDiver) < 35)
            {
                StartCoroutine(Vibration.WaitAndVibro(25, 0.03f, 2));

                playDropAudio(80);

            }
            else if ((_wh.force / MassDiver) > 35)
            {
                StartCoroutine(Vibration.WaitAndVibro(50, 0.03f, 2));

                playDropAudio(100);

            }




        }

        public void playDropAudio(float drop_level)
        {


            _audioService.CarDropEmitterSetParameter("drop_level", Mathf.Clamp01(drop_level));
            _audioService.CarDropEmitterPlay();

        }



        }
    }


