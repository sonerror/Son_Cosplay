using UnityEngine;

public class FloatingItem : MonoBehaviour
{
  [SerializeField]
  protected SimpleCurve curveFloatingIdle = new SimpleCurve(
      new SimpleCurve.Keyframe(0, 0),
      new SimpleCurve.Keyframe(0.5f, 0.3f),
      new SimpleCurve.Keyframe(1, 0)
  );

  [SerializeField] protected float durationFloatingIdle = 2.41f;
  [SerializeField] protected float curveTimeOffSet = 0.25f;
  [SerializeField] protected float speedFloatingIdle = 0.4f;
}
