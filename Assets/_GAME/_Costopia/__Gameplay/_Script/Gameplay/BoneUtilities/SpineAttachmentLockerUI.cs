using System.Collections.Generic;
using Spine;
using Spine.Unity;
// Assuming SlotAttachmentPair is defined/used here or in HoangHH
using UnityEngine;

// Assuming SlotAttachmentPair is a serializable class that holds slotName and attachmentName,
// and potentially a reference to SkeletonDataAsset for editor use.
// If SlotAttachmentPair is custom to your project, ensure it's accessible.
// For this example, I'll assume it's available or part of a common example/utility.

[DisallowMultipleComponent]
public class SpineAttachmentLockerUI : MonoBehaviour
{
  // The main difference: referencing SkeletonGraphic instead of SkeletonAnimation
  [SerializeField] private SkeletonGraphic skeletonGraphic;
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
    // Look for SkeletonGraphic component
    if (!skeletonGraphic) skeletonGraphic = GetComponent<SkeletonGraphic>();
    if (!skeletonGraphic)
    { // H3Log.LogError("SkeletonGraphic missing."); enabled = false; return; }

      // Ensure the skeleton is initialized before initial build
      if (skeletonGraphic.Skeleton == null)
      {
        skeletonGraphic.Initialize(false);
      }

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
    // SkeletonGraphic uses onLateUpdate event for updates
    if (on && !_subscribed)
    {
      skeletonGraphic.OnPostProcessVertices += OnPostProcessVertices;
      _subscribed = true;
    }
    else if (!on && _subscribed)
    {
      skeletonGraphic.OnPostProcessVertices -= OnPostProcessVertices;
      _subscribed = false;
    }
  }

  // SkeletonGraphic's equivalent of UpdateComplete/LateUpdate
  private void OnPostProcessVertices(MeshGeneratorBuffers buffers)
  {
    ApplyOverrides();
  }

  private void RebuildFromSerializedList()
  {
    _map.Clear();
    _entries.Clear();

    // Check for null skeleton (might not be initialized yet)
    if (skeletonGraphic.Skeleton == null) return;

    var skeleton = skeletonGraphic.Skeleton;
    var sda = skeletonGraphic.SkeletonDataAsset;
    // Assuming SlotAttachmentPair needs the SkeletonDataAsset for editor features
    foreach (var o in overrides) o.skeletonDataAsset = sda;

    for (int i = 0; i < overrides.Count; i++)
    {
      var o = overrides[i];
      var slot = skeleton.FindSlot(o.slotName);
      if (slot == null)
      {
        // H3Log.LogWarning($"Slot '{o.slotName}' not found on SkeletonGraphic '{gameObject.name}'.");
        continue;
      }

      string desired = string.IsNullOrEmpty(o.attachmentName) ? null : o.attachmentName;
      Attachment attachRef = null;

      if (desired != null)
      {
        // Use GetAttachment which correctly finds attachments in the active skin
        attachRef = skeleton.GetAttachment(slot.Data.Index, desired);
        // H3Log.LogWarning($"Attachment '{desired}' not found on slot '{o.slotName}' in current skin.");
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

    // Ensure skeleton is not null before applying (safety check)
    if (skeletonGraphic.Skeleton == null) return;

    for (int i = 0; i < _entries.Count; i++)
    {
      var e = _entries[i];
      if (e.slot == null) continue;

      if (e.attachmentName == null)
      {
        // Hide slot
        if (e.slot.Attachment != null)
          e.slot.Attachment = null;
      }
      else
      {
        // Set attachment
        if (e.slot.Attachment != e.attachmentRef)
          e.slot.Attachment = e.attachmentRef;
      }
    }

    // Mark for vertex regeneration after changing attachments
    // SkeletonGraphic needs to be told it needs to redraw, unlike SkeletonAnimation's UpdateComplete
    if (_entries.Count > 0)
    {
      // skeletonGraphic.SetVerticesDirty();
    }
  }

  // ---------- Public Runtime API ----------

  /// <summary>
  /// Sets/updates an override for a slot. Pass null to hide the slot.
  /// Returns true if successful.
  /// </summary>
  public bool SetOverride(string slotName, string attachmentNameOrNull)
  {
    if (skeletonGraphic.Skeleton == null) return false;
    var skeleton = skeletonGraphic.Skeleton;
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
    if (slot == null || skeletonGraphic.Skeleton == null) return false;

    var skel = skeletonGraphic.Skeleton;
    Attachment attachRef = null;
    string desired = string.IsNullOrEmpty(attachmentNameOrNull) ? null : attachmentNameOrNull;

    if (desired != null)
    {
      attachRef = skel.GetAttachment(slot.Data.Index, desired);
      if (attachRef == null)
      {
        // H3Log.LogWarning($"Attachment '{desired}' not found on slot '{slot.Data.Name}' in current skin.");
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
    if (skeletonGraphic.Skeleton == null) return false;
    var skeleton = skeletonGraphic.Skeleton;
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
      // Apply immediately to restore the original attachment
      ApplyOverrides();
      return true;
    }
    return false;
  }

  /// <summary>
  /// Clears all overrides (animation regains control of every slot).
  /// </summary>
  public void ClearOverrides()
  {
    if (_entries.Count == 0) return;
    _map.Clear();
    _entries.Clear();
    // Force a redraw so slots revert to animation state
    if (skeletonGraphic != null) skeletonGraphic.SetVerticesDirty();
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
  //     SkeletonDataAsset dataAsset = skeletonGraphic ? skeletonGraphic.SkeletonDataAsset : null;
  //     foreach (var pair in overrides) pair.skeletonDataAsset = dataAsset;
  //
  //     // If we're in play mode and user edits the list/values, flag for rebuild next frame
  //     if (Application.isPlaying) _needsRebuild = true;
  // }
#endif
}