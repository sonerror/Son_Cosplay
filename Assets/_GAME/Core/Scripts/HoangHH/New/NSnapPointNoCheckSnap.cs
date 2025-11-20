namespace HoangHH
{
    public class NSnapPointNoCheckSnap : NSnapPoint
    {
        public override void OnSnap(NSnapObject snap)
        {
            isSnap = false;
        }
    }
}