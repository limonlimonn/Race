using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HCR;

public class AnimationDragActivator : MonoBehaviour {


    private Animator _animator;
    private Rigidbody rb;
    private CarBase cb;
    private bool boostOrder = true;
    private float slowDrag = 5.5f;
    public float slowPower = 15000;
    private Vector3 vector;
    private float baseDrag = 0.1f;
    public string AudioPath = "event:/Action/break_wood";
    public GameObject explosion;
    private AudioService _audioService;

    void Start () {

        _audioService = Core.Instance.GetService<AudioService>();
        _animator = GetComponent<Animator>();
        _animator.speed = 0f;
    }

    void OnTriggerEnter(Collider other)
    {
       
        if (other.tag == "BodyCollider"  && boostOrder)
        {
            
            Vibration.Vibrate(130);
            rb = other.GetComponentInParent<Rigidbody>() as Rigidbody;
            cb = other.GetComponentInParent<CarBase>() as CarBase;

            boostOrder = false;
            _audioService.RM_PlayOneShot(AudioPath);
            explosion.SetActive(true);
            StartCoroutine(slowDownBody());
            _animator.speed = 1f;
        }
        else if (other.tag == "Barrier" && boostOrder)
        {
            
            boostOrder = false;
            _audioService.RM_PlayOneShot(AudioPath);
            explosion.SetActive(true);
            _animator.speed = 1f;
        }

        
    }

    IEnumerator slowDownBody() {
        float massSqrt = Mathf.Sqrt(rb.mass / 100);
        float dragPercent = cb.controller.speed / cb.controller.maxSpeedForward;
        float pizdos = (cb.controller.speed * rb.mass) / (cb.controller.maxSpeedForward * 3000);
        if (dragPercent > 1)
        {
            dragPercent = 1;
        }

        rb.drag = slowDrag - pizdos * slowDrag;

        YieldInstruction _wait = new WaitForSeconds(rb.drag/50);
        yield return _wait;
        rb.drag = baseDrag;
       
    }

    




}
