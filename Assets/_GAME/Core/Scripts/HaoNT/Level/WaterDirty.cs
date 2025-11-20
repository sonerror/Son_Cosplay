using DG.Tweening;

using System;
using UnityEngine;

namespace Costopia.Gameplay
{
  public class WaterDirty : MonoBehaviour
  {
    [SerializeField] private SpriteRenderer waterRen;
    [SerializeField] private bool startFade = true;

    [SerializeField] private bool isPaintEffectable = true;
    // [SerializeField] private CwChangeCounterEvent p3d;

    [SerializeField] private bool isCollideEffectable = true;
    [SerializeField] private bool disableColAfterDone;
    [SerializeField] private Collider2D thisCollider;

    private void Awake()
    {
      if (startFade)
      {
        if (waterRen != null)
        {
          waterRen.DOFade(0, 0f);
        }
        else
        {
          waterRen = GetComponent<SpriteRenderer>();
          waterRen.DOFade(0, 0f);
        }
      }
    }
    public enum State
    {
      None,
      Scale,
      Active,
      Deactive,
      Done
    }
    State state = State.None;
    public bool IsFadeDone => IsState(State.Done);

    private Action _onDoneFadeInAc, _onDoneFadeOutAc;
    private void ChangeState<T>(T t)
    {
      waterRen.transform.DOKill();
      this.state = (State)(object)t;
      switch (state)
      {
        case State.None:
          if (gameObject.activeSelf) gameObject.SetActive(false);
          break;
        case State.Scale:
          break;
        case State.Active:

          if (!gameObject.activeSelf) gameObject.SetActive(true);

          if (isCollideEffectable)
          {
            waterRen.DOFade(0.5f, 0.75f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
              waterRen.DOFade(1, 0.75f).OnComplete(() =>
                          {
                            ChangeState(State.Done);
                          });
            });
          }
          else
          {
            waterRen.DOFade(1, 1f).OnComplete(() =>
            {
              ChangeState(State.Done);
              if (_onDoneFadeInAc != null)
              {
                _onDoneFadeInAc();
                _onDoneFadeInAc = null;
              }
            });
          }
          break;
        case State.Deactive:
          waterRen.DOFade(0, 1.5f).OnComplete(() =>
          {
            ChangeState(State.None);
            if (_onDoneFadeOutAc != null)
            {
              _onDoneFadeOutAc();
              _onDoneFadeOutAc = null;
            }
          });
          break;
        case State.Done:
          if (disableColAfterDone)
            thisCollider.enabled = false;
          break;
        default:
          break;
      }
    }

    public bool IsState<T>(T t)
    {
      return this.state == (State)(object)t;
    }

    public void FadeInDirty()
    {
      ChangeState(State.Active);
    }
    public void FadeOutDirty()
    {
      ChangeState(State.Deactive);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
      if (!isCollideEffectable) return;
      if (collision.CompareTag("HaoNT") && IsState(State.None))
      {
        FadeInDirty();
      }
    }
    public void FadeInDirty(float delay)
    {
      Invoke(nameof(FadeInDirty), delay);
    }
    public void FadeOutDirty(float delay)
    {
      Invoke(nameof(FadeOutDirty), delay);
    }
    public void FadeInWithAc(Action onDoneFadeInAc = null)
    {
      _onDoneFadeInAc = onDoneFadeInAc;
      ChangeState(State.Active);
    }
    public void FadeOutWithAc(Action onDoneFadeOutAc = null)
    {
      _onDoneFadeOutAc = onDoneFadeOutAc;
      ChangeState(State.Deactive);
    }



    public void Editor()
    {
      if (waterRen == null)
        waterRen = GetComponent<SpriteRenderer>();
    }
  }
}