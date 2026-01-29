using UnityEngine;

namespace HoangHH.UI
{
    public class SafeArea : MonoBehaviour
    {
        private void Awake()
        {
            RectTransform rect = GetComponent<RectTransform>();
            Rect safeArea = UnityEngine.Screen.safeArea;
            Vector2 minAnchor = safeArea.position;
            Vector2 maxAnchor = minAnchor + safeArea.size;

            minAnchor.x /= UnityEngine.Screen.width;
            minAnchor.y /= UnityEngine.Screen.height;
            maxAnchor.x /= UnityEngine.Screen.width;
            maxAnchor.y /= UnityEngine.Screen.height;
            
            rect.anchorMin = minAnchor;
            rect.anchorMax = maxAnchor;
        }
    }
}