using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace HaoNT
{
    public class DraggableByHand : MonoBehaviour
    {
        [SerializeField] private bool isActive = true;
        [SerializeField] private UnityEvent startDragEvents, draggingEvents, endDragEvents, completeRewindEvents;
        public UnityEvent OnStartDragEvent => startDragEvents;
        public UnityEvent OnEndDragEvent => endDragEvents;
        private bool isDragging = false;
        private LevelBase _level;
        private Camera mainCamera;
            
        public void SetActive(bool active) => this.isActive = active;
        private void Awake()
        {
            _level = LevelBase.Instance;
            mainCamera = Camera.main;
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                MouseDown();
            }

            if (Input.GetMouseButton(0))
            {
                MouseDrag();
            }

            if (Input.GetMouseButtonUp(0))
            {
                MouseUp();
            }
        }

        private void MouseDown()
        {
            if (!isActive || !_level.IsAllowInteract) return;

            transform.DOKill();
            isDragging = true;
            startDragEvents?.Invoke();
        }

        private Vector2 newPostision;

        private void MouseDrag()
        {
            if (!_level.IsAllowInteract) return;

            if (isDragging)
            {
                if (!isActive)
                {
                    CancelDragging();
                }

                newPostision = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                transform.position = newPostision;
                draggingEvents?.Invoke();
            }
        }

        private void MouseUp()
        {
            if (!isActive) return;
            if (!isDragging) return;
            transform.localScale = Vector3.one;
            isDragging = false;
            endDragEvents?.Invoke();
        }

        public void CancelDragging()
        {
            isDragging = true;
            MouseUp();
            isActive = false;
        }
    }
}