using System.Collections;
using UnityEngine;

namespace HCR
{
    public class TrickRoot : MonoBehaviour
    {

        public GameObject flipRoot, airRoot, balanceRoot, nineRoot;
        public TextMesh
            flipCount, flipName, flipX,
            balanceCount, balanceName, balanceX,
            airCount, airName, airX,
            nineName, nineDegrees;

        private IEnumerator airDeactivator;


        private float airDelta = 0f;
        private float airPercent;
        private float airFadeOut = 0.6f;
        private float airFadeIn = 0.4f;
        private bool checkAir = false;
        private bool airOpacityIn = true;
        private bool airOpacityOut = false;


        private float flipDelta = 0f;
        private float flipPercent;
        private float flipFadeOut = 0.6f;
        private float flipFadeIn = 0.1f;
        private float flipCountOut = 0.5f;
        private bool flipOpacityOut = false;
        private bool flipOpacityIn = false;
        private bool flipCountChange = false;



        private float balanceDelta = 0f;
        private float balancePercent;
        private float balanceFadeOut = 3f;
        private float balanceFadeIn = 0.1f;
        private float balanceCountOut = 0.5f;
        private bool balanceOpacityOut = false;
        private bool balanceOpacityIn = false;
        private bool balanceCountChange = false;

        private float nineDelta = 0f;
        private float ninePercent;
        private float nineFadeOut = 3f;
        private float nineFadeIn = 0.01f;
        private float nineCountOut = 0.5f;
        private bool nineOpacityOut = false;
        private bool nineOpacityIn = false;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            if (checkAir && airOpacityIn)
            {
                airDelta += Time.deltaTime;
                if (airFadeIn > airDelta)
                {
                    airPercent = 1 - (airFadeIn - airDelta) / airFadeIn;
                    airCount.color = new Color(airCount.color.r, airCount.color.g, airCount.color.b, airPercent);
                    airName.color = new Color(airName.color.r, airName.color.g, airName.color.b, airPercent); ;
                    airX.color = new Color(airX.color.r, airX.color.g, airX.color.b, airPercent);
                }
                else
                {

                    airOpacityIn = false;
                    airDelta = 0;
                    airDeactivator = deActivateAir();
                    StartCoroutine(airDeactivator);


                }

            }

            if (checkAir && airOpacityOut)
            {

                airDelta += Time.deltaTime;
                if (airFadeOut > airDelta)
                {
                    airPercent = (airFadeOut - airDelta) / airFadeOut;
                    airCount.color = new Color(airCount.color.r, airCount.color.g, airCount.color.b, airPercent);
                    airName.color = new Color(airName.color.r, airName.color.g, airName.color.b, airPercent); ;
                    airX.color = new Color(airX.color.r, airX.color.g, airX.color.b, airPercent);
                }
                else
                {
                    checkAir = false;
                    airOpacityOut = false;
                    airRoot.SetActive(false);
                }
            }

            if (flipOpacityIn)
            {
                flipDelta += Time.deltaTime;
                if (flipFadeIn > flipDelta)
                {
                    flipPercent = (flipFadeOut - flipDelta) / flipFadeOut;
                    flipCount.color = new Color(flipCount.color.r, flipCount.color.g, flipCount.color.b, flipPercent);
                    flipName.color = new Color(flipName.color.r, flipName.color.g, flipName.color.b, flipPercent); ;
                    flipX.color = new Color(flipX.color.r, flipX.color.g, flipX.color.b, flipPercent);
                }
                else
                {
                    flipDelta = 0;
                    flipOpacityIn = false;

                }
            }

            if (flipOpacityOut)
            {

                flipDelta += Time.deltaTime;
                if (flipFadeOut > flipDelta)
                {
                    flipPercent = (flipFadeOut - flipDelta) / flipFadeOut;
                    flipCount.color = new Color(flipCount.color.r, flipCount.color.g, flipCount.color.b, flipPercent);
                    flipName.color = new Color(flipName.color.r, flipName.color.g, flipName.color.b, flipPercent); ;
                    flipX.color = new Color(flipX.color.r, flipX.color.g, flipX.color.b, flipPercent);
                }
                else
                {
                    flipOpacityOut = false;
                    flipRoot.SetActive(false);
                }
            }

            if (flipCountChange)
            {
                flipDelta += Time.deltaTime;
                if (flipCountOut > flipDelta)
                {

                    flipCount.fontSize = 60 + (int)Mathf.Round(100 * (flipCountOut - flipDelta) / flipCountOut);
                }
                else
                {

                    flipCountChange = false;
                }
            }

