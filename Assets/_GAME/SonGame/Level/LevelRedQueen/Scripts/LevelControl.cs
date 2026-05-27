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
using Spine.Unity;
public class LevelControl : Singleton<LevelControl>
{
    #region Setting
    [SerializeField] private AudioSource vfx1;
    [SerializeField] private AudioSource vfx2;

    [SerializeField] private LoopAnimTransformFloating eyeLoop;
    [SerializeField] private SpineBoneIKControl boneEye;
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
    [SerializeField] private SlotAttachmentPairList slotHairStart;

    protected virtual void Start()
    {
        slotOffStart.TurnSlotState(false);
        //SetSlotStart();
        StartStep();
        //slotHairStart.TurnSlotState(true);
        //InitSlots(slotHairStart);
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
            EventManager.TriggerEvent("ShowIconLv");
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
        if (targetOrthoSize < 3.55f)
        {
            targetOrthoSize = 3.55f;
        }
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
            //cam.orthographicSize += targetOrthoSizeStep2;
            // cam.transform.DOLocalMoveY(targetLocalYStep2, 0.1f);

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
                TutorialManager.Ins.enableCountTime = false;
                TutorialManager.Ins.SetNewTime(0.5f);
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
            //case 5:
            //    OnStartStep6();
            //    return;
            //case 5:
            //    OnStartStep7();
            //    return;
            case 5:
                OnStartStep8();
                return;
            //case 6:
            //    OnStartStep9();
            //    return;
            case 6:
                OnStartStep10();
                return;
            case 7:
                OnStartStep11();
                return;
            case 8:
                OnStartStep12();
                return;
            case 9:
                OnStartStep13();
                return;
            case 10:
                OnStartStep14();
                return;
            case 11:
                OnStartStep15();
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
    private void SetDoneStep()
    {
        PlayPositiveEmoji();
        isDoneStep = true;

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
    private void FadeOutSlots(SlotAttachmentPairList slotList, float duration = 0.3f, System.Action onComplete = null)
    {
        float currentAlpha = 1f;

        if (slotList.pairs != null && slotList.pairs.Count > 0)
        {
            var firstSlot = character.SkeletonAnimation.Skeleton.FindSlot(slotList.pairs[0].slotName);
            if (firstSlot != null) currentAlpha = firstSlot.A;
        }

        DOVirtual.Float(currentAlpha, 0f, duration, value =>
        {
            SetSlotsAlpha(slotList, value);
        })
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            onComplete?.Invoke();
        });
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
        TutorialManager.Ins.enableCountTime = true;


    }
    #endregion

    #region Step1
    private void OnStartStep1()
    {
        SetDoneStep();
        TryNextStep();
    }
    #endregion

    #region Step2
    [SerializeField] private List<SonThrowObject> sonThrowObjects;
    [SerializeField] private List<SonThrowObject> sonThrowObjectsStart;
    private int countThrow = 0;
    private void OnStartStep2()
    {
        TutorialManager.Ins.enableCountTime = true;

        foreach (SonThrowObject throwObj in sonThrowObjectsStart)
        {
            throwObj.enabled = true;
            throwObj.Col.enabled = true;
        }
        for (int i = 0; i < sonThrowObjects.Count; i++)
        {
            SonThrowObject obj = sonThrowObjects[i];
            obj.onRemoveItem.AddListener(() =>
            {
                int removedIndex = sonThrowObjects.IndexOf(obj);
                if (removedIndex < 0) return;
                sonThrowObjects.RemoveAt(removedIndex);
                // TutorialManager.Ins.ListClothes.RemoveAt(removedIndex);
                // TutorialManager.Ins.ListThrowClothes.RemoveAt(removedIndex);
                // countThrow++;
                // if (countThrow == 1)
                // {
                //     TutorialManager.Ins.SetNewTime(3f);

                // }
                if (sonThrowObjects.Count == 0)
                {
                    SetDoneStep();
                    TryNextStep();
                }
            });
        }
    }
    [SerializeField] private SlotAttachmentPairList slotGlass;
    public void SetStateSlotHat(bool value)
    {
        slotGlass.TurnSlotState(value);
    }

    #endregion
    #region Step3

