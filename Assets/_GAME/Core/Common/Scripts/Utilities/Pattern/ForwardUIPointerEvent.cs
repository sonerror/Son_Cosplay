using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utilities
{
    public class ForwardUIPointerEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public event System.Action<ForwardUIPointerEvent, PointerEventData> onMouseDown;
        public event System.Action<ForwardUIPointerEvent, PointerEventData> onMouseUp;
        public event System.Action<ForwardUIPointerEvent, PointerEventData> onMouseClick;

        public void OnPointerDown(PointerEventData eventData)
        {
            onMouseDown?.Invoke(this, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onMouseUp?.Invoke(this, eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onMouseClick?.Invoke(this, eventData);
        }
    }
}