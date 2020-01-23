using HCR.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Assertions;
using HCR.Enums;
using HCR.GlobalWindow.MainMenu;
using DG.Tweening;
using UnityEngine.UI;
using System.Runtime.InteropServices;

namespace HCR
{
    public class EventService : IService
    {
        private UIManager _uiManager;
        private EventWindow _eventWindow;
        private MainScreenStateManager _mainScreenStateManager;
        private PlayerManager _playerManager;
        private Header _header;
        private UIAnimatorManager _uiAnimatorManager;
        private AudioService _audioService;
        private ScorePanel _scorePanel;

        string audioPath = "event:/UI Sounds/button_click";

        public long DayFirstEndStamp;
        public long DayFirstNowStamp;
        public int DayFirstCuretStamp;

        public long WeekEndStamp;
        public long WeekNowStamp;
        public int WeekCuretStamp;

        public bool DayComplete;
        public bool DayCollect;
        public bool WeekComplete;
        public bool WeekCollect;

        private int DayGold;
        private int DayJewels;
        private int WeekGold;
        private int WeekJewels;

        Action DailyIsLoad;
        Action WeekIsLoad;
        Action HederIsLoad;
        public Dictionary<string, Dictionary<string, string>> Events = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<int, Dictionary<string, int>> Shop = new Dictionary<int, Dictionary<string, int>>();
        Coroutine DailyCoroutine = null;
        Coroutine WeekCoroutine = null;




        public void Init()
        {
            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);

            _eventWindow = _uiManager.GetWindow(UIWindowEnum.EVENT) as EventWindow;

            _mainScreenStateManager = _uiManager.Get_MainScreenStateManager();
            Assert.AreNotEqual(null, _mainScreenStateManager);

            _playerManager = Core.Instance.GetService<PlayerManager>();
            Assert.AreNotEqual(null, _playerManager);

            _uiAnimatorManager = Core.Instance.GetService<UIAnimatorManager>();
            Assert.AreNotEqual(null, _uiAnimatorManager);

            _audioService = Core.Instance.GetService<AudioService>();
            Assert.AreNotEqual(null, _audioService);

            _header = _uiManager.GetWindow(UIWindowEnum.HEADER) as Header;

            _scorePanel = _uiManager.GetWindow(UIWindowEnum.SCORE) as ScorePanel;

            InitEvents();

            _scorePanel.FillShop();


        }
        
        public bool IsEventComplete()
        {
            Debug.Log("IsEvent  " + DayCollect + " " + DayComplete+" "+WeekCollect+" "+WeekComplete);
            if ((!DayCollect && DayComplete) || (!WeekCollect && WeekComplete))
                return true;
            else return false;
        }


        public Dictionary<string, Dictionary<string, string>> Get_Events()
        {
            return Events;
        }

        public void InitEvents()
        { if (WeekCoroutine != null && DailyCoroutine != null) {
                //Debug.Log("StopCoroutine");
                Core.Instance.StopCoroutine(DailyCoroutine);
                Core.Instance.StopCoroutine(WeekCoroutine);
            }
            //__-Daily---
            DayFirstNowStamp = long.Parse(Events["Daily0"]["now_stamp"]);
            DayFirstEndStamp = long.Parse(Events["Daily0"]["end_stamp"]);
            DayFirstCuretStamp = (int)(DayFirstEndStamp - DayFirstNowStamp);
            //---Week---
            WeekEndStamp = long.Parse(Events["Week"]["end_stamp"]);
            WeekNowStamp = long.Parse(Events["Week"]["now_stamp"]);
            WeekCuretStamp = (int)((WeekEndStamp - WeekNowStamp) / 60);

            DayGold =  int.Parse(Events["Daily0"]["gold"]);
            DayJewels =  int.Parse(Events["Daily0"]["jewel"]);
            WeekGold =  int.Parse(Events["Week"]["gold"]);
            WeekJewels =  int.Parse(Events["Week"]["jewel"]);

            CheckCorutine();
        }

        #region Coroutine
        private void CheckCorutine()
        {
            DayComplete = bool.Parse(Events["Daily0"]["Complete"]);
//true;
            DayCollect =  bool.Parse(Events["Daily0"]["collect"]);
//false;
            WeekComplete = bool.Parse(Events["Week"]["Complete"]);
//true;
            WeekCollect = bool.Parse(Events["Week"]["collect"]);
//false;



            if (!DayCollect && !DayComplete)
            {
                _eventWindow.DayFirstComplete.SetActive(false);
                _eventWindow.DayFirstTime.text = "";
                DailyCoroutine = Core.Instance.StartCor(StartDay());
            }
            else if ((!DayCollect && DayComplete )|| DayCollect)
            {
                _eventWindow.DayFirstComplete.SetActive(true);
            }
            
            
            

            if (!WeekCollect && !WeekComplete)
            {
                _eventWindow.WeekComplete.SetActive(false);
                _eventWindow.WeekTime.text = "";
                WeekCoroutine = Core.Instance.StartCor(StartWeek());
            }
            else if((!WeekCollect && WeekComplete) || WeekCollect)
            {
                _eventWindow.WeekComplete.SetActive(true);
            }

            //if ((!DayCollect && DayComplete) || (!WeekCollect && WeekComplete)) { Core.Instance.StartCor(Wait()); }
        }
        //private IEnumerator Wait() { yield return new WaitForSecondsRealtime(1); ShowCompleteAnim(); }