    private void OnStartStep3()
    {
        foreach (SonSnapPoint point in listSnapPointEye)
        {
            point.ChangeCanSnap(true);
        }
        for (int i = 0; i < listSnapObjectEye.Count; i++)
        {
            SonSnapObject obj = listSnapObjectEye[i];
            obj.OnSnap.AddListener(() =>
            {
                int removedIndex = listSnapObjectEye.IndexOf(obj);
                if (removedIndex < 0) return;
                listSnapObjectEye.RemoveAt(removedIndex);
                TutorialManager.Ins.ListObjEye.RemoveAt(removedIndex);
                TutorialManager.Ins.ListPointEye.RemoveAt(removedIndex);
                if (listSnapObjectEye.Count == 0)
                {
                    SetDoneStep();
                    TryNextStep();
                }
            });
        }
    }
    [SerializeField] private List<SonSnapPoint> listSnapPointEye;
    [SerializeField] private List<SonSnapObject> listSnapObjectEye;
    [SerializeField] private SlotAttachmentPairList slotEyeL;
    public void SetStateSlotEyeL()
    {
        slotEyeL.TurnSlotState(true);
    }
    [SerializeField] private SlotAttachmentPairList slotEyeR;
    public void SetStateSlotEyeR()
    {
        slotEyeR.TurnSlotState(true);
    }

    #endregion
    #region Step4
    [SerializeField] private SonSnapPoint listSnapPointWigCap;
    [SerializeField] private SonSnapObject listSnapObjectWigCap;
    [SerializeField] private SlotAttachmentPairList slotWigCap;
    [SerializeField] private SlotAttachmentPairList slothair;
    public void SetStateSlotWigCap()
    {
        slotWigCap.TurnSlotState(true);
        slothair.TurnSlotState(false);
    }
    private void OnStartStep4()
    {
        listSnapPointWigCap.ChangeCanSnap(true);
        listSnapObjectWigCap.OnSnap.AddListener(() =>
        {
            SetDoneStep();
            TryNextStep();
        });
    }
    #endregion
    #region Step5
    [SerializeField] private TriggerWithCertainCollider triggerContour;
    private void OnStartStep5()
    {
        triggerContour.enabled = true;
        triggerContour.Col.enabled = true;
        StartStep5();
    }

    [SerializeField] private AudioSource audioSource;


    [SerializeField] private SonDragItemBase itemContourPen;
    [SerializeField] private OnTransformGoToAffectZone contourTrigger;
    [SerializeField] private float contourDuration = 3f;
    [SerializeField] private SlotAttachmentPairList contourSlots;
    [SerializeField] private bool isTriggerContour = false;

    public void SetStateIsTriggerContour()
    {
        isTriggerContour = true;
        OnContourDragStart();
    }

    private Tween _tweenContour;
    private float _progressContour = 0f;
    private bool _isContourDragging = false;
    private bool _isContourRevealDone = false;

    private void StartStep5()
    {
        itemContourPen.AddUseInStep(StepManager.Ins.CurrentStep);
        itemContourPen.onDragStart.AddListener(OnContourDragStart);
        itemContourPen.onDragStop.AddListener(OnContourDragStop);
        contourTrigger.onEnterZone.AddListener(OnContourEnterZone);
        contourTrigger.onOutZone.AddListener(OnContourExitZone);
        contourTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(contourSlots);
        InitSlots(highlightSlots);
        InitSlots(powderSlots);

    }

    private void OnContourDragStart()
    {
        if (isTriggerContour == false) return;
        _isContourDragging = true;
        contourTrigger.enabled = true;
    }

    private void OnContourDragStop()
    {
        if (isTriggerContour == false) return;
        _isContourDragging = false;
        contourTrigger.enabled = false;
        _tweenContour?.Pause();
        fillCircleBar.Hide();
    }

    private void OnContourEnterZone()
    {
        if (isTriggerContour == false) return;
        fillCircleBar.Show();
        audioSource.Play();
        if (_tweenContour == null)
        {
            _tweenContour = DOVirtual
                .Float(_progressContour, 1f, contourDuration * (1f - _progressContour), value =>
                {
                    _progressContour = value;
                    SetSlotsAlpha(contourSlots, value);
                    SetSlotsAlpha(highlightSlots, value);
                    SetSlotsAlpha(powderSlots, value);
                    fillCircleBar.Fill(_progressContour);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenContour = null;
                    OnContourCompleted();
                });
        }
        else if (!_tweenContour.IsPlaying())
        {
            _tweenContour.Play();
        }
    }

    private void OnContourExitZone()
    {
        audioSource.Stop();
        if (isTriggerContour == false) return;
        if (!_isContourDragging) return;

        _tweenContour?.Pause();
        fillCircleBar.Hide();
    }

