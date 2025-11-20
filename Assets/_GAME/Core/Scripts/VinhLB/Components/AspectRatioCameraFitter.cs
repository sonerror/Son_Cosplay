using System;
using UnityEngine;

namespace VinhLB
{
    [RequireComponent(typeof(Camera))]
    [ExecuteAlways]
    public class AspectRatioCameraFitter : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private Vector2 _targetAspectRatio = new Vector2(9, 16);
        [SerializeField]
        private Vector3 _cameraStartPosition;
        [SerializeField]
        private Transform _cameraTargetTransform;
        
        private readonly Vector2 _rectCenter = new Vector2(0.5f, 0.5f);
        
        private Vector2 _lastResolution;
        private Vector3 _cameraForwardVector;

        private void OnValidate()
        {
            _camera ??= GetComponent<Camera>();
            _cameraForwardVector = _cameraTargetTransform.position - _cameraStartPosition;
        }

        public void LateUpdate()
        {
            Vector2 currentScreenResolution = new Vector2(Screen.width, Screen.height);

            // Don't run all the calculations if the screen resolution has not changed
            if (_lastResolution != currentScreenResolution)
            {
                CalculateCameraRect(currentScreenResolution);
            }

            _lastResolution = currentScreenResolution;
        }

        private void CalculateCameraRect(Vector2 currentScreenResolution)
        {
            Vector2 normalizedAspectRatio = _targetAspectRatio / currentScreenResolution;
            Vector2 size = normalizedAspectRatio / Mathf.Max(normalizedAspectRatio.x, normalizedAspectRatio.y);
            // Debug.Log(size);
            // _camera.rect = new Rect(default, size) { center = _rectCenter };

            float value = size.x / size.y - 1;
            _camera.transform.position = _cameraStartPosition - _cameraForwardVector * value;
        }
    }
}