using System.Collections;
using UnityEngine;

public enum FxType
{
  Angry = 0,
  Happy = 1,
  StartGame = 2,
  Timer = 3,
  Pick = 4,
  Drop = 5,
  Hair_Shave = 6,
  CloseBox = 7,
  OpenLid = 8,
  AddGel = 9,
  Hair_Shave1 = 10,
  Happy1 = 11,
  Happy2 = 12,
  Angry1 = 13,
  Shaving = 14,
  CreamBrush = 15,
  Sfx_Shaver = 16,
  MagicSparkle = 17,
  ToPhan = 18,
  ToPhanDai = 19,

  None = 100,

}

public class SoundManager : Singleton<SoundManager>
{
  public AudioClip[] audioClips;
  public AudioSource sound1;
  private AudioSource[] fx = new AudioSource[30];

  bool isMute = false;
  public bool IsMute => isMute;

  public void PlayFx(FxType fxType)
  {
    if (fxType == FxType.None) return;
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

  public void PlayFxIfNotPlay(FxType fxType)
  {
    if (fxType == FxType.None) return;
    if (!isMute)
    {
      if (fx[(int)fxType] == null)
      {
        fx[(int)fxType] = new GameObject().AddComponent<AudioSource>();
        fx[(int)fxType].clip = audioClips[(int)fxType];
      }

      if (!fx[(int)fxType].isPlaying) fx[(int)fxType].Play();
    }
  }

  public void PlaySoundLoop(FxType fxType)
  {
    if (fxType == FxType.None) return;
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
    if (fxType == FxType.None) return;
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
    if (fxType == FxType.None) return;
    StartCoroutine(IE_PlayFxAfterTime(fxType, time));
  }

  public void Mute()
  {
    isMute = true;
    sound1.Stop();
    for (int i = 0; i < fx.Length; i++)
    {
      if (fx[i] != null)
      {
        fx[i].Stop();
      }
    }
  }
}