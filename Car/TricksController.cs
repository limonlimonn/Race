using System.Collections;
using UnityEngine;
using EVP;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System;

namespace HCR
{
	public class TricksController : MonoBehaviour
	{

        private Transform increaseParent;
        private Transform trickRoot;
        private TrickRoot trickScript;
        private WheelCollider wc_FL;
        private WheelCollider wc_FR;
        private WheelCollider wc_RL;
        private WheelCollider wc_RR;

        private float balanceTime = 0;
        private bool showBalance = false;
        private bool hideBalance = false;
        public float balanceDelay = 1f;

        private bool checkNine = true;
        private bool showNine = false;
        private bool hideNine = false;

        private float inairTime = 0;
        private bool showInAir = false;
        private bool hideInAir = false;
        public float inairDelay = 3f;

        private bool startFlip = false;
        private bool endFlip = false;
        private bool checkFlip = true;
        private float startQuanX = 0f;
        private float startAngle = 0f;

        private float flipCount = 0f;

        private float trickTime = 0;

        private Text nitroPlus;
        private NewNitroController nitroCtrl;
        public float balanceNitroPercent = 0.1f;
        public float airNitroPercent = 0.1f;
        public float flipNitroPercent = 0.5f;
        public float candleNitroPercent = 0.25f;

        private float balanceNitro;
        private float airNitro;
        private float flipNitro;
        private float candleNitro;

        [HideInInspector]
        public Action triggerAir;
        public Action triggerFlip;
        public Action triggerCandle;
        public Action triggerBalance;
        private PlayerManager _playerManager;
        private SafePlayerPrefs _safePlayerPrefs;
        
        private GameObject chas;
        private GameObject strelka;
        private VehicleController vehicleCtrl = new VehicleController();
        // Use this for initialization
        void Start()
        {
            _playerManager = Core.Instance.GetService<PlayerManager>();
            _safePlayerPrefs = Core.Instance.GetService<SafePlayerPrefs>();

            increaseParent = GameObject.Find("Increase_parent").transform;
            trickRoot = increaseParent.Find("TrickRoot").transform;
            trickScript = trickRoot.GetComponent<TrickRoot>();

            Transform tempWcs = transform.Find("WheelColliders");
            vehicleCtrl = GetComponent<VehicleController>() as VehicleController;
            wc_FL = vehicleCtrl.wheels[0].wheelCollider;
            wc_FR = vehicleCtrl.wheels[1].wheelCollider;
            wc_RL = vehicleCtrl.wheels[2].wheelCollider;
            wc_RR = vehicleCtrl.wheels[3].wheelCollider;

            nitroCtrl = GetComponent<NewNitroController>() as NewNitroController;
            nitroPlus = GameObject.Find("NitroPlus").GetComponent<Text>();

            balanceNitro = nitroCtrl.maxNitro * balanceNitroPercent;
            airNitro = nitroCtrl.maxNitro * airNitroPercent;
            flipNitro = nitroCtrl.maxNitro * flipNitroPercent;
            candleNitro = nitroCtrl.maxNitro * candleNitroPercent;

            chas = GameObject.Find("chas");
            chas.transform.parent = null;
            strelka = GameObject.Find("strelka");
            //Debug.LogError(nitroCtrl.maxNitro);
        }

        
        // Update is called once per frame
        void Update()
        {
            float speed = vehicleCtrl.cachedRigidbody.velocity.magnitude;
            
            trickRoot.position = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z - 2);
            

            #region BALANCE

            if (!wc_FL.isGrounded && !wc_FR.isGrounded && wc_RL.isGrounded && wc_RR.isGrounded && speed > 0)
            {
                balanceTime += Time.deltaTime;

                if (balanceTime > balanceDelay)
                {
                    trickScript.activateBalance();
                    trickScript.balanceCounter(balanceTime);


                    showBalance = true;
                }

            }
            else if (showBalance)
            {
                hideBalance = true;
                showBalance = false;
            }else
            {
                balanceTime = 0f;
            }

            if (hideBalance)
            {

                AddNitro(balanceNitro * balanceTime, "balance", balanceTime);

                if (triggerBalance != null)
                {
                    triggerBalance();
                }
                balanceTime = 0f;

                trickScript.deActivateBalance();
                hideBalance = false;


            }
            #endregion

            #region CANDLE

            if (!wc_RL.isGrounded && !wc_RR.isGrounded && !wc_FL.isGrounded && !wc_FR.isGrounded && checkNine)
            {
                if (transform.rotation.eulerAngles.x > 268 && transform.rotation.eulerAngles.x < 274)
                {
                    trickScript.activateNine();
                    showNine = true;
                    if(triggerCandle != null)
                    {
                        triggerCandle();
                    }

                    checkNine = false;
                }


            }

            if (showNine)
            {
                AddNitro(candleNitro, "90", 1);

                StartCoroutine(trickScript.deActivateNine());
                showNine = false;
            }

            if (wc_RL.isGrounded && wc_RR.isGrounded && wc_FL.isGrounded && wc_FR.isGrounded && showNine == false)
            {
                checkNine = true;
            }

            #endregion
            
            #region IN AIR

