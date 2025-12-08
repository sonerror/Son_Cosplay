using System;
using Animation = Spine.Animation;
using AnimationState = Spine.AnimationState;
using UnityEngine;
using Spine.Unity;
using Utilities;

public enum CharacterAnimID
{
  Idle = 0,
  Happy = 1,
  Angry = 2,
}

public class CharacterControl : GameUnit
{
  [SerializeField] private SkeletonAnimation skeletonAnimation;

  [SerializeField] private LoopAnimTransformFloating eye;

  void Start()
  {
  }
  public void TurnSlotAttachment(string slotName, string attachmentName = null)
  {
    skeletonAnimation.Skeleton.SetAttachment(slotName, attachmentName);
  }

  private bool _isEyeClosed = false;

  public void CloseEye()
  {
    if (_isEyeClosed) return;
    _isEyeClosed = true;
    eye.CloseEye();
  }



}
