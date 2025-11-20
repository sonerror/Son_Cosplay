
using Costopia;


public enum LevelStateCostopia
{
  /// <summary>
  ///     Players can't see it
  /// </summary>
  Hidden = 0,

  /// <summary>
  ///     Can see but can't play
  /// </summary>
  Locked = 1,

  /// <summary>
  ///     Already unlocked and can play
  /// </summary>
  Unlocked = 2,

  /// <summary>
  ///     Played once
  /// </summary>
  Played = 3,

  /// <summary>
  ///     Win
  /// </summary>
  Winned = 4
}

public enum LevelLoseTypeCostopia
{
  None = -1,
  OutOfTime = 0,
  OutOfMood = 1,
  Other = 2,
}

public static class CostopiaConstant
{

  #region Gameplay

  public const string KEY_FPS = "FPS";

  #endregion

  #region Scenes Name

  public static string LoadingSceneName => "LoadingCostopia";
  public static string InitSceneName => "InitCostopia";
  public static string HomeSceneName => "HomeCostopia";
  public static string ErrorSceneName => "ErrorCostopia";
  public static string MainLevelOverlaySceneName => "MainLevelOverlayCostopia";

  #endregion

  #region Id-Name

  public static string LevelIndexToLevelId(int index)
  {
    return index.ToString();
  }

  public static int LevelIdToLevelIndex(string levelId)
  {
    return int.Parse(levelId);
  }

  #endregion

  #region URLs

  public const string URL_TIKTOK = "https://www.tiktok.com/@cookingdom66";
  public const string URL_FACEBOOK_PAGE = "https://www.facebook.com/cookingdomgame";
  public const string URL_FACEBOOK_GROUP = "https://www.facebook.com/groups/5145668495491155";
  public const string URL_YOUTUBE = "";
  public const string URL_FEEDBACK_MAIL = "";

  #endregion

  #region Localize

  private const string LocalizeTableName = "LocalizeUI";
  private const string LocalizeDialogueTableName = "LocalizeDialogue";
  private const string NOT_AVAILABLE = "not_available";


  public enum Language
  {
    Arabic = 0,
    Chinese = 1,
    English = 2,
    French = 3,
    German = 4,
    Indonesian = 5,
    Italian = 6,
    Japanese = 7,
    Korean = 8,
    Portuguese = 9,
    Russian = 10,
    Spanish = 11,
    Thai = 12,
    Vietnamese = 13
  }






  public const string KEY_LOCALIZE_NEED_INTERNET_TO_PROCESS = "NetRqToProcess";
  public const string KEY_LOCALIZE_NEED_INTERNET_TO_LOAD_LV = "NetRqToLoadLv";

  #endregion

  #region Tracking Event

  public const string KEY_TRACKING_EVENT_SAVE_ME_TIME = "save_me_time";
  public const string KEY_TRACKING_EVENT_SAVE_ME_MOOD = "save_me_mood";
  private const string KEY_TRACKING_EVENT_USE_MORE_TIME_BOOSTER = "more_time_booster";
  private const string KEY_TRACKING_EVENT_USE_UP_MOOD_BOOSTER = "up_mood_booster";
  private const string KEY_TRACKING_EVENT_USE_HINT_BOOSTER = "hint_booster";

  public static string GetKeyTrackingBooster(InGameBooster type)
  {
    switch (type)
    {

      case InGameBooster.HintItem:
        return KEY_TRACKING_EVENT_USE_HINT_BOOSTER;
      case InGameBooster.MoreTime:
        return KEY_TRACKING_EVENT_USE_MORE_TIME_BOOSTER;
      case InGameBooster.UpMood:
        return KEY_TRACKING_EVENT_USE_UP_MOOD_BOOSTER;
      case InGameBooster.None:
      case InGameBooster.SaveMeTime:
      case InGameBooster.SaveMeMood:
      default:
        return null;
    }
  }

  #endregion
}