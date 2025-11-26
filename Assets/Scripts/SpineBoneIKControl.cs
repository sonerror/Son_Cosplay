using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;

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
  private float _ikWeight; // blend weight (0..1)
  private Tween _weightTween;

  private void Start()
  {
    SetBoneByName();
    if (enableOnStart) EnableIKControl();
  }

  private void OnDestroy()
  {
    DisableIKControl(true); // instantly reset on destroy
  }

  private void UpdateIK(ISkeletonAnimation animated)
  {
    if (_bone == null || target == null) return;

    // target local position in bone's parent space
    float localX, localY;

    if (_bone.Parent != null)
      _bone.Parent.WorldToLocal(
          TargetPosition.x + positionOffset.x,
          TargetPosition.y + positionOffset.y,
          out localX, out localY
      );
    else
      skeletonAnimation.Skeleton.RootBone.WorldToLocal(
          TargetPosition.x + positionOffset.x,
          TargetPosition.y + positionOffset.y,
          out localX, out localY
      );

    // Lerp between animated (setup) pose and IK target
    float blendedX = Mathf.Lerp(_bone.X, localX * scalingX, _ikWeight);
    float blendedY = Mathf.Lerp(_bone.Y, localY * scalingY, _ikWeight);

    _bone.X = blendedX;
    _bone.Y = blendedY;

    if (isBoneRotateControlByTarget)
    {
      float targetRot = target.localRotation.eulerAngles.z;
      _bone.Rotation = Mathf.LerpAngle(_bone.Rotation, targetRot, _ikWeight);
    }
  }

  [Button]
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

  [Button]
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
    if (_bone == null) Debug.LogWarning($"Bone '{boneName}' not found in skeleton.", this);
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
