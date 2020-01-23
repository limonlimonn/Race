using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YashikBoom_1 : MonoBehaviour {

    private Animator _animator;
	// Use this for initialization
	void Start () {
        _animator = GetComponent<Animator>();

    }
    void OnTriggerEnter(Collider other)
{
        if(other.tag == "BodyCollider")
        {
            _animator.SetBool("Boom1", true);
        }
}
        // Update is called once per frame
        void Update () {
		
	}
}
