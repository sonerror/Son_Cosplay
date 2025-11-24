using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    [RequireComponent(typeof(Graphic))]
    public class LoopAnimUIFade : MonoBehaviour
    {
        public float cycleDuration = 1f;
        public AnimationCurve alphaCurve = default;
        public bool isUseUnscaledTime = false;
        public float timeOffset = 0f;
        private Graphic _graphic;
        private Color _color;

        private void OnEnable()
        {
            _graphic = GetComponent<Graphic>();
            _color = _graphic.color;
            if (timeOffset < 0f) timeOffset = UnityEngine.Random.Range(0f, cycleDuration);
        }

        // Update is called once per frame
        void Update()
        {
            _color.a = alphaCurve.Evaluate(Mathf.Repeat(((isUseUnscaledTime ? Time.unscaledTime : Time.time) + timeOffset) / cycleDuration, 1f));
            _graphic.color = _color;
        }
    }
}