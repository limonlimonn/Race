using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCR.Event.Track
{
    public class TrackEvent : IEvent
    {
        public event Events InitEvents;
        public event Events InitSettings;


        private Event Event = new Event();
        public Event GetEvent { get { return Event; } }

        public void Invoke_Init()
        {
            Debug.Log("Track Init");
            if(InitSettings != null)
            InitSettings();
            if (InitEvents != null)
            InitEvents();
        }

        public void RemoveAllEvent()
        {
            Debug.LogError("Track RemoveAll");
            InitEvents = null;
            InitSettings = null;
            Event.RemoveAllEvent();
        }
    }

    public class Event
    {
        public delegate void EventTrack();

        public event EventTrack Start;
        public event EventTrack OnLoad;
        public event EventTrack Restart;
        public event EventTrack Awake;

        public void Invoke_Start()
        {
            if (Start != null)
            {
                Debug.LogError("EVENT Start");
                Start();
            }
        }

        public void Invoke_Awake()
        {
            if (Awake != null)
            {
                Debug.LogError("EVENT Awake");
                Awake();
            }
        }

        public void Invoke_Restart()
        {
            if (Restart != null)
            {
                Debug.LogError("EVENT Restart");
                Restart();
            }
        }

        public void Invoke_OnLoad()
        {
            if (OnLoad != null)
            {
                Debug.LogError("EVENT OnLoad");
                OnLoad();
            }
        }

        public void RemoveAllEvent()
        {
            Start = null;
            OnLoad = null;
            Restart = null;
        }
    }
}

