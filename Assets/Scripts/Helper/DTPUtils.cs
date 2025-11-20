using UnityEngine;

public static class DTPUtils
{
  public static Color SetAlpha(this Color color, float alpha)
  {
    return new Color(color.r, color.g, color.b, alpha);
  }
}
