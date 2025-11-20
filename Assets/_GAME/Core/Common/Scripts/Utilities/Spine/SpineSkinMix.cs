using System.Collections;
using Spine.Unity;
using Spine;
using System.Collections.Generic;
using UnityEngine;


namespace Utilities
{
  public class SpineSkinMix : MonoBehaviour
  {
    private ISkeletonComponent _skeletonRenderer;
    [SpineSkin("", dataField = "_skeletonRenderer")]
    [SerializeField] private string[] _skins;

    private void OnEnable()
    {
      ReloadSkin();
    }

    public void ReloadSkin()
    {
      if (_skeletonRenderer == null) _skeletonRenderer = GetComponent<ISkeletonComponent>();

      Skeleton skeleton = _skeletonRenderer.Skeleton;
      if (skeleton == null) return;
      SkeletonData skeletonData = skeleton.Data;
      Skin customSkin = new Skin("skin");

      if (_skins != null)
      {
        foreach (string skinName in _skins)
        {
          Skin skin = skeletonData.FindSkin(skinName);
          if (skin != null)
          {
            customSkin.AddSkin(skin);
          }
          else
          {
            Debug.LogError("Skin not found: " + skinName);
          }
        }
      }

      skeleton.SetSkin(customSkin);
      skeleton.SetSlotsToSetupPose(); // 2. Make sure it refreshes.

      if (_skeletonRenderer is SkeletonAnimation)
      {
        (_skeletonRenderer as SkeletonAnimation).Update(0);
      }
      else if (_skeletonRenderer is SkeletonMecanim)
      {
        (_skeletonRenderer as SkeletonMecanim).Update();
      }
      else if (_skeletonRenderer is SkeletonGraphic)
      {
        (_skeletonRenderer as SkeletonGraphic).Update();
      }
    }

#if UNITY_EDITOR
    [Header("Editor")]
    [SerializeField] private bool _isUpdateOnValidate = false;

    private void OnInitEditor()
    {
      _skeletonRenderer = GetComponent<ISkeletonComponent>();
    }

    private void OnValidate()
    {
      if (_isUpdateOnValidate)
      {
        ReloadSkin();
      }
    }
#endif
  }
}