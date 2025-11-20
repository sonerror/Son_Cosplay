using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class LoopAnimRotateSeparatedAxis : MonoBehaviour
    {
        public bool isUseScaleTime = false;
        public Vector3 cycleDuration = Vector3.one;
        public AnimationCurve curveX = default;
        public AnimationCurve curveY = default;
        public AnimationCurve curveZ = default;

        public bool isRandomOffset = false;
        public Vector3 timeOffset = Vector3.zero;

        private Transform _transform;
        private Vector3 _originEuler;
        private Vector3 _targetEuler;

        private void Awake()
        {
            _transform = transform;
        }

        private void OnEnable()
        {
            _originEuler = transform.localEulerAngles;
            if (isRandomOffset)
            {
                timeOffset = new Vector3(
                    Random.Range(0f, cycleDuration.x), 
                    Random.Range(0f, cycleDuration.y),
                    Random.Range(0f, cycleDuration.z)
                    );
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
            _targetEuler = _originEuler;
            _targetEuler.x += curveX.Evaluate(timeX);
            _targetEuler.y += curveY.Evaluate(timeY);
            _targetEuler.z += curveZ.Evaluate(timeZ);
            _transform.localEulerAngles = _targetEuler;
        }

        private void OnDisable()
        {
            transform.localEulerAngles = _originEuler;
        }
    }
}