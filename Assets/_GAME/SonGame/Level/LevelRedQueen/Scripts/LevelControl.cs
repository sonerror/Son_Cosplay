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
    public ClockTimer clockTimer;
    [SerializeField] private CharacterControl character;
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
        SetSlotStart(true);
        StartStep();
    }
    [SerializeField] private SlotAttachmentPairList slotHairOnStart;
    [SerializeField] private SlotAttachmentPairList slotHairOffStart;

    [SerializeField] private SlotAttachmentPairList slotHeadOnStart;
    [SerializeField] private SlotAttachmentPairList slotHeadOffStart;

    private void SetSlotStart(bool value)
    {
        slotHairOnStart.TurnSlotState(value);
        slotHairOffStart.TurnSlotState(!value);

        slotHeadOnStart.TurnSlotState(value);
        slotHeadOffStart.TurnSlotState(!value);
    }
    public virtual void SetStep(int step)
    {
        StepManager.Ins.SetCurrentStep(step);
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
        //SoundManager.Ins.PlayFx(FxType.StartGame);
        SoundManager.Ins.PlayBgm();
        transitionPhase.TransitionToPhase(0, 1, () =>
        {
            cam.orthographicSize += 5;
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
                OnStartStep6();
                return;
            case 5:
                OnStartStep5();
                return;
            case 6:
                OnStartStep7();
                return;
            case 7:
                OnStartStep8();
                return;
            case 8:
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
    [SerializeField] private LoopAnimTransformFloating eyeAnim;
    private IEnumerator PlayPositiveEmojiCoroutine()
    {
        eyeAnim.SetPositionLower75();

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
            eyeAnim.enabled = true;
            eyeAnim.ResetAnim();
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
    public void ShowClockTimer()
    {
        clockTimer.Show(2f);
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
    [SerializeField] private ShowObjectEffect effectStep1;
    [SerializeField] private SonDragItemBase itemHairComb;
    [SerializeField] private OnTransformGoToAffectZone hairCombTrigger;
    [SerializeField] private float hairCombTime = 3f;
    [SerializeField] private SlotAttachmentPairList hairCombSlots;
    [SerializeField] private SlotAttachmentPairList hairAfterCombSlots;
    private Tween _tweenHairComb;

    private void OnStartStep2()
    {
        cam.DOOrthoSize(cam.orthographicSize - 5, 0.3f).OnComplete(() =>
        {
            effectStep1.Show(0.625f);

            itemHairComb.AddUseInStep(StepManager.Ins.CurrentStep);
            itemHairComb.onDragStart.AddListener(EnableHairCombTrigger);
            itemHairComb.onDragStop.AddListener(DisableHairCombTrigger);
            hairCombTrigger.onEnterZone.AddListener(TryHairComb);
            hairCombTrigger.onOutZone.AddListener(TryPauseHairComb);
            TutorialManager.Ins.enableCountTime = true;
        });
        hairCombTrigger.enabled = false;
        fillCircleBar.ReFill();
    }

    private bool isTapComb = false;
    private void EnableHairCombTrigger()
    {
        if (isTapComb == false)
        {
            TutorialManager.Ins.SetNewTime(3f);
            isTapComb = true;
        }
        hairCombTrigger.enabled = true;
        TutorialManager.Ins.SetNewTime(3.5f);

    }

    private void DisableHairCombTrigger() => hairCombTrigger.enabled = false;

    private void TryHairComb()
    {
        fillCircleBar.Show();
        if (_tweenHairComb == null)
        {
            _tweenHairComb = DOVirtual
                .Float(0f, 1f, hairCombTime, value =>
                {
                    fillCircleBar.Fill(value);
                    SetSlotsAlpha(hairCombSlots, 1 - value);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenHairComb = null;
                    TryEndHairComb();
                });
        }
        else if (!_tweenHairComb.IsPlaying())
        {
            _tweenHairComb.Play();
        }
    }

    private void TryPauseHairComb()
    {
        fillCircleBar.Hide();
        _tweenHairComb?.Pause();
    }

    private void TryEndHairComb()
    {
        _tweenHairComb?.Kill();
        _tweenHairComb = null;
        fillCircleBar.Hide();
        DisableHairCombTrigger();
        SetSlotsAlpha(hairCombSlots, 0f);
        itemHairComb.onDragStart.RemoveListener(EnableHairCombTrigger);
        itemHairComb.onDragStop.RemoveListener(DisableHairCombTrigger);
        hairCombTrigger.onEnterZone.RemoveListener(TryHairComb);
        hairCombTrigger.onOutZone.RemoveListener(TryPauseHairComb);
        DoneStep();
    }
    #endregion
    #region Step3
    [SerializeField] private SonDragItemBase itemSprayBottle;
    [SerializeField] private OnTransformGoToAffectZone sprayBottleTrigger;
    [SerializeField] private float sprayBottleTime = 3f;
    [SerializeField] private SlotAttachmentPairList sprayBottleSlots;
    [SerializeField] private SlotAttachmentPairList hairOldSlots;
    private Tween _tweenSprayBottle;

    private void OnStartStep3()
    {
        itemSprayBottle.AddUseInStep(StepManager.Ins.CurrentStep);
        itemSprayBottle.onDragStart.AddListener(EnableSprayBottleTrigger);
        itemSprayBottle.onDragStop.AddListener(DisableSprayBottleTrigger);
        sprayBottleTrigger.onEnterZone.AddListener(TrySprayBottle);
        sprayBottleTrigger.onOutZone.AddListener(TryPauseSprayBottle);

        sprayBottleTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(sprayBottleSlots);
    }

    private bool isTapSpray = false;
    private void EnableSprayBottleTrigger()
    {
        if (isTapSpray == false)
        {
            TutorialManager.Ins.SetNewTime(3f);
            isTapSpray = true;
        }
        sprayBottleTrigger.enabled = true;
    }

    private void DisableSprayBottleTrigger() => sprayBottleTrigger.enabled = false;

    private void TrySprayBottle()
    {
        fillCircleBar.Show();
        if (_tweenSprayBottle == null)
        {
            _tweenSprayBottle = DOVirtual
                .Float(0f, 1f, sprayBottleTime, value =>
                {
                    fillCircleBar.Fill(value);
                    SetSlotsAlpha(sprayBottleSlots, value);
                    SetSlotsAlpha(hairOldSlots, 1 - value);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenSprayBottle = null;
                    TryEndSprayBottle();
                });
        }
        else if (!_tweenSprayBottle.IsPlaying())
        {
            _tweenSprayBottle.Play();
        }
    }

    private void TryPauseSprayBottle()
    {
        fillCircleBar.Hide();
        _tweenSprayBottle?.Pause();
    }

    private void TryEndSprayBottle()
    {
        _tweenSprayBottle?.Kill();
        _tweenSprayBottle = null;
        fillCircleBar.Hide();
        DisableSprayBottleTrigger();
        SetSlotsAlpha(sprayBottleSlots, 1f);
        itemSprayBottle.onDragStart.RemoveListener(EnableSprayBottleTrigger);
        itemSprayBottle.onDragStop.RemoveListener(DisableSprayBottleTrigger);
        sprayBottleTrigger.onEnterZone.RemoveListener(TrySprayBottle);
        sprayBottleTrigger.onOutZone.RemoveListener(TryPauseSprayBottle);
        DoneStep();
    }
    #endregion
    #region Step4
    [SerializeField] private SlotAttachmentPairList hairCombSlots2;
    [SerializeField] private SlotAttachmentPairList hairOldCombSlots2;
    private Tween _tweenHairComb2;
    private bool isTapComb2 = false;

    private void OnStartStep4()
    {
        TutorialManager.Ins.enableCountTime = true;

        itemHairComb.AddUseInStep(StepManager.Ins.CurrentStep);
        itemHairComb.onDragStart.AddListener(EnableHairCombTrigger2);
        itemHairComb.onDragStop.AddListener(DisableHairCombTrigger2);
        hairCombTrigger.onEnterZone.AddListener(TryHairComb2);
        hairCombTrigger.onOutZone.AddListener(TryPauseHairComb2);
        hairCombTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(hairCombSlots2);
    }

    private void EnableHairCombTrigger2()
    {
        if (isTapComb2 == false)
        {
            TutorialManager.Ins.SetNewTime(3f);
            isTapComb2 = true;
        }
        hairCombTrigger.enabled = true;
    }

    private void DisableHairCombTrigger2() => hairCombTrigger.enabled = false;

    private void TryHairComb2()
    {
        fillCircleBar.Show();
        if (_tweenHairComb2 == null)
        {
            _tweenHairComb2 = DOVirtual
                .Float(0f, 1f, hairCombTime, value =>
                {
                    fillCircleBar.Fill(value);
                    SetSlotsAlpha(hairCombSlots2, value);
                    SetSlotsAlpha(hairOldCombSlots2, 1 - value);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenHairComb2 = null;
                    TryEndHairComb2();
                });
        }
        else if (!_tweenHairComb2.IsPlaying())
        {
            _tweenHairComb2.Play();
        }
    }

    private void TryPauseHairComb2()
    {
        fillCircleBar.Hide();
        _tweenHairComb2?.Pause();
    }

    private void TryEndHairComb2()
    {
        _tweenHairComb2?.Kill();
        _tweenHairComb2 = null;
        fillCircleBar.Hide();
        DisableHairCombTrigger2();
        SetSlotsAlpha(hairCombSlots2, 1f);
        itemHairComb.onDragStart.RemoveListener(EnableHairCombTrigger2);
        itemHairComb.onDragStop.RemoveListener(DisableHairCombTrigger2);
        hairCombTrigger.onEnterZone.RemoveListener(TryHairComb2);
        hairCombTrigger.onOutZone.RemoveListener(TryPauseHairComb2);
        DoneStep();
    }
    #endregion
    #region Step5
    [SerializeField] private SonSnapObject snapHairLObject;
    [SerializeField] private SonSnapPoint snapHairLPoint;
    [SerializeField] private SlotAttachmentPairList hairMixL;

    private void OnStartStep5()
    {
        snapHairLPoint.ChangeCanSnap(true);
        snapHairLObject.OnSnap.AddListener(() =>
        {
            hairMixL.TurnSlotState(true);
            DoneStep();
            TryNextStep();
        });
    }
    #endregion
    #region Step6
    [SerializeField] private SonSnapObject snapHairRObject;
    [SerializeField] private SonSnapPoint snapHairRPoint;
    [SerializeField] private SlotAttachmentPairList hairMixR;

    private void OnStartStep6()
    {
        snapHairRPoint.ChangeCanSnap(true);
        snapHairRObject.OnSnap.AddListener(() =>
        {
            hairMixR.TurnSlotState(true);
            DoneStep();
            TryNextStep();
        });
    }

    #endregion
    #region Step7

    [SerializeField] private List<SonSnapObject> listSnapTieRObject;
    public List<SonSnapObject> ListSnapTieRObject => listSnapTieRObject;
    [SerializeField] private List<SonSnapPoint> listSnapTieRPoint;

    [SerializeField] private SlotAttachmentPairList hairTieL;
    public void SetStateSlotTieL()
    {
        hairTieL.TurnSlotState(true);
    }
    [SerializeField] private SlotAttachmentPairList hairTieR;
    public void SetStateSlotTieR()
    {
        hairTieR.TurnSlotState(true);
    }
    [SerializeField] private SlotAttachmentPairList hairEyePad;
    public void SetStateSloEyePad()
    {
        hairEyePad.TurnSlotState(true);
    }
    [SerializeField] private SlotAttachmentPairList hairClip;
    public void SetStateSlotHairClip()
    {
        hairClip.TurnSlotState(true);
    }

    private void OnStartStep7()
    {
        foreach (SonSnapPoint point in listSnapTieRPoint)
        {
            point.ChangeCanSnap(true);
        }
        for (int i = 0; i < listSnapTieRObject.Count; i++)
        {
            SonSnapObject obj = listSnapTieRObject[i];
            obj.OnSnap.AddListener(() =>
            {
                int removedIndex = listSnapTieRObject.IndexOf(obj);
                if (removedIndex < 0) return;
                listSnapTieRObject.RemoveAt(removedIndex);
                TutorialManager.Ins.TutorialNode.RemoveAt(removedIndex);
                TutorialManager.Ins.TfItem.RemoveAt(removedIndex);
                if (listSnapTieRObject.Count == 0)
                {
                    TutorialManager.Ins.enableCountTime = false;
                    DoneStep();
                    TryNextStep();
                }
            });
        }
    }
    #endregion
    #region Step8
    [SerializeField] private float detalOrthographicSize = 29f;
    [SerializeField] private float detalCamY = 18f;
    [SerializeField] private float timerOrThographic = 1.25f;

    [SerializeField] private SpriteAlphaGroup BGOld;
    [SerializeField] private SpriteAlphaGroup BGNew;
    [SerializeField] private SpriteRenderer spriteRenderer1, spriteRenderer2;
    private void OnStartStep8()
    {
        effectStep1.Hide(0.75f);
        effectStep1.onHide.AddListener(() =>
        {
            spriteRenderer1.gameObject.SetActive(true);
            spriteRenderer2.gameObject.SetActive(true);
            BGOld.FadeOut(timerOrThographic / 2);
            BGNew.FadeIn(timerOrThographic / 2);
            MoveCamera(cam, cam.orthographicSize + detalOrthographicSize, -detalCamY, timerOrThographic, () =>
            {
                StartStep8();
                TutorialManager.Ins.enableCountTime = true;

            });
        });
    }
    [SerializeField] private List<SonThrowObject> listThrowObject;
    [SerializeField] private SonThrowObject throwObjectShirt;

    [SerializeField] private SlotAttachmentPairList slotShirt;
    [SerializeField] private SlotAttachmentPairList slotSkirt;

    public void OnSetStateSlotShirt(bool value)
    {
        slotShirt.TurnSlotState(value);
    }
    public void OnSetStateSlotSkirt(bool value)
    {
        slotSkirt.TurnSlotState(value);
    }
    private void StartStep8()
    {
        throwObjectShirt.enabled = true;
        throwObjectShirt.Col.enabled = true;

        for (int i = 0; i < listThrowObject.Count; i++)
        {
            SonThrowObject obj = listThrowObject[i];

            obj.onRemoveItem.AddListener(() =>
            {
                int removedIndex = listThrowObject.IndexOf(obj);
                if (removedIndex < 0) return;
                listThrowObject.RemoveAt(removedIndex);
                TutorialManager.Ins.ListTfClothes.RemoveAt(removedIndex);
                TutorialManager.Ins.ListTfTargetClothes.RemoveAt(removedIndex);
                if (listThrowObject.Count == 0)
                {
                    DoneStep();
                    TryNextStep();
                }
                if (listThrowObject.Count == 1)
                {
                    EndGame();
                }
            });
        }
    }
    #endregion
}
