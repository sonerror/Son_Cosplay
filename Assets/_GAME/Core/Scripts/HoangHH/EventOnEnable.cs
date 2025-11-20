using UnityEngine;
using UnityEngine.Events;

namespace HoangHH
{
    public class EventOnEnable : MonoBehaviour
    {
        public UnityEvent triggerEvent;
        
        private void OnEnable()
        {
            triggerEvent?.Invoke();
        }
    }
}