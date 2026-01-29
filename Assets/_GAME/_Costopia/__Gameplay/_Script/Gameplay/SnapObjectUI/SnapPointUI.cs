using UnityEngine.Events;
using sonnv;
public abstract class SnapPointUI : SonMonoBehaviour
{
    public bool canSnap = true;
    public bool isSnap;
    private SnapObjectUI _snapObject;

    public SnapObjectUI SnapObject => _snapObject;
    public System.Action<SnapObjectUI> onSnap;
    public UnityEvent onSnapEvent;


    public virtual void OnSnap(SnapObjectUI snapObject)
    {
        _snapObject = snapObject;
        isSnap = true;
        onSnap?.Invoke(_snapObject);
        onSnapEvent?.Invoke();
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