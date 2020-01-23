using UnityEngine;

namespace HCR
{
	public class FitilAnimator : MonoBehaviour
	{

		public GameObject ParticleRoot;
		public ParticleSystem fire;
		public ParticleSystem smoke;
		public ParticleSystem sparks;
		public ParticleSystem flare;
		private ParticleSystem.EmissionModule emissionFire;
		private ParticleSystem.EmissionModule emissionSmoke;
		private ParticleSystem.EmissionModule emissionSparks;
		private ParticleSystem.EmissionModule emissionFlare;

		private float delta = 0f;
		private float delay;
		public Transform fitil;
		public ExplodeTrigger ExplodeScript;
		private bool enableDelta = true;

		private float emmisionPercent;
		private float deltaPercent;

		// Use this for initialization
		void Start () {
			fitil.gameObject.SetActiveRecursively(true);
			emissionFire = fire.emission;
			emissionSmoke = smoke.emission;
			emissionSparks = sparks.emission;
			emissionFlare = flare.emission;
			delay = ExplodeScript.delay;
		}

		// Update is called once per frame
		void Update()
		{

			if (enableDelta) {
				delta += Time.deltaTime;
				if (delay > delta)
				{
					deltaPercent = (delay - delta) / delay;
					emmisionPercent = Mathf.Pow(deltaPercent, 2);
					emissionFire.rateOverTime = 20f * deltaPercent;
					emissionSmoke.rateOverTime = 20f * deltaPercent;
					emissionSparks.rateOverTime = 100f * deltaPercent;


					if (0.7f > deltaPercent)
					{
						emissionFire.rateOverTime = 0;
						emissionSmoke.rateOverTime = 0;

					}
					if (0.5f > deltaPercent && deltaPercent > 0.15f)
					{
						sparks.startLifetime = 0.8f;
						sparks.gravityModifier = 0.3f;
						sparks.playbackSpeed = 2f;
						emissionSparks.rateOverTime = 300;

					}
				}
				else {
					enableDelta = false;
				}
			}

		}
		void OnDisable()
		{
			ParticleRoot.SetActiveRecursively(false);
		}



	}
}