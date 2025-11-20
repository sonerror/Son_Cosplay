using DG.Tweening;
using UnityEngine;

namespace HoangHH
{
    public class FadeLoopSound : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] [Range(0f, 1f)] float targetVolume = 1;
        [SerializeField] private float fadeInTime = 0.3f;
        [SerializeField] private float fadeOutTime = 0.3f;
        private Tween _fadeTween;

        public void TurnOff(bool activeGob)
        {
            _fadeTween?.Kill();
            _fadeTween = DOTween.To(() => audioSource.volume, x => audioSource.volume = x, 0, fadeOutTime).OnComplete(() =>
            {
                audioSource.Stop();
                if (activeGob)
                {
                    gameObject.SetActive(false);
                }
            });
        }
        
        public void TurnOn()
        {
            gameObject.SetActive(true);
            audioSource.volume = 0;
            audioSource.Play();
            _fadeTween?.Kill();
            _fadeTween = DOTween.To(() => audioSource.volume, x => audioSource.volume = x, targetVolume, fadeInTime);
        }
    }
}