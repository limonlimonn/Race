using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicStay : MonoBehaviour {


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "physicObject")
        {
            
            try
            {
                other.GetComponent<PhysicsActivator>().enablePhysic();
            }
            catch(Exception)
            {
                Debug.LogError(other.name);
            }
        }
        
    }


    void OnTriggerExit(Collider other)
    {
        if (other.tag == "physicObject")
        {
            try
            {
                other.GetComponent<PhysicsActivator>().disablePhysic();
            }
            catch (Exception)
            {
                Debug.LogError(other.name);
            }
        }

    }
}
