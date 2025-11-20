using System.Collections.Generic;

using Spine;
using Spine.Unity;
using UnityEngine;

namespace Utilities
{
  [RequireComponent(typeof(SkeletonGraphic))]
  public class SpineBoneIKControlGraphic : MonoBehaviour
  {
    public SkeletonGraphic skeletonGraphic;

    public string boneName;

    public RectTransform target;
    public Vector3 positionOffset;
    public float scalingX = 1;
    public float scalingY = 1;
    public bool isBoneRotateControlByTarget;

    private Bone _bone;
    private bool _isControl;

    private void Awake()
    {
      if (!skeletonGraphic)
        skeletonGraphic = GetComponent<SkeletonGraphic>();
    }

    private void Start()
    {
      SetBoneByName();
      EnableIKControl();
    }

    private void OnDestroy()
    {
      DisableIKControl();
    }

    private void UpdateIK(ISkeletonAnimation animated)
    {
      if (_bone == null || !target) return;

      float localX, localY;

      // If bone has a parent, convert world target position to parent's local space
      if (_bone.Parent != null)
        _bone.Parent.WorldToLocal(
            TargetPosition.x + positionOffset.x,
            TargetPosition.y + positionOffset.y,
            out localX, out localY
        );
      else
        // Root bone case
        skeletonGraphic.Skeleton.RootBone.WorldToLocal(
            TargetPosition.x + positionOffset.x,
            TargetPosition.y + positionOffset.y,
            out localX, out localY
        );

      _bone.X = localX * scalingX;
      _bone.Y = localY * scalingY;

      if (isBoneRotateControlByTarget)
        _bone.Rotation = target.localRotation.eulerAngles.z;
    }


    public void DisableIKControl()
    {
      if (!_isControl) return;
      _isControl = false;

      skeletonGraphic.UpdateLocal -= UpdateIK;

      _bone?.SetToSetupPose();
    }


    public void EnableIKControl()
    {
      if (_isControl) return;
      _isControl = true;

      skeletonGraphic.UpdateLocal += UpdateIK;
    }

    private void SetBoneByName()
    {
      if (skeletonGraphic == null || skeletonGraphic.Skeleton == null || string.IsNullOrEmpty(boneName))
      {
        _bone = null;
        return;
      }

      _bone = skeletonGraphic.Skeleton.FindBone(boneName);
      if (_bone == null) Debug.LogWarning($"Bone '{boneName}' not found in skeleton.", this);
    }

    private IEnumerable<string> GetBoneNames()
    {
      if (skeletonGraphic == null || skeletonGraphic.Skeleton == null)
        return new List<string> { "(No skeleton)" };

      List<string> names = new List<string>();
      foreach (Bone b in skeletonGraphic.Skeleton.Bones)
        names.Add(b.Data.Name);
      return names;
    }

    private Vector3 TargetPosition
    {
      get
      {
        // Get the rect center in local space of the RectTransform
        Vector3 rectCenter = target.rect.center;

        // Convert that local point into world space
        Vector3 worldPos = target.TransformPoint(rectCenter);

        // Convert world space into the local space of the skeletonGraphic
        Vector3 localPos = skeletonGraphic.transform.InverseTransformPoint(worldPos);

        return localPos;
      }
    }
  }
}
