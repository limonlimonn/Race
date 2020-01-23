
using HCR.Enums;
using UnityEngine;
using UnityEngine.Assertions;
using _AsyncMulty = HCR.Gameplay.AsyncMultiplayer;
using _Single = HCR.Gameplay.Singleplayer;

namespace HCR.Gameplay
{
	/// <summary>
	/// Класс - для получения экземпляра окна "GameWindow"
	/// (в случае, если для этого нужна проверка на тип игры)
	/// </summary>
	public class GameWindowResolver
	{
		// FIELDS

		// dependences
		private readonly GameManager _gameManager;
		private readonly UIManager _uiManager;



		// CONSTRUCTOR

		public GameWindowResolver()
		{
			// pre-condition
			Assert.AreNotEqual(null, Core.Instance);

			_gameManager = Core.Instance.GetService<GameManager>();
			Assert.AreNotEqual(null, _gameManager);

			_uiManager = Core.Instance.GetService<UIManager>();
			Assert.AreNotEqual(null, _uiManager);
		}



		// INTERFACES

		/// <summary>
		/// Возвращает конкретный экземпляр класса "GameWindow", проверяя по типу игры
		/// </summary>
		public ABaseGameWindow GetGameWindow()
		{
			if (IsGameAsyncMultiplayer())
			{
				var gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_ASYNC) as _AsyncMulty.GameWindowAsyncMultiplayer;
				Assert.AreNotEqual(null, gameWindow);

				return gameWindow;
			}
			else if (IsGameSinglePlayer())
			{
				var gameWindow = _uiManager.GetWindow(UIWindowEnum.GAMEWINDOW_SINGLE) as _Single.GameWindowSingleplayer;
				Assert.AreNotEqual(null, gameWindow);

				return gameWindow;
			}

			#region DEBUG
#if UNITY_EDITOR
				Debug.Log("[ERROR] unknown game type = " + _gameManager.gameData.GameType + " !");
#endif
				#endregion
			return null;
		}



		// METHODS

		private bool IsGameAsyncMultiplayer()
		{
			return
				(_gameManager.gameData.GameType == GameTypeEnum.MultyNew ||
				_gameManager.gameData.GameType == GameTypeEnum.MultyRecord ||
				_gameManager.gameData.GameType == GameTypeEnum.MultyReplay ||
				_gameManager.gameData.GameType == GameTypeEnum.MultyJoin);
		}

		private bool IsGameSinglePlayer()
		{
			return
				(_gameManager.gameData.GameType == GameTypeEnum.SingleNew);
		}



	}
}