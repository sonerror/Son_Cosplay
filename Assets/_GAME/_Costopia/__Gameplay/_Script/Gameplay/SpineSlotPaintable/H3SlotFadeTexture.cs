using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using Spine;
using Spine.Unity;
using UnityEngine;

namespace Costopia.Gameplay
{
  public class H3SlotFadeTexture : MonoBehaviour
  {
    private static readonly int MainTexID = Shader.PropertyToID("_MainTex");
    private static readonly int AlphaID = Shader.PropertyToID("_AlphaMultiplier");

    [SerializeField] private bool linkWithSlotPaintableTexture;

    [SerializeField]
    private H3SlotPaintableTexture paintableTexture;

    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private Material skeletonMaterial;
    [SerializeField] private Material paintableMaterialTemplate;

    [SerializeField]
    private string slotName;

    [SerializeField]
    private string attachmentName;

    [SerializeField] private bool hideOnStart = true;
    [SerializeField, HideInInspector] private SkeletonDataAsset skeletonDataAsset;

    public float RevealRatio => _material.GetFloat(AlphaID);

    private float _currentFadeTime;
    private float _fadeDurationInUse;

    private Coroutine _fadeRoutine;
    private Material _material;
    private float _startAlpha;
    private float _targetAlpha;
    public Action onFadeComplete;
    public Action<float> onAlphaChange;

    public bool IsFading => _fadeRoutine != null;
    public bool IsPausing { get; private set; }

    private void Start()
    {
      if (!linkWithSlotPaintableTexture) ApplySlotFade();
      else ApplyLinkWithPaintable();
    }

    private void ApplyLinkWithPaintable()
    {
      if (paintableTexture == null)
      {
        Debug.LogError("Paintable texture reference is missing.");
        return;
      }
      StartCoroutine(WaitForMaterial());
      return;
      IEnumerator WaitForMaterial()
      {
        // Wait until the material is initialized
        while (paintableTexture.Material == null)
          yield return null;

        skeletonAnimation = paintableTexture.SkeletonAnimation;
        _material = paintableTexture.Material;

        SetSlotAlpha(_material.GetFloat(AlphaID));
      }
    }

    private void ApplySlotFade()
    {
      if (skeletonAnimation == null || skeletonAnimation.Skeleton == null)
      {
        Debug.LogError("SkeletonAnimation or Skeleton is not assigned.");
        return;
      }

      Slot slot = skeletonAnimation.Skeleton.FindSlot(slotName);
      if (slot == null)
      {
        Debug.LogError($"Slot '{slotName}' not found in the skeleton.");
        return;
      }

      if (!string.IsNullOrEmpty(attachmentName))
      {
        Attachment attachment = skeletonAnimation.Skeleton.GetAttachment(slotName, attachmentName);
        if (attachment != null)
          slot.Attachment = attachment;
        else
          Debug.LogWarning($"Attachment '{attachmentName}' not found in slot '{slotName}'. Falling back to default.");
      }

      _material = new Material(paintableMaterialTemplate);
      _material.SetTexture(MainTexID, skeletonMaterial.mainTexture);
      skeletonAnimation.CustomSlotMaterials[slot] = _material;

      SetSlotAlpha(hideOnStart ? 0f : 1f);
    }

    public void SetSlotMaterialToBase()
    {
      Slot slot = skeletonAnimation.Skeleton.FindSlot(slotName);
      skeletonAnimation.CustomSlotMaterials[slot] = skeletonMaterial;
    }

    public void DisableSlot()
    {
      skeletonAnimation.Skeleton.SetAttachment(slotName, null);
    }

    public void EnableSlot()
    {
      skeletonAnimation.Skeleton.SetAttachment(slotName, attachmentName);
    }

    public void SetSlotAlpha(float alpha)
    {
      if (_material != null && _material.HasProperty(AlphaID))
      {
        _material.SetFloat(AlphaID, alpha);
        onAlphaChange?.Invoke(alpha);
      }
    }

