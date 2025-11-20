using System;
using System.Collections;
using System.Collections.Generic;
using Costopia;
using Costopia.Level;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utilities;

public enum LevelPlayMode
{
  Auto,
  Normal
}

public class LevelBase : MonoBehaviour
{
  public enum LevelPlayState
  {
    Loading,
    Playing,
    Pausing,
    BeginLose,
    BeginWin,
    Lost,
    Winned,
    Skip,
    DoneSkip
  }

  public enum UpdateTimeCooldownType
  {
    /// <summary>
    ///     Keep current time cooldown
    /// </summary>
    Keep,

    /// <summary>
    ///     Set time cooldown to max time limit
    /// </summary>
    ToMax,

    /// <summary>
    ///     Increase time cooldown by amount equal to max time limit added
    /// </summary>
    Increase
  }

  #region Old Value

  [Serializable]
  public class HintGroup
  {
    public List<Sprite> sprites;
  }

  #endregion

  // ========== Params ===========

  private Action _delegatePlayRandomBgm;
  public LevelLoseTypeCostopia LoseType { get; private set; } = LevelLoseTypeCostopia.None;

  public static LevelBase Instance { get; private set; }
  public bool IsPlayed { get; private set; }
  public bool IsFirstInteracted { get; private set; }

  // ========== Execution ===========
  public event Action OnStartPlay;
  public event Action OnFirstInteract;
  public event Action OnBeginSkip;
  public event Action OnSkip;
  public event Action OnSkipAfterLose;
  public event Action OnBeginWin;
  public event Action OnWin;
  public event Action OnBeginLose;
  public event Action OnLose;

#if UNITY_EDITOR
  private void
      UpdateCheatEditor()
  {
    if (Input.GetKeyDown(KeyCode.F1)) Debug.Break();
    if (Input.GetKeyDown(KeyCode.F2)) Time.timeScale = Mathf.Approximately(Time.timeScale, 1) ? 0f : 1;
    if (Input.GetKeyDown(KeyCode.F3)) LoseGame();
    if (Input.GetKeyDown(KeyCode.F4)) LoseGame(true, LevelLoseTypeCostopia.OutOfMood);
    if (Input.GetKeyDown(KeyCode.F5))
      // reload current scene
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    if (Input.GetKeyDown(KeyCode.F6)) WinGame();
  }
#endif

  //================================================================================================================

  #region State

  public LevelPlayState PlayState { get; private set; } = LevelPlayState.Loading;

  public delegate void OnPlayStateChangedDelegate(LevelPlayState oldState, LevelPlayState newState);

  public event OnPlayStateChangedDelegate OnPlayStateChanged;

  public void ChangePlayState(LevelPlayState newState)
  {
    if (PlayState == newState) return;
    LevelPlayState oldState = PlayState;
    PlayState = newState;
    switch (newState)
    {
      case LevelPlayState.Loading:
        BlockPlayerInteractModifiers.AddModifier(this);
        StopTimeCounterModifiers.AddModifier(this);
        break;
      case LevelPlayState.Playing:
        Time.timeScale = 1f;
        BlockPlayerInteractModifiers.RemoveModifier(this);
        StopTimeCounterModifiers.RemoveModifier(this);
        break;
      case LevelPlayState.Pausing:
        Time.timeScale = 0f;
        BlockPlayerInteractModifiers.AddModifier(this);
        StopTimeCounterModifiers.AddModifier(this);
        break;
      case LevelPlayState.BeginLose:
        // Time.timeScale = 0f;
        break;
      case LevelPlayState.BeginWin:
      case LevelPlayState.Lost:
      case LevelPlayState.Winned:
      case LevelPlayState.Skip:
        BlockPlayerInteractModifiers.AddModifier(this);
        StopTimeCounterModifiers.AddModifier(this);
        break;
    }

    OnPlayStateChanged?.Invoke(oldState, newState);
  }

  private const int MaskIsInEndGameState = (1 << (int)LevelPlayState.Lost) | (1 << (int)LevelPlayState.Winned) |
                                           (1 << (int)LevelPlayState.BeginLose) |
                                           (1 << (int)LevelPlayState.BeginWin) | (1 << (int)LevelPlayState.Skip);

  public bool IsInEndGameState => ((1 << (int)PlayState) & MaskIsInEndGameState) != 0;

  public BoolModifierWithRegisteredSource BlockPlayerInteractModifiers { get; private set; }
  public event Action OnBlockPlayerInteractChanged;
  public bool IsAllowInteract => !BlockPlayerInteractModifiers.Value;