    private void OnContourCompleted()
    {
        _progressContour = 1f;
        _isContourRevealDone = false;
        contourTrigger.enabled = false;
        SetSlotsAlpha(contourSlots, 1f);
        SetSlotsAlpha(highlightSlots, 1);
        SetSlotsAlpha(powderSlots, 1);
        fillCircleBar.Hide();
        contourTrigger.onEnterZone.RemoveListener(OnContourEnterZone);
        contourTrigger.onOutZone.RemoveListener(OnContourExitZone);
        itemContourPen.onDragStart.RemoveListener(OnContourDragStart);
        itemContourPen.onDragStop.RemoveListener(OnContourDragStop);
        SetDoneStep();
    }
    #endregion
    #region Step6
    [SerializeField] private TriggerWithCertainCollider triggerHighlight;
    private void OnStartStep6()
    {
        triggerHighlight.enabled = true;
        triggerHighlight.Col.enabled = true;
        StartStep6();
    }
    [SerializeField] private SonDragItemBase itemHighlightPen;
    [SerializeField] private OnTransformGoToAffectZone highlightTrigger;
    [SerializeField] private float highlightDuration = 3f;
    [SerializeField] private SlotAttachmentPairList highlightSlots;
    [SerializeField] private bool isTriggerHighlight = false;
    public void SetStateIsTriggerHighlight()
    {
        isTriggerHighlight = true;
        OnHighlightDragStart();
    }
    private Tween _tweenHighlight;
    private float _progressHighlight = 0f;
    private bool _isHighlightDragging = false;
    private bool _isHighlightRevealDone = false;
    private void StartStep6()
    {
        itemHighlightPen.AddUseInStep(StepManager.Ins.CurrentStep);
        itemHighlightPen.onDragStart.AddListener(OnHighlightDragStart);
        itemHighlightPen.onDragStop.AddListener(OnHighlightDragStop);
        highlightTrigger.onEnterZone.AddListener(OnHighlightEnterZone);
        highlightTrigger.onOutZone.AddListener(OnHighlightExitZone);
        highlightTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(highlightSlots);
    }
    private void OnHighlightDragStart()
    {
        if (isTriggerHighlight == false) return;
        _isHighlightDragging = true;
        highlightTrigger.enabled = true;
    }
    private void OnHighlightDragStop()
    {
        if (isTriggerHighlight == false) return;
        _isHighlightDragging = false;
        highlightTrigger.enabled = false;
        _tweenHighlight?.Pause();
        fillCircleBar.Hide();
    }
    private void OnHighlightEnterZone()
    {
        if (isTriggerHighlight == false) return;
        fillCircleBar.Show();
        audioSource.Play();

        if (_tweenHighlight == null)
        {
            _tweenHighlight = DOVirtual
                .Float(_progressHighlight, 1f, highlightDuration * (1f - _progressHighlight), value =>
                {
                    _progressHighlight = value;
                    SetSlotsAlpha(highlightSlots, value);
                    fillCircleBar.Fill(_progressHighlight);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenHighlight = null;
                    OnHighlightCompleted();
                });
        }
        else if (!_tweenHighlight.IsPlaying())
        {
            _tweenHighlight.Play();
        }
    }
    private void OnHighlightExitZone()
    {
        audioSource.Stop();
        if (isTriggerHighlight == false) return;
        if (!_isHighlightDragging) return;

        _tweenHighlight?.Pause();
        fillCircleBar.Hide();
    }
    private void OnHighlightCompleted()
    {
        _progressHighlight = 1f;
        _isHighlightRevealDone = false;
        highlightTrigger.enabled = false;
        SetSlotsAlpha(highlightSlots, 1f);
        fillCircleBar.Hide();
        highlightTrigger.onEnterZone.RemoveListener(OnHighlightEnterZone);
        highlightTrigger.onOutZone.RemoveListener(OnHighlightExitZone);
        itemHighlightPen.onDragStart.RemoveListener(OnHighlightDragStart);
        itemHighlightPen.onDragStop.RemoveListener(OnHighlightDragStop);
        SetDoneStep();
    }
    #endregion
    #region Step7
    [SerializeField] private TriggerWithCertainCollider triggerPowder;
    private void OnStartStep7()
    {
        triggerPowder.enabled = true;
        triggerPowder.Col.enabled = true;
        StartStep7();
    }
    [SerializeField] private AudioSource vfxPowder;

