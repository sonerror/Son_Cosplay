using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using sonnv;
using System.Collections;
using HoangHH;
using Satisgame;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Utilities;
public class LevelControl : Singleton<LevelControl>
{
    #region Setting
    [SerializeField] private EmojiControl emojiControl;
    public EmojiControl EmojiControl => emojiControl;
    [SerializeField] private CharacterControl character;
    [SerializeField] private ClockTimer clockTimer;

    public CharacterControl Character => character;
    public void SetNewCharacter(CharacterControl _char)
    {
        character = _char;
    }
    public FillCircleBar fillCircleBar;
    [SerializeField] private bool isChangeAnimHappy = true;
    public bool IsChangeAnimHappy => isChangeAnimHappy;
    private IEnumerator coroutine = null;
    public void SetNewEmoji(EmojiControl newEmoji)
    {
        emojiControl = null;
        emojiControl = newEmoji;
    }
    public void SetEmissionRate(ParticleSystem particle, float rate)
    {
        ParticleSystem.EmissionModule emission = particle.emission;
        emission.rateOverTime = rate;
    }
    public void SetStateChangeAnim(bool value)
    {
        isChangeAnimHappy = value;
    }
    protected void TurnCharacterSlotAttachment(
        List<SlotAttachmentPair> slotDataList,
        bool attached)
    {
        for (int i = 0; i < slotDataList.Count; i++)
        {
            if (character != null)
            {
                character.TurnSlotAttachment(
                    slotDataList[i].slotName,
                    attached ? slotDataList[i].attachmentName : null
                );
            }
        }
    }
    protected virtual void Start()
    {
        SetSlotStart();
        StartStep();
    }
    [SerializeField] private SlotAttachmentPairList slotOnStart;
    [SerializeField] private SlotAttachmentPairList slotOffStart;
    private void SetSlotStart()
    {
        slotOnStart.TurnSlotState(true);
        slotOffStart.TurnSlotState(false);
    }
    private bool isDoneStep;
    public bool IsDoneStep => isDoneStep;
    [SerializeField] private bool isPlayingGame = false;
    public bool IsPlayingGame => isPlayingGame;
    private bool hadClicked = false;
    public void SetStateDoneStep(bool _value)
    {
        isDoneStep = _value;
    }
    protected virtual void DoneStep()
    {
        PlayPositiveEmoji();
        isDoneStep = true;
    }
    private void Update()
    {
        if (isPlayingGame && Input.GetMouseButtonDown(0))
        {
            EventManager.TriggerEvent("ShowBtnInstall");
        }
        if (!hadClicked && Input.GetMouseButtonDown(0))
        {
            hadClicked = true;
            StartGamePlay();
        }
    }
    [SerializeField] private SonTransitionPhase transitionPhase;
    [SerializeField] private Camera cam;
    private void MoveCamera(
               Camera _cam,
               float targetOrthoSize,
               float targetLocalY,
               float duration,
               System.Action onComplete = null)
    {
        if (_cam == null) return;
        Sequence seq = DOTween.Sequence();
        seq.Append(_cam.DOOrthoSize(targetOrthoSize, duration));
        seq.Join(_cam.transform.DOLocalMoveY(targetLocalY, duration));
        if (onComplete != null)
        {
            seq.OnComplete(() => onComplete?.Invoke());
        }
    }

    [SerializeField] private float targetOrthoSize = 19;
    [SerializeField] private float targetLocalY = 1f;
    [SerializeField] private float targetOrthoSizeStart = 4f;

    private void StartGamePlay()
    {
        GamePlayScreen uiScreen = UIManager.Ins.GetUI(0);
        uiScreen.logoUI.SetActive(false);
        EventManager.TriggerEvent("ShowIconLv");
        SoundManager.Ins.PlayBgm();
        transitionPhase.TransitionToPhase(0, 1, () =>
        {
            cam.orthographicSize += targetOrthoSizeStart;
        });
        transitionPhase.onComplete.AddListener(() =>
        {
            OnStartStep1();
        });
        DOVirtual.DelayedCall(0.2f, () =>
        {
            isPlayingGame = true;
            EventManager.TriggerEvent("ShowBtnInstall");
        });
    }
    public void TryNextStep()
    {
        StepManager.Ins.CurrentStep++;
        StartStep();

    }
    #endregion
    public virtual void StartStep()
    {
        isDoneStep = false;
        switch (StepManager.Ins.CurrentStep)
        {
            case 0:
                return;
            case 1:
                TutorialManager.Ins.SetNewTime(2f);
                OnStartStep2();
                return;
            case 2:
                TutorialManager.Ins.SetNewTime(3f);

                OnStartStep3();
                return;
            case 3:
                OnStartStep4();
                return;
            case 4:
                OnStartStep5();
                return;
            case 5:
                OnStartStep6();
                return;
            case 6:
                OnStartStep7();
                return;
            case 7:
                OnStartStep8();
                return;
            case 8:
                OnStartStep9();
                return;
            case 9:
                OnStartStep10();
                return;
            default:
                return;
        }
    }
    #region  Base

