using System;
using System.Collections.Generic;
using Costopia.Gameplay;
using DG.Tweening;
using HoangHH;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Costopia.Level
{
  public abstract class CosplayStepLevel : StepByStepLevel
  {
    [SerializeField]
    private PhaseBackground bg;

    [SerializeField]
    protected CharacterControl character;

    [SerializeField]
    private Desk desk;

    // [SerializeField]
    // private List<PhaseName> makeUpPhases;


    [SerializeField]
    protected float cameraMoveEndGameDelay;


    [SerializeField]
    protected Vector3 cameraMoveEndGame = new Vector3(0, 0, -10);


    [SerializeField]
    protected float cameraOrthoSizeEndGame = 20;


    [SerializeField]
    protected float cameraTimeMoveEndGame = 1f;


    [SerializeField]
    protected Ease cameraEaseEndGame = Ease.InOutSine;


    [SerializeField]
    private bool overrideZoomInCapture;


    [SerializeField]
    private float zoomInCapture = 10f;

    private int _currentMakeUpPhaseIndex;

    private CostopiaLevelSO _soLevel;

    public SkeletonDataAsset SkeletonDataAsset => character.SkeletonDataAsset;
    // public int TotalMakeUpPhases => makeUpPhases.Count;
    // public PhaseName CurrentMakeUpPhase => makeUpPhases[_currentMakeUpPhaseIndex];
    public CharacterControl Character => character;

    protected override void Awake()
    {
      base.Awake();
      OnPhaseChanged += ChangePhase;
    }

    protected override void Start()
    {
      base.Start();
      // H3Log.LogInfo($"Total Phase: {TotalPhase()}, Total Step: {TotalStep()}");
      if (LevelPlayingControlCostopia.IsExistInstance) SetSOLevel(LevelPlayingControlCostopia.Ins.LevelPlaying);
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      OnPhaseChanged -= ChangePhase;
    }

    // public event Action<int, PhaseName> OnMakeUpPhaseChanged;

    public void SetSOLevel(CostopiaLevelSO soLevel)
    {
      _soLevel = soLevel;
    }

    protected override void DoneStep(bool showEmoji = true)
    {
      base.DoneStep(showEmoji);
      character.AnimToIdle(CharacterAnimID.Happy);
      // Add Complete Task Feedback
    }

    public override void OnWrongCurrentStep()
    {
      base.OnWrongCurrentStep();
      character.AnimToIdle(CharacterAnimID.Angry);
    }

    protected IEnumerable<string> GetSlotNames()
    {
      return character.GetSlotNames();
    }

    protected override void OnEndGame()
    {
      StopTimeCounterModifiers.AddModifier(this);
      isEndingGame = true;
      onEndGame?.Invoke();

      DOVirtual.DelayedCall(delayTimeEndGame, EndGame);


      return;
    }

    protected virtual void OnCaptureShow()
    {
    }

    private void ChangePhase(int phaseId)
    {
      // OnMakeUpPhaseChanged?.Invoke(phaseId, makeUpPhases[phaseId]);
    }

    public void MoveCharacterToDesk(int deskSlotId, bool moveCharacter = true)
    {
      if (deskSlotId >= desk.SlotCount) return;
      BlockPlayerInteractModifiers.AddModifier(this);
      // delay a little bit to wait the emoji don
      DOVirtual.DelayedCall(0.6f, () =>
      {
        // anim character fake move
        // Desk move
        // BG Move
        if (moveCharacter)
        {
          character.MoveNext(() =>
                {
                  desk.SetDeskPositionAtIndex(deskSlotId);
                  bg.SetPhase(deskSlotId);
                }, () => { BlockPlayerInteractModifiers.RemoveModifier(this); });
        }
        else
        {
          desk.SetDeskPositionAtIndex(deskSlotId);
          bg.SetPhase(deskSlotId);
          BlockPlayerInteractModifiers.RemoveModifier(this);
        }
      }, false);
    }

    // Form for creating string with search slot/attachment
    /*
    [ValueDropdown(nameof(GetSlotNames), IsUniqueList = true, DropdownWidth = 300)]
    [SpineSlot(dataField: nameof(skeletonDataAsset))]
    public string slotName;
    [SpineAttachment(false, slotField: nameof(slotName), dataField: nameof(skeletonDataAsset))]
    public string attachmentName;
    */
  }

  [Serializable]
  public class CharacterSlotData
  {
    [SpineSlot(dataField: nameof(skeletonDataAsset))]
    public string slotName;

    [SpineAttachment(false, slotField: nameof(slotName), dataField: nameof(skeletonDataAsset))]
    public string attachmentName;

    [SerializeField] private SkeletonDataAsset skeletonDataAsset;

#if UNITY_EDITOR
    private void OnSlotNameChanged()
    {
      if (string.IsNullOrEmpty(slotName) || skeletonDataAsset == null)
        return;

      SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(true);
      if (skeletonData == null)
        return;

      SlotData slotData = skeletonData.FindSlot(slotName);
      if (slotData == null)
        return;

      int slotIndex = slotData.Index;
      Skin skin = skeletonData.DefaultSkin;
      if (skin == null)
        return;

      var entries = new List<Skin.SkinEntry>();
      skin.GetAttachments(slotIndex, entries);

      if (entries.Count > 0)
        attachmentName = entries[0].Name;
    }
#endif
  }
}