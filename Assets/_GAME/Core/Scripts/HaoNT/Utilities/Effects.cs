using Costopia.Gameplay;
using DG.Tweening;
using UnityEngine;

namespace HaoNT
{
  public static class Effects
  {
    public static void SetEmissionRate(ParticleSystem particle, float rate)
    {
      ParticleSystem.EmissionModule emission = particle.emission;
      emission.rateOverTime = rate;
    }

    public static bool CheckEndStepDrawingMultipleTextures(H3SlotPaintableTexture[] slotPaints, float targetPainter)
    {
      bool end = true;
      for (int i = 0; i < slotPaints.Length; i++)
      {
        var slotPaint = slotPaints[i];
        if (slotPaint.RevealRatio < targetPainter)
        {
          end = false;
          break;
        }
      }

      return end;
    }

    public static void FillAllMultiplePaintTextures(H3SlotPaintableTexture[] slotPaints)
    {
      for (int i = 0; i < slotPaints.Length; i++)
      {
        slotPaints[i].FillAll();
      }
    }

    public static void ClearAllMultiplePaintTextures(H3SlotPaintableTexture[] slotPaints)
    {
      for (int i = 0; i < slotPaints.Length; i++)
      {
        slotPaints[i].ClearAll();
      }
    }

    public static void ChangeScaleSingleTime(Transform tf, float time = 0.1f, Ease ease = Ease.InSine)
    {
      tf.DOScale(tf.localScale * 1.2f, time).SetEase(ease).OnComplete(()
          => tf.DOScale(tf.localScale / 1.2f, time).SetEase(ease));
    }
  }
}