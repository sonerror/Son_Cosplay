using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class ToggleBoneSlot : MonoBehaviour
{
  [SerializeField] private SkeletonAnimation skeletonAnimation;

  [SerializeField]
  private List<SlotAttachmentPair> initialAttachments = new();

  public SkeletonAnimation SkeletonAnimation => skeletonAnimation;

  private void Start()
  {
    // Assign the data asset to each element so Odin dropdowns can resolve properly
    SkeletonDataAsset dataAsset = skeletonAnimation?.SkeletonDataAsset;
    foreach (SlotAttachmentPair pair in initialAttachments)
    {
      pair.skeletonDataAsset = dataAsset;
      TurnSlotAttachment(pair.slotName, pair.attachmentName);
    }
  }

  /// <summary>
  ///     Sets the attachment on a specified slot. If attachmentName is null, it detaches the attachment.
  /// </summary>
  public void TurnSlotAttachment(string slotName, string attachmentName = null)
  {
    try
    {
      if (string.IsNullOrEmpty(attachmentName)) attachmentName = null;
      skeletonAnimation.Skeleton.SetAttachment(slotName, attachmentName);
    }
    catch (Exception e)
    {
      Debug.LogError($"Error setting attachment '{attachmentName}' on slot '{slotName}': {e.Message}");
    }
  }

#if UNITY_EDITOR
  private void OnValidate()
  {
    // Update data asset references in edit mode so Odin can resolve dropdowns
    SkeletonDataAsset dataAsset = skeletonAnimation?.SkeletonDataAsset;
    foreach (var pair in initialAttachments)
    {
      pair.skeletonDataAsset = dataAsset;
      TurnSlotAttachment(pair.slotName, pair.attachmentName);
    }
  }

#endif
}
