using UnityEngine;
using DG.Tweening;
public class ShakeObject : MonoBehaviour
{
    [SerializeField] private Transform tf;
    [SerializeField] private float duration;
    [SerializeField] private float strength;
    [SerializeField] private int vibrato;
    [SerializeField] private float randomness;
    [SerializeField] private bool fadeOut;
    [SerializeField] private bool fadeOutIn;
    [SerializeField] private AudioClip shakeSound;

    private Tween _shakeTween;
    private Vector3 _originalLocalPos;


    private void Start()
    {
        if (tf == null) return;
        _originalLocalPos = tf.localPosition;
    }

    public void ChangeOriginalCamPos(Vector3 newPos)
    {
        _originalLocalPos = newPos;
    }

    public void Shake()
    {
        _shakeTween?.Kill();
        _shakeTween = tf.DOShakePosition(duration, strength, vibrato, randomness, fadeOut, fadeOutIn)
            .OnComplete(() => tf.localPosition = _originalLocalPos);
        SoundManager.PlaySFX(shakeSound);
    }
}
