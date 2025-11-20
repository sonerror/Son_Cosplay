using UnityEngine;

namespace Costopia
{
  public enum InGameBooster
  {
    None = 0,
    SaveMeTime,
    SaveMeMood,
    HintItem,
    MoreTime,
    UpMood
  }

  public enum BoosterPlacement
  {
    None = 0,
    Home,
    InGame,
  }

  [System.Serializable]
  public class InGameSysConfigCostopia
  {
    public enum SkipMode
    {
      NO_SKIP = 0,
      SHOW_POPUP_ADS = 1,
      INSTANT_SKIP = 2,
      SHOW_POPUP_GOLD = 3,
      SHOW_POPUP_GOLD_OR_ADS = 4,
    }

    [Header("Save Me")]
    [SerializeField] private int saveMeTime = 60;
    [SerializeField] private int saveMeMoodGain = 100;
    [SerializeField] private int saveMePrice = 600;
    [SerializeField] private int saveMeNumAds = 1;

    [Header("More Time")]
    [SerializeField] private int moreTime = 45;
    [SerializeField] private int moreTimePrice = 250;

    [Header("Up Mood")]
    [SerializeField] private int upMood = 50;
    [SerializeField] private int upMoodPrice = 300;

    [Header("Hint")]
    [SerializeField] private int hintPrice = 200;

    [Header("Win")]
    [SerializeField] private int rwFirst = 10;
    [SerializeField] private int rwWin = 20;

    [Header("Booster Unlock")]
    [SerializeField] private int hintUnlock = 1;
    [SerializeField] private int moreTimeUnlock = 3;
    [SerializeField] private int upMoodUnlock = 5;

    public int SaveMeTime => saveMeTime;
    public int SaveMeMoodGain => saveMeMoodGain;
    public int SaveMePrice => saveMePrice;
    public int SaveMeNumAds => saveMeNumAds;
    public int MoreTime => moreTime;
    public int MoreTimePrice => moreTimePrice;
    public int UpMood => upMood;
    public int UpMoodPrice => upMoodPrice;
    public int HintPrice => hintPrice;
    public int RwFirst => rwFirst;
    public int RwWin => rwWin;

    public const int MAXIMUM_MOOD = 100;
  }
}