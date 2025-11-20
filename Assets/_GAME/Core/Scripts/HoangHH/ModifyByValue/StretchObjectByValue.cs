using UnityEngine;

public class StretchObjectByValue : ModifyByValue
{
    [Header("Stretch Range")] 
    [SerializeField] private Vector2 xStretchRange;
    [SerializeField] private AnimationCurve xStretchCurve;
    [SerializeField] private Vector2 yStretchRange;
    [SerializeField] private AnimationCurve yStretchCurve;
    
    protected override void OnModify(float value)
    {
        float x = xStretchCurve.Evaluate(value) * (xStretchRange.y - xStretchRange.x) + xStretchRange.x;
        float y = yStretchCurve.Evaluate(value) * (yStretchRange.y - yStretchRange.x) + yStretchRange.x;
        Tf.localScale = new Vector3(x, y, 1);
    }
}