using System.Collections.Generic;
using System.Linq;
using Costopia.Base;
using Costopia.Gameplay;
using Costopia.Level;
using DG.Tweening;
using HaoNT;
using HoangHH;
using UnityEngine;

namespace Costapia.LvCosplayWenesday
{
  public class WenesdayRoom_1 : LevelStepBase
  {
    protected override void InitStartStepActions()
    {
      base.InitStartStepActions();
      AddStepAction(OnStartStep0);
      AddStepAction(OnStartStep1);
      AddStepAction(OnStartStep2);
      AddStepAction(OnStartStep3);
      AddStepAction(OnStartStep4);
    }

    private void Start()
    {
      OnStartStep0();
    }

    #region Step 0: Insert Hairbang

    private const string STEP_0 = "Step 0: Insert Hairbang";

    //     [FoldoutGroup(STEP_0)]
    [SerializeField]
    private NSnapObjectEnableSlot hairSnap;

    //     [FoldoutGroup(STEP_0)]
    [SerializeField]
    private NSnapPoint hairSnpoint;

    private void OnStartStep0()
    {

      hairSnpoint.ChangeCanSnap(true);
      hairSnap.AddSnapEvent(OnEndStep0);
    }

    private void OnEndStep0()
    {
      hairSnap.RemoveSnapEvent(OnEndStep0);
      hairSnap.gameObject.SetActive(false);
      DoneStepAction(true);
    }

    #endregion

    #region Step 1: Water face

    private const string STEP_1 = "Step 1: Water face";


    [SerializeField]
    private H3SlotFadeTexture waterSlotFadeTexture, oldSlotFadeTexture;


    [SerializeField]
    private DraggableMakeupItem showerDragObject;


    [SerializeField]
    private OnTransformGoToAffectZone showerAffectZone;


    [SerializeField]
    private ParticleSystem showerParticle;


    [SerializeField]
    private float fadeDuration = 3f;


    [SerializeField]
    private float emissionRateTarget = 5;

    private Tween _tweenEmissionRate;

    private void OnStartStep1()
    {
      showerDragObject.AddUseInStep(CurStep);
      showerDragObject.OnStartDragEvent.AddListener(EnableEffectZone);
      showerDragObject.OnStopDragEvent.AddListener(DisableEffectZone);
      showerAffectZone.onEnterZone.AddListener(TryShowerFace);
      showerAffectZone.onOutZone.AddListener(TryPauseShowerFace);
      oldSlotFadeTexture.gameObject.SetActive(true);
      waterSlotFadeTexture.gameObject.SetActive(true);
      waterSlotFadeTexture.SetFadeDuration(fadeDuration);
      waterSlotFadeTexture.onFadeComplete += OnEndStep1;
      showerParticle.gameObject.SetActive(true);
      Effects.SetEmissionRate(showerParticle, 0f);
    }

    private void EnableEffectZone()
    {
      showerAffectZone.enabled = true;
    }

    private void DisableEffectZone()
    {
      showerAffectZone.enabled = false;
    }

    private void TryShowerFace()
    {
      if (!waterSlotFadeTexture.IsFading)
      {
        waterSlotFadeTexture.FadeIn();
        showerParticle.Play();
        _tweenEmissionRate = DOVirtual.Float(0f, emissionRateTarget, fadeDuration,
                value => { Effects.SetEmissionRate(showerParticle, value); })
            .OnComplete(() => _tweenEmissionRate = null);
      }
      else if (waterSlotFadeTexture.IsPausing)
      {
        waterSlotFadeTexture.ResumeFade();
        _tweenEmissionRate.Play();
      }
    }

    private void TryPauseShowerFace()
    {
      if (waterSlotFadeTexture.IsFading && !waterSlotFadeTexture.IsPausing)
      {
        waterSlotFadeTexture.PauseFade();
        _tweenEmissionRate.Pause();
      }
    }

    private void OnEndStep1()
    {
      showerAffectZone.enabled = false;
      waterSlotFadeTexture.onFadeComplete -= OnEndStep1;
      showerAffectZone.onEnterZone.RemoveListener(TryShowerFace);
      showerAffectZone.onOutZone.RemoveListener(TryPauseShowerFace);
      showerDragObject.OnStartDragEvent.RemoveListener(EnableEffectZone);
      showerDragObject.OnStopDragEvent.RemoveListener(DisableEffectZone);
      oldSlotFadeTexture.FadeOut(0.5f);
      DoneStepAction(false);
    }

