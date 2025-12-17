using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogText : MonoBehaviour
{

  [SerializeField] private string fullText;
  [SerializeField] private float typingSpeed = 0.05f;
  [SerializeField] private TextMeshPro textMeshPro;
  private int currentIndex = 0;
  [SerializeField] private Transform nodeParent;
  public UnityEvent OnCompleteTyping;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    ZeroText();
    // nodeParent.localScale = Vector3.zero;
    // nodeParent.DOScale(Vector3.one, 0.5f)
    // .SetDelay(0.5f)
    // .SetEase(Ease.OutBack)
    // .OnComplete(() =>
    // {
      StartCoroutine(Typing());
    // });
  }

  IEnumerator Typing()
  {
    yield return DTPCache.GetWFS(0.5f);
    textMeshPro.gameObject.SetActive(true);
    
    foreach (char letter in fullText.ToCharArray())
    {
      textMeshPro.text += letter;
      yield return DTPCache.GetWFS(typingSpeed);
    }
    yield return DTPCache.GetWFS(1f);
    OnCompleteTyping?.Invoke();
  }

  public void ZeroText()
  {
    currentIndex = 0;
    textMeshPro.text = "";
  }
}
