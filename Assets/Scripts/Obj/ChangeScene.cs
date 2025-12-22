using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ChangeScene : MonoBehaviour
{
  [SerializeField] private float TimeChange = 1f;
  [SerializeField] private float TimeDelay = 1f;
  [SerializeField] private Vector3 SizeStart = Vector3.one;
  [SerializeField] private Transform NodeMask;


  public void ChangeSceneAndCall(UnityAction action)
  {
    gameObject.SetActive(true);
    StartCoroutine(ChangeSceneCoroutine(action));
  }

  private IEnumerator ChangeSceneCoroutine(UnityAction action)
  {
    NodeMask.localScale = SizeStart;
    NodeMask.gameObject.SetActive(true);
    NodeMask.DOScale(Vector3.zero, TimeChange);
    yield return DTPCache.GetWFS((TimeDelay / 2) + TimeChange);
    action?.Invoke();
    yield return DTPCache.GetWFS(TimeDelay / 2);
    NodeMask.DOScale(SizeStart, TimeChange);
    yield return DTPCache.GetWFS(TimeChange);
    gameObject.SetActive(false);
  }

}
