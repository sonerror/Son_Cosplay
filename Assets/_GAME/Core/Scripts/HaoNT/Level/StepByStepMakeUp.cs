using HoangHH;
using Costopia.Base;
using Costopia.Gameplay;
using Satisgame;
using Spine.Unity;
using UnityEngine;

namespace Costopia.Level
{
  public class StepByStepMakeUp : CosplayStepLevel
  {
    public static StepByStepMakeUp LevelMakeup => Instance as StepByStepMakeUp;
    private LevelStepBase LevelStep { get; set; }


    [SerializeField]
    private LevelStepBase[] levelSteps;

    private int _levelIndex;

    protected override void Start()
    {
      base.Start();
      _levelIndex = 0;
      LevelStep = levelSteps[_levelIndex];
      LevelStep.OnStart(TryNextStep);
    }

    public void SetNewEmojiControl(EmojiControl emoji)
    {
      SetNewEmoji(emoji);
    }


    [SerializeField]
    private ClockTimer clockTimeBusy;

    public CharacterControl Character => character;
    public SkeletonDataAsset SDA => SkeletonDataAsset;

    private void CheckStep()
    {
      if (LevelStep.IsDone())
      {
        LevelStep.OnFinish(NextPhase);
      }
    }

    public void CheckStepRemain(float time)
    {
      if (time > 0)
      {
        CancelInvoke(nameof(CheckStep));
        Invoke(nameof(CheckStep), time);
      }
      else
      {
        CheckStep();
      }
    }

    private void NextPhase()
    {
      Debug.LogError("Next Phase: " + _levelIndex);
      if (_levelIndex >= levelSteps.Length - 1)
      {
        OnEndingGame();
      }
      else
      {
        OnNewPhase();
      }
    }

    private void OnNewPhase()
    {
      LevelStep = levelSteps[++_levelIndex];
      //LevelStep = level.IsHaveRetry ? Instantiate(level) : level;
      LevelStep.OnStart(TryNextStep);
      Debug.LogError("NEXT STAGE: " + _levelIndex);
    }

    protected virtual void OnEndingGame()
    {
      Debug.LogError("End game ha!");
      OnEndGame();
    }

    public void SetTimeBusy(float time)
    {
      clockTimeBusy.Show(time);
    }

    protected override void InitStepActions()
    {
      //StartCoroutine(InitDelayTime(0.2f));
      InitStepActionInPhases();
    }

    // private IEnumerator InitDelayTime(float time)
    // {
    //     yield return new WaitForSeconds(time);
    //     InitStepActionAfterDelay();
    // }

    int stepIndex = 0;

    private void InitStepActionInPhases()
    {
      for (int i = 0; i < levelSteps.Length; i++)
      {
        var stepActions = levelSteps[i].GetStartStepActions();
        // H3Log.LogNote($"Count Step at Phase {i}: {stepActions.Count}");
        for (int j = 0; j < stepActions.Count; j++)
        {
          AddStepAction(i, stepIndex, stepActions[j]);
          stepIndex++;
        }
      }
    }

    protected virtual void TryNextStep(bool nextStepImidiately)
    {
      DoneStep();
      if (!nextStepImidiately) return;
      Debug.LogError("TryNextStep with index: " + currentStep);
      TryNextStep();
    }

    public void ShowEmoji(bool positive)
    {
      if (positive)
      {
        emoji.ShowPositive();
      }
      else
      {
        emoji.ShowNegative();
      }
    }

    public virtual void OnTransitionToNewPhase(System.Action callback = null)
    {
    }

    #region EndGame


    [SerializeField]
    private string poseAnimName, poseAnimLoop;


    [SerializeField]
    private AudioClip poseAnimClip;

    protected override void OnCaptureShow()
    {
      if (poseAnimClip) { } // AudioManager.PlaySFX(poseAnimClip);
      if (string.IsNullOrEmpty(poseAnimName))
      {
        if (string.IsNullOrEmpty(poseAnimLoop)) return;
        Spine.AnimationState state = character.SkeletonAnimation.AnimationState;
        state.SetAnimation(0, poseAnimLoop, true);
      }
      else
      {
        character.DisableIKControl(CharacterControlID.Eye);
        character.DisableIKControl(CharacterControlID.Head);
        character.DisableIKControl(CharacterControlID.Face);
        character.DisableIKControl(CharacterControlID.Body);
        Debug.LogError("Play anim end game: " + poseAnimName);
        Spine.AnimationState state = character.SkeletonAnimation.AnimationState;
        state.SetAnimation(0, poseAnimName, false);

        if (string.IsNullOrEmpty(poseAnimLoop)) return;
        Debug.LogError("Play anim idle: " + poseAnimLoop);
        state.AddAnimation(0, poseAnimLoop, true, 0);
      }
    }

    #endregion
  }
}