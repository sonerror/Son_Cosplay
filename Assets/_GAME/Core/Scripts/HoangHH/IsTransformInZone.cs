using UnityEngine;
using UnityEngine.Events;

namespace HoangHH.TrackWrongStep
{
    public class IsTransformInZone : H3MonoBehaviour
    {
        [SerializeField] private Rect rect;
        public UnityEvent onInZone;

        public void CheckInZone()
        {
            if (rect.Contains(Tf.position)) 
            {
                Debug.Log("InZone");
                onInZone?.Invoke();
            }
        }
    }
}