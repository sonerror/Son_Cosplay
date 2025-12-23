using System;
using Spine.Unity;
using UnityEngine;

public class HideMouse : MonoBehaviour
{
  [SerializeField] private SkeletonAnimation skeletonAnimation;

  [SerializeField] private string nameBoneHide;

  void Start()
  {
    SetAlphaSlotName(nameBoneHide, 0);
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
}
