
using Spine;
using System.Collections;
using UnityEngine;

namespace Utilities
{
  public class AnimatorBehaviourPlayFxOnEnterState : StateMachineBehaviour
  {
    public enum TriggerPosition
    {
      OnEnter,
      OnExit
    }

    private AnimatorStateBehaviourReferences _references;
    public TriggerPosition triggerPosition = TriggerPosition.OnEnter;
    public AudioClip sfxEnter = null;
    public float sfxVolume = 1f;
    public int sfxIndex = -1;
    // public HapticTypes hapticEnter = HapticTypes.None;
    public int vfxIndex = -1;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      base.OnStateEnter(animator, stateInfo, layerIndex);
      if (triggerPosition == TriggerPosition.OnEnter)
      {
        TriggerFx(animator);
      }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      base.OnStateExit(animator, stateInfo, layerIndex);
      if (triggerPosition == TriggerPosition.OnExit)
      {
        TriggerFx(animator);
      }
    }

    private void TriggerFx(Animator animator)
    {
      if (!_references)
      {
        _references = animator.GetComponent<AnimatorStateBehaviourReferences>();
      }
      if (!_references) return;

      if (sfxEnter)
      {
        // _references.audioSource.PlaySfx(sfxEnter, sfxVolume);
      }
      else if (sfxIndex >= 0 && sfxIndex < _references.sfxs.Length)
      {
        // _references.audioSource.PlaySfx(_references.sfxs[sfxIndex], sfxVolume);
      }

      // MMVibrationManager.Haptic(hapticEnter);

      if (vfxIndex >= 0 && vfxIndex < _references.vfxs.Length)
      {
        _references.vfxs[vfxIndex].Play();
      }
    }
  }
}