    [SerializeField] private SonDragItemBase itemPowderBrush;
    [SerializeField] private OnTransformGoToAffectZone powderTrigger;
    [SerializeField] private float powderDuration = 3f;
    [SerializeField] private SlotAttachmentPairList powderSlots;
    [SerializeField] private bool isTriggerPowder = false;
    public void SetStateIsTriggerPowder()
    {
        isTriggerPowder = true;
        OnPowderDragStart();
    }
    private Tween _tweenPowder;
    private float _progressPowder = 0f;
    private bool _isPowderDragging = false;
    private void StartStep7()
    {
        itemPowderBrush.AddUseInStep(StepManager.Ins.CurrentStep);
        itemPowderBrush.onDragStart.AddListener(OnPowderDragStart);
        itemPowderBrush.onDragStop.AddListener(OnPowderDragStop);
        powderTrigger.onEnterZone.AddListener(OnPowderEnterZone);
        powderTrigger.onOutZone.AddListener(OnPowderExitZone);
        powderTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(powderSlots);
    }
    private void OnPowderDragStart()
    {
        if (isTriggerPowder == false) return;
        _isPowderDragging = true;
        powderTrigger.enabled = true;
    }
    private void OnPowderDragStop()
    {
        if (isTriggerPowder == false) return;
        _isPowderDragging = false;
        powderTrigger.enabled = false;
        _tweenPowder?.Pause();
        fillCircleBar.Hide();
    }
    private void OnPowderEnterZone()
    {
        if (isTriggerPowder == false) return;
        fillCircleBar.Show();
        vfxPowder.Play();
        if (_tweenPowder == null)
        {
            _tweenPowder = DOVirtual
                .Float(_progressPowder, 1f, powderDuration * (1f - _progressPowder), value =>
                {
                    _progressPowder = value;
                    SetSlotsAlpha(powderSlots, value);
                    fillCircleBar.Fill(_progressPowder);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenPowder = null;
                    OnPowderCompleted();
                });
        }
        else if (!_tweenPowder.IsPlaying())
        {
            _tweenPowder.Play();
        }
    }
    private void OnPowderExitZone()
    {
        vfxPowder.Stop();
        if (isTriggerPowder == false) return;
        if (!_isPowderDragging) return;

        _tweenPowder?.Pause();
        fillCircleBar.Hide();
    }
    private void OnPowderCompleted()
    {
        _progressPowder = 1f;
        powderTrigger.enabled = false;
        SetSlotsAlpha(powderSlots, 1f);
        fillCircleBar.Hide();
        powderTrigger.onEnterZone.RemoveListener(OnPowderEnterZone);
        powderTrigger.onOutZone.RemoveListener(OnPowderExitZone);
        itemPowderBrush.onDragStart.RemoveListener(OnPowderDragStart);
        itemPowderBrush.onDragStop.RemoveListener(OnPowderDragStop);
        SetDoneStep();
        TutorialManager.Ins.enableCountTime = false;
    }
    #endregion
    #region Step8
    [SerializeField] private ShowObjectEffect showStep1Phase1;
    [SerializeField] private ShowObjectEffect showStep2Phase1;

    private void OnStartStep8()
    {
        StartStep8();
    }
    IEnumerator IE_DelayStep8()
    {
        yield return new WaitForSeconds(0.5f);
        showStep1Phase1.Hide(0.5f);
        showStep2Phase1.Show(1.25f);
        showStep2Phase1.onShowComplete.AddListener(() =>
        {
            TutorialManager.Ins.enableCountTime = true;
            triggerPhanMat.enabled = true;
            triggerPhanMat.Col.enabled = true;
            itemPhanMat.AddUseInStep(StepManager.Ins.CurrentStep);
            itemPhanMat.onDragStart.AddListener(OnPhanMatDragStart);
            itemPhanMat.onDragStop.AddListener(OnPhanMatDragStop);
            phanMatTrigger.onEnterZone.AddListener(OnPhanMatEnterZone);
            phanMatTrigger.onOutZone.AddListener(OnPhanMatExitZone);
            phanMatTrigger.enabled = false;
            fillCircleBar.ReFill();
            InitSlots(phanMatSlots);
            InitSlots(eyelinerSlots);
        });
    }
    private void StartStep8()
    {
        StartCoroutine(IE_DelayStep8());
    }
    [SerializeField] private AudioSource vfxBrush;
    [SerializeField] private TriggerWithCertainCollider triggerPhanMat;

