using UnityEngine;

namespace HoangHH
{
    public class RotateZObjectByValue : ModifyByValue
    {
        [Header("Rotation Range")]
        [SerializeField] private Vector2 zRotationRange;
        [SerializeField] private AnimationCurve zRotationCurve;
    
        protected override void OnModify(float value)
        {
            float z = zRotationCurve.Evaluate(value) * (zRotationRange.y - zRotationRange.x) + zRotationRange.x;
            Tf.localRotation = Quaternion.Euler(Tf.localRotation.eulerAngles.x, Tf.localRotation.eulerAngles.y, z);
        }
    }
}