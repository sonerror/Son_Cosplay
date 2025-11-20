using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour
{
    public static T Ins;

    public virtual void Awake()
    {
        Ins = GetComponent<T>();
    }
}
