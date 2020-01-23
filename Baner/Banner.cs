using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using GoogleMobileAds.Api;

[Serializable]
public class Banner
{
    public BannerSettings bannerSettings;

    public void ShowBaner()
    {
        float height = bannerSettings.Height;
        float width = bannerSettings.Width;
        //bannerSettings.GetSizeBaner(out width, out height);
#if (UNITY_ANDROID || UNITY_IOS) &&!UNITY_EDITOR
         if (bannerSettings.TestBaner != null)
        {
            bannerSettings.TestBaner.SetActive(false);
        }
        AdSize size = new AdSize((int)width, (int)height);
        AdMobManager.adMob.InitBaner(size, bannerSettings.Position);
#endif
#if UNITY_EDITOR

        if (bannerSettings.TestBaner != null)
        {
            Debug.Log("Resize" + width + " " + height);
            bannerSettings.TestBaner.
                GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            bannerSettings.TestBaner.
                GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        }
#endif

    }

    public void HideBaner()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        Debug.Log("UNITY_ANDROID");
        AdMobManager.adMob.HideBaner();
    
#endif
    }
}
