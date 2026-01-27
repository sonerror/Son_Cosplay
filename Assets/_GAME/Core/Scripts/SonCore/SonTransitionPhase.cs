using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace sonnv
{
    public class SonTransitionPhase : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float scale = 20f;
        [SerializeField] private float zoomInDuration = 0.8f;
        [SerializeField] private float zoomOutDuration = 0.3f;

        [Header("Phase Objects")]
        [SerializeField] private List<GameObject> phaseObjects = new List<GameObject>();

        private Vector3 originalScale;
        private int count;
        public UnityEvent onComplete = new UnityEvent();

        private void Awake()
        {
            originalScale = spriteRenderer.transform.localScale;
            count = 0;
        }

        public void ChangeZoomOutDuration(float duration)
        {
            zoomOutDuration = duration;
        }
        public void TransitionToPhase(int fromIndex, int toIndex, Action onBeforeShrink = null)
        {
            count++;
            Debug.Log($"[SonTransitionPhase] Transition #{count}: {fromIndex} → {toIndex}");

            spriteRenderer.transform.DOKill();

            spriteRenderer.transform.localScale = Vector3.zero;
            spriteRenderer.gameObject.SetActive(true);

            spriteRenderer.transform
                .DOScale(scale, zoomInDuration)
                .SetEase(Ease.OutQuad)
                .SetTarget(spriteRenderer.transform)
                .OnComplete(() =>
                {
                    if (toIndex >= 0 && toIndex < phaseObjects.Count)
                    {
                        phaseObjects[toIndex].SetActive(true);
                    }

                    if (fromIndex >= 0 && fromIndex < phaseObjects.Count)
                    {
                        StartCoroutine(DisablePhaseEndOfFrame(phaseObjects[fromIndex]));
                    }

                    onBeforeShrink?.Invoke();

                    spriteRenderer.transform
                        .DOScale(originalScale, zoomOutDuration)
                        .SetEase(Ease.InQuad)
                        .SetTarget(spriteRenderer.transform)
                        .OnComplete(() =>
                        {
                            spriteRenderer.gameObject.SetActive(false);

                            onComplete?.Invoke();
                            onComplete.RemoveAllListeners();
                        });
                });
        }


        private IEnumerator DisablePhaseEndOfFrame(GameObject phase)
        {
            yield return new WaitForEndOfFrame();
            if (phase) phase.SetActive(false);
        }

        private void OnDisable()
        {
            spriteRenderer.transform.DOKill();
            onComplete.RemoveAllListeners();
        }
    }
}
