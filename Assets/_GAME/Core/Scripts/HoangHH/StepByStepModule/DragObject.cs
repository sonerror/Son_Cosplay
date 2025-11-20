using System.Collections.Generic;
using DG.Tweening;


using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace HoangHH.LevelMakeUp
{
  public class DragObject : H3MonoBehaviour
  {
    [SerializeField] private List<int> useInSteps;
    [SerializeField] private GameObject sound;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Collider2D col;
    [SerializeField] private float moveBackTime = 0.3f;
    [SerializeField] private bool scalingWhenUse;
    [SerializeField] Rect triggerInteractZone = new Rect(-1, -1, 2, 2);
    [SerializeField] private bool isTriggerZone;
    [SerializeField] private UnityEvent onTriggerZoneEvent;
    [SerializeField] private bool hasFx;
    [SerializeField] private GameObject fx;

    [SerializeField] private int spriteOrderOnRelease;
    [SerializeField] private int spriteOrderOnDrag;
    [SerializeField] private float dragSlerpSpeed;
    [SerializeField] private Vector3 dragPosOffset;
    [SerializeField] private bool useInitialLocalPos;
    [SerializeField] private bool overrideInitialPos;
    [SerializeField] private Transform initialPosOverride;

    [SerializeField] private bool canRotate;
    [SerializeField] private float onDragRotateZ;
    private Tween _rotateTween;
    private float _initZRot;

    public UnityEvent onMouseDownEvent;
    public UnityEvent onMouseUpEvent;

    [SerializeField] private bool hasAlternativeSprite;
    [SerializeField] private Sprite alternativeSprite;
    private Sprite _originalSprite;

    private Transform _spriteTransform;
    private bool _isDragging;
    private bool _canInteract;
    private Vector3 _nextPos;
    private Camera _mainCamera;
    private Vector3 _initialPos;
    private float _initZ;

    private StepByStepLevel _levelBase;
    private Tween _scaleTween;
    private Tween _moveBackTween;
    private bool _isTriggerFaceZone;

    public float DragSpeed => DistanceToInSqrVec2(_nextPos);
    public bool IsDragging => _isDragging;

    private void SetInteract(bool canMove, bool changeCollider = true)
    {
      _canInteract = canMove;
      if (changeCollider)
      {
        col.enabled = canMove;
      }
    }

    private void OnBlockInteract()
    {
      if (!_levelBase.IsAllowInteract)
      {
        if (!_canInteract) return;
        OnMouseUp();
        SetInteract(false);
      }
      else
      {
        SetInteract(true);
      }
    }

    private void Start()
    {
      _spriteTransform = sprite.transform;
      if (hasAlternativeSprite)
      {
        _originalSprite = sprite.sprite;
      }
      _initZ = Tf.position.z;
      if (useInitialLocalPos)
      {
        _initialPos = Tf.localPosition;
      }
      else
      {
        _initialPos = Tf.position;
        if (overrideInitialPos)
        {
          _initialPos = initialPosOverride.position;
        }
      }
      _initZRot = Tf.eulerAngles.z;
      _nextPos = Tf.position;
      _mainCamera = Camera.main;
      SetInteract(true);
      if (scalingWhenUse)
      {
        _spriteTransform.localScale = Vector3.zero;
      }
      _levelBase = LevelBase.Instance as StepByStepLevel;
      if (_levelBase)
      {
        _levelBase.OnBlockPlayerInteractChanged += OnBlockInteract;
      }
    }

    private void Update()
    {
      if (_canInteract && _isDragging)
      {
        Tf.position = Vector3.Slerp(Tf.position, _nextPos, dragSlerpSpeed * Time.deltaTime);
        MouseDrag();
        if (_isTriggerFaceZone) return;
        _isTriggerFaceZone = triggerInteractZone.Contains(Tf.position);
        if (_isTriggerFaceZone) TriggerFace();
      }
    }

    private void TriggerFace()
    {
      if (useInSteps.Contains(_levelBase.CurrentStep))
      {
        if (isTriggerZone)
        {
          onTriggerZoneEvent.Invoke();
        }
        return;
      }
      _levelBase.OnWrongCurrentStep();
      OnMouseUp();
    }

    private void OnMouseDown()
    {
      if (!_canInteract) return;
      if (_isDragging) return;
      _nextPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
      _nextPos.z = _initZ;
      _nextPos += dragPosOffset;
      _isDragging = true;
      sprite.sortingOrder = spriteOrderOnDrag;
      // AudioManager.PlaySFX(pickUpSound.clip, pickUpSound.volume);
      // MMVibrationManager.Haptic(HapticTypes.Selection);
      _moveBackTween?.Kill();
      if (scalingWhenUse)
      {
        _scaleTween?.Kill();
        _scaleTween = _spriteTransform.DOScale(Vector3.one, moveBackTime);
      }
      if (canRotate)
      {
        _rotateTween?.Kill();
        _rotateTween = Tf.DORotate(new Vector3(0, 0, onDragRotateZ), moveBackTime);
      }
      if (hasAlternativeSprite)
      {
        sprite.sprite = alternativeSprite;
      }
      if (hasFx)
      {
        fx.SetActive(true);
      }

      if (ContainStep(_levelBase.CurrentStep))
      {
        sound.SetActive(true);
      }
      onMouseDownEvent.Invoke();
    }

    protected virtual void MouseDrag() { }

    private void OnMouseDrag()
    {
      if (!_canInteract) return;
      if (!_isDragging) return;
      _nextPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
      _nextPos.z = _initZ;
      _nextPos += dragPosOffset;
      MouseDrag();
    }

    public void OnMouseUp()
    {
      if (!_canInteract) return;
      if (!_isDragging) return;
      _isTriggerFaceZone = false;
      _isDragging = false;
      _nextPos = useInitialLocalPos ? Tf.TransformPoint(_initialPos) : _initialPos;
      SetInteract(false);
      _moveBackTween?.Kill();
      _moveBackTween = useInitialLocalPos ? Tf.DOLocalMove(_initialPos, moveBackTime) : Tf.DOMove(_initialPos, moveBackTime);
      _moveBackTween.OnComplete(() =>
      {
        sprite.sortingOrder = spriteOrderOnRelease;
        SetInteract(true);
      });

      if (scalingWhenUse)
      {
        _scaleTween?.Kill();
        _scaleTween = _spriteTransform.DOScale(Vector3.zero, moveBackTime);
      }
      if (canRotate)
      {
        _rotateTween?.Kill();
        _rotateTween = Tf.DORotate(new Vector3(0, 0, _initZRot), moveBackTime);
      }
      if (hasFx)
      {
        fx.SetActive(false);
      }
      if (hasAlternativeSprite)
      {
        sprite.sprite = _originalSprite;
      }
      sound.SetActive(false);
      onMouseUpEvent.Invoke();
      _levelBase.TryNextStep();
    }


    public bool ContainStep(int step)
    {
      return useInSteps.Contains(step);
    }
  }
}