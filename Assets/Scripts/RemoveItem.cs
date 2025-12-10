using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class RemoveItem : Item
{
  public float DistanceCheck = 1f;

  protected override void Awake()
  {
    base.Awake();
  }

  private Vector3 PosMouseDown;

  public override void MouseDown(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (isDragging) return;
    base.MouseDown(eventData);
    if (!IsReady) { OnWrong?.Invoke(); return; }


    isDragging = true;
    PosMouseDown = GetMouseWorldPos();
  }

  public override void MouseUp(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (!isDragging) return;
    base.MouseUp(eventData);

    isDragging = false;
    OnDropItem?.Invoke();
  }

  public override void MouseDrag(BaseEventData eventData)
  {
    if (isBlocked || !isDragging) return;
    var CurrPos = GetMouseWorldPos();
    if (Vector3.Distance(PosMouseDown, CurrPos) > DistanceCheck)
    {
      IsReady = false;
      OnPickItem?.Invoke();
      Vector3 targetPos = Tf.position;
      if (CurrPos.x > PosMouseDown.x)
      {
        targetPos += Vector3.right * 15f + Vector3.down * 4f;
      }
      else
      {
        targetPos += Vector3.left * 15f + Vector3.down * 4f;
      }
      OnFinish?.Invoke();
      Tf.DOJump(targetPos, 1f, 1, 0.75f).OnComplete(() =>
      {
        gameObject.SetActive(false);
      });
      MouseUp(eventData);
    }
  }
}
