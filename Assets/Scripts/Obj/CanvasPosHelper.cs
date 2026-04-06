using UnityEngine;
using UnityEngine.UI;

public static class CanvasPosHelper
{
    /// <summary>
    /// Lấy anchoredPosition của một RectTransform bất kỳ
    /// quy về local position trên canvas đang chứa target
    /// </summary>
    public static Vector2 GetCanvasPos(RectTransform target)
    {
        Canvas canvas = target.GetComponentInParent<Canvas>();
        if (canvas == null) return Vector2.zero;

        RectTransform canvasRT = canvas.rootCanvas.GetComponent<RectTransform>();

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
            canvas.worldCamera, target.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRT, screenPos, canvas.worldCamera, out Vector2 canvasPos);

        return canvasPos;
    }

    /// <summary>
    /// Lấy vị trí canvas từ world position (dùng cho object thường)
    /// </summary>
    public static Vector2 WorldToCanvasPos(Vector3 worldPos, Canvas canvas, Camera cam)
    {
        if (canvas == null) return Vector2.zero;

        RectTransform canvasRT = canvas.rootCanvas.GetComponent<RectTransform>();
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRT, screenPos, canvas.worldCamera, out Vector2 canvasPos);

        return canvasPos;
    }
}