  #endregion

  //================================================================================================================

  #region Step

  public event Action<int, int> OnStepChanged;
  public event Action<int> OnPhaseChanged;

  public event Action<int, int> OnDoneStep;

  protected void CheckDoneStep()
  {
    OnDoneStep?.Invoke(currentPhase, currentStep);
  }

  public virtual int TotalStep()
  {
    return -1;
  }

  public virtual int TotalPhase()
  {
    return -1;
  }

  public virtual int TotalStepAtPhase(int phase)
  {
    return -1;
  }

  protected void SetStep(int value)
  {
    if (CurrentStep == value) return;
    CurrentStep = value;
  }

  protected void SetPhase(int value)
  {
    if (CurrentPhase == value) return;
    CurrentPhase = value;
  }

  [SerializeField] protected int currentPhase;
  [SerializeField] protected int currentStep;

  public int CurrentStep
  {
    get => currentStep;
    private set
    {
      if (value < 0) value = 0;
      currentStep = value;
      OnStepChanged?.Invoke(currentPhase, currentStep);
    }
  }

  public int CurrentPhase
  {
    get => currentPhase;
    private set
    {
      if (value < 0) value = 0;
      currentPhase = value;
      OnPhaseChanged?.Invoke(currentPhase);
    }
  }

  #endregion

  //================================================================================================================

  #region Base Config

  [Header("Common")]

  [SerializeField]
  private LevelPlayMode levelPlayMode = LevelPlayMode.Auto;


  [SerializeField]
  private bool isAllowMultitouch;


  [SerializeField]
  private AudioClip bgMusic;

  public LevelPlayMode LevelPlayMode => levelPlayMode;

  #endregion

  #region Time Limit

  //================================================================================================================
  public event Action OnTimeLimitChanged;
  public event Action<float> OnTimeRemainedChanged;
  public event Action<int> OnMoodRemainedChanged;
  public event Action<int> OnHintShow;
  public event Action<float> OnTimeCounterChanged;

  public bool IsUseLimitTime => TotalTimeLimit > 0;
  public float TotalTimeLimit { get; private set; }
  public int MoodWrongDecrease { get; private set; }
  public int MoodCorrectIncrease { get; private set; }
  public float TimeRemained { get; private set; }
  public int MoodRemained { get; private set; }

  public float TimeRemainedRatio => TotalTimeLimit > 0f ? TimeRemained / TotalTimeLimit : 0f;

  /// <summary>
  ///     Include time after save me, not include stop time
  /// </summary>
  public float TimePlaying { get; private set; }

  private float _unscaledTimeStartPlayLevel;

  /// <summary>
  ///     Include all: play time, save me, narrative, cut scene, animation end game
  /// </summary>
  public float RealTimePlayLevel => Time.unscaledTime - _unscaledTimeStartPlayLevel;

  public BoolModifierWithRegisteredSource StopTimeCounterModifiers { get; private set; }
  private const int DEFAULT_STOP_TIME_MODIFIER_SOURCE_ID = -1;

  public bool IsStopTime
  {
    get => StopTimeCounterModifiers.Value;
    set
    {
      if (value)
        StopTimeCounterModifiers.AddModifier(DEFAULT_STOP_TIME_MODIFIER_SOURCE_ID);
      else
        StopTimeCounterModifiers.RemoveModifier(DEFAULT_STOP_TIME_MODIFIER_SOURCE_ID);
    }
  }

  public void SetTimeRemained(float value)
  {
    TimeRemained = Mathf.Max(value, 0.001f);
    OnTimeRemainedChanged?.Invoke(TimeRemained);
  }

  public void SetMoodRemained(int value)
  {
    MoodRemained = Mathf.Max(value, 0);
    OnMoodRemainedChanged?.Invoke(MoodRemained);
  }

  public void DecreaseMood()
  {
    MoodRemained = Mathf.Max(MoodRemained - MoodWrongDecrease, 0);
    OnMoodRemainedChanged?.Invoke(MoodRemained);
  }

  public void IncreaseMood()
  {
    MoodRemained = Mathf.Min(MoodRemained + MoodCorrectIncrease, InGameSysConfigCostopia.MAXIMUM_MOOD);
    OnMoodRemainedChanged?.Invoke(MoodRemained);
  }

  public void DisableTimeLimit()
  {
    SetTimeLimit(0);
  }

