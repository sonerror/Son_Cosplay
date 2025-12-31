
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace HoangHH
{
  public class OnTransformGoToAffectZone : GameUnit
  {
    [SerializeField] private bool useRadiusZone;
    [SerializeField] private Transform target;

    [SerializeField]
    private float radius;
    [SerializeField] private Rect affectZone;
    // [SerializeField] private bool checkUpdate = false;
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
      SetInZone(CheckInZone(Tf.position));
    }

    public bool CheckInZone(Vector2 pos2D)
    {
      if (useRadiusZone)
      {
        return Vector2.Distance(pos2D, target.position) <= radius;
      }
      return affectZone.Contains(pos2D);
    }

    public bool CheckInZoneThis()
    {
      if (useRadiusZone)
      {
        return Vector2.Distance(Tf.position, target.position) <= radius;
      }
      return affectZone.Contains(Tf.position);
    }

#if UNITY_EDITOR
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
#endif
    private void OnDisable()
    {
      if (_isInZone)
      {
        SetInZone(false);
      }
    }
  }
}