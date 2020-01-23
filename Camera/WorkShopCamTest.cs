using UnityEngine;
using System.Collections;

public class WorkShopCamTest : AngarCameraTest
{

    //--------------------------------------
    // INITIALIZE
    //--------------------------------------
    

    //--------------------------------------
    // PUBLIC METHODS
    //--------------------------------------

    public override void Update()
    {
        
        if (Input.GetMouseButtonDown(1))
        {
            IsInRotationMode = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            IsInRotationMode = false;
        }

    }

    //--------------------------------------
    // GET / SET
    //--------------------------------------

    //--------------------------------------
    // EVENTS
    //--------------------------------------

    //--------------------------------------
    // PRIVATE METHODS
    //--------------------------------------

    //--------------------------------------
    // DESTROY
    //--------------------------------------
}