  public void SetTimeLimit(float maxTimeLimit,
      UpdateTimeCooldownType updateTimeCooldownType = UpdateTimeCooldownType.ToMax)
  {
    if (maxTimeLimit > 0)
    {
      float increaseTime = Mathf.Max(0f, maxTimeLimit - TotalTimeLimit);
      TotalTimeLimit = maxTimeLimit;

      switch (updateTimeCooldownType)
      {
        case UpdateTimeCooldownType.Keep:
          TimeRemained = Mathf.Min(TimeRemained, maxTimeLimit);
          break;
        case UpdateTimeCooldownType.ToMax:
          TimeRemained = maxTimeLimit;
          break;
        case UpdateTimeCooldownType.Increase:
          TimeRemained = Mathf.Min(TimeRemained + increaseTime, maxTimeLimit);
          break;
      }
    }
    else
    {
      TimeRemained = TotalTimeLimit = 0;
    }

    OnTimeLimitChanged?.Invoke();
  }

  public void UseBoosterMoreTime()
  {
    float timeAdded = _inGameSysConfig.MoreTime;
    TimeRemained += timeAdded;
    NumUseTimeBooster += 1;
    OnTimeRemainedChanged?.Invoke(TimeRemained);
    OnTimeCounterChanged?.Invoke(timeAdded);
  }

  public void UseBoosterUpMood()
  {
    int moodAdded = _inGameSysConfig.UpMood;
    MoodRemained = Mathf.Min(MoodRemained + moodAdded, InGameSysConfigCostopia.MAXIMUM_MOOD);
    NumUseMoodBooster += 1;
    OnMoodRemainedChanged?.Invoke(MoodRemained);
  }

  public void UseBoosterHint()
  {
    NumUseHintBooster += 1;
    OnHintShow?.Invoke(currentStep);
  }

  public void AddTimeBySaveMe()
  {
    TimeRemained = _inGameSysConfig.SaveMeTime;
    OnTimeCounterChanged?.Invoke(TimeRemained);
  }

  public void AddMoodBySaveMe()
  {
    MoodRemained = _inGameSysConfig.SaveMeMoodGain;
    OnMoodRemainedChanged?.Invoke(MoodRemained);
  }

  #endregion

  #region Reward Stars

  //================================================================================================================
  [Header("Reward Stars")]

  [SerializeField]
  private bool isUseTimeCounterToCheckStars;



  [SerializeField]
  private float timeRatioRewardThreeStars = 0.5f;



  [SerializeField]
  private float timeRatioRewardTwoStars = 0.25f;

  public bool IsUseTimeCounterToCheckStarsReward => isUseTimeCounterToCheckStars;
  public float TimeRatioRequiredThreeStars => timeRatioRewardThreeStars;
  public float TimeRatioRequiredTwoStars => timeRatioRewardTwoStars;

  private void UpdateRewardStarsByTimeCounter()
  {
    if (RewardedStars == 3)
      if (TimeRemainedRatio < timeRatioRewardThreeStars)
        SetRewardedStars(2);

    if (RewardedStars == 2)
      if (TimeRemainedRatio < timeRatioRewardTwoStars)
        SetRewardedStars(1);
  }

  private const int MAX_REWARD_STARS = 3;
  public event Action OnRewardStarsChanged;
  public int RewardedStars { get; private set; } = MAX_REWARD_STARS;

  public void SetRewardedStars(int rewardedStars)
  {
    if (RewardedStars == rewardedStars) return;
    RewardedStars = rewardedStars;
    OnRewardStarsChanged?.Invoke();
  }

  public void SetMaxRewardedStars(int rewardedStars)
  {
    if (rewardedStars < RewardedStars)
    {
      RewardedStars = rewardedStars;
      OnRewardStarsChanged?.Invoke();
    }
  }

  #endregion

  #region Save me

  //================================================================================================================
  public delegate void SaveMeDelegate(Action onSaved, Action onFailed);

  /// <summary>
  ///     Will be called when player about to lose. If null, player will lose immediately.
  /// </summary>
  private Action<Action, Action> _saveMeCallback;

  public void SetSaveMeCallback(Action<Action, Action> saveMeCallback)
  {
    _saveMeCallback = saveMeCallback;
  }

  /// <summary>
  ///     Is waiting for using save me?
  /// </summary>
  public bool IsSaveMe { get; private set; }

  public int NumSaveMeDoneUsed { get; private set; }
  public int NumUseTimeBooster { get; private set; }
  public int NumUseMoodBooster { get; private set; }
  public int NumUseHintBooster { get; private set; }

