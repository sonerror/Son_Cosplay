using System.Collections;
using UnityEngine;

public enum FxType
{
  Angry,
  Happy,
  WearHairBang,
  Shower,
  Towel,
  Click,
}

public class SoundManager : Singleton<SoundManager>
{
  public AudioClip[] audioClips;
  // public AudioSource sound1;
  private AudioSource[] fx = new AudioSource[11];

  bool isMute = false;
  public bool IsMute => isMute;

  public void PlayFx(FxType fxType)
  {
    if (!isMute)
    {
      if (fx[(int)fxType] == null)
      {
        fx[(int)fxType] = new GameObject().AddComponent<AudioSource>();
        fx[(int)fxType].clip = audioClips[(int)fxType];
      }

      fx[(int)fxType].Play();
    }
  }

  public void PlaySoundLoop(FxType fxType)
  {
    if (!isMute)
    {
      if (fx[(int)fxType] == null)
      {
        fx[(int)fxType] = new GameObject().AddComponent<AudioSource>();
        fx[(int)fxType].clip = audioClips[(int)fxType];
        fx[(int)fxType].loop = true;
      }

      fx[(int)fxType].Play();
    }
  }

  public void StopSoundLoop(FxType fxType)
  {
    if (fx[(int)fxType] != null)
    {
      fx[(int)fxType].Stop();
    }
  }

  public IEnumerator IE_PlayFxAfterTime(FxType fxType, float time)
  {
    yield return DTPCache.GetWFS(time);
    if (!isMute)
    {
      if (fx[(int)fxType] == null)
      {
        fx[(int)fxType] = new GameObject().AddComponent<AudioSource>();
        fx[(int)fxType].clip = audioClips[(int)fxType];
      }

      fx[(int)fxType].Play();
    }
  }

  public void PlayFxAfterTime(FxType fxType, float time)
  {
    StartCoroutine(IE_PlayFxAfterTime(fxType, time));
  }

  public void Mute()
  {
    isMute = true;
    // sound1.Stop();
    for (int i = 0; i < fx.Length; i++)
    {
      if (fx[i] != null)
      {
        fx[i].Stop();
      }
    }
  }
}