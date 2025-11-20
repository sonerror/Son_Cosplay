using System;
using UnityEngine;

namespace VinhLB
{
    public class AutoRotatingObject : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _rotationAxis;
        [SerializeField]
        private float _rotationAngle;
        [SerializeField]
        private Space _rotationSpace;
        
        private void LateUpdate()
        {
            float smoothAngle = _rotationAngle * Time.deltaTime;
            transform.Rotate(_rotationAxis, smoothAngle, _rotationSpace);
        }
    }
}