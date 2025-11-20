using System.Collections.Generic;
using UnityEngine;

public class DTPCache
{
  private static Dictionary<float, WaitForSeconds> m_WFS = new Dictionary<float, WaitForSeconds>();

  public static WaitForSeconds GetWFS(float key)
  {
    if (!m_WFS.ContainsKey(key))
    {
      m_WFS[key] = new WaitForSeconds(key);
    }

    return m_WFS[key];
  }



  //------------------------------------------------------------------------------------------------------------
}