    private IEnumerator FadeRoutine(bool removeMaterial, Ease ease)
    {
      while (_currentFadeTime < _fadeDurationInUse)
      {
        if (!IsPausing)
        {
          _currentFadeTime += Time.deltaTime;
          float t = Mathf.Clamp01(_currentFadeTime / _fadeDurationInUse);
          // convert t based on the DOTween Ease
          if (ease != Ease.Linear) t = DOVirtual.EasedValue(0f, 1f, t, ease, 1f, 0f);
          float alpha = Mathf.Lerp(_startAlpha, _targetAlpha, t);
          SetSlotAlpha(alpha);
        }

        yield return null;
      }

      SetSlotAlpha(_targetAlpha);
      _fadeRoutine = null;
      onFadeComplete?.Invoke();

      if (removeMaterial)
      {
        var slot = skeletonAnimation.Skeleton.FindSlot(slotName);
        if (slot != null)
        {
          skeletonAnimation.CustomSlotMaterials.Remove(slot);
          slot.Attachment = null;
        }
      }
    }

    public void SetFadeDuration(float duration)
    {
      fadeDuration = duration;
      if (_fadeRoutine != null)
      {
        StopCoroutine(_fadeRoutine);
        _fadeRoutine = null;
      }
      _currentFadeTime = 0f;
    }

    public void FadeIn(float? customDuration = null, Ease ease = Ease.Linear)
    {
      FadeTo(1f, customDuration, false, ease);
    }

    public void FadeOut(float? customDuration = null, bool removeMaterial = false, Ease ease = Ease.Linear)
    {
      FadeTo(0f, customDuration, removeMaterial, ease);
    }

    private void FadeTo(float targetAlpha, float? customDuration, bool removeMaterial, Ease ease)
    {
      if (_material == null)
      {
        Debug.LogWarning("Material not initialized. Applying fade setup.");
        if (!linkWithSlotPaintableTexture) ApplySlotFade();
        else ApplyLinkWithPaintable();
      }

      if (_material == null) return;

      if (_fadeRoutine != null)
        StopCoroutine(_fadeRoutine);

      _startAlpha = _material.GetFloat(AlphaID);
      _targetAlpha = targetAlpha;
      _fadeDurationInUse = customDuration ?? fadeDuration;
      _currentFadeTime = 0f;
      IsPausing = false;
      _fadeRoutine = StartCoroutine(FadeRoutine(removeMaterial, ease));
    }

    public void PauseFade() => IsPausing = true;
    public void ResumeFade() => IsPausing = false;

#if UNITY_EDITOR
    // [Button("Get References")]
    private void GetReferences()
    {
      skeletonAnimation = GetComponentInParent<SkeletonAnimation>();
      if (skeletonAnimation == null)
        Debug.LogError("SkeletonAnimation component not found.");
      skeletonDataAsset = skeletonAnimation?.SkeletonDataAsset;
    }

    private void OnValidate()
    {
      skeletonDataAsset = skeletonAnimation != null ? skeletonAnimation.SkeletonDataAsset : null;
    }

    private IEnumerable<string> GetSlotNames()
    {
      if (skeletonAnimation == null)
        return new[] { "(Skeleton not assigned)" };

      Skeleton skeleton = skeletonAnimation.Skeleton;
      if (skeleton == null)
        return new[] { "(Skeleton not initialized)" };

      var names = new List<string>();
      for (int i = 0; i < skeleton.Slots.Count; i++)
        names.Add(skeleton.Slots.Items[i].Data.Name);

      return names;
    }

    // [Button("Fade In")]
    private void TestFadeIn() => FadeTo(1f, fadeDuration, false, Ease.Linear);

    // [Button("Fade Out")]
    private void TestFadeOut() => FadeTo(0f, fadeDuration, false, Ease.Linear);

    // [Button("Pause Fade")]
    private void TestPauseFade() => PauseFade();

    // [Button("Resume Fade")]
    private void TestResumeFade() => ResumeFade();
#endif

#if UNITY_EDITOR
    private void OnSlotNameChanged()
    {
      if (string.IsNullOrEmpty(slotName) || skeletonDataAsset == null)
        return;

      SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(true);
      if (skeletonData == null)
        return;

      var slotData = skeletonData.FindSlot(slotName);
      if (slotData == null)
        return;

      int slotIndex = slotData.Index;
      var skin = skeletonData.DefaultSkin;
      if (skin == null)
        return;

      var entries = new List<Skin.SkinEntry>();
      skin.GetAttachments(slotIndex, entries);

      if (entries.Count > 0)
        attachmentName = entries[0].Name;
    }
#endif
  }
}