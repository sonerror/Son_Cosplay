using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
  // public CharacterControl Body1, Body2;
  // public ChangeScene changeScene;

  // public GameObject Scene0, Scene1, Scene2;
  // public Animator ContentGamePlay;

  // private int typeScape = 0; // 0: none, 1: landscape, 2: portrait
  // void FixedUpdate()
  // {
  //   if (isPlayingGame) return;
  //   var size = UIManager.Ins.GameSize;
  //   if (size.x > size.y && typeScape != 1)
  //   {
  //     typeScape = 1;
  //     // Show landscape UI
  //     ContentGamePlay.transform.localScale = Vector3.one * 1.3f;
  //     ContentGamePlay.SetTrigger("Land");
  //   }
  //   else if (size.x <= size.y && typeScape != 2)
  //   {
  //     typeScape = 2;
  //     // Show portrait UI
  //     ContentGamePlay.transform.localScale = Vector3.one * 1f;
  //     ContentGamePlay.SetTrigger("Por");
  //   }
  // }

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
      // StartGamePlay();
    }
  }

  void StartGamePlay()
  {
    // Scene0.SetActive(false);
    // Scene1.SetActive(true);

    // changeScene.ChangeSceneAndCall(() =>
    // {
    //   Scene0.SetActive(false);
    //   Scene1.SetActive(true);
    //   // GamePlayManager.Ins.StartStep();
    // });

    EventManager.TriggerEvent("ShowText2");
    TutorialManager.Ins.enableCountTime = true;
    // SoundManager.Ins.PlayFx(FxType.StartGame);
    DOVirtual.DelayedCall(0.2f, () =>
    {
      isPlayingGame = true;
    });

  }

  public bool typeGamePlay1 = true;
  // public void ChooseScene1()
  // {
  //   if (isPlayingGame) return;
  //   StartGamePlay();
  //   Scene1.SetActive(true);

  //   // GamePlayManager.Ins.character = Body1;
  //   GamePlayManager.Ins.SetStep(0);
  //   SoundManager.Ins.PlayFx(FxType.StartGame);
  //   typeGamePlay1 = true;
  //   // Destroy(Scene0);
  //   // Destroy(Scene2);

  //   Scene0.SetActive(false);
  //   Scene2.SetActive(false);
  // }
  // public void ChooseScene2()
  // {
  //   if (isPlayingGame) return;
  //   StartGamePlay();
  //   Scene2.SetActive(true);

  //   // GamePlayManager.Ins.character = Body2;
  //   GamePlayManager.Ins.SetStep(6);
  //   SoundManager.Ins.PlayFx(FxType.WomanHey);
  //   typeGamePlay1 = false;
  //   // Destroy(Scene1);
  //   //  Destroy(Scene0);

  //   Scene0.SetActive(false);
  //   Scene1.SetActive(false);
  // }

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

    if (typeGamePlay1)
    {
      if (i == 0) SoundManager.Ins.PlayFx(FxType.Happy0);
      else if (i == 1) SoundManager.Ins.PlayFx(FxType.Happy1);
      else SoundManager.Ins.PlayFx(FxType.Happy2);
    }
    else
    {
      if (i == 0) SoundManager.Ins.PlayFx(FxType.Happy0_1);
      else if (i == 1) SoundManager.Ins.PlayFx(FxType.Happy1_1);
      else SoundManager.Ins.PlayFx(FxType.Happy2_1);
    }
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


    if (typeGamePlay1)
    {
      if (i == 0) SoundManager.Ins.PlayFx(FxType.Angry0);
      else if (i == 1) SoundManager.Ins.PlayFx(FxType.Angry1);
      else SoundManager.Ins.PlayFx(FxType.Angry2);
    }
    else
    {
      if (i == 0) SoundManager.Ins.PlayFx(FxType.Angry0_1);
      else if (i == 1) SoundManager.Ins.PlayFx(FxType.Angry1_1);
      else SoundManager.Ins.PlayFx(FxType.Angry2_1);
    }
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
