using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Brow_Shaver : Item
{
  public GameObject nodeTriger;
  public Vector2 Angle = new Vector2(0, 0);
  public Renderer rende;
  private int _oriOrder;
  private Vector3 _prePos;
  public bool isLeftDone = false;
  public bool isRightDone = false;
  public void OnRightDone()
  {
    isRightDone = true;
    if (isLeftDone)
    {
      OnDone();
    }
  }

  public void OnLeftDone()
  {
    isLeftDone = true;
    if (isRightDone)
    {
      OnDone();
    }
  }

  public void OnDone()
  {
    IsReady = false;
    OnFinish?.Invoke();
  }

  protected override void Awake()
  {
    base.Awake();
    _prePos = Tf.position;
    _oriOrder = rende.sortingOrder;
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  public override void MouseDown(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (isDragging) return;
    if (!IsReady) { OnWrong?.Invoke(); }
    else
    {
      nodeTriger.SetActive(true);
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
      ChangeLayerDown();
    });

    nodeTriger.SetActive(false);
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
    SoundManager.Ins.PlayFx(FxType.Click);

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
