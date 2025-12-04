using DG.Tweening;
using UnityEngine;

public class ShowSprite : MonoBehaviour
{

  [SerializeField] float fadeDuration = 0.75f;
  private SpriteRenderer _spriteRenderer;

  void Awake()
  {
    _spriteRenderer = GetComponent<SpriteRenderer>();
  }

  public void ShowSpriteStart()
  {
    if (!_spriteRenderer) _spriteRenderer = GetComponent<SpriteRenderer>();
    _spriteRenderer.enabled = true;
    _spriteRenderer.DOFade(1f, fadeDuration);
  }
}
