using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class AnimatorStateBehaviourReferences : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioClip[] sfxs;
        public ParticleSystem[] vfxs;

        public delegate void StateEventDelegate(StateMachineBehaviour stateMachineBehaviour, AnimatorStateInfo stateInfo);
        public event StateEventDelegate onEnterState;
        public event StateEventDelegate onExitState;

        public void TriggerEventEnterState(StateMachineBehaviour stateMachineBehaviour, AnimatorStateInfo stateInfo)
        {
            onEnterState?.Invoke(stateMachineBehaviour, stateInfo);
        }

        public void TriggerEventExitState(StateMachineBehaviour stateMachineBehaviour, AnimatorStateInfo stateInfo)
        {
            onExitState?.Invoke(stateMachineBehaviour, stateInfo);
        }
    }
}