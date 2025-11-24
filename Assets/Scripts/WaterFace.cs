using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaterFace : MonoBehaviour
{
  [SerializeField] private bool isEnable = false;
  [SerializeField]
  float timeChange = 1f;
  [SerializeField] private SpriteRenderer spriteShow;
  [SerializeField] private SpriteRenderer spriteHide;

  public ParticleSystem vfxWater;

  public UnityEvent onComplete;

  private float currentTime = 0f;
  private bool hadPlayVfx = false;
  void Update()
  {
    if (!isEnable) return;
    currentTime += Time.deltaTime;
    if (currentTime >= timeChange)
    {
      isEnable = false;
      enabled = false;

      onComplete.Invoke();
      return;
    }

    if (!hadPlayVfx && currentTime >= timeChange / 2f)
    {
      hadPlayVfx = true;
      vfxWater.Play();
    }
    UpdateImage();
  }

  void UpdateImage()
  {
    float t = currentTime / timeChange;
    spriteShow.color = new Color(1, 1, 1, t);
    spriteHide.color = new Color(1, 1, 1, 1 - t);
  }

  public void SetEnableFade()
  {
    isEnable = true;
  }

  public void SetDisableFade()
  {
    isEnable = false;
  }
}
