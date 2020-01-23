using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsActivator : MonoBehaviour {

    public Rigidbody[] physicalObjects;
    
    private bool stay = true;
    public bool destroyOnEnd = false;
    void Start()
    {
        
        physicalObjects = GetComponentsInChildren<Rigidbody>();
        StartCoroutine(startDisablePhysic());
       
    }
    
    IEnumerator startDisablePhysic()
    {
        yield return new WaitForSeconds(3);
        disableRigidobyes();
    }

    IEnumerator startDestroy()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    public void disableRigidobyes()
    {
        if (stay == true)
        {
            foreach (Rigidbody go in physicalObjects)
            {

                go.isKinematic = true;
                go.useGravity = false;
                go.Sleep();
                //Debug.Log("OFF");

            }
            
            stay = false;
        }
    }

    public void disablePhysic()
    {
        if (destroyOnEnd)
        {
            destroyOnEnd = true;
            stay = false;
            StartCoroutine(startDestroy());
        }
            else
        {
            disableRigidobyes();
        }
        
    }

    public void enablePhysic()
    {
        if (stay == false)
        {
            foreach (Rigidbody go in physicalObjects)
            {

                go.WakeUp();
                go.useGravity = true;
                go.isKinematic = false;

            }
            Debug.Log("ON");
            stay = true;
        }
    }





}
