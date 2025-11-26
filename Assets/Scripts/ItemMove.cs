using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemMove : Item
{

  public float distanDoneToMove = 2f;

  private Vector3 _preMousePos;
  private Vector3 _mouseDownPos;
  private float deltaMove = 0f;

  public override void MouseDown(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (!IsReady) return;
    _preMousePos = GetMouseWorldPos();
    _mouseDownPos = GetMouseWorldPos();
    deltaMove = 0f;
  }

  public override void MouseDrag(BaseEventData eventData)
  {
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
  public override void MouseUp(BaseEventData eventData) { }

  void Move()
  {
    Tf.DOKill();
    var pos = GetMouseWorldPos();
    float dir = 1f;
    if (pos.x < _mouseDownPos.x) { dir = -1f; }
    Tf.DOMove(Tf.position + Vector3.right * 3f * dir, 0.5f).OnComplete(() =>
    {
      gameObject.SetActive(false);
    });
  }
}
