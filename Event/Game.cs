using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HCR.Event.UIControl;
using HCR.Event.Track;
using HCR.Event.Car;

namespace HCR.Event
{
    public class Game
    {
        private ButtonControl buttonControl = new ButtonControl();

        public ref ButtonControl ButtonControl
        {
            get { return ref buttonControl; }
        }

        private MenuButton menuButton = new MenuButton();

        public ref MenuButton MenuButton
        {
            get { return ref menuButton; }
        }

        private TrackEvent trackEvent = new TrackEvent();

        public ref TrackEvent TrackEvent
        {
            get { return ref trackEvent; }
        }

        private CarEvent carEvent = new CarEvent();

        public ref CarEvent CarEvent
        {
            get { return ref carEvent; }
        }


        private PlayerEvent playerEvent = new PlayerEvent();

        public ref PlayerEvent PlayerEvent
        {
            get { return ref playerEvent; }
        }

        public void Invoke_NewGame()
        {
            trackEvent.RemoveAllEvent();
            carEvent.RemoveAllEvent();
        }

        public void Invoke_InitAll()
        {
            trackEvent.Invoke_Init();
            carEvent.Invoke_Init();
        }

    }
}
