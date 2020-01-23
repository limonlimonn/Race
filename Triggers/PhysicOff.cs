using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicOff : MonoBehaviour {

	
    void OnTriggerEnter(Collider other)
    {       
        if (other.tag == "physicObject")
        {
            Debug.Log("OFF");
            other.GetComponent<PhysicsActivator>().disablePhysic();
        }
    }
}
