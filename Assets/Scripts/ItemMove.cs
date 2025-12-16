using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemMove : Item
{
  public float distanDoneToMove = 2f;
  public FxType fxSound = FxType.LipMove;
  private Vector3 _preMousePos;
  private Vector3 _mouseDownPos;
  private float deltaMove = 0f;

  public void SetReady()
  {
    IsReady = true;
  }

  private Material mat = null;

  protected override void Awake()
  {
    base.Awake();
    mat = GetComponent<SpriteRenderer>().material;
  }

  public void SetActiveBorder()
  {
    var rendererr = GetComponent<SpriteRenderer>();
    var material = new Material(rendererr.material);
    rendererr.material = material;
    material.SetFloat("_CurlProgress", 0.9f);
  }

  public override void MouseDown(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (isDragging) return;
    if (GameManager.Ins.clockTimer.gameObject.activeSelf) return;
    if (!IsReady) return;
    SoundManager.Ins.PlayFx(FxType.Pick);
    isDragging = true;
    _preMousePos = GetMouseWorldPos();
    _mouseDownPos = GetMouseWorldPos();
    deltaMove = 0f;
  }

  public override void MouseDrag(BaseEventData eventData)
  {
    if (isBlocked || !isDragging) return;
    var mousePos = GetMouseWorldPos();
    deltaMove += Vector3.Distance(mousePos, _preMousePos);
    _preMousePos = mousePos;

    if (deltaMove >= distanDoneToMove)
    {
      IsReady = false;
      isBlocked = true;
      OnFinish?.Invoke();
      Move();
    }
  }
  public override void MouseUp(BaseEventData eventData)
  {
    base.MouseUp(eventData);
  }

  void Move()
  {
    // Tf.DOKill();
    // 
    // var pos = GetMouseWorldPos();
    // float dir = 1f;
    // if (pos.x < _mouseDownPos.x) { dir = -1f; }
    // Tf.DOMove(Tf.position + Vector3.right * 3f * dir, 0.5f).OnComplete(() =>
    // {
    //   gameObject.SetActive(false);
    // });
    SoundManager.Ins.PlayFx(fxSound);
    var rendererr = GetComponent<SpriteRenderer>();
    var material = new Material(rendererr.material);
    rendererr.material = material;
    float curlValue = 0.925f;
    material.SetFloat("_CurlProgress", curlValue);
    DOTween.To(
          () => curlValue,
          x =>
          {
            curlValue = x;
            material.SetFloat("_CurlProgress", curlValue);
          },
         0f,    // giá trị đích
          0.65f     // thời gian tween (1 giây)
      )
      .OnComplete(() =>
      {
        gameObject.SetActive(false);
      })
      .SetEase(Ease.InOutSine);
  }
}
