using UnityEngine;
using UnityEngine.Events;
using Utilities;
using sonnv;

public class OnTransformGoToAffectZone : SonMonoBehaviour
{
    [SerializeField] private bool useRadiusZone;
    [SerializeField] private Transform target;
    [SerializeField] private float radius;
    [SerializeField] private Rect affectZone;
    public UnityEvent onEnterZone;
    public UnityEvent onOutZone;

    private bool _isInZone;

    private void SetInZone(bool isIn)
    {
        if (_isInZone == isIn) return;
        if (isIn)
        {
            _isInZone = true;
            onEnterZone?.Invoke();
        }
        else
        {
            _isInZone = false;
            onOutZone?.Invoke();
        }
    }

    private void LateUpdate()
    {
        SetInZone(GetInZoneState(Tf.position));
    }

    private bool GetInZoneState(Vector2 pos2D)
    {
        if (useRadiusZone)
        {
            return Vector2.Distance(pos2D, target.position) <= radius;
        }
        return affectZone.Contains(pos2D);
    }

    private void OnDrawGizmosSelected()
    {
        if (useRadiusZone)
        {
            Gizmos.DrawWireSphere(target.position, radius);
        }
        else
        {
            GizmoUtility.DrawRect(affectZone, Color.cyan);
        }
    }

    private void OnDisable()
    {
        if (_isInZone)
        {
            SetInZone(false);
        }
    }
}
