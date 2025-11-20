using DG.Tweening;
using UnityEngine;

namespace Satisgame
{
  public class EmojiControl : MonoBehaviour
  {
    private static readonly int AnimPositive = Animator.StringToHash("Positive");
    private static readonly int AnimNegative = Animator.StringToHash("Negative");

    public Animator spriteAnimator;
    public Transform scaleTransform;

    [Header("Timing")]
    public float durationShow = 0.25f;
    public float durationHold = 2f;
    public float durationHide = 0.25f;

    private Sequence _sequenceShowEmoji;
    private Vector3 _originScale;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sfxPositive;
    public AudioClip sfxNegative;
    public AudioClip[] sfxPositiveList;
    public AudioClip[] sfxNegativeList;

    private void Start()
    {
      _originScale = scaleTransform.localScale;
      if (_originScale == Vector3.zero) _originScale = Vector3.one;
      scaleTransform.localScale = Vector3.zero;
    }

    public void HideEmoji()
    {
      if (_sequenceShowEmoji != null && _sequenceShowEmoji.IsActive())
      {
        _sequenceShowEmoji.Kill();
        _sequenceShowEmoji = DOTween.Sequence()
            .Append(scaleTransform.DOScale(Vector3.zero, durationHide / 2f).SetEase(Ease.OutQuad))
            .SetUpdate(true)
            .Play();
      }
    }

    public void ShowPositive(bool randomSfx = false, float delay = 0f) =>
        ShowEmoji(AnimPositive, randomSfx ? sfxPositiveList : null, sfxPositive, delay);

    public void ShowNegative(bool randomSfx = false, float delay = 0f) =>
        ShowEmoji(AnimNegative, randomSfx ? sfxNegativeList : null, sfxNegative, delay);

    private void ShowEmoji(int animHash, AudioClip[] randomList, AudioClip fallback, float delay)
    {
      if (_sequenceShowEmoji != null && _sequenceShowEmoji.IsActive())
        _sequenceShowEmoji.Complete();

      spriteAnimator.Play(animHash);

      _sequenceShowEmoji = DOTween.Sequence();
      if (delay > 0)
        _sequenceShowEmoji.AppendInterval(delay);

      _sequenceShowEmoji
          .Append(scaleTransform.DOScale(_originScale, durationShow).SetEase(Ease.OutBack))
          .AppendCallback(PlaySfx)
          .AppendInterval(durationHold)
          .Append(scaleTransform.DOScale(Vector3.zero, durationHide).SetEase(Ease.InBack))
          .Play();
      return;

      void PlaySfx()
      {
        if (!audioSource) return;
        AudioClip clip;
        if (randomList != null && randomList.Length > 0)
        {
          clip = randomList[Random.Range(0, randomList.Length)];
        }
        else
        {
          clip = fallback;
        }

        if (clip) audioSource.PlayOneShot(clip);
      }
    }
  }
}
