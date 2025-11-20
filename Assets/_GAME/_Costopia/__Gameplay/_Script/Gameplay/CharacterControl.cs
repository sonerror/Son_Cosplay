using System;
using System.Collections.Generic;
using DG.Tweening;
using HoangHH;
using Spine;
using Spine.Unity;
using UnityEngine;
using Utilities;

namespace Costopia.Gameplay
{
  public enum CharacterControlID
  {
    Head = 0,
    Face = 1,
    Eye = 2,
    Body = 3,
    // Adding more if need
  }

  public enum CharacterAnimID
  {
    Idle = 0,
    Happy = 1,
    Angry = 2,
  }

  public class CharacterControl : H3MonoBehaviour
  {
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    [SerializeField]
    [ArrayElementNameMatchEnum(typeof(CharacterControlID))]
    private SpineBoneIKControl[] spineBoneIKControls;

    [SerializeField]
    private ToggleBoneSlot boneSlotToggle;

    [SerializeField]
    private SpineAttachmentLocker attachmentLocker;


    // [SerializeField]
    // private LoopAnimTransformFloating loopEyeOpen;


    [SerializeField]
    [ArrayElementNameMatchEnum(typeof(CharacterAnimID))]
    private string[] anims = {
            "idle",
            "happy",
            "angry",
        };

    public SkeletonDataAsset SkeletonDataAsset => skeletonAnimation.SkeletonDataAsset;
    public SkeletonAnimation SkeletonAnimation => skeletonAnimation;

    #region Bone Handle

    /// <summary>
    /// Disable IK control for specific control ID
    /// </summary>
    /// <param name="controlID"></param>
    public void DisableIKControl(CharacterControlID controlID)
    {
      int index = (int)controlID;
      DisableIKControl(index);
    }

    public void DisableIKControl(int index)
    {
      if (index < 0 || index >= spineBoneIKControls.Length) return;
      SpineBoneIKControl controller = spineBoneIKControls[index];
      if (controller != null) controller.DisableIKControl();
    }

    /// <summary>
    /// Enable IK control for specific control ID
    /// </summary>
    /// <param name="controlID"></param>
    public void EnableIKControl(CharacterControlID controlID)
    {
      int index = (int)controlID;
      EnableIKControl(index);
    }

    public void EnableIKControl(int index)
    {
      if (index < 0 || index >= spineBoneIKControls.Length) return;
      SpineBoneIKControl controller = spineBoneIKControls[index];
      if (controller != null) controller.EnableIKControl();
    }
    #endregion

    #region Anim Handle

    public void SetBodyCanMove(bool canMove)
    {
      if (canMove)
      {
        spineBoneIKControls[(int)CharacterControlID.Body].DisableIKControl();
        // reset the skeleton anim to the start
      }
      else
      {
        spineBoneIKControls[(int)CharacterControlID.Body].EnableIKControl();
      }
    }

    public void ManualSetAnim(string animName, bool backToIdle)
    {
      Spine.AnimationState state = skeletonAnimation.AnimationState;
      state.SetAnimation(0, animName, false);
      if (backToIdle)
      {
        state.AddAnimation(0, GetAnimName(CharacterAnimID.Idle), true, 0f);
      }
    }

    public void AnimToIdle(CharacterAnimID id)
    {
      if (!gameObject.activeInHierarchy) return;
      if (id == CharacterAnimID.Idle) return;
      string animName = GetAnimName(id);
      Spine.AnimationState state = skeletonAnimation.AnimationState;
      // Interrupt whatever is playing
      state.SetAnimation(0, animName, false);
      // Queue idle after this anim finishes
      state.AddAnimation(0, GetAnimName(CharacterAnimID.Idle), true, 0f);
    }

    private string GetAnimName(CharacterAnimID animID)
    {
      int index = (int)animID;
      if (index < 0 || index >= anims.Length) return null;
      return anims[(int)animID];
    }

    #endregion

    #region Attachment Handle

    /// <summary>
    /// Toggle attachment for slot, if attachmentName is null, it will toggle between no attachment and default attachment
    /// </summary>
    /// <param name="slotName"></param>
    /// <param name="attachmentName"></param>
    public void TurnSlotAttachment(string slotName, string attachmentName = null)
    {
      boneSlotToggle.TurnSlotAttachment(slotName, attachmentName);
    }

    /// <summary>
    /// Force set attachment for slot, will override any change from animation or other script
    /// </summary>
    /// <param name="slotName"></param>
    /// <param name="attachmentName"></param>
    public void SetOverrideAttachment(string slotName, string attachmentName = null)
    {
      attachmentLocker.SetOverride(slotName, attachmentName);
    }


    /// <summary>
    /// Remove override attachment for slot, if attachmentName is null, it will remove override for the slot
    /// </summary>
    /// <param name="slotName"></param>
    public void RemoveOverrideAttachment(string slotName)
    {
      attachmentLocker.RemoveOverride(slotName);
    }

    public void RemoveAllOverrideAttachment()
    {
      attachmentLocker.ClearOverrides();
    }

    #endregion

    #region Moving Handle

    private DG.Tweening.Sequence _moveSeq;


    [SerializeField]
    private float readyMoveTime = 0.4f;

    [SerializeField]
    private float rotateZ = 4f;

    [SerializeField]
    private float offsetFakeMoveX = 0.5f;

    [SerializeField]
    private float offsetFakeMoveY = 0.5f;

    [SerializeField]
    private float moveTime = 0.5f;


    public void MoveNext(Action callbackOnStartFakeMoving = null, Action callbackOnEndFakeMoving = null)
    {
      Vector3 baseRotation = Tf.localEulerAngles;
      Vector3 localFakeMove = new Vector3(Tf.localPosition.x - offsetFakeMoveX,
          Tf.localPosition.y - offsetFakeMoveY, Tf.localPosition.z);
      _moveSeq?.Kill();
      _moveSeq = DOTween.Sequence();
      // rotate to -z rotation
      _moveSeq.Append(Tf.DOLocalRotate(new Vector3(baseRotation.x, baseRotation.y, rotateZ), readyMoveTime).SetEase(Ease.InSine));
      _moveSeq.Join(Tf.DOLocalMove(localFakeMove, readyMoveTime).SetEase(Ease.InSine));
      _moveSeq.AppendCallback(() => callbackOnStartFakeMoving?.Invoke());
      // then move to new x, also rotate z to original
      _moveSeq.Append(Tf.DOLocalMove(Tf.localPosition, moveTime));
      _moveSeq.Join(Tf.DOLocalRotate(baseRotation, moveTime));
      _moveSeq.AppendCallback(() => callbackOnEndFakeMoving?.Invoke());
    }

    #endregion

    #region public method for searching

    public IEnumerable<string> GetSlotNames()
    {
      if (SkeletonDataAsset == null)
        return new[] { "(No SkeletonDataAsset assigned)" };

      SkeletonData skeletonData = SkeletonDataAsset.GetSkeletonData(true);
      if (skeletonData == null)
        return new[] { "(SkeletonData not loaded)" };

      var names = new List<string>();
      foreach (SlotData slot in skeletonData.Slots)
        names.Add(slot.Name);

      return names;
    }

    #endregion
  }
}