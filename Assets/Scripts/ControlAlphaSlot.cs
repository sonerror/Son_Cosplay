using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine;
using UnityEngine;
using UnityEngine.Events;

public class ControlAlphaSlot : MonoBehaviour
{
  public Character character;
  [SerializeField] private float speedLerp = 0.5f;
  public List<SlotAttachmentPair> slotDataList = new List<SlotAttachmentPair>();
  public UnityEvent OnFinish;

  private List<Slot> slotsData = new List<Slot>();
  [SerializeField] private List<TriggerWithCertainCollider> trigerColi = new List<TriggerWithCertainCollider>();

  [Button]
  public void SetReady(Character character)
  {
    // this.character = character;
    foreach (var slotData in slotDataList)
    {
      character.TurnSlotAttachment(slotData.slotName, slotData.attachmentName);
      var slot = character.SkeletonAnimation.Skeleton.FindSlot(slotData.slotName);
      slotsData.Add(slot);
      slot.A = 0f;
    }

    // for (int i = 0; i < trigerColi.Count; i++)
    // {
    //   var trigger = trigerColi[i];
    //   trigger.OnTriggerEvent.AddListener(() => OnColli(trigger));
    // }
  }

  private void OnColli(TriggerWithCertainCollider obj)
  {
    obj.gameObject.SetActive(false);
    InCreaseAlpha();
  }


  private int currCountCollider = 0;
  private float targetAlpha = 0f;
  private float currAlpha = 0f;

  public void InCreaseAlpha()
  {
    currCountCollider++;
    if (currCountCollider >= trigerColi.Count) return;
    targetAlpha = (float)currCountCollider / (trigerColi.Count - 1);
    if (targetAlpha > 1f) targetAlpha = 1f;
  }

  void FixedUpdate()
  {
    if (currAlpha >= targetAlpha) return;
    currAlpha += Time.fixedDeltaTime * speedLerp;
    if (currAlpha > 1f)
    {
      currAlpha = 1f;
      OnFinish?.Invoke();
      gameObject.SetActive(false);
    }
    foreach (var slot in slotsData)
    {
      slot.A = currAlpha;
    }
  }

  [Button]
  void GetTrigerColli()
  {
    trigerColi = new List<TriggerWithCertainCollider>(GetComponentsInChildren<TriggerWithCertainCollider>());
  }
}