    [SerializeField] private SonDragItemBase itemPhanMat;
    [SerializeField] private OnTransformGoToAffectZone phanMatTrigger;
    [SerializeField] private float phanMatDuration = 3f;
    [SerializeField] private SlotAttachmentPairList phanMatSlots;
    [SerializeField] private bool isTriggerPhanMat = false;
    public void SetStateIsTriggerPhanMat()
    {
        isTriggerPhanMat = true;
        OnPhanMatDragStart();
    }
    private Tween _tweenPhanMat;
    private float _progressPhanMat = 0f;
    private bool _isPhanMatDragging = false;
    private bool _isPhanMatRevealDone = false;

    private void OnPhanMatDragStart()
    {
        if (isTriggerPhanMat == false) return;
        _isPhanMatDragging = true;
        phanMatTrigger.enabled = true;
    }
    private void OnPhanMatDragStop()
    {
        if (isTriggerPhanMat == false) return;
        _isPhanMatDragging = false;
        phanMatTrigger.enabled = false;
        _tweenPhanMat?.Pause();
        fillCircleBar.Hide();
    }
    private void OnPhanMatEnterZone()
    {
        if (isTriggerPhanMat == false) return;
        fillCircleBar.Show();
        vfxBrush.Play();
        if (_tweenPhanMat == null)
        {
            _tweenPhanMat = DOVirtual
                .Float(_progressPhanMat, 1f, phanMatDuration * (1f - _progressPhanMat), value =>
                {
                    _progressPhanMat = value;
                    SetSlotsAlpha(phanMatSlots, value);
                    SetSlotsAlpha(eyelinerSlots, value);
                    fillCircleBar.Fill(_progressPhanMat);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenPhanMat = null;
                    OnPhanMatCompleted();
                });
        }
        else if (!_tweenPhanMat.IsPlaying())
        {
            _tweenPhanMat.Play();
        }
    }
    private void OnPhanMatExitZone()
    {
        vfxBrush.Stop();
        if (isTriggerPhanMat == false) return;
        if (!_isPhanMatDragging) return;
        _tweenPhanMat?.Pause();
        fillCircleBar.Hide();
    }
    private void OnPhanMatCompleted()
    {
        _progressPhanMat = 1f;
        _isPhanMatRevealDone = false;
        phanMatTrigger.enabled = false;
        SetSlotsAlpha(phanMatSlots, 1f);
        SetSlotsAlpha(eyelinerSlots, 1f);

        fillCircleBar.Hide();
        phanMatTrigger.onEnterZone.RemoveListener(OnPhanMatEnterZone);
        phanMatTrigger.onOutZone.RemoveListener(OnPhanMatExitZone);
        itemPhanMat.onDragStart.RemoveListener(OnPhanMatDragStart);
        itemPhanMat.onDragStop.RemoveListener(OnPhanMatDragStop);
        SetDoneStep();
    }
    #endregion
    #region Step9
    private void OnStartStep9()
    {
        StartStep9();
    }
    [SerializeField] private SonDragItemBase itemEyeliner;
    [SerializeField] private OnTransformGoToAffectZone eyelinerTrigger;
    [SerializeField] private float eyelinerDuration = 3f;
    [SerializeField] private SlotAttachmentPairList eyelinerSlots;

    private Tween _tweenEyeliner;
    private float _progressEyeliner = 0f;
    private bool _isEyelinerDragging = false;
    private void StartStep9()
    {
        itemEyeliner.AddUseInStep(StepManager.Ins.CurrentStep);
        itemEyeliner.onDragStart.AddListener(OnEyelinerDragStart);
        itemEyeliner.onDragStop.AddListener(OnEyelinerDragStop);
        eyelinerTrigger.onEnterZone.AddListener(OnEyelinerEnterZone);
        eyelinerTrigger.onOutZone.AddListener(OnEyelinerExitZone);
        eyelinerTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(eyelinerSlots);
    }
    private void OnEyelinerDragStart()
    {
        _isEyelinerDragging = true;
        eyelinerTrigger.enabled = true;
    }
    private void OnEyelinerDragStop()
    {
        _isEyelinerDragging = false;
        eyelinerTrigger.enabled = false;
        _tweenEyeliner?.Pause();
        fillCircleBar.Hide();
    }
    private void OnEyelinerEnterZone()
    {
        fillCircleBar.Show();
        vfx2.Play();

        if (_tweenEyeliner == null)
        {
            _tweenEyeliner = DOVirtual
                .Float(_progressEyeliner, 1f, eyelinerDuration * (1f - _progressEyeliner), value =>
                {
                    _progressEyeliner = value;
                    SetSlotsAlpha(eyelinerSlots, value);
                    fillCircleBar.Fill(_progressEyeliner);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenEyeliner = null;
                    OnEyelinerCompleted();
                });
        }
        else if (!_tweenEyeliner.IsPlaying())
        {
            _tweenEyeliner.Play();
        }
    }
    private void OnEyelinerExitZone()
    {
        vfx2.Stop();

        if (!_isEyelinerDragging) return;
        _tweenEyeliner?.Pause();
        fillCircleBar.Hide();
    }
    private void OnEyelinerCompleted()
    {
        _progressEyeliner = 1f;
        eyelinerTrigger.enabled = false;
        SetSlotsAlpha(eyelinerSlots, 1f);
        fillCircleBar.Hide();
        eyelinerTrigger.onEnterZone.RemoveListener(OnEyelinerEnterZone);
        eyelinerTrigger.onOutZone.RemoveListener(OnEyelinerExitZone);
        itemEyeliner.onDragStart.RemoveListener(OnEyelinerDragStart);
        itemEyeliner.onDragStop.RemoveListener(OnEyelinerDragStop);
        SetDoneStep();
    }
    #endregion

