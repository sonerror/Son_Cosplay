using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace HoangHH
{
    public class PlayAnimationManually : MonoBehaviour
    {
        [SerializeField] private Animation anim;
        public UnityEvent onPlayComplete;

        private Tween _delayWaitAnimDone;

        public void PlayAnimation()
        {
            anim.Play();
            _delayWaitAnimDone?.Kill();
            _delayWaitAnimDone = DOVirtual.DelayedCall(anim.clip.length, () =>
            {
                onPlayComplete?.Invoke();
            });
        }
        public void StopAnim()
        {
            anim.Stop();
            _delayWaitAnimDone?.Kill();
        }
        public void PlayAnimation(float delay)
        {
            Invoke(nameof(PlayAnimation), delay);
        }
    }
}