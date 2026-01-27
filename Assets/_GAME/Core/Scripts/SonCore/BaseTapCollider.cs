using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace sonnv
{
    public abstract class BaseTapCollider : SonMonoBehaviour
    {
        [Header("Tap Config")]
        [SerializeField] protected int targetTapCount = 1;
        [SerializeField] protected float scaleMultiplier = 1.1f;
        [SerializeField] protected float scaleUpDuration = 0.1f;
        [SerializeField] protected float scaleDownDuration = 0.25f;
        [SerializeField] protected Collider2D col;

        [Header("Audio")]
        [SerializeField] protected AudioClip tapSfx;

        [Header("Events")]
        public UnityEvent onCompleted;

        protected int currentTapCount;
        protected bool isCompleted;

        protected Vector3 originalScale;
        protected Tweener scaleTweener;

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            originalScale = transform.localScale;
            ResetTap();
        }

        protected virtual void OnDisable()
        {
            KillTween();
        }

        protected virtual void OnDestroy()
        {
            KillTween();
        }

        #endregion

        #region Public API

        public virtual void ResetTap()
        {
            currentTapCount = 0;
            isCompleted = false;
        }

        #endregion

        #region Input

        protected virtual void OnMouseDown()
        {
            if (isCompleted) return;
            HandleTap();
        }

        protected virtual void OnMouseUp()
        {
            PlayScaleDown();
        }

        #endregion

        #region Core Logic

        protected virtual void HandleTap()
        {
            currentTapCount++;

            PlaySfx();
            PlayScaleUp();

            if (currentTapCount >= targetTapCount)
            {
                Complete();
            }
        }

        protected virtual void Complete()
        {
            if (isCompleted) return;

            isCompleted = true;
            OnCompleted();
            if (col != null)
            {
                col.enabled = false;
            }
            onCompleted?.Invoke();
        }

        #endregion

        #region Animation

        protected virtual void PlayScaleUp()
        {
            KillTween();
            scaleTweener = transform
                .DOScale(originalScale * scaleMultiplier, scaleUpDuration)
                .SetEase(Ease.OutBack);
        }

        protected virtual void PlayScaleDown()
        {
            KillTween();
            scaleTweener = transform
                .DOScale(originalScale, scaleDownDuration)
                .SetEase(Ease.OutBack);
        }

        protected void KillTween()
        {
            if (scaleTweener != null && scaleTweener.IsActive())
            {
                scaleTweener.Kill();
            }
        }

        #endregion

        #region Audio

        protected virtual void PlaySfx()
        {
            if (tapSfx != null)
            {
                SoundManager.PlaySFX(tapSfx);
            }
        }

        #endregion

        #region Hooks for Child Classes
        protected abstract void OnCompleted();

        #endregion
    }
}
