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
        StartStep();
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

    private void StartGamePlay()
    {
        GamePlayScreen uiScreen = UIManager.Ins.GetUI(0);
        uiScreen.logoUI.SetActive(false);
        EventManager.TriggerEvent("ShowIconLv");
        SoundManager.Ins.PlayBgm();
        transitionPhase.TransitionToPhase(0, 1, () =>
        {
            cam.orthographicSize += 6.5f;
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
    public virtual void StartStep()
    {
        isDoneStep = false;
        switch (StepManager.Ins.CurrentStep)
        {
            case 0:
                return;
            case 1:
                TutorialManager.Ins.SetNewTime(1f);
                OnStartStep2();
                return;
            case 2:
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
                return;
            default:
                return;
        }
    }
    #region  Base

    public void PlayPositiveEmoji()
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
    #endregion

    #region Step1
    private void OnStartStep1()
    {
        DoneStep();
        TryNextStep();
    }
    #endregion

    private void EndGame()
    {
        AdsManager.Ins.ShowEndGame();
        TutorialManager.Ins.SetNewTime(0.5f);
    }
    #region Step2
    [SerializeField] private List<SonThrowObject> listThrowObject;
    [SerializeField] private SlotAttachmentPairList slotTopBase;
    [SerializeField] private SlotAttachmentPairList slotDressBase;
    [SerializeField] private SlotAttachmentPairList slotShoeL;
    [SerializeField] private SlotAttachmentPairList slotShoeR;

    public void OnSetStateSlotTopBase(bool value)
    {
        slotTopBase.TurnSlotState(value);
    }
    public void OnSetStateSlotDressBase(bool value)
    {
        slotDressBase.TurnSlotState(value);
    }
    public void OnSetStateSlotShoeL(bool value)
    {
        slotShoeL.TurnSlotState(value);
    }
    public void OnSetStateSlotShoeR(bool value)
    {
        slotShoeR.TurnSlotState(value);
    }
    private int countThrow = 0;
    private void OnStartStep2()
    {
        TutorialManager.Ins.enableCountTime = true;
        for (int i = 0; i < listThrowObject.Count; i++)
        {
            SonThrowObject obj = listThrowObject[i];
            obj.enabled = true;
            obj.Col.enabled = true;
            obj.onRemoveItem.AddListener(() =>
            {
                int removedIndex = listThrowObject.IndexOf(obj);
                if (removedIndex < 0) return;
                listThrowObject.RemoveAt(removedIndex);
                TutorialManager.Ins.ListTfClothes.RemoveAt(removedIndex);
                TutorialManager.Ins.ListTfTargetClothes.RemoveAt(removedIndex);
                countThrow++;
                if (countThrow == 1)
                {
                    TutorialManager.Ins.SetNewTime(3.5f);
                }
                if (listThrowObject.Count == 0)
                {
                    DoneStep();
                    TryNextStep();
                }
            });
        }
    }
    #endregion
    #region Step3
    [SerializeField] private ShowObjectEffect effectStep3;
    [SerializeField] private SonSnapObject snapObjHairClip;

    [SerializeField] private SlotAttachmentPairList slotHair;
    [SerializeField] private SlotAttachmentPairList slotHairClip;
    public void SetStateSlotHair()
    {
        slotHair.TurnSlotState(false);
        slotHairClip.TurnSlotState(true);
    }
    private void OnStartStep3()
    {
        effectStep3.Show(0.5f);
        snapObjHairClip.OnSnap.AddListener(() =>
        {
            DoneStep();
            TryNextStep();
        });
    }
    #endregion

    #region Step4
    [SerializeField] private SpineBoneIKControl eyeLoop;
    [SerializeField] private ShowObjectEffect effectStep4;

    [SerializeField] private SonDragItemBase itemSprayBottle;
    [SerializeField] private OnTransformGoToAffectZone sprayBottleTrigger;
    [SerializeField] private float sprayBottleDuration = 3f;
    [SerializeField] private SlotAttachmentPairList sprayBottleSlots;
    [SerializeField] private SpineAttachmentLocker slotHandL;
    private Tween _tweenSprayBottle;

    private void OnStartStep4()
    {
        eyeLoop.DisableIKControl();
        effectStep3.Hide();
        effectStep4.Show(0.5f);
        StartStep4();
    }
    private void StartStep4()
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
    private bool _hasFirstDraggedSprayBottle = false;

    private void OnSprayBottleDragStart()
    {
        if (_hasFirstDraggedSprayBottle == false)
        {
            TutorialManager.Ins.SetNewTime(3f);
            _hasFirstDraggedSprayBottle = true;
        }
        sprayBottleTrigger.enabled = true;
    }

    private void OnSprayBottleDragStop() => sprayBottleTrigger.enabled = false;
    private bool isSetHandL = false;
    private void OnSprayBottleEnterZone()
    {
        fillCircleBar.Show();
        if (_tweenSprayBottle == null)
        {
            _tweenSprayBottle = DOVirtual
                .Float(0f, 1f, sprayBottleDuration, value =>
                {
                    fillCircleBar.Fill(value);
                    SetSlotsAlpha(sprayBottleSlots, value);
                    if (value >= 0.5f && isSetHandL == false)
                    {
                        slotHandL.enabled = true;
                        isSetHandL = true;
                    }
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
        fillCircleBar.Hide();
        _tweenSprayBottle?.Pause();
    }

    private void OnSprayBottleCompleted()
    {
        _tweenSprayBottle?.Kill();
        _tweenSprayBottle = null;
        fillCircleBar.Hide();
        OnSprayBottleDragStop();
        SetSlotsAlpha(sprayBottleSlots, 1f);
        itemSprayBottle.onDragStart.RemoveListener(OnSprayBottleDragStart);
        itemSprayBottle.onDragStop.RemoveListener(OnSprayBottleDragStop);
        sprayBottleTrigger.onEnterZone.RemoveListener(OnSprayBottleEnterZone);
        sprayBottleTrigger.onOutZone.RemoveListener(OnSprayBottleExitZone);
        DoneStep();
    }
    #endregion

    #region Step5
    [SerializeField] private SonDragItemBase itemSprayBottle2;
    [SerializeField] private OnTransformGoToAffectZone sprayBottle2Trigger;
    [SerializeField] private float sprayBottle2Duration = 3f;
    [SerializeField] private SlotAttachmentPairList sprayBottle2Slots;
    private Tween _tweenSprayBottle2;

    private void OnStartStep5()
    {
        itemSprayBottle2.AddUseInStep(StepManager.Ins.CurrentStep);
        itemSprayBottle2.onDragStart.AddListener(OnSprayBottle2DragStart);
        itemSprayBottle2.onDragStop.AddListener(OnSprayBottle2DragStop);
        sprayBottle2Trigger.onEnterZone.AddListener(OnSprayBottle2EnterZone);
        sprayBottle2Trigger.onOutZone.AddListener(OnSprayBottle2ExitZone);

        sprayBottle2Trigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(sprayBottle2Slots);
    }

    private bool _hasFirstDraggedSprayBottle2 = false;

    private void OnSprayBottle2DragStart()
    {
        if (_hasFirstDraggedSprayBottle2 == false)
        {
            TutorialManager.Ins.SetNewTime(3f);
            _hasFirstDraggedSprayBottle2 = true;
        }
        sprayBottle2Trigger.enabled = true;
    }

    private void OnSprayBottle2DragStop() => sprayBottle2Trigger.enabled = false;

    private void OnSprayBottle2EnterZone()
    {
        fillCircleBar.Show();
        if (_tweenSprayBottle2 == null)
        {
            _tweenSprayBottle2 = DOVirtual
                .Float(0f, 1f, sprayBottle2Duration, value =>
                {
                    fillCircleBar.Fill(value);
                    SetSlotsAlpha(sprayBottle2Slots, value);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenSprayBottle2 = null;
                    OnSprayBottle2Completed();
                });
        }
        else if (!_tweenSprayBottle2.IsPlaying())
        {
            _tweenSprayBottle2.Play();
        }
    }

    private void OnSprayBottle2ExitZone()
    {
        fillCircleBar.Hide();
        _tweenSprayBottle2?.Pause();
    }

    private void OnSprayBottle2Completed()
    {
        eyeLoop.EnableIKControl();

        _tweenSprayBottle2?.Kill();
        _tweenSprayBottle2 = null;
        fillCircleBar.Hide();
        OnSprayBottle2DragStop();
        SetSlotsAlpha(sprayBottle2Slots, 1f);
        itemSprayBottle2.onDragStart.RemoveListener(OnSprayBottle2DragStart);
        itemSprayBottle2.onDragStop.RemoveListener(OnSprayBottle2DragStop);
        sprayBottle2Trigger.onEnterZone.RemoveListener(OnSprayBottle2EnterZone);
        sprayBottle2Trigger.onOutZone.RemoveListener(OnSprayBottle2ExitZone);
        DoneStep();
    }
    #endregion
    #region Step6
    [SerializeField] private List<SonSnapObject> listSnapSticker;
    [SerializeField] private SlotAttachmentPairList stickerFace;
    public void SetStateSlotStickerFace()
    {
        stickerFace.TurnSlotState(true);
    }
    [SerializeField] private SlotAttachmentPairList stickerHand;
    public void SetStateSlotStickerHand()
    {
        stickerHand.TurnSlotState(true);
    }
    [SerializeField] private SlotAttachmentPairList stickerLeg;
    public void SetStateSlotStickerLeg()
    {
        stickerLeg.TurnSlotState(true);
    }
    [SerializeField] private ShowObjectEffect effectStep5;

    private void OnStartStep6()
    {
        effectStep4.Hide();

        effectStep5.Show(0.5f);
        for (int i = 0; i < listSnapSticker.Count; i++)
        {
            SonSnapObject obj = listSnapSticker[i];
            obj.enabled = true;
            obj.Col.enabled = true;
            obj.OnSnap.AddListener(() =>
            {
                int removedIndex = listSnapSticker.IndexOf(obj);
                if (removedIndex < 0) return;
                listSnapSticker.RemoveAt(removedIndex);
                TutorialManager.Ins.ListTfSticker.RemoveAt(removedIndex);
                TutorialManager.Ins.ListTfTargetSticker.RemoveAt(removedIndex);
                if (listSnapSticker.Count == 0)
                {
                    TutorialManager.Ins.enableCountTime = false;
                    DoneStep();
                    TryNextStep();
                }
            });
        }
    }
    #endregion
    private Tween _tweenSticker;


    private void OnStartStep7()
    {
        SetSlotsAlpha(sticker, 0f);
        StartCoroutine(IE_DelayStep7());
    }
    IEnumerator IE_DelayStep7()
    {
        yield return new WaitForSeconds(0.5f);
        effectStep5.Hide();
        float duration = 3f;

        clockTimer.OnTimeOut = () =>
        {
            DoneStep();
            TryNextStep();

        };
        clockTimer.Show(duration);
        StickerfadeSprites(duration);
    }
    [SerializeField] private SlotAttachmentPairList sticker;
    [SerializeField] private SlotAttachmentPairList stickerOn;

    private void StickerfadeSprites(float duration)
    {
        _tweenSticker = DOVirtual
                .Float(0f, 1f, duration, value =>
                {
                    SetSlotsAlpha(sticker, value);
                    SetSlotsAlpha(stickerOn, 1 - value);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenSticker = null;
                    SetSlotsAlpha(sticker, 1f);
                    SetSlotsAlpha(stickerOn, 0);

                });
    }
    #region Step8
    [SerializeField] private List<SonThrowObject> listThrowObjectStiker;
    [SerializeField] private SlotAttachmentPairList slotStickerFace;
    [SerializeField] private SlotAttachmentPairList slotStickerBody;
    [SerializeField] private SlotAttachmentPairList slotStickerLeg;

    public void OnSetStateSlotStickerFace(bool value)
    {
        slotStickerFace.TurnSlotState(value);
    }
    public void OnSetStateSlotStickerBody(bool value)
    {
        slotStickerBody.TurnSlotState(value);
    }
    public void OnSetStateSlotStickerLeg(bool value)
    {
        slotStickerLeg.TurnSlotState(value);
    }

    private void OnStartStep8()
    {
        TutorialManager.Ins.enableCountTime = true;
        for (int i = 0; i < listThrowObjectStiker.Count; i++)
        {
            SonThrowObject obj = listThrowObjectStiker[i];
            obj.enabled = true;
            obj.Col.enabled = true;
            obj.onRemoveItem.AddListener(() =>
            {
                int removedIndex = listThrowObjectStiker.IndexOf(obj);
                if (removedIndex < 0) return;
                listThrowObjectStiker.RemoveAt(removedIndex);
                TutorialManager.Ins.ListTfDragSticker.RemoveAt(removedIndex);
                TutorialManager.Ins.ListTfDragTargetSticker.RemoveAt(removedIndex);
                if (listThrowObjectStiker.Count == 0)
                {
                    TutorialManager.Ins.enableCountTime = false;
                    DoneStep();
                    TryNextStep();
                }
            });
        }
    }
    #endregion
    #region Step9
    [SerializeField] private ShowObjectEffect effectStep6;
    [SerializeField] private float detalOrthographicSize = 29f;
    [SerializeField] private float detalCamY = 18f;
    [SerializeField] private float timerOrThographic = 1.25f;

    [SerializeField] private SpriteAlphaGroup BGOld;
    [SerializeField] private SpriteAlphaGroup BGNew;
    private void OnStartStep9()
    {
        StartCoroutine(IE_DelayStep9());
    }

    IEnumerator IE_DelayStep9()
    {
        yield return new WaitForSeconds(0.5f);
        BGOld.FadeOut(timerOrThographic / 2);
        BGNew.FadeIn(timerOrThographic / 2);
        MoveCamera(cam, cam.orthographicSize + detalOrthographicSize, -detalCamY, timerOrThographic, () =>
        {
            effectStep6.Show();
            EndGame();
            TutorialManager.Ins.enableCountTime = true;
        });
    }
    #endregion
}
