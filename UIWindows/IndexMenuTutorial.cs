using HCR.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace HCR
{
    public class IndexMenuTutorial : MonoBehaviour, IUIWindow
    {

        [SerializeField]
        protected CanvasGroup _canvaseGroup;

        private UIManager _uiManger;

        public void Show()
        {
            WindowViewHandler.Show(_canvaseGroup);
        }


        public void Hide()
        {
            WindowViewHandler.Hide(_canvaseGroup);
        }

       
        
        public void Init()
        {
            _uiManger = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManger);
        }
    }
}

