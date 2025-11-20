using System;
using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
    public class AutoMovingObject : MonoBehaviour
    {
        [SerializeField]
        private Transform _targetTransform;
        [SerializeField]
        private List<Transform> _pointList;
        [SerializeField]
        private int _startIndex;
        [SerializeField]
        private float _speed;

        private int _currentIndex;
        
        private void Start()
        {
            _currentIndex = _startIndex;

            _targetTransform.localPosition = _pointList[_currentIndex].localPosition;
        }

        private void Update()
        {
            if (Vector3.Distance(_targetTransform.localPosition, _pointList[_currentIndex].localPosition) < 0.1f)
            {
                _currentIndex = (_currentIndex + 1) % _pointList.Count;
            }

            Vector3 moveDirection = (_pointList[_currentIndex].localPosition - _targetTransform.localPosition).normalized;
            _targetTransform.Translate(moveDirection * _speed * Time.deltaTime, Space.Self);
        }
    }
}