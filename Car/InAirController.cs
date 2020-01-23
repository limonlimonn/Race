using UnityEngine;
using System;

namespace HCR
{
	public class InAirController : MonoBehaviour
	{

		public float InAirSpeed = 3000f;
		public float COMCoefficent = 40f;
		//public float AeroDynamicCoeff = 5f;
		private Rigidbody m_rigidbody;
        public float rotateAndroid;
        private float moduleRotateAndroid;

		public WheelCollider Frontwheels;
		public WheelCollider Rearwheels;
		private WheelHit hit;
        //public float COMSpeedCorrection;
		float rotator;
		float angl;
		float AerodynamicResult;
		float currentTorque;
		float stabilizationCoeff;
        
        public float rotateAndroidCurrent = 100f;
        public float rotateAndroidMin = 50f;
        public float rotateAndroidMax = 100f;

        private Vector3 localVelosity, AerodynamicTorque, VerticalAirTorque, Totaltorque;

        private WheelCollider FL, FR, RL, RR;


        private bool rearIn;
		private bool frontIn;
        private float euler_x, euler_y;

        private Transform com_sphere;
		// Use this for initialization
		void Start()
		{
          //  GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
          //  sphere.GetComponent<SphereCollider>().isTrigger = true;
         //   sphere.transform.SetParent(transform);

            //com_sphere = transform.Find("Sphere");

        //  com_sphere = sphere.transform;
       
            m_rigidbody = transform.GetComponent<Rigidbody>();

            m_rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY  | RigidbodyConstraints.FreezeRotationZ;
            //m_rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        }

   
        

        void FixedUpdate()
		{
#if UNITY_EDITOR
            rotateAndroid = Input.GetAxis("Vertical");
#endif  
            moduleRotateAndroid = Mathf.Abs(rotateAndroid);
            #region wheel Detector
            //Wheel Detector
            if (Rearwheels.GetGroundHit(out hit))
			{
				rearIn = true;
			}
			else
			{
				rearIn = false;
			}

			if (Frontwheels.GetGroundHit(out hit))
			{
				frontIn = true;
			}
			else
			{
				frontIn = false;
			}
            #endregion


            //m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, m_rigidbody.velocity.y, 0f);

            ////Center of Mass per speed Coeff
            //COMSpeedCorrection = m_rigidbody.velocity.magnitude / COMCoefficent;

            ///m_rigidbody.centerOfMass = new Vector3(m_rigidbody.centerOfMass.x, m_rigidbody.centerOfMass.y + moduleRotateAndroid / 4 /*+ COMSpeedCorrection*/, m_rigidbody.centerOfMass.z - rotateAndroid / 4);

           // com_sphere.transform.localPosition = m_rigidbody.centerOfMass;

            ////AeroDynamic Coeff

            #region Rotation Stabilizator
            var euler = new Quaternion(
                    transform.rotation.x,
                    -transform.rotation.w,
                    transform.rotation.x,
                    transform.rotation.w
                );

            //Debug.Log(transform.rotation);
            transform.SetPositionAndRotation(transform.position, euler);

            #endregion

            #region Aerodyninamic



            //localVelosity = transform.InverseTransformDirection(m_rigidbody.velocity);

            //angl = Vector3.Angle(localVelosity, Vector3.forward);

            //if (angl < 90 && angl > 0)
            //{
            //    rotator = angl % 90 / 90;
            //}
            //else
            //{
            //    rotator = 0;
            //}


            //AerodynamicResult = rotator * Mathf.Sign(localVelosity.y) * (float)Math.Pow(m_rigidbody.velocity.magnitude, 2);

            //AerodynamicTorque = Vector3.left * AerodynamicResult * this.AeroDynamicCoeff;

            #endregion


            #region power graphic

            if (rotateAndroid == 0)
            {
                rotateAndroidCurrent = 100f;
                
            }
            else if 
                (Mathf.Abs(rotateAndroid) > 0 &&
                rotateAndroidCurrent > rotateAndroidMin &&
                rotateAndroidCurrent <= rotateAndroidMax)
            {
                rotateAndroidCurrent -= 1f;
            }

            #endregion



            if (rearIn == false && frontIn && rotateAndroid < 0)
            {

                //Totaltorque = AerodynamicTorque;

                //currentTorque = Vector3.Dot(m_rigidbody.angularVelocity.normalized, Totaltorque.normalized);

                //stabilizationCoeff = currentTorque < 0 ? 3 : 1;
               // m_rigidbody.AddTorque(Totaltorque * stabilizationCoeff, ForceMode.Force);
            }
          
            else if(rotateAndroid != 0)
            {
                //Totaltorque = AerodynamicTorque + VerticalAirTorque;
                //currentTorque = Vector3.Dot(m_rigidbody.angularVelocity.normalized, Totaltorque.normalized);
                //stabilizationCoeff = currentTorque < 0 ? 3 : 1;

                    var ResultPower = rotateAndroidCurrent * this.InAirSpeed / 100;
                    VerticalAirTorque = transform.right * -rotateAndroid * ResultPower;
                    m_rigidbody.AddTorque(VerticalAirTorque, ForceMode.Force);
                    
             
                
               
            }

  
        }

        

    }
}