        public IEnumerator StartWeek()
        {
            _eventWindow.WeekTime.text = TimerForWeek();
            while (WeekCuretStamp > 0)
            {
                yield return new WaitForSecondsRealtime(59);
                WeekCuretStamp--;
                _eventWindow.WeekTime.text = TimerForWeek();
            }

            Core.Instance.GetService<NetworkManager>().TimeEventIsOver();


            // UPDATE EVENT!!!!!!!!!!!!!
        }

        public IEnumerator StartDay()
        {
            _eventWindow.DayFirstTime.text = TimerForDay();
            while (DayFirstCuretStamp > 0)
            {
                yield return new WaitForSecondsRealtime(1);
                DayFirstCuretStamp--;
                _eventWindow.DayFirstTime.text = TimerForDay();
            }

            Core.Instance.GetService<NetworkManager>().TimeEventIsOver();
            // UPDATE EVENT!!!!!!!!!!!!!
        }

        public void ShowCompleteAnim(Action OnComplete)
        {
            _mainScreenStateManager.SwitchState(MainScreenStatesEnum.EVENT);
            
            if (!DayCollect && DayComplete && !WeekCollect && WeekComplete)
            {
                Debug.LogError("Bouth");
                _eventWindow.DayFirstComplete.SetActive(false);
                _eventWindow.WeekComplete.SetActive(false);

                
                _header.gold.text = (_playerManager.gold - DayGold - WeekGold).ToString();
                
                _header.jewels.text = (_playerManager.jewels - DayJewels - WeekJewels).ToString();
                _mainScreenStateManager.SwitchState(MainScreenStatesEnum.EVENT);
                
                _eventWindow.DayColectGold.transform.DOPunchScale(new Vector3(0.1f, 0.1f), 1.5f, 1, 1).OnComplete(() => { DayGoldsAnim(); DayGoldsAnimHeder(); });

                 DailyIsLoad += () => {

                     _eventWindow.WeekColectGold.transform.DOPunchScale(new Vector3(0.1f, 0.1f), 1.5f, 1, 1).OnComplete(() => {
                         WeekGoldsAnim(); WeekGoldsAnimHeder();
                     });

                 };

                WeekIsLoad += () =>
                 {
                     _eventWindow.DayFirstComplete.SetActive(true);
                     _eventWindow.WeekComplete.SetActive(true);
                     if (WeekCoroutine != null && DailyCoroutine != null)
                     {
                         Core.Instance.StopCoroutine(DailyCoroutine);
                         Core.Instance.StopCoroutine(WeekCoroutine);
                     }
                     _eventWindow.DayFirstTime.text = "";
                     _eventWindow.WeekTime.text = "";
                     OnComplete();
                 };

            }
            else if (!WeekCollect && WeekComplete)
            {
                _eventWindow.WeekComplete.SetActive(false);
                _header.gold.text = (_playerManager.gold - int.Parse(Events["Week"]["gold"])).ToString();
                
                _header.jewels.text = (_playerManager.jewels - int.Parse(Events["Week"]["jewel"])).ToString();

                _mainScreenStateManager.SwitchState(MainScreenStatesEnum.EVENT);
                
                _eventWindow.WeekColectGold.transform.DOPunchScale(new Vector3(0.1f, 0.1f), 1.5f, 1, 1).OnComplete(() => { WeekGoldsAnim(); WeekGoldsAnimHeder(); });

                WeekIsLoad += () =>
                {
                    _eventWindow.WeekComplete.SetActive(true);
                    if (WeekCoroutine != null)
                    {
                        Core.Instance.StopCoroutine(WeekCoroutine);
                    }
                    
                    _eventWindow.WeekTime.text = "";
                    OnComplete();
                };
            }
            else if (!DayCollect && DayComplete)
            {
                _eventWindow.DayFirstComplete.SetActive(false);
                DailyIsLoad += InitEvents;
                _header.gold.text = (_playerManager.gold - DayGold).ToString();
                _header.jewels.text = (_playerManager.jewels - DayJewels).ToString();
                _mainScreenStateManager.SwitchState(MainScreenStatesEnum.EVENT);
               
                _eventWindow.DayColectGold.transform.DOPunchScale(new Vector3(0.1f, 0.1f), 1.5f, 1, 1).OnComplete(() => { DayGoldsAnim(); DayGoldsAnimHeder(); });

                DailyIsLoad += () =>
                {
                    _eventWindow.DayFirstComplete.SetActive(true);
                    if (DailyCoroutine != null)
                    {
                        Core.Instance.StopCoroutine(DailyCoroutine);
                    }

                    _eventWindow.DayFirstTime.text = "";
                    OnComplete();
                };

            }
        }

