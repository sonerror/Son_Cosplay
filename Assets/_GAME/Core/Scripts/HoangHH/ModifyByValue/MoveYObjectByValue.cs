using UnityEngine;

public class MoveYObjectByValue : ModifyByValue
{
    [Header("Move Range")]
    [SerializeField] private Vector2 yMoveRange;
    [SerializeField] private AnimationCurve yMoveCurve;

    protected override void OnModify(float value)
    {
        float y = yMoveCurve.Evaluate(value) * (yMoveRange.y - yMoveRange.x) + yMoveRange.x;
        Tf.localPosition = new Vector3(Tf.localPosition.x, y, Tf.localPosition.z);
    }
}