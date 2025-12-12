using System.Collections.Generic;
using DG.Tweening;
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
      default:
        break;
    }
  }

  #region Step0
  private const string SLOT_GLASS = "Set_Base_glass";
  private const string ATTACHMENT_GLASS = "Set_Base/glass";

  [SerializeField] private GameObject NodeOpenGlassBox, NodeCloseGlassBox;

  public void HideGlasses()
  {
    character.TurnSlotAttachment(SLOT_GLASS);
  }

  public void ShowGlasses()
  {
    character.TurnSlotAttachment(SLOT_GLASS, ATTACHMENT_GLASS);
  }

  private void OnStartStep0()
  {
    var GlassesObject = items[0] as HairBang;
    GlassesObject.SetReady();
    GlassesObject.SetBlockItem(false);
    GlassesObject.OnPickItem.AddListener(HideGlasses);
    GlassesObject.OnDropItem.AddListener(ShowGlasses);
    GlassesObject.OnFinish.AddListener(OnEndStep0);
  }

  private void OnEndStep0()
  {
    this.WaitToDo(() =>
    {
      CloseGlassBox();
    }, 0.5f);
    var GlassesObject = items[0] as HairBang;
    GlassesObject.OnFinish.RemoveListener(OnEndStep0);
    GlassesObject.OnPickItem.RemoveListener(HideGlasses);
    GlassesObject.OnDropItem.RemoveListener(ShowGlasses);
    DoneStep();
    TryNextStep();
  }

  void CloseGlassBox()
  {
    NodeOpenGlassBox.SetActive(false);
    NodeCloseGlassBox.SetActive(true);
    items[0].gameObject.SetActive(false);
  }
  #endregion Step0

  #region Step1
  [SerializeField]


  private void OnStartStep1()
  {
    var LidCreamBox = items[1] as TapOpener;
    LidCreamBox.SetReady();
    LidCreamBox.OnFinish.AddListener(OnEndStep1);
  }

  private void OnEndStep1()
  {
    var LidCreamBox = items[1] as TapOpener;
    LidCreamBox.OnFinish.RemoveListener(OnEndStep1);
    LidCreamBox.SetBlockItem(true);
    DoneStep();
    TryNextStep();
  }

  #endregion Step1

  #region Step2

  [SerializeField]

  private void OnStartStep2()
  {
    var item = items[2] as BrushBlushWithMaskGroup;
    item.OnFinish.AddListener(OnEndStep2);
    item.SetReady();
  }

  private void OnEndStep2()
  {
    var item = items[2] as BrushBlushWithMaskGroup;
    item.OnFinish.RemoveListener(OnEndStep2);
    item.IsReady = false;

    DoneStep();
    TryNextStep();
  }

  #endregion Step2

  #region Step3

  private void OnStartStep3()
  {
    var item = items[3] as FaceTowelWithVfx;
    item.OnFinish.AddListener(OnEndStep3);
    item.maskGroup.SetTypeActive(false);
    item.maskGroup.ResetMask();
    item.SetReady();
  }

  private void OnEndStep3()
  {
    var item = items[3] as FaceTowelWithVfx;
    item.OnFinish.RemoveListener(OnEndStep3);
    item.IsReady = false;
    DoneStep();
    TryNextStep();
  }

  #endregion Step3

  #region Step4

  private void OnStartStep4()
  {
    var item = items[4] as FaceTowelWithVfx;
    item.OnFinish.AddListener(OnEndStep4);
    item.SetReady();
  }

  private void OnEndStep4()
  {

    var item = items[4] as FaceTowelWithVfx;
    item.OnFinish.RemoveListener(OnEndStep4);
    item.IsReady = false;

    DoneStep();
    TryNextStep();
  }

  #endregion Step4

  #region Step5

  private void OnStartStep5()
  {
    AdsManager.Ins.ShowEndGame();
  }

  // private void OnEndStep5()
  // {
  //   var item = items[5] as ItemMove;
  //   item.OnFinish.RemoveListener(OnEndStep5);
  //   item.IsReady = false;
  //   ChangeGroupItemStart();
  //   DoneStep();
  //   TryNextStep();
  // }

  #endregion Step5

  // #region Step6

  // private void OnStartStep6()
  // {
  //   var item = items[6] as FoundationBottleWithFx;
  //   item.OnFinish.AddListener(OnEndStep6);
  //   item.SetReady();
  // }

  // private void OnEndStep6()
  // {
  //   var item = items[6] as FoundationBottleWithFx;
  //   item.OnFinish.RemoveListener(OnEndStep6);
  //   item.IsReady = false;

  //   DoneStep();
  //   TryNextStep();
  // }

  // #endregion Step6
  // #region Step7

  // private void OnStartStep7()
  // {
  //   var item = items[7] as FoundationBottleWithFx;
  //   item.OnFinish.AddListener(OnEndStep7);
  //   item.SetReady();
  // }

  // private void OnEndStep7()
  // {
  //   var item = items[7] as FoundationBottleWithFx;
  //   item.OnFinish.RemoveListener(OnEndStep7);
  //   item.IsReady = false;

  //   DoneStep();
  //   TryNextStep();
  // }
  // #endregion Step7

  // #region  Step8
  // private void OnStartStep8()
  // {
  //   AdsManager.Ins.ShowEndGame();
  // }
  // #endregion Step8
}