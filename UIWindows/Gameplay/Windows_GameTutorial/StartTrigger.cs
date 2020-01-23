using HCR.Enums;
using UnityEngine;
using UnityEngine.Assertions;



namespace HCR.Gameplay.Tutorial
{
    public class StartTrigger : MonoBehaviour
    {
        
       
        private UIManager _uiManager;
        private TrigersTutorial _trigersTutorial;
        public Transform StartPosition;
        public string nameButton;
        public int triggerOrder;

        public void Start()
        {
            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);

            _trigersTutorial = _uiManager.GetWindow(UIWindowEnum.TRIGERS_TUTORIAL) as TrigersTutorial;
            Assert.AreNotEqual(null, _trigersTutorial);

        }

        void OnTriggerEnter(Collider other)
        {
           

            
            if (other.tag == "BodyCollider")
            {
                var s = StartPosition;
                _trigersTutorial.SetStartPosition(s.position.x, s.position.y, s.position.z);
                _trigersTutorial.ShowTutorialHint(nameButton, triggerOrder);
                    //Destroy(gameObject);
            }
                
            
        }

       
    }



    }

