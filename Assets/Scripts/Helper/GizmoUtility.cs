using UnityEngine;

public static class GizmoUtility
{
  public static void DrawRect(Rect rect, Color color)
  {
    Color backupColor = Gizmos.color;
    Gizmos.color = color;
    Gizmos.DrawWireCube(rect.center, (Vector3)(rect.size) + Vector3.forward);
    Gizmos.color = backupColor;
  }
}
