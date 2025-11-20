using System.Collections.Generic;
using HoangHH;
using Spine.Unity;
using UnityEngine;

namespace HaoNT
{
  public class NSnapPointEnableSlotVsSp : NSnapPoint
  {
    [Header("Slot Handle")]
    [SerializeField]
    private SkeletonAnimation skeletonAnimation;



    [SerializeField] private List<SlotAttachmentPair> disableSlot = new List<SlotAttachmentPair>();

    [SerializeField] private List<SlotAttachmentPair> enableSlot = new List<SlotAttachmentPair>();

    private void Awake()
    {
      // Update data asset references in edit mode so Odin can resolve dropdowns
      SetDataAsset();
    }

    public override void OnSnap(NSnapObject snap)
    {
      base.OnSnap(snap);

      foreach (SlotAttachmentPair pair in disableSlot)
      {
        skeletonAnimation.Skeleton.SetAttachment(pair.slotName, null);
      }

      foreach (SlotAttachmentPair pair in enableSlot)
      {
        if (string.IsNullOrEmpty(pair.attachmentName)) pair.attachmentName = null;
        skeletonAnimation.Skeleton.SetAttachment(pair.slotName, pair.attachmentName);
      }
    }

    private void SetDataAsset()
    {
      SkeletonDataAsset dataAsset = skeletonAnimation?.SkeletonDataAsset;
      foreach (SlotAttachmentPair pair in disableSlot)
      {
        pair.skeletonDataAsset = dataAsset;
      }

      foreach (SlotAttachmentPair pair in enableSlot)
      {
        pair.skeletonDataAsset = dataAsset;
      }
    }
  }
}