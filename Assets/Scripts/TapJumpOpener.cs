using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HoangHH
{
  public class TapJumpOpener : Item
  {
    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private int orderOnOpen;
    [SerializeField] private Transform targetLocal;
    [SerializeField] private float jumpPower = 2f;
    [SerializeField] private float time = 1f;
    [SerializeField] private bool isRotate = false;

    [SerializeField] private FxType SoundFx = FxType.None;

    public override void SetReady()
    {
      base.SetReady();
      col.enabled = true;
    }


    public override void MouseUp(BaseEventData eventData)
    {
      if (isBlocked) return;
      base.MouseUp(eventData);
      SetOpen();
    }

    private void SetOpen()
    {
      if (isBlocked) return;
      if (!IsReady) return;
      IsReady = false;
      Debug.Log("TapJumpOpener SetOpen");
      OnFinish?.Invoke();
      col.enabled = false;
      SoundManager.Ins.PlayFx(SoundFx);
      Tf.DOJump(targetLocal.position, jumpPower, 1, time).SetEase(Ease.OutQuad);

      if (isRotate)
      {
        Tf.DORotate(targetLocal.eulerAngles, time, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);
      }
    }
  }
}