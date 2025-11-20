using System;
using UnityEngine;

namespace HoangHH
{
    public class LoopRotateShake : H3MonoBehaviour
    {
        [SerializeField] private float startAngle = -2f;
        [SerializeField] private float endAngle = 2f;
        [SerializeField] private float timeRotate = 0.1f;
        private float _currentTime;
        
        private bool _isToEndAngle;
        private bool _isLevelBase;

        private void Start()
        {
            if (LevelBase.Instance) _isLevelBase = true;
        }

        private void Update()
        {
            if (_isLevelBase && LevelBase.Instance.IsAllowInteract) return;
            _currentTime += Time.deltaTime;
            if (_currentTime >= timeRotate)
            {
                _currentTime = 0;
                Tf.localEulerAngles = new Vector3(0, 0, _isToEndAngle ? startAngle : endAngle);
                _isToEndAngle = !_isToEndAngle;
            }
            else
            {
                float t = _currentTime / timeRotate;
                float angle = Mathf.Lerp(_isToEndAngle ? endAngle : startAngle, _isToEndAngle ? startAngle : endAngle, t); 
                Tf.localEulerAngles = new Vector3(0, 0, angle);
            }
        }
    }
}