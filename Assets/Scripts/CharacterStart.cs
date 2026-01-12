using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using Utilities;

public class CharacterStart : MonoBehaviour
{

  [SerializeField] private SkeletonAnimation skeletonAnimation;
  public List<SlotAttachmentPair> slotActiveStart = new List<SlotAttachmentPair>();
  public List<SlotAttachmentPair> slotDeactiveStart = new List<SlotAttachmentPair>();

  [SerializeField] private LoopAnimTransformFloating eyeFloating;

  public void CloseEye()
  {
    eyeFloating.CloseEye();
    // TurnSlotAttachment("eye", "eye_closed");
  }

  public List<SlotAttachmentPair> slotActiveStep1 = new List<SlotAttachmentPair>();
  public List<SlotAttachmentPair> slotDeactiveStep1 = new List<SlotAttachmentPair>();

  public void OnStartStep1()
  {
    foreach (SlotAttachmentPair pair in slotActiveStep1)
    {
      TurnSlotAttachment(pair.slotName, pair.attachmentName);
    }

    foreach (SlotAttachmentPair pair in slotDeactiveStep1)
    {
      TurnSlotAttachment(pair.slotName, null);
    }
  }

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
