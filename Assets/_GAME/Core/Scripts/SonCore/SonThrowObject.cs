using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;
using System.Collections.Generic;

namespace sonnv
{
    public class SonThrowObject : SonMonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private SpriteRenderer sr;

        [SerializeField] private Collider2D col;
        [SerializeField] private Rigidbody2D rb;

        [Header("Properties")]
        [SerializeField]
        private bool hasAlternativeSprite;

        [ShowIf("hasAlternativeSprite")]
        [SerializeField]
        private Sprite alternativeSprite;

        [SerializeField] private float distanceChangeToThrow;
        [SerializeField] private AudioData pickUpSound;
        [SerializeField] private Vector3 offsetOnPick;

        [Header("Events")][SerializeField] public UnityEvent onMouseDown;

        [SerializeField] private UnityEvent onMouseUp;
        [SerializeField] private UnityEvent onRemoveItem;
        public CharacterControl characterStart;
        public List<SlotAttachmentPair> slotDeactiveClick = new List<SlotAttachmentPair>();
        private bool _canDestroy = true;
        private Sprite _initialSprite;
        private bool _isDone;
        private bool _isDragging;
        private Camera _mainCamera;

        public UnityEvent OnStartDrag => onMouseDown;
        public UnityEvent OnEndDrag => onMouseUp;
        public UnityEvent OnRemoveItem => onRemoveItem;
        // TEMP
        private ScalingOnPick _scalingOnPick;

        private Vector3 _startPos;

        public Collider2D Col => col;
        public SpriteRenderer SpriteRenderer => sr;

        public bool IsDragging => _isDragging;
        public UnityEvent onDone;
        private int layerCurrent = 0;
        private void Awake()
        {
            _mainCamera = Camera.main;
            _scalingOnPick = GetComponent<ScalingOnPick>();
            _initialSprite = sr.sprite;
            layerCurrent = sr.sortingOrder;
        }

        private void Start()
        {
            col.enabled = true;

        }

        private void OnDestroy()
        {
        }

        private void OnMouseDown()
        {
            if (_isDone) return;
            if (_isDragging) return;
            _startPos = Tf.position;
            _startPos.z = 0;
            _isDragging = true;
            characterStart.TurnOffSlotsAttachment(slotDeactiveClick);
            if (pickUpSound.clip != null)
            {
                SoundManager.PlaySFX(pickUpSound.clip, pickUpSound.volume);
            }
            onMouseDown.Invoke();
            if (hasAlternativeSprite) sr.sprite = alternativeSprite;
            if (sr != null)
            {
                sr.sortingOrder = 30;
            }
        }

        private void OnMouseDrag()
        {
            if (_isDone) return;
            if (!_isDragging) return;
            var mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            mousePos += offsetOnPick;
            Tf.position = mousePos;
        }

        private void OnMouseUp()
        {
            if (_isDone) return;
            if (!_isDragging) return;
            _isDragging = false;
            if (!_canDestroy) return;
            if (sr != null)
            {
                sr.sortingOrder = layerCurrent;
            }
            if (DistanceToInSqrVec2(_startPos) > distanceChangeToThrow)
            {
                characterStart.TurnOffSlotsAttachment(slotDeactiveClick);
                Tf.SetParent(null);
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 2;
                _isDone = true;
                // Delay destroy to avoid error
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

                characterStart.TurnOnSlotsAttachment(slotDeactiveClick);
                Tf.position = _startPos;
                if (hasAlternativeSprite) sr.sprite = _initialSprite;
                onMouseUp.Invoke();
            }
        }

        public event Action OnRemoveItemEv;

        private void OnBlockPlayerInteractChanged()
        {

        }

#if UNITY_EDITOR

        [Button]
        private void GetReference()
        {
            sr = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();
            rb = GetComponent<Rigidbody2D>();
        }

#endif
    }

}