    #region Step10

    private void OnStartStep10()
    {
        foreach (SonSnapPoint point in listSnapPointEyeLash)
        {
            point.ChangeCanSnap(true);
        }
        for (int i = 0; i < listSnapObjectEyeLash.Count; i++)
        {
            SonSnapObject obj = listSnapObjectEyeLash[i];
            obj.OnSnap.AddListener(() =>
            {
                int removedIndex = listSnapObjectEyeLash.IndexOf(obj);
                if (removedIndex < 0) return;
                listSnapObjectEyeLash.RemoveAt(removedIndex);
                TutorialManager.Ins.ListObjEyeLash.RemoveAt(removedIndex);
                TutorialManager.Ins.ListPointEyeLash.RemoveAt(removedIndex);
                if (listSnapObjectEyeLash.Count == 0)
                {
                    SetDoneStep();
                    TryNextStep();
                }
            });
        }
    }
    [SerializeField] private List<SonSnapPoint> listSnapPointEyeLash;
    [SerializeField] private List<SonSnapObject> listSnapObjectEyeLash;
    [SerializeField] private SlotAttachmentPairList slotEyeLashL;
    public void SetStateSlotEyeLashL()
    {
        slotEyeLashL.TurnSlotState(true);
    }
    [SerializeField] private SlotAttachmentPairList slotEyeLashR;
    public void SetStateSlotEyeLashR()
    {
        slotEyeLashR.TurnSlotState(true);
    }

    #endregion

    #region Step11
    private void OnStartStep11()
    {
        StartStep10();
    }
    [SerializeField] private SonDragItemBase itemBrow;
    [SerializeField] private OnTransformGoToAffectZone browTrigger;
    [SerializeField] private float browDuration = 3f;
    [SerializeField] private SlotAttachmentPairList browSlots;
    [SerializeField] private SlotAttachmentPairList browSlotsOld;

    private Tween _tweenBrow;
    private float _progressBrow = 0f;
    private bool _isBrowDragging = false;
    private bool _isBrowRevealDone = false;
    private void StartStep10()
    {
        itemBrow.AddUseInStep(StepManager.Ins.CurrentStep);
        itemBrow.onDragStart.AddListener(OnBrowDragStart);
        itemBrow.onDragStop.AddListener(OnBrowDragStop);
        browTrigger.onEnterZone.AddListener(OnBrowEnterZone);
        browTrigger.onOutZone.AddListener(OnBrowExitZone);
        browTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(browSlots);
    }
    private void OnBrowDragStart()
    {
        _isBrowDragging = true;
        browTrigger.enabled = true;
    }
    private void OnBrowDragStop()
    {
        _isBrowDragging = false;
        browTrigger.enabled = false;
        _tweenBrow?.Pause();
        fillCircleBar.Hide();
    }
    private void OnBrowEnterZone()
    {
        fillCircleBar.Show();
        vfx1.Play();

        if (_tweenBrow == null)
        {
            _tweenBrow = DOVirtual
                .Float(_progressBrow, 1f, browDuration * (1f - _progressBrow), value =>
                {
                    _progressBrow = value;
                    SetSlotsAlpha(browSlots, value);
                    SetSlotsAlpha(browSlotsOld, 1 - value);
                    fillCircleBar.Fill(_progressBrow);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenBrow = null;
                    OnBrowCompleted();
                });
        }
        else if (!_tweenBrow.IsPlaying())
        {
            _tweenBrow.Play();
        }
    }
    private void OnBrowExitZone()
    {
        vfx1.Stop();

        if (!_isBrowDragging) return;
        _tweenBrow?.Pause();
        fillCircleBar.Hide();
    }
    private void OnBrowCompleted()
    {
        _progressBrow = 1f;
        _isBrowRevealDone = false;
        browTrigger.enabled = false;
        SetSlotsAlpha(browSlots, 1f);
        SetSlotsAlpha(browSlotsOld, 0);

        fillCircleBar.Hide();
        browTrigger.onEnterZone.RemoveListener(OnBrowEnterZone);
        browTrigger.onOutZone.RemoveListener(OnBrowExitZone);
        itemBrow.onDragStart.RemoveListener(OnBrowDragStart);
        itemBrow.onDragStop.RemoveListener(OnBrowDragStop);
        SetDoneStep();
    }
    #endregion
    #region Step12
    private void OnStartStep12()
    {
        StartStep11();
    }
    [SerializeField] private SonDragItemBase itemPaineBody;
    [SerializeField] private OnTransformGoToAffectZone paineBodyTrigger;
    [SerializeField] private float paineBodyDuration = 3f;
    [SerializeField] private SlotAttachmentPairList paineBodySlots;

