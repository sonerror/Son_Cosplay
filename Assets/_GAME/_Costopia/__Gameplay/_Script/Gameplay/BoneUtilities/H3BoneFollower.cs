
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

public class H3BoneFollower : MonoBehaviour
{

  [SerializeField] private SkeletonAnimation skeletonAnimation;

  [SerializeField] private string boneName;

  public bool followXYPosition = true;
  public bool followZPosition = true;
  public bool followBoneRotation = true;

  public bool followSkeletonFlip = true;

  public bool followLocalScale = false;

  public bool followParentWorldScale = false;

  public enum AxisOrientation { XAxis = 1, YAxis }
  public AxisOrientation maintainedAxisOrientation = AxisOrientation.XAxis;

  [SerializeField] private Vector3 offsetPosition = Vector3.zero;


  private Bone bone;
  private bool valid;
  private Transform skeletonTransform;
  private bool skeletonTransformIsParent;

  private void Awake()
  {
    Initialize();
  }

  private void OnEnable()
  {
    if (skeletonAnimation != null)
      skeletonAnimation.OnRebuild -= HandleRebuildRenderer;

    if (skeletonAnimation != null)
      skeletonAnimation.OnRebuild += HandleRebuildRenderer;

    Initialize();
  }

  private void OnDestroy()
  {
    if (skeletonAnimation != null)
      skeletonAnimation.OnRebuild -= HandleRebuildRenderer;
  }

  private void HandleRebuildRenderer(SkeletonRenderer r)
  {
    Initialize();
  }

  public void Initialize()
  {
    valid = skeletonAnimation != null && skeletonAnimation.Skeleton != null;
    if (!valid) return;

    skeletonTransform = skeletonAnimation.transform;
    skeletonTransformIsParent = Transform.ReferenceEquals(skeletonTransform, transform.parent);

    SetBoneByName();
  }

  private void LateUpdate()
  {
    if (!valid)
    {
      Initialize();
      return;
    }

    if (bone == null) return;

    Transform thisTransform = transform;
    float additionalFlipScale = 1f;

    if (skeletonTransformIsParent)
    {
      // Local follow (optimized path)
      thisTransform.localPosition = new Vector3(
          followXYPosition ? bone.WorldX : thisTransform.localPosition.x,
          followXYPosition ? bone.WorldY : thisTransform.localPosition.y,
          followZPosition ? 0f : thisTransform.localPosition.z
      ) + offsetPosition;


      if (followBoneRotation)
      {
        float halfRotation = Mathf.Atan2(bone.C, bone.A) * 0.5f;
        if (followLocalScale && bone.ScaleX < 0)
          halfRotation += Mathf.PI * 0.5f;

        Quaternion q = default;
        q.z = Mathf.Sin(halfRotation);
        q.w = Mathf.Cos(halfRotation);
        thisTransform.localRotation = q;
      }
    }
    else
    {
      // World follow (fallback path)
      Vector3 targetWorldPosition = skeletonTransform.TransformPoint(new Vector3(bone.WorldX, bone.WorldY, 0f) + offsetPosition);
      if (!followZPosition) targetWorldPosition.z = thisTransform.position.z;
      if (!followXYPosition)
      {
        targetWorldPosition.x = thisTransform.position.x;
        targetWorldPosition.y = thisTransform.position.y;
      }

      Vector3 skeletonLossyScale = skeletonTransform.lossyScale;
      Transform parent = thisTransform.parent;
      Vector3 parentLossyScale = parent != null ? parent.lossyScale : Vector3.one;

      if (followBoneRotation)
      {
        float boneWorldRotation = bone.WorldRotationX;
        if ((skeletonLossyScale.x * skeletonLossyScale.y) < 0)
          boneWorldRotation = -boneWorldRotation;

        if (followSkeletonFlip || maintainedAxisOrientation == AxisOrientation.XAxis)
        {
          if ((skeletonLossyScale.x * parentLossyScale.x) < 0)
            boneWorldRotation += 180f;
        }
        else
        {
          if ((skeletonLossyScale.y * parentLossyScale.y) < 0)
            boneWorldRotation += 180f;
        }

        Vector3 worldRotation = skeletonTransform.rotation.eulerAngles;
        if (followLocalScale && bone.ScaleX < 0)
          boneWorldRotation += 180f;

        thisTransform.SetPositionAndRotation(
            targetWorldPosition,
            Quaternion.Euler(worldRotation.x, worldRotation.y, worldRotation.z + boneWorldRotation)
        );
      }
      else
      {
        thisTransform.position = targetWorldPosition;
      }

      additionalFlipScale = Mathf.Sign(
          skeletonLossyScale.x * parentLossyScale.x * skeletonLossyScale.y * parentLossyScale.y
      );
    }

    if (followParentWorldScale || followLocalScale || followSkeletonFlip)
    {
      Vector3 localScale = Vector3.one;

      if (followParentWorldScale && bone.Parent != null)
        localScale = new Vector3(bone.Parent.WorldScaleX, bone.Parent.WorldScaleY, 1f);

      if (followLocalScale)
        localScale.Scale(new Vector3(bone.ScaleX, bone.ScaleY, 1f));

      if (followSkeletonFlip)
        localScale.y *= Mathf.Sign(bone.Skeleton.ScaleX * bone.Skeleton.ScaleY) * additionalFlipScale;

      thisTransform.localScale = localScale;
    }
  }

  private void SetBoneByName()
  {
    if (skeletonAnimation == null || skeletonAnimation.Skeleton == null || string.IsNullOrEmpty(boneName))
    {
      bone = null;
      return;
    }

    bone = skeletonAnimation.Skeleton.FindBone(boneName);
    if (bone == null)
    {
      Debug.LogWarning($"Bone '{boneName}' not found in skeleton.", this);
    }
  }

  private IEnumerable<string> GetBoneNames()
  {
    if (skeletonAnimation == null || skeletonAnimation.Skeleton == null)
      return new List<string> { "(No skeleton)" };

    List<string> names = new List<string>();
    foreach (var bone in skeletonAnimation.Skeleton.Bones)
      names.Add(bone.Data.Name);
    return names;
  }
}
