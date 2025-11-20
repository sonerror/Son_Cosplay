using UnityEngine;
using UnityEngine.Events;

namespace Utilities
{
    public class Loop01Value : MonoBehaviour
    {
        [Header("Config")]
        public bool isUseScaleTime = false;
        public float cycleDuration = 1f;
        public AnimationCurve curveValue = AnimationCurve.Linear(0, 0, 1, 1);
        public float timeOffset = -1f;
        public float multiplier = 1f;

        [Header("Output")]
        [Range(0f, 1f)] public float value01; // current value
        public UnityEvent<float> onValueChanged; // event callback

        private float _lastValue;

        private void OnEnable()
        {
            if (timeOffset < 0f)
                timeOffset = Random.Range(0f, cycleDuration);

            _lastValue = -999f; // force update
        }

        private void Update()
        {
            float time = isUseScaleTime
                ? (Time.time + timeOffset) % cycleDuration / cycleDuration
                : (Time.unscaledTime + timeOffset) % cycleDuration / cycleDuration;

            value01 = Mathf.Clamp01(curveValue.Evaluate(time) * multiplier);

            if (!Mathf.Approximately(value01, _lastValue))
            {
                _lastValue = value01;
                onValueChanged?.Invoke(value01);
            }
        }

        private void OnDisable()
        {
            value01 = 0f;
            onValueChanged?.Invoke(value01);
        }
    }
}