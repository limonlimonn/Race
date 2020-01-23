using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

using HCR.Interfaces;
using FMODUnity;

namespace HCR.Gameplay
{
    public abstract class ABaseRecordHandler
    {
        // PROPERTIES

        // не та переменная, что в GameWindow (triesCount) !!!
        // вычисляется >>> (попытки - 3) и результат берется по модулю
        

        private string audioCounterPath = "event:/UI Sounds/film_countdown";
        private float previousSecondCounter;
       
        // FIELDS
        protected List<Vector4> _rawData = new List<Vector4>();
        //protected List<RecordParameter> _rawData = new List<RecordParameter>();
        //protected List<InputParameter> InputData = new List<InputParameter>();

        protected CarBase _playerCar;

        protected Coroutine _coroutineStartTimer;
        protected Coroutine _coroutineMovePlayerOnFinish;
        protected Coroutine _coroutineStartTimeInfo;


        // dependences
        protected CarConstructor _carConstructor;
        protected GameManager _gameManager;
        protected PlayerManager _playerManager;
        protected RecordObjectController _recordObjectController;
        protected UIManager _uiManager;
        protected AudioService _audioService;
        protected SafePlayerPrefs _safePlayerPrefs;
        protected CarBase _ghostCar;
        protected List<Vector4> _replayData = new List<Vector4>();
        protected ReplyObjectController _replyObjectController;


        // must be init from another class
        protected GameData _gameData;
        List<Rigidbody> _keysInDict = new List<Rigidbody>();
        List<Vector3> _objPositoinList = new List<Vector3>();
        List<Quaternion> _objRotationList = new List<Quaternion>();

        protected int replayFrame = 0;


        // METHODS

        #region HIDE_COINS

        protected void HideSilverCoins()
        {
            GameObject trackJewel = GameObject.Find("TrackJewel");

            if (trackJewel != null)
            {
                trackJewel.SetActive(false);
            }
            else
            {
                #region DEBUG
#if UNITY_EDITOR
                Debug.Log("[ERROR] can't find prefab - TrackJewel !");
#endif
                #endregion
            }
        }

        protected void HideGoldCoins()
        {
            GameObject trackGold = GameObject.Find("TrackGold");

            if (trackGold != null)
            {
                trackGold.SetActive(false);
            }
            else
            {
                #region DEBUG
#if UNITY_EDITOR
                Debug.Log("[ERROR] can't find prefab - TrackGold !");
#endif
                #endregion
            }
        }

        #endregion

        #region COROUTINES

       

        protected IEnumerator StartTimer(IGameWindow gameWindow, Action OnComplete = null)
        {
            
            _audioService.StartGameMusic();
            float seconds = 4;
            

            previousSecondCounter = seconds;

            Rigidbody _prb = null;
            if (_playerCar != null)
            {
                _prb = _playerCar.GetComponent<Rigidbody>();
            }
            
            Rigidbody _erb = null;
            if (_ghostCar != null)
            {
                _erb = _ghostCar.GetComponent<Rigidbody>();
            }
            
            
            gameWindow.Get_TimerText().GetComponent<CanvasGroup>().alpha = 1;

            while (seconds > 1)
            {

                gameWindow.Get_TimerText().text = (seconds - (seconds % 1)).ToString();
            
                if(previousSecondCounter != (seconds - (seconds % 1)) && seconds != 4)
                {
                    previousSecondCounter = (seconds - (seconds % 1));
                     _audioService.RM_PlayOneShot(audioCounterPath);
                }
                
                yield return new WaitForEndOfFrame();
                seconds -= Time.deltaTime;

                if (_prb != null)
                {
                    _prb.velocity = new Vector3(0, _prb.velocity.y, _prb.velocity.z);
                }

                if (_erb != null)
               {
                     _erb.velocity = new Vector3(0, _erb.velocity.y, _erb.velocity.z);
               }

                _playerCar.transform.position = new Vector3(_playerCar.transform.position.x, _playerCar.transform.position.y, 2);
            }
            
            gameWindow.Get_TimerText().GetComponent<CanvasGroup>().alpha = 0;

            _playerCar.EnablePlayerControll(true);
            if (_gameData.ghostData.Count > 0)
            {
                _ghostCar.EnableEnemyControll(true);
            }

            

            gameWindow.Get_PauseButton().interactable = true;

            

            // invoke callback
            if (OnComplete != null)
            {
                OnComplete();
            }
        }

