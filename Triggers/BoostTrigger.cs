using UnityEngine;

namespace HCR
{
	public class BoostTrigger : MonoBehaviour
	{

		public float boostPower = 30000f;
		private Rigidbody player;
		private bool boostOrder;



		void OnTriggerEnter (Collider hit)
		{
			Debug.Log(hit.tag);
			if (hit.tag == "BodyCollider" || hit.tag == "RoofCollider") {
				player = hit.GetComponentInParent<Rigidbody>() as Rigidbody;
				boostOrder = true;
			}
		}

		void FixedUpdate ()
		{
			if (boostOrder)

				player.AddForce(-transform.right * Mathf.Sqrt(player.mass) * 50 * boostPower);
		}


		void OnTriggerExit (Collider hit)
		{
			if (hit.tag == "BodyCollider" || hit.tag == "RoofCollider")
				boostOrder = false;
		}



	}
}