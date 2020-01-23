using HCR.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Assertions;
using HCR.Enums;
using DG.Tweening;

namespace HCR
{
    public class EventWindow : MonoBehaviour, IUIWindow
    {
        public CanvasGroup canvaseGroup;
        private EventService _eventService;
        private UIManager _uiManager;

        [Header("Daily_1")]
        public Text DayFirstType;
        public Text DayFirstProgress;
        public Text DayFirstDescription;
        public Text DayFirstTime;
        public GameObject DayFirstComplete;
        public Text DayColectGold;
        public Text DayColectJewel;
        [Header("Week")]
        public Text WeekType;
        public Text WeekProgress;
        public Text WeekDescription;
        public Text WeekTime;
        public GameObject WeekComplete;
        public Text WeekColectGold;
        public Text WeekColectJewel;



        public void Show()
        {

            //canvaseGroup.alpha = 1;
            _uiManager.ShowCanvas(canvaseGroup);
            canvaseGroup.blocksRaycasts = true;

            _eventService = Core.Instance.GetService<EventService>();
            InitEvent(_eventService.Get_Events());
            

        }

        public void Hide()
        {
            //canvaseGroup.alpha = 0;
            _uiManager.HideCanvas(canvaseGroup);
            canvaseGroup.blocksRaycasts = false;
        }

        public void Init()
        {
            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);
        }


        public void InitEvent(Dictionary<string, Dictionary<string, string>> Events)
        {
            //----Daily----
            DayFirstType.text = "Daily Event";
            DayFirstProgress.text = Events["Daily0"]["status"];
            DayFirstDescription.text = Events["Daily0"]["description"];
            int gold = int.Parse(Events["Daily0"]["gold"]);
            int jewel = int.Parse(Events["Daily0"]["jewel"]);
            if (gold > 0)
            {
                DayColectGold.gameObject.SetActive(true);
                DayColectGold.text = gold.ToString();
            }else { DayColectGold.gameObject.SetActive(false); }
            if (jewel > 0)
            {
                DayColectJewel.gameObject.SetActive(true);
                DayColectJewel.text = jewel.ToString();
            }
            else { DayColectJewel.gameObject.SetActive(false); }
            
            

            //----Week-----
            WeekType.text = "Week Event";
            WeekProgress.text = Events["Week"]["status"];
            WeekDescription.text = Events["Week"]["description"];
            gold = int.Parse(Events["Week"]["gold"]);
            jewel = int.Parse(Events["Week"]["jewel"]);
            if (gold > 0)
            {
                WeekColectGold.gameObject.SetActive(true);
                WeekColectGold.text = gold.ToString();
            }
            else { WeekColectGold.gameObject.SetActive(false); }
            if (jewel > 0)
            {
                WeekColectJewel.gameObject.SetActive(true);
                WeekColectJewel.text = jewel.ToString();
            }
            else { WeekColectJewel.gameObject.SetActive(false); }
            ;
        }


   










}
}