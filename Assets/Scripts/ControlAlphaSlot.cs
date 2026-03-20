using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

public class ControlAlphaSlot : MonoBehaviour
{
  public Character character;
  [SerializeField] private float speedLerp = 2f;
  public List<SlotAttachmentPair> slotDataList = new List<SlotAttachmentPair>();
  public UnityEvent OnFinish;

  private List<Slot> slotsData = new List<Slot>();
  [SerializeField] private List<TriggerWithCertainCollider> trigerColi = new List<TriggerWithCertainCollider>();

  private int currCountCollider = 0;
  private float targetAlpha = 0f;
  private float currAlpha = 0f;
  private bool isFinished = false;

  [Button]
  public void SetReady(Character charRef)
  {
    this.character = charRef;
    slotsData.Clear();

    foreach (var slotData in slotDataList)
    {
      character.TurnSlotAttachment(slotData.slotName, slotData.attachmentName);
      var slot = character.SkeletonAnimation.Skeleton.FindSlot(slotData.slotName);
      if (slot != null)
      {
        slotsData.Add(slot);
        slot.A = 0f;
      }
    }

    for (int i = 0; i < trigerColi.Count; i++)
    {
      var trigger = trigerColi[i];
      trigger.OnTriggerEvent.RemoveAllListeners();
      trigger.OnTriggerEvent.AddListener(() => OnColli(trigger));
    }

    character.SkeletonAnimation.UpdateLocal -= UpdateSpineAlpha;
    character.SkeletonAnimation.UpdateLocal += UpdateSpineAlpha;

    currCountCollider = 0;
    currAlpha = 0f;
    targetAlpha = 0f;
    isFinished = false;
  }

  private void OnColli(TriggerWithCertainCollider obj)
  {
    obj.gameObject.SetActive(false);
    InCreaseAlpha();
  }

  public void InCreaseAlpha()
  {
    currCountCollider++;

    if (trigerColi.Count == 0) return;
    targetAlpha = (float)currCountCollider / trigerColi.Count;

    if (targetAlpha > 1f) targetAlpha = 1f;
  }

  void Update()
  {
    if (isFinished || currAlpha >= targetAlpha) return;

    currAlpha = Mathf.MoveTowards(currAlpha, targetAlpha, Time.deltaTime * speedLerp);

    if (currAlpha >= 1f && !isFinished)
    {
      currAlpha = 1f;
      isFinished = true;
      OnFinish?.Invoke();

      if (character != null && character.SkeletonAnimation != null)
        character.SkeletonAnimation.UpdateLocal -= UpdateSpineAlpha;

      gameObject.SetActive(false);
    }
  }

  private void UpdateSpineAlpha(ISkeletonAnimation animatedObject)
  {
    for (int i = 0; i < slotsData.Count; i++)
    {
      slotsData[i].A = currAlpha;
    }
  }

  [Button]
  void GetTrigerColli()
  {
    trigerColi = new List<TriggerWithCertainCollider>(GetComponentsInChildren<TriggerWithCertainCollider>(true));
  }

  private void OnDestroy()
  {
    if (character != null && character.SkeletonAnimation != null)
      character.SkeletonAnimation.UpdateLocal -= UpdateSpineAlpha;
  }
}