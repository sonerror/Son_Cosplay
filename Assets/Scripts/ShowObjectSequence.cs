using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace HoangHH
{
  public class ShowObjectSequence : MonoBehaviour
  {
    [Serializable]
    public class ShowObjectSequenceItem
    {
      public ShowObjectEffect effect;
      public float delayNext;
    }

    [SerializeField] private ShowObjectSequenceItem[] items;
    [SerializeField] private float firstDelay;
    [SerializeField] private bool showOnEnable = true;
    [ShowIf("showOnEnable")][SerializeField] private bool delayFirstFrame;
    [SerializeField] private bool hasSound;
    [ShowIf("hasSound")][SerializeField] private AudioClip soundOnShow;

    private bool _isShowing;

    public bool ShowOnEnable => showOnEnable;

    public UnityEvent onComplete;

    private void Start()
    {
      if (showOnEnable)
      {
        if (delayFirstFrame)
        {
          this.WaitOneFrame(Show);
        }
        else
        {
          Show();
        }
      }
    }

    public void Show()
    {
      for (int i = 0; i < items.Length; i++)
      {
        items[i].effect.gameObject.SetActive(false);
      }
      StartCoroutine(ShowSequence());
    }

    public void Hide()
    {
      StartCoroutine(HideSequence());
    }

    private IEnumerator ShowSequence()
    {
      if (_isShowing) yield break;
      _isShowing = true;
      yield return new WaitForSeconds(firstDelay);
      for (int index = 0; index < items.Length; index++)
      {
        var item = items[index];
        item.effect.Show();
        if (hasSound)
        {
          switch (index)
          {
            case 0:
            case > 0 when items[index - 1].delayNext > 0.0001f:
              // AudioManager.PlaySFX(soundOnShow);
              break;
          }
        }

        yield return new WaitForSeconds(item.delayNext);
      }
      onComplete?.Invoke();
      _isShowing = false;
    }

    private IEnumerator HideSequence()
    {
      if (_isShowing) yield break;
      _isShowing = true;
      yield return new WaitForSeconds(firstDelay);
      for (int index = 0; index < items.Length; index++)
      {
        var item = items[index];
        item.effect.Hide();
        yield return new WaitForSeconds(item.delayNext);
      }
      onComplete?.Invoke();
      _isShowing = false;
    }
  }
}
