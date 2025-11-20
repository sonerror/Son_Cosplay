using System.Collections.Generic;
using DG.Tweening;

using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace HoangHH.LevelMakeUp
{
  public class AccessoryObject : H3MonoBehaviour
  {
    [SerializeField] private List<int> useInSteps;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Collider2D col;
    [SerializeField] private float moveBackTime = 0.3f;
    [SerializeField] private bool scalingWhenUse;
    [SerializeField] Rect triggerInteractZone = new Rect(-1, -1, 2, 2);
    [SerializeField] private SnapPoint[] snapPoint;
    [SerializeField] private float distanceToAcceptSnap = 0.25f;
    [SerializeField] private bool isTriggerZone;
    [SerializeField] private UnityEvent onTriggerZoneEvent;


    [SerializeField] private int spriteOrderOnRelease;
    [SerializeField] private int spriteOrderOnDrag;
    [SerializeField] private float dragSlerpSpeed;
    [SerializeField] private Vector3 dragPosOffset;

    [Header("Event")]
    [SerializeField] private UnityEvent onMouseDownEvent;
    [SerializeField] private UnityEvent onMouseUpEvent;
    [SerializeField] private UnityEvent onSnapEvent;

    private Transform _spriteTransform;
    private bool _isDragging;
    private bool _canInteract;
    private Vector3 _nextPos;
    private Camera _mainCamera;
    private Vector3 _initialPos;

    private StepByStepLevel _levelBase;
    private Tween _scaleTween;
    private Tween _moveBackTween;
    private bool _isTriggerFaceZone;
    private bool _isSnap;

    private bool NearSnapPoint(out SnapPoint sp)
    {
      for (int i = 0; i < snapPoint.Length; i++)
      {
        if (snapPoint[i].isSnap) continue;
        if (!(DistanceToInSqrVec2(snapPoint[i].Tf.position) < distanceToAcceptSnap)) continue;
        sp = snapPoint[i];
        return true;
      }
      sp = null;
      return false;
    }

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
      _initialPos = Tf.position;
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
      if (_canInteract && _isDragging && !_isSnap)
      {
        Tf.position = Vector3.Slerp(Tf.position, _nextPos, dragSlerpSpeed * Time.deltaTime);
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
      if (_isSnap) return;
      if (!_canInteract) return;
      if (_isDragging) return;
      _nextPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
      _nextPos.z = _initialPos.z;
      _nextPos += dragPosOffset;
      _isDragging = true;
      sprite.sortingOrder = spriteOrderOnDrag;
      // AudioManager.PlaySFX(pickUpSound.clip, pickUpSound.volume);
      _moveBackTween?.Kill();
      if (scalingWhenUse)
      {
        _scaleTween?.Kill();
        _scaleTween = _spriteTransform.DOScale(Vector3.one, moveBackTime);
      }
      onMouseDownEvent.Invoke();
    }

    private void OnMouseDrag()
    {
      if (_isSnap) return;
      if (!_canInteract) return;
      if (!_isDragging) return;
      _nextPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
      _nextPos.z = _initialPos.z;
      _nextPos += dragPosOffset;
    }

    private void OnMouseUp()
    {
      if (_isSnap) return;
      if (!_canInteract) return;
      if (!_isDragging) return;
      _isTriggerFaceZone = false;
      _isDragging = false;
      _nextPos = _initialPos;
      sprite.sortingOrder = spriteOrderOnRelease;
      SetInteract(false);
      if (NearSnapPoint(out SnapPoint sp) && useInSteps.Contains(_levelBase.CurrentStep))
      {
        sp.isSnap = true;
        onSnapEvent.Invoke();
        // AudioManager.PlaySFX(snapSound.clip, snapSound.volume);
        _moveBackTween = Tf.DOMove(sp.Tf.position, moveBackTime);
        _isSnap = true;
        // attachToSnapPoint
        Tf.SetParent(sp.Tf);
        _levelBase.TryNextStep();
      }
      else
      {
        _moveBackTween = Tf.DOMove(_initialPos, moveBackTime)
            .OnComplete(() => SetInteract(true));
        if (scalingWhenUse)
        {
          _scaleTween?.Kill();
          _scaleTween = _spriteTransform.DOScale(Vector3.zero, moveBackTime);
        }
        onMouseUpEvent.Invoke();
      }
    }

  }
}