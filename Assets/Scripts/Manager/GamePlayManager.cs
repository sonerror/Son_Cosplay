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

  public int currentStep = 0;

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
    OnStartStep1();
  }

  void DoneStep()
  {
    GameManager.Ins.emojiControl.ShowPositive();
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
    headBandObject.OnReady();
    headBandObject.OnFinish.AddListener(OnEndStep1);
  }

  private void OnEndStep1()
  {
    headBandObject.gameObject.SetActive(false);

    TurnCharacterSlotAttachment(headBandSlotDataList, true);
    TurnCharacterSlotAttachment(frontHairSlotDataList, false);

    headBandObject.OnFinish.RemoveListener(OnEndStep1);

    DoneStep();
    TryNextStep();
  }

  #endregion Step1
}
