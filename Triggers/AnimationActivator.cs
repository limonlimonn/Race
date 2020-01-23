using HCR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationActivator : MonoBehaviour {

    private AudioService _audioService;
    private Animator _animator;
    private bool boostOrder = true;
    public string AudioPath = "event:/Action/break_wood";
  

    void Start () {
        _animator = GetComponent<Animator>();
        _animator.speed = 0f;
        _audioService = Core.Instance.GetService<AudioService>();
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "BodyCollider" && boostOrder)
        {
             _audioService.RM_PlayOneShot(AudioPath);
            boostOrder = false;
            _animator.speed = 1f;
            

        }
    }
}