    #endregion

    #region Step 2: Towel face

    private const string STEP_2 = "Step 2: Towel face";

    //     [FoldoutGroup(STEP_2)]
    [SerializeField]
    private H3SlotPaintableTexture waterSlotPaintableTexture;

    //     [FoldoutGroup(STEP_2)]
    [SerializeField]
    private float targetEraseWater = 0.8f;

    //     [FoldoutGroup(STEP_2)]
    [SerializeField]
    private DraggableMakeupItem towelDragObject;

    //     [FoldoutGroup(STEP_2)]
    [SerializeField]
    private AudioSource towelAudioSrc;

    private Tween _tweenFadeOut;

    private void OnStartStep2()
    {
      towelDragObject.AddUseInStep(CurStep);
      towelDragObject.OnStartDragEvent.AddListener(TurnOnTowelTrigger);
      towelDragObject.OnStopDragEvent.AddListener(TurnOffTowelTrigger);

      waterSlotPaintableTexture.enabled = true;
      waterSlotPaintableTexture.OnRatioChange += CheckTowelPainterRatio;
    }

    private void CheckTowelPainterRatio(float ratio)
    {
      if (ratio <= targetEraseWater)
      {
        OnEndStep2();
      }
    }

    private void TurnOnTowelTrigger()
    {
      // towelPainter.enabled = true;
      towelAudioSrc.Play();
    }

    private void TurnOffTowelTrigger()
    {
      // towelPainter.enabled = false;
      towelAudioSrc.Stop();
    }

    private void OnEndStep2()
    {
      showerParticle.Stop();
      TurnOffTowelTrigger();
      waterSlotPaintableTexture.ClearAll();
      towelDragObject.OnStartDragEvent.RemoveListener(TurnOnTowelTrigger);
      towelDragObject.OnStopDragEvent.RemoveListener(TurnOffTowelTrigger);
      DoneStepAction(false);
    }

    #endregion

    #region Step 3: Apply oil

    private const string STEP_3 = "Step 3: Apply oil";

    //     [FoldoutGroup(STEP_3)]
    [SerializeField]
    private List<TriggerWithCertainCollider> oilTrigger;

    //     [FoldoutGroup(STEP_3)]
    [SerializeField]
    private List<SpriteRenderer> oilSprite;

    //     [FoldoutGroup(STEP_3)]
    [SerializeField]
    private DraggableMakeupItem oilDragObject;

    //     [FoldoutGroup(STEP_3)]
    [SerializeField]
    private GameObject oilTriggerPoint;

    //     [FoldoutGroup(STEP_3)]
    [SerializeField]
    private AudioClip squirt;

    private void OnStartStep3()
    {
      for (int index = 0; index < oilTrigger.Count; index++)
      {
        int i = index; // Capture the index for the lambda
        TriggerWithCertainCollider trigger = oilTrigger[i];
        trigger.gameObject.SetActive(true);
        trigger.OnTriggerEvent.AddListener(() => RemoveShampooTriggerPoint(trigger, oilSprite[i]));
      }

      oilDragObject.OnStartDragEvent.AddListener(TurnOnCleanserTriggerPoint);
      oilDragObject.OnStopDragEvent.AddListener(TurnOffCleanserTriggerPoint);
      oilDragObject.AddUseInStep(CurStep);
    }

    private void TurnOnCleanserTriggerPoint()
    {
      oilTriggerPoint.gameObject.SetActive(true);
    }

    private void TurnOffCleanserTriggerPoint()
    {
      oilTriggerPoint.gameObject.SetActive(false);
    }

    private void RemoveShampooTriggerPoint(TriggerWithCertainCollider trigger, SpriteRenderer sprite)
    {
      if (!oilTrigger.Remove(trigger)) return;
      // AudioManager.PlaySFX(squirt);
      sprite.enabled = true;
      if (oilTrigger.Count == 0) OnEndStep3();
    }

    private void OnEndStep3()
    {
      oilTriggerPoint.SetActive(false);
      oilDragObject.OnStartDragEvent.RemoveListener(TurnOnCleanserTriggerPoint);
      oilDragObject.OnStopDragEvent.RemoveListener(TurnOffCleanserTriggerPoint);
      DoneStepAction(false);
    }

