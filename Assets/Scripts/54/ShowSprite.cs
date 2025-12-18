using DG.Tweening;
using UnityEngine;

public class ShowSprite : MonoBehaviour
{

  [SerializeField] float fadeDuration = 0.75f;
  [SerializeField] float TargetAlpha = 1f;
  private SpriteRenderer _spriteRenderer;
  [SerializeField] private FxType soundFx = FxType.None;

  void Awake()
  {
    _spriteRenderer = GetComponent<SpriteRenderer>();
  }

  public void ShowSpriteStart()
  {
    if (!_spriteRenderer) _spriteRenderer = GetComponent<SpriteRenderer>();
    _spriteRenderer.enabled = true;
    _spriteRenderer.DOFade(TargetAlpha, fadeDuration);
  }

  public void ChangeTypeShow()
  {
    TargetAlpha = 1f;
  }

  public void ChangeTypeHide()
  {
    TargetAlpha = 0f;
  }

}
