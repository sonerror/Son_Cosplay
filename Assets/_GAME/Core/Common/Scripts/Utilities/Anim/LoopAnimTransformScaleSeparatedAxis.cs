using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class LoopAnimTransformScaleSeparatedAxis : MonoBehaviour
    {
        public bool isUseScaleTime = false;
        public Vector3 cycleDuration = Vector3.one;
        public AnimationCurve curveX = default;
        public AnimationCurve curveY = default;
        public AnimationCurve curveZ = default;

        public bool isRandomOffset = false;
        public Vector3 timeOffset = Vector3.zero;

        private Transform _transform;
        private Vector3 _originScale;
        private Vector3 _targetScale;

        private void Awake()
        {
            _transform = transform;
        }

        private void OnEnable()
        {
            _originScale = transform.localScale;
            if (isRandomOffset)
            {
                timeOffset = new Vector3(
                    Random.Range(0f, cycleDuration.x), 
                    Random.Range(0f, cycleDuration.y),
                    Random.Range(0f, cycleDuration.z));
            }
        }

        // Update is called once per frame
        void Update()
        {
            float timeX, timeY, timeZ;
            if (isUseScaleTime)
            {
                timeX = (Time.time + timeOffset.x) % cycleDuration.x / cycleDuration.x;
                timeY = (Time.time + timeOffset.y) % cycleDuration.y / cycleDuration.y;
                timeZ = (Time.time + timeOffset.z) % cycleDuration.z / cycleDuration.z;
            }
            else
            {
                timeX = (Time.unscaledTime + timeOffset.x) % cycleDuration.x / cycleDuration.x;
                timeY = (Time.unscaledTime + timeOffset.y) % cycleDuration.y / cycleDuration.y;
                timeZ = (Time.unscaledTime + timeOffset.z) % cycleDuration.z / cycleDuration.z;
            }
            _targetScale = _originScale;
            _targetScale.x *= curveX.Evaluate(timeX);
            _targetScale.y *= curveY.Evaluate(timeY);
            _targetScale.z *= curveZ.Evaluate(timeZ);
            _transform.localScale = _targetScale;
        }

        private void OnDisable()
        {
            transform.localScale = _originScale;
        }
    }
}