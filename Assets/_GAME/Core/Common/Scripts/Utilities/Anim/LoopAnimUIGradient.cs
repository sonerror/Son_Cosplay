using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class LoopAnimUIGradient : MonoBehaviour
    {
        public float cycleDuration = 1f;
        public Gradient gradient = default;
        public bool isUseUnscaledTime = false;
        public bool isMultiplyOriginalColor = false;
        public float timeOffset = 0f;
        private Graphic _graphic;
        private Color _originColor;

        private void Awake()
        {
            _graphic = GetComponent<Graphic>();
            _originColor = _graphic.color;
        }

        private void OnEnable()
        {
            if (timeOffset < 0) timeOffset = UnityEngine.Random.Range(0f, cycleDuration);
        }

        // Update is called once per frame
        void Update()
        {
            Color colorGradient = gradient.Evaluate(Mathf.Repeat(((isUseUnscaledTime ? Time.unscaledTime : Time.time) + timeOffset) / cycleDuration, 1f));
            if (isMultiplyOriginalColor)
            {
                _graphic.color = _originColor * colorGradient;
            }
            else
            {
                _graphic.color = colorGradient;
            }
        }
    }
}