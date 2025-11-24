using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace HoangHH
{
  public class BounceObjectEffect : H3MonoBehaviour
  {
    [SerializeField] private float bounceTime = 0.5f;

    [SerializeField]
    private AnimationCurve xCurve = new AnimationCurve(new Keyframe[]
    {
            new Keyframe(0f, 1f, 0.23f, 0.23f),
            new Keyframe(0.433f, 1.05f, -0.78f, -0.78f),
            new Keyframe(0.6f, 0.9f, -0.65f, -0.65f),
            new Keyframe(1f, 1f, 0.5f, 0.5f)
    });

    [SerializeField]
    private AnimationCurve yCurve = new AnimationCurve(new Keyframe[]
    {
            new Keyframe(0f, 1f, -0.46f, -0.46f),
            new Keyframe(0.433f, 0.9f, 0.37f, 0.37f),
            new Keyframe(0.6f, 1f, 0.6f, 0.6f),
            new Keyframe(1f, 1f, 0f, 0f)
    });

    private Sequence _bounceSeq;
    private bool _isBouncing;
    public UnityEvent onComplete;


    public void Bounce()
    {
      if (_isBouncing) return;

      _bounceSeq?.Kill();
      Vector3 originalScale = Tf.localScale;
      _isBouncing = true;

      _bounceSeq = DOTween.Sequence();

      _bounceSeq.Append(DOVirtual.Float(0f, 1f, bounceTime, (float value) =>
      {
        float xMultiplier = xCurve.Evaluate(value);
        float yMultiplier = yCurve.Evaluate(value);
        Tf.localScale = new Vector3(
                  originalScale.x * xMultiplier,
                  originalScale.y * yMultiplier,
                  originalScale.z
              );
      }));

      _bounceSeq.OnComplete(() =>
      {
        Tf.localScale = originalScale;
        _isBouncing = false;
        onComplete?.Invoke();
      });
    }

    private void OnDisable()
    {
      _bounceSeq?.Kill();
      _isBouncing = false;
    }
  }
}
