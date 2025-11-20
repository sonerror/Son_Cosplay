using System.Linq;
using Costopia.Level;
using DG.Tweening;


using UnityEngine;
using UnityEngine.Events;

namespace HoangHH
{
  public class NSnapObject : H3MonoBehaviour
  {
    // References

    [SerializeField]
    protected NSnapPoint[] snapToPosition;


    [SerializeField]
    protected SpriteRenderer sprite;

    [SerializeField]
    protected Collider col;

    // Moving

    [SerializeField]
    private bool ignoreRigidBody;


    [SerializeField]
    private Vector3 offset;


    [SerializeField]
    private bool useUpdateToDragLerp;


    [SerializeField]
    [Range(0f, 1f)]
    private float speedInterpolate = 0.3f;


    [SerializeField]
    private float scaleOnDrag = 1;


    [SerializeField]
    private bool overrideInitScale;


    [SerializeField]
    private Vector3 initScale;


    [SerializeField]
    protected bool moveBackOnDrop = true;



    [SerializeField]
    private bool isBackByLocalPosition;



    [SerializeField]
    protected Transform backPos;

    // Rotate

    [SerializeField]
    protected bool isRotate;


    [SerializeField]
    private float zRotateOnDrag;


    [SerializeField]
    protected float zRotateOnSnap;


    [SerializeField]
    private bool isRandomChangeZOnDrop;



    [SerializeField]
    private Vector2 randomChangeZOnDrop;

    // Snap

    [SerializeField]
    private bool attachToSnapPoint;


    [SerializeField]
    private bool overrideScaleOnSnap;


    [SerializeField]
    private Vector3 scaleOnSnap;


    [SerializeField]
    private bool overrideLocalRotationOnSnap;


    [SerializeField]
    private Vector3 localRotationOnSnap;


    [SerializeField]
    private bool overrideLocalPositionOnSnap;


    [SerializeField]
    private Vector3 localPositionOnSnap;


    [SerializeField]
    private float snapDistance;


    [SerializeField]
    protected int onDropOrderLayer;


    [SerializeField]
    protected int onDragOrderLayer;


    [SerializeField]
    protected int onSnapOrderLayer;


    // Haptic

    [SerializeField]
    private bool enableHaptic = true;

    // Event

    [SerializeField]
    private UnityEvent onStartDrag;


    [SerializeField]
    private UnityEvent onEndDrag;


    [SerializeField]
    protected UnityEvent onSnap;


    [SerializeField]
    private UnityEvent onEnableEvent;

    public System.Action onMoveBackEnd;


    [SerializeField] private bool disableOnSnapDone;
    [SerializeField] private bool isEnableCollideWhenEnableThis;
    [SerializeField] protected bool notLockAfterSnap;
    [SerializeField] private bool notChangeSnapAfterDrag;
    [SerializeField] private bool hasAlternativeSprite;

    [SerializeField]
    private Sprite alternativeSprite;

    [SerializeField] private bool changeOrderWhenDragBackEnd;
    [SerializeField] private bool callEndDragWhenDragBackEnd;
    [SerializeField] private bool notSetOrderOnStart;
    [SerializeField] protected UnityEvent notSnapWhenNearSnapPoint;

    private Camera _mainCam;
    private bool _isDragging;
    private Vector3 _initScale;
    private Vector3 _initRot;
    private Vector3 _initLocalPos;
    private Sprite _initSprite;

    private Vector3 _mousePos;
    private bool _canInteract = true;
    protected LevelBase levelBase;

    // for snap no lock
    private NSnapPoint _snapPoint;

    public Collider Col => col;
    public bool AttachToSnapPoint => attachToSnapPoint;
    public bool IsSnap { get; private set; }
    public bool HasSnapToPoint => _snapPoint != null;
    public NSnapPoint SnapPoint => _snapPoint;

