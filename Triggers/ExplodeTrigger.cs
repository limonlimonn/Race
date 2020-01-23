using HCR.Event;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace HCR
{
	public class ExplodeTrigger : MonoBehaviour
	{

		public float force = 300f;
		public float delay = 5f;


		private Rigidbody[] deadObjs;
		public Rigidbody physics;
		public Transform liveBody;
		public Transform deadBody;
		public GameObject explosion;



		public ExplodeTrigger ExplodeScript;
		public FitilAnimator FitilAnimator;

		private bool notDestroyed = true;
		private bool startDestroy = false;
		private int destroyIndex = 0;
        private AudioService _audioService;


        public CapsuleCollider XCapsule;
		public CapsuleCollider YCapsule;
		private Vector3 XstartPt, YstartPt;
		private Vector3 XendPt, YendPt;
		private float Xradius, Yradius;

		void Start () {
            _audioService = Core.Instance.GetService<AudioService>();
            Assert.AreNotEqual(null, _audioService);

            deadObjs = new Rigidbody[deadBody.childCount];
			for (int a = 0; a < deadBody.childCount; a ++)
				deadObjs[a] = deadBody.GetChild(a).GetComponent<Rigidbody>() as Rigidbody;



		}

		public void CheckCrashCapsules() {

			XstartPt = new Vector3(XCapsule.transform.position.x - (XCapsule.height - XCapsule.radius * 2) / 2, XCapsule.transform.position.y, XCapsule.transform.position.z);
			XendPt = new Vector3(XCapsule.transform.position.x + (XCapsule.height - XCapsule.radius * 2) / 2, XCapsule.transform.position.y, XCapsule.transform.position.z);
			Xradius = XCapsule.radius;

			YstartPt = new Vector3(YCapsule.transform.position.x, YCapsule.transform.position.y - (YCapsule.height - YCapsule.radius * 2) / 2, YCapsule.transform.position.z);
			YendPt = new Vector3(YCapsule.transform.position.x, YCapsule.transform.position.y + (YCapsule.height - YCapsule.radius * 2) / 2, YCapsule.transform.position.z);
			Yradius = YCapsule.radius;

			Collider[] Xres = Physics.OverlapCapsule(XstartPt, XendPt, Xradius);
			Collider[] Yres = Physics.OverlapCapsule(YstartPt, YendPt, Yradius);


			foreach (Collider col in Xres)
			{
               

                if (col.tag == "BodyCollider" || col.tag == "RoofCollider")
				{
                    Debug.Log(col.name);
                    Debug.Log(col.tag);
                    if (col.GetComponentInParent<CarBase>().isPlayer)
                    {
                        EventManager._init.Game.CarEvent.Player.GetEvent.Invoke_Crash();
                    }
                    return;
                }
			}

			foreach (Collider col in Yres)
			{
				if (col.tag == "BodyCollider" || col.tag == "RoofCollider")
				{
                    Debug.Log(col.name);
                    Debug.Log("dva");
                    if (col.GetComponentInParent<CarBase>().isPlayer)
                    {
                        EventManager._init.Game.CarEvent.Player.GetEvent.Invoke_Crash();
                    }
                    return;


                }

			}
			return;


		}

		public void DestroyObject() {

			liveBody.gameObject.SetActive(false);
			deadBody.gameObject.SetActive(true);
            _audioService.RM_PlayOneShot("event:/Action/Crash");
            explosion.SetActive(true);
            
			//physics.useGravity = false;
			// physics.isKinematic = true;

			foreach (Rigidbody deadObj in deadObjs)
			{
				deadObj.AddForce(new Vector3(Random.Range(-1f, 1f),
					                 Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * force);
			}




		}

		IEnumerator OnCollisionEnter(Collision collision)
		{
            //Debug.Log("collision.other.tag : " + collision.other.tag.ToString());
			if (collision.other.tag == "Player" && notDestroyed)
			{
				FitilAnimator.enabled = true;

				notDestroyed = false;
				yield return new WaitForSeconds(delay);


				StartCoroutine(destroyCounter());
				DestroyObject();
				CheckCrashCapsules();




			}
		}

		private IEnumerator destroyCounter() {

			yield return new WaitForSeconds(0.5f);

			FitilAnimator.enabled = false;
			ExplodeScript.enabled = false;

		}




	}
}