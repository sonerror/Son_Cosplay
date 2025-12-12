using System.Collections.Generic;
using DG.Tweening;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Blazer : Item
{
  public Vector2 Angle = new Vector2(0, 0);
  public SortingGroup ren;
  private int _oriOrder;
  private Vector3 _prePos;

  public Transform SecondaryNode;
  public Vector2 SecondaryAngle = new Vector2(0, 0);


  protected override void Awake()
  {
    base.Awake();
    _prePos = Tf.position;
    _oriOrder = ren.sortingOrder;
  }



  public override void MouseDown(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (isDragging) return;
    if (!IsReady) { if (ShowEmoijOnWrong) GameManager.Ins.PlayNegativeEmoji(); }

    base.MouseDown(eventData);

    OnHandleMouseDown();
  }

  public override void MouseUp(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (!isDragging) return;
    base.MouseUp(eventData);

    isDragging = false;
    OnDropItem?.Invoke();

    Tf.DOKill();
    Tf.DOScale(Vector3.one, 0.2f);
    Tf.DORotate(Vector3.forward * Angle.x, 0.2f);
    if (SecondaryNode)
    {
      SecondaryNode.DOKill();
      SecondaryNode.DOLocalRotate(Vector3.forward * SecondaryAngle.x, 0.2f);
    }
    Tf.DOMove(_prePos, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
    {
      SoundManager.Ins.PlayFx(FxType.Drop);
      ChangeLayerDown();
    });


  }

  public override void MouseDrag(BaseEventData eventData)
  {
    if (isBlocked || !isDragging) return;
    Vector3 targetPos = GetMouseWorldPos() + offSet;
    Vector3 pos = Tf.position;
    pos = Vector3.Lerp(pos, targetPos, 0.4f * Time.deltaTime * 50f);
    Tf.position = pos;
  }

  void OnHandleMouseDown()
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

    if (SecondaryNode)
    {
      SecondaryNode.DOKill();
      SecondaryNode.DOLocalRotate(Vector3.forward * SecondaryAngle.y, 0.2f);
    }
    ChangeLayerUp();
  }
  void ChangeLayerUp()
  {
    ren.sortingOrder = _oriOrder + 100;
  }

  void ChangeLayerDown()
  {
    ren.sortingOrder = _oriOrder;
  }

}
