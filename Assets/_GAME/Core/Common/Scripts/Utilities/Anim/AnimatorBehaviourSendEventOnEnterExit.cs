
using Spine;
using System.Collections;
using UnityEngine;

namespace Utilities
{
  public class AnimatorBehaviourSendEventOnEnterExit : StateMachineBehaviour
  {
    public enum TriggerPosition
    {
      OnEnter,
      OnExit
    }

    private AnimatorStateBehaviourReferences _references;
    public bool sendEventOnEnter = false;
    public bool sendEventOnExit = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      base.OnStateEnter(animator, stateInfo, layerIndex);

      if (!_references)
      {
        _references = animator.GetComponent<AnimatorStateBehaviourReferences>();
      }
      if (!_references) return;

      if (sendEventOnEnter)
      {
        _references.TriggerEventEnterState(this, stateInfo);
      }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      base.OnStateExit(animator, stateInfo, layerIndex);

      if (!_references)
      {
        _references = animator.GetComponent<AnimatorStateBehaviourReferences>();
      }
      if (!_references) return;

      if (sendEventOnExit)
      {
        _references.TriggerEventExitState(this, stateInfo);
      }
    }
  }
}