    private Tween _tweenPaineBody;
    private float _progressPaineBody = 0f;
    private bool _isPaineBodyDragging = false;
    private bool _isPaineBodyRevealDone = false;
    private void StartStep11()
    {
        itemPaineBody.AddUseInStep(StepManager.Ins.CurrentStep);
        itemPaineBody.onDragStart.AddListener(OnPaineBodyDragStart);
        itemPaineBody.onDragStop.AddListener(OnPaineBodyDragStop);
        paineBodyTrigger.onEnterZone.AddListener(OnPaineBodyEnterZone);
        paineBodyTrigger.onOutZone.AddListener(OnPaineBodyExitZone);
        paineBodyTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(paineBodySlots);
    }
    private void OnPaineBodyDragStart()
    {
        _isPaineBodyDragging = true;
        paineBodyTrigger.enabled = true;
    }
    private void OnPaineBodyDragStop()
    {
        _isPaineBodyDragging = false;
        paineBodyTrigger.enabled = false;
        _tweenPaineBody?.Pause();
        fillCircleBar.Hide();
    }
    private void OnPaineBodyEnterZone()
    {
        fillCircleBar.Show();
        vfx2.Play();

        if (_tweenPaineBody == null)
        {
            _tweenPaineBody = DOVirtual
                .Float(_progressPaineBody, 1f, paineBodyDuration * (1f - _progressPaineBody), value =>
                {
                    _progressPaineBody = value;
                    SetSlotsAlpha(paineBodySlots, value);
                    fillCircleBar.Fill(_progressPaineBody);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenPaineBody = null;
                    OnPaineBodyCompleted();
                });
        }
        else if (!_tweenPaineBody.IsPlaying())
        {
            _tweenPaineBody.Play();
        }
    }
    private void OnPaineBodyExitZone()
    {
        vfx2.Stop();

        if (!_isPaineBodyDragging) return;
        _tweenPaineBody?.Pause();
        fillCircleBar.Hide();
    }
    private void OnPaineBodyCompleted()
    {
        _progressPaineBody = 1f;
        _isPaineBodyRevealDone = false;
        paineBodyTrigger.enabled = false;
        SetSlotsAlpha(paineBodySlots, 1f);
        fillCircleBar.Hide();
        paineBodyTrigger.onEnterZone.RemoveListener(OnPaineBodyEnterZone);
        paineBodyTrigger.onOutZone.RemoveListener(OnPaineBodyExitZone);
        itemPaineBody.onDragStart.RemoveListener(OnPaineBodyDragStart);
        itemPaineBody.onDragStop.RemoveListener(OnPaineBodyDragStop);
        SetDoneStep();
    }
    #endregion
    #region Step13
    private void OnStartStep13()
    {
        StartStep12();
    }
    [SerializeField] private SpineAttachmentLocker mouth;
    [SerializeField] private SonDragItemBase itemLipstick;
    [SerializeField] private OnTransformGoToAffectZone lipstickTrigger;
    [SerializeField] private float lipstickDuration = 3f;
    [SerializeField] private SlotAttachmentPairList lipstickSlots;

