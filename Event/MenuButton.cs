using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton
{
    public delegate void MenuEvent();

    public event MenuEvent TryAgain;
    public event MenuEvent Restart;
    public event MenuEvent Resume;
    public event MenuEvent BuyTries;


    public void Invoke_TryAgain()
    {
        TryAgain();
    }
    public void Invoke_Restart()
    {
        Restart();
    }

    public void Invoke_BuyTries()
    {
        BuyTries();
    }


    public void Invoke_Resume()
    {
        Resume();
    }
}
