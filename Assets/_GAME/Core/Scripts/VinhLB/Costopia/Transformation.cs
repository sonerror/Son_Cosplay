using System;
using UnityEngine;

namespace VinhLB
{
    public class Transformation : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _targetLocalEulerAngles;

        private Vector3 _initialLocalEulerAngles;

        private void Awake()
        {
            _initialLocalEulerAngles = transform.localEulerAngles;
        }

        public void SetTargetLocalRotation()
        {
            transform.localEulerAngles = _targetLocalEulerAngles;
        }

        public void ResetLocalRotation()
        {
            transform.localEulerAngles = _initialLocalEulerAngles;
        }
    }
}