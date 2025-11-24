using UnityEngine;
using Random = UnityEngine.Random;

namespace Utilities
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class LoopAnimSpriteRendererFade : MonoBehaviour
    {
        public float cycleDuration = 1f;
        public AnimationCurve alphaCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float timeOffset;
        public bool resetOnDisable = true;

        private SpriteRenderer _renderer;
        private Color _color;
        private float _localTime;

        private void OnEnable()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _color = _renderer.color;

            if (timeOffset < 0f)
                timeOffset = Random.Range(0f, cycleDuration);

            _localTime = 0f; // reset timer when enabled
            UpdateAlpha(0f);
        }

        private void Update()
        {
            _localTime += Time.deltaTime;
            float normalizedTime = Mathf.Repeat((_localTime + timeOffset) / cycleDuration, 1f);
            UpdateAlpha(normalizedTime);
        }

        private void UpdateAlpha(float t)
        {
            _color.a = alphaCurve.Evaluate(t);
            _renderer.color = _color;
        }

        private void OnDisable()
        {
            if (resetOnDisable && _renderer != null)
            {
                timeOffset = 0f;
                UpdateAlpha(0f);
            }
        }
    }
}