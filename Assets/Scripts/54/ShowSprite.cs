using DG.Tweening;
using UnityEngine;

public class ShowSprite : MonoBehaviour
{

  [SerializeField] float fadeDuration = 0.75f;
  [SerializeField] float TargetAlpha = 1f;
  [SerializeField] float delay = 0f;
  private SpriteRenderer _spriteRenderer;
  [SerializeField] private FxType soundFx = FxType.None;

  void Awake()
  {
    _spriteRenderer = GetComponent<SpriteRenderer>();
  }

  public void ShowSpriteStart()
  {
    gameObject.SetActive(true);
    if (!_spriteRenderer) _spriteRenderer = GetComponent<SpriteRenderer>();
    SoundManager.Ins.PlayFx(soundFx);
    _spriteRenderer.enabled = true;
    _spriteRenderer.DOFade(TargetAlpha, fadeDuration).SetDelay(delay);
  }

  public void ShowCustomSprite(float targetAlpha, float duration, float delayTime = 0f)
  {
    gameObject.SetActive(true);
    if (!_spriteRenderer) _spriteRenderer = GetComponent<SpriteRenderer>();
    _spriteRenderer.enabled = true;
    _spriteRenderer.DOFade(targetAlpha, duration).SetDelay(delayTime);
  }

  public void SetAlphaSprite(float alpha)
  {
    if (!_spriteRenderer) _spriteRenderer = GetComponent<SpriteRenderer>();
    Color color = _spriteRenderer.color;
    color.a = alpha;
    _spriteRenderer.color = color;
  }

}
