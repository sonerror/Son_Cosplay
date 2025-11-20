using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Text;
using System.IO;
using System.IO.Compression;
using UnityEngine.Networking;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;

#endif

public class MGUtils : MonoBehaviour
{
  private static MGUtils _instance;
  /// <summary>
  /// Supposed to be destroyed on load, to host coroutine separately each scene.
  /// </summary>
  public static MGUtils instance
  {
    get
    {
      if (_instance == null)
      {
        _instance = new GameObject().AddComponent<MGUtils>();
        instance.name = "[MGUtils]";
      }
      return _instance;
    }
  }


  /// <summary>
  /// Tints a color to another without changing saturation nor brightness.
  /// </summary>
  /// <param name="fromColor"></param>
  /// <param name="toBaseColor"></param>
  /// <returns></returns>
  public static Color TintColor(Color fromColor, Color toBaseColor)
  {
    //HSBColor hsbFrom = HSBColor.FromColor(fromColor);
    //HSBColor hsbTo = HSBColor.FromColor(toBaseColor);
    //hsbFrom.h = hsbTo.h;

    float a = fromColor.a;

    float h, s, v;
    Color.RGBToHSV(toBaseColor, out h, out s, out v);

    float fh, fs, fv;
    Color.RGBToHSV(toBaseColor, out fh, out fs, out fv);

    Color retC = Color.HSVToRGB(h, fs, fv);
    retC.a = a;

    return retC;
    //return hsbFrom.ToColor();
  }


}
