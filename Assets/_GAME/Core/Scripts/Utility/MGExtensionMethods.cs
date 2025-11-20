
using System.Collections.Generic;
using UnityEngine;


public static class MGExtensionMethods
{
  /// <summary>
  /// Returns editor-like rotation Z (can be negative)
  /// </summary>
  /// <param name="t"></param>
  /// <returns></returns>
  public static float GetRotationZ(this Transform t)
  {
    float z = t.eulerAngles.z;
    if (z > 180) z -= 360;
    return z;
  }



  private static Color tmpColor;

  public static void SetAlpha(this SpriteRenderer sr, float alpha)
  {
    tmpColor = sr.color;
    tmpColor.a = alpha;
    sr.color = tmpColor;
  }



  public static bool Contains<T>(this T[] array, T obj)
  {
    bool result = false;
    foreach (T item in array)
    {
      if (item.Equals(obj))
      {
        result = true;
        break;
      }
    }
    return result;
  }


  public static void SetStartColor(this ParticleSystem ps, Color c, bool withChildren = false)
  {
    ParticleSystem.MainModule main;
    if (withChildren)
    {
      ParticleSystem[] pss = ps.transform.GetComponentsInChildren<ParticleSystem>();
      for (int i = 0; i < pss.Length; i++)
      {
        //pss[i].startColor = c;
        main = pss[i].main;
        main.startColor = c;
      }
    }
    else
    {
      //ps.startColor = c;
      main = ps.main;
      main.startColor = c;
    }
  }


  public static void TintColor(this ParticleSystem ps, Color c, bool withChildren = false)
  {
    if (withChildren)
    {
      ParticleSystem[] pss = ps.transform.GetComponentsInChildren<ParticleSystem>();
      for (int i = 0; i < pss.Length; i++)
      {
        pss[i].TintColor(c, false);
      }
    }
    else
    {
      ParticleSystem.MainModule main = ps.main;
      main.startColor = MGUtils.TintColor(main.startColor.color, c);
    }
  }

  public static void SetSortingOrder(this GameObject go, int sortingOrder)
  {
    if (go.GetComponent<Renderer>() != null) go.GetComponent<Renderer>().sortingOrder = sortingOrder;
    for (int i = 0; i < go.transform.childCount; i++)
    {
      go.transform.GetChild(i).gameObject.SetSortingOrder(sortingOrder);
    }
  }



  // Shuffle List
  public static void Shuffle<T>(this IList<T> list)
  {
    // Source: http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
    System.Random rng = new System.Random();
    int n = list.Count;
    while (n > 1)
    {
      n--;
      int k = rng.Next(n + 1);
      T value = list[k];
      list[k] = list[n];
      list[n] = value;
    }
  }
}
