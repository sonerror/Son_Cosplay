using UnityEngine;

namespace HoangHH
{
    public class MoveXYObjectByValue : ModifyByValue
    {
        [Header("Move Range")] 
        [SerializeField] private Vector2 xMoveRange;
        [SerializeField] private Vector2 yMoveRange;
        [SerializeField] private AnimationCurve xMoveCurve;
        [SerializeField] private AnimationCurve yMoveCurve;
        
        protected override void OnModify(float value)
        {
            float x = xMoveCurve.Evaluate(value) * (xMoveRange.y - xMoveRange.x) + xMoveRange.x;
            float y = yMoveCurve.Evaluate(value) * (yMoveRange.y - yMoveRange.x) + yMoveRange.x;
            Tf.localPosition = new Vector3(x, y, Tf.localPosition.z);
        }
        
    }
}