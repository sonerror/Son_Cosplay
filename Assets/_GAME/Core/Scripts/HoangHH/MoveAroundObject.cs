using System;
using UnityEngine;

namespace HoangHH
{
    public class MoveAroundObject : H3MonoBehaviour
    {
        [SerializeField] private Transform aroundPoint;
        [SerializeField] private float moveSensitivity = 1f;
        [SerializeField] private float ignoreInputAtCenterRadius = 0.5f;
        [SerializeField] private bool manualBlockInteract;

        public event Action<float> OnPositionChanged;

        private bool _canInteract;
        private bool _isDragging;
        private LevelBase _level;
        private Camera _mainCam;
        private float _initialZ;
        private const float TOLERANCE = 0.0001f;

        private void Awake()
        {
            _mainCam = Camera.main;
            if (aroundPoint == null) aroundPoint = Tf;
            _initialZ = Tf.localPosition.z;
        }

        private void Start()
        {
            _canInteract = true;
            _level = LevelBase.Instance;
            if (_level)
            {
                _level.OnBlockPlayerInteractChanged += OnBlockInteract;
            }
            else
            {
                _canInteract = false;
            }
        }

        private void OnBlockInteract()
        {
            if (_level.IsAllowInteract)
            {
                _canInteract = true;
            }
            else
            {
                if (_isDragging) OnMouseUp();
                _canInteract = false;
            }
        }

        private void OnMouseDown()
        {
            if (!_canInteract || _isDragging) return;
            _isDragging = true;
        }

        private void OnMouseDrag()
        {
            if (!_canInteract || !_isDragging) return;
            if (manualBlockInteract) return;
            Vector2 currentMouseInput = _mainCam.ScreenToWorldPoint(Input.mousePosition);

            if (IsValidInput(currentMouseInput))
            {
                Vector2 oldPos = Tf.localPosition;
                Vector2 newPos = GetNewPosition(currentMouseInput);
                Tf.localPosition = new Vector3(newPos.x, newPos.y, _initialZ);
                float distanceSqr = (newPos - oldPos).sqrMagnitude;
                if (Math.Abs(distanceSqr) > TOLERANCE) OnPositionChanged?.Invoke(distanceSqr);
            }
        }

        private bool IsValidInput(Vector2 currentMouseInput) => 
            Vector2.Distance(currentMouseInput, aroundPoint.position) >= ignoreInputAtCenterRadius;

        private Vector2 GetNewPosition(Vector2 currentMouseInput)
        {
            Vector2 direction = (currentMouseInput - (Vector2)aroundPoint.position).normalized;
            float distance = Vector2.Distance(Tf.localPosition, aroundPoint.localPosition);
            return (Vector2)aroundPoint.localPosition + direction * distance * moveSensitivity;
        }

        public void OnMouseUp()
        {
            if (!_isDragging) return;
            _isDragging = false;
        }
        
        public void SetManualBlockInteract(bool canInteract)
        {
            manualBlockInteract = canInteract;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(aroundPoint.position, ignoreInputAtCenterRadius);
        }
#endif
    }
}