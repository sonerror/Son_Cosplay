using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

[DisallowMultipleComponent]
public class SpineAttachmentLocker : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    [SerializeField] private List<SlotAttachmentPair> overrides = new List<SlotAttachmentPair>();

    private class OverrideRuntime
    {
        public bool isActive;
        public Slot slot;
        public string attachmentName;
        public Attachment attachmentRef;
    }

    private readonly Dictionary<int, OverrideRuntime> _map = new Dictionary<int, OverrideRuntime>();
    private readonly List<OverrideRuntime> _entries = new List<OverrideRuntime>();
    private bool _subscribed;

    void Awake()
    {
        if (!skeletonAnimation) skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (!skeletonAnimation) { enabled = false; return; }
        RebuildFromSerializedList();
    }

    void OnEnable()
    {
        Subscribe(true);
        ApplyOverrides();
    }

    void OnDisable() => Subscribe(false);

    private void Subscribe(bool on)
    {
        if (on && !_subscribed)
        {
            skeletonAnimation.UpdateLocal += OnUpdateLocal;
            _subscribed = true;
        }
        else if (!on && _subscribed)
        {
            skeletonAnimation.UpdateLocal -= OnUpdateLocal;
            _subscribed = false;
        }
    }

    private void OnUpdateLocal(ISkeletonAnimation _)
    {
        ApplyOverrides();
    }

    private void RebuildFromSerializedList()
    {
        _map.Clear();
        _entries.Clear();

        var skeleton = skeletonAnimation.Skeleton;
        if (skeleton == null) return;

        for (int i = 0; i < overrides.Count; i++)
        {
            var o = overrides[i];
            var slot = skeleton.FindSlot(o.slotName);
            if (slot == null) continue;

            string desired = string.IsNullOrEmpty(o.attachmentName) ? null : o.attachmentName;
            Attachment attachRef = null;

            if (desired != null)
            {
                attachRef = skeleton.GetAttachment(slot.Data.Index, desired);
            }

            var entry = new OverrideRuntime
            {
                isActive = true,
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
        for (int i = 0; i < _entries.Count; i++)
        {
            var e = _entries[i];

            if (!e.isActive || e.slot == null) continue;

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

    public bool SetOverride(string slotName, string attachmentNameOrNull)
    {
        var skeleton = skeletonAnimation.Skeleton;
        var slot = skeleton.FindSlot(slotName);
        return SetOverride(slot, attachmentNameOrNull);
    }

    public bool SetOverride(Slot slot, string attachmentNameOrNull)
    {
        if (slot == null) return false;

        var skel = skeletonAnimation.Skeleton;
        Attachment attachRef = null;
        string desired = string.IsNullOrEmpty(attachmentNameOrNull) ? null : attachmentNameOrNull;

        if (desired != null)
        {
            attachRef = skel.GetAttachment(slot.Data.Index, desired);
        }

        if (_map.TryGetValue(slot.Data.Index, out var entry))
        {
            entry.isActive = true;
            entry.attachmentName = desired;
            entry.attachmentRef = attachRef;
        }
        else
        {
            entry = new OverrideRuntime
            {
                isActive = true,
                slot = slot,
                attachmentName = desired,
                attachmentRef = attachRef
            };
            _map[slot.Data.Index] = entry;
            _entries.Add(entry);
        }

        ApplyOverrides();
        return true;
    }

    public bool RemoveOverride(string slotName)
    {
        var slot = skeletonAnimation.Skeleton.FindSlot(slotName);
        return RemoveOverride(slot);
    }

    public bool RemoveOverride(Slot slot)
    {
        if (slot == null) return false;

        if (_map.TryGetValue(slot.Data.Index, out var entry))
        {
            entry.isActive = false;
            return true;
        }
        return false;
    }

    public void ClearOverrides()
    {
        for (int i = 0; i < _entries.Count; i++)
        {
            _entries[i].isActive = false;
        }
    }
}