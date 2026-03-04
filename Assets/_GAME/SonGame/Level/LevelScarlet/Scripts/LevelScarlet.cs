using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using sonnv;
using System.Collections;
using HoangHH;
using Satisgame;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class LevelScarlet : Singleton<LevelScarlet>
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
        SoundManager.Ins.PlayFx(FxType.StartGame);
        SoundManager.Ins.PlayBgm();
        transitionPhase.TransitionToPhase(0, 1);
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
        //TutorialManager.Ins.enableCountTime = true;
        isDoneStep = false;
        switch (StepManager.Ins.CurrentStep)
        {
            case 0:
                //OnStartStep1();
                return;
            case 1:
                OnStartStep2();
                return;
            case 2:
                OnStartStep3();
                return;
            case 3:
                OnStartStep4();
                return;
            case 4:
                StartStep2();
                return;
            case 5:
                OnStartStep5();
                return;
            case 6:
                OnStartStep6();
                return;
            case 7:
                return;
            default:
                return;
        }
    }
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
    [SerializeField] private List<SonThrowObject> listThrowObject;
    private void OnStartStep1()
    {
        SetNewEmoji(emojiControlStep1);
        PlayPositiveEmoji();
        MoveCamera(cam, targetOrthoSize, 1, 1f, () =>
        {
            TutorialManager.Ins.enableCountTime = true;
        });

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
                    DoneStep();
                    TryNextStep();
                }
            });
        }
    }
    [SerializeField] private SlotAttachmentPairList slotClother;
    [SerializeField] private SlotAttachmentPairList slotDress;
    [SerializeField] private SlotAttachmentPairList slotShirt;
    [SerializeField] private SlotAttachmentPairList slotShoeL;
    [SerializeField] private SlotAttachmentPairList slotShoeR;
    public void OnSetStateSlotClother(bool value)
    {
        slotClother.TurnSlotState(value);
    }
    public void OnSetStateSlotDress(bool value)
    {
        slotDress.TurnSlotState(value);
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
    [SerializeField] private float targetOrthoSizeStepMakeUp = 8f;
    [SerializeField] private float targetLocalYStepMakeUp = 5f;
    [SerializeField] private SonEffectShowObject effectShowObject;
    [SerializeField] private EmojiControl emojiControlStep1;
    private void OnStartStep2()
    {
        MoveCamera(cam, targetOrthoSizeStepMakeUp, targetLocalYStepMakeUp, 0.75f, () =>
        {
            effectShowObject.Show();
            StartStep2();
        });
    }
    [SerializeField] private SonDragItemBase itemShowerDrag;
    [SerializeField] private OnTransformGoToAffectZone showerTrigger;
    [SerializeField] private float showerTime = 3f;
    [SerializeField] private ParticleSystem showerParticle;
    private void StartStep2()
    {
        itemShowerDrag.AddUseInStep(StepManager.Ins.CurrentStep);
        Debug.Log("Step2");
        itemShowerDrag.onDragStart.AddListener(EnableShowerTrigger);
        itemShowerDrag.onDragStop.AddListener(DisableShowerTrigger);
        showerTrigger.onEnterZone.AddListener(TryShower);
        showerTrigger.onOutZone.AddListener(TryPausShower);
        fillCircleBar.ReFill();
    }
    private void EnableShowerTrigger()
    {
        showerTrigger.enabled = true;
    }
    private void DisableShowerTrigger()
    {
        showerTrigger.enabled = false;
    }
    private Tween tweenShower;
    private void TryShower()
    {
        fillCircleBar.Show();
        if (tweenShower == null)
        {
            tweenShower = DOVirtual.Float(0, 1, showerTime,
                    value =>
                    {
                        fillCircleBar.Fill(value);
                    }).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    tweenShower = null;
                    TryEndShower();
                });
        }
        else if (!tweenShower.IsPlaying())
        {
            tweenShower.Play();
        }
    }
    private void TryPausShower()
    {
        fillCircleBar.Hide();

        if (tweenShower != null && tweenShower.IsPlaying())
        {
            tweenShower.Pause();
        }
    }
    [SerializeField] private SlotAttachmentPairList hairMakeF;
    private void TryEndShower()
    {
        hairMakeF.TurnSlotState(true);
        fillCircleBar.Hide();
        DisableShowerTrigger();
        itemShowerDrag.onDragStart.RemoveListener(EnableShowerTrigger);
        itemShowerDrag.onDragStop.RemoveListener(DisableShowerTrigger);
        showerTrigger.onEnterZone.RemoveListener(TryShower);
        showerTrigger.onOutZone.RemoveListener(TryPausShower);
        DoneStep();
    }
    [SerializeField] private GameObject objShampoo;
    [SerializeField] private List<TriggerWithCertainCollider> listTriggerCleanser;
    [SerializeField] private SonDragItemBase itemBoxCleanser;
    [SerializeField] private AudioData vfxCleanser;
    private int countTrigger = 0;
    private void OnStartStep3()
    {
        objShampoo.SetActive(true);
        itemBoxCleanser.AddUseInStep(StepManager.Ins.CurrentStep);
        for (int i = 0; i < listTriggerCleanser.Count; i++)
        {
            TriggerWithCertainCollider cleanser = listTriggerCleanser[i];
            cleanser.EnableCol(true);
            cleanser.AddTriggerEvent(() =>
            {
                SoundManager.PlaySFX(vfxCleanser.clip, 1);
                countTrigger++;
                if (countTrigger >= listTriggerCleanser.Count)
                {
                    DoneStep();
                    //TryNextStep();
                }
            });
        }
    }
    [SerializeField] private SonDragItemBase itemBrushDrag;
    [SerializeField] private OnTransformGoToAffectZone brushTrigger;
    [SerializeField] private float brushTime = 3f;
    private void OnStartStep4()
    {
        itemBrushDrag.AddUseInStep(StepManager.Ins.CurrentStep);
        itemBrushDrag.onDragStart.AddListener(EnableBrushTrigger);
        itemBrushDrag.onDragStop.AddListener(DisableBrushTrigger);
        brushTrigger.onEnterZone.AddListener(TryBrush);
        brushTrigger.onOutZone.AddListener(TryPausBrush);
        fillCircleBar.ReFill();
    }
    private void EnableBrushTrigger()
    {
        brushTrigger.enabled = true;
    }
    private void DisableBrushTrigger()
    {
        brushTrigger.enabled = false;
    }
    private Tween tweenBrush;
    [SerializeField] private List<SpriteRenderer> listSpriteShampoo;
    private void ChangeAlphaCleanser(List<SpriteRenderer> _listSprite, float value)
    {
        value = Mathf.Clamp01(value);

        foreach (SpriteRenderer sprite in _listSprite)
        {
            if (sprite == null) continue;

            Color c = sprite.color;
            c.a = value;
            sprite.color = c;
        }
    }
    private void TryBrush()
    {
        fillCircleBar.Show();
        if (tweenBrush == null)
        {
            tweenBrush = DOVirtual.Float(0, 1, showerTime,
                    value =>
                    {
                        ChangeAlphaCleanser(listSpriteShampoo, 1 - value);
                        fillCircleBar.Fill(value);
                    }).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    tweenBrush = null;
                    TryEndBrush();
                });
        }
        else if (!tweenBrush.IsPlaying())
        {
            tweenBrush.Play();
        }
    }
    private void TryPausBrush()
    {
        fillCircleBar.Hide();

        if (tweenBrush != null && tweenBrush.IsPlaying())
        {
            tweenBrush.Pause();
        }
    }
    [SerializeField] private SlotAttachmentPairList hairMakeCream;

    private void TryEndBrush()
    {
        fillCircleBar.Hide();
        hairMakeCream.TurnSlotState(true);
        DisableBrushTrigger();
        itemBrushDrag.onDragStart.RemoveListener(EnableBrushTrigger);
        itemBrushDrag.onDragStop.RemoveListener(DisableBrushTrigger);
        showerTrigger.onEnterZone.RemoveListener(TryBrush);
        showerTrigger.onOutZone.RemoveListener(TryPausBrush);
        DoneStep();
    }

    //=====================


    [SerializeField] private SonDragItemBase itemTowelDrag;
    [SerializeField] private OnTransformGoToAffectZone TowelTrigger;
    [SerializeField] private float TowelTime = 3f;
    private void OnStartStep5()
    {
        itemTowelDrag.AddUseInStep(StepManager.Ins.CurrentStep);
        itemTowelDrag.onDragStart.AddListener(EnableTowelTrigger);
        itemTowelDrag.onDragStop.AddListener(DisableTowelTrigger);
        TowelTrigger.onEnterZone.AddListener(TryTowel);
        TowelTrigger.onOutZone.AddListener(TryPauseTowel);
        fillCircleBar.ReFill();
    }
    private void EnableTowelTrigger()
    {
        TowelTrigger.enabled = true;
    }
    private void DisableTowelTrigger()
    {
        TowelTrigger.enabled = false;
    }
    private Tween tweenTowel;
    [SerializeField] private SlotAttachmentPairList hairMakeTowel;
    private bool isShowTowel = false;
    private void TryTowel()
    {
        fillCircleBar.Show();
        if (tweenTowel == null)
        {
            tweenTowel = DOVirtual.Float(0, 1, TowelTime,
                    value =>
                    {
                        if (value >= 0.5f && !isShowTowel)
                        {
                            isShowTowel = true;
                            Debug.Log("Towel");
                            hairMakeTowel.TurnSlotState(true);
                        }
                        fillCircleBar.Fill(value);
                    }).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    tweenTowel = null;
                    TryEndTowel();
                });
        }
        else if (!tweenTowel.IsPlaying())
        {
            tweenTowel.Play();
        }
    }
    private void TryPauseTowel()
    {
        fillCircleBar.Hide();

        if (tweenTowel != null && tweenTowel.IsPlaying())
        {
            tweenTowel.Pause();
        }
    }

    private void TryEndTowel()
    {
        fillCircleBar.Hide();
        DisableTowelTrigger();
        itemTowelDrag.onDragStart.RemoveListener(EnableTowelTrigger);
        itemTowelDrag.onDragStop.RemoveListener(DisableTowelTrigger);
        TowelTrigger.onEnterZone.RemoveListener(TryTowel);
        TowelTrigger.onOutZone.RemoveListener(TryPauseTowel);
        DoneStep();
        AdsManager.Ins.ShowEndGame();
    }
    [SerializeField] private SonDragItemBase itemDryerDrag;
    private void OnStartStep6()
    {
        itemDryerDrag.AddUseInStep(StepManager.Ins.CurrentStep);
    }
}