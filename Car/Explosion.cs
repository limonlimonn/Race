using UnityEngine;
using UnityEngine.Assertions;

namespace HCR
{
	public class Explosion : MonoBehaviour
	{

		public KeyCode key = KeyCode.M;
		public float force = 300f;
		private Rigidbody[] deadObjs;
		
		//private Rigidbody physics;
		private Transform liveBody;
		private Transform deadBody;
		private GameObject explosion;
		private GameObject carColliders;
        private AudioService _audioService;

        void Start () {
			liveBody = transform.Find("LiveBody");
			deadBody = transform.Find("DeadBody");
			explosion = transform.Find("Explosion").gameObject;

			carColliders = transform.Find("Colliders").gameObject;
			deadObjs = new Rigidbody[deadBody.childCount];
			for (int a = 0; a < deadBody.childCount; a ++)
				deadObjs[a] = deadBody.GetChild(a).GetComponent<Rigidbody>() as Rigidbody;

            _audioService = Core.Instance.GetService<AudioService>();
            Assert.AreNotEqual(null, _audioService);


        }
		void Update () {
			if (Input.GetKeyDown(key))
				DestroyCar();
		}
		public void DestroyCar (string WhoExpl = "nothing") {
            //Debug.Log("Explosion");

            liveBody.gameObject.SetActive(false);
			carColliders.SetActive(false);
			deadBody.gameObject.SetActive(true);
            if(WhoExpl != "enemy")
            _audioService.StopGameMusic();
            explosion.SetActive(true);
            _audioService.RM_PlayOneShot("event:/Action/Crash");
            

            foreach (Rigidbody deadObj in deadObjs)
				deadObj.AddForce(new Vector3(Random.Range(-1f, 1f),
					                 Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * force);
		}
        




	}
}