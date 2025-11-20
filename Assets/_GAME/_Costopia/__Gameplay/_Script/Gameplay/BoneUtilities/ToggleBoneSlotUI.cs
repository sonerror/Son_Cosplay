using System;
using System.Collections.Generic;
using HoangHH;
using Spine;
using Spine.Unity;
using UnityEngine;

public class ToggleBoneSlotUI : MonoBehaviour
{
  [SerializeField] private SkeletonGraphic skeletonGraphic;

  [Header("Initial Slot Attachments")]
  [SerializeField]
  private List<SlotAttachmentPair> initialAttachments = new List<SlotAttachmentPair>();

  private void Start()
  {
    // Assign the data asset to each element so Odin dropdowns can resolve properly
    SkeletonDataAsset dataAsset = skeletonGraphic?.SkeletonDataAsset;
    foreach (SlotAttachmentPair pair in initialAttachments)
    {
      pair.skeletonDataAsset = dataAsset;
      TurnSlotAttachment(pair.slotName, pair.attachmentName);
    }
  }

#if UNITY_EDITOR
  private void OnValidate()
  {
    // Update data asset references in edit mode so Odin can resolve dropdowns
    SkeletonDataAsset dataAsset = skeletonGraphic?.SkeletonDataAsset;
    foreach (SlotAttachmentPair pair in initialAttachments)
    {
      pair.skeletonDataAsset = dataAsset;
      TurnSlotAttachment(pair.slotName, pair.attachmentName);
    }
  }


  private void ValidateInitialAttachments()
  {
    if (skeletonGraphic == null || skeletonGraphic.Skeleton == null)
    {
      Debug.LogWarning("SkeletonGraphic or its Skeleton is not assigned.");
      return;
    }

    Skeleton skeleton = skeletonGraphic.Skeleton;

    for (int i = initialAttachments.Count - 1; i >= 0; i--)
    {
      var pair = initialAttachments[i];
      if (string.IsNullOrEmpty(pair.slotName))
      {
        Debug.LogWarning($"Empty slot name found at index {i}, removing entry.");
        initialAttachments.RemoveAt(i);
        continue;
      }

      // Check if the slot exists in the skeleton
      Slot slot = skeleton.FindSlot(pair.slotName);
      if (slot == null)
      {
        Debug.LogWarning($"Slot '{pair.slotName}' not found in skeleton, removing entry.");
        initialAttachments.RemoveAt(i);
      }
    }

    UnityEditor.EditorUtility.SetDirty(this);
  }

#endif

  /// <summary>
  ///     Sets the attachment on a specified slot. If attachmentName is null, it detaches the attachment.
  /// </summary>
  public void TurnSlotAttachment(string slotName, string attachmentName = null)
  {
    try
    {
      if (string.IsNullOrEmpty(attachmentName)) attachmentName = null;
      skeletonGraphic.Skeleton.SetAttachment(slotName, attachmentName);
    }
    catch (Exception e)
    {
      // H3Log.LogWarning($"Error setting attachment '{attachmentName}' on slot '{slotName}': {e.Message}");
    }
  }
}