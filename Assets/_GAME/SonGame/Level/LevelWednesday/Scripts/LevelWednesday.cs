using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using sonnv;
using System.Collections;

public class LevelWednesday : GamePlayManager
{
    private bool hadClicked = false;
    [SerializeField] private bool isPlayingGame = false;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private GameObject objItem;
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
        switch (CurrentStep)
        {
            case 0:
                OnStartStep1();
                return;
            case 1:
                OnStartStep2();
                return;
            case 2:
                return;
            default:
                return;
        }
    }
    [SerializeField] private SpriteEditor handSprEditor;
    [SerializeField] private SonDragItemBase mixtureDrag;
    [SerializeField] private OnTransformGoToAffectZone mixtureTrigger;
    [SerializeField] private float fadingTime;
    [SerializeField] private GameObject objWaterFlip;
    [SerializeField] private SpriteRenderer sprWaterFlip;


    private void OnStartStep1()
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

    private void SetStateWaterFlip(bool isActive)
    {
        objWaterFlip.SetActive(isActive);
        sprWaterFlip.enabled = !isActive;
    }
    private void TryPouringResin()
    {
        SetStateWaterFlip(true);
        if (pouringResin == null)
        {
            pouringResin = DOVirtual.Float(handSprEditor.fillAmount, 1, fadingTime,
                    value =>
                    {
                        handSprEditor.fillAmount = value;
                        handSprEditor.TryApplyVerticalFill();
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
        SetStateWaterFlip(false);
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
    [SerializeField] private SonThrowObject moldLid;
    private void OnStartStep2()
    {
        moldLid.transform.DOLocalJump(new Vector3(0.5f, 2f, 0), 0.5f, 1, 0.2f);
        moldLid.enabled = true;
        moldLid.Col.enabled = true;
    }
}
