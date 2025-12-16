using System.Collections.Generic;
using HoangHH;
using Sirenix.OdinInspector;
using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
  public CharacterControl character;
  public ShowObjectEffect Gr1, Gr2;

  public List<Item> items = new List<Item>();

  public void ChangeGroupItemStart()
  {
    Debug.Log("ChangeGroupItemStart ");
    Gr1.Hide(0.5f);
    Gr2.Show(1f);
  }

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

  void DoneStep()
  {
    GameManager.Ins.PlayPositiveEmoji();
  }

  void TryNextStep()
  {
    currentStep++;
    StartStep();
  }

#if UNITY_EDITOR
  [Button]
#endif
  void StartStep()
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
      default:
        break;
    }
  }

  #region Step0
  [SerializeField] private RemoveItem headscarfToRemove;
  [SerializeField] private List<SlotAttachmentPair> headscarfDisableSlotDataList;
  [SerializeField] private List<SlotAttachmentPair> headscarfEnableSlotDataList;

  private void OnStartStep0()
  {
    headscarfToRemove.gameObject.SetActive(true);
    headscarfToRemove.OnFinish.AddListener(OnEndStep0);
  }

  private void OnEndStep0()
  {
    headscarfToRemove.OnFinish.RemoveListener(OnEndStep0);
    OnRemoveHeadscarf();
    DoneStep();
    TryNextStep();
  }

  private void OnRemoveHeadscarf()
  {
    TurnCharacterSlotAttachment(headscarfDisableSlotDataList, false);
    TurnCharacterSlotAttachment(headscarfEnableSlotDataList, true);
  }
  #endregion Step0

  #region Step1
  [SerializeField]
  private HairBang headBandObject;
  [SerializeField]
  private List<SlotAttachmentPair> headBandSlotDataList;
  [SerializeField]
  private List<SlotAttachmentPair> frontHairSlotDataList;

  private void OnStartStep1()
  {
    headBandObject.SetReady();
    headBandObject.OnFinish.AddListener(OnEndStep1);
  }

  private void OnEndStep1()
  {
    headBandObject.gameObject.SetActive(false);

    TurnCharacterSlotAttachment(headBandSlotDataList, true);
    TurnCharacterSlotAttachment(frontHairSlotDataList, false);

    headBandObject.OnFinish.RemoveListener(OnEndStep1);
    TutorialManager.Ins.IncreaseTimeHide();
    DoneStep();
    TryNextStep();
  }

  #endregion Step1

  #region Step2

  private void OnStartStep2()
  {
    var item = items[2] as WaterFaucet;
    item.OnFinish.AddListener(OnEndStep2);
    item.SetReady();
  }

  private void OnEndStep2()
  {
    var item = items[2] as WaterFaucet;
    item.OnFinish.RemoveListener(OnEndStep2);
    item.IsReady = false;

    DoneStep();
    TryNextStep();
  }

  #endregion Step2

  #region Step3

  private void OnStartStep3()
  {
    var item = items[3] as FaceTowel;
    item.OnFinish.AddListener(OnEndStep3);
    item.SetReady();
  }

  private void OnEndStep3()
  {
    var item = items[3] as FaceTowel;
    item.OnFinish.RemoveListener(OnEndStep3);
    item.IsReady = false;
    DoneStep();
    TryNextStep();
  }

  #endregion Step3

  #region Step4

  private void OnStartStep4()
  {
    var item = items[4] as HairBang;
    item.OnFinish.AddListener(OnEndStep4);
    item.SetReady();
  }

  private void OnEndStep4()
  {

    var item = items[4] as HairBang;
    item.OnFinish.RemoveListener(OnEndStep4);
    item.IsReady = false;

    DoneStep();
    TryNextStep();
  }

  #endregion Step4

  #region Step5

  private void OnStartStep5()
  {
    var item = items[5] as ItemMove;
    item.OnFinish.AddListener(OnEndStep5);
    item.SetReady();

    GameManager.Ins.ShowClockTimer(() =>
    {
      item.SetActiveBorder();
    });
  }

  private void OnEndStep5()
  {
    var item = items[5] as ItemMove;
    item.OnFinish.RemoveListener(OnEndStep5);
    item.IsReady = false;
    ChangeGroupItemStart();
    DoneStep();
    TryNextStep();
  }

  #endregion Step5

  #region Step6

  private void OnStartStep6()
  {
    var item = items[6] as FoundationBottleWithFx;
    item.OnFinish.AddListener(OnEndStep6);
    item.SetReady();
  }

  private void OnEndStep6()
  {
    var item = items[6] as FoundationBottleWithFx;
    item.OnFinish.RemoveListener(OnEndStep6);
    item.IsReady = false;

    DoneStep();
    TryNextStep();
  }

  #endregion Step6
  #region Step7

  private void OnStartStep7()
  {
    var item = items[7] as FoundationBottleWithFx;
    item.OnFinish.AddListener(OnEndStep7);
    item.SetReady();
  }

  private void OnEndStep7()
  {
    var item = items[7] as FoundationBottleWithFx;
    item.OnFinish.RemoveListener(OnEndStep7);
    item.IsReady = false;

    DoneStep();
    TryNextStep();
  }
  #endregion Step7

  #region  Step8
  private void OnStartStep8()
  {
    AdsManager.Ins.ShowEndGame();
  }
  #endregion Step8
}
