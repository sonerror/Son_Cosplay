using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;


[System.Serializable]
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

