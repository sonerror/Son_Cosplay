using System.Collections.Generic;
using DG.Tweening;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public enum BrushEyeState
{
  None,
  Blue,
  Pink
}

public class Brush_Eye : Item
{

  private Vector3 _prePos;
  public Vector2 Angle = new Vector2(0, 0);
  public SortingGroup rende;
  private int _oriOrder = 0;
  public MaskGroupCustom maskGroupBlue, maskGroupPink;
  public GameObject headBrushBlue, headBrushPink;
  public ParticleSystem parBlue, parPink, _CurrPar = null, MagicPar = null;
  public Transform NodeCheckPos;
  [SerializeField] public List<TriggerWithCertainCollider> CertainColliders;

  private BrushEyeState _CurrBrushEyeState = BrushEyeState.None;
  public BrushEyeState CurrBrushEyeState => _CurrBrushEyeState;

  protected override void Awake()
  {
    base.Awake();
    _prePos = Tf.position;
    Angle.x = Tf.eulerAngles.z;
    _oriOrder = rende.sortingOrder;
    if (!NodeCheckPos) NodeCheckPos = Tf;
  }

  void Start()
  {
    if (IsReady) SetReady();
  }

#if UNITY_EDITOR
  [Button]
#endif
  public void SetReady()
  {
    IsReady = true;
    _OnReady();
  }

  private void _OnReady()
  {
    for (int index = 0; index < CertainColliders.Count; index++)
    {
      int i = index; // Capture the index for the lambda  
      TriggerWithCertainCollider trigger = CertainColliders[i];
      trigger.gameObject.SetActive(true);
      trigger.OnTriggerEvent.AddListener(() => OnColi(trigger, i));
    }
  }

  private void OnColi(TriggerWithCertainCollider trigger, int index)
  {
    Debug.Log("OnColi Brush Eye " + index);
    if (index == 0) { ChangeTypeToBlue(); }
    else { ChangeTypeToPink(); }
  }

  private bool hadShowEyeMask = false;
  void ChangeTypeToBlue()
  {
    if (!hadShowEyeMask)
    {
      maskGroupBlue.gameObject.SetActive(true);
      maskGroupPink.gameObject.SetActive(true);
      hadShowEyeMask = true;
    }
    if (_CurrBrushEyeState == BrushEyeState.Blue) return;
    _CurrBrushEyeState = BrushEyeState.Blue;
    if (_CurrPar) _CurrPar.Stop();
    _CurrPar = parBlue;
    if (isDragging) _CurrPar.Play();
    SoundManager.Ins.PlayFx(FxType.MagicSparkle);
    MagicPar.Play();
    headBrushBlue.SetActive(true);
    headBrushPink.SetActive(false);
  }

  void ChangeTypeToPink()
  {
    if (!hadShowEyeMask)
    {
      maskGroupBlue.gameObject.SetActive(true);
      maskGroupPink.gameObject.SetActive(true);
      hadShowEyeMask = true;
    }
    if (_CurrBrushEyeState == BrushEyeState.Pink) return;
    _CurrBrushEyeState = BrushEyeState.Pink;
    if (_CurrPar) _CurrPar.Stop();
    _CurrPar = parPink;
    if (isDragging) _CurrPar.Play();
    SoundManager.Ins.PlayFx(FxType.MagicSparkle);
    MagicPar.Play();
    headBrushBlue.SetActive(false);
    headBrushPink.SetActive(true);
  }

  public override void MouseDown(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (isDragging) return;
    if (!IsReady) { OnWrong?.Invoke(); }
    base.MouseDown(eventData);

    OnHandleMouseDown();
  }

  protected virtual void OnHandleMouseDown()
  {
    isDragging = true;
    OnPickItem?.Invoke();
    // SoundManager.Ins.PlaySoundLoop(fxSound);
    // SoundManager.Ins.PlayFx(FxType.Click);
    if (IsReady) GameManager.Ins.characterControl.CloseEye();
    if (_CurrPar) _CurrPar.Play();
    Vector3 mouseWorldPos = GetMouseWorldPos();
    offSet = Tf.position - mouseWorldPos;
    Vector3 targetPos = mouseWorldPos + offSet;
    targetPos.z = 0;

    Tf.DOKill();
    Tf.DOScale(Vector3.one * 1.15f, 0.2f);
    Tf.DORotate(Vector3.forward * Angle.y, 0.2f);
    ChangeLayerUp();
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
    if (_CurrBrushEyeState == BrushEyeState.None) return;

    if (_CurrBrushEyeState == BrushEyeState.Blue)
    {
      if (maskGroupBlue.CheckMasks(NodeCheckPos.position))
      {
        if (maskGroupPink.IsDone())
        {
          IsReady = false;
          OnFinish?.Invoke();
        }
      }
    }

    if (_CurrBrushEyeState == BrushEyeState.Pink)
    {
      if (maskGroupPink.CheckMasks(NodeCheckPos.position))
      {
        if (maskGroupBlue.IsDone())
        {
          IsReady = false;
          OnFinish?.Invoke();
        }
      }
    }
  }

  public override void MouseUp(BaseEventData eventData)
  {
    if (isBlocked) return;
    if (!isDragging) return;
    isDragging = false;
    OnDropItem?.Invoke();
    OnHandleMouseUp();
  }

  protected virtual void OnHandleMouseUp()
  {
    if (_CurrPar) _CurrPar.Stop();
    Tf.DOKill();
    Tf.DOScale(Vector3.one, 0.3f);
    Tf.DORotate(Vector3.forward * Angle.x, 0.3f);
    Tf.DOMove(_prePos, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
    {
      ChangeLayerDown();
    });


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
