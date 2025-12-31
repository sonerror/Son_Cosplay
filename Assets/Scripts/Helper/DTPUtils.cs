using System.Collections;
using UnityEngine;

public static class DTPUtils
{
  public static Color SetAlpha(this Color color, float alpha)
  {
    return new Color(color.r, color.g, color.b, alpha);
  }

  public static Coroutine WaitToDo(this MonoBehaviour monoBehaviour, System.Action action, float waitDuration)
  {
    if (waitDuration <= 0)
    {
      action?.Invoke();
      return null;
    }
    else
    {
      return monoBehaviour.StartCoroutine(DelayToDo(action, waitDuration));
    }
  }

  private static IEnumerator DelayToDo(System.Action action, float waitDuration)
  {
    yield return new WaitForSeconds(waitDuration);
    action?.Invoke();
  }
  public static Coroutine WaitOneFrame(this MonoBehaviour monoBehaviour, System.Action action)
  {
    return monoBehaviour.StartCoroutine(DelayOneFrame(action));
  }
  private static IEnumerator DelayOneFrame(System.Action action)
  {
    yield return new WaitForEndOfFrame();
    action?.Invoke();
  }

  public static void DrawRect(Rect rect, Color color)
  {
    Color backupColor = Gizmos.color;
    Gizmos.color = color;
    Gizmos.DrawWireCube(rect.center, (Vector3)(rect.size) + Vector3.forward);
    Gizmos.color = backupColor;
  }
}
