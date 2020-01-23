using HCR.Enums;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace HCR.Gameplay.Tutorial
{
    public class StartThrottleTrigger : MonoBehaviour
    {


        private UIManager _uiManager;
        private TrigersTutorial _trigersTutorial;

        public string nameButton;
        public int triggerOrder;
        public Transform StartPosition;

        private bool isTriggered = false;
        public void Start()
        {
            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);

            _trigersTutorial = _uiManager.GetWindow(UIWindowEnum.TRIGERS_TUTORIAL) as TrigersTutorial;
            Assert.AreNotEqual(null, _trigersTutorial);

        }

        void OnTriggerStay(Collider other)
        {
            if (other.tag == "BodyCollider" && isTriggered == false)
            {
                
                if (other.GetComponentInParent<CarBase>().controller.enabled)
                {
                    isTriggered = true;
                    Check();
                }
            }
        }

        void Check()
        {
            //yield return new WaitForSeconds(0.1f);
            var s = StartPosition;
            _trigersTutorial.SetStartPosition(s.position.x, s.position.y, s.position.z);
            _trigersTutorial.ShowTutorialHint(nameButton, triggerOrder);
            //Destroy(gameObject);
        }

    }
}



