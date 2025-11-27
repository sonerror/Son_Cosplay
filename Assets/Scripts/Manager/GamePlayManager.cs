using System.Collections;
using System.Collections.Generic;
using HoangHH;
using Satisgame;
using Sirenix.Utilities;
using Spine.Unity;
using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
  public EmojiControl emojiControl;
  public ClockTimer clockTimer;
  public bool isPlayingGame = false;
  public SkeletonAnimation playerSkeleton;
  public CharactorControl characterControl;

  public GameObject Scene0, Scene1;

  public List<Item> items = new List<Item>();

  private IEnumerator coroutine = null;

  private bool hadClicked = false;

  private void Update()
  {
    if (isPlayingGame && Input.GetMouseButtonDown(0))
    {
      EventManager.TriggerEvent("ShowBtnInstall");
    }

    if (!hadClicked && Input.GetMouseButtonDown(0))
    {
      hadClicked = true;
      StartGamePlay();
    }
  }

  void StartGamePlay()
  {
    Scene0.SetActive(false);
    Scene1.SetActive(true);
    TutorialManager.Ins.enableCountTime = true;
    isPlayingGame = true;
  }

  public void PlayPositiveEmoji()
  {
    if (coroutine != null)
    {
      StopCoroutine(coroutine);
    }

    coroutine = PlayPositiveEmojiCoroutine();
    StartCoroutine(coroutine);
  }

  private IEnumerator PlayPositiveEmojiCoroutine()
  {
    emojiControl.ShowPositive();
    playerSkeleton.AnimationState.SetAnimation(0, "happy", false);
    SoundManager.Ins.PlayFx(FxType.Happy);
    yield return new WaitForSeconds(1f);
    playerSkeleton.AnimationState.SetAnimation(0, "idle", true);
    coroutine = null;
  }
  public void PlayNegativeEmoji()
  {
    if (coroutine != null)
    {
      StopCoroutine(coroutine);
      return;
    }

    coroutine = PlayNegativeEmojiCoroutine();
    StartCoroutine(coroutine);
  }

  private IEnumerator PlayNegativeEmojiCoroutine()
  {
    yield return new WaitForSeconds(1f);
    playerSkeleton.AnimationState.SetAnimation(0, "angry", false);
    emojiControl.ShowNegative();
    SoundManager.Ins.PlayFx(FxType.Angry);
    yield return new WaitForSeconds(1f);
    playerSkeleton.AnimationState.SetAnimation(0, "idle", true);
    coroutine = null;
  }

  public void ShowClockTimer()
  {
    clockTimer.Show(2f);
  }


  [SerializeField]
  private int currState = 0;
  public int CurrState { get { return currState; } }

  bool isDoneState1 = false;
  [SerializeField]
  private List<int> stateId = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
  public List<int> StateId { get { return stateId; } }
  public void OnDoneState(int state)
  {
    currState++;
    stateId.Remove(state);
    if (!isDoneState1 && !stateId.Contains(0) && !stateId.Contains(1))
    {
      isDoneState1 = true;
      for (int i = 3; i < items.Count; i++)
      {
        items[i].IsReady = true;
      }
    }

    if (stateId.Count <= 1) GameManager.Ins.showEndGame();
  }
}
