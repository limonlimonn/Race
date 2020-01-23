using HCR.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Assertions;
using UnityEngine.UI;
using DG.Tweening;

namespace HCR.GlobalWindow.MainMenu
{
    public class StatisticPanel : MonoBehaviour, IUIWindow
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        private UIManager _uiManager;
        public Image _imageExpProgrssBar;
        private MainScreenStateManager _mainScreenStateManager;
        private PlayerManager _playerManager;
        public Text nameUser;
        public Text LevelUser;
        public Text TotalExp;
        public Text PreTotalExp;
        public Text UperTotalExp;

        public Image ImageAverageStars1;
        public Image ImageAverageStars2;
        public Image ImageAverageStars3;
        public Text StarsCount;

        public Text TextMaxGold;
        public Text TextTotalGold;
        public Text TextMaxJewels;
        public Text TextTotalJewels;
        public Text TextMaxXP;
        public Text TextAverageXP;
        public Text TextMaxBalance;
        public Text TextTotalBalance;
        public Text TextMaxFlip;
        public Text TextTotalFlip;
        public Text TextMaxInAir;
        public Text TextTotalInAir;
        public Text TextMaxBlunt;
        public Text TextTotalBlunt;
        public Text TextWin;
        public Text TextDraw;
        public Text TextLosing;
        public Text TextInGame;
        public GameObject TextAccount;
        public Text TextTimeAccount;
        private Coroutine AccountTime;
        private int StampAccountTime = 0;

        private string AverageStars;
        [SerializeField]
        private Banner banner;



        public void Hide()
        {
            banner.HideBaner();
            //Debug.Log("Hide");
            //_canvasGroup.alpha = 0;
            _uiManager.HideCanvas(_canvasGroup);

            _canvasGroup.blocksRaycasts = false;
            _imageExpProgrssBar.DOFillAmount(0, 0f);

            ImageAverageStars1.DOKill();
            ImageAverageStars2.DOKill();
            ImageAverageStars3.DOKill();
            ImageAverageStars1.DOFillAmount(0,0);
            ImageAverageStars2.DOFillAmount(0,0);
            ImageAverageStars3.DOFillAmount(0,0);
            StarsCount.text = "0.0";

        }

        public void Init()
        {
            _uiManager = Core.Instance.GetService<UIManager>();
            Assert.AreNotEqual(null, _uiManager);

            _mainScreenStateManager = _uiManager.Get_MainScreenStateManager();
            Assert.AreNotEqual(null, _mainScreenStateManager);

            _playerManager = Core.Instance.GetService<PlayerManager>();
            Assert.AreNotEqual(null, _playerManager);
            

            if (_playerManager.AverageStars % 1 == 0)
                AverageStars = _playerManager.AverageStars.ToString() + ".0";
            else AverageStars = Math.Round(_playerManager.AverageStars,1 ).ToString();
            if (_playerManager.accountType == 1)
            {
                StampAccountTime = (_playerManager.accountTime - _playerManager.now_stamp) / 60;
                Debug.Log("nowStamp " + _playerManager.now_stamp + " accountTime " + _playerManager.accountTime + " StampAccountTime " + StampAccountTime);
                TextAccount.SetActive(true);
                AccountTime = Core.Instance.StartCor(ShowAccountTime());

            }
            else
                TextAccount.SetActive(false);
                
            
        }        
       

        public void Show()
        {
            banner.ShowBaner();
            Init();
            //_canvasGroup.alpha = 1;
            _uiManager.ShowCanvas(_canvasGroup);
            _canvasGroup.blocksRaycasts = true;
            
            ShowLevelProgress();
            ShowDetailsUser();
            ShowStars();
            
        }

