using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SfxPool : PoolMember
{
  public AudioSource audioSource;

  public override void OnDespawn()
  {
    audioSource.Stop();
  }

  public override void OnSpawn()
  {
    if (!SoundManager.Ins.IsMute)
    {
      audioSource.Play();
    }
    DOVirtual.DelayedCall(audioSource.clip.length, DespawnThis);
  }

  void DespawnThis()
  {
    PoolManager.Ins.Despawn(this);
  }
}
