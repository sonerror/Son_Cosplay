using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Satisgame
{
  public class EmojiControl : MonoBehaviour
  {

    public GameObject nodePositive, nodeNegative;
    bool isHide = true;
    private IEnumerator coroutine = null;

    void Start()
    {
      HideEmoji();
    }

    public void ShowPositive()
    {
      nodePositive.SetActive(true);
      nodeNegative.SetActive(false);
      ResetTimeHide();
    }

    private void HideEmoji()
    {
      isHide = true;
      nodePositive.SetActive(false);
      nodeNegative.SetActive(false);
    }

    public void ShowNegative()
    {
      nodePositive.SetActive(false);
      nodeNegative.SetActive(true);
      ResetTimeHide();
    }

    void ResetTimeHide()
    {
      isHide = false;
      timeHide = 1.5f;
    }
    float timeHide = 1.5f;
    void Update()
    {
      if (!isHide)
      {
        timeHide -= Time.deltaTime;
        if (timeHide <= 0)
        {
          HideEmoji();
        }
      }
    }


  }
}
