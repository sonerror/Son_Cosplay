namespace HoangHH
{
    public class NSnapPoint : H3MonoBehaviour
    {
        public bool canSnap = true;
        public bool isSnap;
        private NSnapObject _snapObject;
        
        public NSnapObject SnapObject => _snapObject;
        
        public virtual void OnSnap(NSnapObject snap)
        {
            _snapObject = snap;
            isSnap = true;
        }
        
        public virtual void ForceChangeSnap(bool snap)
        {
            isSnap = snap;
            if (!snap)
            {
                _snapObject = null;
            }
        }
        
        public void ChangeCanSnap(bool snap)
        {
            canSnap = snap;
        }
        
        public void SetNullSnapObject()
        {
            _snapObject = null;
        }
    }
}