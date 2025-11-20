using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FxType
{
    Click = 0,
    Cow = 1,
    Pig = 2,
    Chicken = 3,
    Humm = 4,
    Engine = 5,
    Star = 6,
    CompleteBuild = 7,
    Happy = 8,
    Done = 9,
    OpenPopup = 10,
}

public class SoundManager : Singleton<SoundManager>
{
    public AudioClip[] audioClips;
    public AudioSource sound1;
    private AudioSource[] fx = new AudioSource[11];

    bool isMute = false;

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

    public IEnumerator IE_PlayFxAfterTime(FxType fxType, float time)
    {
        yield return Cache.GetWFS(time);
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