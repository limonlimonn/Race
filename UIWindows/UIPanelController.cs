
using HCR;
using HCR.Event;
using HCR.Event.Car;
using HCR.Event.Track;
using HCR.Event.UIControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelController : MonoBehaviour
{

    private UiPanelOld uiPanel;
    private ButtonControl buttonControl;
    
    private GameData gameData;
    #region EVENT
    private Game gameEvent;
    private TrackEvent trackEvent;
    private Player playerCarEvent;
    #endregion 
    #region Bool Button Pressed
    private bool isNitroPressed = false;
    private bool isFrowardPressed = false;
    private bool isBackPressed = false;
    private bool isUpPressed = false;
    private bool isDownPressed = false;
    #endregion

    private void Awake()
    {
        uiPanel = GetComponent<UiPanelOld>();
        buttonControl = EventManager._init.Game.ButtonControl;
       
        InitEvent();
    }

    private void Start()
    {
        //Debug.Log("Start " + gameData.GetTryes);
    }

    private void InitEvent()
    {
        gameEvent = EventManager._init.Game;
        trackEvent = gameEvent.TrackEvent;
        trackEvent.InitEvents += Init_TrackEvent;
        playerCarEvent = gameEvent.CarEvent.Player;
        playerCarEvent.InitEvents += Init_PlayerCarEvent;


    }

    private void Init_TrackEvent()
    {
        trackEvent.GetEvent.OnLoad += OnLoadTrack;
    }

    private void Init_PlayerCarEvent()
    {
        Debug.LogError("+= OnPlayerCrash ");
        playerCarEvent.GetEvent.Crash += OnPlayerCrash;
    }

    private void OnLoadTrack()
    {
        LoadGameData();
        if (gameData == null) Debug.LogError("GameData == null");
        UpdateTries();
        uiPanel.UnblockPauseButton();
    }

    private void LoadGameData()
    {
        gameData = GameDataManager._init.GameData;
    }

    private void OnPlayerCrash()
    {
        uiPanel.BlockPauseButton();
        UpdateTries();
    }

    private void UpdateTries()
    {
        Debug.Log("Update try " + GameDataManager._init.GameData.GetTryes);
        uiPanel.UpdateTriesView(GameDataManager._init.GameData.GetTryes);
    }

    #region Button Event

    public void OnNitroPressed(bool pressed)
    {
        Debug.Log("OnNitroPressed");
        isNitroPressed = pressed;
        if (isNitroPressed)
        {
            OnForwardPressed(true);
            uiPanel.NitroPressed();
            buttonControl.Invoke_OnNitroPressed();
        }
        else
        {
            
            OnForwardPressed(false);
            uiPanel.NitroUnPressed();
            buttonControl.Invoke_OnNitroUnPressed();
            
        }

        
    }

    public void OnForwardPressed(bool pressed)
    {
        
        if (!isBackPressed)
        {
            isFrowardPressed = pressed;
            if (isFrowardPressed)
            {
                uiPanel.Set_RotateCircle_F(true);
                // Invoke Event
                buttonControl.Invoke_OnForwardPressed();
            }

            else
            {
                uiPanel.Set_RotateCircle_F(false);
                // Invoke Event
                buttonControl.Invoke_OnForwardUnPressed();
            }
        }
    }

    public void OnBackPressed(bool pressed)
    {

        if (!isFrowardPressed)
        {
            isBackPressed = pressed;
            if (isBackPressed)
            {
                //_playerCar.throttleInput = -1;
                uiPanel.Set_RotateCircle_B(true);
                buttonControl.Invoke_OnBackPressed();
            }
            else
            {
                uiPanel.Set_RotateCircle_B(false);
                buttonControl.Invoke_OnBackUnPressed();
                //_playerCar.throttleInput = 0;
            }
        }
    }

    public void OnUpPressed(bool pressed)
    {
        if (!isDownPressed)
        {
            isUpPressed = pressed;
            if (pressed)
            {
                uiPanel.Set_RotateCircle_Up(true);
                buttonControl.Invoke_OnUpPressed();
            }
            else
            {
                uiPanel.Set_RotateCircle_Up(false);
                buttonControl.Invoke_OnUpPressed();
            }
        }
    }

    public void OnDownPressed(bool pressed)
    {
        
        if (!isUpPressed)
        {
            isDownPressed = pressed;
            if (isDownPressed)
            {
                uiPanel.Set_RotateCircle_Down(true);
                buttonControl.Invoke_OnDownPressed();
            }
            else
            {
                uiPanel.Set_RotateCircle_Down(false);
                buttonControl.Invoke_OnDownUnPressed();
            }
        }
    }
    #endregion


}