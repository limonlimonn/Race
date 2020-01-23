using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineActivator : MonoBehaviour
{
    private bool mineOrder = true;
    private Rigidbody rb;
    public Detonator bomb;
    public float power = 300000f;
    public float radius = 1;
    public MeshRenderer MeshRender;
    public Color color;
    // Use this for initialization
    void Start()
    {

    }
 
    
    void OnTriggerEnter(Collider other)
    {
        

        if (other.tag == "WheelCapsule" && mineOrder)
        {


            mineOrder = false;

            StartCoroutine(Boom(other));
        }       
    }
    
    IEnumerator Boom( Collider other) {
        MeshRender.materials[0].SetColor("_EmissionColor", color);
        yield return new WaitForSeconds(0.5f);
        bomb.Explode();
        other.GetComponentInParent<Rigidbody>().AddExplosionForce(power, transform.position, radius, 3.0F);
        
       // mineOrder = true;
        
    }

    


}