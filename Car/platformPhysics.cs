using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformPhysics : MonoBehaviour {

    // Use this for initialization
    public int speed = 1;
    public Transform platform;
    private float currentX;
    private float previuosX;
    private float xDif;
    private Vector3 moveVector;
    private Rigidbody fixedRB;
    private Vector3 tmpVector;
    private bool move = false;
	void Start () {
        previuosX = platform.position.x;
        moveVector = new Vector3(0, 0, 0);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        currentX = platform.position.x;
        xDif = (currentX - previuosX);
        moveVector.x = xDif;
        previuosX = platform.position.x;
        if (move)
        {
            tmpVector = moveVector * Mathf.Sqrt(fixedRB.mass) * 50 * speed;
            fixedRB.AddForce(tmpVector);
            Debug.Log("move " + (tmpVector) );
        }
        
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "BodyCollider")
        {
            fixedRB = other.GetComponentInParent<Rigidbody>();
            move = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "BodyCollider")
        {
            move = false;
        }
    }
}
