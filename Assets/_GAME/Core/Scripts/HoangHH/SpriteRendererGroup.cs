using System;
using UnityEngine;

namespace HoangHH
{
  public class SpriteRendererGroup : H3MonoBehaviour
  {
    [SerializeField] private Color color;
    [SerializeField] private bool affectColor = true;
    [SerializeField] private int order;

    [SerializeField] private SpriteRenderer[] spriteRenderers;

    public Color Color => color;
    public int NumberSpriteRenderers => spriteRenderers.Length;

    public void AddSpriteRenderers(SpriteRenderer spriteRenderer)
    {
      if (spriteRenderer is null) return;

      // Prevent duplicates
      if (Array.Exists(spriteRenderers, sr => sr == spriteRenderer)) return;

      // Resize and add
      Array.Resize(ref spriteRenderers, spriteRenderers.Length + 1);
      spriteRenderers[spriteRenderers.Length - 1] = spriteRenderer;

      // Apply current settings to new spriteRenderer
      if (affectColor)
        spriteRenderer.color = color;
      spriteRenderer.sortingOrder = order;
    }

    public void SetVisible(bool visible)
    {
      foreach (var spriteRenderer in spriteRenderers)
      {
        spriteRenderer.enabled = visible;
      }
    }

    public void SetColor(Color colorSet)
    {
      if (!affectColor) return;
      foreach (var spriteRenderer in spriteRenderers)
      {
        spriteRenderer.color = colorSet;
      }
    }

    public void SetAlpha(float alpha)
    {
      foreach (var spriteRenderer in spriteRenderers)
      {
        spriteRenderer.color = spriteRenderer.color.SetAlpha(alpha);
      }
    }

    public void SetOrder(int sortOrder)
    {
      order = sortOrder;
      foreach (var spriteRenderer in spriteRenderers)
      {
        spriteRenderer.sortingOrder = order;
      }
    }

  }
}