    public SpriteRenderer Sprite => sprite;
    public Vector3 InitialPos => isBackByLocalPosition ? _initLocalPos : backPos.position;
    public bool CanMoveBack => moveBackOnDrop;
    public bool IsDragging => _isDragging;

    protected Transform BackPos => backPos;
    protected Vector3 InitRot => _initRot;

    private void Awake()
    {
      _mainCam = Camera.main;
      offset.z += Tf.position.z;
      _canInteract = true;
      _initRot = Tf.eulerAngles;
      if (isBackByLocalPosition) _initLocalPos = Tf.localPosition;
      OnAwake();
      if (hasAlternativeSprite) _initSprite = sprite.sprite;
    }

    private void Start()
    {
      _initScale = overrideInitScale ? initScale : Tf.localScale;
      if (!notSetOrderOnStart) sprite.sortingOrder = onDropOrderLayer;
      levelBase = LevelBase.Instance;
      if (levelBase)
      {
        levelBase.OnBlockPlayerInteractChanged += OnBlockInteract;
        if (!levelBase.IsAllowInteract) OnBlockInteract();
        if (levelBase is CosplayStepLevel cosplayLevel)
        {
          notSnapWhenNearSnapPoint.AddListener(() => cosplayLevel.OnWrongCurrentStep());
        }
      }

      OnStart();
    }

    private void OnBlockInteract()
    {
      if (!levelBase.IsAllowInteract)
      {
        _canInteract = false;
        if (_isDragging) OnMouseUp();
      }
      else
      {
        _canInteract = true;
      }
    }

    private void Update()
    {
      if (!useUpdateToDragLerp) return;
      if (!_isDragging || IsSnap) return;
      // lerp to mouse position
      Tf.position = Vector3.Lerp(Tf.position, _mousePos, speedInterpolate);
      // slerp rotation to 0
      if (!isRotate) Tf.localRotation = Quaternion.Slerp(Tf.localRotation, Quaternion.identity, speedInterpolate);
    }

    protected virtual void OnAwake()
    {
    }

    protected virtual void OnStart()
    {
    }

    protected virtual bool CanSnap()
    {
      return true;
    }

    protected Tween rotateTween;
    private Tween _moveSnapTween;

    public void OnKilSnapMovement(bool isComplete)
    {
      _moveSnapTween?.Kill(isComplete);
      rotateTween?.Kill(isComplete);
    }

    protected virtual void OnStartDrag()
    {
      // MMVibrationManager.Haptic(HapticTypes.Selection);
      if (_snapPoint != null && notLockAfterSnap)
      {
        if (attachToSnapPoint) Tf.SetParent(null);
        if (!notChangeSnapAfterDrag) _snapPoint.ForceChangeSnap(false);
        _snapPoint = null;
      }

      if (!isRotate) return;
      if (notLockAfterSnap) _moveSnapTween?.Kill();
      rotateTween?.Kill();
      rotateTween = Tf.DOLocalRotate(new Vector3(0, 0, zRotateOnDrag), 0.3f);
    }

    protected virtual void OnDrop()
    {
      if (!isRotate) return;
      rotateTween?.Kill();
      if (isRandomChangeZOnDrop)
      {
        Vector3 randomRot = new Vector3(_initRot.x, _initRot.y,
            Random.Range(randomChangeZOnDrop.x, randomChangeZOnDrop.y));
        rotateTween = Tf.DOLocalRotate(randomRot, 0.3f);
      }
      else
      {
        rotateTween = Tf.DOLocalRotate(_initRot, 0.3f);
      }
    }

    protected virtual void OnSnap()
    {
      /*if (levelBase is CosplayStepLevel cosplayLevel)
      {
          cosplayLevel.Character.SetBodyCanMove(true);
      }*/
    }

