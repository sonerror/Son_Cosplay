using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Satisgame
{
  public class EmojiControl : MonoBehaviour
  {

    public GameObject nodePositive, nodeNegative;

    private IEnumerator coroutine = null;

    void Start()
    {
      nodeNegative.SetActive(false);
      nodePositive.SetActive(false);
    }

    public void ShowPositive()
    {
      nodePositive.SetActive(true);
      nodeNegative.SetActive(false);
      if (coroutine != null)
      {
        StopCoroutine(coroutine);
      }
      coroutine = HideNegativeAfterDelay();
      StartCoroutine(coroutine);
    }

    private IEnumerator HideNegativeAfterDelay()
    {
      yield return new WaitForSeconds(2f);
      nodePositive.SetActive(false);
      nodeNegative.SetActive(false);
    }

    public void ShowNegative()
    {
      nodePositive.SetActive(false);
      nodeNegative.SetActive(true);

      if (coroutine != null)
      {
        StopCoroutine(coroutine);
      }
      coroutine = HideNegativeAfterDelay();
      StartCoroutine(coroutine);
    }

  }
}
