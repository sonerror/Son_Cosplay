using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using sonnv;
using System.Collections;
using HoangHH;
using Satisgame;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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
        StartStep();
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
            //ZoomOutCamera();
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
    [SerializeField] private SonDragItemBase itemWaterFaucet;
    [SerializeField] private OnTransformGoToAffectZone waterFaucetTrigger;
    [SerializeField] private float waterFaucetTime = 3f;
    [SerializeField] private SlotAttachmentPairList waterFaucetSlots;
    private Tween _tweenWaterFaucet;

    private void OnStartStep2()
    {
        cam.DOOrthoSize(cam.orthographicSize - 5, 0.3f).OnComplete(() =>
        {
            effectStep1.Show(0.5f);
            TutorialManager.Ins.enableCountTime = true;

            itemWaterFaucet.AddUseInStep(StepManager.Ins.CurrentStep);
            itemWaterFaucet.onDragStart.AddListener(EnableWaterFaucetTrigger);
            itemWaterFaucet.onDragStop.AddListener(DisableWaterFaucetTrigger);
            waterFaucetTrigger.onEnterZone.AddListener(TryWaterFaucet);
            waterFaucetTrigger.onOutZone.AddListener(TryPauseWaterFaucet);
        });
        waterFaucetTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(waterFaucetSlots);
    }
    private bool isTap = false;
    private void EnableWaterFaucetTrigger()
    {
        if (isTap == false)
        {
            TutorialManager.Ins.SetNewTime(3f);
            isTap = true;
        }
        waterFaucetTrigger.enabled = true;

    }

    private void DisableWaterFaucetTrigger() => waterFaucetTrigger.enabled = false;

    private void TryWaterFaucet()
    {
        fillCircleBar.Show();
        if (_tweenWaterFaucet == null)
        {
            _tweenWaterFaucet = DOVirtual
                .Float(0f, 1f, waterFaucetTime, value =>
                {
                    fillCircleBar.Fill(value);
                    SetSlotsAlpha(waterFaucetSlots, value);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenWaterFaucet = null;
                    TryEndWaterFaucet();
                });
        }
        else if (!_tweenWaterFaucet.IsPlaying())
        {
            _tweenWaterFaucet.Play();
        }
    }

    private void TryPauseWaterFaucet()
    {
        fillCircleBar.Hide();
        _tweenWaterFaucet?.Pause();
    }

    private void TryEndWaterFaucet()
    {
        _tweenWaterFaucet?.Kill();
        _tweenWaterFaucet = null;
        fillCircleBar.Hide();
        DisableWaterFaucetTrigger();
        SetSlotsAlpha(waterFaucetSlots, 1f);
        itemWaterFaucet.onDragStart.RemoveListener(EnableWaterFaucetTrigger);
        itemWaterFaucet.onDragStop.RemoveListener(DisableWaterFaucetTrigger);
        waterFaucetTrigger.onEnterZone.RemoveListener(TryWaterFaucet);
        waterFaucetTrigger.onOutZone.RemoveListener(TryPauseWaterFaucet);
        DoneStep();
    }
    #endregion

    #region Step3
    [SerializeField] private TriggerWithCertainCollider triggerSoap;

    private void OnStartStep3()
    {
        triggerSoap.enabled = true;
        triggerSoap.Col.enabled = true;
        triggerSoap.OnTriggerEvent.AddListener(() =>
        {
            DoneStep();
            TryNextStep();
        });
    }
    #endregion

    #region Step4
    [SerializeField] private SonDragItemBase itemBrushHairSoap;
    [SerializeField] private OnTransformGoToAffectZone brushHairSoapTrigger;
    [SerializeField] private float brushHairSoapTime = 3f;
    [SerializeField] private SlotAttachmentPairList brushHairSoapSlots;
    [SerializeField] private SpriteRenderer spriteSoap;
    private Tween _tweenBrushHairSoap;

    private void OnStartStep4()
    {
        itemBrushHairSoap.AddUseInStep(StepManager.Ins.CurrentStep);
        itemBrushHairSoap.onDragStart.AddListener(EnableBrushHairSoapTrigger);
        itemBrushHairSoap.onDragStop.AddListener(DisableBrushHairSoapTrigger);
        brushHairSoapTrigger.onEnterZone.AddListener(TryBrushHairSoap);
        brushHairSoapTrigger.onOutZone.AddListener(TryPauseBrushHairSoap);

        brushHairSoapTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(brushHairSoapSlots);
    }

    private void EnableBrushHairSoapTrigger() => brushHairSoapTrigger.enabled = true;
    private void DisableBrushHairSoapTrigger() => brushHairSoapTrigger.enabled = false;

    private void TryBrushHairSoap()
    {
        fillCircleBar.Show();
        if (_tweenBrushHairSoap == null)
        {
            _tweenBrushHairSoap = DOVirtual
                .Float(0f, 1f, brushHairSoapTime, value =>
                {
                    fillCircleBar.Fill(value);
                    SetSlotsAlpha(brushHairSoapSlots, value);
                    SetSlotsAlpha(waterFaucetSlots, 1f - value);
                    ChangeAlphaSprite(spriteSoap, 1f - value);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenBrushHairSoap = null;
                    TryEndBrushHairSoap();
                });
        }
        else if (!_tweenBrushHairSoap.IsPlaying())
        {
            _tweenBrushHairSoap.Play();
        }
    }

    private void TryPauseBrushHairSoap()
    {
        fillCircleBar.Hide();
        _tweenBrushHairSoap?.Pause();
    }

    private void TryEndBrushHairSoap()
    {
        _tweenBrushHairSoap?.Kill();
        _tweenBrushHairSoap = null;
        fillCircleBar.Hide();
        DisableBrushHairSoapTrigger();
        SetSlotsAlpha(brushHairSoapSlots, 1f);
        itemBrushHairSoap.onDragStart.RemoveListener(EnableBrushHairSoapTrigger);
        itemBrushHairSoap.onDragStop.RemoveListener(DisableBrushHairSoapTrigger);
        brushHairSoapTrigger.onEnterZone.RemoveListener(TryBrushHairSoap);
        brushHairSoapTrigger.onOutZone.RemoveListener(TryPauseBrushHairSoap);
        DoneStep();
    }
    #endregion

    #region Step5
    [SerializeField] private SlotAttachmentPairList waterFaucetSlotsStep5;
    private Tween _tweenWaterFaucetStep5;

    private void OnStartStep5()
    {
        itemWaterFaucet.AddUseInStep(StepManager.Ins.CurrentStep);
        itemWaterFaucet.onDragStart.AddListener(EnableWaterFaucetTrigger);
        itemWaterFaucet.onDragStop.AddListener(DisableWaterFaucetTrigger);
        waterFaucetTrigger.onEnterZone.AddListener(TryWaterFaucetStep5);
        waterFaucetTrigger.onOutZone.AddListener(TryPauseWaterFaucetStep5);

        waterFaucetTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(waterFaucetSlotsStep5);
    }

    private void TryWaterFaucetStep5()
    {
        fillCircleBar.Show();
        if (_tweenWaterFaucetStep5 == null)
        {
            _tweenWaterFaucetStep5 = DOVirtual
                .Float(0f, 1f, waterFaucetTime, value =>
                {
                    fillCircleBar.Fill(value);
                    SetSlotsAlpha(waterFaucetSlotsStep5, value);
                    SetSlotsAlpha(brushHairSoapSlots, 1f - value);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenWaterFaucetStep5 = null;
                    TryEndWaterFaucetStep5();
                });
        }
        else if (!_tweenWaterFaucetStep5.IsPlaying())
        {
            _tweenWaterFaucetStep5.Play();
        }
    }

    private void TryPauseWaterFaucetStep5()
    {
        fillCircleBar.Hide();
        _tweenWaterFaucetStep5?.Pause();
    }

    private void TryEndWaterFaucetStep5()
    {
        _tweenWaterFaucetStep5?.Kill();
        _tweenWaterFaucetStep5 = null;
        fillCircleBar.Hide();
        DisableWaterFaucetTrigger();
        SetSlotsAlpha(waterFaucetSlotsStep5, 1f);
        itemWaterFaucet.onDragStart.RemoveListener(EnableWaterFaucetTrigger);
        itemWaterFaucet.onDragStop.RemoveListener(DisableWaterFaucetTrigger);
        waterFaucetTrigger.onEnterZone.RemoveListener(TryWaterFaucetStep5);
        waterFaucetTrigger.onOutZone.RemoveListener(TryPauseWaterFaucetStep5);
        DoneStep();
    }
    #endregion

    #region Step6
    [SerializeField] private SonDragItemBase itemHairDryer;
    [SerializeField] private OnTransformGoToAffectZone hairDryerTrigger;
    [SerializeField] private float hairDryerTime = 3f;
    [SerializeField] private SlotAttachmentPairList hairDryerSlotsHairNew;
    [SerializeField] private SlotAttachmentPairList hairHairOld;
    private Tween _tweenHairDryer;

    private void OnStartStep6()
    {
        itemHairDryer.AddUseInStep(StepManager.Ins.CurrentStep);
        itemHairDryer.onDragStart.AddListener(EnableHairDryerTrigger);
        itemHairDryer.onDragStop.AddListener(DisableHairDryerTrigger);
        hairDryerTrigger.onEnterZone.AddListener(TryHairDryer);
        hairDryerTrigger.onOutZone.AddListener(TryPauseHairDryer);

        hairDryerTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(hairDryerSlotsHairNew);
    }

    private void EnableHairDryerTrigger() => hairDryerTrigger.enabled = true;
    private void DisableHairDryerTrigger() => hairDryerTrigger.enabled = false;

    private void TryHairDryer()
    {
        fillCircleBar.Show();
        if (_tweenHairDryer == null)
        {
            _tweenHairDryer = DOVirtual
                .Float(0f, 1f, hairDryerTime, value =>
                {
                    fillCircleBar.Fill(value);
                    SetSlotsAlpha(hairDryerSlotsHairNew, value);
                    SetSlotsAlpha(hairHairOld, 1f - value);
                    SetSlotsAlpha(waterFaucetSlotsStep5, 1f - value);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenHairDryer = null;
                    TryEndHairDryer();
                });
        }
        else if (!_tweenHairDryer.IsPlaying())
        {
            _tweenHairDryer.Play();
        }
    }

    private void TryPauseHairDryer()
    {
        fillCircleBar.Hide();
        _tweenHairDryer?.Pause();
    }

    private void TryEndHairDryer()
    {
        _tweenHairDryer?.Kill();
        _tweenHairDryer = null;
        fillCircleBar.Hide();
        DisableHairDryerTrigger();
        SetSlotsAlpha(hairDryerSlotsHairNew, 1f);
        itemHairDryer.onDragStart.RemoveListener(EnableHairDryerTrigger);
        itemHairDryer.onDragStop.RemoveListener(DisableHairDryerTrigger);
        hairDryerTrigger.onEnterZone.RemoveListener(TryHairDryer);
        hairDryerTrigger.onOutZone.RemoveListener(TryPauseHairDryer);
        DoneStep();
    }
    #endregion

    #region Step7
    [SerializeField] private SonDragItemBase itemGel;
    [SerializeField] private OnTransformGoToAffectZone gelTrigger;
    [SerializeField] private float gelTime = 3f;
    [SerializeField] private SlotAttachmentPairList gelSlots;
    [SerializeField] private SlotAttachmentPairList hairBalckOld;
    private Tween _tweenGel;

    private void OnStartStep7()
    {
        itemGel.AddUseInStep(StepManager.Ins.CurrentStep);
        itemGel.onDragStart.AddListener(EnableGelTrigger);
        itemGel.onDragStop.AddListener(DisableGelTrigger);
        gelTrigger.onEnterZone.AddListener(TryGel);
        gelTrigger.onOutZone.AddListener(TryPauseGel);

        gelTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(gelSlots);
    }

    private void EnableGelTrigger() => gelTrigger.enabled = true;
    private void DisableGelTrigger() => gelTrigger.enabled = false;

    private void TryGel()
    {
        fillCircleBar.Show();
        if (_tweenGel == null)
        {
            _tweenGel = DOVirtual
                .Float(0f, 1f, gelTime, value =>
                {
                    fillCircleBar.Fill(value);
                    SetSlotsAlpha(gelSlots, value);
                    SetSlotsAlpha(hairDryerSlotsHairNew, 1 - value);
                    SetSlotsAlpha(hairBalckOld, 1 - value);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenGel = null;
                    TryEndGel();
                });
        }
        else if (!_tweenGel.IsPlaying())
        {
            _tweenGel.Play();
        }
    }

    private void TryPauseGel()
    {
        fillCircleBar.Hide();
        _tweenGel?.Pause();
    }

    private void TryEndGel()
    {
        _tweenGel?.Kill();
        _tweenGel = null;
        fillCircleBar.Hide();
        DisableGelTrigger();
        SetSlotsAlpha(gelSlots, 1f);
        itemGel.onDragStart.RemoveListener(EnableGelTrigger);
        itemGel.onDragStop.RemoveListener(DisableGelTrigger);
        gelTrigger.onEnterZone.RemoveListener(TryGel);
        gelTrigger.onOutZone.RemoveListener(TryPauseGel);
        DoneStep();
        TutorialManager.Ins.enableCountTime = false;

    }
    #endregion
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
    [SerializeField] private List<SonThrowObject> listThrowObjectStart;

    [SerializeField] private SlotAttachmentPairList slotPant;
    [SerializeField] private SlotAttachmentPairList slotCoat;
    [SerializeField] private SlotAttachmentPairList slotShirt;
    [SerializeField] private SlotAttachmentPairList slotShoeL;
    [SerializeField] private SlotAttachmentPairList slotShoeR;
    public void OnSetStateSlotPant(bool value)
    {
        slotPant.TurnSlotState(value);
    }
    public void OnSetStateSlotCoat(bool value)
    {
        slotCoat.TurnSlotState(value);
    }
    public void OnSetStateSlotShirt(bool value)
    {
        slotShirt.TurnSlotState(value);
    }
    public void OnSetStateSlotShoeL(bool value)
    {
        slotShoeL.TurnSlotState(value);
    }

    public void OnSetStateSlotShoeR(bool value)
    {
        slotShoeR.TurnSlotState(value);
    }
    private void StartStep8()
    {
        for (int i = 0; i < listThrowObject.Count; i++)
        {
            SonThrowObject obj = listThrowObject[i];

            obj.onRemoveItem.AddListener(() =>
            {
                int removedIndex = listThrowObject.IndexOf(obj);
                if (removedIndex < 0) return;
                listThrowObject.RemoveAt(removedIndex);
                TutorialManager.Ins.TutorialNode.RemoveAt(removedIndex);
                TutorialManager.Ins.TfItem.RemoveAt(removedIndex);
                if (listThrowObject.Count == 0)
                {
                    TutorialManager.Ins.enableCountTime = false;

                    DoneStep();
                    TryNextStep();
                }
            });
        }
        foreach (SonThrowObject throwObj in listThrowObjectStart)
        {
            throwObj.enabled = true;
            throwObj.Col.enabled = true;
        }
    }
    [SerializeField] private float detalOrthographicSizeEnd = 29f;
    [SerializeField] private float detalCamYEnd = 18f;
    [SerializeField] private float timerOrThographicEnd = 1.25f;
    private void OnStartStep9()
    {
        MoveCamera(cam, detalOrthographicSizeEnd, detalCamYEnd, timerOrThographicEnd, () =>
           {
               SnapObjectScrollUIController.Instance.Show();
               LoadUI();
               EndGame();


           });
    }
    [SerializeField] private List<SnapObjectUI.SnapObjectUIConfig> dressItemSnapConfig;

    private void LoadUI()
    {
        for (int i = 0; i < dressItemSnapConfig.Count; i++)
        {
            SnapObjectScrollUIController.Instance.AddData(dressItemSnapConfig[i]);
            SnapObjectScrollUIController.Instance.ManualSetOrder(GetSequence(dressItemSnapConfig.Count, false));
        }
        TutorialManager.Ins.enableCountTime = true;
        // for (int i = 0; i < dressItemSnapConfig.Count; i++)
        // {
        //     var item = dressItemSnapConfig[i];

        //     item.onSnap += () => SnapDress(item);
        // }
    }
    public Vector3 GetFirstDressItemWorldPos()
    {
        return SnapObjectScrollUIController.Instance.GetFirstScrollObjectWorldPos(Camera.main);
    }
    public Vector2 GetFirstDressItemCanvasPos()
    {
        return SnapObjectScrollUIController.Instance.GetFirstScrollObjectCanvasPos();
    }
    private int countSnapUI = 0;
    private void SnapDress(SnapObjectUI.SnapObjectUIConfig item)
    {
        if (dressItemSnapConfig.Remove(item))
        {
            countSnapUI++;
            item.onSnap = null;
            item.onRelease = null;
            item.onStartDrag = null;
            if (dressItemSnapConfig.Count == 0)
            {
                DoneStep();
                TryNextStep();
            }
            if (countSnapUI >= 5)
            {
                AdsManager.Ins.ShowEndGame();
            }
        }
    }
    private List<int> GetSequence(int count, bool random)
    {
        List<int> result = new List<int>(count);

        for (int i = 0; i < count; i++)
            result.Add(i);

        if (!random)
            return result;

        for (int i = count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (result[i], result[randomIndex]) = (result[randomIndex], result[i]);
        }

        return result;
    }
}
