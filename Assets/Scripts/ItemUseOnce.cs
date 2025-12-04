using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemUseOnce : Item
{
  private Vector3 _prePos;
  public Vector2 Angle = new Vector2(0, 0);
  public Renderer rende;
  public TargetInfo targetInfos = new TargetInfo();
  private int _oriOrder;
  protected override void Awake()
  {
    _oriOrder = rende.sortingOrder;
    base.Awake();
    _prePos = Tf.position;
    Angle.x = Tf.eulerAngles.z;
  }
  public override void MouseDown(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (isDragging) return;
    if (GameManager.Ins.clockTimer.gameObject.activeSelf) return;

    if (!IsReady) { OnWrong?.Invoke(); }
    base.MouseDown(eventData);
    OnHandleMouseDown();
  }

  protected virtual void OnHandleMouseDown()
  {
    isDragging = true;
    OnPickItem?.Invoke();
    SoundManager.Ins.PlayFx(FxType.Pick);
    Vector3 mouseWorldPos = GetMouseWorldPos();
    offSet = Tf.position - mouseWorldPos;
    Vector3 targetPos = mouseWorldPos + offSet;
    targetPos.z = 0;

    Tf.DOKill();
    Tf.DORotate(Vector3.forward * Angle.y, 0.2f);
    ChangeLayerUp();
  }


  public override void MouseDrag(BaseEventData eventData)
  {
    if (isBlocked || !isDragging) return;
    Vector3 targetPos = GetMouseWorldPos() + offSet;
    Vector3 pos = Tf.position;
    pos = Vector3.Lerp(pos, targetPos, 0.4f * Time.deltaTime * 50f);
    Tf.position = pos;

  }

  bool CheckToTarget()
  {
    if (targetInfos.targetTf == null) return false;
    float dist = Vector3.Distance(Tf.position, targetInfos.targetTf.position);
    if (dist <= targetInfos.distance)
    {
      IsReady = false;
      isBlocked = true;
      Tf.DOMove(targetInfos.targetTf.position, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
      {
        OnFinish?.Invoke();
        gameObject.SetActive(false);
        SoundManager.Ins.PlayFx(FxType.AddGel);
      });
      return true;
    }
    return false;
  }

  public override void MouseUp(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (!isDragging) return;
    base.MouseUp(eventData);
    if (GameManager.Ins.clockTimer.gameObject.activeSelf) return;


    isDragging = false;


    if (IsReady)
    {
      if (!CheckToTarget())
      {
        OnHandleMouseUp();
      }
    }
    else
    {
      OnHandleMouseUp();
    }
  }

  protected virtual void OnHandleMouseUp()
  {
    Debug.Log("aaaa");
    Tf.DOKill();
    Tf.DORotate(Vector3.forward * Angle.x, 0.1f);
    Tf.DOMove(_prePos, 0.1f).SetEase(Ease.OutBack).OnComplete(() =>
    {
      OnDropItem?.Invoke();
      ChangeLayerDown();
    });
  }
  void ChangeLayerUp()
  {
    rende.sortingOrder = _oriOrder + 100;
  }


  void ChangeLayerDown()
  {

    rende.sortingOrder = _oriOrder;

  }
}