    public virtual void OnMouseDown()
    {
      Debug.Log("OnMouseDown1");
      if (!_canInteract) return;
      if (_isDragging || IsSnap) return;
      _isDragging = true;
      sprite.sortingOrder = onDragOrderLayer;
      col.isTrigger = true;
      _mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
      _mousePos += offset;
      _mousePos.z = offset.z;
      if (!useUpdateToDragLerp)
      {
        Tf.position = _mousePos;
        if (!isRotate) Tf.localRotation = Quaternion.identity;
      }

      Tf.localScale = _initScale * scaleOnDrag;
      // AudioManager.PlaySFX(onDragAudio.clip, onDragAudio.volume);
      if (hasAlternativeSprite) sprite.sprite = alternativeSprite;
      OnStartDrag();
      onStartDrag.Invoke();
      _moveSnapTween?.Kill();
      /*if (levelBase is CosplayStepLevel cosplayLevel)
      {
          cosplayLevel.Character.SetBodyCanMove(false);
      }*/
    }

    public virtual void OnMouseDrag()
    {
      if (!_canInteract) return;
      if (!_isDragging || IsSnap) return;
      _mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
      _mousePos += offset;
      _mousePos.z = offset.z;
      if (!useUpdateToDragLerp) Tf.position = _mousePos;
    }

    public void OnMouseUp()
    {
      if (!_isDragging || IsSnap) return;
      _isDragging = false;
      if (!changeOrderWhenDragBackEnd) sprite.sortingOrder = onDropOrderLayer;
      col.isTrigger = false;
      Tf.localScale = _initScale;
      if (_canInteract)
      {
        NSnapPoint point = GetSnapPoint(out bool isNearSnapPoint);
        if (point)
        {
          SnapToPoint(point);
          return;
        }
        if (isNearSnapPoint)
        {
          notSnapWhenNearSnapPoint.Invoke();
        }
      }

      if (moveBackOnDrop)
      {
        _moveSnapTween?.Kill();
        _moveSnapTween = isBackByLocalPosition
            ? Tf.DOLocalMove(_initLocalPos, 0.3f)
            : Tf.DOMove(backPos.position, 0.3f);
        _moveSnapTween.OnComplete(() =>
        {
          if (changeOrderWhenDragBackEnd) sprite.sortingOrder = onDropOrderLayer;
          if (callEndDragWhenDragBackEnd) onEndDrag.Invoke();
          onMoveBackEnd?.Invoke();
        });
      }

      if (hasAlternativeSprite) sprite.sprite = _initSprite;

      OnDrop();
      if (!callEndDragWhenDragBackEnd) onEndDrag.Invoke();

      return;

      NSnapPoint GetSnapPoint(out bool isNearSnapPoint)
      {
        isNearSnapPoint = false;
        NSnapPoint nearest = null;
        float minDistance = float.MaxValue;
        for (int i = 0; i < snapToPosition.Length; i++)
        {
          NSnapPoint snapPoint = snapToPosition[i];
          float dist = DistanceToInSqrVec2(snapPoint.Tf);
          if (dist < snapDistance)
          {
            isNearSnapPoint = true;
            if (dist < minDistance && snapPoint.canSnap && !snapPoint.isSnap && CanSnap())
            {
              nearest = snapPoint;
              minDistance = dist;
            }
          }
        }

        return nearest;
      }

      void SnapToPoint(NSnapPoint sp)
      {
        if (attachToSnapPoint) Tf.SetParent(sp.Tf);
        sp.isSnap = true;
        sp.OnSnap(this);
        _snapPoint = sp;
        sprite.sortingOrder = onSnapOrderLayer;
        // AudioManager.PlaySFX(onSnapAudio.clip, onSnapAudio.volume);
        if (enableHaptic) // MMVibrationManager.Haptic(onSnapHaptic);
          if (overrideScaleOnSnap) Tf.localScale = scaleOnSnap;
        if (notLockAfterSnap)
        {
          OnSnap();
          onSnap.Invoke();
          if (overrideLocalPositionOnSnap)
          {
            _moveSnapTween = Tf.DOLocalMove(localPositionOnSnap, 0.1f);
          }
          else
          {
            _moveSnapTween = Tf.DOMove(sp.Tf.position, 0.1f)
                .OnComplete(() => { Tf.position = sp.Tf.position; });
          }
        }
        else
        {
          IsSnap = true;
          col.enabled = false;
          if (disableOnSnapDone) enabled = false;
          if (overrideLocalPositionOnSnap)
          {
            _moveSnapTween = Tf.DOLocalMove(localPositionOnSnap, 0.1f).OnComplete(() =>
            {
              onSnap.Invoke();
              OnSnap();
            });
          }
          else
          {
            _moveSnapTween = Tf.DOMove(sp.Tf.position, 0.1f).OnComplete(() =>
            {
              Tf.position = sp.Tf.position;
              onSnap.Invoke();
              OnSnap();
            });
          }
        }

        if (overrideLocalRotationOnSnap)
        {
          Tf.localRotation = Quaternion.Euler(localRotationOnSnap);
        }
        else if (isRotate)
        {
          rotateTween?.Kill();
          rotateTween = Tf.DOLocalRotate(new Vector3(0, 0, zRotateOnSnap), 0.2f);
        }
      }
    }



