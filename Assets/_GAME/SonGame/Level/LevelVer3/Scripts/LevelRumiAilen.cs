using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using sonnv;
using System.Collections;
using HoangHH;
using Satisgame;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LevelRumiAilen : Singleton<LevelRumiAilen>
{
    //base
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
    public List<Item> items = new List<Item>();

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
    public void SetStateDoneStep(bool _value)
    {
        isDoneStep = _value;
    }
    protected virtual void DoneStep()
    {
        PlayPositiveEmoji();
        isDoneStep = true;
        Debug.Log("Fx step" + StepManager.Ins.CurrentStep);
    }

    public void TryNextStep()
    {
        StepManager.Ins.CurrentStep++;
        Debug.LogWarning("Next To" + StepManager.Ins.CurrentStep);
        StartStep();
    }
    [SerializeField] private SlotAttachmentPairList skinRumiStartOn;
    [SerializeField] private SlotAttachmentPairList skinRumStartiOff;
    public virtual void StartStep()
    {
        skinRumiStartOn.TurnSlotState(true);
        skinRumStartiOff.TurnSlotState(false);
        TutorialManager.Ins.enableCountTime = true;
        isDoneStep = false;

        switch (StepManager.Ins.CurrentStep)
        {
            case 0:
                OnStartStep1();
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
                return;
            case 5:
                return;
            case 6:
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
                Debug.Log("Happy");
            }
            else
            {
                Debug.Log("idle");

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
    //===




    private bool hadClicked = false;
    [SerializeField] private bool isPlayingGame = false;
    [SerializeField] private UIManager uIManager;
    public bool IsPlayingGame => isPlayingGame;
    private void Awake()
    {
    }
    private void StartGamePlay()
    {
        DOVirtual.DelayedCall(0.2f, () =>
        {
            isPlayingGame = true;
            EventManager.TriggerEvent("ShowBtnInstall");
        });
    }
    private void UpdateUIRumi()
    {
        SoundManager.Ins.PlayBgm();
    }
    private void UpdateUIAilen()
    {
        SoundManager.Ins.PlayBgmAilen();
    }
    [SerializeField] private TapSelectItem eventSelectRumi;
    [SerializeField] private TapSelectItem eventSelectAilen;
    [SerializeField] private int _currentCharacter = 0;

    private void OnStartStep1()
    {

        eventSelectRumi.onSelected.AddListener(() =>
        {
            DOVirtual.DelayedCall(0.2f, () =>
      {
          EventManager.TriggerEvent("ShowIconLv");
          isPlayingGame = true;
          EventManager.TriggerEvent("ShowBtnInstall");
      });
            TutorialManager.Ins.SetStateIsTap();
            _currentCharacter = 1;
            UpdateUIRumi();
            ChangePhase(1);
        });
        eventSelectAilen.onSelected.AddListener(() =>
        {
            DOVirtual.DelayedCall(0.2f, () =>
      {
          isPlayingGame = true;
          EventManager.TriggerEvent("ShowIconLv");
          EventManager.TriggerEvent("ShowBtnInstall");
      });
            TutorialManager.Ins.SetStateIsTap();

            _currentCharacter = 2;
            UpdateUIAilen();
            ChangePhase(2);

        });
    }
    [SerializeField] private SonTransitionPhase transitionPhase;

    private void ChangePhase(int _phaseIndex)
    {

        transitionPhase.TransitionToPhase(0, _phaseIndex, () =>
        {
            OnSetStateSkin(_phaseIndex);
        });
        transitionPhase.onComplete.AddListener(() =>
        {
            Debug.Log("Done Step11111");
            DoneStep();
            TryNextStep();
        });
    }
    [SerializeField] private CharacterControl characterRumi;
    [SerializeField] private CharacterControl characterAilen;
    private void OnSetStateSkin(int _index)
    {
        switch (_index)
        {
            case 1:
                SetNewCharacter(characterRumi);
                SetSkinRumi();
                return;
            case 2:
                SetNewCharacter(characterAilen);
                SetSkinAilen();
                return;
            default:
                return;
        }
    }
    [SerializeField] private SlotAttachmentPairList skinRumiOn;
    [SerializeField] private SlotAttachmentPairList skinRumiOff;

    private void SetSkinRumi()
    {
        skinRumiOn.TurnSlotState(true);
        skinRumiOff.TurnSlotState(false);

        Debug.Log("Rumi");
    }
    [SerializeField] private SlotAttachmentPairList skinAilenOn;
    [SerializeField] private SlotAttachmentPairList skinAilenOff;
    [SerializeField] private SlotAttachmentPairList faceAilenOn;
    [SerializeField] private SlotAttachmentPairList hairAilenOn;

    private void SetSkinAilen()
    {
        skinAilenOn.TurnSlotState(true);
        skinAilenOff.TurnSlotState(false);

        faceAilenOn.TurnSlotState(true);
        hairAilenOn.TurnSlotState(true);

        Debug.Log("Ailen");

    }
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
    [SerializeField] private float targetOrthoSizeStep1 = 10;
    [SerializeField] private float targetLocalYStep1 = 10f;
    [SerializeField] private float durationStep1 = 0.75f;
    [SerializeField] private SonEffectShowObject effectShow;
    [SerializeField] private bool canBrush = false;

    public void SetCanBrush()
    {
        canBrush = true;
        EnableShowerTrigger();
    }
    public void SetEmojiControl(EmojiControl newEmoji)
    {
        if (newEmoji == null) return;
        LevelRumiAilen.Ins.SetNewEmoji(newEmoji);
        Debug.Log("SET NEW EMOJI");
    }
    public EmojiControl emojiControlStep1;


    private void OnStartStep2()
    {
        switch (_currentCharacter)
        {
            case 1:
                StartCoroutine(IE_OnMoveCamBrushStep1());
                return;
            case 2:
                StartCoroutine(IE_OnMoveCamAilen());

                return;
            default:
                return;
        }
    }
    [SerializeField] private float targetOrthoSizeAilen = 10;
    [SerializeField] private float targetLocalYAilen = 10f;
    [SerializeField] private float durationAilen = 0.75f;
    public EmojiControl emojiControlAilen;

    IEnumerator IE_OnMoveCamAilen()
    {
        yield return new WaitForSeconds(0.75f);
        MoveCamera(cam, targetOrthoSizeAilen, targetLocalYAilen, durationAilen, () =>
        {
            SetNewEmoji(emojiControlAilen);
            SnapObjectScrollUIController.Instance.Show();
            Debug.Log("show");
            LoadUI();
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
        for (int i = 0; i < dressItemSnapConfig.Count; i++)
        {
            var item = dressItemSnapConfig[i];

            item.onSnap += () => SnapDress(item, 5);
        }


    }
    private int countSnapUI = 0;
    private void SnapDress(SnapObjectUI.SnapObjectUIConfig item, int targget)
    {
        if (dressItemSnapConfig.Remove(item))
        {
            PlayAnimHappy();
            countSnapUI++;
            item.onSnap = null;
            item.onRelease = null;
            item.onStartDrag = null;
            // if (dressItemSnapConfig.Count == 0)
            // {
            //     DoneStep();
            //     TryNextStep();
            // }
            if (countSnapUI >= targget)
            {
                Debug.Log("WIN");
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



















    [SerializeField] private AudioSource audioBrush;

    IEnumerator IE_OnMoveCamBrushStep1()
    {
        yield return new WaitForSeconds(0.75f);
        MoveCamera(cam, targetOrthoSizeStep1, targetLocalYStep1, durationStep1, () =>
        {
            effectShow.Show();
            effectShow.onShowComplete.AddListener(() =>
            {
                SetNewEmoji(emojiControlStep1);
            });
        });
        itemBrushDrag.AddUseInStep(StepManager.Ins.CurrentStep);
        itemBrushDrag.onDragStart.AddListener(EnableShowerTrigger);
        itemBrushDrag.onDragStop.AddListener(DisableShowerTrigger);
        showerTrigger.onEnterZone.AddListener(TryShower);
        showerTrigger.onOutZone.AddListener(TryPausShower);
    }
    [SerializeField] private SonDragItemBase itemBrushDrag;
    [SerializeField] private OnTransformGoToAffectZone showerTrigger;
    [SerializeField] private float showerTime;
    private void EnableShowerTrigger()
    {
        if (canBrush)
        {
            showerTrigger.enabled = true;

        }
    }

    private void DisableShowerTrigger()
    {
        showerTrigger.enabled = false;
    }

    private Tween tweenShower;

    [SerializeField] private SpriteRenderer spriteTattooBody;

    private void TryShower()
    {
        fillCircleBar.Show();
        audioBrush.Play();
        if (tweenShower == null)
        {
            tweenShower = DOVirtual.Float(0, 1, showerTime,
                    value =>
                    {
                        ChangeAlphaCleanser(spriteTattooBody, value);
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
    private void ChangeAlphaCleanser(SpriteRenderer _sprite, float value)
    {
        value = Mathf.Clamp01(value);
        Color c = _sprite.color;
        c.a = value;
        _sprite.color = c;
    }
    private void TryPausShower()
    {
        fillCircleBar.Hide();
        audioBrush.Stop();


        if (tweenShower != null && tweenShower.IsPlaying())
        {
            tweenShower.Pause();
        }
    }

    private void TryEndShower()
    {
        audioBrush.Stop();

        fillCircleBar.Hide();
        DisableShowerTrigger();
        itemBrushDrag.onDragStart.RemoveListener(EnableShowerTrigger);
        itemBrushDrag.onDragStop.RemoveListener(DisableShowerTrigger);
        showerTrigger.onEnterZone.RemoveListener(TryShower);
        showerTrigger.onOutZone.RemoveListener(TryPausShower);
        DoneStep();
        Debug.Log(IsDoneStep + " DoneStep");
        SetStateDoneStep(true);
        //TryNextStep();
    }
    [SerializeField] private OnTransformGoToAffectZone showerTriggerStep2;

    [SerializeField] private float targetLocalYStep2 = -3.5f;
    public EmojiControl emojiControlStep2;

    private void OnStartStep3()
    {
        switch (_currentCharacter)
        {
            case 1:
                fillCircleBar.ReFill();
                StartCoroutine(IE_OnMoveCamBrushStep2());
                return;
            case 2:
                return;
            default:
                return;
        }

    }
    IEnumerator IE_OnMoveCamBrushStep2()
    {
        yield return new WaitForSeconds(0.75f);
        MoveCamera(cam, targetOrthoSizeStep1, targetLocalYStep2, durationStep1, () =>
        {
            SetNewEmoji(emojiControlStep2);
        });
        itemBrushDrag.AddUseInStep(StepManager.Ins.CurrentStep);
        itemBrushDrag.onDragStart.AddListener(EnableShowerTriggerStep2);
        itemBrushDrag.onDragStop.AddListener(DisableShowerTriggerStep2);
        showerTriggerStep2.onEnterZone.AddListener(TryShowerStep2);
        showerTriggerStep2.onOutZone.AddListener(TryPausShower);
    }
    [SerializeField] private SpriteRenderer spriteTattooLeg;
    private void EnableShowerTriggerStep2()
    {
        if (canBrush)
        {
            showerTriggerStep2.enabled = true;

        }
    }

    private void DisableShowerTriggerStep2()
    {
        showerTriggerStep2.enabled = false;
    }
    private void TryShowerStep2()
    {
        fillCircleBar.Show();
        audioBrush.Play();

        if (tweenShower == null)
        {
            tweenShower = DOVirtual.Float(0, 1, showerTime,
                    value =>
                    {
                        ChangeAlphaCleanser(spriteTattooLeg, value);
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
        audioBrush.Stop();

        fillCircleBar.Hide();
        DisableShowerTrigger();
        itemBrushDrag.onDragStart.RemoveListener(EnableShowerTriggerStep2);
        itemBrushDrag.onDragStop.RemoveListener(DisableShowerTriggerStep2);
        showerTriggerStep2.onEnterZone.RemoveListener(TryShowerStep2);
        showerTriggerStep2.onOutZone.RemoveListener(TryPausShower);
        DoneStep();
        //TryNextStep();
    }
    private void OnStartStep4()
    {
        switch (_currentCharacter)
        {
            case 1:
                StartCoroutine(IE_OnMoveCamStepSkinRumi());
                return;
            case 2:
                return;
            default:
                return;
        }

    }
    [SerializeField] private float targetOrthoSizeStepSkinRumi = 16.9f;
    [SerializeField] private float targetLocalYStepSkinRumi = 0f;
    [SerializeField] private float durationStepSKinRumi = 0.75f;
    IEnumerator IE_OnMoveCamStepSkinRumi()
    {
        effectShow.Hide();
        yield return new WaitForSeconds(0.75f);
        MoveCamera(cam, targetOrthoSizeStepSkinRumi, targetLocalYStepSkinRumi, durationStepSKinRumi, () =>
        {
            SnapObjectScrollUIController.Instance.Show();
            Debug.Log("show Rumi");
            LoadUIRumi();
        });
    }
    [SerializeField] private List<SnapObjectUI.SnapObjectUIConfig> dressItemSnapConfigRumi;
    private void LoadUIRumi()
    {
        for (int i = 0; i < dressItemSnapConfigRumi.Count; i++)
        {
            SnapObjectScrollUIController.Instance.AddData(dressItemSnapConfigRumi[i]);
            SnapObjectScrollUIController.Instance.ManualSetOrder(GetSequence(dressItemSnapConfigRumi.Count, false));
        }
        for (int i = 0; i < dressItemSnapConfigRumi.Count; i++)
        {
            var item = dressItemSnapConfigRumi[i];
            Debug.Log(i + " item");

            item.onSnap += () => SnapDressRumi(item);
        }


    }
    private int countSnapUIRumi = 0;

    private void SnapDressRumi(SnapObjectUI.SnapObjectUIConfig item)
    {
        if (dressItemSnapConfigRumi.Remove(item))
        {
            PlayAnimHappy();
            countSnapUIRumi++;
            item.onSnap = null;
            item.onRelease = null;
            item.onStartDrag = null;
            Debug.Log(countSnapUIRumi + " countSnapUIRumi");
            if (countSnapUIRumi >= 3)
            {
                Debug.Log("WIN");
                AdsManager.Ins.ShowEndGame();
            }
        }
    }
}