    public void PlayPositiveEmoji()//9.25
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = PlayPositiveEmojiCoroutine();
        StartCoroutine(coroutine);
    }

    private int idSoundHappy = 0;
    private IEnumerator PlayPositiveEmojiCoroutine()
    {

        emojiControl.ShowPositive();
        if (character != null)
        {
            if (isChangeAnimHappy)
            {
                character.SetMouseHappy();
            }
            else
            {
                character.SetMouseIdle();
            }
        }
        var i = idSoundHappy % 3;
        if (i == 0) SoundManager.Ins.PlayFx(FxType.Happy0);
        else if (i == 1) SoundManager.Ins.PlayFx(FxType.Happy1);
        else SoundManager.Ins.PlayFx(FxType.Happy2);
        idSoundHappy++;
        yield return new WaitForSeconds(1f);
        if (character != null)
        {
            character.SetMouseIdle();
        }
        coroutine = null;
    }
    public void PlayAnimHappy()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = IE_PlayAnimHappy();
        StartCoroutine(coroutine);
    }
    private IEnumerator IE_PlayAnimHappy()
    {
        if (character != null)
        {
            if (isChangeAnimHappy)
            {
                character.SetMouseHappy();
            }
            else
            {
                character.SetMouseIdle();
            }
        }
        yield return new WaitForSeconds(0.5f);
        if (character != null)
        {
            character.SetMouseIdle();
        }
        coroutine = null;
    }
    public void PlayPositiveEmojiOnSnap()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = PlayHappyOnSnap();
        StartCoroutine(coroutine);
    }
    private IEnumerator PlayHappyOnSnap()
    {
        if (character != null)
        {
            character.SetMouseHappy();
        }
        yield return new WaitForSeconds(1f);
        if (character != null)
        {
            character.SetMouseIdle();
        }
        coroutine = null;
    }
    public void PlayNegativeEmoji()
    {
        if (coroutine != null)
        {
            return;
        }
        coroutine = PlayNegativeEmojiCoroutine();
        StartCoroutine(coroutine);
    }
    private int idSoundAngry = 0;
    private IEnumerator PlayNegativeEmojiCoroutine()
    {
        yield return new WaitForSeconds(0.01f);
        if (character != null)
        {
            character.SetMouseAngry();
        }
        emojiControl.ShowNegative();
        var i = idSoundAngry % 3;
        if (i == 0) SoundManager.Ins.PlayFx(FxType.Angry0);
        else if (i == 1) SoundManager.Ins.PlayFx(FxType.Angry1);
        else SoundManager.Ins.PlayFx(FxType.Angry2);
        idSoundAngry++;
        yield return new WaitForSeconds(0.65f);
        if (character != null)
        {
            character.SetMouseIdle();
        }
        coroutine = null;
    }

    #endregion
    #region Helpers
    private void InitSlots(SlotAttachmentPairList slotList)
    {
        foreach (var pair in slotList.pairs)
        {
            character.TurnSlotAttachment(pair.slotName, pair.attachmentName);
            var slot = character.SkeletonAnimation.Skeleton.FindSlot(pair.slotName);
            if (slot != null) slot.A = 0f;
        }
    }
    private void InitSlotsDone(SlotAttachmentPairList slotList)
    {
        foreach (var pair in slotList.pairs)
        {
            character.TurnSlotAttachment(pair.slotName, pair.attachmentName);
            var slot = character.SkeletonAnimation.Skeleton.FindSlot(pair.slotName);
            if (slot != null) slot.A = 0f;
        }
    }
    private void SetSlotsAlpha(SlotAttachmentPairList slotList, float alpha)
    {
        float clamped = Mathf.Clamp01(alpha);
        foreach (var pair in slotList.pairs)
        {
            character.TurnSlotAttachment(pair.slotName, pair.attachmentName);
            var slot = character.SkeletonAnimation.Skeleton.FindSlot(pair.slotName);
            if (slot != null) slot.A = clamped;
        }
    }
    private void ChangeAlphaSprite(SpriteRenderer sprite, float value)
    {
        if (sprite == null) return;
        Color c = sprite.color;
        c.a = Mathf.Clamp01(value);
        sprite.color = c;
    }
    private void EndGame()
    {
        AdsManager.Ins.ShowEndGame();
        TutorialManager.Ins.SetNewTime(0.5f);
    }
    #endregion
    #region Step1
    private void OnStartStep1()
    {
        DoneStep();
        TryNextStep();
    }
    #endregion
    #region Step2
    [SerializeField] private ShowObjectEffect effectStep2;
    [SerializeField] private float detalOrthographicSize = 29f;
    [SerializeField] private float detalCamY = 18f;
    [SerializeField] private float timerOrThographic = 1.25f;
    [SerializeField] private TapBox tapBox;



    private void OnStartStep2()
    {
        StartCoroutine(IE_DelayStep2());
    }
    IEnumerator IE_DelayStep2()
    {
        yield return new WaitForSeconds(0.5f);
        MoveCamera(cam, cam.orthographicSize + detalOrthographicSize, -detalCamY, timerOrThographic, () =>
            {
                TutorialManager.Ins.SetNewTime(1f);
                effectStep2.Show();
                TutorialManager.Ins.enableCountTime = true;
                tapBox.onCompleted.AddListener(() =>
                {
                    SetIsTrigger();
                    DoneStep();
                    TryNextStep();
                });
            });
    }
    #endregion
    #region Step3

    [SerializeField] private SonDragItemBase itemBrushEye;
    [SerializeField] private OnTransformGoToAffectZone brushEyeTriggerL;
    [SerializeField] private OnTransformGoToAffectZone brushEyeTriggerR;
    [SerializeField] private float brushEyeDuration = 3f;
    [SerializeField] private SlotAttachmentPairList brushEyeSlotsL;
    [SerializeField] private SlotAttachmentPairList brushEyeSlotsR;
    [SerializeField] private bool isTriggerBox = false;

    public void SetIsTrigger()
    {
        OnBrushEyeDragStart();
        isTriggerBox = true;
        _isDragging = true;
    }

    private Tween _tweenBrushEyeL;
    private Tween _tweenBrushEyeR;
    private float _progressL = 0f;
    private float _progressR = 0f;
    private bool _isDragging = false;

    private void OnStartStep3()
    {
        StartStep3();
    }

    private void StartStep3()
    {
        itemBrushEye.AddUseInStep(StepManager.Ins.CurrentStep);
        itemBrushEye.onDragStart.AddListener(OnBrushEyeDragStart);
        itemBrushEye.onDragStop.AddListener(OnBrushEyeDragStop);
        brushEyeTriggerR.onEnterZone.AddListener(OnBrushEyeEnterZoneR);
        brushEyeTriggerR.onOutZone.AddListener(OnBrushEyeExitZoneR);
        brushEyeTriggerL.onEnterZone.AddListener(OnBrushEyeEnterZoneL);
        brushEyeTriggerL.onOutZone.AddListener(OnBrushEyeExitZoneL);

        brushEyeTriggerL.enabled = false;
        brushEyeTriggerR.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(brushEyeSlotsL);
        InitSlots(brushEyeSlotsR);
    }

    private bool _hasFirstDraggedBrushEye = false;
    private bool _isBrushEyeLDone = false;
    private bool _isBrushEyeRDone = false;

    private void OnBrushEyeDragStart()
    {
        if (isTriggerBox == false) return;
        _isDragging = true;
        SoundManager.Ins.PlaySoundSpray();
        if (_hasFirstDraggedBrushEye == false)
        {
            TutorialManager.Ins.SetNewTime(3f);
            _hasFirstDraggedBrushEye = true;
        }
        if (!_isBrushEyeLDone)
        {
            brushEyeTriggerL.enabled = true;
        }
        if (!_isBrushEyeRDone)
        {
            brushEyeTriggerR.enabled = true;
        }
    }

    private void OnBrushEyeDragStop()
    {
        if (isTriggerBox == false) return;
        SoundManager.Ins.StopSoundSpray();
        _isDragging = false;
        brushEyeTriggerL.enabled = false;
        brushEyeTriggerR.enabled = false;
        _tweenBrushEyeL?.Pause();
        _tweenBrushEyeR?.Pause();
        fillCircleBar.Hide();
    }

    private void UpdateFillCircleBar()
    {
        fillCircleBar.Fill(_progressL * 0.5f + _progressR * 0.5f);
    }
    private void OnBrushEyeEnterZoneL()
    {
        fillCircleBar.Show();
        if (_tweenBrushEyeL == null)
        {
            _tweenBrushEyeL = DOVirtual
                .Float(_progressL, 1f, brushEyeDuration * (1f - _progressL), value =>
                {
                    _progressL = value;
                    SetSlotsAlpha(brushEyeSlotsL, value);
                    UpdateFillCircleBar();
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenBrushEyeL = null;
                    OnBrushEyeCompletedL();
                });
        }
        else if (!_tweenBrushEyeL.IsPlaying())
        {
            _tweenBrushEyeL.Play();
        }
    }

    private void OnBrushEyeExitZoneL()
    {
        if (!_isDragging) return;
        _tweenBrushEyeL?.Pause();
        if (_tweenBrushEyeR == null || !_tweenBrushEyeR.IsPlaying())
        {
            //fillCircleBar.Hide();

        }
    }

    private void OnBrushEyeCompletedL()
    {
        _progressL = 1f;
        _isBrushEyeLDone = true;
        brushEyeTriggerL.enabled = false;
        SetSlotsAlpha(brushEyeSlotsL, 1f);
        brushEyeTriggerL.onEnterZone.RemoveListener(OnBrushEyeEnterZoneL);
        brushEyeTriggerL.onOutZone.RemoveListener(OnBrushEyeExitZoneL);
        UpdateFillCircleBar();
        TryFinishBrushEyeStep();
    }

    private void OnBrushEyeEnterZoneR()
    {
        fillCircleBar.Show();
        if (_tweenBrushEyeR == null)
        {
            _tweenBrushEyeR = DOVirtual
                .Float(_progressR, 1f, brushEyeDuration * (1f - _progressR), value =>
                {
                    _progressR = value;
                    SetSlotsAlpha(brushEyeSlotsR, value);
                    UpdateFillCircleBar();
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenBrushEyeR = null;
                    OnBrushEyeCompletedR();
                });
        }
        else if (!_tweenBrushEyeR.IsPlaying())
        {
            _tweenBrushEyeR.Play();
        }
    }

    private void OnBrushEyeExitZoneR()
    {
        if (!_isDragging) return;
        _tweenBrushEyeR?.Pause();
        if (_tweenBrushEyeL == null || !_tweenBrushEyeL.IsPlaying())
        {
            //fillCircleBar.Hide();

        }
    }

    private void OnBrushEyeCompletedR()
    {
        _progressR = 1f;
        _isBrushEyeRDone = true;
        brushEyeTriggerR.enabled = false;
        SetSlotsAlpha(brushEyeSlotsR, 1f);
        brushEyeTriggerR.onEnterZone.RemoveListener(OnBrushEyeEnterZoneR);
        brushEyeTriggerR.onOutZone.RemoveListener(OnBrushEyeExitZoneR);
        UpdateFillCircleBar();
        TryFinishBrushEyeStep();
    }
    private void TryFinishBrushEyeStep()
    {
        if (!_isBrushEyeLDone || !_isBrushEyeRDone) return;
        fillCircleBar.Hide();
        itemBrushEye.onDragStart.RemoveListener(OnBrushEyeDragStart);
        itemBrushEye.onDragStop.RemoveListener(OnBrushEyeDragStop);
        DoneStep();
    }
    #endregion
    #region Step4

    [SerializeField] private SonDragItemBase itemEyeLash;
    [SerializeField] private OnTransformGoToAffectZone eyeLashTriggerL;
    [SerializeField] private OnTransformGoToAffectZone eyeLashTriggerR;
    [SerializeField] private float eyeLashDuration = 3f;
    [SerializeField] private SlotAttachmentPairList eyeLashSlotsL;
    [SerializeField] private SlotAttachmentPairList eyeLashSlotsR;

    private Tween _tweenEyeLashL;
    private Tween _tweenEyeLashR;
    private float _progressEyeLashL = 0f;
    private float _progressEyeLashR = 0f;
    private bool _isEyeLashDragging = false;

    private void OnStartStep4()
    {
        StartStep4();
    }

    private void StartStep4()
    {
        itemEyeLash.AddUseInStep(StepManager.Ins.CurrentStep);
        itemEyeLash.onDragStart.AddListener(OnEyeLashDragStart);
        itemEyeLash.onDragStop.AddListener(OnEyeLashDragStop);
        eyeLashTriggerR.onEnterZone.AddListener(OnEyeLashEnterZoneR);
        eyeLashTriggerR.onOutZone.AddListener(OnEyeLashExitZoneR);
        eyeLashTriggerL.onEnterZone.AddListener(OnEyeLashEnterZoneL);
        eyeLashTriggerL.onOutZone.AddListener(OnEyeLashExitZoneL);

        eyeLashTriggerL.enabled = false;
        eyeLashTriggerR.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(eyeLashSlotsL);
        InitSlots(eyeLashSlotsR);
    }

    private bool _hasFirstDraggedEyeLash = false;
    private bool _isEyeLashLDone = false;
    private bool _isEyeLashRDone = false;

    private void OnEyeLashDragStart()
    {
        _isEyeLashDragging = true;
        SoundManager.Ins.PlaySoundSpray();
        if (_hasFirstDraggedEyeLash == false)
        {
            TutorialManager.Ins.SetNewTime(3f);
            _hasFirstDraggedEyeLash = true;
        }
        if (!_isEyeLashLDone) eyeLashTriggerL.enabled = true;
        if (!_isEyeLashRDone) eyeLashTriggerR.enabled = true;
    }

    private void OnEyeLashDragStop()
    {
        SoundManager.Ins.StopSoundSpray();
        _isEyeLashDragging = false;
        eyeLashTriggerL.enabled = false;
        eyeLashTriggerR.enabled = false;
        _tweenEyeLashL?.Pause();
        _tweenEyeLashR?.Pause();
        fillCircleBar.Hide();
    }

    private void UpdateEyeLashFillCircleBar()
    {
        fillCircleBar.Fill(_progressEyeLashL * 0.5f + _progressEyeLashR * 0.5f);
    }

    private void OnEyeLashEnterZoneL()
    {
        fillCircleBar.Show();
        if (_tweenEyeLashL == null)
        {
            _tweenEyeLashL = DOVirtual
                .Float(_progressEyeLashL, 1f, eyeLashDuration * (1f - _progressEyeLashL), value =>
                {
                    _progressEyeLashL = value;
                    SetSlotsAlpha(eyeLashSlotsL, value);
                    UpdateEyeLashFillCircleBar();
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenEyeLashL = null;
                    OnEyeLashCompletedL();
                });
        }
        else if (!_tweenEyeLashL.IsPlaying())
        {
            _tweenEyeLashL.Play();
        }
    }

    private void OnEyeLashExitZoneL()
    {
        if (!_isEyeLashDragging) return;
        _tweenEyeLashL?.Pause();
        if (_tweenEyeLashR == null || !_tweenEyeLashR.IsPlaying())
        {

            // fillCircleBar.Hide();
        }

    }

    private void OnEyeLashCompletedL()
    {
        _progressEyeLashL = 1f;
        _isEyeLashLDone = true;
        eyeLashTriggerL.enabled = false;
        SetSlotsAlpha(eyeLashSlotsL, 1f);
        eyeLashTriggerL.onEnterZone.RemoveListener(OnEyeLashEnterZoneL);
        eyeLashTriggerL.onOutZone.RemoveListener(OnEyeLashExitZoneL);
        UpdateEyeLashFillCircleBar();
        TryFinishEyeLashStep();
    }

    private void OnEyeLashEnterZoneR()
    {
        fillCircleBar.Show();
        if (_tweenEyeLashR == null)
        {
            _tweenEyeLashR = DOVirtual
                .Float(_progressEyeLashR, 1f, eyeLashDuration * (1f - _progressEyeLashR), value =>
                {
                    _progressEyeLashR = value;
                    SetSlotsAlpha(eyeLashSlotsR, value);
                    UpdateEyeLashFillCircleBar();
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenEyeLashR = null;
                    OnEyeLashCompletedR();
                });
        }
        else if (!_tweenEyeLashR.IsPlaying())
        {
            _tweenEyeLashR.Play();
        }
    }

    private void OnEyeLashExitZoneR()
    {
        if (!_isEyeLashDragging) return;
        _tweenEyeLashR?.Pause();
        if (_tweenEyeLashL == null || !_tweenEyeLashL.IsPlaying())
        {
            //fillCircleBar.Hide();

        }

    }

    private void OnEyeLashCompletedR()
    {
        _progressEyeLashR = 1f;
        _isEyeLashRDone = true;
        eyeLashTriggerR.enabled = false;
        SetSlotsAlpha(eyeLashSlotsR, 1f);
        eyeLashTriggerR.onEnterZone.RemoveListener(OnEyeLashEnterZoneR);
        eyeLashTriggerR.onOutZone.RemoveListener(OnEyeLashExitZoneR);
        UpdateEyeLashFillCircleBar();
        TryFinishEyeLashStep();
    }

    private void TryFinishEyeLashStep()
    {
        if (!_isEyeLashLDone || !_isEyeLashRDone) return;
        fillCircleBar.Hide();
        itemEyeLash.onDragStart.RemoveListener(OnEyeLashDragStart);
        itemEyeLash.onDragStop.RemoveListener(OnEyeLashDragStop);
        DoneStep();
    }
    #endregion
    #region Step5

    [SerializeField] private SonDragItemBase itemLipStick;
    [SerializeField] private OnTransformGoToAffectZone lipStickTrigger;
    [SerializeField] private float lipStickDuration = 3f;
    [SerializeField] private SlotAttachmentPairList lipStickSlots;
    [SerializeField] private SpineAttachmentLocker lipStickSlotsBlock;
    [SerializeField] private bool isLipStickTriggerBox = false;

    public void SetIsLipStickTrigger()
    {
        OnLipStickDragStart();
        isLipStickTriggerBox = true;
        _isLipStickDragging = true;
    }

    private Tween _tweenLipStick;
    private float _progressLipStick = 0f;
    private bool _isLipStickDragging = false;

    private void OnStartStep5()
    {
        StartStep5();
    }

    private void StartStep5()
    {
        lipStickSlotsBlock.enabled = false;
        itemLipStick.AddUseInStep(StepManager.Ins.CurrentStep);
        itemLipStick.onDragStart.AddListener(OnLipStickDragStart);
        itemLipStick.onDragStop.AddListener(OnLipStickDragStop);
        lipStickTrigger.onEnterZone.AddListener(OnLipStickEnterZone);
        lipStickTrigger.onOutZone.AddListener(OnLipStickExitZone);

        lipStickTrigger.enabled = false;
        fillCircleBar.ReFill();
        lipStickSlots.TurnSlotState(true);
        InitSlots(lipStickSlots);
    }


    private void OnLipStickDragStart()
    {
        _isLipStickDragging = true;
        SoundManager.Ins.PlaySoundSpray();
        lipStickTrigger.enabled = true;
    }

    private void OnLipStickDragStop()
    {
        SoundManager.Ins.StopSoundSpray();
        _isLipStickDragging = false;
        lipStickTrigger.enabled = false;
        _tweenLipStick?.Pause();
        fillCircleBar.Hide();
    }

    private void OnLipStickEnterZone()
    {
        fillCircleBar.Show();
        if (_tweenLipStick == null)
        {
            _tweenLipStick = DOVirtual
                .Float(_progressLipStick, 1f, lipStickDuration * (1f - _progressLipStick), value =>
                {
                    _progressLipStick = value;
                    SetSlotsAlpha(lipStickSlots, value);
                    fillCircleBar.Fill(_progressLipStick);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenLipStick = null;
                    OnLipStickCompleted();
                });
        }
        else if (!_tweenLipStick.IsPlaying())
        {
            _tweenLipStick.Play();
        }
    }

    private void OnLipStickExitZone()
    {
        if (!_isLipStickDragging) return;
        _tweenLipStick?.Pause();
        fillCircleBar.Hide();
    }

    private void OnLipStickCompleted()
    {
        _progressLipStick = 1f;
        lipStickTrigger.enabled = false;
        SetSlotsAlpha(lipStickSlots, 1f);
        fillCircleBar.Hide();
        lipStickTrigger.onEnterZone.RemoveListener(OnLipStickEnterZone);
        lipStickTrigger.onOutZone.RemoveListener(OnLipStickExitZone);
        itemLipStick.onDragStart.RemoveListener(OnLipStickDragStart);
        itemLipStick.onDragStop.RemoveListener(OnLipStickDragStop);
        DoneStep();
    }
    #endregion
    #region Step6
    [SerializeField] private ShowObjectEffect effectStep6;
    [SerializeField] private ShowObjectEffect effectStep5;
    [SerializeField] private SonThrowObject throwObjectHair;
    [SerializeField] private SlotAttachmentPairList slotHandBand;
    [SerializeField] private SlotAttachmentPairList slotHair;
    public void OnSetStateSlotHandBand(bool value)
    {
        slotHandBand.TurnSlotState(value);
        slotHair.TurnSlotState(!value);
    }
    private void OnStartStep6()
    {
        effectStep5.Hide(0.5f);
        effectStep6.Show(1.5f);
        throwObjectHair.enabled = true;
        throwObjectHair.Col.enabled = true;
        throwObjectHair.onRemoveItem.AddListener(() =>
        {
            DoneStep();
            TryNextStep();
        });
    }

    #endregion
    #region step7
    [SerializeField] private SlotAttachmentPairList slotHairCutOld;
    [SerializeField] private SlotAttachmentPairList slotHairCutNew;
    [SerializeField] private TriggerWithCertainCollider triggerWithCertainCollider;
    [SerializeField] private SonDragItemBase itemScissors;

    public void OnSetStateSlotHairCut(bool value)
    {
        slotHairCutNew.TurnSlotState(value);
        slotHairCutOld.TurnSlotState(!value);
    }
    private void OnStartStep7()
    {
        itemScissors.AddUseInStep(StepManager.Ins.CurrentStep);
        triggerWithCertainCollider.enabled = true;
        triggerWithCertainCollider.Col.enabled = true;
        triggerWithCertainCollider.OnTriggerEvent.AddListener(() =>
        {
            DoneStep();
        });
    }
    #endregion
    #region Step8

    [SerializeField] private SonDragItemBase itemSprayBottle;
    [SerializeField] private OnTransformGoToAffectZone sprayBottleTrigger;
    [SerializeField] private float sprayBottleDuration = 3f;
    [SerializeField] private SlotAttachmentPairList sprayBottleSlots;
    [SerializeField] private SlotAttachmentPairList sprayBottleSlotsOld;



    private Tween _tweenSprayBottle;
    private float _progressSprayBottle = 0f;
    private bool _isSprayBottleDragging = false;

    private void OnStartStep8()
    {
        StartStep8();
    }

    private void StartStep8()
    {
        itemSprayBottle.AddUseInStep(StepManager.Ins.CurrentStep);
        itemSprayBottle.onDragStart.AddListener(OnSprayBottleDragStart);
        itemSprayBottle.onDragStop.AddListener(OnSprayBottleDragStop);
        sprayBottleTrigger.onEnterZone.AddListener(OnSprayBottleEnterZone);
        sprayBottleTrigger.onOutZone.AddListener(OnSprayBottleExitZone);

        sprayBottleTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(sprayBottleSlots);
    }


    private void OnSprayBottleDragStart()
    {
        _isSprayBottleDragging = true;
        sprayBottleTrigger.enabled = true;
    }

    private void OnSprayBottleDragStop()
    {
        _isSprayBottleDragging = false;
        sprayBottleTrigger.enabled = false;
        _tweenSprayBottle?.Pause();
        fillCircleBar.Hide();
    }
    [SerializeField] private bool isChangeAlphaSlotHairOld = false;
    private void OnSprayBottleEnterZone()
    {
        fillCircleBar.Show();
        if (_tweenSprayBottle == null)
        {
            _tweenSprayBottle = DOVirtual
                .Float(_progressSprayBottle, 1f, sprayBottleDuration * (1f - _progressSprayBottle), value =>
                {
                    _progressSprayBottle = value;
                    SetSlotsAlpha(sprayBottleSlots, value);
                    fillCircleBar.Fill(_progressSprayBottle);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenSprayBottle = null;
                    OnSprayBottleCompleted();
                });
        }
        else if (!_tweenSprayBottle.IsPlaying())
        {
            _tweenSprayBottle.Play();
        }
    }

    private void OnSprayBottleExitZone()
    {
        if (!_isSprayBottleDragging) return;
        _tweenSprayBottle?.Pause();
        fillCircleBar.Hide();
    }

    private void OnSprayBottleCompleted()
    {
        _progressSprayBottle = 1f;
        sprayBottleTrigger.enabled = false;
        SetSlotsAlpha(sprayBottleSlots, 1f);
        SetSlotsAlpha(sprayBottleSlotsOld, 0);
        fillCircleBar.Hide();
        sprayBottleTrigger.onEnterZone.RemoveListener(OnSprayBottleEnterZone);
        sprayBottleTrigger.onOutZone.RemoveListener(OnSprayBottleExitZone);
        itemSprayBottle.onDragStart.RemoveListener(OnSprayBottleDragStart);
        itemSprayBottle.onDragStop.RemoveListener(OnSprayBottleDragStop);
        DoneStep();
    }
    #endregion
    #region Step9

    [SerializeField] private List<SonSnapObject> listSnapObjectHairTie;
    [SerializeField] private List<SonSnapPoint> listSnapPointHairTie;

    [SerializeField] private SlotAttachmentPairList SlotHairDoneLeft;
    [SerializeField] private SlotAttachmentPairList SlotHairOldLeft;
    public void OnSetStateSlotHairDoneL(bool value)
    {
        SlotHairDoneLeft.TurnSlotState(value);
        SlotHairOldLeft.TurnSlotState(!value);
    }
    [SerializeField] private SlotAttachmentPairList SlotHairDoneRight;
    [SerializeField] private SlotAttachmentPairList SlotHairOldRight;

    public void OnSetStateSlotHairDoneR(bool value)
    {
        SlotHairDoneRight.TurnSlotState(value);
        SlotHairOldRight.TurnSlotState(!value);

    }
    private void OnStartStep9()
    {
        foreach (SonSnapPoint point in listSnapPointHairTie)
        {
            point.ChangeCanSnap(true);
        }
        for (int i = 0; i < listSnapObjectHairTie.Count; i++)
        {
            SonSnapObject obj = listSnapObjectHairTie[i];
            obj.enabled = true;
            obj.Col.enabled = true;
            obj.OnSnap.AddListener(() =>
            {
                int removedIndex = listSnapObjectHairTie.IndexOf(obj);
                if (removedIndex < 0) return;
                listSnapObjectHairTie.RemoveAt(removedIndex);
                TutorialManager.Ins.ListTfDragTie.RemoveAt(removedIndex);
                TutorialManager.Ins.ListTfDragTargetTie.RemoveAt(removedIndex);
                if (listSnapObjectHairTie.Count == 0)
                {
                    DoneStep();
                    TryNextStep();
                }
            });
        }
    }
    #endregion
    #region Step10

    [SerializeField] private List<SonSnapObject> listSnapObjectHairDeco;
    [SerializeField] private List<SonSnapPoint> listSnapPointHairDeco;

    [SerializeField] private SlotAttachmentPairList SlotDecoDoneLeft;
    public void OnSetStateSlotDecoDoneL(bool value)
    {
        SlotDecoDoneLeft.TurnSlotState(value);
    }
    [SerializeField] private SlotAttachmentPairList SlotHairDecoDoneRight;

    public void OnSetStateSlotDecoDoneR(bool value)
    {
        SlotHairDecoDoneRight.TurnSlotState(value);

    }
    private void OnStartStep10()
    {
        foreach (SonSnapPoint point in listSnapPointHairDeco)
        {
            point.ChangeCanSnap(true);
        }
        for (int i = 0; i < listSnapObjectHairDeco.Count; i++)
        {
            SonSnapObject obj = listSnapObjectHairDeco[i];
            obj.enabled = true;
            obj.Col.enabled = true;
            obj.OnSnap.AddListener(() =>
            {
                int removedIndex = listSnapObjectHairDeco.IndexOf(obj);
                if (removedIndex < 0) return;
                listSnapObjectHairDeco.RemoveAt(removedIndex);
                TutorialManager.Ins.ListTfDragAccessory.RemoveAt(removedIndex);
                TutorialManager.Ins.ListTfDragTargerAccessory.RemoveAt(removedIndex);
                if (listSnapObjectHairDeco.Count == 0)
                {
                    DoneStep();
                    TryNextStep();
                    EndGame();
                }
            });
        }
    }
    #endregion
}
