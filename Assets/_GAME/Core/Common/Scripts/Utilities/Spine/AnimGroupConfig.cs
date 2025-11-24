using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Spine
{
    /// <summary>
    /// Config specific animations each track
    /// </summary>
    [System.Serializable]
    public class AnimGroupConfig
    {
        public float timeScale = 1f;
        [SpineAnimation] public string[] animations = null;
    }

    [System.Serializable]
    public class AnimAssetGroupConfig
    {
        public float timeScale = 1f;
        public AnimationReferenceAsset[] animations = null;
    }
}