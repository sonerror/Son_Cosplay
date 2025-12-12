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
  public SortingGroup RenderGroup;
  public int ValSortUp = 0;
  private int _ValSortGroup = 0;
  public Vector2 Angle = Vector2.zero;
  public Vector3 SizeUp = Vector3.one * 1.15f;
  protected Vector3 _originalSize;
  protected Vector3 _prePos;

  public bool isMouseUpCheck = true;
  public bool isHideOnDone = true;
  [SerializeField] protected float dropDistance = 1f;
  [SerializeField] protected Transform target, center;

#if UNITY_EDITOR
  [FoldoutGroup("Event")]
#endif
  public UnityEvent OnRewound;

  protected override void Awake()
  {
    base.Awake();
    _prePos = transform.position;
    _originalSize = transform.localScale;
    RenderGroup = GetComponent<SortingGroup>();
    if (RenderGroup)
    {
      _ValSortGroup = RenderGroup.sortingOrder;
    }

    if (!center) center = Tf;

  }

  public override void MouseDown(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (isDragging) return;
    isDragging = true;
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

  }
  public virtual void OnActionMouseUp()
  {
    OnDropItem?.Invoke();
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

  protected void CheckTarget()
  {
    if (!IsInRange(center.position, target.position)) return;

    if (IsReady) OnComplete();
    else if (isMouseUpCheck) OnIncorrectUse();
  }

  protected bool IsInRange(Vector3 pos, Vector3 target, float rangeRation = 1f)
  {
    return (Vector2.Distance(pos, target) < dropDistance * rangeRation);
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
    if (!RenderGroup) return;
    RenderGroup.sortingOrder = _ValSortGroup + ValSortUp;
  }

  protected void ChangeLayerDown()
  {
    if (!RenderGroup) return;
    RenderGroup.sortingOrder = _ValSortGroup;
  }

}