            if (!wc_RL.isGrounded && !wc_RR.isGrounded && !wc_FL.isGrounded && !wc_FR.isGrounded)
            {
                inairTime += Time.deltaTime;
                showInAir = true;
            }
            else if (showInAir)
            {
                if (inairTime > inairDelay)
                {
                    trickScript.activateAir(inairTime);
                    showInAir = false;
                    hideInAir = true;
                    if (triggerAir != null)
                    {
                        triggerAir();

                    }
                        
                    
                }
                else
                {
                    inairTime = 0f;
                }


            }

            if (hideInAir)
            {
                trickScript.deActivateAir();
                AddNitro(airNitro * inairTime, "InAir", inairTime);
                inairTime = 0f;
                hideInAir = false;
                
            }

            #endregion

        }

        private string currentHour = "c";
        private string previuosHour = "c";
        private int hourCount = 0;

        public void AddFlip(string name)
        {
            hourCount++;
            currentHour = name;
            
            if (hourCount == 2 && currentHour != previuosHour) {
                flipCount += 1f;
                resetFlipVars();
            } else if (hourCount == 2) {
                resetFlipVars();
            }
            previuosHour = name;
        }

        public void resetFlipVars()
        {
            hourCount = 0;
            currentHour = "c";
            previuosHour = "c";
        }
        void FixedUpdate() {
     
            #region FLIP

            chas.transform.position = transform.position;

            if (!wc_RL.isGrounded && !wc_RR.isGrounded && !wc_FL.isGrounded && !wc_FR.isGrounded && checkFlip)
            {

                checkFlip = false;
                startFlip = true;
                startQuanX = transform.rotation.x;
                chas.transform.rotation = transform.rotation;
                flipCount = 0f;


            }



            if (!wc_RL.isGrounded && !wc_RR.isGrounded && !wc_FL.isGrounded && !wc_FR.isGrounded && startFlip)
            {
                
              

               
               
                if (Mathf.Abs(startQuanX - transform.rotation.x) >= 0.5)
                {
                    //flipCount += 0.5f;
                    //startQuanX = transform.rotation.x;
                }



                float textCounter = Mathf.Floor(flipCount);
                //Debug.LogError("flipCount: " + flipCount);
                if (textCounter >= 1)
                {
                    trickScript.activateFlip();
                    trickScript.flipCounter(textCounter);
                }
                else if (textCounter >= 2)
                {
                    trickScript.flipCounter(textCounter);
                }



            }
            else if (startFlip)
            {
                endFlip = true;
                startFlip = false;
                

                resetFlipVars();
            }

            if (endFlip)
            {


                endFlip = false;
                checkFlip = true;

                float endDiff = 0;
                if (flipCount > 0)
                {

                    if (flipCount % 1 == 0.5f)
                    {

                        endDiff = Mathf.Abs(startQuanX - transform.rotation.x);

                        Debug.LogError(endDiff);
                        if (flipCount == 0.5 && endDiff > 0.25f)
                        {
                            flipCount = 1f;
                            trickScript.flipCounter(flipCount);
                        }
                        else if (endDiff > 0.25f)
                        {
                            flipCount = Mathf.Round(flipCount + endDiff);
                            trickScript.flipCounter(flipCount);
                        }


                    }


                    StartCoroutine(WaitDeactivateFlip(flipCount));

                    /*Debug.LogError(
                        "FlipCount: " + flipCount +
                        "  Raznica: " + endDiff

                        );*/

                    AddNitro(flipCount * flipNitro, "Flip", flipCount);
                }



            }

            //Debug.Log(transform.rotation);


            #endregion
        }

        private IEnumerator WaitDeactivateFlip(float flipCount)
        {
            
            if (triggerFlip != null)
            {
                triggerFlip();
            }
            if(trickScript.flipRoot.activeSelf == false && flipCount >= 1)
            {
                trickScript.activateFlip();
                trickScript.flipCounter(flipCount);
            }
            yield return new WaitForSeconds(1);
            trickScript.deActivateFlip();
        }

        public void AddNitro(float nitroCount, string trickName, float trickCount)
        {
            nitroCtrl.nitroCurrent += nitroCount;

            //Debug.LogError("Nitro Count: " + nitroCount);
            //Debug.LogError("nitroCtrl.maxNitro: " + nitroCtrl.maxNitro);
            float xp = Mathf.Ceil(nitroCount / nitroCtrl.maxNitro * 100);
            _safePlayerPrefs.AddEarnedExpForTrick(xp);

            switch (trickName)
            {
                case "Flip":        _safePlayerPrefs.AddTrickFlipSum((int)trickCount);   break;
                case "InAir":       _safePlayerPrefs.AddTrickAirTime(trickCount);        break;
                case "90":     _safePlayerPrefs.AddTrick90Time((int)trickCount);         break;
                case "balance":          _safePlayerPrefs.AddTrickHorseTime(trickCount);      break;
              

                default:
                    break;
            }




            nitroPlus.text = "+ " + Mathf.Ceil(nitroCount / nitroCtrl.maxNitro * 100) + " n2o";

            if (nitroCtrl.nitroCurrent > nitroCtrl.maxNitro)
                nitroCtrl.nitroCurrent = nitroCtrl.maxNitro;


            StartCoroutine(ClearNitroPlus());

        }

        public IEnumerator ClearNitroPlus()
        {
            yield return new WaitForSeconds(0.7f);
            nitroPlus.text = "";
        }

        public void hideTricks()
        {
            trickScript.deActivateAll();
        }
        
    

    }
}