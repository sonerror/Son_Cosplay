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
  Spray = 10,
  AddGel = 11,
  Pick1 = 12,
  AddEye = 13,
  OpenPack = 14,
  Tower1 = 15,
  Brush = 16,
  ToSon = 17,
  XitGel = 18,
  Comb = 19,

  None = 100,
}

public class SoundManager : Singleton<SoundManager>
{
  public AudioSource SfxSource { get; private set; }
  public AudioClip[] audioClips;
  public AudioSource bgm;
  private AudioSource[] fx = new AudioSource[30];
  bool isMute = false;
  public bool IsMute => isMute;
  protected override void Awake()
  {
    base.Awake();

    if (SfxSource == null)
    {
      SfxSource = gameObject.AddComponent<AudioSource>();
      SfxSource.playOnAwake = false;
    }
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
  public static AudioSource PlaySfx(AudioClip clip, float volume = 1f, bool isLoop = false)
  {
    Instance.SfxSource.loop = isLoop;
    Instance.SfxSource.volume = volume;
    Instance.SfxSource.clip = clip;
    Instance.SfxSource.Play();
    return Instance.SfxSource;
  }
  public static void PlaySFX(params AudioClip[] clips)
  {
    if (Instance == null) return;
    if (Instance.SfxSource == null) return;
    if (clips == null || clips.Length == 0) return;

    AudioClip c = clips[UnityEngine.Random.Range(0, clips.Length)];
    if (c == null) return;

    Instance.SfxSource.clip = c;
    Instance.SfxSource.Play();
  }


  public static void PlaySFX(AudioClip clip, float volume = 1f)
  {
    if (Instance == null) return;
    if (Instance.SfxSource == null) return;
    if (clip == null) return;

    Instance.SfxSource.volume = volume;
    Instance.SfxSource.clip = clip;
    Instance.SfxSource.Play();
  }

  public void PlaySoundSpray()
  {
    PlaySoundLoop(FxType.Spray);
  }
  public void StopSoundSpray()
  {
    StopSoundLoop(FxType.Spray);
  }
  public void PlaySoundComb()
  {
    PlaySoundLoop(FxType.AddGel);
  }
  public void StopSoundComb()
  {
    StopSoundLoop(FxType.AddGel);
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
    bgm.Stop();
    for (int i = 0; i < fx.Length; i++)
    {
      if (fx[i] != null)
      {
        fx[i].Stop();
      }
    }
  }

  public void PlayBgm()
  {
    if (isMute) return;
    bgm.Play();
  }
}