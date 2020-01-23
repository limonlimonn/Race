using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ZSpeed : MonoBehaviour
{
    private Rigidbody curspeed;
    private float maxspeed;

    public float possitonX, possitonY , neededY;

    private WorkShopCamTest MainCamera;
    public BikeFollowObject followObject;
    private EVP.VehicleController vehicleCntrl;

    public float followXMultiplyForward = -2;
    public float followXMultiplyBack = 12;

    public float diapazon_z = 4f;
    public float min_z = 12f;
    public float step_z = 0.05f;

    public float diapzon_y = 1.5f;
    public float sdvig_y = 1.5f;
    public float step_y = 0.05f;

    private float position_z;
    private float position_y;
    private Dictionary<int, float> positionsX = new Dictionary<int, float>();
    private Dictionary<int, float> positionsY = new Dictionary<int, float>();
    private float prev_x, cur_x, prev_y, cur_y, dif_y, needed_y;


    private float speedPercent;
    private Transform mainTransform, carTransform;
    private Quaternion mainQuan;

    private Vector3 lastVelocity = Vector3.zero;
    private Vector3 acceleration = Vector3.zero;
    public float BikeFollowStartX = 2.5f;
    private float needed_x;
    private float dif_x;
    private float dif_pos_x;
    private float dif_pos_y;

    public float eX = 10, eY = 210, eZ = 0f;
    public float minX = 5f, maxX = 5f, dipY = 10f, dZ = 5f;
    private float tempRy;
    public float rX, rY, rZ;

    private float flyDiapazonZ = 0f;
    // Use this for initialization
    void Start()
    {
        Init();
        followObject.possitonX = BikeFollowStartX;
    }

    public void Init()
    {
        curspeed = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        vehicleCntrl = GameObject.FindGameObjectWithTag("Player").GetComponent<EVP.VehicleController>();
        maxspeed = vehicleCntrl.maxSpeedForward;
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<WorkShopCamTest>();
        mainTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        carTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        
    }



    void Update()
    {
        if (curspeed != null)
        {
            if (vehicleCntrl.wheels[0].wheelCollider.isGrounded && vehicleCntrl.wheels[0].wheelCollider.isGrounded)
            {
                flyDiapazonZ = 0;
            }
            else {
                flyDiapazonZ = 3.5f;
            }

            speedPercent = curspeed.velocity.magnitude / maxspeed;

            if (speedPercent > 1.2f) {
                speedPercent = 1.2f;
            }

            position_z = min_z + (diapazon_z + flyDiapazonZ) * speedPercent;
            
            if (MainCamera.distanceZ < position_z)
            {
                MainCamera.distanceZ = MainCamera.distanceZ + step_z;
            }
           
            if (MainCamera.distanceZ > position_z)
            {
                MainCamera.distanceZ = MainCamera.distanceZ - step_z;
            }
            
           position_y = speedPercent * diapzon_y - sdvig_y;

           if (MainCamera.distanceY < position_y)
           {
               MainCamera.distanceY = MainCamera.distanceY + step_y;
           }

           if (MainCamera.distanceY > position_y)
           {
               MainCamera.distanceY = MainCamera.distanceY - step_y;
           }
           

            positionsX.Add(Time.frameCount, carTransform.position.x);

            if (positionsX.Count > 2)
            {
                positionsX.Remove(Time.frameCount - 2);

                prev_x = positionsX[Time.frameCount - 1];
                cur_x = positionsX[Time.frameCount];
                if (curspeed.velocity.normalized.x < 0)
                {
                    dif_x = (float)Math.Round(cur_x - prev_x, 4);
                }
                else {
                    dif_x = (float)Math.Round(cur_x - prev_x, 4);
                }
                    
            }
             
            
            if (curspeed.velocity.normalized.x < 0)
            {
                needed_x = BikeFollowStartX + dif_x * followXMultiplyForward;

                dif_pos_x = (float)Math.Round(Math.Abs(followObject.possitonX - needed_x) / 100, 4);

                if (followObject.possitonX < needed_x)
                {
                    followObject.possitonX += 0.001f + dif_pos_x;
                }


                if (followObject.possitonX > needed_x)
                {
                    followObject.possitonX -= 0.001f + dif_pos_x;
                }
            }
            else 
            {
                needed_x = BikeFollowStartX + dif_x * followXMultiplyBack;

                dif_pos_x = (float)Math.Round(Math.Abs(followObject.possitonX - needed_x) / 100, 4);

                if (followObject.possitonX < needed_x)
                {
                    followObject.possitonX += 0.001f + dif_pos_x;
                    //followObject.possitonX -= 0.001f + dif_pos_x;
                }


                if (followObject.possitonX > needed_x)
                {
                    //followObject.possitonX += 0.001f + dif_pos_x;
                    followObject.possitonX -= 0.001f + dif_pos_x;
                }
            }
            
            
            positionsY.Add(Time.frameCount, carTransform.position.y);
            if (positionsY.Count > 2)
            {
                positionsY.Remove(Time.frameCount - 2);

                prev_y = positionsY[Time.frameCount - 1];
                cur_y = positionsY[Time.frameCount];
                dif_y = (float)Math.Round(cur_y - prev_y, 4);
            }



            needed_y = 0.1f - dif_y * 10;

            dif_pos_y = (float)Math.Round(Math.Abs(followObject.possitonY - needed_y) / 50, 4);

            followObject.neededY = dif_pos_y;


            if (followObject.possitonY < needed_y && followObject.possitonY < 1f)
            {
                followObject.possitonY += 0.001f + dif_pos_y;
            }
              
            if (followObject.possitonY > needed_y && followObject.possitonY > -1f)
            {
                followObject.possitonY -= 0.001f + dif_pos_y/2; 
            }

            //X rotation
            if (rX < (eX + maxX) && dif_y < 0) {
                rX += 0.05f + -dif_y/5;
            }

            if (rX > (eX - minX) && dif_y > 0)
            {
                rX -= 0.05f + dif_y /5;
            }

            //Y rotation
            
            tempRy = eY + dipY * speedPercent;
            
            

            if (rY < tempRy) {

                rY += 0.05f + (tempRy - rY)/10;
            }

            if (rY > tempRy )
            {
                rY -= 0.05f - (tempRy - rY)/10;
            }

            //Z rotation
            if (rZ < (eZ + dZ) && dif_y < 0)
            {
                rZ += 0.05f + -dif_y / 5;
            }

            if (rZ > (eZ - dZ) && dif_y > 0)
            {
                rZ -= 0.05f + dif_y / 5;
            }

            mainTransform.rotation = Quaternion.Euler(rX, rY, rZ);

        }

        possitonX = followObject.possitonX;
        possitonY = followObject.possitonY;
        neededY = followObject.neededY;
    }
}
