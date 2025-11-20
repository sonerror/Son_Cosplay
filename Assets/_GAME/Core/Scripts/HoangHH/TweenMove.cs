using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace HoangHH
{
    public class TweenMove : H3MonoBehaviour
    {
        [SerializeField] private Transform endPosition;
        [SerializeField] private float moveTime;
        [SerializeField] private Ease ease = Ease.Linear;
        public System.Action onStart;
        public UnityEvent onComplete;
        
        private Tween _moveTween;

        public void Move(bool setParentToEndPos)
        {
            _moveTween?.Kill();
            onStart?.Invoke();
            if (setParentToEndPos)
            {
                Tf.SetParent(endPosition);
                _moveTween = Tf.DOLocalMove(Vector3.zero, moveTime)
                    .SetEase(ease)
                    .OnComplete(() => onComplete?.Invoke());
            }
            else
            {
                _moveTween = Tf.DOMove(endPosition.position, moveTime)
                    .SetEase(ease)
                    .OnComplete(() => onComplete?.Invoke());
            }
        }
        
        public void Move(Vector3 endPos)
        {
            _moveTween?.Kill();
            _moveTween = Tf.DOMove(endPos, moveTime)
                .SetEase(ease)
                .OnComplete(() => onComplete?.Invoke());
        }
        
        public void LocalMove(Vector3 endPos)
        {
            _moveTween?.Kill();
            _moveTween = Tf.DOLocalMove(endPos, moveTime)
                .SetEase(ease)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void LocalMoveY(float yPos)
        {
            _moveTween?.Kill();
            _moveTween = Tf.DOLocalMoveY(yPos, moveTime)
                .SetEase(ease)
                .OnComplete(() => onComplete?.Invoke());
        }
           
        public void LocalMoveX(float xPos)
        {
            _moveTween?.Kill();
            _moveTween = Tf.DOLocalMoveX(xPos, moveTime)
                .SetEase(ease)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}