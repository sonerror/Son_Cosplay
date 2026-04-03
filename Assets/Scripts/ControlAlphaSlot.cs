using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

public class ControlAlphaSlot : MonoBehaviour
{
  public Character character;

  [SerializeField, Range(0.1f, 10f)]
  private float speedLerp = 2f;

  [SerializeField] private SlotAttachmentPairList slotDataList;
  [SerializeField] private List<TriggerWithCertainCollider> triggerColliders = new List<TriggerWithCertainCollider>();

  public UnityEvent OnFinish;

  private readonly List<Slot> _cachedSlots = new List<Slot>();
  private int _triggeredCount;
  private int _totalTriggers;
  private float _targetAlpha;
  private float _currentAlpha;

  // ─── Setup ────────────────────────────────────────────────────────────────

  [Button]
  public void SetReady(Character charRef)
  {
    character = charRef;
    _cachedSlots.Clear();

    foreach (var pair in slotDataList.pairs)
    {
      character.TurnSlotAttachment(pair.slotName, pair.attachmentName);
      var slot = character.SkeletonAnimation.Skeleton.FindSlot(pair.slotName);
      if (slot == null) continue;

      _cachedSlots.Add(slot);
      slot.A = 0f;
    }

    _totalTriggers = triggerColliders.Count;

    for (int i = 0; i < _totalTriggers; i++)
    {
      var trigger = triggerColliders[i];
      trigger.gameObject.SetActive(true);
      trigger.OnTriggerEvent.RemoveAllListeners();
      trigger.OnTriggerEvent.AddListener(() => OnTriggerHit(trigger));
    }

    UnsubscribeSpineUpdate();
    character.SkeletonAnimation.UpdateLocal += UpdateSpineAlpha;

    _triggeredCount = 0;
    _currentAlpha = 0f;
    _targetAlpha = 0f;
    enabled = true;
  }

  // ─── Trigger ──────────────────────────────────────────────────────────────

  private void OnTriggerHit(TriggerWithCertainCollider trigger)
  {
    trigger.gameObject.SetActive(false);
    IncreaseAlpha();
  }

  public void IncreaseAlpha()
  {
    if (_totalTriggers == 0) return;

    _triggeredCount = Mathf.Min(_triggeredCount + 1, _totalTriggers);
    _targetAlpha = (float)_triggeredCount / _totalTriggers;
  }

  // ─── Update ───────────────────────────────────────────────────────────────

  private void Update()
  {
    if (_currentAlpha >= _targetAlpha) return;

    _currentAlpha = Mathf.MoveTowards(_currentAlpha, _targetAlpha, Time.deltaTime * speedLerp);

    if (_currentAlpha < 1f) return;

    _currentAlpha = 1f;
    OnFinish?.Invoke();
    Finish();
  }

  private void UpdateSpineAlpha(ISkeletonAnimation _)
  {
    float alpha = _currentAlpha;
    for (int i = 0; i < _cachedSlots.Count; i++)
      _cachedSlots[i].A = alpha;
  }

  // ─── Cleanup ──────────────────────────────────────────────────────────────

  private void Finish()
  {
    UnsubscribeSpineUpdate();
    enabled = false;
  }

  private void UnsubscribeSpineUpdate()
  {
    if (character != null && character.SkeletonAnimation != null)
      character.SkeletonAnimation.UpdateLocal -= UpdateSpineAlpha;
  }

  private void OnDestroy() => UnsubscribeSpineUpdate();

  // ─── Editor ───────────────────────────────────────────────────────────────

  [Button]
  private void FetchTriggerColliders()
  {
    triggerColliders = new List<TriggerWithCertainCollider>(
        GetComponentsInChildren<TriggerWithCertainCollider>(true));
  }
}