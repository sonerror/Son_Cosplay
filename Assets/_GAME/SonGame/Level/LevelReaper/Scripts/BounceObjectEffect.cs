using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace sonnv
{
    public class BounceObjectEffect : SonMonoBehaviour
    {
        [SerializeField] private float bounceTime = 0.5f;

        [SerializeField]
        private AnimationCurve xCurve = new AnimationCurve(
            new Keyframe(0f, 1f, 0.23f, 0.23f),
            new Keyframe(0.433f, 1.05f, -0.78f, -0.78f),
            new Keyframe(0.6f, 0.9f, -0.65f, -0.65f),
            new Keyframe(1f, 1f, 0.5f, 0.5f)
        );

        [SerializeField]
        private AnimationCurve yCurve = new AnimationCurve(
            new Keyframe(0f, 1f, -0.46f, -0.46f),
            new Keyframe(0.433f, 0.9f, 0.37f, 0.37f),
            new Keyframe(0.6f, 1f, 0.6f, 0.6f),
            new Keyframe(1f, 1f, 0f, 0f)
        );

        public UnityEvent onComplete;

        private Sequence _bounceSeq;
        private bool _isBouncing;

        private Vector3 _originalScale;
        private Transform _cachedTransform;

        private void Awake()
        {
            _cachedTransform = transform;
        }

        private void OnDisable()
        {
            _bounceSeq?.Kill();
            _isBouncing = false;

            if (_cachedTransform != null && _originalScale != Vector3.zero)
            {
                _cachedTransform.localScale = _originalScale;
            }
        }

        [Button]
        public void Bounce()
        {
            if (_isBouncing) return;

            _bounceSeq?.Kill();
            _originalScale = _cachedTransform.localScale;
            _isBouncing = true;
            _bounceSeq = DOTween.Sequence();
            _bounceSeq.Append(DOVirtual.Float(0f, 1f, bounceTime, OnBounceUpdate));
            _bounceSeq.OnComplete(() =>
            {
                _cachedTransform.localScale = _originalScale;
                _isBouncing = false;
                onComplete?.Invoke();
            });
        }
        public void BounceDelay(float delay)
        {
            DOVirtual.DelayedCall(delay, Bounce);
        }

        private void OnBounceUpdate(float value)
        {
            float xMultiplier = xCurve.Evaluate(value);
            float yMultiplier = yCurve.Evaluate(value);

            _cachedTransform.localScale = new Vector3(
                _originalScale.x * xMultiplier,
                _originalScale.y * yMultiplier,
                _originalScale.z
            );
        }
    }
}