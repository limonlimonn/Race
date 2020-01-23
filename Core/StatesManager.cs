using System.Collections.Generic;
using HCR.Enums;
using HCR.Gameplay.Singleplayer;
using HCR.Interfaces;
using HCR.Gameplay.Tutorial;
using UnityEngine;

namespace HCR
{
	/// <summary>
	/// Класс - глобальный менеджер стейтов, для переключения между окнами
	/// ("Авторизация", "Мультиплеер-начало", "Гонка")
	/// </summary>

	public class StatesManager : IService
	{
		// FIELDS

		private Dictionary<StatesEnum, IState> _states;
		private IState _currentState;
        private StatesEnum _currentStateEnum;


        // INTERFACES

        public void Init()
		{
			InitDictionaryStates();
			SwitchState(StatesEnum.Connect);
		}
        public StatesEnum GetCurrentState()
        {
            return _currentStateEnum;
        }
        public void SwitchState(StatesEnum state)
		{
            //Debug.Log(" "+ state.ToString());

            Core.Instance.UnMute();
            if (_currentState != null) {
				_currentState.Disable(); }

			_currentState = _states[state];
            _currentStateEnum = state;

            _currentState.Enable();
		}



		// METHODS

		private void InitDictionaryStates()
		{
			_states = new Dictionary<StatesEnum, IState>();

			//
			_states.Add( StatesEnum.Connect, 					new ConnectState()			);
			_states.Add( StatesEnum.Auth, 						new AuthState()				);
            _states.Add( StatesEnum.Tutorial,                   new TutorialState()         );
			_states.Add( StatesEnum.MAIN_MULTIPLAYER_WINDOW, 	new MainMultiPlayerState()	);

			//
			_states.Add( StatesEnum.GAME_ASYNC, 				new GameAsyncState()		);
			_states.Add( StatesEnum.GAME_SINGLE, 				new GameSingleState()		);
		}




	}
}