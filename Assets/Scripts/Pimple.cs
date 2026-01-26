using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pimple : Item
{
  private const string SLOT_ACNE_1 = "Draw_Acne";
  private const string SLOT_ACNE_2 = "Draw_Acne2";
  private const string SLOT_ACNE_3 = "Draw_Acne3";
  public Collider2D triggerCollider;
  public Vector2 Angle = new Vector2(0, 0);
  public Renderer rende;
  private int _oriOrder;
  private Vector3 _prePos;
  [SerializeField] public List<TriggerWithCertainCollider> acnePimpleColliders;


  protected override void Awake()
  {
    base.Awake();
    _prePos = Tf.position;
    _oriOrder = rende.sortingOrder;
  }

  void Start()
  {
    if (IsReady) OnReady();
  }

  public void OnReady()
  {
    IsReady = true;
    for (int index = 0; index < acnePimpleColliders.Count; index++)
    {
      int i = index; // Capture the index for the lambda
      TriggerWithCertainCollider trigger = acnePimpleColliders[i];
      trigger.gameObject.SetActive(true);
      trigger.OnTriggerEvent.AddListener(() => RemoveAcne(trigger, i));
    }
  }
  private List<string> _acneSlots = new List<string>()
        {
            SLOT_ACNE_1,
            SLOT_ACNE_2,
            SLOT_ACNE_3
        };
  private int _currentAcneIndex;
  private void RemoveAcne(TriggerWithCertainCollider trigger, int index)
  {
    trigger.OnTriggerEvent.RemoveAllListeners();
    trigger.gameObject.SetActive(false);
    // GamePlayManager.Ins.character.TurnSlotAttachment(_acneSlots[index]);
    PoolManager.Ins.Spawn(PoolType.SFX_Acne, trigger.transform.position, Quaternion.identity);
    _currentAcneIndex++;
    if (_currentAcneIndex >= _acneSlots.Count) OnDone();
  }

  void OnDone()
  {
    if (!IsReady) return;
    IsReady = false;
    for (int index = 0; index < acnePimpleColliders.Count; index++)
    {
      acnePimpleColliders[index].gameObject.SetActive(true);
    }
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
    rende.sortingOrder = _oriOrder + 100;
  }

  void ChangeLayerDown()
  {
    rende.sortingOrder = _oriOrder;
  }

}
