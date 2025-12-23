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

  public GameObject Mouse1, Mouse2, Mouse3;
  public GameObject MouseDone1, MouseDone2, MouseDone3;

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
    if (Mouse1.activeSelf)
    {
      Mouse1.SetActive(false);
      MouseDone1.SetActive(true);
    }

    if (Mouse2.activeSelf)
    {
      Mouse2.SetActive(false);
      MouseDone2.SetActive(true);
    }

    if (Mouse3.activeSelf)
    {
      Mouse3.SetActive(false);
      MouseDone3.SetActive(true);
    }
  }
  public void SetMouseHappy()
  {
    if (isDoneMouse)
    {
      MouseDone1.SetActive(false);
      MouseDone2.SetActive(true);
      MouseDone3.SetActive(false);
      return;
    }
    else
    {
      Mouse1.SetActive(false);
      Mouse2.SetActive(true);
      Mouse3.SetActive(false);
    }
  }

  public void SetMouseIdle()
  {
    if (isDoneMouse)
    {
      MouseDone1.SetActive(true);
      MouseDone2.SetActive(false);
      MouseDone3.SetActive(false);
      return;
    }
    else
    {
      Mouse1.SetActive(true);
      Mouse2.SetActive(false);
      Mouse3.SetActive(false);
    }
  }

  public void SetMouseAngry()
  {
    if (isDoneMouse)
    {
      MouseDone1.SetActive(false);
      MouseDone2.SetActive(false);
      MouseDone3.SetActive(true);
      return;
    }
    else
    {
      Mouse1.SetActive(false);
      Mouse2.SetActive(false);
      Mouse3.SetActive(true);
    }
  }

  // private bool _isEyeClosed = false;

  // public void CloseEye()
  // {
  //   if (_isEyeClosed) return;
  //   _isEyeClosed = true;
  //   eye.CloseEye();
  // }



}
