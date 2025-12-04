using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemMove : Item
{

  public float distanDoneToMove = 2f;
  public FxType fxSound = FxType.LipMove;
  private Vector3 _preMousePos;
  private Vector3 _mouseDownPos;
  private float deltaMove = 0f;

  public override void MouseDown(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (isDragging) return;
    if (GameManager.Ins.clockTimer.gameObject.activeSelf) return;
    if (!IsReady) return;
    SoundManager.Ins.PlayFx(FxType.Pick);
    isDragging = true;
    _preMousePos = GetMouseWorldPos();
    _mouseDownPos = GetMouseWorldPos();
    deltaMove = 0f;
  }

  public override void MouseDrag(BaseEventData eventData)
  {
    if (isBlocked || !isDragging) return;
    var mousePos = GetMouseWorldPos();
    deltaMove += Vector3.Distance(mousePos, _preMousePos);
    _preMousePos = mousePos;

    if (deltaMove >= distanDoneToMove)
    {
      IsReady = false;
      isBlocked = true;
      OnFinish?.Invoke();
      Move();
    }
  }
  public override void MouseUp(BaseEventData eventData)
  {
    base.MouseUp(eventData);
  }

  void Move()
  {
    Tf.DOKill();
    SoundManager.Ins.PlayFx(fxSound);
    var pos = GetMouseWorldPos();
    float dir = 1f;
    if (pos.x < _mouseDownPos.x) { dir = -1f; }
    Tf.DOMove(Tf.position + Vector3.right * 3f * dir, 0.5f).OnComplete(() =>
    {
      gameObject.SetActive(false);
    });
  }
}
