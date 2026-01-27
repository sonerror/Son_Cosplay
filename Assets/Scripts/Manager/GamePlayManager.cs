using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GamePlayManager : Singleton<GamePlayManager>
{
  public List<ControlAlphaSlot> ControlAlphaSlot = new List<ControlAlphaSlot>();
  public List<RemoveItem> ClotherRemove = new List<RemoveItem>();
  public CharacterControl character;
  public List<Item> items = new List<Item>();

  [SerializeField]
  protected int currentStep = 0;
  public int CurrentStep => currentStep;

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

  protected virtual void DoneStep()
  {
    GameManager.Ins.PlayPositiveEmoji();
    Debug.Log("Fx step" + currentStep);
  }

  protected virtual void TryNextStep()
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
  }
}
