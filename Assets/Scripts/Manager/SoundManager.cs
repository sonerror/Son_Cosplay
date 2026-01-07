using System.Collections;
using UnityEngine;

public enum FxType
{
  Pick = 0,
  Drop = 1,
  StartGame = 2,
  Timer = 3,
  Happy0 = 4,
  Happy1 = 5,
  Happy2 = 6,
  Angry0 = 7,
  Angry1 = 8,
  Angry2 = 9,
  Cut = 10,
  AddGel = 11,
  XoaGel = 12,
  BrushAdd = 13,
  Brush0 = 14,
  Brush1 = 15,
  OpenLid = 16,
  ToSon = 17,
  XitGel = 18,
  Happy0_1 = 19,
  Happy1_1 = 20,
  Happy2_1 = 21,
  Angry0_1 = 22,
  Angry1_1 = 23,
  Angry2_1 = 24,
  WomanHey = 25,
  Water = 26,
  FaceTowel = 27,

  None = 100,
}

public class SoundManager : Singleton<SoundManager>
{
  public AudioClip[] audioClips;
  public AudioSource sound1;
  private AudioSource[] fx = new AudioSource[30];

  bool isMute = false;
  public bool IsMute => isMute;

  public void PlaySoundLoopWater()
  {
    PlaySoundLoop(FxType.Water);
  }

  public void StopSoundLoopWater()
  {
    StopSoundLoop(FxType.Water);
  }

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