using System;
using System.Collections.Generic;
using DG.Tweening;

using Satisgame;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace HoangHH
{
  public abstract class StepByStepLevel : LevelBase
  {
    private readonly struct StepData
    {
      private readonly Action _stepAction;
      public int PhaseId { get; }

      public void PlayAction()
      {
        _stepAction?.Invoke();
      }

      public StepData(int phaseId, Action stepAction)
      {
        PhaseId = phaseId;
        _stepAction = stepAction;
      }
    }

    #region Base

    private Camera _cam;

    protected Camera Camera => _cam ? _cam : _cam = Camera.main;

    private readonly HashSet<int> _doneSteps = new HashSet<int>();

    [SerializeField]
    protected EmojiControl emoji;

    [SerializeField] protected UnityEvent onEndGame;
    [SerializeField] protected float delayTimeEndGame = 5f;

    protected void SetNewEmoji(EmojiControl newEmoji)
    {
      emoji = newEmoji;
    }

    protected List<Sprite> HintList;

    private void SetUpHintList()
    {
      HintList = new List<Sprite>();
      List<List<Sprite>> hintData = new List<List<Sprite>>(50);
      for (int i = 0; i < hintData.Count; i++)
      {
        HintList.AddRange(hintData[i]);
      }
    }

    #endregion

    #region Control Step

    public override int TotalStep()
    {
      return _stepActions.Count;
    }

    private readonly Dictionary<int, StepData> _stepActions = new Dictionary<int, StepData>();
    private readonly Dictionary<int, HashSet<int>> _phaseSteps = new Dictionary<int, HashSet<int>>();
    protected int StepActionCount => _stepActions.Count;

    protected override void Awake()
    {
      base.Awake();
      InitStepActions();
    }

    protected override void Start()
    {
      base.Start();
      SetUpHintList();
    }

    protected virtual void InitStepActions() { }

    protected void AddStepAction(int step, Action action)
    {
      _stepActions.Add(step, new StepData(currentPhase, action));
      AddPhaseStep(currentPhase, step);
    }

    protected void AddStepAction(int phase, int step, Action action)
    {
      _stepActions.Add(step, new StepData(phase, action));
      AddPhaseStep(phase, step);
    }

    private void AddPhaseStep(int phase, int step)
    {
      if (_phaseSteps.TryGetValue(phase, out var phaseStep))
      {
        phaseStep.Add(step);
      }
      else
      {
        _phaseSteps.Add(phase, new HashSet<int>());
        _phaseSteps[phase].Add(step);
      }
    }

    protected void RemoveStepAction(int step)
    {
      _stepActions.Remove(step);
    }

    public virtual void OnWrongCurrentStep()
    {
      emoji.ShowNegative();
      DecreaseMood();
      // LoseFullHeart();
    }

    public override int TotalPhase()
    {
      return _phaseSteps.Count;
    }

    public override int TotalStepAtPhase(int phase)
    {
      return _phaseSteps[phase].Count;
    }

    public bool IsWrongStep(int step)
    {
      if (currentStep != step)
      {
        OnWrongCurrentStep();
        return true;
      }

      return false;
    }

    private bool IsCurrentStepDone()
    {
      return _doneSteps.Contains(currentStep);
    }

    protected void DoneRecipeStep(int phaseId, int recipeStepId)
    {
      // SetDonePhaseAndStep(phaseId, recipeStepId);
    }

    public void DoneRecipeStep(string phaseAndStepIdString)
    {
      // Format: phaseId, recipeStepId
      // try convert and Set
      // Expected format: "1,2"
      if (string.IsNullOrWhiteSpace(phaseAndStepIdString))
      {
        return;
      }
      var parts = phaseAndStepIdString.Split(',');
      if (parts.Length != 2)
      {
        return;
      }
      if (int.TryParse(parts[0].Trim(), out int phaseId) &&
          int.TryParse(parts[1].Trim(), out int recipeStepId))
      {
        DoneRecipeStep(phaseId, recipeStepId);
      }
    }

    protected virtual void DoneStep(bool showEmoji = true)
    {
      if (_doneSteps.Add(currentStep))
      {
#if UNITY_EDITOR
        // H3Log.LogNote($"Phase {currentPhase}: Done Step {currentStep}");
#endif
        // MMVibrationManager.Haptic(HapticTypes.SoftImpact);
        if (showEmoji && emoji) emoji.ShowPositive();
        CheckDoneStep();
        IncreaseMood();
      }
    }

    public bool IsDoneStep(int step)
    {
      return _doneSteps.Contains(step);
    }

    // CAREFULLY WHEN USING THIS METHOD, IT WILL SKIP ALL BEFORE STEPS
    protected void ForceChangeCurStep(int step)
    {
      SetStep(step);
    }

    public void TryNextStep()
    {
      if (!IsCurrentStepDone()) return;
      OnNextStep();
    }

    protected virtual void OnNextStep()
    {
      int nextStep = currentStep + 1;
      int phase = GetPhaseAtStep(nextStep);
      if (phase != currentPhase) SetPhase(phase);
      SetStep(currentStep + 1);
      if (_stepActions.TryGetValue(nextStep, out var data))
      {
        data.PlayAction();
      }
    }

    private int GetPhaseAtStep(int step)
    {
      if (_stepActions.TryGetValue(step, out var data))
      {
        return data.PhaseId;
      }
      return -1;
    }

    protected virtual void OnEndGame()
    {
      isEndingGame = true;
      onEndGame?.Invoke();
      DOVirtual.DelayedCall(delayTimeEndGame, EndGame);

    }

    #endregion


    public void SkipToStep(int step)
    {
      currentStep = step - 1;
      OnNextStep();
    }

  }
}