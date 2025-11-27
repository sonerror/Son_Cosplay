using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaterFace : MonoBehaviour
{
  [SerializeField] private bool isEnable = false;
  [SerializeField]
  float timeChange = 1f;
  // [SerializeField] private SpriteRenderer spriteShow;
  [SerializeField] private SpriteRenderer spriteHide;
  public FxType fxSound = FxType.Hair_Shave;


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
      SoundManager.Ins.StopSoundLoop(fxSound);
      return;
    }

    if (!hadPlayVfx && currentTime >= timeChange / 2f)
    {
      hadPlayVfx = true;
    }
    UpdateImage();
  }

  void UpdateImage()
  {
    float t = currentTime / timeChange;
    // spriteShow.color = new Color(1, 1, 1, t);
    spriteHide.color = new Color(1, 1, 1, 1 - t);
  }

  public void SetEnableFade()
  {
    isEnable = true;
    SoundManager.Ins.PlaySoundLoop(fxSound);
  }

  public void SetDisableFade()
  {
    isEnable = false;
    SoundManager.Ins.StopSoundLoop(fxSound);
  }
}