        public string TimerForDay()
        {
            string time = "";
            int hours = DayFirstCuretStamp / 3600;
            
            int min = DayFirstCuretStamp / 60 - hours * 60;
            int second = DayFirstCuretStamp % 60;
            //Debug.Log("hours" + hours + ":" + min + ":" + second);

            time = (hours > 9 ? hours.ToString() : "0" + hours.ToString()) + ":" + ((min > 9) ? min.ToString() : "0" + min.ToString()) + ":" + (second > 9 ? second.ToString() : "0" + second.ToString());


            //Debug.Log("time" + time );

            return time;
        }

        public string TimerForWeek()
        {
            string time = "";
            int day = WeekCuretStamp / 60 / 24;
            int hours = WeekCuretStamp / 60 - 24 * day;
            int min = WeekCuretStamp - hours * 60 - 24 * day * 60;

            time = "Day:" + (day > 9 ? day.ToString() : "0" + day.ToString()) + " hours:" + (hours > 9 ? hours.ToString() : "0" + hours.ToString()) + " min:" + (min > 9 ? min.ToString() : "0" + min.ToString());
            return time;
        }
        #endregion

       
        #region ANIMATION
        private void DayGoldsAnimHeder()
        {
            if (DayGold != 0) 
            HederAnimation(_header.gold,_playerManager.gold - WeekGold, _eventWindow.DayColectGold, HederIsLoad);
            
        }
        private void DayGoldsAnim()
        {
            if (DayGold != 0)
                WindowAnimation(_eventWindow.DayColectGold, 0, DayJewelsAnim);
            else DayJewelsAnim();
        }

        private void DayJewelsAnim()
        {
            if (DayJewels != 0)
                _eventWindow.DayColectJewel.transform.DOPunchScale(new Vector3(0.1f, 0.1f), 1.5f, 1, 1).OnComplete(() =>
                {
                    HederAnimation(_header.jewels, _playerManager.jewels - WeekJewels, _eventWindow.DayColectJewel, HederIsLoad);
                    WindowAnimation(_eventWindow.DayColectJewel, 0, DailyIsLoad);
                });
            else DailyIsLoad();
        }

        private void WeekGoldsAnimHeder()
        {
            
            if(WeekGold != 0)
            HederAnimation(_header.gold, _playerManager.gold, _eventWindow.WeekColectGold, HederIsLoad);
            
        }

        private void WeekGoldsAnim()
        {
            
            if (WeekGold != 0)
                WindowAnimation(_eventWindow.WeekColectGold, 0, WeekJewelsAnim);
            else WeekJewelsAnim();
        }
        private void WeekJewelsAnim()
        {
            
            if (WeekJewels != 0)
                _eventWindow.DayColectJewel.transform.DOPunchScale(new Vector3(0.1f, 0.1f), 1.5f, 1, 1).OnComplete(() =>
                {
                    WindowAnimation(_eventWindow.WeekColectJewel, 0, WeekIsLoad);
                    HederAnimation(_header.jewels, _playerManager.jewels,  _eventWindow.WeekColectJewel, HederIsLoad);
                   
                });
            else WeekIsLoad();
        }


        private void HederAnimation(Text heder, int price, Text speed,  Action Action)
        {
            heder.transform.DOPunchScale(new Vector3(0.1f, 0.1f), 1 / (float.Parse(speed.text)+1) , 1, 1).OnComplete(() => {
                if (heder.text != price.ToString())
                {
                    
                    if ((price- int.Parse(heder.text) ) <= 3 || (float.Parse(heder.text)% (float.Parse(heder.text)/10) == 0))
                    {
                        //Debug.Log(price - int.Parse(heder.text));
                       
                             
                        _audioService.RM_PlayOneShot(audioPath);
                    }
                    heder.text = (int.Parse(heder.text) + 1).ToString();
                    HederAnimation(heder, price,  speed, Action);
                    
                }
                else
                {
                    //Debug.Log("Action : " + Action + " window " + heder.name);
                }
            });
        }

        private void WindowAnimation(Text window, int price, Action Action)
        {
            window.transform.DOPunchScale(new Vector3(0.1f, 0.1f), 1/((float.Parse(window.text)+1)), 1, 1).OnComplete(() => {
                if (window.text != price.ToString())
                {
                    window.text = (int.Parse(window.text) - 1).ToString();
                    WindowAnimation(window, price,  Action);
                }else
                {
                    
                    
                    Action();
                }
            });
        }
        #endregion
    }
}
