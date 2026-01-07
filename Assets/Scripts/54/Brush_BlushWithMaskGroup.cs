using System.Collections.Generic;
using DG.Tweening;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class BrushBlushWithMaskGroup : Item
{
  public Collider2D triggerCollider;
  public Vector2 Angle = new Vector2(0, 0);
  public Vector2 Scale = new Vector2(0, 0);
  public SortingGroup rendeGroup;
  private int _oriOrder;
  [SerializeField] private bool ShowCirBar = false;
  private Vector3 _prePos;
  [SerializeField] public MaskGroupCustom FlourFill;
  [SerializeField] public TriggerWithCertainCollider BoxFlour;
  public Transform Brush;
  public ParticleSystem brushOnPainterFx;
  [SerializeField] private BrushBlushState currBrushBlushState = BrushBlushState.Drag;
  public BrushBlushState CurrBrushBlushState => currBrushBlushState;

  protected override void Awake()
  {
    base.Awake();
    _prePos = Tf.position;
    _oriOrder = rendeGroup.sortingOrder;

    Angle.x = Tf.eulerAngles.z;
    Scale.x = Tf.localScale.x;
  }

#if UNITY_EDITOR
  [Button]
#endif
  public override void SetReady()
  {
    base.SetReady();

    if (currBrushBlushState == BrushBlushState.DragWithPainter)
    {
      FlourFill.ResetMask();
    }

    if (currBrushBlushState == BrushBlushState.Drag)
    {
      TriggerWithCertainCollider trigger = BoxFlour;
      trigger.gameObject.SetActive(true);
      trigger.OnTriggerEvent.AddListener(() => ChangeStatePainter(trigger));
    }
  }
  public Vector3 GetPosTargetActive()
  {

    return FlourFill.Tf.position;
  }
  void ChangeStatePainter(TriggerWithCertainCollider trigger)
  {
    trigger.OnTriggerEvent.RemoveAllListeners();
    SoundManager.Ins.PlayFx(FxType.AddGel);
    // SoundManager.Ins.PlayFx(FxType.MagicSparkle);
    // trigger.gameObject.SetActive(false);
    Brush.gameObject.SetActive(true);
    if (brushOnPainterFx) brushOnPainterFx.Play();
    currBrushBlushState = BrushBlushState.DragWithPainter;
    if (ShowCirBar) GameManager.Ins.fillCircleBar.Show();
    SetReady();
  }

  void OnDone()
  {
    if (!IsReady) return;
    IsReady = false;
    MouseUp(null);
    OnFinish?.Invoke();
  }

  public override void MouseDown(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (isDragging) return;
    if (!IsReady)
    {
      if (ShowEmoijOnWrong) GameManager.Ins.PlayNegativeEmoji();

    }
    else
    {
      if (ShowCirBar && currBrushBlushState == BrushBlushState.DragWithPainter) GameManager.Ins.fillCircleBar.Show();
      triggerCollider.gameObject.SetActive(true);
    }
    base.MouseDown(eventData);

    OnHandleMouseDown();
  }

  public override void MouseUp(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (!isDragging) return;
    base.MouseUp(eventData);
    if (ShowCirBar) GameManager.Ins.fillCircleBar.Hide();
    isDragging = false;
    OnDropItem?.Invoke();

    Tf.DOKill();
    Tf.DOScale(Scale.x, 0.2f);
    Tf.DORotate(Vector3.forward * Angle.x, 0.2f);
    Tf.DOMove(_prePos, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
    {
      SoundManager.Ins.PlayFx(FxType.Drop);
      ChangeLayerDown();
    });
    triggerCollider.gameObject.SetActive(false);
  }

  public override void MouseDrag(BaseEventData eventData)
  {
    if (isBlocked || !isDragging) return;
    Vector3 targetPos = GetMouseWorldPos() + offSet;
    Vector3 pos = Tf.position;
    pos = Vector3.Lerp(pos, targetPos, 0.4f * Time.deltaTime * 50f);
    Tf.position = pos;

    if (IsReady) CheckTarget();
  }

  void CheckTarget()
  {
    if (currBrushBlushState != BrushBlushState.DragWithPainter) return;

    if (FlourFill.CheckMasksColider(Brush.position))
    {
      if (ShowCirBar) GameManager.Ins.fillCircleBar.Fill(FlourFill.GetPercentFill());
      if (FlourFill.IsDoneRating(true))
      {
        if (ShowCirBar) GameManager.Ins.fillCircleBar.Fill(FlourFill.GetPercentFill());
        Debug.Log(FlourFill.GetPercentFill());
        Debug.Log("Done");
        OnDone();
      }

    }



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
    Tf.DOScale(Scale.y, 0.2f);
    Tf.DORotate(Vector3.forward * Angle.y, 0.2f);
    ChangeLayerUp();
  }
  void ChangeLayerUp()
  {
    rendeGroup.sortingOrder = _oriOrder + 100;
  }

  void ChangeLayerDown()
  {
    rendeGroup.sortingOrder = _oriOrder;
  }

}
