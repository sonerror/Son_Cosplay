using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class Character : GameUnit
{
  [SerializeField] protected SkeletonAnimation skeletonAnimation;
  public SkeletonAnimation SkeletonAnimation => skeletonAnimation;

  public void TurnSlotAttachment(string slotName, string attachmentName = null)
  {
    skeletonAnimation.Skeleton.SetAttachment(slotName, attachmentName);
  }

  public void TurnOnSlotsAttachment(List<SlotAttachmentPair> slotAttachmentPairs)
  {
    foreach (SlotAttachmentPair pair in slotAttachmentPairs)
    {
      TurnSlotAttachment(pair.slotName, pair.attachmentName);
    }
  }

  public void TurnOffSlotsAttachment(List<SlotAttachmentPair> slotAttachmentPairs)
  {
    foreach (SlotAttachmentPair pair in slotAttachmentPairs)
    {
      TurnSlotAttachment(pair.slotName, null);
    }
  }

  public void SetAlphaSlotName(string slotName, float alphaSlotName)
  {
    var slot = skeletonAnimation.Skeleton.FindSlot(slotName);
    if (slot != null)
    {
      slot.A = alphaSlotName;
    }
    else
    {
      Debug.LogWarning($"Slot {slotName} not found in skeleton.");
    }
  }
}
