using System.Collections.Generic;
using DG.Tweening;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
public enum BrushBlushState
{
  Drag,
  DragWithPainter,
  Done
}
public class Brush_Blush : Item
{
  public Collider2D triggerCollider;
  public Vector2 Angle = new Vector2(0, 0);
  public SortingGroup rendeGroup;
  private int _oriOrder;
  private Vector3 _prePos;
  [SerializeField] public List<TriggerWithCertainCollider> phanColliders;
  [SerializeField] public List<ShowSprite> spriteShows;
  [SerializeField] public TriggerWithCertainCollider hopPhanColliders;
  public GameObject brushOnPainter;
  public ParticleSystem brushOnPainterFx;
  [SerializeField] private BrushBlushState currBrushBlushState = BrushBlushState.Drag;

  protected override void Awake()
  {
    base.Awake();
    _prePos = Tf.position;
    _oriOrder = rendeGroup.sortingOrder;
  }

  void Start()
  {
    if (IsReady) OnReady();
  }
#if UNITY_EDITOR
  [Button]
#endif
  public void OnReady()
  {
    IsReady = true;

    if (currBrushBlushState == BrushBlushState.DragWithPainter)
    {
      for (int index = 0; index < phanColliders.Count; index++)
      {
        int i = index; // Capture the index for the lambda
        TriggerWithCertainCollider trigger = phanColliders[i];
        ShowSprite show = spriteShows[i];
        trigger.gameObject.SetActive(true);
        trigger.OnTriggerEvent.AddListener(() => AddPhan(trigger, show, i));
      }
    }

    if (currBrushBlushState == BrushBlushState.Drag)
    {
      TriggerWithCertainCollider trigger = hopPhanColliders;
      trigger.gameObject.SetActive(true);
      trigger.OnTriggerEvent.AddListener(() => ChangeStatePainter(trigger));
    }
  }

  void ChangeStatePainter(TriggerWithCertainCollider trigger)
  {
    trigger.OnTriggerEvent.RemoveAllListeners();
    trigger.gameObject.SetActive(false);
    brushOnPainter.SetActive(true);
    brushOnPainterFx.Play();
    currBrushBlushState = BrushBlushState.DragWithPainter;
    OnReady();
  }
  private int _currentAcneIndex;
  private void AddPhan(TriggerWithCertainCollider trigger, ShowSprite show, int index)
  {
    trigger.OnTriggerEvent.RemoveAllListeners();
    trigger.gameObject.SetActive(true);
    show.ShowSpriteStart();
    // PoolManager.Ins.Spawn(PoolType.SFX_Acne, trigger.transform.position, Quaternion.identity);
    _currentAcneIndex++;
    if (_currentAcneIndex >= phanColliders.Count) OnDone();
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
    if (!IsReady) { OnWrong?.Invoke(); }
    else
    {
      triggerCollider.enabled = true;
    }
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
    Tf.DOMove(_prePos, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
    {
      SoundManager.Ins.PlayFx(FxType.Drop);
      ChangeLayerDown();
    });
    triggerCollider.enabled = false;
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
