using DG.Tweening;
using HoangHH;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Costopia.Gameplay
{
  public class TriggerWithColAndTouch : MonoBehaviour
  {
    [SerializeField] protected bool inStepByStepLv = true;

    [SerializeField]
    private List<int> useInSteps;

    [SerializeField] private bool colliable;
    [SerializeField] protected Sprite objectLinkChangeToSpr;
    [SerializeField] protected Collider2D triggerWith;
    [SerializeField] protected Collider2D col;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected UnityEvent onTriggerEvent;
    [SerializeField] protected bool disableTriggerWith;
    [SerializeField] protected bool turnOffSimulated = true;
    [SerializeField] protected bool changeRbBodyType = false, multiCollie;

    [SerializeField]
    protected RigidbodyType2D bodyTypeCustom;

    [SerializeField] private bool needToOpen = false;

    [SerializeField]
    protected bool needChangeSpr = true;

    [SerializeField]
    protected Sprite changeToSpr;

    [SerializeField]
    protected SpriteRenderer thisSpr;

    [SerializeField]
    protected UnityEvent onClickEvent;


    protected bool _isDone, _isOpen = true;
    public bool IsDone => _isDone;
    public Collider2D Collider => col;
    public UnityEvent OnTriggerEvent => onTriggerEvent;
    public UnityEvent OnClickEvent => onClickEvent;

    public void EnableCol(bool value)
    {
      col.enabled = value;
    }

    public void ReSetupCollie()
    {
      ReSetupCollie(null, false);
    }
    public void ReSetupCollie(Collider2D newCol = null, bool deleteTriggerEv = false)
    {
      if (newCol) triggerWith = newCol;
      _isDone = false;
      col.enabled = true;
      if (turnOffSimulated) rb.simulated = true;
      if (deleteTriggerEv) onTriggerEvent.RemoveAllListeners();
    }

    public void AddUseInStep(int step)
    {
      useInSteps.Add(step);
    }

    private void Start()
    {
      if (needToOpen)
        _isOpen = false;
    }

    private void OnMouseDown()
    {
      if (inStepByStepLv) return;
      if (!needToOpen || _isOpen) return;
      _isOpen = true;
      if (needChangeSpr) thisSpr.sprite = changeToSpr;
      onClickEvent?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (inStepByStepLv) return;
      if (_isDone || !_isOpen || !colliable) return;
      if (other != triggerWith) return;
      onTriggerEvent.Invoke();
      if (multiCollie) return;
      _isDone = true;
      col.enabled = false;
      if (turnOffSimulated) rb.simulated = false;
      if (disableTriggerWith) triggerWith.enabled = false;
      if (changeRbBodyType)
      {
        rb.bodyType = bodyTypeCustom;
        OnChangeRbBodyType();
      }

      var otherSpr = other.GetComponent<SpriteRenderer>();
      if (otherSpr == null || objectLinkChangeToSpr == null) return;
      otherSpr.sprite = objectLinkChangeToSpr;
      otherSpr.enabled = true;
    }

    protected virtual void OnChangeRbBodyType()
    {
      rb.simulated = true;
      //rb.gravityScale = rb.gravityScale * 0.5f;
      transform.DORotate(Vector3.forward * Random.Range(-90, 90), 2f)
          .OnComplete(() => { gameObject.SetActive(false); });
    }

#if UNITY_EDITOR


    private void GetRef()
    {
      col = GetComponent<Collider2D>();
      rb = GetComponent<Rigidbody2D>();
      thisSpr = GetComponent<SpriteRenderer>();
    }

#endif
  }
}