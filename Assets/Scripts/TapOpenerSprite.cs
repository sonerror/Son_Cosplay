using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HoangHH
{
  public class TapOpenerSprite : Item
  {
    [SerializeField] private FxType soundOnOpen = FxType.None;
    [SerializeField] private bool isOpen;
    [SerializeField] private GameObject openObject;
    [SerializeField] private GameObject closeObject;



    public override void MouseUp(BaseEventData eventData)
    {
      if (isBlocked) return;
      base.MouseUp(eventData);
      if (!IsReady) return;
      SetOpen();
      IsReady = false;
    }

    private void SetOpen()
    {
      if (isOpen) return;
      isOpen = true;

      openObject.SetActive(true);
      closeObject.SetActive(false);

      OnFinish?.Invoke();
      SoundManager.Ins.PlayFx(soundOnOpen);

    }

  }
}