        protected IEnumerator MovePlayerOnFinish()
        {
            Camera.main.GetComponent<WorkShopCamTest>().enabled = false;

            _playerCar.Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ
                                               | RigidbodyConstraints.FreezeRotationY
                                               | RigidbodyConstraints.FreezeRotationZ;

            _playerCar.controller.enabled = true;
            _playerCar.controller.throttleInput = 0f;
            _playerCar.controller.brakeInput = 1f;
            _playerCar.Rigidbody.drag = 0.5f;

            YieldInstruction _wait = new WaitForSeconds(2f);
            yield return _wait;

            if (_playerCar != null)
            {
                _playerCar.Rigidbody.drag = 1.5f;
            }

            SafeStopCoroutine_Record();
        }

        #endregion

        protected void SaveRecordData()
        {
            

            byte[] bytesToSend;
            using (MemoryStream memStream = new MemoryStream(_recordObjectController.offset + _rawData.Count * 16))
            {
                if (_recordObjectController.IsFind)
                {
                    List<int> _totalFrameOfObject = _recordObjectController.GetTotalFrameForObjects();

                    foreach (int key in _recordObjectController.ObjectsRecordInfo.Keys)
                    {
                        memStream.Write(BitConverter.GetBytes(_totalFrameOfObject[key]), 0, 4);
                    }

                    foreach (int key in _recordObjectController.ObjectsRecordInfo.Keys)
                    {

                        foreach (var val in _recordObjectController.ObjectsRecordInfo[key].Keys)
                        {
                            memStream.Write(BitConverter.GetBytes(val), 0, 4);
                            memStream.Write(BitConverter.GetBytes(_recordObjectController.ObjectsRecordInfo[key][val]
                                .RecordedPosition.x), 0, 4);
                            memStream.Write(BitConverter.GetBytes(_recordObjectController.ObjectsRecordInfo[key][val]
                                .RecordedPosition.y), 0, 4);
                            memStream.Write(BitConverter.GetBytes(_recordObjectController.ObjectsRecordInfo[key][val]
                                .RecordedRotation.x), 0, 4);
                            memStream.Write(BitConverter.GetBytes(_recordObjectController.ObjectsRecordInfo[key][val]
                                .RecordedRotation.y), 0, 4);
                            memStream.Write(BitConverter.GetBytes(_recordObjectController.ObjectsRecordInfo[key][val]
                                .RecordedRotation.z), 0, 4);
                            memStream.Write(BitConverter.GetBytes(_recordObjectController.ObjectsRecordInfo[key][val]
                                .RecordedRotation.w), 0, 4);
                        }
                    }

                    memStream.Write(BitConverter.GetBytes(_recordObjectController.offset), 0, 4);
                }
                foreach (var val in _rawData)
                {
                    memStream.Write(BitConverter.GetBytes(val.x), 0, 4);
                    memStream.Write(BitConverter.GetBytes(val.y), 0, 4);
                    memStream.Write(BitConverter.GetBytes(val.z), 0, 4);
                    memStream.Write(BitConverter.GetBytes(val.w), 0, 4);
                }

                bytesToSend = memStream.GetBuffer();
            }

            if (bytesToSend != null)
            {
                _gameData.recordData = Convert.ToBase64String(bytesToSend);
            }

            _gameManager.Games.Remove(_gameManager.GetGameDataByID(_gameData.gameId));

            _gameManager.Games.Add(_gameData);


            GameData data = new GameData();
            data = _gameManager.GetGameDataByID(_gameData.gameId);

        }

        protected virtual void OnPlayerFinishClick()
        {
            SaveRecordData();
            _gameManager.Finish();
        }

        //      protected virtual void OnPlayerFinishClickForVelocity()
        //{
        //	byte[] bytesToSend;
        //          using (MemoryStream memStream = new MemoryStream(_rawData.Count * 36))
        //          {
        //              foreach (var val in _rawData)
        //              {
        //                  memStream.Write(BitConverter.GetBytes(val.MoveParam.x), 0, 4);
        //                  memStream.Write(BitConverter.GetBytes(val.MoveParam.y), 0, 4);
        //                  memStream.Write(BitConverter.GetBytes(val.MoveParam.z), 0, 4);
        //                  memStream.Write(BitConverter.GetBytes(val.VelocityParam.x), 0, 4);
        //                  memStream.Write(BitConverter.GetBytes(val.VelocityParam.y), 0, 4);
        //                  memStream.Write(BitConverter.GetBytes(val.VelocityParam.z), 0, 4);
        //                  memStream.Write(BitConverter.GetBytes(val.AngularVelocityParam.x), 0, 4);
        //                  memStream.Write(BitConverter.GetBytes(val.AngularVelocityParam.y), 0, 4);
        //                  memStream.Write(BitConverter.GetBytes(val.AngularVelocityParam.z), 0, 4);
        //              }

        //          bytesToSend = memStream.GetBuffer();
        //	}

        //	if (bytesToSend != null)
        //	{
        //		_gameData.recordData = Convert.ToBase64String(bytesToSend);
        //	}

