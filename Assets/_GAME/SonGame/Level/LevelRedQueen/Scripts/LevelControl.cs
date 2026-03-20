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
#if UNITY_EDITOR

      [Sirenix.OdinInspector.Button]
       public void TurnOffSlotMouth()
        {
        }

#endif
    public void TurnOffSlotMouthDone()
    {
    }
    [SerializeField] private SlotAttachmentPairList slotDirtStart;
    [SerializeField] private SlotAttachmentPairList slotMouthStart;

    protected virtual void Start()
    {
        slotDirtStart.TurnSlotState(true);
        slotMouthStart.TurnSlotState(false);
        StartStep();
        Debug.Log("DOTween Version: " + DOTween.Version);
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
                OnStartStep8();
                return;
            case 8:
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
    #region Step1
    private void OnStartStep1()
    {
        DoneStep();
        TryNextStep();
    }
    #endregion
    #region Step2
    [SerializeField] private List<SonThrowObject> listThrowObject;
    [SerializeField] private SlotAttachmentPairList slotOffHeadband;
    public void SetStateSlotHeadband(bool value)
    {
        slotOffHeadband.TurnSlotState(value);
    }
    private int countThrowObj = 0;
    private void OnStartStep2()
    {
        TurnOffSlotMouthDone();

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
                // TutorialManager.Ins.TutorialNode.RemoveAt(removedIndex);
                //  TutorialManager.Ins.TfItem.RemoveAt(removedIndex);
                if (listThrowObject.Count == 0)
                {
                    DoneStep();
                    TryNextStep();
                    TutorialManager.Ins.SetNewTime(4f);
                }
            });
        }
    }
    #endregion
    #region Step3
    [SerializeField] private SonSnapObject wigCapSnapObj;
    [SerializeField] private SonSnapPoint wigCapSnapPoint;
    private void OnStartStep3()
    {
        Debug.Log("Step3");
        wigCapSnapPoint.ChangeCanSnap(true);
        wigCapSnapObj.OnSnap.AddListener(() =>
        {
            SetStateSlotHeadBand();
            DoneStep();
            TryNextStep();
        });
    }
    [SerializeField] private SlotAttachmentPairList slotWigCap;
    [SerializeField] private SlotAttachmentPairList slotHair;

    public void SetStateSlotHeadBand()
    {
        slotWigCap.TurnSlotState(true);
        slotHair.TurnSlotState(false);
    }
    #endregion
    #region Step4
    [SerializeField] private SonSnapObject hairSnapObj;
    [SerializeField] private SonSnapPoint hairSnapPoint;
    private void OnStartStep4()
    {
        Debug.Log("Step4");

        hairSnapPoint.ChangeCanSnap(true);
        hairSnapObj.OnSnap.AddListener(() =>
        {
            SetStateSlotHair();
            DoneStep();
            TryNextStep();
        });
    }
    [SerializeField] private SlotAttachmentPairList slotHairNew;

    public void SetStateSlotHair()
    {
        slotWigCap.TurnSlotState(false);
        slotHairNew.TurnSlotState(true);
    }
    #endregion
    #region Step5
    [SerializeField] private ShowObjectEffect step1;
    [SerializeField] private ShowObjectEffect step2;
    private void OnStartStep5()
    {
        step1.Hide();
        step2.Show(0.75f);
        step2.onShowComplete.AddListener(() =>
        {
            OnStartStep();
        });
    }
    [SerializeField] private List<TriggerWithCertainCollider> listTriggerCleanser;
    [SerializeField] private SonDragItemBase itemBoxCleanser;
    [SerializeField] private AudioData vfxCleanser;
    private int countTrigger = 0;
    private void OnStartStep()
    {
        Debug.Log("Step5");
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
    #endregion
    #region Step6
    [SerializeField] private SonDragItemBase itemWashMachineDrag;
    [SerializeField] private OnTransformGoToAffectZone washTrigger;
    [SerializeField] private float washTime = 3;
    [SerializeField] private List<SpriteRenderer> listSpriteCleanser;
    [SerializeField] private SpriteRenderer spriteFoamCleanser;

    private void ChangeAlphaCleanser(float value)
    {
        value = Mathf.Clamp01(value);

        foreach (SpriteRenderer sprite in listSpriteCleanser)
        {
            if (sprite == null) continue;

            Color c = sprite.color;
            c.a = value;
            sprite.color = c;
        }
    }

    private void ChangeAlphaFoam(float value)
    {
        if (spriteFoamCleanser == null) return;

        value = Mathf.Clamp01(value);

        Color c = spriteFoamCleanser.color;
        c.a = value;
        spriteFoamCleanser.color = c;
    }

    private void OnStartStep6()
    {
        itemWashMachineDrag.AddUseInStep(StepManager.Ins.CurrentStep);
        itemWashMachineDrag.onDragStart.AddListener(EnableWashTrigger);
        itemWashMachineDrag.onDragStop.AddListener(DisableWashTrigger);
        washTrigger.onEnterZone.AddListener(TryWash);
        washTrigger.onOutZone.AddListener(TryPausWash);
        fillCircleBar.ReFill();
    }

    private void EnableWashTrigger()
    {
        washTrigger.enabled = true;
    }

    private void DisableWashTrigger()
    {
        washTrigger.enabled = false;
    }

    private Tween tweenWash;


    private void TryWash()
    {
        fillCircleBar.Show();
        if (tweenWash == null)
        {
            tweenWash = DOVirtual.Float(0, 1, washTime,
                    value =>
                    {

                        fillCircleBar.Fill(value);
                        ChangeAlphaCleanser(1 - value);
                        ChangeAlphaFoam(value);
                        SetStateChangeAnim(false);
                    }).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    tweenWash = null;
                    slotDirtStart.TurnSlotState(false);

                    TryEndWash();
                });
        }
        else if (!tweenWash.IsPlaying())
        {
            tweenWash.Play();
        }
    }

    private void TryPausWash()
    {
        fillCircleBar.Hide();

        if (tweenWash != null && tweenWash.IsPlaying())
        {
            tweenWash.Pause();
        }
    }

    private void TryEndWash()
    {
        fillCircleBar.Hide();
        ChangeAlphaCleanser(0);
        DisableWashTrigger();
        itemWashMachineDrag.onDragStart.RemoveListener(EnableWashTrigger);
        itemWashMachineDrag.onDragStop.RemoveListener(DisableWashTrigger);
        washTrigger.onEnterZone.RemoveListener(TryWash);
        washTrigger.onOutZone.RemoveListener(TryPausWash);
        DoneStep();
        //TryNextStep();
    }
    #endregion

    #region Step4

    [SerializeField] private SonDragItemBase itemShowerDrag;
    [SerializeField] private OnTransformGoToAffectZone showerTrigger;
    [SerializeField] private float showerTime;
    private void OnStartStepShower()
    {
        itemShowerDrag.AddUseInStep(StepManager.Ins.CurrentStep);
        itemShowerDrag.onDragStart.AddListener(EnableShowerTrigger);
        itemShowerDrag.onDragStop.AddListener(DisableShowerTrigger);
        showerTrigger.onEnterZone.AddListener(TryShower);
        showerTrigger.onOutZone.AddListener(TryPausShower);
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
            // showerParticle.Play();
            tweenShower = DOVirtual.Float(0, 1, showerTime,
                    value =>
                    {
                        //SetEmissionRate(showerParticle, (value * 10f));
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

    private void TryEndShower()
    {
        fillCircleBar.Hide();
        // SetEmissionRate(showerParticle, 10);
        // showerParticle.Play();
        DisableShowerTrigger();
        itemShowerDrag.onDragStart.RemoveListener(EnableShowerTrigger);
        itemShowerDrag.onDragStop.RemoveListener(DisableShowerTrigger);
        showerTrigger.onEnterZone.RemoveListener(TryShower);
        showerTrigger.onOutZone.RemoveListener(TryPausShower);
        DoneStep();
        //TryNextStep();
    }
    #endregion

    #region Step7
    private void OnStartStep7()
    {
        SetStateChangeAnim(true);
        itemShowerDrag.AddUseInStep(StepManager.Ins.CurrentStep);
        itemShowerDrag.onDragStart.AddListener(EnableShowerTrigger);
        itemShowerDrag.onDragStop.AddListener(DisableShowerTrigger);
        showerTrigger.onEnterZone.AddListener(TryShowerStep2);
        showerTrigger.onOutZone.AddListener(TryPausShower);
    }
    private void TryShowerStep2()
    {
        fillCircleBar.Show();
        if (tweenShower == null)
        {
            tweenShower = DOVirtual.Float(0, 1, showerTime,
                    value =>
                    {
                        ChangeAlphaFoam(1 - value);

                        fillCircleBar.Fill(value);
                    }).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    tweenShower = null;
                    TryEndShowerStep2();
                });
        }
        else if (!tweenShower.IsPlaying())
        {
            tweenShower.Play();
        }
    }
    private void TryEndShowerStep2()
    {
        fillCircleBar.Hide();
        DisableShowerTrigger();
        itemShowerDrag.onDragStart.RemoveListener(EnableShowerTrigger);
        itemShowerDrag.onDragStop.RemoveListener(DisableShowerTrigger);
        showerTrigger.onEnterZone.RemoveListener(TryShowerStep2);
        showerTrigger.onOutZone.RemoveListener(TryPausShower);
        DoneStep();
        // TryNextStep();
    }
    #endregion

    // [SerializeField] private SonDragItemBase faceTowelDrag;
    // [SerializeField] private OnTransformGoToAffectZone faceToweTrigger;
    // [SerializeField] private float faceToweTime;


    // private void OnStartStep8()
    // {
    //     faceTowelDrag.AddUseInStep(StepManager.Ins.CurrentStep);
    //     faceTowelDrag.onDragStart.AddListener(EnableMixtureTrigger);
    //     faceTowelDrag.onDragStop.AddListener(DisableMixtureTrigger);
    //     faceToweTrigger.onEnterZone.AddListener(TryPouringResin);
    //     faceToweTrigger.onOutZone.AddListener(TryPauseResin);
    //     fillCircleBar.ReFill();
    // }

    // private void EnableMixtureTrigger()
    // {
    //     faceToweTrigger.enabled = true;
    // }

    // private void DisableMixtureTrigger()
    // {
    //     faceToweTrigger.enabled = false;
    // }

    // private Tween pouringResin;



    // private void TryPouringResin()
    // {
    //     fillCircleBar.Show();

    //     if (pouringResin == null)
    //     {
    //         pouringResin = DOVirtual.Float(0, 1, faceToweTime,
    //                 value =>
    //                 {
    //                     SetEmissionRate(showerParticle, 10 - value);
    //                     fillCircleBar.Fill(value);

    //                 }).SetEase(Ease.Linear)
    //             .OnComplete(() =>
    //             {
    //                 pouringResin = null;
    //                 TryEndPouringResin();
    //             });
    //     }
    //     else if (!pouringResin.IsPlaying())
    //     {
    //         pouringResin.Play();
    //     }
    // }

    // private void TryPauseResin()
    // {
    //     fillCircleBar.Hide();

    //     if (pouringResin != null && pouringResin.IsPlaying())
    //     {
    //         pouringResin.Pause();
    //     }
    // }

    // private void TryEndPouringResin()
    // {
    //     DisableMixtureTrigger();
    //     fillCircleBar.Hide();
    //     SetEmissionRate(showerParticle, 0);
    //     showerParticle.Stop();
    //     faceTowelDrag.onDragStart.RemoveListener(EnableMixtureTrigger);
    //     faceTowelDrag.onDragStop.RemoveListener(DisableMixtureTrigger);
    //     faceToweTrigger.onEnterZone.RemoveListener(TryPouringResin);
    //     faceToweTrigger.onOutZone.RemoveListener(TryPauseResin);
    //     DoneStep();
    //     TutorialManager.Ins.enableCountTime = false;
    //     //TryNextStep();
    // }
    #region Step8


    [SerializeField] private List<SonSnapPoint> listSnapPointLens;
    [SerializeField] private List<SonSnapObject> listSnapObjLens;
    private int countSnap = 0;
    private void OnStartStep8()
    {

        listSnapPointLens[0].ChangeCanSnap(true);
        listSnapPointLens[1].ChangeCanSnap(true);
        for (int i = 0; i < listSnapObjLens.Count; i++)
        {
            SonSnapObject obj = listSnapObjLens[i];
            obj.OnSnap.AddListener(() =>
            {
                int removedIndex = listSnapObjLens.IndexOf(obj);
                if (removedIndex < 0) return;
                listSnapObjLens.RemoveAt(removedIndex);
                TutorialManager.Ins.ListTFLens.RemoveAt(removedIndex);
                TutorialManager.Ins.ListTFBoxLens.RemoveAt(removedIndex);
                if (listSnapObjLens.Count == 0)
                {
                    DoneStep();
                    TryNextStep();
                    AdsManager.Ins.ShowEndGame();
                    TutorialManager.Ins.SetNewTime(0.5f);
                }
            });
        }
    }
    [SerializeField] private SlotAttachmentPairList slotLensL;
    [SerializeField] private SlotAttachmentPairList slotLensR;

    public void SetStateSlotslotLensL()
    {
        slotLensL.TurnSlotState(true);
    }

    public void SetStateSlotslotLensR()
    {
        slotLensR.TurnSlotState(true);
    }

    #endregion
    #region Step9
    private void OnStartStep9()
    {
        AdsManager.Ins.ShowEndGame();
    }
    #endregion
}
