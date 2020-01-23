using UnityEngine;
using System.Collections.Generic;
using System.Collections;


using HCR.Interfaces;
using System;
using System.IO;
using HCR.Event;
using HCR.Event.Car;

namespace HCR
{
	public class Core : MonoBehaviour
	{
        // PROPERTIES

      

        public bool ingoreTutorial = true;
        public bool isTestCore = false;
        //public string PING = "8.8.8.8";
		public static Core Instance {
			get { return _instance; } }


        // FIELDS

        private static Core _instance;
		private List<IService> _services = new List<IService>();

        // SOUND
        private FMOD.Studio.Bus masterBus;

        

        private void Awake(){
           
            if (isTestCore) {

           InitTest();
           return;
       }   

       if (_instance == null)
       {
           _instance = this;
           DontDestroyOnLoad(this.gameObject);
           Init();
       }
       else if (_instance != this)
       {
           Destroy(this.gameObject);
       }

          




        }

       
        public void IsNetwork(float time,Action<bool> OnComplete )
{
//Debug.Log("IsNetwork");
//Ping ping = new Ping("8.8.8.8");
//StartCor(PingCheck(ping, time, (done) =>
//{
    OnComplete(true);
//}));

}

private static IEnumerator PingCheck(Ping ping, float time , Action<bool> OnComplete )
{

while(time > 0 && !ping.isDone)
{
    yield return new WaitForSecondsRealtime(0.001f);
    time -= 0.001f;
    Debug.Log(" time " + time);
}
Debug.Log(" ping.isDone " + ping.isDone);
OnComplete(ping.isDone);
}



// INTERFACES

public T GetService<T>() where T : IService
{
return (T)_services.Find(s => s is T);
}

private IService AddService(IService service)
{
_services.Add(service);

return service;
}

//
public Coroutine StartCor(IEnumerator coroutine)
{
return StartCoroutine(coroutine);
}

public void StopCor(Coroutine coroutine)
{
if(coroutine != null)
StopCoroutine(coroutine);
}

//
public void DestroySelf()
{
Destroy(gameObject);
}


// METHODS

private void Init()
{
masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
// init services

GameDataManager eventManager = GameDataManager._init;
AddService(GetComponent<UIAnimatorManager>());
AddService(new SafePlayerPrefs());
AddService(new CarConstructor());
AddService(new StampTimerManager());
AddService(new PlayerManager());
AddService(new NetworkManager());
AddService(GetComponentInChildren<UIManager>());
AddService(new GameManager());
AddService(new StatesManager());
AddService(new AudioService());
AddService(new StatisticsService());
AddService(new EventService());

//For record Objects
AddService(new RecordObjectController());
AddService(new ReplyObjectController());

// invoke services Init()
GetService<SafePlayerPrefs>().Init();
GetService<NetworkManager>().Init();
GetService<UIManager>().Init();
GetService<GameManager>().Init();
GetService<StatesManager>().Init();
GetService<AudioService>().Init();






}







public void InitTest() {

if (_instance == null)
{
    _instance = this;
    DontDestroyOnLoad(this.gameObject);

}
else if (_instance != this)
{
    Destroy(this.gameObject);
}

masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
AddService(new AudioService());
AddService(new SafePlayerPrefs());
AddService(new GameManager());
AddService(new UIManager());
GameObject.Find("DirectStart").GetComponent<SceneTestStart>().enabled = true;
var GameId = 10;
/*
//PlayerPrefs.DeleteKey(PPKeys.GameId + "my");
if (!PlayerPrefs.HasKey(PPKeys.GameId + "my"))
{

    Dictionary<string, PlayerSave> Games = new Dictionary<string, PlayerSave>();
    for (int i = 0; i < 10; i++)
    {
        PlayerSave player = new PlayerSave()
        {
            gameId = GameId + i,
            playerName = "eee" + i.ToString(),
            isGame = false,
            dool = 'd',
        };
        Games.Add(GameId.ToString(), player);
        GameId++;
    }
    foreach (var item in Games)
    {
        //Debug.Log("item" + item.Key + item.Value.playerName);
    }




    PlayerPrefs.SetString(PPKeys.GameId + "my", ObjectToGSData(Games).JSON.ToString());

    //Debug.Log("trt" + trt);
}else
{
    Debug.Log("HasKey == true");
    string Data = PlayerPrefs.GetString(PPKeys.GameId + "my");
    JObject respObj = JObject.Parse(Data);
    respObj.Add("main", "tries");
    Debug.Log("respObj " + respObj.ToString());
    foreach (var item in respObj.Childs)
    {
        Debug.Log("item" + item +" val " +item.Value);
    }
    for (int i = 0; i < 9; i++)
    {
        JObject playerData = new JObject();
        playerData = respObj[i.ToString()];
        string arr = playerData["gameId"];
        Debug.Log("arr"+i + " " + arr + "Count " + respObj.Count);
    }
    //DData.AddJSONStringAsObject("my", Data);



}
*/
        }

        private void CheckObb()
        {
#if !UNITY_EDITOR && UNITY_ANDROID

			bool install = true;

			if (!GooglePlayDownloader.RunningOnAndroid())
			{
				GUI.Label(new Rect(10, 10, Screen.width - 10, 20), "Use GooglePlayDownloader only on Android device!");
				return;
			}

			string expPath = GooglePlayDownloader.GetExpansionFilePath();

			if (expPath == null)
			{
				GUI.Label(new Rect(10, 10, Screen.width - 10, 20), "External storage is not available!");
			}
			else
			{
				string mainPath = GooglePlayDownloader.GetMainOBBPath(expPath);
				string patchPath = GooglePlayDownloader.GetPatchOBBPath(expPath);

				//GUI.Label(new Rect(10, 10, Screen.width-10, 20), "Main = ..."  + ( mainPath == null ? " NOT AVAILABLE" :  mainPath.Substring(expPath.Length)));
				//GUI.Label(new Rect(10, 25, Screen.width-10, 20), "Patch = ..." + (patchPath == null ? " NOT AVAILABLE" : patchPath.Substring(expPath.Length)));

				if ((mainPath == null || patchPath == null) && install)
				{
					//if (GUI.Button(new Rect(10, 100, 100, 100), "Fetch OBBs"))
					install = false;
				}

				GooglePlayDownloader.FetchOBB();
			}

#endif
        }


        public void Mute()
        {
            masterBus.setPaused(true);
        }

        public void UnMute()
        {
            masterBus.setPaused(false);
        }

        

    }
}