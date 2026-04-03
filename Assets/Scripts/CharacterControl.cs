using System;
using Animation = Spine.Animation;
using AnimationState = Spine.AnimationState;
using UnityEngine;
using Spine.Unity;
using Utilities;
using System.Collections.Generic;

public enum CharacterAnimID
{
  Idle = 0,
  Happy = 1,
  Angry = 2,
}

public class CharacterControl : Character
{


  public GameObject MouseNorBf, MouseHappyBf, MouseAngryBf;
  public GameObject MouseNorAf, MouseHappyAf, MouseAngryAf;
  [SerializeField] private string happyName = "happy";
  [SerializeField] private string idleName = "idle";
  [SerializeField] private string angryName = "angry";

  public void SetNewNameAngry()
  {
    angryName = idleName;
  }
  private bool isDoneMouse = false;
  [SerializeField] private bool customMouseAnim = false;

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
    skeletonAnimation.AnimationState.SetAnimation(0, happyName, false);
    if (!customMouseAnim) return;
    if (isDoneMouse)
    {
      MouseNorAf.SetActive(false);
      MouseHappyAf.SetActive(true);
      MouseAngryAf.SetActive(false);
      return;
    }
    else
    {
      MouseNorBf.SetActive(false);
      MouseHappyBf.SetActive(true);
      MouseAngryBf.SetActive(false);
    }
  }

  public void SetMouseIdle()
  {
    skeletonAnimation.AnimationState.SetAnimation(0, idleName, true);
    if (!customMouseAnim) return;

    if (isDoneMouse)
    {
      MouseNorAf.SetActive(true);
      MouseHappyAf.SetActive(false);
      MouseAngryAf.SetActive(false);
      return;
    }
    else
    {
      MouseNorBf.SetActive(true);
      MouseAngryBf.SetActive(false);
      MouseHappyBf.SetActive(false);
    }
  }

  public void SetMouseAngry()
  {
    skeletonAnimation.AnimationState.SetAnimation(0, angryName, false);
    if (!customMouseAnim) return;

    if (isDoneMouse)
    {
      MouseNorAf.SetActive(false);
      MouseHappyAf.SetActive(false);
      MouseAngryAf.SetActive(true);
      return;
    }
    else
    {
      MouseNorBf.SetActive(false);
      MouseHappyBf.SetActive(false);
      MouseAngryBf.SetActive(true);
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
