using System.Collections;
using System.Collections.Generic;
using Satisgame;
using Spine.Unity;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
  public EmojiControl emojiControl;
  public SkeletonAnimation playerSkeleton;

  public GameObject Scene0, Scene1;

  public List<Item> items = new List<Item>();

  private IEnumerator coroutine = null;

  private bool hadClicked = false;

  void Start()
  {
    Setstate();
  }

  private void Update()
  {
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

  private int currState = 0;

  public void OnDoneState()
  {
    currState++;
    Setstate();
  }

  void Setstate()
  {
    for (int i = 0; i < items.Count; i++)
    {
      items[i].SetIsReady(i == currState);
    }

    if (currState >= items.Count)
    {
      GameManager.Ins.showEndGame();
    }
  }

}
