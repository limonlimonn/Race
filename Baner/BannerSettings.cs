using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GoogleMobileAds.Api;

[Serializable]
public class BannerSettings
{
    public float Width;
    public float Height;
    public AdPosition Position;
    public GameObject TestBaner;

    public void GetSizeBaner(out float width, out float height)
    {
        width = Camera.main.pixelWidth / (1920 / Width);
        height = Camera.main.pixelHeight / (1080 / Height);
    }
}
