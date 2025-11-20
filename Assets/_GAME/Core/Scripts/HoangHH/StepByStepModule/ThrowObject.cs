using System;
using DG.Tweening;

using UnityEngine;
using UnityEngine.Events;

namespace HoangHH
{
  public class ThrowObject : H3MonoBehaviour
  {
    [SerializeField]
    private SpriteRenderer sr;

    [SerializeField] private Collider2D col;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField]
    private bool hasAlternativeSprite;

    [SerializeField]
    private Sprite alternativeSprite;

    [SerializeField] private float distanceChangeToThrow;
    [SerializeField] private Vector3 offsetOnPick;

    [SerializeField] public UnityEvent onMouseDown;

    [SerializeField] private UnityEvent onMouseUp;
    [SerializeField] private UnityEvent onRemoveItem;
    private bool _canDestroy = true;
    private Sprite _initialSprite;
    private bool _isDone;
    private bool _isDragging;
    private LevelBase _levelBase;
    private Camera _mainCamera;

    public UnityEvent OnStartDrag => onMouseDown;
    public UnityEvent OnEndDrag => onMouseUp;
    // TEMP
    private ScalingOnPick _scalingOnPick;

    private Vector3 _startPos;

    public Collider2D Col => col;
    public SpriteRenderer SpriteRenderer => sr;

    public bool IsDragging => _isDragging;
    private void Awake()
    {
      _mainCamera = Camera.main;
      _scalingOnPick = GetComponent<ScalingOnPick>();
      _initialSprite = sr.sprite;
    }

    private void Start()
    {
      col.enabled = true;
      _levelBase = LevelBase.Instance;
      if (_levelBase) _levelBase.OnBlockPlayerInteractChanged += OnBlockPlayerInteractChanged;
    }

    private void OnDestroy()
    {
      if (_levelBase) _levelBase.OnBlockPlayerInteractChanged -= OnBlockPlayerInteractChanged;
    }

    private void OnMouseDown()
    {
      if (_isDone) return;
      if (!_levelBase.IsAllowInteract) return;
      if (_isDragging) return;
      _startPos = Tf.position;
      _startPos.z = 0;
      _isDragging = true;
      // AudioManager.PlaySFX(pickUpSound.clip, pickUpSound.volume);
      onMouseDown.Invoke();
      if (hasAlternativeSprite) sr.sprite = alternativeSprite;
    }

    private void OnMouseDrag()
    {
      if (_isDone) return;
      if (!_levelBase.IsAllowInteract) return;
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
      if (DistanceToInSqrVec2(_startPos) > distanceChangeToThrow)
      {
        Tf.SetParent(null);
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2;
        _isDone = true;
        // Delay destroy to avoid error
        onRemoveItem.Invoke();
        OnRemoveItemEv?.Invoke();
        sr.DOFade(0, 1f).OnComplete(() => Destroy(gameObject));
      }
      else
      {
        Tf.position = _startPos;
        if (hasAlternativeSprite) sr.sprite = _initialSprite;
        onMouseUp.Invoke();
      }
    }

    public event Action OnRemoveItemEv;

    private void OnBlockPlayerInteractChanged()
    {
      if (!_levelBase.IsAllowInteract)
      {
        _scalingOnPick?.isPreventScale.AddModifier(this);
        _canDestroy = false;
        OnMouseUp();
      }
      else
      {
        _scalingOnPick?.isPreventScale.RemoveModifier(this);
        _canDestroy = true;
      }
    }

#if UNITY_EDITOR


    private void GetReference()
    {
      sr = GetComponent<SpriteRenderer>();
      col = GetComponent<Collider2D>();
      rb = GetComponent<Rigidbody2D>();
    }

#endif
  }
}