    protected virtual void GetReferences()
    {
      sprite = GetComponentInChildren<SpriteRenderer>();
      col = GetComponentInChildren<Collider>();
    }
    public void ResetIsSnapState()
    {
      IsSnap = false;
    }




    private void GetBackPosAsParent()
    {
      backPos = Tf.parent;
    }

    public bool HasSnapPoint(out Vector3 pos)
    {
      pos = Vector3.zero;
      for (int i = 0; i < snapToPosition.Length; i++)
      {
        if (snapToPosition[i].canSnap && !snapToPosition[i].isSnap)
        {
          pos = snapToPosition[i].Tf.position;
          return true;
        }
      }

      return false;
    }


    protected virtual void OnEnable()
    {
      if (isEnableCollideWhenEnableThis) col.enabled = true;
      onEnableEvent.Invoke();
    }

    #region Additional Function

    public void ForceSetZ(float z)
    {
      if (useUpdateToDragLerp) _mousePos.z = z;
      else Tf.position = new Vector3(Tf.position.x, Tf.position.y, z);
    }

    public void RotateX(float value)
    {
      Tf.localRotation = Quaternion.Euler(value, Tf.localRotation.eulerAngles.z, Tf.localRotation.eulerAngles.z);
    }

    public void ForceChangeLocalScale(float scale)
    {
      Tf.localScale = Vector3.one * scale;
    }

    #endregion

    #region Add/Remove Event

    public void AddEnableEvent(UnityAction action)
    {
      onEnableEvent.AddListener(action);
    }

    public void RemoveEnableEvent(UnityAction action)
    {
      onEnableEvent.RemoveListener(action);
    }

    public void AddSnapEvent(UnityAction action)
    {
      onSnap.AddListener(action);
    }

    public void RemoveAllSnapEvent()
    {
      onSnap.RemoveAllListeners();
    }

    public void RemoveSnapEvent(UnityAction action)
    {
      onSnap.RemoveListener(action);
    }

    public void AddStartDragEvent(UnityAction action)
    {
      onStartDrag.AddListener(action);
    }

    public void RemoveAllStartDragEvent()
    {
      onStartDrag.RemoveAllListeners();
    }

    public void RemoveStartDragEvent(UnityAction action)
    {
      onStartDrag.RemoveListener(action);
    }

    public void AddEndDragEvent(UnityAction action)
    {
      onEndDrag.AddListener(action);
    }

    public void RemoveEndDragEvent(UnityAction action)
    {
      onEndDrag.RemoveListener(action);
    }

    public void RemoveAllEndDragEvent()
    {
      onEndDrag.RemoveAllListeners();
    }

    #endregion
  }
}