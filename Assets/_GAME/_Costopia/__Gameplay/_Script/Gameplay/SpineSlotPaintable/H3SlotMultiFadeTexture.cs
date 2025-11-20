using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using Spine;
using Spine.Unity;
using UnityEngine;

namespace Costopia.Gameplay
{
  public class H3SlotMultiFadeTexture : MonoBehaviour
  {
    private static readonly int MainTexID = Shader.PropertyToID("_MainTex");
    private static readonly int AlphaID = Shader.PropertyToID("_AlphaMultiplier");

    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private Material skeletonMaterial;
    [SerializeField] private Material paintableMaterialTemplate;
    [SerializeField] private bool hideOnStart = true;

    [SerializeField, HideInInspector] private SkeletonDataAsset skeletonDataAsset;

    [SerializeField]
    private List<SlotAttachmentPair> slotAttachments = new List<SlotAttachmentPair>();

    private Coroutine _fadeRoutine;
    private Material _material;
    private float _startAlpha;
    private float _targetAlpha;
    private float _fadeDurationInUse;
    private float _currentFadeTime;

    public Action onFadeComplete;
    public Action<float> onAlphaChange;
    public bool IsFading => _fadeRoutine != null;
    public bool IsPausing { get; private set; }
    public float RevealRatio => _material.GetFloat(AlphaID);

    private void Start()
    {
      if (skeletonAnimation == null)
        skeletonAnimation = GetComponentInParent<SkeletonAnimation>();

      skeletonDataAsset = skeletonAnimation?.SkeletonDataAsset;
      foreach (var pair in slotAttachments)
        pair.skeletonDataAsset = skeletonDataAsset;

      ApplyMultiSlotFade();
    }

    private void ApplyMultiSlotFade()
    {
      if (skeletonAnimation == null || skeletonAnimation.Skeleton == null)
      {
        Debug.LogError("SkeletonAnimation or Skeleton is not assigned.");
        return;
      }

      _material = new Material(paintableMaterialTemplate);
      _material.SetTexture(MainTexID, skeletonMaterial.mainTexture);

      foreach (var pair in slotAttachments)
      {
        var slot = skeletonAnimation.Skeleton.FindSlot(pair.slotName);
        if (slot == null)
        {
          Debug.LogError($"Slot '{pair.slotName}' not found.");
          continue;
        }

        if (!string.IsNullOrEmpty(pair.attachmentName))
        {
          var attachment = skeletonAnimation.Skeleton.GetAttachment(pair.slotName, pair.attachmentName);
          if (attachment != null)
            slot.Attachment = attachment;
          else
            Debug.LogWarning($"Attachment '{pair.attachmentName}' not found in slot '{pair.slotName}'.");
        }

        skeletonAnimation.CustomSlotMaterials[slot] = _material;
      }

      SetSlotAlpha(hideOnStart ? 0f : 1f);
    }

    public void SetSlotAlpha(float alpha)
    {
      if (_material != null && _material.HasProperty(AlphaID))
      {
        _material.SetFloat(AlphaID, alpha);
        onAlphaChange?.Invoke(alpha);
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

    private void DisableSlot(string slotName)
    {
      skeletonAnimation.Skeleton.SetAttachment(slotName, null);
    }

    public void DisableAllSlot()
    {
      for (int i = 0; i < slotAttachments.Count; i++)
        DisableSlot(slotAttachments[i].slotName);
    }

    private void SetSlotMaterialToBase(string slotName)
    {
      Slot slot = skeletonAnimation.Skeleton.FindSlot(slotName);
      skeletonAnimation.CustomSlotMaterials[slot] = skeletonMaterial;
    }

    public void SetAllSlotMaterialToBase()
    {
      for (int i = 0; i < slotAttachments.Count; i++)
        SetSlotMaterialToBase(slotAttachments[i].slotName);
    }

    public void EnableAllSlot()
    {
      for (int i = 0; i < slotAttachments.Count; i++)
      {
        skeletonAnimation.Skeleton.SetAttachment(slotAttachments[i].slotName, slotAttachments[i].attachmentName);
      }
    }

    public void EnableSlot(string slotName)
    {
      string attachmentName = null;
      for (int i = 0; i < slotAttachments.Count; i++)
      {
        if (slotAttachments[i].slotName != slotName) continue;
        attachmentName = slotAttachments[i].attachmentName;
        break;
      }
      if (attachmentName == null) return;
      skeletonAnimation.Skeleton.SetAttachment(slotName, attachmentName);
    }

    public void RemoveSlotFromList(string slotName)
    {
      for (int i = slotAttachments.Count - 1; i >= 0; i--)
      {
        if (slotAttachments[i].slotName != slotName) continue;
        slotAttachments.RemoveAt(i);
        return;
      }
    }

    public void FadeIn(float? customDuration = null, Ease ease = Ease.Linear) => FadeTo(1f, customDuration, false, ease);
    public void FadeOut(float? customDuration = null, bool removeMaterial = false, Ease ease = Ease.Linear) => FadeTo(0f, customDuration, removeMaterial, ease);

    private void FadeTo(float targetAlpha, float? customDuration = null, bool removeMaterial = false, Ease ease = Ease.Linear)
    {
      if (_material == null)
      {
        Debug.LogWarning("Material not initialized. Applying fade setup.");
        ApplyMultiSlotFade();
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

    private IEnumerator FadeRoutine(bool removeMaterial, Ease ease)
    {
      while (_currentFadeTime < _fadeDurationInUse)
      {
        if (!IsPausing)
        {
          _currentFadeTime += Time.deltaTime;
          float t = Mathf.Clamp01(_currentFadeTime / _fadeDurationInUse);
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
        foreach (var pair in slotAttachments)
        {
          var slot = skeletonAnimation.Skeleton.FindSlot(pair.slotName);
          if (slot != null)
          {
            skeletonAnimation.CustomSlotMaterials.Remove(slot);
            slot.Attachment = null;
          }
        }
      }
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

      foreach (var pair in slotAttachments)
        pair.skeletonDataAsset = skeletonDataAsset;
    }

    private void OnValidate()
    {
      skeletonDataAsset = skeletonAnimation?.SkeletonDataAsset;
      foreach (var pair in slotAttachments)
        pair.skeletonDataAsset = skeletonDataAsset;
    }

    // [Button("Fade In")]
    private void TestFadeIn() => FadeTo(1f);

    // [Button("Fade Out")]
    private void TestFadeOut() => FadeTo(0f);

    // [Button("Pause Fade")]
    private void TestPauseFade() => PauseFade();

    // [Button("Resume Fade")]
    private void TestResumeFade() => ResumeFade();
#endif
  }
}
