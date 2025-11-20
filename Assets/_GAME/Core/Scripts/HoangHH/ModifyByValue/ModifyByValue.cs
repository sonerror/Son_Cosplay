using DG.Tweening;
using HoangHH;

using UnityEngine;

public abstract class ModifyByValue : H3MonoBehaviour
{
  [Header("Control Value")]
  [SerializeField]
  protected float startValue;

  [SerializeField][Range(0f, 1f)] protected float modifyValue;
  [SerializeField] protected bool applyModify = true;

  [Header("Cancel Modify")]
  [SerializeField]
  protected float timeBackToOrigin = 0.5f;

  [SerializeField] protected Ease easeBackToOrigin = Ease.InBack;

  [Header("Testing")][SerializeField] protected bool changeInEditor;

  private Tween _tweenBackToOrigin;

  public float ModifyValue => modifyValue;

  private void Awake()
  {
    SetValue(startValue);
  }

#if UNITY_EDITOR
  private void OnValidate()
  {
    if (!changeInEditor) return;
    if (!applyModify) return;
    OnModify(modifyValue);
  }
#endif


  public void ActiveModify(bool active)
  {
    applyModify = active;
    if (active)
    {
      OnModify(modifyValue);
    }
    else
    {
      _tweenBackToOrigin = DOVirtual.Float(modifyValue, startValue, timeBackToOrigin, x =>
      {
        OnModify(x);
        modifyValue = x;
      }).SetEase(easeBackToOrigin);
    }
  }

  public void SetValue(float value)
  {
    if (!applyModify) return;
    modifyValue = Mathf.Clamp01(value);
    _tweenBackToOrigin?.Kill();
    OnModify(modifyValue);
  }

  public void SetValueTween(float value)
  {
    if (!applyModify) return;
    float newValue = Mathf.Clamp01(value);
    _tweenBackToOrigin?.Kill();
    _tweenBackToOrigin = DOVirtual.Float(modifyValue, newValue, timeBackToOrigin, x =>
    {
      OnModify(x);
      modifyValue = x;
    }).SetEase(Ease.Linear);
  }

  public void AddValue(float value)
  {
    SetValue(modifyValue + value);
  }

  protected abstract void OnModify(float value);
}