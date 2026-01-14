using System;
using System.Collections.Generic;
using System.Linq;
using AnhPD.Cook;
using DG.Tweening;
using HoangHH;
using Sirenix.OdinInspector;
// using Unity.VisualScripting;
using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
  public CharacterControl character;
  public ShowObjectEffect Gr1, Gr2;

  public List<Item> items = new List<Item>();

  // public void ChangeGroupItemStart()
  // {
  //   Debug.Log("ChangeGroupItemStart ");
  //   Gr1.Hide(0.5f);
  //   Gr2.Show(1f);
  // }

  [SerializeField] private int currentStep = 0;
  public int CurrentStep => currentStep;

  protected void TurnCharacterSlotAttachment(List<SlotAttachmentPair> slotDataList, bool attached)
  {
    for (int i = 0; i < slotDataList.Count; i++)
    {
      character.TurnSlotAttachment(slotDataList[i].slotName,
          attached ? slotDataList[i].attachmentName : null);
    }
  }

  void Start()
  {
    StartStep();
  }

  public void SetStep(int step)
  {
    currentStep = step;
    StartStep();
  }

  void DoneStep()
  {
    GameManager.Ins.PlayPositiveEmoji();
  }

  void TryNextStep()
  {
    currentStep++;
    Debug.LogWarning("nextto" + currentStep);
    StartStep();
  }

#if UNITY_EDITOR
  [Button]
