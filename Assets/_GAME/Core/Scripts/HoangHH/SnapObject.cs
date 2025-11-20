using DG.Tweening;


using UnityEngine;
using UnityEngine.Events;

namespace HoangHH
{
  [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
  public class SnapObject : H3MonoBehaviour
  {
    [SerializeField] protected SnapPoint[] snapToPosition;
    [SerializeField] protected SpriteRenderer sprite;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] protected Collider2D col;

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


    [Header("Event")]
    [SerializeField] protected UnityEvent onSnap;

    [Header("Other")][SerializeField] private bool isEnableCollideWhenEnableThis;
    [SerializeField] private bool moveBack;

    private Camera _mainCam;
    private bool _isDragging;
    private Vector3 _initScale;
    private Vector3 _mousePos;
    private bool _canInteract = true;
    private Tween _moveBackTween;
    private Vector3 _initLocalPos;
    protected LevelBase levelBase;

    public bool IsSnap { get; private set; }
    public UnityEvent OnSnap => onSnap;

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

    private void Start()
    {
      _initScale = Tf.localScale;
      sprite.sortingOrder = onDropOrderLayer;
      if (ignoreRigidBody && rb) rb.bodyType = RigidbodyType2D.Static;
      levelBase = LevelBase.Instance;
      if (levelBase)
      {
        levelBase.OnBlockPlayerInteractChanged += OnBlockInteract;
        if (!levelBase.IsAllowInteract) OnBlockInteract();
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
      // AudioManager.PlaySFX(onDragAudio.clip, onDragAudio.volume);
      // MMVibrationManager.Haptic(HapticTypes.Selection);
      _moveBackTween?.Kill();
      OnStartDrag();
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
      sprite.sortingOrder = onDropOrderLayer;
      if (!ignoreRigidBody) rb.bodyType = RigidbodyType2D.Dynamic;
      col.isTrigger = false;
      Tf.localScale = _initScale;
      if (_canInteract)
      {
        for (int i = 0; i < snapToPosition.Length; i++)
        {
          SnapPoint snapPoint = snapToPosition[i];
          if (snapPoint.Tf.gameObject.activeSelf && snapPoint.isSnap) continue;
          if (!(DistanceToInSqrVec2(snapPoint.Tf) < snapDistance)) continue;
          if (!CanSnap()) continue;
          if (!ignoreRigidBody) rb.bodyType = RigidbodyType2D.Static;
          if (attachToSnapPoint) Tf.SetParent(snapPoint.Tf);
          IsSnap = true;
          snapPoint.isSnap = true;
          snapPoint.OnSnap();
          sprite.sortingOrder = onSnapOrderLayer;
          col.enabled = false;
          // AudioManager.PlaySFX(onSnapAudio.clip, onSnapAudio.volume);
          Tf.DOMove(snapPoint.Tf.position, 0.2f).OnComplete(() =>
          {
            // MMVibrationManager.Haptic(HapticTypes.SoftImpact);
            onSnap.Invoke();
          });
          return;
        }
      }
      OnDrop();
      if (moveBack)
      {
        _moveBackTween = Tf.DOLocalMove(_initLocalPos, 0.3f);
      }
    }


    protected virtual void GetReferences()
    {
      sprite = GetComponentInChildren<SpriteRenderer>();
      rb = GetComponentInChildren<Rigidbody2D>();
      col = GetComponentInChildren<Collider2D>();
    }

    private void OnEnable()
    {
      if (isEnableCollideWhenEnableThis) col.enabled = true;
    }
  }
}
