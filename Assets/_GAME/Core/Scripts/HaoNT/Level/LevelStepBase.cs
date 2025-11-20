
using System;
using System.Collections;
using System.Collections.Generic;
using Costopia.Gameplay;
using Costopia.Level;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace Costopia.Base
{
  public abstract class LevelStepBase : MonoBehaviour
  {

    [SerializeField]
    private float blockTime = 1;

    [SerializeField]
    protected bool tryNextStepOnEnterStage, tryMoveCharacterToThisPhase;

    [SerializeField]
    protected UnityEvent onDoneStageEvent, onStartStageEvent;

    private List<Action> stepActions = new List<Action>();
    protected Action<bool> DoneStepAction;
    protected StepByStepMakeUp _level;
    protected CharacterControl _character => _level.Character;
    protected SkeletonDataAsset _skeletonDA => _level.SDA;

    protected int CurStep => _level.CurrentStep;

    public List<Action> GetStartStepActions()
    {
      InitStartStepActions();
      return stepActions;
    }

    protected virtual void InitStartStepActions()
    {
    }

    protected void AddStepAction(Action action)
    {
      if (action != null)
      {
        stepActions.Add(action);
      }
    }

    protected virtual void Awake()
    {
    }

    protected virtual void EnterThisStage()
    {
      if (tryMoveCharacterToThisPhase)
      {
        //Debug.LogError("Try enter new phase after done!");
        _level.MoveCharacterToDesk(_level.CurrentPhase);
      }

      onStartStageEvent?.Invoke();
    }

    protected virtual void ExitThisStage()
    {
    }

    public virtual void OnLose()
    {
    }

    public abstract bool IsDone();

    public virtual void OnStart()
    {
      if (_level == null)
      {
        _level = StepByStepMakeUp.LevelMakeup;
        //Debug.LogError("Try get Level: " + _level);
      }

      if (tryNextStepOnEnterStage)
      {
        //Debug.LogError("Try next step on enter new phase");
        _level.TryNextStep();
      }

      EnterThisStage();
    }

    public virtual void OnStart(Action<bool> assginAC)
    {
      DoneStepAction = assginAC;
      //Debug.LogError("On Start with list Action: " + stepActions.Count + " OnEndAc: " + DoneStepAction);
      OnStart();
    }

    public virtual void OnFinish(Action action = null)
    {
      action?.Invoke();
      ExitThisStage();
    }


  }
}