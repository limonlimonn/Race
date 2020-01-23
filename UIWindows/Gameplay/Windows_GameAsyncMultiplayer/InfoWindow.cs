using DG.Tweening;
using HCR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindow : MonoBehaviour
{
    
    public Text RecordingFor_Text;
    public Text TimeText;
    public Text InfoWindowPreTime;
    public GameObject TransformTimeInfo;
    public GameObject StartPosition;


    public Action TimeIsOver;

    public void Init()
    {
        Set_Default();
        OnOffWindow(true);
    }

    public void Load()
    {
        Set_Default();
    }

    public void StartRace()
    {
        Set_RecordForWhom("");
    }

    public void InitWindow()
    {
       
    }

    public void HideInfo_Text()
    {
        Set_InfoWindowPreTime("");
        Set_TimeText("");
    }

    public void Restart()
    {
        Set_RecordForWhom("");
        Set_TimeText("");
        Set_InfoWindowPreTime("");
        Set_ColorTimeText(Color.white);
    }

    public void Set_Default()
    {
        Set_RecordForWhom("");
        Set_TimeText("");
        Set_InfoWindowPreTime("");
        Set_ColorTimeText(Color.white);
        GoToDefaultTimeInfo();
    }

    public void Set_RecordForWhom(string text)
    {
        Debug.Log("RecordForWhom " + text);
        RecordingFor_Text.text = text;
    }
    
    public void Set_TimeText(string text)
    {
        TimeText.text = text;
    }

    public void Set_InfoWindowPreTime(string text)
    {
        InfoWindowPreTime.text = text;
    }

    public void GoToDefaultTimeInfo()
    {
        TimeText.transform.position = StartPosition.transform.position;
    }

    public void GoTo_Position()
    {
        TimeText.transform.DOMove(new Vector3(TransformTimeInfo.transform.position.x, TransformTimeInfo.transform.position.y, 0f), 1f);
    }

    public void Set_TimeIsOver()
    {
        Set_ColorTimeText(Color.red);
    }

    private void Set_ColorTimeText(Color color)
    {
        TimeText.color = color;
    }

    private void OnOffWindow(bool on)
    {
        RecordingFor_Text.gameObject.SetActive(on);
        TimeText.gameObject.SetActive(on);
        InfoWindowPreTime.gameObject.SetActive(on);
    }
    
}
