using System;
using System.Collections.Generic;
using AnhPD.Cook;
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
    // character.SetAlphaSlotName("TOP-mouth-1", 0);
    // character.SetAlphaSlotName("TOP-mouthbase-1", 0);
    StartStep();
  }

  public void TurnOnMouthNew()
  {
    // character.SetAlphaSlotName("TOP-mouth-1", 1);
    // var slot = character.SkeletonAnimation.Skeleton.FindSlot("TOP-mouth-2");
    // DOVirtual.Float(0f, 1f, 1f, (alpha) =>
    // {
    //   slot.A = alpha;
    // });

    character.SetMouseDone();
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
  private const string STEP_0 = "Step 0: Cut hair";
  private const string SLOT_CUT_HAIR_0 = "Phase_1_hairF_cut_outL1";
  private const string SLOT_CUT_HAIR_1 = "Phase_1_hairF_cut_outL2";
  private const string SLOT_CUT_HAIR_2 = "Phase_1_hairF_cut_outL3";
  private const string SLOT_CUT_HAIR_3 = "Phase_1_hairB_cut_outL1";
  private const string SLOT_CUT_HAIR_4 = "Phase_1_hairB_cut_outL2";

  private const string SLOT_CUT_HAIR_5 = "Phase_1_hairF_cut_outR1";
  private const string SLOT_CUT_HAIR_6 = "Phase_1_hairF_cut_outR2";
  private const string SLOT_CUT_HAIR_7 = "Phase_1_hairF_cut_outR3";
  private const string SLOT_CUT_HAIR_8 = "Phase_1_hairB_cut_outR1";
  private const string SLOT_CUT_HAIR_9 = "Phase_1_hairB_cut_outR2";

  [FoldoutGroup(STEP_0)]
  [SerializeField]
  private Scissors scissor;

  [FoldoutGroup(STEP_0)]
  [SerializeField]
  private Collider2D scissorTrigger;

  [FoldoutGroup(STEP_0)]
  [SerializeField]
  private List<TriggerWithCertainCollider> hairCutTriggerL, hairCutTriggerR;

  [FoldoutGroup(STEP_0)]
  [SerializeField]
  private List<SpineBoneIKControl> hairCutBoneControlL, hairCutBoneControlR;

  [FoldoutGroup(STEP_0)]
  [SerializeField]
  private List<AutoDestroy> hairCutAutoDestroyL, hairCutAutoDestroyR;

  [FoldoutGroup(STEP_0)]
  [SerializeField]
  private List<OnTransformGoToAffectZone> hairCutTriggers;

  [FoldoutGroup(STEP_0)]
  [SerializeField]
  private ParticleSystem hairParticle;

  [FoldoutGroup(STEP_0)]
  [SerializeField]
  private List<SlotAttachmentPair> hairCutL, hairCutR, hairCutOldL, hairCutOldR, hairCutNewL, hairCutNewR;

  private readonly List<string> _cutHairSlotNameL = new()
        {
            SLOT_CUT_HAIR_0,
            SLOT_CUT_HAIR_1,
            SLOT_CUT_HAIR_2,
            SLOT_CUT_HAIR_3,
            SLOT_CUT_HAIR_4,
        };

  private readonly List<string> _cutHairSlotNameR = new()
        {
            SLOT_CUT_HAIR_5,
            SLOT_CUT_HAIR_6,
            SLOT_CUT_HAIR_7,
            SLOT_CUT_HAIR_8,
            SLOT_CUT_HAIR_9
        };

  private float maxCountHairL, maxCountHairR;

  private void OnStartStep0()
  {
    Debug.Log("OnStartStep0");
    scissor.OnPickItem.AddListener(EnableScissorTrigger);
    scissor.OnDropItem.AddListener(DisableScissorTrigger);
    foreach (OnTransformGoToAffectZone trigger in hairCutTriggers)
    {
      trigger.onEnterZone.AddListener(EnableCutTrigger);
      trigger.onOutZone.AddListener(DisableCutTrigger);
    }

    maxCountHairL = hairCutTriggerL.Count;
    maxCountHairR = hairCutTriggerR.Count;
    SetUpCutHairTrigger(hairCutTriggerL, hairCutBoneControlL, hairCutAutoDestroyL, _cutHairSlotNameL, hairCutL,
        maxCountHairL, hairCutOldL, hairCutNewL
    );
    SetUpCutHairTrigger(hairCutTriggerR, hairCutBoneControlR, hairCutAutoDestroyR, _cutHairSlotNameR, hairCutR,
        maxCountHairR, hairCutOldR, hairCutNewR
    );
  }

  private void SetUpCutHairTrigger(List<TriggerWithCertainCollider> triggers,
      List<SpineBoneIKControl> boneControls, List<AutoDestroy> autoDestroys, List<string> _cutHairSlotName,
      List<SlotAttachmentPair> hairCut
      , float maxCount,
      List<SlotAttachmentPair> oldSlot, List<SlotAttachmentPair> newSlot
  )
  {
    for (int i = 0; i < triggers.Count; i++)
    {
      int index = i;
      TriggerWithCertainCollider cutTrigger = triggers[index];
      cutTrigger.gameObject.SetActive(true);
      cutTrigger.EnableCol(true);
      cutTrigger.enabled = true;
      cutTrigger.OnTriggerEvent.AddListener(() =>
          CutHair(triggers, boneControls, autoDestroys, cutTrigger, maxCount
              , oldSlot, newSlot, _cutHairSlotName, hairCut,
              index));
    }
  }

  private void CutHair(List<TriggerWithCertainCollider> triggers, List<SpineBoneIKControl> boneControls,
      List<AutoDestroy> autoDestroys, TriggerWithCertainCollider cutTrigger, float maxCount,
      List<SlotAttachmentPair> oldSlot, List<SlotAttachmentPair> newSlot,
      List<string> _cutHairSlotName, List<SlotAttachmentPair> hairCut,
      int index)
  {
    if (triggers.Remove(cutTrigger))
    {
      // DOVirtual.Float(0, 1 - triggers.Count / maxCount, 0.3f, value =>
      // {
      // oldSlot.SetSlotAlpha(1 - value / 3);
      foreach (var slot in oldSlot)
      {
        character.SetAlphaSlotName(slot.slotName, 0);
        character.TurnSlotAttachment(slot.slotName);
      }
      // newSlot.SetSlotAlpha(value);
      foreach (var slot in newSlot)
      {
        character.SetAlphaSlotName(slot.slotName, 1);
        character.TurnSlotAttachment(slot.slotName, slot.attachmentName);
      }

      // }).SetEase(Ease.OutBack);
      cutTrigger.Rigidbody.simulated = true;
      cutTrigger.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
      boneControls[index].enabled = true;
      autoDestroys[index].enabled = true;
      autoDestroys[index].onDestroy += () =>
      {
        character.TurnSlotAttachment(_cutHairSlotName[index]);
        hairCut = hairCut.FindAll(x => x.slotName != _cutHairSlotName[index]);
      };
      if (triggers.Count == 0)
      {
        // oldSlot.FadeOut(0.3f, true);
        OnEndStep0();
      }
    }
  }
  private void EnableScissorTrigger()
  {
    foreach (OnTransformGoToAffectZone trigger in hairCutTriggers)
    {
      trigger.enabled = true;
    }
  }

  private void DisableScissorTrigger()
  {
    foreach (OnTransformGoToAffectZone trigger in hairCutTriggers)
    {
      trigger.enabled = false;
    }
  }

  private void EnableCutTrigger()
  {
    hairParticle.Play();
    scissorTrigger.enabled = true;
  }

  private void DisableCutTrigger()
  {
    hairParticle.Stop();
    scissorTrigger.enabled = false;
  }


  private void OnEndStep0()
  {
    if (hairCutTriggerL.Count > 0 || hairCutTriggerR.Count > 0) return;
    DisableScissorTrigger();
    scissor.OnPickItem.RemoveListener(EnableScissorTrigger);
    scissor.OnDropItem.RemoveListener(DisableScissorTrigger);
    foreach (OnTransformGoToAffectZone trigger in hairCutTriggers)
    {
      trigger.onEnterZone.RemoveListener(EnableCutTrigger);
      trigger.onOutZone.RemoveListener(DisableCutTrigger);
    }

    DoneStep();
    TryNextStep();
  }

  // private void OnStartStep0()
  // {
  //   var GlassesObject = items[0] as RemoveItem;
  //   GlassesObject.SetReady();
  //   GlassesObject.SetBlockItem(false);
  //   GlassesObject.OnFinish.AddListener(OnEndStep0);
  // }

  // private void OnEndStep0()
  // {
  //   var GlassesObject = items[0] as RemoveItem;
  //   GlassesObject.OnFinish.RemoveListener(OnEndStep0);
  //   DoneStep();
  //   TryNextStep();
  // }
  #endregion Step0

  #region Step1
  [SerializeField]


  private void OnStartStep1()
  {
    // var LidCreamBox = items[1] as TapJumpOpener;
    // LidCreamBox.SetReady();
    // LidCreamBox.OnFinish.AddListener(OnEndStep1);
  }

  private void OnEndStep1()
  {
    var LidCreamBox = items[1] as TapJumpOpener;
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
    var item = items[2] as FoundationBottle;
    item.OnFinish.AddListener(OnEndStep2);
    item.SetReady();
  }

  private void OnEndStep2()
  {
    var item = items[2] as FoundationBottle;
    item.OnFinish.RemoveListener(OnEndStep2);
    // item.IsReady = false;

    DoneStep();
    TryNextStep();
  }

  #endregion Step2

  #region Step3

  private void OnStartStep3()
  {
    Debug.Log("OnStartStep3");
    var item = items[3] as TapButton;
    item.OnFinish.AddListener(OnEndStep3);
    item.SetReady();
  }

  private void OnEndStep3()
  {
    var item = items[3] as TapButton;
    item.OnFinish.RemoveListener(OnEndStep3);
    item.IsReady = false;
    DoneStep();
    TryNextStep();
  }

  #endregion Step3

  #region Step4

  private void OnStartStep4()
  {
    Debug.Log("OnStartStep4");
    var item = items[4] as FoundationBottle;
    item.OnFinish.AddListener(OnEndStep4);
    item.SetReady();
  }

  private void OnEndStep4()
  {

    var item = items[4] as FoundationBottle;
    item.OnFinish.RemoveListener(OnEndStep4);
    item.IsReady = false;

    DoneStep();
    TryNextStep();
  }

  #endregion Step4

  #region Step5

  public SpriteRenderer spriteStep5;
  private void OnStartStep5()
  {
    var item = items[5] as BrushBlushWithMaskGroup;
    item.OnFinish.AddListener(OnEndStep5);
    item.SetReady();
  }

  private void OnEndStep5()
  {
    spriteStep5.maskInteraction = SpriteMaskInteraction.None;
    var item = items[5] as BrushBlushWithMaskGroup;
    item.FlourFill.gameObject.SetActive(false);
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
    var item = items[6] as FaceTowel;
    item.OnFinish.AddListener(OnEndStep6);
    item.SetReady();
  }

  private void OnEndStep6()
  {

    var item = items[6] as FaceTowel;
    item.OnFinish.RemoveListener(OnEndStep6);
    item.IsReady = false;

    DoneStep();
    TryNextStep();
  }
  #endregion Step6

  #region Step7
  private void OnStartStep7()
  {
    var item = items[7] as TapOpenerSingleUse;
    item.OnFinish.AddListener(OnEndStep7);
    item.SetReady();
  }

  private void OnEndStep7()
  {

    var item = items[7] as TapOpenerSingleUse;
    item.OnFinish.RemoveListener(OnEndStep7);
    item.IsReady = false;

    // DoneStep();
    TryNextStep();
  }
  #endregion Step7

  #region Step8
  private void OnStartStep8()
  {
    var item = items[8] as FoundationBottle;
    item.OnFinish.AddListener(OnEndStep8);
    item.SetReady();
  }

  private void OnEndStep8()
  {

    var item = items[8] as FoundationBottle;
    item.OnFinish.RemoveListener(OnEndStep8);
    item.IsReady = false;

    DoneStep();
    TryNextStep();
  }
  #endregion Step8
  #region Step9

  private void AddHairband()
  {
    Debug.Log("AddHairband");
    SoundManager.Ins.PlayFx(FxType.Drop);
    character.TurnSlotAttachment("TOP-hair-baseB");
    character.TurnSlotAttachment("TOP-hair-baseF");
    character.TurnSlotAttachment("TOP-hair-net", "phase-outfit-solo/TOP-hair-net");
    character.TurnSlotAttachment("TOP-hair-netcap", "phase-outfit-solo/TOP-hair-netcap");

  }

  private void OnStartStep9()
  {
    var item = items[9];
    item.OnFinish.AddListener(OnEndStep9);
    item.OnFinish.AddListener(AddHairband);

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

  private String[] step10SlotNames = new string[]
  {
    "TOP-brow-R12",
    "TOP-brow-R11",
    "SANTA-beard3",
    "SANTA-beardb3",
    "SANTA-hairB3",
    "SANTA-hairF3",
  };

  private String[] step10AttachmentNames = new string[]
  {
    "phase-Santa/SANTA-brow",
    "phase-Santa/SANTA-brow",
    "phase-Santa/SANTA-beard",
    "phase-Santa/SANTA-beardb",
    "phase-Santa/SANTA-hairB",
    "phase-Santa/SANTA-hairF",
  };

  private void OnStartStep10()
  {
    var indexCount = 0;
    for (int i = 10; i < 16; i++)
    {
      var item = items[i];
      int indexSlot = indexCount; // Capture the index for the lambda
      indexCount++;
      int index = i;
      item.OnFinish.AddListener(() => CheckDone(indexSlot, index));
      item.SetReady();
    }
  }

  void CheckDone(int indexSlot, int index)
  {
    Debug.Log("CheckDone Step10 " + indexSlot);
    SoundManager.Ins.PlayFx(FxType.Drop);
    character.TurnSlotAttachment(step10SlotNames[indexSlot], step10AttachmentNames[indexSlot]);
    items[index].OnFinish.RemoveAllListeners();
    items[index].IsReady = false;
    items[index].gameObject.SetActive(false);
    for (int i = 10; i < 16; i++)
    {
      if (items[i].IsReady) return;
    }
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
    AdsManager.Ins.ShowEndGame();
    GameManager.Ins.ChangeToScene2();
  }

  #endregion Step11


}