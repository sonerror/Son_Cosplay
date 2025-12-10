using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

public class HairBang : Item
{
  private Vector3 _prePos;
  public Vector2 Angle = new Vector2(0, 0);
  public List<Renderer> renderers = new List<Renderer>();
  public TargetInfo targetInfos = new TargetInfo();

  protected override void Awake()
  {
    base.Awake();
    _prePos = Tf.position;
    Angle.x = Tf.eulerAngles.z;
  }

  public void SetReady()
  {
    IsReady = true;
  }
  public override void MouseDown(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (isDragging) return;
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
    Tf.DOScale(Vector3.one * 1.15f, 0.2f);
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
    if (IsReady) CheckToTarget();
  }

  void CheckToTarget()
  {
    if (targetInfos.targetTf == null) return;
    float dist = Vector3.Distance(Tf.position, targetInfos.targetTf.position);
    if (dist <= targetInfos.distance)
    {
      IsReady = false;
      isBlocked = true;

      Tf.DOMove(targetInfos.targetTf.position, 0.35f).SetEase(Ease.OutBack).OnComplete(() =>
      {
        OnFinish?.Invoke();
        // gameObject.SetActive(false);
      });
    }
  }

  public override void MouseUp(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (!isDragging) return;
    isDragging = false;
    OnDropItem?.Invoke();
    OnHandleMouseUp();
    base.MouseUp(eventData);

  }

  protected virtual void OnHandleMouseUp()
  {
    Tf.DOKill();
    Tf.DOScale(Vector3.one, 0.3f);
    Tf.DORotate(Vector3.forward * Angle.x, 0.3f);
    Tf.DOMove(_prePos, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
    {
      ChangeLayerDown();
    });


  }
  void ChangeLayerUp()
  {
    foreach (var renderer in renderers)
    {
      renderer.sortingOrder += 100;
    }
  }

  void ChangeLayerDown()
  {
    foreach (var renderer in renderers)
    {
      renderer.sortingOrder -= 100;
    }
  }
}
