using System;
using Animation = Spine.Animation;
using AnimationState = Spine.AnimationState;
using UnityEngine;
using Spine.Unity;

public enum CharacterAnimID
{
  Idle = 0,
  Happy = 1,
  Angry = 2,
}

public class CharacterControl : GameUnit
{
  [SerializeField] private SkeletonAnimation skeletonAnimation;
  [SerializeField] private ToggleBoneSlot boneSlotToggle;

  void Start()
  {
  }


  public void TurnSlotAttachment(string slotName, string attachmentName = null)
  {
    boneSlotToggle.TurnSlotAttachment(slotName, attachmentName);
  }



}
