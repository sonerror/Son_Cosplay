using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HoangHH
{
    public class OnTransformMoveInAffectZone : H3MonoBehaviour
    {
        public bool blockCheck;

        [Header("Zone")] 
        [SerializeField] private bool useRadiusZone;
        [HideIf("useRadiusZone")] [SerializeField] private Rect affectZone;
        [ShowIf("useRadiusZone")] [SerializeField] private Transform target;
        [ShowIf("useRadiusZone")] [SerializeField] private float radius;

        [Header("Events")] public UnityEvent onMovingInZone;
        public UnityEvent onNotMovingOrOutZone;

        [Header("Movement Settings")]
        [Tooltip("Minimum per-frame movement (world units) to count as 'moving'.")]
        [SerializeField]
        private float minMoveDistance = 0.01f;

        private bool _hasLastPos;

        // State
        private bool _isMovingInZone;
        private Vector2 _lastPos2D;

        private float MinMoveSqr => minMoveDistance * minMoveDistance;

        private void LateUpdate()
        {
            if (blockCheck) return;
            Vector2 pos2D = Tf.position;
            bool inZone = GetInZoneState(pos2D);

            bool moving = false;
            if (_hasLastPos)
            {
                Vector2 delta = pos2D - _lastPos2D;
                moving = delta.sqrMagnitude >= MinMoveSqr;
            }

            // TRUE only when both inside AND moving
            SetMovingInZone(inZone && moving);

            _lastPos2D = pos2D;
            _hasLastPos = true;
        }

        private bool GetInZoneState(Vector2 pos2D)
        {
            if (useRadiusZone)
            {
                return Vector2.Distance(pos2D, target.position) <= radius;
            }
            return affectZone.Contains(pos2D);
        }

        private void OnDisable()
        {
            if (_isMovingInZone)
                SetMovingInZone(false);

            _hasLastPos = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            if (useRadiusZone)
            {
                Gizmos.DrawWireSphere(target.position, radius);
            }
            else
            {
                // Simple gizmo to visualize the Rect
                Vector3 center = new(affectZone.x + affectZone.width * 0.5f, affectZone.y + affectZone.height * 0.5f,
                    Tf ? Tf.position.z : 0f);
                Vector3 size = new(affectZone.width, affectZone.height, 0f);
                Gizmos.DrawWireCube(center, size);
            }
        }
#endif

        private void SetMovingInZone(bool movingInZone)
        {
            if (_isMovingInZone == movingInZone) return;
            _isMovingInZone = movingInZone;

            if (movingInZone)
                onMovingInZone?.Invoke();
            else
                onNotMovingOrOutZone?.Invoke();
        }

        // Optional helper if you want to adjust at runtime
        public void SetZone(Rect newZone)
        {
            affectZone = newZone;
        }
    }
}