using UnityEngine;

public class ShakeObjectByValue : ModifyByValue
{
    [Header("Shake Settings")]
    [SerializeField] private float maxShakeStrength = 1f; // Max shake intensity
    [SerializeField] private float shakeSpeed = 20f; // Frequency of shake movement
    [SerializeField] private AnimationCurve shakeCurve;

    private Vector3 _originalPosition;
    private float _currentStrength;

    private void Start()
    {
        _originalPosition = Tf.localPosition;
    }

    private void Update()
    {
        if (_currentStrength > 0)
        {
            ApplyShake();
        }
        else
        {
            ResetPosition();
        }
    }

    protected override void OnModify(float value)
    {
        _currentStrength = shakeCurve.Evaluate(value) * maxShakeStrength;
    }

    private void ApplyShake()
    {
        float shakeX = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0) - 0.5f) * _currentStrength;
        float shakeY = (Mathf.PerlinNoise(0, Time.time * shakeSpeed) - 0.5f) * _currentStrength;
        Tf.localPosition = _originalPosition + new Vector3(shakeX, shakeY, 0);
    }

    private void ResetPosition()
    {
        Tf.localPosition = _originalPosition; // Reset when shaking stops
    }
}