using HCR.Enums;
using HCR.Interfaces;
using HCR.Loading;
using UnityEngine;

namespace HCR
{
	public class ConnectState : IState
	{
		// FIELDS

		// dependences
		private Core _core;
		private NetworkManager _networkManager;
		private StatesManager _statesManager;
		private PlayerManager _playerManager;

		private UIManager _uiManager;
		private SplashScreen _splashScreen;



		// I_STATE

		public void Enable()
		{
			if (_core == null)
			{
				_core = Core.Instance;
				_networkManager = _core.GetService<NetworkManager>();
				_statesManager = _core.GetService<StatesManager>();
				_playerManager = _core.GetService<PlayerManager>();

				_uiManager = _core.GetService<UIManager>();
				_splashScreen = _uiManager.GetWindow(UIWindowEnum.SPLASH) as SplashScreen;
			}

			_core.GetService<UIManager>().ShowWindow(UIWindowEnum.SPLASH,true);
			_statesManager.SwitchState(StatesEnum.Auth);

			
		}

		public void Disable()
		{
			_core.GetService<UIManager>().ShowWindow(UIWindowEnum.SPLASH, false);
			
		}



		// METHODS

		

       



	}
}