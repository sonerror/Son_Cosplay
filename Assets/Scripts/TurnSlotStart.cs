using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class TurnSlotStart : MonoBehaviour
{

  [SerializeField] private SkeletonAnimation skeletonAnimation;
  public SkeletonAnimation SkeletonAnimation => skeletonAnimation;
  public List<SlotAttachmentPair> slotActiveStart = new List<SlotAttachmentPair>();
  public List<SlotAttachmentPair> slotDeactiveStart = new List<SlotAttachmentPair>();

  void Start()
  {
    foreach (SlotAttachmentPair pair in slotActiveStart)
    {
      TurnSlotAttachment(pair.slotName, pair.attachmentName);
    }

    foreach (SlotAttachmentPair pair in slotDeactiveStart)
    {
      TurnSlotAttachment(pair.slotName, null);
    }
  }

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
}
