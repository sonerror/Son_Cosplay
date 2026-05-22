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
        if(targetOrthoSize < 3.55f)
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
               // OnStartStep2();
                return;
            case 2:
                TutorialManager.Ins.SetNewTime(3f);
               // OnStartStep3();
                return;
            case 3:
              //  OnStartStep4();
                return;
            case 4:
              //  OnStartStep5();
                return;
            case 5:
                // OnStartStep6();
                return;
            case 6:
                // OnStartStep7();
                return;
            case 7:
                // OnStartStep8();
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
        emojiControl.ShowPositive();
        isDoneStep = true;
        SoundManager.Ins.PlayFx(FxType.Pick);
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
        DoneStep();
        TryNextStep();
    }
    #endregion
    //#region Step 2
    //[SerializeField] private List<SonThrowObject> sonThrowObjects;
    //[SerializeField] private List<SonThrowObject> sonThrowObjectsStart;
    //private int countThrow = 0;
    //private void OnStartStep2()
    //{
    //    TutorialManager.Ins.enableCountTime = true;

    //    foreach (SonThrowObject throwObj in sonThrowObjectsStart)
    //    {
    //        throwObj.enabled = true;
    //        throwObj.Col.enabled = true;
    //    }
    //    for (int i = 0; i < sonThrowObjects.Count; i++)
    //    {
    //        SonThrowObject obj = sonThrowObjects[i];
    //        obj.onRemoveItem.AddListener(() =>
    //        {
    //            int removedIndex = sonThrowObjects.IndexOf(obj);
    //            if (removedIndex < 0) return;
    //            sonThrowObjects.RemoveAt(removedIndex);
    //            TutorialManager.Ins.ListClothes.RemoveAt(removedIndex);
    //            TutorialManager.Ins.ListThrowClothes.RemoveAt(removedIndex);
    //            countThrow++;
    //            if (countThrow == 1)
    //            {
    //                TutorialManager.Ins.SetNewTime(3f);

    //            }
    //            if (sonThrowObjects.Count == 0)
    //            {
    //                DoneStep();
    //                TryNextStep();
    //            }
    //        });
    //    }
    //}
    //[SerializeField] private SlotAttachmentPairList slotHat;
    //public void SetStateSlotHat(bool value)
    //{
    //    slotHat.TurnSlotState(value);
    //}
    //[SerializeField] private SlotAttachmentPairList slotCoat;
    //public void SetStateSlotCoat(bool value)
    //{
    //    slotCoat.TurnSlotState(value);
    //}
    //[SerializeField] private SlotAttachmentPairList slotPant;
    //public void SetStateSlotPant(bool value)
    //{
    //    slotPant.TurnSlotState(value);
    //}

    //[SerializeField] private SlotAttachmentPairList slotShoeL;
    //public void SetStateSlotShoeL(bool value)
    //{
    //    slotShoeL.TurnSlotState(value);
    //}
    //[SerializeField] private SlotAttachmentPairList slotShoeR;
    //public void SetStateSlotShoeR(bool value)
    //{
    //    slotShoeR.TurnSlotState(value);
    //}
    //[SerializeField] private SlotAttachmentPairList slotShirt;
    //public void SetStateSlotShirt(bool value)
    //{
    //    slotShirt.TurnSlotState(value);
    //}
    //#endregion
    //#region Step3
    //[SerializeField] private ShowObjectEffect showStep1Phase1;

    //private void OnStartStep3()
    //{
    //    showStep1Phase1.Show(0.5f);
    //    showStep1Phase1.onShowComplete.AddListener(() =>
    //    {
    //        StartStep3();
    //    });
    //}

    //[SerializeField] private SonDragItemBase itemBodyPainter;
    //[SerializeField] private OnTransformGoToAffectZone bodyPainterTrigger;
    //[SerializeField] private float bodyPainterDuration = 3f;
    //[SerializeField] private SlotAttachmentPairList bodyPainterSlots;
    //[SerializeField] private SpineAttachmentLocker lipStickSlotsBlock;
    //private Tween _tweenBodyPainter;
    //private float _progressBodyPainter = 0f;
    //private bool _isBodyPainterDragging = false;
    //private void StartStep3()
    //{
    //    lipStickSlotsBlock.enabled = false;
    //    itemBodyPainter.AddUseInStep(StepManager.Ins.CurrentStep);
    //    itemBodyPainter.onDragStart.AddListener(OnBodyPainterDragStart);
    //    itemBodyPainter.onDragStop.AddListener(OnBodyPainterDragStop);
    //    bodyPainterTrigger.onEnterZone.AddListener(OnBodyPainterEnterZone);
    //    bodyPainterTrigger.onOutZone.AddListener(OnBodyPainterExitZone);
    //    bodyPainterTrigger.enabled = false;
    //    fillCircleBar.ReFill();
    //    InitSlots(bodyPainterSlots);
    //}

    //private void OnBodyPainterDragStart()
    //{
    //    _isBodyPainterDragging = true;
    //    bodyPainterTrigger.enabled = true;
    //}

    //private void OnBodyPainterDragStop()
    //{
    //    _isBodyPainterDragging = false;
    //    bodyPainterTrigger.enabled = false;
    //    _tweenBodyPainter?.Pause();
    //    fillCircleBar.Hide();
    //}
    //private bool isShowFlip = false;
    //private void OnBodyPainterEnterZone()
    //{
    //    fillCircleBar.Show();
    //    if (_tweenBodyPainter == null)
    //    {
    //        _tweenBodyPainter = DOVirtual
    //            .Float(_progressBodyPainter, 1f, bodyPainterDuration * (1f - _progressBodyPainter), value =>
    //            {
    //                _progressBodyPainter = value;
    //                SetSlotsAlpha(bodyPainterSlots, value);
    //                if (value >= 0.85f && isShowFlip == false)
    //                {
    //                    lipStickSlotsBlock.enabled = false;
    //                    isShowFlip = true;
    //                }
    //                fillCircleBar.Fill(_progressBodyPainter);
    //            })
    //            .SetEase(Ease.Linear)
    //            .OnComplete(() =>
    //            {
    //                _tweenBodyPainter = null;
    //                OnBodyPainterCompleted();
    //            });
    //    }
    //    else if (!_tweenBodyPainter.IsPlaying())
    //    {
    //        _tweenBodyPainter.Play();
    //    }
    //}

    //private void OnBodyPainterExitZone()
    //{
    //    if (!_isBodyPainterDragging) return;
    //    _tweenBodyPainter?.Pause();
    //    fillCircleBar.Hide();
    //}

    //private void OnBodyPainterCompleted()
    //{
    //    _progressBodyPainter = 1f;
    //    bodyPainterTrigger.enabled = false;
    //    SetSlotsAlpha(bodyPainterSlots, 1f);
    //    fillCircleBar.Hide();
    //    bodyPainterTrigger.onEnterZone.RemoveListener(OnBodyPainterEnterZone);
    //    bodyPainterTrigger.onOutZone.RemoveListener(OnBodyPainterExitZone);
    //    itemBodyPainter.onDragStart.RemoveListener(OnBodyPainterDragStart);
    //    itemBodyPainter.onDragStop.RemoveListener(OnBodyPainterDragStop);
    //    DoneStep();
    //    TutorialManager.Ins.enableCountTime = false;

    //}
    //#endregion
    //#region Step4
    //[SerializeField] private EmojiControl emojiControlNew;
    //[SerializeField] private ShowObjectEffect showStep2Phase1;
    //[SerializeField] private SpineBoneIKControl spineBoneIKControl;
    //[SerializeField] private LoopAnimTransformFloating loopAnimTransformFloating;

    //private void OnStartStep4()
    //{
    //    SetNewEmoji(emojiControlNew);
    //    StartStep4();
    //}

    //[SerializeField] private float targetLocalYStep3 = 1f;
    //[SerializeField] private float targetOrthoSizeStep3 = 4f;
    //[SerializeField] private float timerMoveStep3 = 0.75f;
    //[SerializeField] private SpriteAlphaGroup spriteAlphaGroupStep2;
    //[SerializeField] private SpriteAlphaGroup spriteAlphaGroupStep3;
    //[SerializeField] private CameraScaler cameraScaler;
    //private int countEye = 0;
    //private bool isShowHintEye = false;
    //IEnumerator IE_DelayStep4()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    //spineBoneIKControl.enabled = false;
    //    // loopAnimTransformFloating.enabled = false;
    //    spriteAlphaGroupStep2.FadeOut();
    //    MoveCamera(cam, cam.orthographicSize + targetOrthoSizeStep3, -targetLocalYStep3, timerMoveStep3, () =>
    //    {
    //        showStep2Phase1.Show(0.25f);
    //        showStep2Phase1.onShowComplete.AddListener(() =>
    //        {
    //            foreach (SonSnapPoint point in listSnapPointEye)
    //            {
    //                point.ChangeCanSnap(true);
    //            }
    //            for (int i = 0; i < listSnapObjectEye.Count; i++)
    //            {
    //                SonSnapObject obj = listSnapObjectEye[i];
    //                obj.OnSnap.AddListener(() =>
    //                {
    //                    int removedIndex = listSnapObjectEye.IndexOf(obj);
    //                    if (removedIndex < 0) return;
    //                    listSnapObjectEye.RemoveAt(removedIndex);
    //                    TutorialManager.Ins.ListObjEye.RemoveAt(removedIndex);
    //                    TutorialManager.Ins.ListPointEye.RemoveAt(removedIndex);
    //                    countEye++;
    //                    if (countEye == 1 && isShowHintEye == false)
    //                    {
    //                        TutorialManager.Ins.SetNewTime(3f);
    //                        isShowHintEye = true;
    //                    }
    //                    if (listSnapObjectEye.Count == 0)
    //                    {
    //                        DoneStep();
    //                        TryNextStep();
    //                    }
    //                });
    //            }
    //            TutorialManager.Ins.SetNewTime(0.5f);
    //            TutorialManager.Ins.enableCountTime = true;
    //        });
    //    });
    //}
    //[SerializeField] private List<SonSnapPoint> listSnapPointEye;
    //[SerializeField] private List<SonSnapObject> listSnapObjectEye;
    //[SerializeField] private SlotAttachmentPairList slotEyeL;
    //public void SetStateSlotEyeL()
    //{
    //    slotEyeL.TurnSlotState(true);
    //}
    //[SerializeField] private SlotAttachmentPairList slotEyeR;
    //public void SetStateSlotEyeR()
    //{
    //    slotEyeR.TurnSlotState(true);
    //}

    //private void StartStep4()
    //{
    //    showStep1Phase1.Hide(0.5f);
    //    showStep1Phase1.onHide.AddListener(() =>
    //    {
    //        StartCoroutine(IE_DelayStep4());
    //    });
    //}

    //#endregion
    //#region Step5
    //[SerializeField] private SonDragItemBase dragFoundation;

    //[SerializeField] private List<TriggerWithCertainCollider> listTrigger;
    //private int countTrigger = 0;
    //private void OnStartStep5()
    //{
    //    dragFoundation.AddUseInStep(StepManager.Ins.CurrentStep);
    //    for (int i = 0; i < listTrigger.Count; i++)
    //    {
    //        TriggerWithCertainCollider trigger = listTrigger[i];
    //        trigger.enabled = true;
    //        trigger.Col.enabled = true;
    //        trigger.OnTriggerEvent.AddListener(() =>
    //        {
    //            countTrigger++;
    //            if (countTrigger >= listTrigger.Count)
    //            {
    //                DoneStep();
    //                EndGame();
    //            }
    //        });
    //    }
    //}
    //#endregion
}
