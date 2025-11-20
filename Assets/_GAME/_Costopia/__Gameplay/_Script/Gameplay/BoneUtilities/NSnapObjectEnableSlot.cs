using System.Collections.Generic;
using Costopia.Level;
using Spine.Unity;
using UnityEngine;

namespace HoangHH
{
  public class NSnapObjectEnableSlot : NSnapObject
  {
    [Header("Slot Handle")]
    [SerializeField]
    private SkeletonAnimation skeletonAnimation;

    [SerializeField] private List<SlotAttachmentPair> disableSlot = new List<SlotAttachmentPair>();

    [SerializeField] private List<SlotAttachmentPair> enableSlot = new List<SlotAttachmentPair>();

    private CosplayStepLevel LevelCosplay => levelBase as CosplayStepLevel;

    protected override void OnStart()
    {
      // Update data asset references in edit mode so Odin can resolve dropdowns
      SkeletonDataAsset dataAsset = skeletonAnimation?.SkeletonDataAsset;
      foreach (var pair in disableSlot)
      {
        pair.skeletonDataAsset = dataAsset;
      }
      foreach (var pair in enableSlot)
      {
        pair.skeletonDataAsset = dataAsset;
      }
    }

    protected override void OnSnap()
    {
      base.OnSnap();
      foreach (SlotAttachmentPair pair in disableSlot)
      {
        skeletonAnimation.Skeleton.SetAttachment(pair.slotName, null);
      }
      foreach (SlotAttachmentPair pair in enableSlot)
      {
        if (string.IsNullOrEmpty(pair.attachmentName)) pair.attachmentName = null;
        skeletonAnimation.Skeleton.SetAttachment(pair.slotName, pair.attachmentName);
      }
      gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
      // Update data asset references in edit mode so Odin can resolve dropdowns
      SkeletonDataAsset dataAsset = skeletonAnimation?.SkeletonDataAsset;
      foreach (var pair in disableSlot)
      {
        pair.skeletonDataAsset = dataAsset;
      }
      foreach (var pair in enableSlot)
      {
        pair.skeletonDataAsset = dataAsset;
      }
    }
#endif
  }
}