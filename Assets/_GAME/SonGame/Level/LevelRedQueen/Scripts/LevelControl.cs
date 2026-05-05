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
        //SetSlotStart();
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
                TutorialManager.Ins.SetNewTime(0.5f);
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
                //OnStartStep8();
                return;
            case 8:
                // OnStartStep9();
                return;
            case 9:
                // OnStartStep10();
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
        TutorialManager.Ins.enableCountTime = true;


    }
    #endregion
    #region Step1
    private void OnStartStep1()
    {
        DoneStep();
        TryNextStep();
    }
    #endregion


    #region Step9

    [SerializeField] private List<SonThrowObject> listThrowClothes;


    [SerializeField] private SlotAttachmentPairList SlotPant;
    public void OnSetStateSlotPant(bool value)
    {
        SlotPant.TurnSlotState(value);

    }
    [SerializeField] private SlotAttachmentPairList SlotShirt;
    public void OnSetStateSlotShirt(bool value)
    {
        SlotShirt.TurnSlotState(value);

    }
    [SerializeField] private SlotAttachmentPairList SlotHat;
    public void OnSetStateSlotHat(bool value)
    {
        SlotHat.TurnSlotState(value);

    }
    [SerializeField] private SlotAttachmentPairList SlotShoesL;
    public void OnSetStateSlotShoesL(bool value)
    {
        SlotShoesL.TurnSlotState(value);

    }
    [SerializeField] private SlotAttachmentPairList SlotShoesR;
    public void OnSetStateSlotShoesR(bool value)
    {
        SlotShoesR.TurnSlotState(value);

    }
    private void OnStartStep2()
    {
        TutorialManager.Ins.enableCountTime = true;
        OnStartStepThrowClothes();
    }
    private int countThrow = 0;
    private void OnStartStepThrowClothes()
    {

        for (int i = 0; i < listThrowClothes.Count; i++)
        {
            SonThrowObject obj = listThrowClothes[i];
            obj.enabled = true;
            obj.Col.enabled = true;
            obj.onRemoveItem.AddListener(() =>
            {
                int removedIndex = listThrowClothes.IndexOf(obj);
                if (removedIndex < 0) return;
                listThrowClothes.RemoveAt(removedIndex);
                TutorialManager.Ins.ListClothes.RemoveAt(removedIndex);
                TutorialManager.Ins.ListThrowClothes.RemoveAt(removedIndex);
                countThrow++;
                if (countThrow == 1)
                {
                    TutorialManager.Ins.SetNewTime(3f);

                }
                if (listThrowClothes.Count == 0)
                {
                    DoneStep();
                    TryNextStep();
                }
            });
        }
    }
    #endregion

    #region Step3
    [SerializeField] private ShowObjectEffect effectStep2;
    [SerializeField] private float detalOrthographicSize = 29f;
    [SerializeField] private float detalCamY = 18f;
    [SerializeField] private float timerOrThographic = 1.25f;

    private void OnStartStep3()
    {
        TutorialManager.Ins.enableCountTime = false;

        StartCoroutine(IE_DelayStep3());
    }
    IEnumerator IE_DelayStep3()
    {
        yield return new WaitForSeconds(0.5f);
        MoveCamera(cam, cam.orthographicSize + detalOrthographicSize, -detalCamY, timerOrThographic, () =>
            {
                TutorialManager.Ins.SetNewTime(1f);
                effectStep2.Show();
                TutorialManager.Ins.enableCountTime = true;
                StartStep3();
            });
    }

    [SerializeField] private SonDragItemBase itemBodyPainter;
    [SerializeField] private OnTransformGoToAffectZone bodyPainterTrigger;
    [SerializeField] private float bodyPainterDuration = 3f;
    [SerializeField] private SlotAttachmentPairList bodyPainterSlots;
    [SerializeField] private SpineAttachmentLocker lipStickSlotsBlock;
    private Tween _tweenBodyPainter;
    private float _progressBodyPainter = 0f;
    private bool _isBodyPainterDragging = false;
    private void StartStep3()
    {
        lipStickSlotsBlock.enabled = false;
        itemBodyPainter.AddUseInStep(StepManager.Ins.CurrentStep);
        itemBodyPainter.onDragStart.AddListener(OnBodyPainterDragStart);
        itemBodyPainter.onDragStop.AddListener(OnBodyPainterDragStop);
        bodyPainterTrigger.onEnterZone.AddListener(OnBodyPainterEnterZone);
        bodyPainterTrigger.onOutZone.AddListener(OnBodyPainterExitZone);
        bodyPainterTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(bodyPainterSlots);
    }

    private void OnBodyPainterDragStart()
    {
        _isBodyPainterDragging = true;
        bodyPainterTrigger.enabled = true;
    }

    private void OnBodyPainterDragStop()
    {
        _isBodyPainterDragging = false;
        bodyPainterTrigger.enabled = false;
        _tweenBodyPainter?.Pause();
        fillCircleBar.Hide();
    }

    private void OnBodyPainterEnterZone()
    {
        fillCircleBar.Show();
        if (_tweenBodyPainter == null)
        {
            _tweenBodyPainter = DOVirtual
                .Float(_progressBodyPainter, 1f, bodyPainterDuration * (1f - _progressBodyPainter), value =>
                {
                    _progressBodyPainter = value;
                    SetSlotsAlpha(bodyPainterSlots, value);
                    fillCircleBar.Fill(_progressBodyPainter);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenBodyPainter = null;
                    OnBodyPainterCompleted();
                });
        }
        else if (!_tweenBodyPainter.IsPlaying())
        {
            _tweenBodyPainter.Play();
        }
    }

    private void OnBodyPainterExitZone()
    {
        if (!_isBodyPainterDragging) return;
        _tweenBodyPainter?.Pause();
        fillCircleBar.Hide();
    }

    private void OnBodyPainterCompleted()
    {
        _progressBodyPainter = 1f;
        bodyPainterTrigger.enabled = false;
        SetSlotsAlpha(bodyPainterSlots, 1f);
        fillCircleBar.Hide();
        bodyPainterTrigger.onEnterZone.RemoveListener(OnBodyPainterEnterZone);
        bodyPainterTrigger.onOutZone.RemoveListener(OnBodyPainterExitZone);
        itemBodyPainter.onDragStart.RemoveListener(OnBodyPainterDragStart);
        itemBodyPainter.onDragStop.RemoveListener(OnBodyPainterDragStop);
        DoneStep();
    }
    #endregion
    #region Step4
    private void OnStartStep4()
    {
        StartStep4();
    }

    private void StartStep4()
    {
        StartCoroutine(IE_DelayStep4());
    }

    [SerializeField] private SlotAttachmentPairList hairBackSlots;
    public void OnSetStateSlotHairBack(bool value)
    {
        hairBackSlots.TurnSlotState(value);
    }
    [SerializeField] private SonSnapObject snapObjectHairBack;
    [SerializeField] private List<SonSnapPoint> listSnapPointtHairBack;

    [SerializeField] private float durationStep4 = 0.5f;
    [SerializeField] private float detalYStep4 = 1f;
    IEnumerator IE_DelayStep4()
    {
        yield return new WaitForSeconds(0.5f);
        character.gameObject.transform.DOMoveY(character.gameObject.transform.position.y - detalYStep4, durationStep4).SetEase(Ease.Linear);
        character.gameObject.transform.DOScale(1f, durationStep4).SetEase(Ease.Linear);
        foreach (var snapPoint in listSnapPointtHairBack)
        {
            snapPoint.ChangeCanSnap(true);
        }
        snapObjectHairBack.OnSnap.AddListener(() =>
        {
            DoneStep();
            TryNextStep();
        });
    }
    #endregion
    #region Step5
    [SerializeField] private SonDragItemBase itemHairSpray;
    [SerializeField] private OnTransformGoToAffectZone hairSprayTrigger;
    [SerializeField] private float hairSprayDuration = 3f;
    [SerializeField] private SlotAttachmentPairList hairSpraySlots;
    [SerializeField] private SlotAttachmentPairList hairOldSpraySlots;

    private Tween _tweenHairSpray;
    private float _progressHairSpray = 0f;
    private bool _isHairSprayDragging = false;

    private void OnStartStep5()
    {
        StartStep5();
    }
    private void StartStep5()
    {
        itemHairSpray.AddUseInStep(StepManager.Ins.CurrentStep);
        itemHairSpray.onDragStart.AddListener(OnHairSprayDragStart);
        itemHairSpray.onDragStop.AddListener(OnHairSprayDragStop);
        hairSprayTrigger.onEnterZone.AddListener(OnHairSprayEnterZone);
        hairSprayTrigger.onOutZone.AddListener(OnHairSprayExitZone);
        hairSprayTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(hairSpraySlots);
    }

    private void OnHairSprayDragStart()
    {
        _isHairSprayDragging = true;
        hairSprayTrigger.enabled = true;
    }

    private void OnHairSprayDragStop()
    {
        _isHairSprayDragging = false;
        hairSprayTrigger.enabled = false;
        _tweenHairSpray?.Pause();
        fillCircleBar.Hide();
    }
    private bool isShowHairSpray = false;
    private void OnHairSprayEnterZone()
    {
        fillCircleBar.Show();
        if (_tweenHairSpray == null)
        {
            _tweenHairSpray = DOVirtual
                .Float(_progressHairSpray, 1f, hairSprayDuration * (1f - _progressHairSpray), value =>
                {
                    _progressHairSpray = value;
                    SetSlotsAlpha(hairSpraySlots, value);
                    fillCircleBar.Fill(_progressHairSpray);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenHairSpray = null;
                    OnHairSprayCompleted();
                });
        }
        else if (!_tweenHairSpray.IsPlaying())
        {
            _tweenHairSpray.Play();
        }
    }

    private void OnHairSprayExitZone()
    {
        if (!_isHairSprayDragging) return;
        _tweenHairSpray?.Pause();
        fillCircleBar.Hide();
    }

    private void OnHairSprayCompleted()
    {
        hairOldSpraySlots.TurnSlotState(false);
        _progressHairSpray = 1f;
        hairSprayTrigger.enabled = false;
        SetSlotsAlpha(hairSpraySlots, 1f);
        fillCircleBar.Hide();
        hairSprayTrigger.onEnterZone.RemoveListener(OnHairSprayEnterZone);
        hairSprayTrigger.onOutZone.RemoveListener(OnHairSprayExitZone);
        itemHairSpray.onDragStart.RemoveListener(OnHairSprayDragStart);
        itemHairSpray.onDragStop.RemoveListener(OnHairSprayDragStop);
        DoneStep();
    }
    #endregion
    #region Step7
    [SerializeField] private SonDragItemBase itemHairGel;
    [SerializeField] private OnTransformGoToAffectZone hairGelTrigger;
    [SerializeField] private float hairGelDuration = 3f;
    [SerializeField] private SlotAttachmentPairList hairGelSlots;
    [SerializeField] private SlotAttachmentPairList hairGelOutSlots;

    private Tween _tweenHairGel;
    private float _progressHairGel = 0f;
    private bool _isHairGelDragging = false;

    private void OnStartStep6()
    {
        StartStep6();
    }
    private void StartStep6()
    {
        itemHairGel.AddUseInStep(StepManager.Ins.CurrentStep);
        itemHairGel.onDragStart.AddListener(OnHairGelDragStart);
        itemHairGel.onDragStop.AddListener(OnHairGelDragStop);
        hairGelTrigger.onEnterZone.AddListener(OnHairGelEnterZone);
        hairGelTrigger.onOutZone.AddListener(OnHairGelExitZone);
        hairGelTrigger.enabled = false;
        fillCircleBar.ReFill();
        InitSlots(hairGelSlots);
    }

    private void OnHairGelDragStart()
    {
        _isHairGelDragging = true;
        SoundManager.Ins.PlaySoundSpray();
        hairGelTrigger.enabled = true;
    }

    private void OnHairGelDragStop()
    {
        SoundManager.Ins.StopSoundSpray();
        _isHairGelDragging = false;
        hairGelTrigger.enabled = false;
        _tweenHairGel?.Pause();
        fillCircleBar.Hide();
    }

    private void OnHairGelEnterZone()
    {
        fillCircleBar.Show();
        if (_tweenHairGel == null)
        {
            _tweenHairGel = DOVirtual
                .Float(_progressHairGel, 1f, hairGelDuration * (1f - _progressHairGel), value =>
                {
                    _progressHairGel = value;
                    SetSlotsAlpha(hairGelSlots, value);
                    SetSlotsAlpha(hairGelOutSlots, 1 - value);

                    fillCircleBar.Fill(_progressHairGel);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tweenHairGel = null;
                    OnHairGelCompleted();
                });
        }
        else if (!_tweenHairGel.IsPlaying())
        {
            _tweenHairGel.Play();
        }
    }

    private void OnHairGelExitZone()
    {
        if (!_isHairGelDragging) return;
        _tweenHairGel?.Pause();
        fillCircleBar.Hide();
    }

    private void OnHairGelCompleted()
    {
        _progressHairGel = 1f;
        hairGelTrigger.enabled = false;
        SetSlotsAlpha(hairGelSlots, 1f);
        fillCircleBar.Hide();
        hairGelTrigger.onEnterZone.RemoveListener(OnHairGelEnterZone);
        hairGelTrigger.onOutZone.RemoveListener(OnHairGelExitZone);
        itemHairGel.onDragStart.RemoveListener(OnHairGelDragStart);
        itemHairGel.onDragStop.RemoveListener(OnHairGelDragStop);
        DoneStep();
    }
    #endregion
    #region Step7
    private void OnStartStep7()
    {
        TutorialManager.Ins.enableCountTime = false;

        StartCoroutine(IE_DelayStep7());
    }
    [SerializeField] private ShowObjectEffect effectStep3;
    [SerializeField] private ShowObjectEffect effectStep4;

    IEnumerator IE_DelayStep7()
    {
        yield return new WaitForSeconds(0.5f);
        effectStep3.Hide(0.5f);
        effectStep4.Show(1.25f);
        yield return new WaitForSeconds(1.25f);
        EndGame();

    }
    #endregion
}