    private Tween _tweenLipstick;
    private float _progressLipstick = 0f;
    private bool _isLipstickDragging = false;
    private bool _isLipstickRevealDone = false;
    private void StartStep12()
    {
        mouth.enabled = false;
        itemLipstick.AddUseInStep(StepManager.Ins.CurrentStep);
        itemLipstick.onDragStart.AddListener(OnLipstickDragStart);
        itemLipstick.onDragStop.AddListener(OnLipstickDragStop);
        lipstickTrigger.onEnterZone.AddListener(OnLipstickEnterZone);
        lipstickTrigger.onOutZone.AddListener(OnLipstickExitZone);
        lipstickTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(lipstickSlots);
    }
    private void OnLipstickDragStart()
    {
        _isLipstickDragging = true;
        lipstickTrigger.enabled = true;
    }
    private void OnLipstickDragStop()
    {
        _isLipstickDragging = false;
        lipstickTrigger.enabled = false;
        _tweenLipstick?.Pause();
        fillCircleBar.Hide();
    }
    private void OnLipstickEnterZone()
    {
        fillCircleBar.Show();
        vfx2.Play();

        if (_tweenLipstick == null)
        {
            _tweenLipstick = DOVirtual
                .Float(_progressLipstick, 1f, lipstickDuration * (1f - _progressLipstick), value =>
                {
                    _progressLipstick = value;
                    SetSlotsAlpha(lipstickSlots, value);
                    fillCircleBar.Fill(_progressLipstick);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenLipstick = null;
                    OnLipstickCompleted();
                });
        }
        else if (!_tweenLipstick.IsPlaying())
        {
            _tweenLipstick.Play();
        }
    }
    private void OnLipstickExitZone()
    {
        vfx2.Stop();

        if (!_isLipstickDragging) return;
        _tweenLipstick?.Pause();
        fillCircleBar.Hide();
    }
    private void OnLipstickCompleted()
    {
        _progressLipstick = 1f;
        _isLipstickRevealDone = false;
        lipstickTrigger.enabled = false;
        SetSlotsAlpha(lipstickSlots, 1f);
        fillCircleBar.Hide();
        lipstickTrigger.onEnterZone.RemoveListener(OnLipstickEnterZone);
        lipstickTrigger.onOutZone.RemoveListener(OnLipstickExitZone);
        itemLipstick.onDragStart.RemoveListener(OnLipstickDragStart);
        itemLipstick.onDragStop.RemoveListener(OnLipstickDragStop);
        SetDoneStep();
    }
    #endregion


    #region Step14
    [SerializeField] private ShowObjectEffect showStep3Phase1;
    [SerializeField] private SonSnapPoint listSnapPointHairFake;
    [SerializeField] private SonSnapObject listSnapObjHairFake;
    [SerializeField] private SlotAttachmentPairList slotHairFake;
    public void SetStateSlotHairFake()
    {
        slotWigCap.TurnSlotState(false);
        slotHairFake.TurnSlotState(true);
    }
    private void OnStartStep14()
    {
        StartCoroutine(IE_DelayStep14());
    }

    IEnumerator IE_DelayStep14()
    {
        yield return new WaitForSeconds(0.5f);
        showStep2Phase1.Hide(0.5f);
        showStep3Phase1.Show(1.25f);
        showStep3Phase1.onShowComplete.AddListener(() =>
        {
            listSnapPointHairFake.ChangeCanSnap(true);
            listSnapObjHairFake.OnSnap.AddListener(() =>
            {
                SetDoneStep();
                TryNextStep();
                TutorialManager.Ins.enableCountTime = false;
            });
        });
    }
    #endregion
    [SerializeField] private ShowObjectEffect showStep1Phase2;
    [SerializeField] private SpriteAlphaGroup bg1;
    [SerializeField] private SpriteAlphaGroup bg2;
    [SerializeField] private SpriteRenderer bg3;

    [SerializeField] private float camY = 0f;
    [SerializeField] private float camOrthoSize = 10f;

    private void OnStartStep15()
    {
        StartCoroutine(IE_DelayStep15());
    }

    IEnumerator IE_DelayStep15()
    {
        yield return new WaitForSeconds(0.5f);
        showStep1Phase2.Hide(0.5f);
        bg1.FadeOut();
        bg2.FadeIn();
        showStep1Phase2.onHide.AddListener(() =>
        {
            MoveCamera(cam, camOrthoSize, camY, 0.75f, () =>
            {
                EndGame();
            });
        });
        yield return new WaitForSeconds(1);
        bg3.gameObject.SetActive(true);
    }
}
