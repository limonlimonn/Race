using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Events();

public interface IEvent
{
    event Events InitEvents;
    event Events InitSettings;

    void Invoke_Init();
}
