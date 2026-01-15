using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HoangHH;
using Satisgame;
// using Sirenix.Utilities;
// using Spine.Unity;
// using Unity.VisualScripting;
using UnityEngine;
// using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
  public EmojiControl emojiControl;
  public ClockTimer clockTimer;
  public FillCircleBar fillCircleBar;
  public bool isPlayingGame = false;
  public CharacterControl Body1, Body2;
  public ChangeScene changeScene;

  public GameObject Scene0, Scene1, Scene2;


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

  public void ChangeSceneToGamePlay2(Action onComplete = null)
  {
    changeScene.ChangeSceneAndCall(() =>
    {
      Scene1.SetActive(false);
      Scene2.SetActive(true);
      onComplete?.Invoke();
      GamePlayManager.Ins.character = Body2;
      emojiControl.transform.localPosition = new Vector3(2.11f, 6.17f, 0f);
    });
  }

  public void StartGamePlay()
  {
    Scene1.SetActive(true);
    Scene0.SetActive(false);
    EventManager.TriggerEvent("ShowText2");
    TutorialManager.Ins.enableCountTime = true;
    SoundManager.Ins.PlayFx(FxType.StartGame);
    DOVirtual.DelayedCall(0.2f, () =>
    {
      isPlayingGame = true;
    });

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
    GamePlayManager.Ins.character.SetMouseHappy();
    var i = idSoundHappy % 3;


    if (i == 0) SoundManager.Ins.PlayFx(FxType.Happy0);
    else if (i == 1) SoundManager.Ins.PlayFx(FxType.Happy1);
    else SoundManager.Ins.PlayFx(FxType.Happy2);

    idSoundHappy++;

    yield return new WaitForSeconds(1f);
    GamePlayManager.Ins.character.SetMouseIdle();
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
    GamePlayManager.Ins.character.SetMouseAngry();
    emojiControl.ShowNegative();

    var i = idSoundAngry % 3;

    if (i == 0) SoundManager.Ins.PlayFx(FxType.Angry0);
    else if (i == 1) SoundManager.Ins.PlayFx(FxType.Angry1);
    else SoundManager.Ins.PlayFx(FxType.Angry2);
    idSoundAngry++;

    yield return new WaitForSeconds(0.65f);
    GamePlayManager.Ins.character.SetMouseIdle();
    coroutine = null;
  }

  public void ShowClockTimer()
  {
    clockTimer.Show(2f);
  }

}
