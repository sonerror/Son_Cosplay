using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpriteAlphaGroup : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    [SerializeField] private float fadeDuration = 1f;

    private Tween _tween;

    // ─── Public ───────────────────────────────────────────────────────────────

    public void FadeIn() => Fade(1f);
    public void FadeOut() => Fade(0f);

    public void FadeIn(float duration) => Fade(1f, duration);
    public void FadeOut(float duration) => Fade(0f, duration);

    public void SetAlpha(float alpha)
    {
        _tween?.Kill();
        ApplyAlpha(Mathf.Clamp01(alpha));
    }

    public void Fade(float targetAlpha, float duration = -1f)
    {
        _tween?.Kill();

        float dur = duration > 0f ? duration : fadeDuration;
        float clamped = Mathf.Clamp01(targetAlpha);
        foreach (var sr in sprites)
        {
            if (sr == null) continue;
            float startAlpha = sr.color.a;
            DOVirtual.Float(startAlpha, clamped, dur, value =>
            {
                if (sr == null) return;
                Color c = sr.color;
                c.a = value;
                sr.color = c;
            }).SetEase(Ease.Linear);
        }
    }

    // ─── Private ──────────────────────────────────────────────────────────────

    private void ApplyAlpha(float alpha)
    {
        foreach (var sr in sprites)
        {
            if (sr == null) continue;
            Color c = sr.color;
            c.a = Mathf.Clamp01(alpha);
            sr.color = c;
        }
    }

    private void OnDestroy() => _tween?.Kill();
}