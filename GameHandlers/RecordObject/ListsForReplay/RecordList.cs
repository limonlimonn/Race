using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordList : MonoBehaviour
{
    public List<Rigidbody> RecordObjList;

    private void Awake()
    {
        //RecordObjList = new List<Rigidbody>();
        //foreach (GameObject val in GameObject.FindGameObjectsWithTag("RecordableObj"))
        //{
        //    RecordObjList.Add(val.GetComponent<Rigidbody>());
        //}
    }
    public List<Rigidbody> GetList()
    {
        return RecordObjList;
    }
	
}
