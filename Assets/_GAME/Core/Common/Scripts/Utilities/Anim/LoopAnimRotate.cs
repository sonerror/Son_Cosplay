using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class LoopAnimRotate : MonoBehaviour
    {
        public bool isUseScaleTime = false;
        public float cycleDuration = 1f;
        public float multiplier = 1f;
        public AnimationCurve curveRotate = default;

        public float timeOffset = -1f;

        private Transform _transform;
        private float _originEulerZ;
        private Vector3 _targetEuler;

        private void Awake()
        {
            _transform = transform;
        }

        private void OnEnable()
        {
            _originEulerZ = _transform.localEulerAngles.z;
            if (timeOffset < 0f) timeOffset = Random.Range(0f, cycleDuration);
            _targetEuler = _transform.localEulerAngles;
        }

        // Update is called once per frame
        void Update()
        {
            float time;
            if (isUseScaleTime)
            {
                time = (Time.time + timeOffset) % cycleDuration / cycleDuration;
            }
            else
            {
                time = (Time.unscaledTime + timeOffset) % cycleDuration / cycleDuration;
            }
            _targetEuler.z = _originEulerZ + curveRotate.Evaluate(time) * multiplier;
            _transform.localEulerAngles = _targetEuler;
        }

        private void OnDisable()
        {
            _targetEuler.z = _originEulerZ;
            transform.localEulerAngles = _targetEuler;
        }
    }
}