namespace HCR.Enums
{
	/// <summary>
	/// Перечисление - состояния переходов "Главного Меню"
	/// </summary>

	public enum MainScreenStatesEnum
	{
		NONE,

		MAIN_MULTIPLAYER,
		CHOOSE_CAR,
        CHOOSE_TRACK,
		TREE,
		SETTINGS,
        STATISTIC,
        SCORE,
        EVENT,

		GAME_ASYNC,
		GAME_SINGLE,
        GAME_TUTORIAL,
	}

}