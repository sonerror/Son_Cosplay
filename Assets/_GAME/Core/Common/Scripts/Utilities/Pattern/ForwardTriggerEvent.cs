using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class ForwardTriggerEvent : MonoBehaviour
    {
        public event System.Action<Collider2D> onTriggerEnter;
        public event System.Action<Collider2D> onTriggerStay;
        public event System.Action<Collider2D> onTriggerExit;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            onTriggerEnter?.Invoke(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            onTriggerStay?.Invoke(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            onTriggerExit?.Invoke(collision);
        }
    }
}