using UnityEngine;

public class LoopScaleObjectByValue : ModifyByValue
{
  [Header("Scale Settings")]
  [SerializeField] private Vector2 scaleRange = new Vector2(0.8f, 1.2f); // Min & Max Scale
  [SerializeField] private float scaleSpeed = 2f; // Speed of the scale animation
  [SerializeField] private AnimationCurve scaleCurve;

  private Vector3 _originalScale;
  private float _currentStrength;

  private void Start()
  {
    _originalScale = Tf.localScale;
  }

  private void Update()
  {
    if (_currentStrength > 0)
    {
      ApplyLoopScale();
    }
    else
    {
      ResetScale();
    }
  }

  protected override void OnModify(float value)
  {
    _currentStrength = scaleCurve.Evaluate(value);
  }

  private void ApplyLoopScale()
  {
    float scaleMultiplier = Mathf.Lerp(scaleRange.x, scaleRange.y, (Mathf.Sin(Time.time * scaleSpeed) + 1) / 2);
    Tf.localScale = _originalScale * Mathf.Lerp(1f, scaleMultiplier, _currentStrength);
  }

  private void ResetScale()
  {
    Tf.localScale = _originalScale; // Reset when scaling stops
  }
}