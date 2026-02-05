using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Satisgame;
using HoangHH;
using System;
using System.Collections;

public class GamePlayManager : Singleton<GamePlayManager>
{
  public EmojiControl emojiControl;
  public ClockTimer clockTimer;
  public CharacterControl character;
  public FillCircleBar fillCircleBar;
  public List<Item> items = new List<Item>();
  [SerializeField] protected int currentStep = 0;
  [SerializeField] private bool isChangeAnimHappy = true;
  public bool IsChangeAnimHappy => isChangeAnimHappy;
  public int CurrentStep => currentStep;
  private IEnumerator coroutine = null;
  public void SetEmissionRate(ParticleSystem particle, float rate)
  {
    ParticleSystem.EmissionModule emission = particle.emission;
    emission.rateOverTime = rate;
  }
  public void SetStateChangeAnim(bool value)
  {
    isChangeAnimHappy = value;
  }
  protected void TurnCharacterSlotAttachment(
      List<SlotAttachmentPair> slotDataList,
      bool attached)
  {
    for (int i = 0; i < slotDataList.Count; i++)
    {
      character.TurnSlotAttachment(
          slotDataList[i].slotName,
          attached ? slotDataList[i].attachmentName : null
      );
    }
  }

  protected virtual void Start()
  {
    StartStep();
  }

  public virtual void SetStep(int step)
  {
    currentStep = step;
    StartStep();
  }
  private bool isDoneStep = false;
  public bool IsDoneStep => isDoneStep;
  protected virtual void DoneStep()
  {
    PlayPositiveEmoji();
    isDoneStep = true;
    Debug.Log("Fx step" + currentStep);
  }

  public void TryNextStep()
  {
    currentStep++;
    Debug.LogWarning("Next To" + currentStep);
    StartStep();
  }

#if UNITY_EDITOR
    [Button]
#endif
  public virtual void StartStep()
  {
    isDoneStep = false;
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
    if (isChangeAnimHappy)
    {
      character.SetMouseHappy();

    }
    else
    {
      character.SetMouseIdle();
    }
    var i = idSoundHappy % 3;
    if (i == 0) SoundManager.Ins.PlayFx(FxType.Happy0);
    else if (i == 1) SoundManager.Ins.PlayFx(FxType.Happy1);
    else SoundManager.Ins.PlayFx(FxType.Happy2);
    idSoundHappy++;
    yield return new WaitForSeconds(1f);
    character.SetMouseIdle();
    coroutine = null;
  }
  public void PlayPositiveEmojiOnSnap()
  {
    if (coroutine != null)
    {
      StopCoroutine(coroutine);
    }

    coroutine = PlayHappyOnSnap();
    StartCoroutine(coroutine);
  }
  private IEnumerator PlayHappyOnSnap()
  {
    character.SetMouseHappy();
    yield return new WaitForSeconds(1f);
    character.SetMouseIdle();
    coroutine = null;
  }
  public void PlayNegativeEmoji()
  {
    if (coroutine != null)
    {
      return;
    }
    coroutine = PlayNegativeEmojiCoroutine();
    StartCoroutine(coroutine);
  }
  private int idSoundAngry = 0;
  private IEnumerator PlayNegativeEmojiCoroutine()
  {
    yield return new WaitForSeconds(0.01f);
    character.SetMouseAngry();
    emojiControl.ShowNegative();
    var i = idSoundAngry % 3;
    if (i == 0) SoundManager.Ins.PlayFx(FxType.Angry0);
    else if (i == 1) SoundManager.Ins.PlayFx(FxType.Angry1);
    else SoundManager.Ins.PlayFx(FxType.Angry2);
    idSoundAngry++;
    yield return new WaitForSeconds(0.65f);
    character.SetMouseIdle();
    coroutine = null;
  }

  public void ShowClockTimer()
  {
    clockTimer.Show(2f);
  }

}
