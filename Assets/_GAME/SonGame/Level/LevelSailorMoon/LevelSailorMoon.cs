using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using sonnv;
using System.Collections;
using HoangHH;
public class LevelSailorMoon : GamePlayManager
{
    private bool hadClicked = false;
    [SerializeField] private bool isPlayingGame = false;
    [SerializeField] private UIManager uIManager;
    public bool IsPlayingGame => isPlayingGame;
    private void Update()
    {
        // if (isPlayingGame && Input.GetMouseButtonDown(0))
        // {
        //     EventManager.TriggerEvent("ShowBtnInstall");
        // }

        if (!hadClicked && Input.GetMouseButtonDown(0))
        {
            hadClicked = true;
            StartGamePlay();
        }
    }
    private void StartGamePlay()
    {
        GamePlayScreen uiScreen = UIManager.Ins.GetUI(0);
        uiScreen.logoUI.SetActive(false);
        EventManager.TriggerEvent("ShowIconLv");
        SoundManager.Ins.PlayFx(FxType.StartGame);
        SoundManager.Ins.PlayBgm();
        DOVirtual.DelayedCall(0.2f, () =>
        {
            isPlayingGame = true;
            EventManager.TriggerEvent("ShowBtnInstall");
        });
    }
    public override void StartStep()
    {
        base.StartStep();
        //TutorialManager.Ins.enableCountTime = true;
        switch (CurrentStep)
        {
            case 0:
                OnStartStep1();
                return;
            case 1:
                // OnStartStep2();
                return;
            case 2:
                //StartStep3();
                return;
            case 3:
                //OnStartStep4();
                return;
            default:
                return;
        }
    }
    [SerializeField] private SonThrowObject throwObjectGlasses;
    [SerializeField] private SlotAttachmentPairList slotOffGlasses;
    public void SetStateSlotGlasses(bool value)
    {
        slotOffGlasses.TurnSlotState(value);
    }
    private void OnStartStep1()
    {
        throwObjectGlasses.onRemoveItem.AddListener(() =>
        {
            DoneStep();
            TryNextStep();
        });
    }
    [SerializeField] private SonDragItemBase mixtureDrag;
    public SonDragItemBase MixtureDrag => mixtureDrag;
    [SerializeField] private OnTransformGoToAffectZone mixtureTrigger;
    [SerializeField] private float fadingTime;


    private void OnStartStep()
    {
        mixtureDrag.onDragStart.AddListener(EnableMixtureTrigger);
        mixtureDrag.onDragStop.AddListener(DisableMixtureTrigger);
        mixtureTrigger.onEnterZone.AddListener(TryPouringResin);
        mixtureTrigger.onOutZone.AddListener(TryPauseResin);
    }

    private void EnableMixtureTrigger()
    {

        mixtureTrigger.enabled = true;
    }

    private void DisableMixtureTrigger()
    {


        mixtureTrigger.enabled = false;
    }

    private Tween pouringResin;



    private void TryPouringResin()
    {
        if (pouringResin == null)
        {
            pouringResin = DOVirtual.Float(0, 1, fadingTime,
                    value =>
                    {
                    }).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    pouringResin = null;
                    TryEndPouringResin();
                });
        }
        else if (!pouringResin.IsPlaying())
        {
            pouringResin.Play();
        }
    }

    private void TryPauseResin()
    {
        if (pouringResin != null && pouringResin.IsPlaying())
        {
            pouringResin.Pause();
        }
    }

    private void TryEndPouringResin()
    {
        DisableMixtureTrigger();
        mixtureDrag.onDragStart.RemoveListener(EnableMixtureTrigger);
        mixtureDrag.onDragStop.RemoveListener(DisableMixtureTrigger);
        mixtureTrigger.onEnterZone.RemoveListener(TryPouringResin);
        mixtureTrigger.onOutZone.RemoveListener(TryPauseResin);
        DoneStep();
        TryNextStep();
    }

}
