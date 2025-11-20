namespace HoangHH.AdsIAP
{
    public enum AdsType
    {
        None = -1,
        Banner = 0,
        Collaps = 1,
        Inters = 2,
        Reward = 3,
        AOA = 4,
    }
    
    public enum AdsNetwork
    {
        None = 0,
        Admob = 1,
        Max = 2,
    }
    
    // MODIFY base on your project
    
    public enum AOAPosition
    {
        None = 0,
        AppOpen = 1,
        AppSwitch = 2,
        FirsOP = 3
    }

    public enum AdsPlacement
    {
        None = -1,
        Home,
        InGame,
        BuyBooster,
        BuyBoosterHint,
        BuyBoosterMood,
        BuyBoosterTime,
        Shop,
        DailyReward,
        SaveMeTime,
        SaveMeMood,
        Lose,
        Win,
        Retry,
        MoreLives,
    }

    /// <summary>
    /// Also include mrec
    /// </summary>
    public enum BannerPosition
    {
        None = -1,
        Home = 0,
        InGame = 1,
    }

    public enum MrecPosition
    {
        None = -1,
        HomeLvDetail = 0,
        HomeSetting = 1,
        MoreLive = 2,
        DailyReward = 3,
        PauseGame = 4,
        SaveMe = 5,
        EndGame = 6,
        ConfirmSkip = 7,
        Hint = 8,
        InGameLvDetail = 9,
        ConfirmQuit = 10,
        ConfirmFocusTime = 11,
        ResPizzaConfirmRefillDough = 12,
        ResPizzaConfirmAutoTopping = 13,
        ResPizzaConfirmBoostBaker = 14,
        ResPizzaConfirmIncTip = 15,
        ResPizzaConfirmMoreCustomer = 16,
        ResDailyReward = 17,
    }
}