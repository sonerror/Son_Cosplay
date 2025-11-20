using System;
using HoangHH;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Costopia.Level
{
  public class LevelPlayingControlCostopia : H3Singleton<LevelPlayingControlCostopia>
  {
    public class BoosterUsedData
    {
      public int saveMeTime;
      public int saveMeMood;
      public int hint;
      public int focusTime;
      public int upMood;
    }

    public enum PlayingMode
    {
      Normal,
    }

    public enum PlayingState
    {
      None,
      Playing,
      Win,
      Lose,
    }

    private const string KEY_INT_COUNT_RETRY = "numRetry";
    private const string KEY_INT_COUNT_RETRY_AFTER_WIN = "numRetryWin";
    private const string KEY_MAX_LEVEL_INDEX_WINNED_CONTINUOUSLY = "maxLvWinCont";
    private const string KEY_MAX_LEVEL_INDEX_WINNED = "maxLvWin";

    [SerializeField] private InGameSysConfigCostopia inGameConfig;


    public InGameSysConfigCostopia InGameConfig => inGameConfig;

    private bool _isLoadingScene;
    private int _holdingHeart;
    private int _holdingCoinSpend;
    private int _holdingRetryCount;
    private int _reviveCount;
    private int _holdingBoosterSpend;
    private int _holdingCoinEarn;
    private int _holdingBoosterEarn;
    private int _holdingRewardCount;
    private float _lastTimeDoneStep;

    public int HoldingHeart => _holdingHeart;

    public int HoldingRetryCount => _holdingRetryCount;

    public event Action OnHeartStatusChanged;
    // NOTE: Remote config if needed

    #region Data

    private CostopiaLevelEventSO EventPlaying { get; set; }
    public CostopiaLevelSO LevelPlaying { get; private set; }

    private PlayingMode ModePlaying { get; set; }

    private bool IsFirstTimePlay { get; set; }


    public int LevelIndexPlaying { get; private set; }

    public int CurrentMappedLevel => EventPlaying.GetMappedLevel(LevelIndexPlaying);

    public BoosterUsedData BoosterUsedDataAll { get; private set; } = new BoosterUsedData();
    public BoosterUsedData BoosterUsedDataAds { get; private set; } = new BoosterUsedData();

    private PlayingState _playingState = PlayingState.None;
    public PlayingState PlayState => _playingState;
    public bool IsInGameplay => _playingState != PlayingState.None;


    public void PlayLevel(CostopiaLevelEventSO eventSO, int levelIndex, PlayingMode mode = PlayingMode.Normal)
    {
      if (_isLoadingScene)
      {
        // H3Log.Log("Loading");
        return;
      }
      _isLoadingScene = true;

      EventPlaying = eventSO;
      LevelIndexPlaying = levelIndex;
      ModePlaying = mode;
      LevelPlaying = null;
      _reviveCount = 0;
      _playingState = PlayingState.None;
      _lastTimeDoneStep = 0;

      // H3Log.Log("Play Event " + eventSO.NameLocalizeKey + " Level: " + levelIndex);

      string levelId = CostopiaConstant.LevelIndexToLevelId(levelIndex);
      void LoadScene()
      {
        // H3Log.Log("Loading Scene");
        LoadLevelAsync();
      }

      async void LoadLevelAsync()
      {
        int mappedLevel = EventPlaying.GetMappedLevel(LevelIndexPlaying);
        SceneManager.LoadScene(LevelPlaying.SceneName);

        // check scene loaded
        Scene loadedScene = SceneManager.GetSceneByName(LevelPlaying.SceneName);
        if (!loadedScene.IsValid())
        {
          // H3Log.LogError("Scene " + LevelPlaying.SceneName + " is not loaded");
          SceneManager.LoadScene(CostopiaConstant.ErrorSceneName);
        }

        _isLoadingScene = false;
      }
    }

    #endregion


    private void RecoverHeart()
    {
      if (_holdingHeart > 0)
      {
        int addValue = _holdingHeart;
        _holdingHeart = 0;
        try { OnHeartStatusChanged?.Invoke(); }
        catch (Exception e) { Debug.LogException(e); }
      }
    }

    public void RetryLevel()
    {
      // RecoverHeart();
      PlayLevel(EventPlaying, LevelIndexPlaying);
    }

    public int WinnedLevelId { get; private set; } = -1;

    public void ReturnHome(bool returnByWin)
    {
      if (returnByWin)
      {
        WinnedLevelId = LevelIndexPlaying;
      }
      LevelIndexPlaying = -1;
      _playingState = PlayingState.None;
    }

    public void ResetCheckWin()
    {
      WinnedLevelId = -1;
    }
  }
}