  #endregion

  #region End Game Params

  //================================================================================================================
  [Header("End Game")]

  public UnityEvent onBeforeEndDelay;

  public UnityEvent onAfterEndDelay;


  [SerializeField]
  private bool playSuccessSoundOnEnd;


  [SerializeField]
  private float beforeEndGameDelay;


  [SerializeField]
  private bool playEndMusicBeforeDelay = true;

  private const float DELAY_LOSE_GAME = 2f;
  private const float DELAY_WIN_GAME = 0.5f;

  public bool isEndingGame;
  public bool IsLoseGame => PlayState == LevelPlayState.Lost || PlayState == LevelPlayState.BeginLose;
  public bool IsSkip { get; private set; }

  #endregion

  #region SetUp

  private InGameSysConfigCostopia _inGameSysConfig;

  //================================================================================================================
  protected virtual void Awake()
  {
    Instance = this;

    StopTimeCounterModifiers = new BoolModifierWithRegisteredSource();
    BlockPlayerInteractModifiers =
        new BoolModifierWithRegisteredSource(() => OnBlockPlayerInteractChanged?.Invoke());
    _unscaledTimeStartPlayLevel = Time.unscaledTime;

    if (levelPlayMode == LevelPlayMode.Auto) levelPlayMode = LevelPlayMode.Normal;

    // if (LevelPlayingControlCostopia.IsExistInstance) LevelPlayingControlCostopia.Ins.OnLevelAwake(this);

    // 

    MoodRemained = InGameSysConfigCostopia.MAXIMUM_MOOD;
    OnMoodRemainedChanged += CheckMoodRemainedState;
  }

  private void SetLevelInfo(int currentLevelIndex, int currentMappedLevel, InGameSysConfigCostopia costopiaLevelConfig,
      Action delegatePlayRandomBgm)
  {
    _inGameSysConfig = costopiaLevelConfig;
    _delegatePlayRandomBgm = delegatePlayRandomBgm;
  }

  protected virtual void Start()
  {
    Input.multiTouchEnabled = isAllowMultitouch;

    // audio
    // AudioManager.StopMusic();

    if (bgMusic != null) { } // AudioManager.PlayMusic(bgMusic);
    else
    {
      _delegatePlayRandomBgm?.Invoke();
    }

    //Time limit
    float maxTimeLimit = TotalTimeLimit;

    SetTimeLimit(maxTimeLimit);

    // start play
    TimePlaying = 0f;
    ChangePlayState(LevelPlayState.Playing);
    IsPlayed = true;
    OnStartPlay?.Invoke();
    StartCoroutine(IEWaitFirstInteract());
  }

  private const int ID_MODIFIER_STOP_TIME_UNTIL_FIRST_INTERACT = -23;

  private IEnumerator IEWaitFirstInteract()
  {
    StopTimeCounterModifiers.AddModifier(ID_MODIFIER_STOP_TIME_UNTIL_FIRST_INTERACT);
    while (!Input.GetMouseButton(0)) yield return null;
    StopTimeCounterModifiers.RemoveModifier(ID_MODIFIER_STOP_TIME_UNTIL_FIRST_INTERACT);
    IsFirstInteracted = true;
    OnFirstInteract?.Invoke();
  }

  protected virtual void OnDestroy()
  {
    Input.multiTouchEnabled = true;
    Resources.UnloadUnusedAssets();
    if (Instance == this) Instance = null;
    Time.timeScale = 1;
    // HProcess.RemoveActWithKey(KeyAction.Evt_BuyShopID, OnBuyShop);

    // 

    OnMoodRemainedChanged -= CheckMoodRemainedState;
  }

  #endregion

  #region Playing

  //================================================================================================================
  protected virtual void Update()
  {
#if UNITY_EDITOR
    UpdateCheatEditor();
#endif
    CooldownLimitTime();
  }

  private void CooldownLimitTime()
  {
    if (IsUseLimitTime && !IsStopTime && PlayState == LevelPlayState.Playing && !isEndingGame && TimeRemained > 0)
    {
      TimeRemained -= Time.deltaTime;
      OnTimeRemainedChanged?.Invoke(TimeRemained);
      if (TimeRemained < 0)
      {
        SetRewardedStars(0);
        LoseGame();
      }
      else
      {
        if (isUseTimeCounterToCheckStars) UpdateRewardStarsByTimeCounter();
      }
    }

    if (!IsStopTime && !isEndingGame) TimePlaying += Time.deltaTime;
  }

