using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using sonnv;

public class SonDragItemBase : SonMonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private List<int> useInSteps;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Collider2D col;

    [SerializeField] private bool useRectTriggerZone;
    [SerializeField] private Transform facePoint;
    [SerializeField] private float facePointZoneRadius = 2f;
    [SerializeField] private Rect rectTriggerZone;
    [SerializeField] private float checkStepAfterSecondsInZone = 1f;

    [SerializeField] private bool forceCheckWrongStep;
    [SerializeField] private bool isMoveBackOnRelease = true;
    [SerializeField] private float moveBackTime = 0.3f;

    [SerializeField] private int sortingOrderOnDrag = 10;
    [SerializeField] private int sortingOrderOnRelease = 1;

    [SerializeField] private float dragSlerpSpeed = 30f;
    [SerializeField] private Vector3 dragPosOffset = new Vector3(0f, 1f, 0f);

    [SerializeField] private bool rotateOnDrag;
    [SerializeField] private float onDragRotateZ = 10f;

    [SerializeField] private bool scaleOnPick;
    [SerializeField] private Vector3 scaling;

    [SerializeField] private AudioClip pickUpSound;

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
    private Tween _rotateTween;

    private Vector3 _nextPos;

    public float DragSpeed => DistanceToInSqrVec2(_nextPos);
    public bool IsDragging { get; private set; }
    public Vector3 InitialPos => _initialPos;

    private void Start()
    {
        _initZ = Tf.position.z;
        _initialPos = Tf.localPosition;
        _initZRot = Tf.eulerAngles.z;
        _nextPos = Tf.position;

        if (scaleOnPick)
            _initScale = Tf.localScale;

        _mainCamera = Camera.main;
        SetInteract(true);
    }

    private void Update()
    {
        if (_canInteract && IsDragging)
        {
            Tf.position = Vector3.Slerp(
                Tf.position,
                _nextPos,
                dragSlerpSpeed * Time.deltaTime
            );
        }
    }

    /* ================= POINTER EVENTS (LUNA) ================= */

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_canInteract || IsDragging) return;

        UpdateNextPos(eventData);

        IsDragging = true;
        sprite.sortingOrder = sortingOrderOnDrag;

        if (pickUpSound)
            SoundManager.PlaySFX(pickUpSound);

        if (_moveBackTween != null) _moveBackTween.Kill();

        if (rotateOnDrag)
        {
            if (_rotateTween != null) _rotateTween.Kill();
            _rotateTween = Tf.DORotate(
                new Vector3(0, 0, onDragRotateZ),
                moveBackTime
            );
        }

        if (scaleOnPick)
            Tf.localScale = scaling;

        onDragStart.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_canInteract || !IsDragging) return;
        UpdateNextPos(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_canInteract || !IsDragging) return;

        IsDragging = false;
        _nextPos = Tf.TransformPoint(_initialPos);

        SetInteract(false);

        if (_moveBackTween != null) _moveBackTween.Kill();
        _moveBackTween = Tf.DOLocalMove(_initialPos, moveBackTime)
            .OnComplete(() =>
            {
                sprite.sortingOrder = sortingOrderOnRelease;
                SetInteract(true);
                onMoveBackEnd.Invoke();
            });

        if (rotateOnDrag)
        {
            if (_rotateTween != null) _rotateTween.Kill();
            _rotateTween = Tf.DORotate(
                new Vector3(0, 0, _initZRot),
                moveBackTime
            );
        }

        if (scaleOnPick)
            Tf.localScale = _initScale;

        onDragStop.Invoke();
    }

    /* ================= HELPERS ================= */

    private void UpdateNextPos(PointerEventData eventData)
    {
        Vector3 pos = _mainCamera.ScreenToWorldPoint(eventData.position);
        pos.z = _initZ;
        pos += dragPosOffset;
        _nextPos = pos;
    }

    public void AddUseInStep(int step)
    {
        if (!useInSteps.Contains(step))
            useInSteps.Add(step);
    }

    public void RemoveUseInStep(int step)
    {
        if (useInSteps.Contains(step))
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
            if (useRectTriggerZone)
                return rectTriggerZone.Contains(Tf.position);

            if (!facePoint)
                return false;

            return Vector2.Distance(facePoint.position, Tf.position)
                   <= facePointZoneRadius;
        }
    }

    private void SetInteract(bool canMove, bool changeCollider = true)
    {
        _canInteract = canMove;
        if (changeCollider && col)
            col.enabled = canMove;
    }
}
