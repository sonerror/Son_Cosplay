using System;
using UnityEngine;

namespace VinhLB
{
    public class VisualFx : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem _particleSystem;
        [SerializeField]
        private AudioClip _audioClip;
        
        public ParticleSystem ParticleSystem => _particleSystem;
        public AudioClip AudioClip => _audioClip;

        public virtual void Play()
        {
            _particleSystem.Play();
        }
        
        public virtual void Play(Vector3 position)
        {
            transform.position = position;
            
            Play();
        }
        
        public virtual void Play(Action<AudioClip> onHandleAudioClip)
        {
            Play();
            
            onHandleAudioClip?.Invoke(_audioClip);
        }
        
        public virtual void Play(Vector3 position, Action<AudioClip> onHandleAudioClip)
        {
            Play(position);
            
            onHandleAudioClip?.Invoke(_audioClip);
        }
    }
}