using UnityEngine;

public class MoveXObjectByValue : ModifyByValue
{
    [Header("Move Range")] [SerializeField]
    private Vector2 xMoveRange;

    [SerializeField] private AnimationCurve xMoveCurve;
    
    protected override void OnModify(float value)
    {
        float x = xMoveCurve.Evaluate(value) * (xMoveRange.y - xMoveRange.x) + xMoveRange.x;
        Tf.localPosition = new Vector3(x, Tf.localPosition.y, Tf.localPosition.z);
    }
}