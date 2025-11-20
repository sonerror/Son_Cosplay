using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
  protected static T _ins;

  public static bool instanceExists
  {
    get
    {
      return _ins != null;
    }
  }

  public static T Ins
  {
    get
    {
      InitInstance();

      return _ins;
    }
  }

  public static void InitInstance()
  {
    if (_ins != null) return;

    // if null, finds an existing one
    if (_ins == null) _ins = (T)FindObjectOfType(typeof(T));

    // attempt to laod from resources
    if (_ins == null)
    {
      GameObject go = Resources.Load<GameObject>(typeof(T).Name);
      if (go != null)
      {
        _ins = Instantiate(go).GetComponent<T>();
        _ins.name = "(singleton)" + typeof(T).ToString();
      }
    }

    // if still null, instantiates
    if (_ins == null)
    {
      Debug.Log(">> Instantiating Singleton: " + typeof(T).Name + "\n" + System.Environment.StackTrace);
      GameObject singleton = new GameObject();
      _ins = singleton.AddComponent<T>();
      singleton.name = "(singleton)" + typeof(T).ToString();
    }

    // if not null but inactive, finds an active one
    else if (!_ins.gameObject.activeInHierarchy)
    {
      Object[] allObjsOfType = FindObjectsOfType(typeof(T));
      if (allObjsOfType.Length > 1)
      {
        GameObject[] a = (GameObject[])allObjsOfType;
        for (int i = 0; i < a.Length; i++)
        {
          if (a[i].activeInHierarchy)
          {
            _ins = a[i].GetComponent<T>();
            break;
          }
        }
      }
    }
  }
}

