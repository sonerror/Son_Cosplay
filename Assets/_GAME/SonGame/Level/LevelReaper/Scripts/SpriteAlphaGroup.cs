using System.Collections.Generic;
using DG.Tweening;
using sonnv;
using UnityEngine;

public class SpriteAlphaGroup : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    [SerializeField] private float fadeDuration = 1f;

    public void FadeIn() => Fade(1f);
    public void FadeOut() => Fade(0f);
    public void FadeIn(float duration) => Fade(1f, duration);
    public void FadeOut(float duration) => Fade(0f, duration);

    public void SetAlpha(float alpha)
    {
        DOTween.Kill(this);
        ApplyAlpha(Mathf.Clamp01(alpha));
    }

    public void Fade(float targetAlpha, float duration = -1f)
    {
        DOTween.Kill(this);

        float dur = duration > 0f ? duration : fadeDuration;
        float start = sprites.Count > 0 && sprites[0] != null ? sprites[0].color.a : 0f;
        float clamped = Mathf.Clamp01(targetAlpha);

        DOVirtual.Float(start, clamped, dur, ApplyAlpha)
            .SetEase(Ease.Linear)
            .SetTarget(this);
    }

    private void ApplyAlpha(float alpha)
    {
        foreach (var sr in sprites)
        {
            if (sr == null) continue;
            sr.color = sr.color.SetAlpha(alpha);
        }
    }

    private void OnDestroy() => DOTween.Kill(this);
}