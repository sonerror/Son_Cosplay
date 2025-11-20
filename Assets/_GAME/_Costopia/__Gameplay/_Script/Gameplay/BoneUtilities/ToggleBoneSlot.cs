using System;
using System.Collections.Generic;
using HoangHH;

using Spine;
using Spine.Unity;
using UnityEngine;

[Serializable]
public class SlotAttachmentPairList
{
  public SkeletonAnimation skeletonAnimation;

  public List<SlotAttachmentPair> pairs = new List<SlotAttachmentPair>();

  // Ensure all pairs use the same SkeletonDataAsset
  private void UpdatePairsSkeletonAsset()
  {
    foreach (var pair in pairs)
    {
      pair.skeletonDataAsset = skeletonAnimation.skeletonDataAsset;
    }
  }

  public void TurnSlotState(bool isEnable)
  {
    foreach (var pair in pairs)
    {
      skeletonAnimation.Skeleton.SetAttachment(pair.slotName, isEnable ? pair.attachmentName : null);
    }
  }
}

[Serializable]
public class SlotAttachmentPair
{


  public string slotName;

  public string attachmentName;

  // This is required so Odin can resolve the slot list contextually
  public SkeletonDataAsset skeletonDataAsset;

  // Dropdown function
  private IEnumerable<string> GetSlotNames()
  {
    if (skeletonDataAsset == null)
      return new[] { "(No SkeletonDataAsset assigned)" };

    SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(true);
    if (skeletonData == null)
      return new[] { "(SkeletonData not loaded)" };

    var names = new List<string>();
    foreach (SlotData slot in skeletonData.Slots)
      names.Add(slot.Name);

    return names;
  }

#if UNITY_EDITOR
  private void OnSlotNameChanged()
  {
    if (string.IsNullOrEmpty(slotName) || skeletonDataAsset == null)
      return;

    SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(true);
    if (skeletonData == null)
      return;

    var slotData = skeletonData.FindSlot(slotName);
    if (slotData == null)
      return;

    int slotIndex = slotData.Index;
    var skin = skeletonData.DefaultSkin;
    if (skin == null)
      return;

    var entries = new List<Skin.SkinEntry>();
    skin.GetAttachments(slotIndex, entries);

    if (entries.Count > 0)
      attachmentName = entries[0].Name;
  }
#endif
}

public class ToggleBoneSlot : MonoBehaviour
{
  [SerializeField] private SkeletonAnimation skeletonAnimation;

  [Header("Initial Slot Attachments")]
  [SerializeField]
  private List<SlotAttachmentPair> initialAttachments = new List<SlotAttachmentPair>();

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
      // H3Log.LogWarning($"Error setting attachment '{attachmentName}' on slot '{slotName}': {e.Message}");
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
