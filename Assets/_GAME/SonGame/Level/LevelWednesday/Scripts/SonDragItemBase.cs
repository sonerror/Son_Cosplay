using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using sonnv;

public class SonDragItemBase : SonMonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    /* ================= CONFIG ================= */

    [SerializeField] private List<int> useInSteps = new List<int>();

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private LevelZozo _level;
    [SerializeField] private Collider2D col;

    [Header("Face Zone")]
    [SerializeField] private bool useRectTriggerZone;
    [SerializeField] private Transform facePoint;
    [SerializeField] private float facePointZoneRadius = 2f;
    [SerializeField] private Rect rectTriggerZone;
    [SerializeField] private float checkStepAfterSecondsInZone = 1f;

    [Header("Drag Behaviour")]
    [SerializeField] private bool forceCheckWrongStep;
    [SerializeField] private bool isMoveBackOnRelease = true;
    [SerializeField] private float moveBackTime = 0.3f;

    [SerializeField] private int sortingOrderOnDrag = 10;
    [SerializeField] private int sortingOrderOnRelease = 1;

    [SerializeField] private float dragSlerpSpeed = 30f;
    [SerializeField] private Vector3 dragPosOffset = new Vector3(0f, 1f, 0f);

    [Header("Visual")]
    [SerializeField] private bool rotateOnDrag;
    [SerializeField] private float onDragRotateZ = 10f;

    [SerializeField] private bool scaleOnPick;
    [SerializeField] private Vector3 scaling = Vector3.one;

    [SerializeField] private AudioClip pickUpSound;

    [Header("Events")]
    public UnityEvent onDragStart;
    public UnityEvent onDragStop;
    public UnityEvent onMoveBackEnd;

    /* ================= STATE ================= */

    private bool _canInteract;
    public bool IsDragging { get; private set; }

    private Vector3 _initialPos;
    private Vector3 _initScale;
    private float _initZ;
    private float _initZRot;

    private Vector3 _nextPos;

    private Camera _mainCamera;

    private Tween _moveBackTween;
    private Tween _rotateTween;

    private Coroutine checkStepCoroutine;

    /* ================= UNITY ================= */

    private void Awake()
    {
        if (_level == null)
        {
            _level = LevelZozo.Ins;
        }
        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();
        if (!col) col = GetComponent<Collider2D>();
    }

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

    /* ================= POINTER EVENTS ================= */

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_canInteract || IsDragging) return;

        UpdateNextPos(eventData);

        IsDragging = true;

        if (sprite)
            sprite.sortingOrder = sortingOrderOnDrag;

        if (pickUpSound)
            SoundManager.PlaySFX(pickUpSound);

        _moveBackTween?.Kill();

        if (rotateOnDrag)
        {
            _rotateTween?.Kill();
            _rotateTween = Tf.DORotate(
                new Vector3(0, 0, onDragRotateZ),
                moveBackTime
            );
        }

        if (scaleOnPick)
            Tf.localScale = scaling;

        onDragStart?.Invoke();

        // Start auto-check coroutine
        if (LevelScarlet.Ins != null &&
            (!useInSteps.Contains(StepManager.Ins.CurrentStep) || forceCheckWrongStep))
        {
            if (checkStepCoroutine != null)
                StopCoroutine(checkStepCoroutine);

            checkStepCoroutine = StartCoroutine(WaitCheckCurrentStep());
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_canInteract || !IsDragging) return;
        UpdateNextPos(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ReleaseDrag();
    }
    /* ================= CORE LOGIC ================= */
    private void ReleaseDrag()
    {
        // stop auto timer
        if (checkStepCoroutine != null)
        {
            StopCoroutine(checkStepCoroutine);
            checkStepCoroutine = null;
        }
        if (!_canInteract || !IsDragging) return;

        IsDragging = false;
        _nextPos = Tf.TransformPoint(_initialPos);

        SetInteract(false);

        _moveBackTween?.Kill();

        if (isMoveBackOnRelease)
        {
            _moveBackTween = Tf.DOLocalMove(_initialPos, moveBackTime)
                .OnComplete(() =>
                {
                    if (sprite)
                        sprite.sortingOrder = sortingOrderOnRelease;

                    SetInteract(true);
                    onMoveBackEnd?.Invoke();
                });
        }
        else
        {
            SetInteract(true);
            onMoveBackEnd?.Invoke();
        }

        if (rotateOnDrag)
        {
            _rotateTween?.Kill();
            _rotateTween = Tf.DORotate(
                new Vector3(0, 0, _initZRot),
                moveBackTime
            );
        }

        if (scaleOnPick)
            Tf.localScale = _initScale;

        onDragStop?.Invoke();

        // Wrong step feedback
        if (_level != null &&
            (!useInSteps.Contains(StepManager.Ins.CurrentStep) || forceCheckWrongStep))
        {
            if (InsideFaceZone)
            {
                _level.PlayNegativeEmoji();
            }
        }

        if (_level != null)
        {

            if (_level.IsDoneStep)
            {
                _level.TryNextStep();
            }
        }
    }

    /* ================= COROUTINE ================= */

    private IEnumerator WaitCheckCurrentStep()
    {
        float timer = 0f;

        while (IsDragging)
        {
            if (InsideFaceZone)
            {
                timer += Time.deltaTime;

                if (timer >= checkStepAfterSecondsInZone)
                {
                    ReleaseDrag();
                    yield break;
                }
            }
            else
            {
                timer = 0f;
            }

            yield return null;
        }
    }

    /* ================= HELPERS ================= */

    private void UpdateNextPos(PointerEventData eventData)
    {
        Vector3 pos = _mainCamera.ScreenToWorldPoint(eventData.position);
        pos.z = _initZ;
        pos += dragPosOffset;
        _nextPos = pos;
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


#if UNITY_EDITOR
    [Sirenix.OdinInspector.Button]
    private void GetRef()
    {
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        if (!sprite)
            sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (useRectTriggerZone)
        {
            Gizmos.DrawWireCube(rectTriggerZone.center,
                (Vector3)rectTriggerZone.size + Vector3.forward);
        }
        else if (facePoint && facePointZoneRadius > 0)
        {
            Gizmos.DrawWireSphere(facePoint.position, facePointZoneRadius);
        }
    }
#endif
}
