using UnityEngine;
using System;

namespace HCR
{
	public class DynamicSuspension : MonoBehaviour
	{

		public Boolean enable = true;
		public Transform WheelPointFL, WheelPointFR, WheelPointRL, WheelPointRR;
		public WheelCollider wheelColliderFR, wheelColliderRR;
		public Transform ArmFL, ArmFR, ArmRL, ArmRR;
		public Transform HelixFL, HelixFR, HelixRL, HelixRR;
		public Transform Axes_F, Axes_R;

		private double currentPos;
		private float anglez;
		private Vector2 sphere, wheel, hypotun;
		private Vector3 Scale;

		// Update is called once per frame
		void Update () {

			if (enable)
			{
				//transformHelix(WheelPointRR, wheelColliderRR, HelixRL);
				//transformHelix(WheelPointFR, wheelColliderFR, HelixFL);
				if (ArmRR != null && ArmRL != null)
				{
					transformArm(ArmRR, WheelPointRR);
					transformArm(ArmRL, WheelPointRL);
				}

				if (ArmFR != null && ArmFL != null) {
					transformArm(ArmFR, WheelPointFR);
					transformArm(ArmFL, WheelPointFL);
				}

				if(Axes_R != null && Axes_F != null){
					transformAxe(Axes_R, WheelPointRR);
					transformAxe(Axes_F, WheelPointFR);
				}

                if (HelixRR)
                {
                    transformHelix(WheelPointRR, wheelColliderRR, HelixRR);
                }

                if (HelixFR)
                {
                    transformHelix(WheelPointFR, wheelColliderFR, HelixFR);
                }
                
                if (HelixRL)
                {
                    HelixRL.localScale = HelixRR.localScale;
                }
                if (HelixFL)
                {
                    HelixFL.localScale = HelixFR.localScale;
                }
               

			}
		}


		public void transformAxe(Transform Axe, Transform WheelPoint) {
			Axe.localPosition = new Vector3(Axe.localPosition.x, WheelPoint.localPosition.y, Axe.localPosition.z);
		}

		public void transformHelix(Transform WheelPoint, WheelCollider WheelCollider, Transform Helix)
		{
			currentPos = WheelCollider.transform.position.y - WheelCollider.suspensionDistance - 0.02f - WheelPoint.position.y;


			Scale = new Vector3(
				1f,
				(1f + ((float)(Math.Round(currentPos, 2) / WheelCollider.suspensionDistance) * WheelCollider.suspensionDistance))*1.25f,
				1f
			);

			Helix.localScale = Scale;
		}

		public void transformArm(Transform Arm, Transform WheelPoint)
		{

			sphere = new Vector2(Arm.localPosition.z, Arm.localPosition.y);
			wheel = new Vector2(WheelPoint.localPosition.z, WheelPoint.localPosition.y);
			hypotun = new Vector2(WheelPoint.localPosition.z, Arm.localPosition.y);

			anglez = (float)Math.Acos(Vector2.Distance(hypotun, sphere) / Vector2.Distance(sphere, wheel)) * 180 / 3.14159265f;
			Arm.localRotation = Quaternion.Euler(anglez, Arm.localRotation.eulerAngles.y, Arm.localRotation.eulerAngles.z );

		}



	}
}