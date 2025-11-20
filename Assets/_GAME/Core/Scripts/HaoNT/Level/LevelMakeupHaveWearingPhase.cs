using System;
using Costopia.Level;
using DG.Tweening;
using HoangHH;
using UnityEngine;

namespace Costapia.LevelWearing
{
  public class LevelMakeupHaveWearingPhase : StepByStepMakeUp
  {
    [SerializeField]
    private ShowObjectSequence showObjectSeqEffect;


    protected override void Start()
    {
      base.Start();
      showObjectSeqEffect.Show();
    }

    [SerializeField]
    private Transform characterContainer;

    [SerializeField]
    private GameObject wearingPhase;

    [SerializeField]
    private SpriteRendererGroup oldBg;

    [SerializeField]
    private float newCameraOrthoSize;

    [SerializeField]
    private float newCameraPositionY;
    [SerializeField]
    private float newBoduPositionY = 3f;

    private void OnTransitionToWearingPhase(System.Action callback = null)
    {
      BlockPlayerInteractModifiers.AddModifier(this);
      if (showObjectSeqEffect.gameObject.activeSelf)
      {
        showObjectSeqEffect.onComplete.AddListener(ShowWearingPhase);
        showObjectSeqEffect.Hide();
      }
      else
      {
        ShowWearingPhase();
      }

      void ShowWearingPhase()
      {
        showObjectSeqEffect.onComplete.RemoveListener(ShowWearingPhase);
        wearingPhase.gameObject.SetActive(true);
        Sequence s = DOTween.Sequence();
        s.Append(DOVirtual.Float(1f, 0f, 1f, x => oldBg.SetAlpha(x)).OnComplete(() => oldBg.gameObject.SetActive(false)));
        s.Append(Camera.DOOrthoSize(newCameraOrthoSize, 1f).OnStart(() =>
        {
        }));
        s.Join(Camera.transform.DOLocalMoveY(newCameraPositionY, 1f));
        s.OnComplete(() =>
        {
          BlockPlayerInteractModifiers.RemoveModifier(this);
          callback?.Invoke();
        });
      }
    }

    public override void OnTransitionToNewPhase(Action callback = null)
    {
      OnTransitionToWearingPhase(callback);
    }

  }
}