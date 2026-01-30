using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace HoangHH.UI
{
    public class SnapPointUIEnableSlot : SnapPointUI
    {
        [Header("Slot Handle")]
        [SerializeField]
        protected SkeletonAnimation skeletonAnimation;

        [SerializeField] protected List<SlotAttachmentPair> disableSlot = new List<SlotAttachmentPair>();

        [SerializeField] protected List<SlotAttachmentPair> enableSlot = new List<SlotAttachmentPair>();

        private void Awake()
        {
            // Update data asset references in edit mode so Odin can resolve dropdowns
            SetDataAsset();
        }

        public override void OnSnap(SnapObjectUI snap)
        {
            base.OnSnap(snap);
            foreach (SlotAttachmentPair pair in disableSlot)
            {
                skeletonAnimation.Skeleton.SetAttachment(pair.slotName, null);
            }
            foreach (SlotAttachmentPair pair in enableSlot)
            {
                if (string.IsNullOrEmpty(pair.attachmentName)) pair.attachmentName = null;
                skeletonAnimation.Skeleton.SetAttachment(pair.slotName, pair.attachmentName);
            }
        }

        private void SetDataAsset()
        {
            SkeletonDataAsset dataAsset = skeletonAnimation?.SkeletonDataAsset;
            foreach (SlotAttachmentPair pair in disableSlot)
            {
                pair.skeletonDataAsset = dataAsset;
            }
            foreach (SlotAttachmentPair pair in enableSlot)
            {
                pair.skeletonDataAsset = dataAsset;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetDataAsset();
        }
#endif
    }
}