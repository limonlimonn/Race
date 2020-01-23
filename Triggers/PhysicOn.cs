using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicOn : MonoBehaviour {

	

    void OnTriggerEnter(Collider other)
    {
       if(other.tag == "physicObject")
        {
            Debug.Log("ON");
            other.GetComponent<PhysicsActivator>().enablePhysic();
        }
    }
}