        //	_gameManager.Finish();
        //}

        //protected virtual void OnPlayerFinishClickForInput()
        //{
        //    byte[] bytesToSend;

        //    using (MemoryStream memStream = new MemoryStream(InputData.Count * 36))
        //    {
        //        foreach (var val in InputData)
        //        {
        //            memStream.Write(BitConverter.GetBytes(val.steerInput), 0, 4);
        //            memStream.Write(BitConverter.GetBytes(val.throttleInput), 0, 4);
        //            memStream.Write(BitConverter.GetBytes(val.brakeInput), 0, 4);
        //            memStream.Write(BitConverter.GetBytes(val.handbrakeInput), 0, 4);
        //            memStream.Write(BitConverter.GetBytes(val.NitroPower), 0, 4);
        //            memStream.Write(BitConverter.GetBytes(val.RotateAndroid), 0, 4);
        //            memStream.Write(BitConverter.GetBytes(val.Sin), 0, 8);
        //            memStream.Write(BitConverter.GetBytes(val.Cos), 0, 8);
        //        }


        //        bytesToSend = memStream.GetBuffer();
        //    }

        //    if (bytesToSend != null)
        //    {
        //        _gameData.recordData = Convert.ToBase64String(bytesToSend);
        //    }

        //    _gameManager.Finish();
        //}

       

        protected void SetShadowCarSettings()
        {
            _ghostCar = _carConstructor.CreateEnemyCar(_playerManager.selectedCar, _playerManager.selectedCar.current_color);
            Assert.AreNotEqual(null, _ghostCar);

            //_shadowCar.transform.tag = "Player";
            _ghostCar.SetColorInGame(_playerManager.selectedCar.current_color);
            _ghostCar.Transform.position = new Vector3(0f, 3f, -2f);

            _ghostCar.controller.isEnemy = false;
            _ghostCar.controller.enabled = false;
        }

        protected void SetPlayerCarSettings(IGameWindow gameWindow)
        {
            _playerCar = _carConstructor.CreatePlayerCar(_playerManager.selectedCar, _playerManager.selectedCar.current_color);
            Assert.AreNotEqual(null, _playerCar);

            _playerCar.SetColorInGame(_playerManager.selectedCar.current_color);
            _playerCar.transform.tag = "Player";
            _playerCar.Transform.position = new Vector3(0f, 3f, 2f);
            _playerManager.PlayerCar = _playerCar;

            //_playerCar.SetValue_RaceTimer(gameWindow.Get_TextRaceTimer());
        }

        #region CHECK_WHAT_WINDOW_SHOW_ON_CRASH

        protected void CheckWhatWindowShowOnCrash()
        {
            if (_gameData.GetTryes > 0)
            {
                ShowWindow_Retry();
            }
            else
            {
                ShowWindow_CrashFinish();
            }
        }

        // abstract
        protected abstract void ShowWindow_Retry();

        // abstract
        protected abstract void ShowWindow_CrashFinish();

        #endregion

        #region CHECK_WHAT_DOING_ON_RESTART

        protected void CheckWhatDoingOnRestart()
        {
            _playerCar.EnablePlayerControll(false);
           
            Debug.LogError("tries = " + _gameData.GetTryes);

            if (_gameData.GetTryes > 0)
            {
                RestartWork_HaveTries();
            }
            else
            {
                RestartWork_HaveNoTriesLeft();
            }
        }

        // abstract
        protected abstract void RestartWork_HaveTries();

        // abstract
        protected abstract void RestartWork_HaveNoTriesLeft();

        #endregion

        #region SAFE_STOP_COROUTINES

        protected void SafeStopCoroutine_StartTimer()
        {
            if (_coroutineStartTimer != null)
            {
                Core.Instance.StopCor(_coroutineStartTimer);
            }

            _coroutineStartTimer = null;
        }

        protected void SafeStopCoroutine_MovePlayerOnFinish()
        {
            if (_coroutineMovePlayerOnFinish != null)
            {
                Core.Instance.StopCor(_coroutineMovePlayerOnFinish);
            }

            _coroutineMovePlayerOnFinish = null;
        }

        protected abstract void SafeStopCoroutine_Record();

        #endregion


        //protected void ObjectDataToList()
        //{
        //    foreach (var val in _recordObjectController.GetRecordObj().Keys)
        //    {
        //        _keysInDict.Add(val);
        //    }

        //    for (int i = 0; i < _keysInDict.Count; i++)
        //    {
        //        foreach (var val in _recordObjectController.GetRecordObj()[_keysInDict[i]].Keys)
        //        {
        //            _objPositoinList.Add(val);
        //            _objRotationList.Add(_recordObjectController.GetRecordObj()[_keysInDict[i]][val]);
        //        }


        //    }

        //}

    }
}