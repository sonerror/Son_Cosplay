using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using DG.Tweening;

namespace Utilities
{
  public class SpineBoneIKControl : MonoBehaviour
  {
    public SkeletonAnimation skeletonAnimation;
    public string boneName;

    public Transform target;
    public Vector3 positionOffset;
    public float scalingX = 1;
    public float scalingY = 1;
    public bool isBoneRotateControlByTarget;
    public bool controlByTargetLocalPos = true;
    [SerializeField] private bool enableOnStart = true;
    [SerializeField] private float transitionDuration = 0.3f;

    private Bone _bone;
    private bool _isControl;
    private float _ikWeight;
    private Tween _weightTween;

    void Start()
    {
      SetBoneByName();
      if (enableOnStart) EnableIKControl();
    }

    void OnDestroy()
    {
      DisableIKControl(true);
    }

    // *** FIXED signature ***
    private void UpdateIK(ISkeletonAnimation anim)
    {
      if (_bone == null || target == null) return;

      float localX, localY;

      // Convert target world/local pos -> skeleton local
      if (_bone.Parent != null)
      {
        _bone.Parent.WorldToLocal(
            TargetPosition.x + positionOffset.x,
            TargetPosition.y + positionOffset.y,
            out localX, out localY
        );
      }
      else
      {
        skeletonAnimation.Skeleton.RootBone.WorldToLocal(
            TargetPosition.x + positionOffset.x,
            TargetPosition.y + positionOffset.y,
            out localX, out localY
        );
      }

      _bone.X = Mathf.Lerp(_bone.X, localX * scalingX, _ikWeight);
      _bone.Y = Mathf.Lerp(_bone.Y, localY * scalingY, _ikWeight);

      if (isBoneRotateControlByTarget)
      {
        float targetRot = target.eulerAngles.z;
        _bone.Rotation = Mathf.LerpAngle(_bone.Rotation, targetRot, _ikWeight);
      }
    }

    public void DisableIKControl(bool instant = false)
    {
      if (!_isControl) return;
      _isControl = false;

      _weightTween?.Kill();

      if (instant)
      {
        _ikWeight = 0f;
        skeletonAnimation.UpdateLocal -= UpdateIK;
        _bone?.SetToSetupPose();
      }
      else
      {
        _weightTween = DOTween.To(() => _ikWeight, x => _ikWeight = x, 0f, transitionDuration)
            .OnComplete(() =>
            {
              skeletonAnimation.UpdateLocal -= UpdateIK;
              _bone?.SetToSetupPose();
            });
      }
    }

    public void EnableIKControl()
    {
      if (_isControl) return;
      _isControl = true;

      _weightTween?.Kill();
      skeletonAnimation.UpdateLocal += UpdateIK;
      _weightTween = DOTween.To(() => _ikWeight, x => _ikWeight = x, 1f, transitionDuration);
    }

    private void SetBoneByName()
    {
      if (skeletonAnimation == null || skeletonAnimation.Skeleton == null || string.IsNullOrEmpty(boneName))
      {
        _bone = null;
        return;
      }

      _bone = skeletonAnimation.Skeleton.FindBone(boneName);

      if (_bone == null)
        Debug.LogWarning($"Bone '{boneName}' not found.", this);
    }

    private IEnumerable<string> GetBoneNames()
    {
      if (skeletonAnimation == null || skeletonAnimation.Skeleton == null)
        return new List<string> { "(No skeleton)" };

      List<string> names = new List<string>();
      foreach (Bone b in skeletonAnimation.Skeleton.Bones)
        names.Add(b.Data.Name);
      return names;
    }

    private Vector3 TargetPosition => controlByTargetLocalPos ? target.localPosition : target.position;
  }
}
