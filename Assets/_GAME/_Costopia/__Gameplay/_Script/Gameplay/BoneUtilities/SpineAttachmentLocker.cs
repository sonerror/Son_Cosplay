using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

[DisallowMultipleComponent]
public class SpineAttachmentLocker : MonoBehaviour
{
  [SerializeField] private SkeletonAnimation skeletonAnimation;
  [SerializeField] private List<SlotAttachmentPair> overrides = new List<SlotAttachmentPair>();

  // runtime caches (mutable)
  private class OverrideRuntime
  {
    public Slot slot;
    public string attachmentName;     // null means hide
    public Attachment attachmentRef;  // cached attachment if not null
  }

  // slotIndex -> runtime entry
  private readonly Dictionary<int, OverrideRuntime> _map = new Dictionary<int, OverrideRuntime>();
  // keep a list for iteration (values of _map)
  private readonly List<OverrideRuntime> _entries = new List<OverrideRuntime>();

  private bool _subscribed;
  private bool _needsRebuild;

  void Awake()
  {
    if (!skeletonAnimation) skeletonAnimation = GetComponent<SkeletonAnimation>();
    if (!skeletonAnimation)
    { // H3Log.LogError("SkeletonAnimation missing."); enabled = false; return; }
      RebuildFromSerializedList();   // initial build
    }

    void OnEnable()
    {
      Subscribe(true);
      ApplyOverrides();              // enforce from frame 0
    }

    void OnDisable() => Subscribe(false);

    void Update()
    {
      // If user edited the list in the Inspector during Play, rebuild lazily
      if (_needsRebuild)
      {
        _needsRebuild = false;
        RebuildFromSerializedList();
        ApplyOverrides();
      }
    }
  }

  private void Subscribe(bool on)
  {
    if (on && !_subscribed)
    {
      skeletonAnimation.UpdateComplete += OnUpdateComplete;
      _subscribed = true;
    }
    else if (!on && _subscribed)
    {
      skeletonAnimation.UpdateComplete -= OnUpdateComplete;
      _subscribed = false;
    }
  }

  private void OnUpdateComplete(ISkeletonAnimation _)
  {
    ApplyOverrides();
  }

  private void RebuildFromSerializedList()
  {
    _map.Clear();
    _entries.Clear();

    var skeleton = skeletonAnimation.Skeleton;
    var sda = skeletonAnimation.SkeletonDataAsset;
    foreach (var o in overrides) o.skeletonDataAsset = sda;

    for (int i = 0; i < overrides.Count; i++)
    {
      var o = overrides[i];
      var slot = skeleton.FindSlot(o.slotName);
      if (slot == null)
      {
        // H3Log.LogWarning($"Slot '{o.slotName}' not found.");
        continue;
      }

      string desired = string.IsNullOrEmpty(o.attachmentName) ? null : o.attachmentName;
      Attachment attachRef = null;

      if (desired != null)
      {
        attachRef = skeleton.GetAttachment(slot.Data.Index, desired);
        // H3Log.LogWarning($"Attachment '{desired}' not found on slot '{o.slotName}'.");
      }

      var entry = new OverrideRuntime
      {
        slot = slot,
        attachmentName = desired,
        attachmentRef = attachRef
      };

      _map[slot.Data.Index] = entry;
      _entries.Add(entry);
    }
  }

  private void ApplyOverrides()
  {
    if (_entries.Count == 0) return;

    for (int i = 0; i < _entries.Count; i++)
    {
      var e = _entries[i];
      if (e.slot == null) continue;

      if (e.attachmentName == null)
      {
        if (e.slot.Attachment != null)
          e.slot.Attachment = null;
      }
      else
      {
        if (e.slot.Attachment != e.attachmentRef)
          e.slot.Attachment = e.attachmentRef;
      }
    }
  }

  // ---------- Public Runtime API ----------

  /// <summary>
  /// Sets/updates an override for a slot. Pass null to hide the slot.
  /// Returns true if successful.
  /// </summary>
  public bool SetOverride(string slotName, string attachmentNameOrNull)
  {
    var skeleton = skeletonAnimation.Skeleton;
    var slot = skeleton.FindSlot(slotName);
    if (slot == null)
    {
      // H3Log.LogWarning($"Slot '{slotName}' not found.");
      return false;
    }
    return SetOverride(slot, attachmentNameOrNull);
  }

  /// <summary>
  /// Sets/updates an override for a slot (by Slot). Pass null to hide.
  /// </summary>
  public bool SetOverride(Slot slot, string attachmentNameOrNull)
  {
    if (slot == null) return false;

    var skel = skeletonAnimation.Skeleton;
    Attachment attachRef = null;
    string desired = string.IsNullOrEmpty(attachmentNameOrNull) ? null : attachmentNameOrNull;

    if (desired != null)
    {
      attachRef = skel.GetAttachment(slot.Data.Index, desired);
      if (attachRef == null)
      {
        // H3Log.LogWarning($"Attachment '{desired}' not found on slot '{slot.Data.Name}'.");
        // We still create an entry so we keep trying (in case skin changes later)
      }
    }

    OverrideRuntime entry;
    if (_map.TryGetValue(slot.Data.Index, out entry))
    {
      entry.attachmentName = desired;
      entry.attachmentRef = attachRef;
    }
    else
    {
      entry = new OverrideRuntime
      {
        slot = slot,
        attachmentName = desired,
        attachmentRef = attachRef
      };
      _map[slot.Data.Index] = entry;
      _entries.Add(entry);
    }

    // Enforce immediately
    ApplyOverrides();
    return true;
  }

  /// <summary>
  /// Removes an override for the slot (slot goes back to animation control).
  /// </summary>
  public bool RemoveOverride(string slotName)
  {
    var skeleton = skeletonAnimation.Skeleton;
    var slot = skeleton.FindSlot(slotName);
    if (slot == null) return false;
    return RemoveOverride(slot);
  }

  public bool RemoveOverride(Slot slot)
  {
    if (slot == null) return false;
    int key = slot.Data.Index;
    if (_map.TryGetValue(key, out var entry))
    {
      _map.Remove(key);
      _entries.Remove(entry);
      return true;
    }
    return false;
  }

  /// <summary>
  /// Clears all overrides (animation regains control of every slot).
  /// </summary>
  public void ClearOverrides()
  {
    _map.Clear();
    _entries.Clear();
  }

  /// <summary>
  /// Rebuild from the serialized 'overrides' list at runtime (useful if you modified it via code).
  /// </summary>
  public void RefreshFromInspectorList()
  {
    RebuildFromSerializedList();
    ApplyOverrides();
  }

#if UNITY_EDITOR
  // private void OnValidate() {
  //     // Keep Odin dropdowns working in Edit Mode
  //     SkeletonDataAsset dataAsset = skeletonAnimation ? skeletonAnimation.SkeletonDataAsset : null;
  //     foreach (var pair in overrides) pair.skeletonDataAsset = dataAsset;
  //
  //     // If we're in play mode and user edits the list/values, flag for rebuild next frame
  //     if (Application.isPlaying) _needsRebuild = true;
  // }
#endif
}
