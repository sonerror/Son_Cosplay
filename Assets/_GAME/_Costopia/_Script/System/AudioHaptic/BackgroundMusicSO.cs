using System.Collections.Generic;
using UnityEngine;

namespace HoangHH
{
    [CreateAssetMenu(fileName = "BGM", menuName = "HoangHH/BGM")]
    public class BackgroundMusicSO : ScriptableObject
    {
        [SerializeField] private List<AudioClip> bgmHomes;
        [SerializeField] private List<AudioClip> bgmInGame;

        public AudioClip GetBGM(int index)
        {
            if (index >= 0 && index < bgmHomes.Count) return bgmHomes[index];
            return null;
        }
        
        public AudioClip GetRandomInGameBGM()
        {
            return bgmInGame[Random.Range(0, bgmInGame.Count)];
        }
    }
}