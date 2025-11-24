using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class ForwardCollision2DEvent : MonoBehaviour
    {
        public event System.Action<Collision2D> onCollisionEnter;
        public event System.Action<Collision2D> onCollisionStay;
        public event System.Action<Collision2D> onCollisionExit;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            onCollisionEnter?.Invoke(collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            onCollisionStay?.Invoke(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            onCollisionExit?.Invoke(collision);
        }
    }
}