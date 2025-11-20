using HoangHH;
using UnityEngine;

namespace VinhLB
{
    public class VFXForItem : MonoBehaviour
    {
        [SerializeField]
        private OnTransformMoveInAffectZone _zone;
        [SerializeField]
        private ParticleSystem _mainParticle;

        public void SetActiveZone(bool value)
        {
            _zone.gameObject.SetActive(value);
        }

        public void SetColor(Color color)
        {
            _mainParticle.SetStartColor(color);
        }

        public void Play()
        {
            _mainParticle.Play();
        }

        public void Stop()
        {
            _mainParticle.Stop();
        }
    }
}