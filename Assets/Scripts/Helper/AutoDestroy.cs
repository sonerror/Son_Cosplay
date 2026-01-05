using System;
using System.Collections;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
  [Tooltip("Destroy after seconds, if greater than 0")]
  public float lifeTime = 1f;

  [Tooltip("Destroy if object goes below this abs Y position")]
  public float destroyY = 10;
  [Tooltip("Destroy if object goes beyond this abs X position")]
  public float destroyX = 7;

  public System.Action onDestroy;

  // Use this for initialization
  void Start()
  {
    // if (lifeTime > 0)
    // {
    //     StartCoroutine(DestroyAfterSeconds());
    // }
  }

  private void Update()
  {
    if ((destroyY > 0 && Mathf.Abs(transform.position.y) > destroyY) || (destroyX > 0 && Mathf.Abs(transform.position.x) > destroyX))
    {
      Destroy(gameObject);
    }
  }

  // IEnumerator DestroyAfterSeconds()
  // {
  //     yield return new WaitForSeconds(lifeTime);
  //     Destroy(gameObject);
  // }

  private void OnDestroy()
  {
    onDestroy?.Invoke();
  }
}