            if (balanceOpacityIn)
            {
                balanceDelta += Time.deltaTime;
                if (balanceFadeIn > balanceDelta)
                {
                    balancePercent = (balanceFadeIn - balanceDelta) / balanceFadeIn;
                    balanceCount.color = new Color(balanceCount.color.r, balanceCount.color.g, balanceCount.color.b, balancePercent);
                    //balanceName.color = new Color(balanceName.color.r, balanceName.color.g, balanceName.color.b, balancePercent); ;
                    //balanceX.color = new Color(balanceX.color.r, balanceX.color.g, balanceX.color.b, balancePercent);
                }
                else
                {
                    balanceDelta = 0;
                    balanceOpacityIn = false;

                }
            }

            if (balanceOpacityOut)
            {
                balanceDelta += Time.deltaTime;
                if (balanceFadeOut > balanceDelta)
                {
                    balancePercent = (balanceFadeOut - balanceDelta) / balanceFadeOut;
                    balanceCount.color = new Color(balanceCount.color.r, balanceCount.color.g, balanceCount.color.b, balancePercent);
                    balanceName.color = new Color(balanceName.color.r, balanceName.color.g, balanceName.color.b, balancePercent); ;
                    balanceX.color = new Color(balanceX.color.r, balanceX.color.g, balanceX.color.b, balancePercent);
                }
                else
                {
                    balanceOpacityOut = false;
                    balanceRoot.SetActive(false);
                }
            }

            if (balanceCountChange)
            {
                balanceDelta += Time.deltaTime;
                if (balanceCountOut > balanceDelta)
                {

                    balanceCount.fontSize = 60 + (int)Mathf.Round(30 * (balanceCountOut - balanceDelta) / balanceCountOut);
                }
                else
                {
                    balanceCountChange = false;
                }
            }


            if (nineOpacityIn)
            {
                nineDelta += Time.deltaTime;
                if (nineFadeIn > nineDelta)
                {
                    ninePercent = (nineFadeOut - nineDelta) / nineFadeOut;
                    nineName.color = new Color(nineName.color.r, nineName.color.g, nineName.color.b, ninePercent);
                    nineDegrees.color = new Color(nineDegrees.color.r, nineDegrees.color.g, nineDegrees.color.b, ninePercent);
                }
                else
                {
                    nineDelta = 0;
                    nineOpacityIn = false;

                }
            }

            if (nineOpacityOut)
            {
               
                nineDelta += Time.deltaTime;
                if (nineFadeOut > nineDelta)
                {
                    ninePercent = (nineFadeOut - nineDelta) / nineFadeOut;
                    nineName.color = new Color(nineName.color.r, nineName.color.g, nineName.color.b, ninePercent);
                    nineDegrees.color = new Color(nineDegrees.color.r, nineDegrees.color.g, nineDegrees.color.b, ninePercent);
                }
                else
                {
                    nineOpacityOut = false;
                    nineRoot.SetActive(false);
                }
            }



        }

        public void activateAir(float count)
        {
            airCount.text = count.ToString("f1");
            //nineRoot.SetActive(false);
            //flipRoot.SetActive(false);
            //balanceRoot.SetActive(false);
            airRoot.SetActive(true);

            airDelta = 0;
            airOpacityIn = true;
            airOpacityOut = false;
            checkAir = true;



        }
        public IEnumerator deActivateAir()
        {

            yield return new WaitForSeconds(1f);
            airOpacityOut = true;
        }

        public void activateFlip()
        {

            nineRoot.SetActive(false);
            //balanceRoot.SetActive(false);
            //airRoot.SetActive(false);
            flipRoot.SetActive(true);

            flipOpacityIn = true;
            flipDelta = 0;
        }

        public void flipCounter(float count)
        {


            flipDelta = 0;
            flipCount.text = count.ToString();
            flipCountChange = true;
        }

        public void deActivateFlip()
        {
            flipDelta = 0;
            flipOpacityOut = true;

        }


        public void activateBalance()
        {
            nineRoot.SetActive(false);
            //airRoot.SetActive(false);
            //flipRoot.SetActive(false);
            balanceRoot.SetActive(true);


            balanceOpacityIn = true;
        }


        public void balanceCounter(float count)
        {
            balanceCount.text = count.ToString("f1");
            balanceDelta = 0;
            balanceCountChange = true;
        }

        public void deActivateBalance()
        {
            balanceDelta = 0;
            balanceOpacityOut = true;



        }

        public void activateNine()
        {

            airRoot.SetActive(false);
            //flipRoot.SetActive(false);
            balanceRoot.SetActive(false);
            nineRoot.SetActive(true);


            nineOpacityIn = true;
        }

        public IEnumerator deActivateNine()
        {
            
            yield return new WaitForSeconds(0.1f);
            nineOpacityOut = true;


        }

        public void deActivateAll()
        {
            airRoot.SetActive(false);
            flipRoot.SetActive(false);
            balanceRoot.SetActive(false);
            nineRoot.SetActive(false);
        }
    }
}