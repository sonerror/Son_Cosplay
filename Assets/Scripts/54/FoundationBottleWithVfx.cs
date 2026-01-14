using System.Collections.Generic;
using DG.Tweening;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

using UnityEngine;
using UnityEngine.EventSystems;

public class FoundationBottleWithFx : Item
{
  public Collider2D triggerCollider;
  public Vector2 Angle = new Vector2(0, 0);
  public Renderer rende;
  private int _oriOrder;
  private Vector3 _prePos;
  [SerializeField] public List<TriggerWithCertainCollider> acnePimpleColliders;
  [SerializeField] public List<ShowSprite> spriteShows;
  public ParticleSystem applyFx;


  protected override void Awake()
  {
    base.Awake();
    _prePos = Tf.position;
    _oriOrder = rende.sortingOrder;
  }

  void Start()
  {
    if (IsReady) SetReady();
  }
#if UNITY_EDITOR
  [Button]
#endif
  public override void SetReady()
  {
    base.SetReady();
    for (int index = 0; index < acnePimpleColliders.Count; index++)
    {
      int i = index; // Capture the index for the lambda
      TriggerWithCertainCollider trigger = acnePimpleColliders[i];
      ShowSprite show = spriteShows[i];
      trigger.gameObject.SetActive(true);
      trigger.OnTriggerEvent.AddListener(() => AddAcne(trigger, show, i));
    }
  }
  private int _currentAcneIndex;
  private void AddAcne(TriggerWithCertainCollider trigger, ShowSprite show, int index)
  {
    trigger.OnTriggerEvent.RemoveAllListeners();
    trigger.gameObject.SetActive(true);
    trigger.Col.enabled = false;
    show.ShowSpriteStart();
    applyFx.Play();
    // PoolManager.Ins.Spawn(PoolType.SFX_Acne, trigger.transform.position, Quaternion.identity);
    _currentAcneIndex++;
    if (_currentAcneIndex >= acnePimpleColliders.Count) OnDone();
  }

  public Vector3 getPosTargetActive()
  {
    for (int i = 0; i < acnePimpleColliders.Count; i++)
    {
      if (acnePimpleColliders[i].Col.enabled)
      {
        return acnePimpleColliders[i].Tf.position;
      }
    }
    return Vector3.zero;
  }

  void OnDone()
  {
    if (!IsReady) return;
    IsReady = false;
    OnFinish?.Invoke();
  }

  public override void MouseDown(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (isDragging) return;
    if (!IsReady) { if (ShowEmoijOnWrong) GameManager.Ins.PlayNegativeEmoji(); }
    else
    {
      triggerCollider.enabled = true;
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
    triggerCollider.gameObject.SetActive(false);

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
    rende.sortingOrder = _oriOrder + 100;
  }

  void ChangeLayerDown()
  {
    rende.sortingOrder = _oriOrder;
  }

}
