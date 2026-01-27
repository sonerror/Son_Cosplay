using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
namespace sonnv
{
    public class SonDragItemBase : SonMonoBehaviour
    {
        [FoldoutGroup("Base")]
        [SerializeField]
        private List<int> useInSteps;

        [FoldoutGroup("Base")]
        [SerializeField]
        private SpriteRenderer sprite;

        [FoldoutGroup("Base")]
        [SerializeField]
        private Collider2D col;

        [FoldoutGroup("Base")]
        [SerializeField]
        private bool useRectTriggerZone;
        [FoldoutGroup("Base"), HideIf("useRectTriggerZone")]
        [SerializeField]
        private Transform facePoint;
        [FoldoutGroup("Base"), HideIf("useRectTriggerZone")]
        [SerializeField]
        private float facePointZoneRadius = 2f;
        [FoldoutGroup("Base"), ShowIf("useRectTriggerZone")]
        [SerializeField]
        private Rect rectTriggerZone;
        [FoldoutGroup("Base")]
        [SerializeField]
        private float checkStepAfterSecondsInZone = 1f;

        [FoldoutGroup("Base")]
        [SerializeField]
        private bool forceCheckWrongStep;

        [FoldoutGroup("Dragging")]
        [SerializeField]
        private bool isMoveBackOnRelease = true;

        [FoldoutGroup("Dragging")]
        [SerializeField]
        [ShowIf("isMoveBackOnRelease")]
        private float moveBackTime = 0.3f;

        [FoldoutGroup("Dragging")]
        [SerializeField]
        private int sortingOrderOnDrag = 10;

        [FoldoutGroup("Dragging")]
        [SerializeField]
        private int sortingOrderOnRelease = 1;

        [FoldoutGroup("Dragging")]
        [SerializeField]
        private float dragSlerpSpeed = 30f;

        [FoldoutGroup("Dragging")]
        [SerializeField]
        private Vector3 dragPosOffset = new(0, 1, 0);

        [FoldoutGroup("Dragging")]
        [SerializeField]
        private bool rotateOnDrag;

        [FoldoutGroup("Dragging")]
        [ShowIf("rotateOnDrag")]
        [SerializeField]
        private float onDragRotateZ = 10f;

        [FoldoutGroup("Dragging")]
        [SerializeField]
        private bool scaleOnPick;
        [FoldoutGroup("Dragging")]
        [SerializeField]
        [ShowIf("scaleOnPick")]
        private Vector3 scaling;

        [FoldoutGroup("Dragging")]
        [SerializeField]
        private AudioClip pickUpSound;

        [FoldoutGroup("Event")]
        public UnityEvent onDragStart;

        [FoldoutGroup("Event")]
        public UnityEvent onDragStop;

        [FoldoutGroup("Event"), ShowIf("isMoveBackOnRelease")]
        public UnityEvent onMoveBackEnd;

        private bool _canInteract;
        private Vector3 _initialPos;
        private Vector3 _initScale;
        private float _initZ;
        private float _initZRot;

        private Camera _mainCamera;

        private Tween _moveBackTween;
        private Vector3 _nextPos;
        private Vector3 _initScaling;

        private Tween _rotateTween;

        public float DragSpeed => DistanceToInSqrVec2(_nextPos);
        public bool IsDragging { get; private set; }
        public Vector3 InitialPos => _initialPos;

        private void Start()
        {
            _initZ = Tf.position.z;
            _initialPos = Tf.localPosition;
            _initZRot = Tf.eulerAngles.z;
            _nextPos = Tf.position;
            if (scaleOnPick) _initScale = Tf.localScale;
            _mainCamera = Camera.main;
            SetInteract(true);

        }

        private void Update()
        {
            if (_canInteract && IsDragging)
            {
                Tf.position = Vector3.Slerp(Tf.position, _nextPos, dragSlerpSpeed * Time.deltaTime);
            }
        }

        private void OnMouseDown()
        {
            if (!_canInteract) return;
            if (IsDragging) return;
            _nextPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            _nextPos.z = _initZ;
            _nextPos += dragPosOffset;
            IsDragging = true;
            sprite.sortingOrder = sortingOrderOnDrag;
            SoundManager.PlaySFX(pickUpSound);
            _moveBackTween?.Kill();
            if (rotateOnDrag)
            {
                _rotateTween?.Kill();
                _rotateTween = Tf.DORotate(new Vector3(0, 0, onDragRotateZ), moveBackTime);
            }
            if (scaleOnPick) Tf.localScale = scaling;
            onDragStart.Invoke();


        }


        private void OnMouseDrag()
        {
            if (!_canInteract) return;
            if (!IsDragging) return;
            _nextPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            _nextPos.z = _initZ;
            _nextPos += dragPosOffset;
        }

        public void OnMouseUp()
        {
            if (!_canInteract) return;
            if (!IsDragging) return;
            IsDragging = false;
            _nextPos = Tf.TransformPoint(_initialPos);
            SetInteract(false);
            _moveBackTween?.Kill();
            _moveBackTween = Tf.DOLocalMove(_initialPos, moveBackTime);
            _moveBackTween.OnComplete(() =>
            {
                sprite.sortingOrder = sortingOrderOnRelease;
                SetInteract(true);
                onMoveBackEnd.Invoke();
            });
            if (rotateOnDrag)
            {
                _rotateTween?.Kill();
                _rotateTween = Tf.DORotate(new Vector3(0, 0, _initZRot), moveBackTime);
            }

            if (scaleOnPick)
            {
                Tf.localScale = _initScale;
            }
            onDragStop.Invoke();
        }

        public void ResetCheckCoroutine()
        {

        }

        public void AddUseInStep(int step)
        {
            if (useInSteps.Contains(step)) return;
            useInSteps.Add(step);
        }

        public void RemoveUseInStep(int step)
        {
            if (!useInSteps.Contains(step)) return;
            useInSteps.Remove(step);
        }

        public void SetForceCheckWrongStep(bool isCheck)
        {
            forceCheckWrongStep = isCheck;
            if (!isCheck)
            {
            }
        }

        private bool InsideFaceZone
        {
            get
            {
                if (useRectTriggerZone) return rectTriggerZone.Contains(Tf.position);
                if (!facePoint) return false;
                return Vector2.Distance(facePoint.position, Tf.position) <= facePointZoneRadius;
            }
        }



        private void SetInteract(bool canMove, bool changeCollider = true)
        {
            _canInteract = canMove;
            if (changeCollider) col.enabled = canMove;
        }

        private void OnBlockInteract()
        {

        }

#if UNITY_EDITOR
        [Button]
        private void GetRef()
        {
            col = GetComponent<Collider2D>();
            sprite = GetComponent<SpriteRenderer>();
            if (sprite == null)
            {
                sprite = GetComponentInChildren<SpriteRenderer>();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            if (useRectTriggerZone)
            {
                Gizmos.DrawWireCube(rectTriggerZone.center, (Vector3)(rectTriggerZone.size) + Vector3.forward);
            }
            else if (facePoint)
            {
                if (facePointZoneRadius <= 0) return;
                Gizmos.DrawWireSphere(facePoint.position, facePointZoneRadius);
            }
        }
#endif
    }

}
