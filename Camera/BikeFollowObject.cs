using UnityEngine;
using System.Collections;

public class BikeFollowObject : MonoBehaviour
{


    [SerializeField]
    [HideInInspector]
    public float possitonX;

    [SerializeField]
    [HideInInspector]
    public float possitonY, neededY;
    private GameObject bike;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        bike = GameObject.FindGameObjectWithTag("target");
    }

    void FixedUpdate()
    {
        if (bike != null)
        {
            Vector3 pos = bike.transform.position;

            pos.y += possitonY;
            pos.x += possitonX;


            transform.position = pos;
        }
    }

}
