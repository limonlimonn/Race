using UnityEngine;

using HCR.Enums;
using HCR.GlobalWindow;
using HCR.GlobalWindow.MainMenu;
using HCR.Interfaces;
using HCR.Network;

namespace HCR
{
    /// <summary>
    /// * your summary text *
    /// </summary>

    public class AuthState : IState
    {
        Core _core;
        NetworkManager _nm;
        PlayerManager _pm;
        UIManager _uim;
        AuthentificationWindow authificationWindow;
        MultiplayerWindow multyWindow;
        ConnectState _connectState;
        EnternetWindow _enternetWindow;

        public void Enable()
        {
            //Get UI, cache ref
            if (_core == null)
            {
                _core = Core.Instance;
                _nm = _core.GetService<NetworkManager>();

               
                _pm = _core.GetService<PlayerManager>();
                _uim = _core.GetService<UIManager>();
                _enternetWindow = _uim.GetWindow(UIWindowEnum.IS_ENTERNET) as EnternetWindow;
                authificationWindow = (AuthentificationWindow)_uim.GetWindow(UIWindowEnum.AUTHENTIFICATION);

            }


            //_pm.OnUserLoaded += OnUserLoaded;

            authificationWindow.OnLogin += OnLoginSend;
            authificationWindow.OnRegistered += OnRegisterSend;

            var savedPlayerName = PlayerPrefs.GetString(PPKeys.name);
            var savedPlayerPass = PlayerPrefs.GetString(PPKeys.pass);
            
            if (string.IsNullOrEmpty(savedPlayerName) || string.IsNullOrEmpty(savedPlayerPass))
            {
                PlayerPrefs.SetString(PPKeys.name, "");         // ???????
                PlayerPrefs.SetString(PPKeys.pass, "");
                authificationWindow.Show();
                Debug.Log("Enable Auth");
            }else
            {
                _uim.ShowWindow(UIWindowEnum.SPLASH, true);
                _nm.Authentication(savedPlayerName, savedPlayerPass, 
                    (obj, error) =>
                        {
                            //Core.Instance.GetService<SafePlayerPrefs>().DeleteDataForLogOut(); 

                            if (error != "timeout")
                            {
                                PlayerPrefs.SetString(PPKeys.name, "");// ???????
                                PlayerPrefs.SetString(PPKeys.pass, "");// ???????
                                Core.Instance.GetService<StatesManager>().SwitchState(StatesEnum.Auth);
                            }
                            else
                            {
                                _enternetWindow.ShowErrorEnternet(()=> { Core.Instance.GetService<StatesManager>().SwitchState(StatesEnum.Auth); }, "Try Again");
                                authificationWindow.Show();
                            }

                            Debug.Log("Enable Auth");
                            
                            _uim.ShowWindow(UIWindowEnum.SPLASH, false);
                        },
                    (obj) => 
                        {
                            GoToNextState();
                        }

                );
            }
           

            

        }

      
       
       




        void OnLoginSend(string name, string pass)
        {
            authificationWindow.OnLogin -= OnLoginSend;
            
                
                    Debug.Log("OnLoginSend");
                    _uim.ShowWindow(UIWindowEnum.SPLASH, true);
                    _nm.Authentication(name, pass, (err, BaseError) =>
                    {
                        _uim.ShowWindow(UIWindowEnum.SPLASH, false);
                        authificationWindow.OnLogin += OnLoginSend;
                        //Show error msg1

                        if (BaseError != "timeout")
                            authificationWindow.LoginErrorMessage(err);
                        else
                            _enternetWindow.ShowErrorEnternet();
                        

                    },
                    (obj) =>
                    {
                        Debug.Log("OnAuthSuccess");
                        authificationWindow.OnLogin -= OnLoginSend;
                        GoToNextState();
                        //_uim.ShowWindow(UIWindowEnum.SPLASH, false);
                    }


                    );
                
        }

        void OnRegisterSend(string name, string pass,string date , string male)
        {
            authificationWindow.OnRegistered -= OnRegisterSend;
             
                    _nm.Registration(name, name, date, pass, male, (err, BaseError) =>
              {
                  authificationWindow.OnRegistered += OnRegisterSend;
                  _uim.ShowWindow(UIWindowEnum.SPLASH, false);
                  //Show error msg
                  if(BaseError != "timeout")
                  authificationWindow.RegistrationErrorMessage(err);
                  else
                      _enternetWindow.ShowErrorEnternet();
              },
                (success) =>
                {

                    authificationWindow.OnRegistered -= OnRegisterSend;
                    GoToNextState();
                    //_uim.ShowWindow(UIWindowEnum.SPLASH, false);
                }

                );
                
                
            
           
        }

        void GoToNextState()
        {
            

            if (_pm.isTutorial == 1)
            {

                Core.Instance.GetService<StatesManager>().SwitchState(StatesEnum.Tutorial);
                _uim.ShowWindow(UIWindowEnum.SPLASH, false);
            }
            else
            {

                Core.Instance.GetService<StatesManager>().SwitchState(StatesEnum.MAIN_MULTIPLAYER_WINDOW);
                //_uim.ShowWindow(UIWindowEnum.SPLASH, false);
            }
            //_uim.ShowWindow(UIWindowEnum.CHOOSECAR, true);
            authificationWindow.Hide();
        }

        void OnUserLoaded()
        {
            //_uim.ShowWindow(UIWindowEnum.SPLASH, false);
           

            _pm.OnUserLoaded -= OnUserLoaded;


            GoToNextState();
        }

        public void Disable()
        {
            
            _pm.OnUserLoaded -= OnUserLoaded;
            authificationWindow.OnLogin -= OnLoginSend;
            authificationWindow.OnRegistered -= OnRegisterSend;
        }



    }
}