    #endregion

    #region Step 4: Oil by hand

    private const string STEP_4 = "Step 4: Oil by hand";

    //     [FoldoutGroup(STEP_4)]
    [SerializeField]
    private ShowObjectEffect faucet;

    //     [FoldoutGroup(STEP_4)]
    [SerializeField]
    private List<TriggerWithCertainCollider> oilByHandTrigger;

    //     [FoldoutGroup(STEP_4)]
    [SerializeField]
    private DraggableByHand handDraw;

    //     [FoldoutGroup(STEP_4)]
    [SerializeField]
    private Collider2D handCollider;

    //     [FoldoutGroup(STEP_4)]
    [SerializeField]
    private OnTransformGoToAffectZone oilHandTrigger;

    //     [FoldoutGroup(STEP_4)]
    [SerializeField]
    private H3SlotPaintableTexture faceOilPaintTexture;

    //     [FoldoutGroup(STEP_4)]
    [SerializeField]
    private H3SlotFadeTexture oldFaceOilTexture;

    //     [FoldoutGroup(STEP_4)]
    [SerializeField]
    private float targetFaceOil = 0.8f;

    //     [FoldoutGroup(STEP_4)]
    [SerializeField]
    private AudioSource handSound;

    private void OnStartStep4()
    {
      handDraw.gameObject.SetActive(true);
      handDraw.OnStartDragEvent.AddListener(EnableHandPainter);
      handDraw.OnEndDragEvent.AddListener(DisableHandPainter);
      oilHandTrigger.onEnterZone.AddListener(EnableHandSound);
      oilHandTrigger.onOutZone.AddListener(DisableHandSound);

      faceOilPaintTexture.gameObject.SetActive(true);
      faceOilPaintTexture.OnRatioChange += CheckHandPainterRatio;

      for (int index = 0; index < oilByHandTrigger.Count; index++)
      {
        int i = index; // Capture the index for the lambda
        TriggerWithCertainCollider trigger = oilByHandTrigger[i];
        trigger.ReEnable(handCollider);
        trigger.OnTriggerEvent.AddListener(() => RemoverByHandTriggerPoint(trigger, oilSprite[i]));
      }
    }

    private void EnableHandSound()
    {
      handSound.Play();
    }

    private void DisableHandSound()
    {
      handSound.Stop();
    }

    private void EnableHandPainter()
    {
      oilHandTrigger.enabled = true;
      // oilPainter.enabled = true;
    }

    private void DisableHandPainter()
    {
      oilHandTrigger.enabled = false;
      // oilPainter.enabled = false;
    }

    private void RemoverByHandTriggerPoint(TriggerWithCertainCollider trigger, SpriteRenderer sprite)
    {
      if (!oilByHandTrigger.Remove(trigger)) return;
      DOVirtual.Float(1f, 0f, 1, sprite.SetAlpha)
          .OnComplete(() => { sprite.gameObject.SetActive(false); });
    }

    private void CheckHandPainterRatio(float ratio)
    {
      if (ratio >= targetFaceOil)
      {
        OnFadeOldTex();
      }
    }

    private void OnFadeOldTex()
    {
      foreach (var oil in oilSprite.Where(oil => oil.gameObject.activeSelf))
      {
        DOVirtual.Float(0.5f, 0f, 1, oil.SetAlpha)
            .OnComplete(() => { oil.gameObject.SetActive(false); });
      }

      oldFaceOilTexture.FadeOut(2, false, Ease.InBack);
      faceOilPaintTexture.FillAll(0.5f);
      handDraw.gameObject.SetActive(false);
      oldFaceOilTexture.onFadeComplete += OnEndStep4;
      Invoke(nameof(FadeOldTex), 1f);
    }

    private void FadeOldTex()
    {
      faceOilPaintTexture.ClearAll(1);
    }

    private void OnEndStep4()
    {
      faucet.Hide();
      oldFaceOilTexture.onFadeComplete -= OnEndStep4;
      DoneStepAction(false);
      onDoneStageEvent?.Invoke();
    }

    #endregion

    public override bool IsDone()
    {
      return true;
    }
  }
}