using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace sonnv
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public class SonSnapObject : SonMonoBehaviour, SonISnapObject,
        IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private LevelControl _level;
        [SerializeField] protected SonSnapPoint[] snapToPosition;
        private SonSnapPoint _snapPoint;
        public SonSnapPoint SnapPoint => _snapPoint;
        [SerializeField] protected SpriteRenderer sprite;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] protected Collider2D col;

        [SerializeField] protected bool moveBackOnDrop = true;

        [SerializeField] private bool useUpdateToDragLerp;
        [SerializeField, Range(0f, 1f)] private float interpolateSpeed = 0.8f;
        [SerializeField] private bool ignoreRigidBody;
        [SerializeField] private bool attachToSnapPoint;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float snapDistance;
        [SerializeField] private float scaleOnDrag = 1;
        [SerializeField] private int onDropOrderLayer;
        [SerializeField] private int onDragOrderLayer;
        [SerializeField] private int onSnapOrderLayer;
        [SerializeField] protected bool isRotate;

        [SerializeField] private AudioData onDragAudio;
        [SerializeField] private AudioData onSnapAudio;

        [SerializeField] protected UnityEvent onSnap;
        [SerializeField] protected UnityEvent onStartDrag;
        [SerializeField] protected UnityEvent onDrop;
        [SerializeField] protected UnityEvent onMouseUp;
        [SerializeField] protected UnityEvent notSnapWhenNearSnapPoint;

        [SerializeField] private bool isEnableCollideWhenEnableThis;
        [SerializeField] private bool moveBack;
        [SerializeField] private bool isChangeScaleAffterSnap = false;
        [SerializeField] private float scaleAffterSnap = 1;

        [SerializeField] private bool rotateOnDrag;
        [SerializeField] private float onDragRotateZ = 10f;

        [SerializeField] private bool isBounceLoop = false;
        [SerializeField] protected float durationFloatingIdle = 2.41f;
        [SerializeField] protected float curveTimeOffSet = 0.25f;
        [SerializeField] protected float speedFloatingIdle = 0.1f;
        [SerializeField] protected bool isFloatingStart;

        protected Vector3 floatingAnchor;
        protected float timeOffsetFloating;
        protected bool isFloating;

        private Camera _mainCam;
        private bool _isDragging;
        private bool _canInteract = true;

        private Vector3 _initScale;
        private float _initZRot;
        private Vector3 _mousePos;
        private Vector3 _initLocalPos;

        private Tween _moveBackTween;
        private Tween _rotateTween;

        public bool IsSnap { get; private set; }
        public bool CanMoveBack => moveBack;
        public bool IsDragging => _isDragging;

        public UnityEvent OnSnap => onSnap;
        public System.Action onMoveBackEnd;
        public Collider2D Col => col;

        /* ================= LIFECYCLE ================= */

        private void Awake()
        {
            _mainCam = Camera.main;
            offset.z += Tf.position.z;

            if (moveBack)
                _initLocalPos = Tf.localPosition;

            OnAwake();
        }

        private void Start()
        {
            _initScale = Tf.localScale;
            _initZRot = Tf.eulerAngles.z;

            sprite.sortingOrder = onDropOrderLayer;

            if (ignoreRigidBody && rb)
                rb.bodyType = RigidbodyType2D.Static;

            OnStart();

            if (isFloatingStart)
                StartFloating();
        }

        private void Update()
        {
            if (!useUpdateToDragLerp) return;
            if (!_isDragging || IsSnap) return;

            Tf.position = Vector3.Lerp(Tf.position, _mousePos, interpolateSpeed);

            if (!isRotate)
                Tf.localRotation = Quaternion.Slerp(
                    Tf.localRotation,
                    Quaternion.identity,
                    interpolateSpeed
                );
        }

        /* ================= POINTER EVENTS (LUNA) ================= */

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_canInteract || _isDragging || IsSnap) return;

            _isDragging = true;
            StopFloating();

            sprite.sortingOrder = onDragOrderLayer;

            if (!ignoreRigidBody && rb)
                rb.bodyType = RigidbodyType2D.Static;

            col.isTrigger = true;

            UpdateMousePos(eventData);

            if (!useUpdateToDragLerp)
            {
                Tf.position = _mousePos;
                Tf.localRotation = Quaternion.identity;
            }

            Tf.localScale = _initScale * scaleOnDrag;

            SoundManager.PlaySFX(onDragAudio.clip, onDragAudio.volume);

            if (rotateOnDrag)
            {
                if (_rotateTween != null) _rotateTween.Kill();
                _rotateTween = Tf.DORotate(
                    new Vector3(0, 0, onDragRotateZ),
                    0.3f
                );
            }

            if (_moveBackTween != null) _moveBackTween.Kill();

            OnStartDrag();
            onStartDrag.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_canInteract || !_isDragging || IsSnap) return;

            UpdateMousePos(eventData);

            if (!useUpdateToDragLerp)
                Tf.position = _mousePos;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isDragging || IsSnap) return;

            _isDragging = false;
            StartFloating();

            sprite.sortingOrder = onDropOrderLayer;

            if (!ignoreRigidBody && rb)
                rb.bodyType = RigidbodyType2D.Dynamic;

            col.isTrigger = false;
            Tf.localScale = _initScale;

            if (_canInteract)
            {
                for (int i = 0; i < snapToPosition.Length; i++)
                {
                    SonSnapPoint snapPoint = snapToPosition[i];
                    if ((DistanceToInSqrVec2(snapPoint.Tf) < snapDistance))
                    {
                        if (!snapPoint.isSnap && !snapPoint.canSnap)
                        {
                            _level.PlayNegativeEmoji();
                        }
                    }
                    if (!snapPoint.Tf.gameObject.activeSelf) continue;
                    if (snapPoint.isSnap || !snapPoint.canSnap) continue;
                    if (!(DistanceToInSqrVec2(snapPoint.Tf) < snapDistance)) continue;
                    _snapPoint = snapPoint;
                    if (!CanSnap())
                    {
                        notSnapWhenNearSnapPoint.Invoke();
                        break;
                    }

                    if (!ignoreRigidBody && rb)
                        rb.bodyType = RigidbodyType2D.Static;

                    if (attachToSnapPoint)
                        Tf.SetParent(snapPoint.Tf);

                    IsSnap = true;
                    snapPoint.isSnap = true;
                    snapPoint.OnSnap();

                    sprite.sortingOrder = onSnapOrderLayer;
                    col.enabled = false;

                    SoundManager.PlaySFX(onSnapAudio.clip, onSnapAudio.volume);

                    OnSnapObject();

                    Tf.DOMove(snapPoint.Tf.position, 0.2f)
                      .OnComplete(() =>
                      {
                          onSnap.Invoke();
                      });



                    return;
                }
            }

            // 👇 GIỮ HÀNH VI onMouseUp
            onMouseUp.Invoke();

            OnDrop();

            if (rotateOnDrag)
            {
                if (_rotateTween != null) _rotateTween.Kill();
                _rotateTween = Tf.DORotate(
                    new Vector3(0, 0, _initZRot),
                    0.3f
                );
            }

            if (moveBack)
            {
                _moveBackTween = Tf.DOLocalMove(_initLocalPos, 0.3f)
                    .OnComplete(() =>
                    {
                        if (onMoveBackEnd != null)
                            onMoveBackEnd.Invoke();

                        onDrop.Invoke();
                    });
            }
            else
            {
                onDrop.Invoke();
            }

        }

        /* ================= HELPERS ================= */

        private void UpdateMousePos(PointerEventData eventData)
        {
            Vector3 worldPos = _mainCam.ScreenToWorldPoint(eventData.position);
            worldPos += offset;
            worldPos.z = offset.z;
            _mousePos = worldPos;
        }

        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }
        protected virtual bool CanSnap() { return true; }
        protected virtual void OnStartDrag() { }
        protected virtual void OnDrop() { }

        private void OnSnapObject()
        {
            if (isChangeScaleAffterSnap)
                Tf.localScale = Vector3.one * scaleAffterSnap;

            StopFloating();
        }

        public void StartFloating()
        {
            if (!isBounceLoop) return;

            isFloating = true;
            floatingAnchor = Tf.position;

            timeOffsetFloating =
                (Mathf.Round(Time.time / durationFloatingIdle) + curveTimeOffSet)
                * durationFloatingIdle - Time.time;
        }

        public void StopFloating()
        {
            isFloating = false;
        }

        private void OnEnable()
        {
            if (isEnableCollideWhenEnableThis && col)
                col.enabled = true;
        }
    }
}
