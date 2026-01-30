using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
namespace sonnv
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public class SonSnapObject : SonMonoBehaviour, SonISnapObject
    {
        [SerializeField] protected SonSnapPoint[] snapToPosition;
        [SerializeField] protected SpriteRenderer sprite;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] protected Collider2D col;

        [SerializeField]
        protected bool moveBackOnDrop = true;




        [SerializeField] private bool useUpdateToDragLerp;
        [SerializeField][Range(0f, 1f)] private float interpolateSpeed = 0.8f;
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
        [SerializeField] protected UnityEvent notSnapWhenNearSnapPoint;

     [SerializeField] private bool isEnableCollideWhenEnableThis;
        [SerializeField] private bool moveBack;
        [SerializeField] private bool isChangeScaleAffterSnap = false;
       [SerializeField] private float scaleAffterSnap = 1;

        [SerializeField] private bool rotateOnDrag;
       [SerializeField] private float onDragRotateZ = 10f;

       [SerializeField] private bool isBounceLoop = false;
        [SerializeField] protected AnimationCurve curveFloatingIdle = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        [SerializeField] protected float durationFloatingIdle = 2.41f;

       [SerializeField] protected float curveTimeOffSet = 0.25f;

       [SerializeField] protected float speedFloatingIdle = 0.1f;
       [SerializeField] protected bool isFloatingStart;
       protected Vector3 floatingAnchor;
       protected float timeOffsetFloating;
        protected bool isFloating;

        private Camera _mainCam;
        private bool _isDragging;

        private Vector3 _initScale;
        private float _initZRot;
        private Vector3 _mousePos;
        private bool _canInteract = true;
        private Tween _moveBackTween;
        private Tween _rotateTween;
        private Vector3 _initLocalPos;

        public bool IsSnap { get; private set; }
        public bool CanMoveBack => moveBack;
        public bool IsDragging => _isDragging;

        public UnityEvent OnSnap => onSnap;
        public System.Action onMoveBackEnd;
        public Collider2D Col => col;
        public void ForceChangeSnap(bool isSnap)
        {
            IsSnap = isSnap;
        }

        private void Awake()
        {
            _mainCam = Camera.main;
            offset.z += Tf.position.z;
            _canInteract = true;
            if (moveBack)
            {
                _initLocalPos = Tf.localPosition;
            }
            OnAwake();
        }
        public void OnInit()
        {
            _initScale = Tf.localScale;
            _initZRot = Tf.eulerAngles.z;
            sprite.sortingOrder = onDropOrderLayer;
            if (ignoreRigidBody && rb) rb.bodyType = RigidbodyType2D.Static;

            OnStart();
            if (isFloatingStart)
            {
                StartFloating();
            }
        }
        private void Start()
        {
            OnInit();
        }
        public void StartFloating()
        {
            if (isBounceLoop)
            {
                isFloating = true;
                floatingAnchor = transform.position;

                timeOffsetFloating = (Mathf.Round(Time.time / durationFloatingIdle) + curveTimeOffSet) * durationFloatingIdle - Time.time;
            }
        }
        public void StopFloating()
        {

            if (isBounceLoop)
            {
                isFloating = false;

            }
        }
        protected void HandleFloatingMovement()
        {
            if (isBounceLoop)
            {
                if (!isFloating) return;
                float timeProgress = Mathf.Repeat((Time.time + timeOffsetFloating) / durationFloatingIdle, 1f);
                Vector3 targetPos = floatingAnchor + Vector3.up * curveFloatingIdle.Evaluate(timeProgress);
                transform.position = Vector3.Lerp(transform.position, targetPos, speedFloatingIdle);

            }

        }
        public void SetInteract(bool value)
        {
            _canInteract = value;
        }
        private void OnBlockInteract()
        {

        }

        private void Update()
        {
            HandleFloatingMovement();
            if (!useUpdateToDragLerp) return;
            if (!_isDragging || IsSnap) return;
            // lerp to mouse position
            Tf.position = Vector3.Lerp(Tf.position, _mousePos, interpolateSpeed);
            // slerp rotation to 0
            if (!isRotate) Tf.localRotation = Quaternion.Slerp(Tf.localRotation, Quaternion.identity, interpolateSpeed);
        }

        protected virtual void OnAwake() { }

        protected virtual void OnStart() { }

        protected virtual bool CanSnap()
        {
            return true;
        }

        protected virtual void OnStartDrag() { }

        protected virtual void OnDrop() { }

        protected virtual void OnMouseDown()
        {
            if (!_canInteract) return;
            if (_isDragging || IsSnap) return;
            _isDragging = true;
            StopFloating();
            sprite.sortingOrder = onDragOrderLayer;
            if (!ignoreRigidBody) rb.bodyType = RigidbodyType2D.Static;
            col.isTrigger = true;
            _mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
            _mousePos += offset;
            _mousePos.z = offset.z;
            if (!useUpdateToDragLerp)
            {
                Tf.position = _mousePos;
                Tf.localRotation = Quaternion.identity;
            }
            Tf.localScale = _initScale * scaleOnDrag;
            SoundManager.PlaySFX(onDragAudio.clip, onDragAudio.volume);
            if (rotateOnDrag)
            {
                _rotateTween?.Kill();
                _rotateTween = Tf.DORotate(new Vector3(0, 0, onDragRotateZ), 0.3f);
            }
            _moveBackTween?.Kill();
            OnStartDrag();
            onStartDrag?.Invoke();
        }

        private void OnMouseDrag()
        {
            if (!_canInteract) return;
            if (!_isDragging || IsSnap) return;
            _mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
            _mousePos += offset;
            _mousePos.z = offset.z;
            if (!useUpdateToDragLerp) Tf.position = _mousePos;
        }

        private void OnMouseUp()
        {
            if (!_isDragging || IsSnap) return;
            _isDragging = false;
            StartFloating();
            sprite.sortingOrder = onDropOrderLayer;
            if (!ignoreRigidBody) rb.bodyType = RigidbodyType2D.Dynamic;
            col.isTrigger = false;
            Tf.localScale = _initScale;
            if (_canInteract)
            {
                for (int i = 0; i < snapToPosition.Length; i++)
                {
                    SonSnapPoint snapPoint = snapToPosition[i];
                    if (snapPoint.Tf.gameObject.activeSelf && snapPoint.isSnap) continue;
                    if (!snapPoint.canSnap) continue;
                    if (!(DistanceToInSqrVec2(snapPoint.Tf) < snapDistance)) continue;
                    if (!CanSnap())
                    {
                        notSnapWhenNearSnapPoint.Invoke();
                    }
                    else
                    {
                        if (!ignoreRigidBody) rb.bodyType = RigidbodyType2D.Static;
                        if (attachToSnapPoint) Tf.SetParent(snapPoint.Tf);
                        IsSnap = true;
                        snapPoint.isSnap = true;
                        snapPoint.OnSnap();
                        sprite.sortingOrder = onSnapOrderLayer;
                        col.enabled = false;
                        SoundManager.PlaySFX(onSnapAudio.clip, onSnapAudio.volume);
                        OnSnapObject();
                        Tf.DOMove(snapPoint.Tf.position, 0.2f).OnComplete(() =>
                        {
                            onSnap.Invoke();
                        });
                        return;

                    }
                }
            }
            OnDrop();
            if (rotateOnDrag)
            {
                _rotateTween?.Kill();
                _rotateTween = Tf.DORotate(new Vector3(0, 0, _initZRot), 0.3f);
            }
            if (moveBack)
            {
                _moveBackTween = Tf.DOLocalMove(_initLocalPos, 0.3f).OnComplete(() =>
                {
                    onMoveBackEnd?.Invoke();
                    onDrop?.Invoke();
                });
            }
            else
            {
                onDrop?.Invoke();
            }
        }
        private void OnSnapObject()
        {
            if (isChangeScaleAffterSnap)
            {
                ForceChangeLocalScale(scaleAffterSnap);
            }
            StopFloating();
        }
        public bool HasSnapPoint(out Vector3 pos)
        {
            pos = Vector3.zero;
            for (int i = 0; i < snapToPosition.Length; i++)
            {
                if (/*snapToPosition[i].canSnap &&*/ !snapToPosition[i].isSnap)
                {
                    pos = snapToPosition[i].Tf.position;
                    return true;
                }
            }

            return false;
        }

        private void ForceChangeLocalScale(float scale)
        {
            Tf.localScale = Vector3.one * scale;
        }

        [Button]
        protected virtual void GetReferences()
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
            rb = GetComponentInChildren<Rigidbody2D>();
            col = GetComponentInChildren<Collider2D>();
        }
        public void AddStartDragEvent(UnityAction action)
        {
            onStartDrag.AddListener(action);
        }
        public void RemoveStartDragEvent(UnityAction action)
        {
            onStartDrag.RemoveListener(action);
        }
        public void RemoveEndDragEvent(UnityAction action)
        {
            onDrop.RemoveListener(action);
        }
        private void OnEnable()
        {
            if (isEnableCollideWhenEnableThis) col.enabled = true;
        }
        public void AddSnapEvent(UnityAction action)
        {
            onSnap.AddListener(action);
        }
        public void RemoveSnapEvent(UnityAction action)
        {
            onSnap.RemoveListener(action);
        }
        public void AddEndDragEvent(UnityAction action)
        {
            onDrop.AddListener(action);
        }

    }

}
