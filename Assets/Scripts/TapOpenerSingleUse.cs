using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HoangHH
{
  public class TapOpenerSingleUse : Item
  {
    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Vector3 Posnew;
    [SerializeField] private float TimeMove;
    [SerializeField] private bool UseFade;
    [SerializeField] private bool DeactiveAfterUse;
    [SerializeField] public FxType SoundFx = FxType.None;

    public override void MouseUp(BaseEventData eventData)
    {
      if (isBlocked) return;
      base.MouseUp(eventData);
      if (!IsReady) return;
      SetOpen();
      SoundManager.Ins.PlayFx(SoundFx);
      IsReady = false;
    }

    private void SetOpen()
    {
      Tf.DOLocalMove(Posnew, TimeMove)
        .OnComplete(() =>
        {
          OnFinish?.Invoke();
          if (DeactiveAfterUse)
          {
            gameObject.SetActive(false);
          }
        });

      if (UseFade)
      {
        sprite.DOFade(0, TimeMove);
      }
    }
  }
}