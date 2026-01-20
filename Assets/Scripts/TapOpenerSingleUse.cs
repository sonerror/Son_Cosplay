using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HoangHH
{
  public class TapOpenerSingleUse : Item
  {
    [SerializeField] private Transform TfOpen;
    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Transform TargetPos;
    [SerializeField] private float TimeMove;
    [SerializeField] private bool DeactiveAfterUse;
    [SerializeField] public FxType SoundFx = FxType.None;

    public override void MouseUp(BaseEventData eventData)
    {
      if (isBlocked) return;
      base.MouseUp(eventData);
      if (!IsReady) return;
      SetOpen();
      SetBlockItem(true);
      SoundManager.Ins.PlayFx(SoundFx);
      IsReady = false;
    }

    private void SetOpen()
    {

      TfOpen.gameObject.SetActive(true);
      TfOpen.DOMove(TargetPos.position, TimeMove)
        .OnComplete(() =>
        {
          OnFinish?.Invoke();
          if (DeactiveAfterUse)
          {
            TfOpen.gameObject.SetActive(false);
          }
        });

      if (sprite)
      {
        sprite.DOFade(0, TimeMove);
      }
    }
  }
}