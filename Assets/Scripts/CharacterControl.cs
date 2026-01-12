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
  public SkeletonAnimation SkeletonAnimation => skeletonAnimation;

  public GameObject MouseNorBf, MouseHappyBf, MouseAngryBf;
  public GameObject MouseNorAf, MouseHappyAf, MouseAngryAf;

  // [SerializeField] private LoopAnimTransformFloating eye;

  void Start()
  {
  }
  public void TurnSlotAttachment(string slotName, string attachmentName = null)
  {
    skeletonAnimation.Skeleton.SetAttachment(slotName, attachmentName);
  }

  public void SetAlphaSlotName(string slotName, float alphaSlotName)
  {
    var slot = skeletonAnimation.Skeleton.FindSlot(slotName);
    if (slot != null)
    {
      slot.A = alphaSlotName;
    }
    else
    {
      Debug.LogWarning($"Slot {slotName} not found in skeleton.");
    }
  }

  private bool isDoneMouse = false;

  public void SetMouseDone()
  {
    isDoneMouse = true;
    if (MouseNorBf.activeSelf)
    {
      MouseNorBf.SetActive(false);
      MouseNorAf.SetActive(true);
    }

    if (MouseHappyBf.activeSelf)
    {
      MouseHappyBf.SetActive(false);
      MouseHappyAf.SetActive(true);
    }

    if (MouseAngryBf.activeSelf)
    {
      MouseAngryBf.SetActive(false);
      MouseAngryAf.SetActive(true);
    }
  }
  public void SetMouseHappy()
  {
    skeletonAnimation.AnimationState.SetAnimation(0, "happy", false);

    // if (isDoneMouse)
    // {
    //   MouseNorAf.SetActive(false);
    //   MouseHappyAf.SetActive(true);
    //   MouseAngryAf.SetActive(false);
    //   return;
    // }
    // else
    // {
    //   MouseNorBf.SetActive(false);
    //   MouseHappyBf.SetActive(true);
    //   MouseAngryBf.SetActive(false);
    // }
  }

  public void SetMouseIdle()
  {
    skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);

    // if (isDoneMouse)
    // {
    //   MouseNorAf.SetActive(true);
    //   MouseHappyAf.SetActive(false);
    //   MouseAngryAf.SetActive(false);
    //   return;
    // }
    // else
    // {
    //   MouseNorBf.SetActive(true);
    //   MouseAngryBf.SetActive(false);
    //   MouseHappyBf.SetActive(false);
    // }
  }

  public void SetMouseAngry()
  {
    skeletonAnimation.AnimationState.SetAnimation(0, "angry", false);

    // if (isDoneMouse)
    // {
    //   MouseNorAf.SetActive(false);
    //   MouseHappyAf.SetActive(false);
    //   MouseAngryAf.SetActive(true);
    //   return;
    // }
    // else
    // {
    //   MouseNorBf.SetActive(false);
    //   MouseHappyBf.SetActive(false);
    //   MouseAngryBf.SetActive(true);
    // }
  }

  // private bool _isEyeClosed = false;

  // public void CloseEye()
  // {
  //   if (_isEyeClosed) return;
  //   _isEyeClosed = true;
  //   eye.CloseEye();
  // }



}
