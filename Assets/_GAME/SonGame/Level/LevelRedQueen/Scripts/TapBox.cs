using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace sonnv
{
    public class TapBox : SonMonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Tap Config")]
        [SerializeField] private bool isTapDone = false;
        public bool IsTapDone => isTapDone;
        [SerializeField] int targetTapCount = 1;
        [SerializeField] private bool isChangeScale = false;
        [SerializeField] private float scaleMultiplier = 1.1f;
        [SerializeField] private float scaleUpDuration = 0.1f;
        [SerializeField] private float scaleDownDuration = 0.25f;
        [SerializeField] private Collider2D col;

        [Header("Audio")]
        [SerializeField] private AudioClip tapSfx;

        [Header("Events")]
        public UnityEvent onCompleted;
        public UnityEvent onTapDown;

        private int currentTapCount;
        private bool isCompleted;

        Vector3 originalScale;
        Tweener scaleTweener;

        #region Unity Lifecycle

        private void Awake()
        {
            originalScale = transform.localScale;
            ResetTap();
        }

        private void OnDisable()
        {
            KillTween();
        }

        private void OnDestroy()
        {
            KillTween();
        }

        #endregion

        #region Public API

        public void ResetTap()
        {
            currentTapCount = 0;
            isCompleted = false;
        }

        #endregion

        #region Input (Luna Web Ready)

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isCompleted) return;
            onTapDown?.Invoke();
            isTapDone = true;
            HandleTap();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PlayScaleDown();
        }

        #endregion

        #region Core Logic

        private void HandleTap()
        {
            currentTapCount++;

            PlaySfx();
            PlayScaleUp();

            if (currentTapCount >= targetTapCount)
            {
                Complete();
            }
        }

        private void Complete()
        {
            if (isCompleted) return;

            isCompleted = true;

            if (col != null)
            {
                col.enabled = false;
            }

            onCompleted?.Invoke();
        }

        #endregion

        #region Animation

        private void PlayScaleUp()
        {
            if (isChangeScale == false)
            {
                KillTween();
                scaleTweener = transform
                    .DOScale(originalScale * scaleMultiplier, scaleUpDuration)
                    .SetEase(Ease.OutBack);

            }
        }

        private void PlayScaleDown()
        {
            KillTween();
            scaleTweener = transform
                .DOScale(originalScale, scaleDownDuration)
                .SetEase(Ease.OutBack);

        }

        private void KillTween()
        {
            if (scaleTweener != null && scaleTweener.IsActive())
            {
                scaleTweener.Kill();
            }
        }

        #endregion

        #region Audio

        private void PlaySfx()
        {
            if (tapSfx != null)
            {
                SoundManager.PlaySFX(tapSfx);
            }
        }

        #endregion

        [SerializeField] private float scaleAffterSnap = 1f;

        public void ChaneScale()
        {
            Debug.Log("SetScale 1");
            Tf.localScale = Vector3.one * scaleAffterSnap;
        }
    }
}