using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
	[SerializeField] private Text _textFPS;

    float deltaTime = 0.0f;
    private int framenumber = 0;

	private float _fps = 0f;
	private float _avg = 0f;




	private void Start()
	{
		StartCoroutine(DelayedFpsShow());
	}


	void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	    framenumber += 1;

	    _fps = 1.0f / deltaTime;
	    _avg = framenumber/Time.timeSinceLevelLoad;


    }

	IEnumerator DelayedFpsShow()
	{
		while (true)
		{
			 _textFPS.text = string.Format("{0:0.} avg | {1:0.} fps", _avg, _fps);
			 yield return new WaitForSecondsRealtime(0.5f);


		}
	}








//    void OnGUI()
//    {
//        int w = Screen.width, h = Screen.height;
//
//        GUIStyle style = new GUIStyle();
//
//        Rect rect = new Rect(0, 0, w, h * 2 / 100);
//        style.alignment = TextAnchor.UpperRight;
//        style.fontSize = h * 4 / 100;
//        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
//        float msec = deltaTime * 1000.0f;
//        float fps = 1.0f / deltaTime;
//        float avg = framenumber/Time.timeSinceLevelLoad;
//
//	    //
//	    Debug.Log("fps = " + fps);
//
//
//	    //
//        string text = string.Format("{0:0.} avg {1:0.} fps", avg, fps);
//        GUI.Label(rect, text, style);
//    }




}