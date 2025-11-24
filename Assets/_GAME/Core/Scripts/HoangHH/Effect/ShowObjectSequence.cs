using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] private bool delayFirstFrame;

    private bool _isShowing;

    public bool ShowOnEnable => showOnEnable;

    public UnityEvent onComplete;

    private void Start()
    {
      if (showOnEnable)
      {

        Show();

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

        yield return new WaitForSeconds(item.delayNext);
      }
      onComplete?.Invoke();
      SoundManager.Ins.PlayFx(FxType.WearHairBang);

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
