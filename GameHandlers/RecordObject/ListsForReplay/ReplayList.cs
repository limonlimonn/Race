using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayList : MonoBehaviour
{
    public List<Rigidbody> ReplayObjList;

    private void Awake()
    {
        //ReplayObjList = new List<Rigidbody>();
        //foreach (GameObject val in GameObject.FindGameObjectsWithTag("ReplayObj"))
        //{
        //    ReplayObjList.Add(val.GetComponent<Rigidbody>());
        //}
    }
    public List<Rigidbody> GetList()
    {
        return ReplayObjList;
    }
}
