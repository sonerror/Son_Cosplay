using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SimpleCurve
{
    [System.Serializable]
    public struct Keyframe
    {
        public float time;
        public float value;

        public Keyframe(float time, float value)
        {
            this.time = time;
            this.value = value;
        }
    }

    public Keyframe[] keys;

    public SimpleCurve(params Keyframe[] keys)
    {
        this.keys = keys;
    }

    public float Evaluate(float time)
    {
        if (keys == null || keys.Length == 0)
            return 0f;

        if (keys.Length == 1)
            return keys[0].value;

        // Clamp time to the range of the curve
        time = Mathf.Clamp(time, keys[0].time, keys[keys.Length - 1].time);

        // Find the keyframes to interpolate between
        for (int i = 0; i < keys.Length - 1; i++)
        {
            if (time >= keys[i].time && time <= keys[i + 1].time)
            {
                float t = Mathf.InverseLerp(keys[i].time, keys[i + 1].time, time);
                return Mathf.Lerp(keys[i].value, keys[i + 1].value, t);
            }
        }

        return keys[keys.Length - 1].value;
    }
}