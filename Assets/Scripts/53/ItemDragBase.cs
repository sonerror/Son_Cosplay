using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Events;


#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif
public class ItemDragBase : Item
{
  public FxType soundPick = FxType.None;
  public FxType soundMove = FxType.None;
  public FxType soundDrop = FxType.None;
  public Renderer Rende;
  public int ValSortUp = 100;
  private int _ValSortGroup = 0;
  public Vector2 Angle = Vector2.zero;
  public Vector3 SizeUp = Vector3.one * 1.15f;
  protected Vector3 _originalSize;
  protected Vector3 _prePos;

  public bool isMouseUpCheck = true;

#if UNITY_EDITOR
  [FoldoutGroup("Event")]
#endif
  public UnityEvent OnRewound;

  protected override void Awake()
  {
    base.Awake();
    _prePos = transform.position;
    _originalSize = transform.localScale;

    _ValSortGroup = Rende.sortingOrder;


  }

  public override void MouseDown(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (isDragging) return;
    isDragging = true;
    if (!IsReady) { if (ShowEmoijOnWrong) GameManager.Ins.PlayNegativeEmoji(); }
    SoundManager.Ins.PlaySoundLoop(soundMove);
    base.MouseDown(eventData);
    OnActionMouseDown();
    OnHandleMouseDown();
  }

  public override void MouseUp(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (!isDragging) return;
    base.MouseUp(eventData);
    SoundManager.Ins.StopSoundLoop(soundMove);
    isDragging = false;
    OnActionMouseUp();
    OnHandleMouseUp();
  }

  public override void MouseDrag(BaseEventData eventData)
  {
    if (isBlocked || !isDragging) return;
    base.MouseDrag(eventData);
    OnActionMouseDrag();
    OnHandleMouseDrag();
  }
  public virtual void OnActionMouseDown()
  {
    OnPickItem?.Invoke();
  }
  public virtual void OnActionMouseDrag()
  {
    if (!isMouseUpCheck) CheckTarget();
  }
  public virtual void OnActionMouseUp()
  {
    OnDropItem?.Invoke();
    if (isMouseUpCheck) CheckTarget();
  }

  protected virtual void OnHandleMouseDown()
  {
    if (isBlocked) return;
    SoundManager.Ins.PlayFx(soundPick);
    Vector3 mouseWorldPos = GetMouseWorldPos();
    offSet = Tf.position - mouseWorldPos;
    Vector3 targetPos = mouseWorldPos + offSet;
    targetPos.z = 0;

    Tf.DOKill();
    Tf.DOScale(SizeUp, 0.2f);
    Tf.DORotate(Vector3.forward * Angle.y, 0.2f);
    ChangeLayerUp();
  }

  public virtual void OnHandleMouseDrag()
  {
    if (isBlocked) return;
    Vector3 targetPos = GetMouseWorldPos() + offSet;
    Vector3 pos = Tf.position;
    pos = Vector3.Lerp(pos, targetPos, 0.4f * Time.deltaTime * 50f);
    Tf.position = pos;
  }

  public virtual void OnHandleMouseUp()
  {
    if (isBlocked) return;

    Tf.DOKill();
    Tf.DOScale(_originalSize, 0.2f);
    Tf.DORotate(Vector3.forward * Angle.x, 0.2f);
    Tf.DOMove(_prePos, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
    {
      OnRewound?.Invoke();
      SoundManager.Ins.PlayFx(soundDrop);
      ChangeLayerDown();
    });

  }

  protected virtual void CheckTarget()
  {

  }

  public virtual void OnComplete()
  {

  }

  public virtual void OnIncorrectUse()
  {
    if (ShowEmoijOnWrong) GameManager.Ins.PlayNegativeEmoji();
  }

  protected void ChangeLayerUp()
  {

    if (!Rende) return;
    Rende.sortingOrder = _ValSortGroup + ValSortUp;
  }

  protected void ChangeLayerDown()
  {

    if (!Rende) return;
    Rende.sortingOrder = _ValSortGroup;
  }

}