  private void CheckMoodRemainedState(int moodRemained)
  {
    if (moodRemained == 0) LoseGame(true, LevelLoseTypeCostopia.OutOfMood);
  }

  #endregion

  #region End Game

  //================================================================================================================
  /// <summary>
  ///     Call to WIN game
  /// </summary>
  public void WinGame()
  {
    EndGame();
  }

  /// <summary>
  ///     Call to WIN game. But should call WinGame() instead for clear code.
  /// </summary>
  public virtual void EndGame()
  {
    if (IsInEndGameState) return;

    ChangePlayState(LevelPlayState.BeginWin);
    OnBeginWin?.Invoke();

    isEndingGame = true;

    StartCoroutine(CorWaitEndGame());

    IEnumerator CorWaitEndGame()
    {
      // AudioManager.StopMusic();
      if (playSuccessSoundOnEnd) // AudioManager.PlaySFX(CommonSound.Success);
        if (beforeEndGameDelay > 0) yield return new WaitForSecondsRealtime(beforeEndGameDelay);
      if (playEndMusicBeforeDelay) // AudioManager.PlaySFX(CommonSound.LevelEnd);
        onBeforeEndDelay?.Invoke();
      yield return
          new WaitForSecondsRealtime(
              DELAY_WIN_GAME); // @daivq: it's valid use of endGameDelay, please ignore obsolete warning
      if (!playEndMusicBeforeDelay) // AudioManager.PlaySFX(CommonSound.LevelEnd);
        onAfterEndDelay?.Invoke();
      ChangePlayState(LevelPlayState.Winned);
      HandleWinGame();
    }
  }

  public void SkipLevel(Action onDone)
  {
    if (IsInEndGameState) return;

    ChangePlayState(LevelPlayState.Skip);
    isEndingGame = true;

    IsSkip = true;
    SetRewardedStars(1);

    OnBeginSkip?.Invoke();

    StartCoroutine(CorWaitSkip(onDone));

    IEnumerator CorWaitSkip(Action onDoneCallback)
    {
      // AudioManager.StopMusic();
      // AudioManager.PlaySFX(CommonSound.LevelEnd);
      yield return new WaitForSecondsRealtime(0.5f);
      onDoneCallback?.Invoke();
      ChangePlayState(LevelPlayState.DoneSkip);
      OnSkip?.Invoke();
    }
  }

  public void SkipLevelAfterLose()
  {
    ChangePlayState(LevelPlayState.Skip);
    IsSkip = true;
    SetRewardedStars(1);
    OnSkipAfterLose?.Invoke();
  }

  /// <summary>
  ///     Call to LOSE game immediately<br />
  ///     Also auto lose when time out
  ///     If there're save me available, will call to save me first
  /// </summary>
  public void LoseGame(bool isAllowSaveMe = true, LevelLoseTypeCostopia loseType = LevelLoseTypeCostopia.OutOfTime)
  {
    if (IsInEndGameState) return;
    LoseType = loseType;
    ChangePlayState(LevelPlayState.BeginLose);
    OnBeginLose?.Invoke();

    if (isAllowSaveMe && _saveMeCallback != null)
    {
      IsSaveMe = true;
      _saveMeCallback.Invoke(OnSaveMeSuccess, OnSaveMeFailed);
    }
    else
    {
      OnSaveMeFailed();
    }

    void OnSaveMeSuccess()
    {
      NumSaveMeDoneUsed += 1;
      IsSaveMe = false;
      isEndingGame = false;
      HandleSaveMeSuccess();
    }

    void OnSaveMeFailed()
    {
      IsSaveMe = false;

      SetRewardedStars(0);

      isEndingGame = true;
      ChangePlayState(LevelPlayState.Lost);
      StartCoroutine(CorWaitEndGame());
    }

    IEnumerator CorWaitEndGame()
    {
      // AudioManager.StopMusic();
      onBeforeEndDelay?.Invoke();
      yield return
          new WaitForSeconds(
              DELAY_LOSE_GAME); // @daivq: it's valid use of endGameDelay, please ignore obsolete warning
      onAfterEndDelay?.Invoke();
      HandleLoseGame();
    }
  }

  protected virtual void HandleWinGame()
  {
    OnWin?.Invoke();
  }

  protected virtual void HandleSaveMeSuccess()
  {
    SetRewardedStars(2);
    ChangePlayState(LevelPlayState.Playing);
  }

  protected virtual void HandleLoseGame()
  {
    OnLose?.Invoke();
  }

  #endregion
}
