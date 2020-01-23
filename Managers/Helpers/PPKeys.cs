
namespace HCR
{
    /// <summary>
    /// Класс - для хранения ключей, используемых в PlayerPrefs
    /// чтоб не размазывать строки по всей программе
    /// </summary>

    public static class PPKeys
    {
        // FIELDS

        // ----------------------------------------------------
        // Authentification
        public static readonly string name = "name";
        public static readonly string pass = "pass";

        // ----------------------------------------------------
        // Silver, Gold, Exp
        public static readonly string playerJewels = "PlrJewels";
        public static readonly string playerGold = "PlrGold";
        public static readonly string playerExp = "PlrExp";

        public static readonly string earnedJewels = "EarnedJewels";
        public static readonly string earnedGold = "EarnedGold";
        public static readonly string earnedExp = "EarnedExp";

        // ----------------------------------------------------
        // Tricks
        public static readonly string trickHorseTime = "HorseTime";
        public static readonly string trickAirTime = "AirTime";
        public static readonly string trickFlipSum = "FlipSum";
        public static readonly string trick90Time = "Flip90";

        // ----------------------------------------------------
        // SETTINGS
        public static readonly string uiType = "UIType";
        public static readonly string qualityShadows = "QualityShadows";
        public static readonly string vibration = "Vibration";
        public static readonly string music = "Music";
        public static readonly string volumeMusic = "VolumeMusic";
        public static readonly string volumeSounds = "VolumeSounds";
        // ----------------------------------------------------
        // SETTINGS
        public static readonly string Tutorial_step = "tutorial_step";
        // ----------------------------------------------------
        //GoldWithMultiplier
        public static readonly string goldEarned = "GOLD_EARNED";
        public static readonly string multiplier = "multiplier";
        // ----------------------------------------------------
        // GAME_DATA
        public static readonly string JSON = "GamesSafe";
        public static readonly string GamesId = "GamesId";
        public static readonly string EmptyData = "EmptyData";
        public static readonly string ReplayJSON = "ReplayJSON";



    }
}