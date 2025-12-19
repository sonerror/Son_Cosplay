using System.Collections;
using System.Collections.Generic;
using HoangHH;
using Satisgame;
using Sirenix.Utilities;
using Spine.Unity;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
  public EmojiControl emojiControl;
  public ClockTimer clockTimer;
  public FillCircleBar fillCircleBar;
  public bool isPlayingGame = false;
  public SkeletonAnimation playerSkeleton;
  public CharacterControl characterControl;
  public ChangeScene changeScene;

  public GameObject Scene0, Scene1;

  private IEnumerator coroutine = null;

  private bool hadClicked = false;

  private void Update()
  {
    // if (isPlayingGame && Input.GetMouseButtonDown(0))
    // {
    //   EventManager.TriggerEvent("ShowBtnInstall");
    // }

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
    SoundManager.Ins.PlayFx(FxType.StartGame);
    isPlayingGame = true;
    EventManager.TriggerEvent("ShowBtnInstall");
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

  private int idSoundHappy = 0;
  private IEnumerator PlayPositiveEmojiCoroutine()
  {
    emojiControl.ShowPositive();
    playerSkeleton.AnimationState.SetAnimation(0, "happy", false);
    var i = idSoundHappy % 3;
    if (i == 0) SoundManager.Ins.PlayFx(FxType.Happy);
    else if (i == 1) SoundManager.Ins.PlayFx(FxType.Happy1);
    else SoundManager.Ins.PlayFx(FxType.Happy2);
    idSoundHappy++;

    yield return new WaitForSeconds(1f);
    playerSkeleton.AnimationState.SetAnimation(0, "idle", true);
    coroutine = null;
  }
  public void PlayNegativeEmoji()
  {
    if (coroutine != null)
    {
      // StopCoroutine(coroutine);
      return;
    }

    coroutine = PlayNegativeEmojiCoroutine();
    StartCoroutine(coroutine);
  }

  private int idSoundAngry = 0;
  private IEnumerator PlayNegativeEmojiCoroutine()
  {
    yield return new WaitForSeconds(1f);
    playerSkeleton.AnimationState.SetAnimation(0, "angry", false);
    emojiControl.ShowNegative();

    var i = idSoundAngry % 2;
    if (i == 0) SoundManager.Ins.PlayFx(FxType.Angry);
    else SoundManager.Ins.PlayFx(FxType.Angry1);
    idSoundAngry++;

    yield return new WaitForSeconds(1f);
    playerSkeleton.AnimationState.SetAnimation(0, "idle", true);
    coroutine = null;
  }

  public void ShowClockTimer()
  {
    clockTimer.Show(2f);
  }

}
