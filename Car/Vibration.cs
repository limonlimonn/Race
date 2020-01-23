using UnityEngine;
using System.Collections;

public static class Vibration
{

#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaObject vibrator;
#endif
    public static bool IsOn = true;
    public static void Vibrate()
    {
        if (IsOn)
        {
            if (isAndroid())
                vibrator.Call("vibrate");
           // else
              //  Handheld.Vibrate();
        }
    }


    public static void Vibrate(long milliseconds)
    {
        if (IsOn)
        {
            if (isAndroid())
                vibrator.Call("vibrate", milliseconds);
            //else
               // Handheld.Vibrate();
        }
    }

    public static void Vibrate(long[] pattern, int repeat)
    {

        if (isAndroid())
            vibrator.Call("vibrate", pattern, repeat);
        //else
          //  Handheld.Vibrate();
    }
    public static IEnumerator WaitAndVibro(long Durability, float Delay, int Iterator)
    {
        if (IsOn)
        {
            YieldInstruction _fupd = new WaitForSeconds(Delay);
            for (int i = 0; i < Iterator; i++)
            {
                yield return _fupd;
                Vibration.Vibrate(Durability);

            }
        }
    }

    public static bool HasVibrator()
    {
        return isAndroid();
    }

    public static void Cancel()
    {
        if (isAndroid())
            vibrator.Call("cancel");
    }

    private static bool isAndroid()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
	return true;
#else
        return false;
#endif
    }
}
