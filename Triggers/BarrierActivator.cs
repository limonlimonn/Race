using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierActivator : MonoBehaviour {

    public GameObject[] gameObjects;

    // Use this for initialization
    void Start () {
        int i;
        gameObjects = new GameObject[transform.childCount];

        for (i = 0; i < transform.childCount; i++)
        {
            gameObjects[i] = transform.GetChild(i).gameObject;
        }
        disableObjects();

    }
	
	public void disableObjects()
    {
        foreach(GameObject go in gameObjects)
        {
            go.SetActive(false);
        }
    }

    public void enableObjects()
    {
        foreach (GameObject go in gameObjects)
        {
            go.SetActive(true);
        }
    }

    public void destroyObject()
    {
        Destroy(gameObject);
    }
}
