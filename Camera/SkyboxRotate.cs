using UnityEngine;
using System.Collections;

public class SkyboxRotate : MonoBehaviour {

    public float curRot = 0;
    public float speed = 50;
    public float perc = 360;
    void Update()
    {
        curRot += speed * Time.deltaTime;
        curRot %= perc;
        RenderSettings.skybox.SetFloat("_Rotation", curRot);
    }
}
