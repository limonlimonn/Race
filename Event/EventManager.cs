using HCR.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCR.Event
{
    public class EventManager 
    {
        private Game game = new Game();

        public ref Game Game
        {
            get { return ref game; }
        }

        static public EventManager _init = new EventManager();

    }

}