        private void ShowStars()
        {
            Coroutine StarCountCour = null;

            StarCountCour = Core.Instance.StartCor(ShowStarsCount());
                ImageAverageStars1.DOFillAmount(_playerManager.AverageStars, 1f).OnComplete(() =>
            ImageAverageStars2.DOFillAmount(_playerManager.AverageStars - 1, 1f).OnComplete(() =>
            ImageAverageStars3.DOFillAmount(_playerManager.AverageStars - 2, 1f)));
            
            
        }
        private IEnumerator ShowStarsCount() {
            decimal starCount = 0;
            
            while (AverageStars != StarsCount.text)
            {
                yield return new WaitForSecondsRealtime(0.09f);
                starCount += (decimal)0.1;
                StarsCount.text = starCount.ToString();
                
            }
            Core.Instance.StopCoroutine("StarCountCour");
          }

        private IEnumerator ShowAccountTime()
        {
            TextTimeAccount.text = Timer();
            while (StampAccountTime > 0)
            {
                yield return new WaitForSecondsRealtime(59);
                StampAccountTime--;
                TextTimeAccount.text = Timer();
            }
        }

        public string Timer()
        {
            string time = "";
            int day = StampAccountTime / 60 / 24;
            int hours = StampAccountTime / 60 - 24 * day;
            int min = StampAccountTime - hours * 60 - 24 * day * 60;

            time = "Day:" + (day > 9 ? day.ToString() : "0" + day.ToString()) + " hours:" + (hours > 9 ? hours.ToString() : "0" + hours.ToString());
            return time;
        }

        private void ShowDetailsUser()
        {
            nameUser.text = _playerManager.PlayerDisplayName;
            LevelUser.text = "level " + _playerManager.level.ToString();
            TextMaxGold.text =  _playerManager.MaxGold.ToString();
            TextTotalGold.text =  _playerManager.TotalGold.ToString();
            TextMaxJewels.text =  _playerManager.MaxJewels.ToString();
            TextTotalJewels.text =  _playerManager.TotalJewels.ToString();
            TextMaxXP.text =  _playerManager.MaxXP.ToString();
            TextAverageXP.text =  _playerManager.AverageXP.ToString();
            TextMaxBalance.text = "Max Balance : " + _playerManager.MaxBalance.ToString();
            TextTotalBalance.text = "Total Balance : " + _playerManager.TotalBalance.ToString();
            TextMaxFlip.text = "Max Flip : " + _playerManager.MaxFlip.ToString();
            TextTotalFlip.text = "Total Flip : " + _playerManager.TotalFlip.ToString();
            TextMaxInAir.text = "Max InAir : " + _playerManager.MaxInAir.ToString();
            TextTotalInAir.text = "Total InAir : " + _playerManager.TotalInAir.ToString();
            TextMaxBlunt.text = "Max Blunt : " + _playerManager.MaxBlunt.ToString();
            TextTotalBlunt.text = "Total Blunt : " + _playerManager.TotalBlunt.ToString();
            TextWin.text =  _playerManager.Win.ToString();
            TextDraw.text = _playerManager.Draw.ToString();
            TextLosing.text =  _playerManager.Losing.ToString();
            TextInGame.text =  _playerManager.InGame.ToString();

        }

        private void ShowLevelProgress()
        {
            int xpTotal = _playerManager.xp;
            TotalExp.text = xpTotal.ToString();
            int xpNeeded = GetXpNeeded(_playerManager.level);

            float Min = 0;
            if (_playerManager.level > 1)
                Min = GetXpNeeded(_playerManager.level - 1);

            PreTotalExp.text = Min.ToString();
            UperTotalExp.text = xpNeeded.ToString();


            float expBarValue = (xpNeeded - Min + (xpTotal - xpNeeded)) / Math.Abs(xpNeeded - Min);

            _imageExpProgrssBar.DOFillAmount(expBarValue, 0.5f);


        }


        private int GetXpNeeded(int lvl)
        {
            for (int i = 0; i < _playerManager.allLevelsXP.Count; i++)
            {
                if (lvl == _playerManager.allLevelsXP[i].ID)
                {
                    return _playerManager.allLevelsXP[i].maxValue;
                }
            }
            return 0;
        }
    }
}