#endif
  public void StartStep()
  {
    switch (currentStep)
    {
      case 0:
        OnStartStep0();
        break;
      case 1:
        OnStartStep1();
        break;
      case 2:
        OnStartStep2();
        break;
      case 3:
        OnStartStep3();
        break;
      case 4:
        OnStartStep4();
        break;
      case 5:
        OnStartStep5();
        break;
      case 6:
        OnStartStep6();
        break;
      case 7:
        OnStartStep7();
        break;
      case 8:
        OnStartStep8();
        break;
      case 9:
        OnStartStep9();
        break;
      case 10:
        OnStartStep10();
        break;
      case 11:
        OnStartStep11();
        break;
      default:
        Debug.LogWarning("No more steps!");
        break;
    }
  }

  #region Step0
  public List<RemoveItem> ClotherRemove = new List<RemoveItem>();

  private int countCutHair = 0;
  private void OnStartStep0()
  {
    ClotherRemove[0].SetReady();
    ClotherRemove[1].SetReady();
    ClotherRemove[2].SetReady();
    for (int i = 0; i < ClotherRemove.Count; i++)
    {
      ClotherRemove[i].OnFinish.AddListener(OnRemoveClother);
    }
  }

  void OnRemoveClother()
  {
    countCutHair++;
    SoundManager.Ins.PlayFx(FxType.Cut);
    if (countCutHair >= ClotherRemove.Count)
    {
      TutorialManager.Ins.IncreaseTimeHide();
      OnEndStep0();
    }
  }

  private void OnEndStep0()
  {
    DoneStep();
    TryNextStep();
  }
  #endregion Step0

  #region Step1
  private void OnStartStep1()
  {
    var item = items[1];
    item.SetReady();
    item.OnFinish.AddListener(OnEndStep1);
  }

  private void OnEndStep1()
  {
    var item = items[1];
    item.OnFinish.RemoveListener(OnEndStep1);

    DOVirtual.DelayedCall(1f, () =>
    {
      GameManager.Ins.ChangeSceneToGamePlay2(() =>
      {
        DoneStep();
        TryNextStep();
      });
    });
  }

  #endregion Step1

  #region Step2

  [SerializeField] private List<SlotAttachmentPair> slotStep2 = new List<SlotAttachmentPair>();


  private void OnStartStep2()
  {
    var item = items[2];
    item.OnFinish.AddListener(OnEndStep2);
    item.SetReady();
  }

  private void OnEndStep2()
  {
    var item = items[2];
    item.OnFinish.RemoveListener(OnEndStep2);
    item.gameObject.SetActive(false);
    character.TurnOnSlotsAttachment(slotStep2);

    DoneStep();
    TryNextStep();
  }

  #endregion Step2

  #region Step3
  private void OnStartStep3()
  {
    Debug.Log("OnStartStep3");
    var item = items[3];
    item.OnFinish.AddListener(OnEndStep3);
    item.SetReady();
  }

  private void OnEndStep3()
  {
    var item = items[3];
    item.OnFinish.RemoveListener(OnEndStep3);
    item.IsReady = false;

    DoneStep();
    TryNextStep();
  }

  #endregion Step3

  #region Step4

  [SerializeField] private List<SlotAttachmentPair> slotEyeL = new List<SlotAttachmentPair>();
  [SerializeField] private List<SlotAttachmentPair> slotEyeR = new List<SlotAttachmentPair>();

  public void AddEyeL()
  {
    character.TurnOnSlotsAttachment(slotEyeL);
  }

  public void AddEyeR()
  {
    character.TurnOnSlotsAttachment(slotEyeR);
  }

  private void OnStartStep4()
  {
    Debug.Log("OnStartStep4");
    var item = items[4];
    item.OnFinish.AddListener(CheckEndStep4);
    item.SetReady();
  }

  void CheckEndStep4()
  {
    // if ()
  }

  private void OnEndStep4()
  {
    DoneStep();
    TryNextStep();
  }

  #endregion Step4

  #region Step5

  private void OnStartStep5()
  {
    AdsManager.Ins.ShowEndGame();
  }

  private void OnEndStep5()
  {
  }
  #endregion Step5

  #region Step6
  private void OnStartStep6()
  {
    var item = items[6] as HairBang;
    item.OnFinish.AddListener(OnEndStep6);
    item.SetReady();
  }

  [SerializeField] List<SlotAttachmentPair> slotDisableHairband = new List<SlotAttachmentPair>();
  [SerializeField] List<SlotAttachmentPair> slotEnableHairband = new List<SlotAttachmentPair>();

  private void AddHairband()
  {
    SoundManager.Ins.PlayFx(FxType.Drop);
    for (int i = 0; i < slotDisableHairband.Count; i++)
    {
      character.TurnSlotAttachment(slotDisableHairband[i].slotName, null);
    }

    for (int i = 0; i < slotEnableHairband.Count; i++)
    {
      character.TurnSlotAttachment(slotEnableHairband[i].slotName, slotEnableHairband[i].attachmentName);
    }
  }

  private void OnEndStep6()
  {
    var item = items[6] as HairBang;
    item.OnFinish.RemoveListener(OnEndStep6);
    item.IsReady = false;
    item.gameObject.SetActive(false);
    AddHairband();

    DoneStep();
    TryNextStep();
  }
  #endregion Step6

  #region Step7
  private void OnStartStep7()
  {
    var item = items[7];
    item.OnFinish.AddListener(OnEndStep7);
    item.SetReady();
  }

  private void OnEndStep7()
  {

    var item = items[7];
    item.OnFinish.RemoveListener(OnEndStep7);
    item.IsReady = false;

    DoneStep();
    TryNextStep();
  }
  #endregion Step7

  #region Step8
  private void OnStartStep8()
  {
    var item = items[8];
    item.OnFinish.AddListener(OnEndStep8);
    item.SetReady();
  }

  private void OnEndStep8()
  {

    var item = items[8];
    item.OnFinish.RemoveListener(OnEndStep8);
    item.IsReady = false;

    DoneStep();
    TryNextStep();
  }
  #endregion Step8
  #region Step9

  private void OnStartStep9()
  {
    var item = items[9] as FoundationBottle;
    item.OnFinish.AddListener(OnEndStep9);
    item.SetReady();
  }

  private void OnEndStep9()
  {

    var item = items[9];
    item.OnFinish.RemoveListener(OnEndStep9);
    item.IsReady = false;

    DoneStep();
    TryNextStep();
  }
  #endregion Step9

  #region Step10
  private int countItemClickStep10 = 0;
  [SerializeField] private List<ShowSprite> showSprites = new List<ShowSprite>();
  private void OnStartStep10()
  {
    int idSpr = 0;
    for (int i = 10; i < 13; i++)
    {
      var index = idSpr;
      var item = items[i];
      item.GetComponent<BoxCollider2D>().enabled = true;
      item.OnFinish.AddListener(() => CheckDone(index));
      item.SetReady();
    }
  }

  void CheckDone(int id)
  {
    countItemClickStep10++;
    for (int i = 0; i < showSprites.Count; i++)
    {
      if (!showSprites[i].gameObject.activeSelf) continue;
      showSprites[i].ShowCustomSprite(0.4f + countItemClickStep10 * 0.2f, 0.5f);
    }

    if (showSprites.All(x => x.gameObject.activeSelf == true))
      OnEndStep10();
  }

  private void OnEndStep10()
  {

    DoneStep();
    TryNextStep();
  }
  #endregion Step10

  #region Step11
  private void OnStartStep11()
  {
    DOVirtual.DelayedCall(0.3f, () =>
    {
      AdsManager.Ins.ShowEndGame();
    });
  }
  #endregion Step11
}