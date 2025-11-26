using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace HoangHH
{
  public class TapOpener : H3MonoBehaviour
  {
    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private int orderOnOpen;
    [SerializeField] private float localYOnOpen;
    [SerializeField] private bool isOpen;


    private bool _isBlocking;
    private int _initOrder;

    public void SetBlockAction(bool block)
    {
      _isBlocking = block;
    }

    public Action<bool> onOpenChange;
    public UnityEvent onOpen;
    public UnityEvent onClose;
    public bool IsOpen => isOpen;
    private Tween _openTween;
    private Vector3 _startPosition;

    private void Awake()
    {
      _startPosition = Tf.localPosition;
      _initOrder = sprite.sortingOrder;
    }

    private void Start()
    {
      SetOpen(isOpen, false, false);
    }

    private void OnMouseUpAsButton()
    {
      if (_isBlocking) return;
      SetOpen(!isOpen);
    }

    private void SetOpen(bool open, bool useTween = true, bool playSound = true)
    {
      isOpen = open;
      if (useTween)
      {
        _openTween?.Kill();
        _openTween = Tf.DOLocalMoveY(isOpen ? localYOnOpen : _startPosition.y, 0.3f)
            .OnComplete(() =>
            {
              if (!isOpen) sprite.sortingOrder = isOpen ? orderOnOpen : _initOrder;
            });
        if (isOpen) sprite.sortingOrder = isOpen ? orderOnOpen : _initOrder;
      }
      else
      {
        Tf.localPosition = _startPosition;
        sprite.sortingOrder = isOpen ? orderOnOpen : _initOrder;
      }
      onOpenChange?.Invoke(isOpen);
      if (isOpen)
      {
        onOpen.Invoke();
      }
      else
      {
        onClose.Invoke();
      }
      if (playSound)
      {
        // AudioManager.PlaySFX(isOpen ? openSound : closeSound);
        // MMVibrationManager.Haptic(HapticTypes.LightImpact);
      }
    }

    public void ForceChangeOpen(bool open)
    {
      SetOpen(open);
    }
  }
}