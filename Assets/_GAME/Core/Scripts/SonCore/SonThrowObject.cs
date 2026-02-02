using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace sonnv
{
    public class SonThrowObject : SonMonoBehaviour,
        IPointerDownHandler,
        IDragHandler,
        IPointerUpHandler
    {
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Collider2D col;
        public Collider2D Col => col;
        [SerializeField] private Rigidbody2D rb;

        [SerializeField] private bool hasAlternativeSprite;
        [SerializeField] private Sprite alternativeSprite;

        [SerializeField] private float distanceChangeToThrow;
        [SerializeField] private AudioData pickUpSound;
        [SerializeField] private Vector3 offsetOnPick;

        public UnityEvent onMouseDown;
        public UnityEvent onMouseUp;
        public UnityEvent onRemoveItem;
        public UnityEvent onDone;


        private bool _canDestroy = true;
        private bool _isDone;
        private bool _isDragging;

        private Sprite _initialSprite;
        private Vector3 _startPos;
        private int layerCurrent;

        private Camera _mainCamera;

        public event Action OnRemoveItemEv;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _initialSprite = sr.sprite;
            layerCurrent = sr.sortingOrder;
        }

        /* ===================== POINTER EVENTS ===================== */

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isDone || _isDragging) return;

            _startPos = Tf.position;
            _startPos.z = 0;
            _isDragging = true;


            if (pickUpSound.clip != null)
                SoundManager.PlaySFX(pickUpSound.clip, pickUpSound.volume);

            onMouseDown.Invoke();

            if (hasAlternativeSprite)
                sr.sprite = alternativeSprite;

            sr.sortingOrder = 30;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDone || !_isDragging) return;

            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;
            worldPos += offsetOnPick;

            Tf.position = worldPos;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isDone || !_isDragging) return;

            _isDragging = false;

            sr.sortingOrder = layerCurrent;

            if (!_canDestroy)
                return;

            if (DistanceToInSqrVec2(_startPos) > distanceChangeToThrow)
            {
                Tf.SetParent(null);

                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 2;

                _isDone = true;

                onRemoveItem.Invoke();
                OnRemoveItemEv?.Invoke();

                sr.DOFade(0, 1f).OnComplete(() =>
                {
                    onDone?.Invoke();
                    Destroy(gameObject);
                });
            }
            else
            {
                Tf.position = _startPos;

                if (hasAlternativeSprite)
                    sr.sprite = _initialSprite;

                onMouseUp.Invoke();
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Get Reference")]
        private void GetReference()
        {
            sr = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();
            rb = GetComponent<Rigidbody2D>();
        }
#endif
    }
}
