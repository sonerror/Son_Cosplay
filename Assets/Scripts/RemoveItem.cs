using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class RemoveItem : Item
{
  [SerializeField]
  SpriteRenderer spriteRenderer;
  public float DistanceCheck = 1f;
  private Vector3 _prePos;
  private int _oriOrder;

  public CharacterStart characterStart;
  [SerializeField] public FxType SoundFx = FxType.None;

  public List<SlotAttachmentPair> slotActiveClick = new List<SlotAttachmentPair>();
  public List<SlotAttachmentPair> slotDeactiveClick = new List<SlotAttachmentPair>();

  protected override void Awake()
  {
    base.Awake();
    _prePos = Tf.position;
    _oriOrder = spriteRenderer.sortingOrder;
  }

  private Vector3 PosMouseDown;

  public override void MouseDown(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (isDragging) return;
    base.MouseDown(eventData);
    OnPickItem?.Invoke();
    if (!IsReady) { if (ShowEmoijOnWrong) GameManager.Ins.PlayNegativeEmoji(); return; }
    else
    {
      characterStart.TurnOnSlotsAttachment(slotActiveClick);
      characterStart.TurnOffSlotsAttachment(slotDeactiveClick);
      spriteRenderer.enabled = true;
    }


    isDragging = true;
    PosMouseDown = GetMouseWorldPos();
    offSet = Tf.position - PosMouseDown;
    ChangeLayerUp();
  }

  public override void MouseUp(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (!isDragging) return;
    base.MouseUp(eventData);


    if (IsReady)
    {
      var CurrPos = GetMouseWorldPos();
      if (Vector3.Distance(PosMouseDown, CurrPos) > DistanceCheck)
      {
        IsReady = false;
        SetBlockItem(true);
        SoundManager.Ins.PlayFx(SoundFx);
        Vector3 newPosMove = Tf.position + Vector3.down * 2f;
        OnFinish?.Invoke();
        Tf.DOMove(newPosMove, 0.75f).OnComplete(() =>
        {
          gameObject.SetActive(false);
          isDragging = false;
        });
        spriteRenderer.DOFade(0f, 0.75f);

        return;
      }
    }

    OnDropItem?.Invoke();
    Tf.DOKill();
    Tf.DOMove(_prePos, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
    {
      isDragging = false;
      ChangeLayerDown();
      characterStart.TurnOnSlotsAttachment(slotDeactiveClick);
      characterStart.TurnOffSlotsAttachment(slotActiveClick);
      spriteRenderer.enabled = false;
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

  void ChangeLayerUp()
  {
    spriteRenderer.sortingOrder = _oriOrder + 100;
  }

  void ChangeLayerDown()
  {
    spriteRenderer.sortingOrder = _oriOrder;
  }
}

