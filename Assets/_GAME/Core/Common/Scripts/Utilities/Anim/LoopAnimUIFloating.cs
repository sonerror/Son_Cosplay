using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class LoopAnimUIFloating : MonoBehaviour
    {
        public Vector2 cycleDuration = Vector2.one;
        public float positionCurveMul = 1f;
        public AnimationCurve positionCurveX = default;
        public AnimationCurve positionCurveY = default;
        public bool isUseScaleTime = false;
        public bool isRandomTimeOffset = false;
        public Vector2 timeOffset = Vector2.zero;

        private RectTransform _rectTransform;
        private Vector2 _originAnchoredPos;

        private void OnEnable()
        {
            _rectTransform = GetComponent<RectTransform>();
            _originAnchoredPos = _rectTransform.anchoredPosition;
            if (isRandomTimeOffset)
            {
                timeOffset = new Vector2(
                    UnityEngine.Random.Range(0f, cycleDuration.x),
                    UnityEngine.Random.Range(0f, cycleDuration.y)
                    );
            }
        }

        // Update is called once per frame
        void Update()
        {
            float time = isUseScaleTime ? Time.time : Time.unscaledTime;
            Vector2 targetAnchoredPos = _originAnchoredPos;
            targetAnchoredPos.x += positionCurveX.Evaluate(Mathf.Repeat((time + timeOffset.x) / cycleDuration.x, 1f)) * positionCurveMul;
            targetAnchoredPos.y += positionCurveY.Evaluate(Mathf.Repeat((time + timeOffset.y) / cycleDuration.y, 1f)) * positionCurveMul;
            _rectTransform.anchoredPosition = targetAnchoredPos;
        }

        private void OnDisable()
        {
            _rectTransform.anchoredPosition = _originAnchoredPos;
        }
    }
}