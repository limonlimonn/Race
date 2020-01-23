using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HCR.Event.UIControl
{
    public class ButtonControl
    {
        public delegate void ButtonEvent();

        public event ButtonEvent OnNitroPressed;
        public event ButtonEvent OnNitroUnPressed;
        public event ButtonEvent OnForwardPressed;
        public event ButtonEvent OnForwardUnPressed;
        public event ButtonEvent OnBackPressed;
        public event ButtonEvent OnBackUnPressed;
        public event ButtonEvent OnUpPressed;
        public event ButtonEvent OnUpUnPressed;
        public event ButtonEvent OnDownPressed;
        public event ButtonEvent OnDownUnPressed;


        public void Invoke_OnNitroPressed()
        {
            OnNitroPressed();
        }
        public void Invoke_OnNitroUnPressed()
        {
            OnNitroUnPressed();
        }
        public void Invoke_OnForwardPressed()
        {
            OnForwardPressed();
        }
        public void Invoke_OnForwardUnPressed()
        {
            OnForwardUnPressed();
        }
        public void Invoke_OnBackPressed()
        {
            OnBackPressed();
        }
        public void Invoke_OnBackUnPressed()
        {
            OnBackUnPressed();
        }
        public void Invoke_OnUpPressed()
        {
            OnUpPressed();
        }
        public void Invoke_OnUpUnPressed()
        {
            OnUpUnPressed();
        }
        public void Invoke_OnDownPressed()
        {
            OnDownPressed();
        }
        public void Invoke_OnDownUnPressed()
        {
            OnDownUnPressed();
        }



    }

    public abstract class Events
    {

    }

    
}
