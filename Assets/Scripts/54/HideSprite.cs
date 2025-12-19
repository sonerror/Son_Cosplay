using DG.Tweening;
using UnityEngine;

public class HideSprite : MonoBehaviour
{

  [SerializeField] float fadeDuration = 0.75f;
  [SerializeField] float TargetAlpha = 0f;
  private SpriteRenderer _spriteRenderer;
  [SerializeField] private FxType soundFx = FxType.None;
  [SerializeField] private bool deactiveOnComplete = false;

  void Awake()
  {
    _spriteRenderer = GetComponent<SpriteRenderer>();
  }

  public void ShowSpriteStart()
  {
    if (!_spriteRenderer) _spriteRenderer = GetComponent<SpriteRenderer>();
    _spriteRenderer.enabled = true;
    SoundManager.Ins.PlayFx(soundFx);
    _spriteRenderer.DOFade(TargetAlpha, fadeDuration).OnComplete(() =>
    {
      if (deactiveOnComplete)
      {
        gameObject.SetActive(false);
      }
    });
  }

}
