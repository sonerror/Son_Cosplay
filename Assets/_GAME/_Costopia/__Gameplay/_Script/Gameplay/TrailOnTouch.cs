using UnityEngine;

namespace Costopia.Gameplay
{
    public class TrailOnTouch : MonoBehaviour
    {
        private TrailRenderer trailRenderer;
        private Camera mainCamera;
        private bool isDragging = false;

        private void Awake()
        {
            mainCamera = Camera.main;
            trailRenderer = GetComponent<TrailRenderer>();
            if (trailRenderer != null)
                trailRenderer.enabled = false;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;

                CancelInvoke(nameof(DisableTrail));

                Vector3 startWorldPos = GetWorldPosition(Input.mousePosition);

                // Move first, before enabling trail
                transform.position = startWorldPos;

                if (trailRenderer != null)
                {
                    trailRenderer.enabled = false;  // Turn off before clearing
                    trailRenderer.Clear();          // Clear to reset points
                    trailRenderer.enabled = true;   // Re-enable clean
                }
            }

            if (Input.GetMouseButton(0) && isDragging)
            {
                UpdatePosition(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;

                if (trailRenderer != null)
                {
                    Invoke(nameof(DisableTrail), trailRenderer.time); // Let it fade out
                }
            }
        }

        private void DisableTrail()
        {
            if (trailRenderer != null)
                trailRenderer.enabled = false;
        }

        private void UpdatePosition(Vector3 screenPosition)
        {
            Vector3 worldPos = GetWorldPosition(screenPosition);
            transform.position = worldPos;
        }

        private Vector3 GetWorldPosition(Vector3 screenPosition)
        {
            Vector3 pos = mainCamera.ScreenToWorldPoint(screenPosition);
            pos.z = 0f;
            return pos;
        }
    }
}
