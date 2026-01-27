using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
  public static T Ins;

  protected virtual void Awake()
  {
    Ins = GetComponent<T>();
  }

  public static bool instanceExists
  {
    get
    {
      return Ins != null;
    }
  }

  public static T Instance
  {
    get
    {
      InitInstance();

      return Ins;
    }
  }

  public static void InitInstance()
  {
    if (Ins != null) return;

    // if null, finds an existing one
    if (Ins == null) Ins = (T)FindObjectOfType(typeof(T));

    // attempt to laod from resources
    if (Ins == null)
    {
      GameObject go = Resources.Load<GameObject>(typeof(T).Name);
      if (go != null)
      {
        Ins = Instantiate(go).GetComponent<T>();
        Ins.name = "(singleton)" + typeof(T).ToString();
      }
    }

    // if still null, instantiates
    if (Ins == null)
    {
      Debug.Log(">> Instantiating Singleton: " + typeof(T).Name + "\n" + System.Environment.StackTrace);
      GameObject singleton = new GameObject();
      Ins = singleton.AddComponent<T>();
      singleton.name = "(singleton)" + typeof(T).ToString();
    }

    // if not null but inactive, finds an active one
    else if (!Ins.gameObject.activeInHierarchy)
    {
      Object[] allObjsOfType = FindObjectsOfType(typeof(T));
      if (allObjsOfType.Length > 1)
      {
        GameObject[] a = (GameObject[])allObjsOfType;
        for (int i = 0; i < a.Length; i++)
        {
          if (a[i].activeInHierarchy)
          {
            Ins = a[i].GetComponent<T>();
            break;
          }
        }
      }
    }
  }

}
