using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HoangHH
{
  public class TapOpener : Item
  {
    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private int orderOnOpen;
    [SerializeField] private float localYOnOpen;
    [SerializeField] private bool isOpen;

    private int _initOrder;

    public bool IsOpen => isOpen;
    private Tween _openTween;
    private Vector3 _startPosition;

    protected override void Awake()
    {
      base.Awake();
      _startPosition = Tf.localPosition;
      _initOrder = sprite.sortingOrder;
    }

    private void Start()
    {
      SetOpen(isOpen, false, false);
    }

    public override void MouseUp(BaseEventData eventData)
    {
      if (isBlocked) return;
      base.MouseUp(eventData);
      if (!IsReady) return;
      SetOpen(!isOpen);
      IsReady = false;
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
      if (isOpen)
      {
        OnFinish?.Invoke();
      }

      if (playSound)
      {

        SoundManager.Ins.PlayFx(FxType.Pick);
      }
    }

    public void ForceChangeOpen(bool open)
    {
      SetOpen(open);
    }
  }
}