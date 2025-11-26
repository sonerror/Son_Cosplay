using System;
using Spine.Unity;
using UnityEngine;

public class CharactorControl : GameUnit
{

  private const string SLOT_FRONT_HAIR = "Base_front_bang";
  private const string SLOT_FRONT_HAIR_DIRT = "Draw_Dirt_hair";
  private const string ATTACHMENT_HEADBAND = "Base/band 1";


  [SerializeField] private SkeletonAnimation skeletonAnimation;
  // [SerializeField] private ToggleBoneSlot boneSlotToggle;

  [SerializeField]
  [SpineSlot(dataField: nameof(SkeletonDataAsset))]
  private string slotHeadband;

  [SerializeField]
  [SpineSlot(dataField: nameof(SkeletonDataAsset))]
  private string slotGlass;
  void Start()
  {
    TurnSlotAttachment(slotHeadband, ATTACHMENT_HEADBAND);
    TurnSlotAttachment(SLOT_FRONT_HAIR);
    TurnSlotAttachment(SLOT_FRONT_HAIR_DIRT);
    TurnSlotAttachment(slotGlass);
  }


  public void TurnSlotAttachment(string slotName, string attachmentName = null)
  {
    // boneSlotToggle.TurnSlotAttachment(slotName, attachmentName);

    try
    {
      // if (string.IsNullOrEmpty(attachmentName)) attachmentName = null;
      skeletonAnimation.Skeleton.SetAttachment(slotName, attachmentName);
    }
    catch (Exception e)
    {
      Debug.LogError($"Error setting attachment '{attachmentName}' on slot '{slotName}': {e.Message}");
    }
  }

}
