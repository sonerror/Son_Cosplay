using UnityEngine.Events;

namespace HoangHH
{
    public class NSnapPointWithEvent : NSnapPoint
    {
        public UnityEvent onSnap;
        
        public override void OnSnap(NSnapObject snap)
        {
            base.OnSnap(snap);
            onSnap?.Invoke();
        }
    }
}