using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using sonnv;
public class SonDragItemBase : SonMonoBehaviour
{
    [SerializeField]
    private List<int> useInSteps;

    [SerializeField]
    private SpriteRenderer sprite;

    [SerializeField]
    private Collider2D col;

    [SerializeField]
    private bool useRectTriggerZone;
    [SerializeField]
    private Transform facePoint;
    [SerializeField]
    private float facePointZoneRadius = 2f;
    [SerializeField]
    private Rect rectTriggerZone;
    [SerializeField]
    private float checkStepAfterSecondsInZone = 1f;

    [SerializeField]
    private bool forceCheckWrongStep;

    [SerializeField]
    private bool isMoveBackOnRelease = true;

    [SerializeField]
    private float moveBackTime = 0.3f;

    [SerializeField]
    private int sortingOrderOnDrag = 10;

    [SerializeField]
    private int sortingOrderOnRelease = 1;

    [SerializeField]
    private float dragSlerpSpeed = 30f;

    [SerializeField]
    private Vector3 dragPosOffset = new(0, 1, 0);

    [SerializeField]
    private bool rotateOnDrag;

    [SerializeField]
    private float onDragRotateZ = 10f;

    [SerializeField]
    private bool scaleOnPick;
    [SerializeField]
    private Vector3 scaling;

    [SerializeField]
    private AudioClip pickUpSound;

    public UnityEvent onDragStart;

    public UnityEvent onDragStop;

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
#if UNITY_EDITOR
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
