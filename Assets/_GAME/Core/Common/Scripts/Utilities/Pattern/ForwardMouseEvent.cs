using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class ForwardMouseEvent : MonoBehaviour
    {
        public event System.Action<ForwardMouseEvent> onMouseDown;
        public event System.Action<ForwardMouseEvent> onMouseDrag;
        public event System.Action<ForwardMouseEvent> onMouseUp;

        private void OnMouseDown()
        {
            onMouseDown?.Invoke(this);
        }

        private void OnMouseDrag()
        {
            onMouseDrag?.Invoke(this);
        }

        private void OnMouseUp()
        {
            onMouseUp?.Invoke(this);
        }
    }
}