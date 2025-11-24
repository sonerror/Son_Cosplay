using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour
{
  public static T Ins;

  protected virtual void Awake()
  {
    Ins = GetComponent<T>();
  }
}
