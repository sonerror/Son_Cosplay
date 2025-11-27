using System.Collections;
using DG.Tweening;
using UnityEngine;

public class HandCtrl : MonoBehaviour
{
  public Animator animator;
  private Vector3 pos1;
  private Vector3 pos2;


  private bool isShowHandState1 = false;
  public void ShowHandAtPos(Vector3 pos)
  {
    if (isShowHandState1) return;
    isShowHandState1 = true;
    gameObject.SetActive(true);
    animator.Play("Hand");
    transform.position = pos;
  }

  public void HideHand()
  {
    // Debug.Log("HideHand");
    gameObject.SetActive(false);
    StopAllCoroutines();
    transform.DOKill();
  }

  public void setHandPlayBox(Vector3 pos)
  {
    gameObject.SetActive(true);
    animator.Play("HandCir");
    transform.position = pos + Vector3.up * 0.5f;
  }

  public void ShowHandState2(Vector3 pos1, Vector3 pos2)
  {
    StopAllCoroutines();
    transform.DOKill();
    gameObject.SetActive(true);
    this.pos1 = pos1;
    this.pos2 = pos2;

    ShowHand();
  }

  void ShowHand()
  {
    transform.position = pos1;
    animator.SetTrigger("HandDown");
    transform.DOMove(pos2, 1f).SetDelay(0.75f)
    .OnComplete(() =>
    {
      animator.SetTrigger("HandUp");
    });
    StartCoroutine(IEShowHand());
  }

  IEnumerator IEShowHand()
  {
    yield return DTPCache.GetWFS(3f);
    if (gameObject.activeSelf)
      ShowHand();
  }
}
