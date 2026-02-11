// using UnityEngine;
// using DG.Tweening;
// using System.Collections.Generic;
// using sonnv;
// using System.Collections;
// using HoangHH;
// public class LevelWednesday : GamePlayManager
// {
//     private bool hadClicked = false;
//     [SerializeField] private bool isPlayingGame = false;
//     [SerializeField] private UIManager uIManager;
//     [SerializeField] private GameObject objItem;
//     public bool IsPlayingGame => isPlayingGame;
//     private void Update()
//     {
//         // if (isPlayingGame && Input.GetMouseButtonDown(0))
//         // {
//         //     EventManager.TriggerEvent("ShowBtnInstall");
//         // }

//         if (!hadClicked && Input.GetMouseButtonDown(0))
//         {
//             hadClicked = true;
//             StartGamePlay();
//         }
//     }
//     private void StartGamePlay()
//     {
//         GamePlayScreen uiScreen = UIManager.Ins.GetUI(0);
//         uiScreen.logoUI.SetActive(false);
//         EventManager.TriggerEvent("ShowIconLv");
//         SoundManager.Ins.PlayFx(FxType.StartGame);
//         SoundManager.Ins.PlayBgm();
//         DOVirtual.DelayedCall(0.2f, () =>
//         {
//             isPlayingGame = true;
//             EventManager.TriggerEvent("ShowBtnInstall");
//         });
//     }

//     [SerializeField] private SonDragItemBase mixtureDrag;
//     public SonDragItemBase MixtureDrag => mixtureDrag;
//     [SerializeField] private OnTransformGoToAffectZone mixtureTrigger;
//     [SerializeField] private float fadingTime;
//     [SerializeField] private GameObject objWaterFlip;
//     [SerializeField] private SpriteRenderer sprWaterFlip;
//     [SerializeField] private AudioSource sfxPouringResin;


//     private void OnStartStep1()
//     {
//         mixtureDrag.onDragStart.AddListener(EnableMixtureTrigger);
//         mixtureDrag.onDragStop.AddListener(DisableMixtureTrigger);
//         mixtureTrigger.onEnterZone.AddListener(TryPouringResin);
//         mixtureTrigger.onOutZone.AddListener(TryPauseResin);
//     }

//     private void EnableMixtureTrigger()
//     {

//         mixtureTrigger.enabled = true;
//     }

//     private void DisableMixtureTrigger()
//     {


//         mixtureTrigger.enabled = false;
//     }

//     private Tween pouringResin;

//     private void SetStateWaterFlip(bool isActive)
//     {
//         objWaterFlip.SetActive(isActive);
//         sprWaterFlip.enabled = !isActive;
//     }
//     [SerializeField] private SpriteMaskFill maskFill;

//     private void TryPouringResin()
//     {
//         sfxPouringResin.Play();
//         SetStateWaterFlip(true);
//         if (pouringResin == null)
//         {
//             pouringResin = DOVirtual.Float(0, 1, fadingTime,
//                     value =>
//                     {
//                         maskFill.SetFill(value);
//                     }).SetEase(Ease.Linear)
//                 .OnComplete(() =>
//                 {
//                     pouringResin = null;
//                     TryEndPouringResin();
//                 });
//         }
//         else if (!pouringResin.IsPlaying())
//         {
//             pouringResin.Play();
//         }
//     }

//     private void TryPauseResin()
//     {
//         sfxPouringResin.Stop();
//         SetStateWaterFlip(false);
//         if (pouringResin != null && pouringResin.IsPlaying())
//         {
//             pouringResin.Pause();
//         }
//     }

//     private void TryEndPouringResin()
//     {
//         maskFill.fillTransform.gameObject.SetActive(false);
//         DisableMixtureTrigger();
//         mixtureDrag.onDragStart.RemoveListener(EnableMixtureTrigger);
//         mixtureDrag.onDragStop.RemoveListener(DisableMixtureTrigger);
//         mixtureTrigger.onEnterZone.RemoveListener(TryPouringResin);
//         mixtureTrigger.onOutZone.RemoveListener(TryPauseResin);
//         DoneStep();
//         TryNextStep();
//     }
//     [SerializeField] private SonThrowObject moldLid;
//     private void OnStartStep2()
//     {
//         moldLid.transform.DOLocalJump(new Vector3(0.5f, 2f, 0), 0.5f, 1, 0.2f);
//         moldLid.enabled = true;
//         moldLid.Col.enabled = true;
//         moldLid.onRemoveItem.AddListener(() =>
//         {
//             TryEndThrowLid();
//         });
//     }
//     private void TryEndThrowLid()
//     {
//         ShakeHand();
//     }
//     [SerializeField] private Transform stealInteracts;
//     [SerializeField] private ShowObjectEffect lidRig;
//     private void ShakeHand()
//     {
//         lidRig.Hide(0.15f);
//         stealInteracts.transform.DOMoveY(stealInteracts.transform.position.y + 1, 0.3f).SetEase(Ease.Linear);
//         stealInteracts.transform.DOScale(1.3f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
//         {
//             OnEndStep2();
//         });

//     }
//     private void OnEndStep2()
//     {
//         DoneStep();
//         TryNextStep();
//     }
//     [SerializeField] private SonDragItemBase airBrushDrag;
//     [SerializeField] private OnTransformGoToAffectZone airBrushTrigger;
//     [SerializeField] private float fadingAirBrushTime;
//     [SerializeField] private AudioSource sfxAirBrush;

//     private void StartStep3()
//     {
//         airBrushDrag.onDragStart.AddListener(EnableAirBrushTrigger);
//         airBrushDrag.onDragStop.AddListener(DisableAirBrushTrigger);
//         airBrushTrigger.onEnterZone.AddListener(TryTweenFade);
//         airBrushTrigger.onOutZone.AddListener(TryPauseTweenFade);
//     }
//     private void EnableAirBrushTrigger()
//     {
//         airBrushTrigger.enabled = true;
//         sfxAirBrush.Play();
//     }

//     private void DisableAirBrushTrigger()
//     {
//         airBrushTrigger.enabled = false;
//         sfxAirBrush.Stop();
//     }

//     private Tween tweenFade;
//     [SerializeField] private SpriteRenderer spriteAir;

//     [SerializeField] private FillCircleBar fillCircleBar;

//     private void TryTweenFade()
//     {
//         fillCircleBar.Show();
//         if (tweenFade == null)
//         {
//             tweenFade = DOVirtual.Float(0, 1, fadingAirBrushTime,
//                     value =>
//                     {
//                         Color c = spriteAir.color;
//                         c.a = value;
//                         spriteAir.color = c;
//                         fillCircleBar.Fill(value);
//                     }).SetEase(Ease.Linear)
//                 .OnComplete(() =>
//                 {
//                     tweenFade = null;
//                     TryEndFadeAir();
//                 });
//         }
//         else if (!tweenFade.IsPlaying())
//         {
//             tweenFade.Play();
//         }
//     }

//     private void TryPauseTweenFade()
//     {
//         fillCircleBar.Hide();

//         if (tweenFade != null && tweenFade.IsPlaying())
//         {
//             tweenFade.Pause();
//         }
//     }

//     private void TryEndFadeAir()
//     {
//         DisableAirBrushTrigger();
//         fillCircleBar.Hide();

//         airBrushDrag.onDragStart.RemoveListener(EnableAirBrushTrigger);
//         airBrushDrag.onDragStop.RemoveListener(DisableAirBrushTrigger);
//         airBrushTrigger.onEnterZone.RemoveListener(TryTweenFade);
//         airBrushTrigger.onOutZone.RemoveListener(TryPauseTweenFade);
//         DoneStep();
//         TryNextStep();
//     }
//     [SerializeField] private ShowObjectEffect effectStep1;
//     [SerializeField] private ShowObjectEffect effectStep2;
//     [SerializeField] private List<SonSnapObject> listSnapNail;
//     private int countSnapNail = 0;
//     private void OnStartStep4()
//     {
//         effectStep1.Hide();
//         stealInteracts.transform.DOMoveY(1.25f, 0.75f).SetEase(Ease.Linear);
//         effectStep2.Show(0.75f);

//         for (int i = 0; i < listSnapNail.Count; i++)
//         {
//             SonSnapObject obj = listSnapNail[i];
//             obj.OnSnap.AddListener(() =>
//             {
//                 ControllerDoneVFX.Instance.SpawnSnapVFX(obj.SnapPoint.Tf);
//                 countSnapNail++;
//                 if (countSnapNail >= listSnapNail.Count)
//                 {
//                     DoneStep();
//                     TryNextStep();
//                     AdsManager.Ins.ShowEndGame();

//                 }
//             });
//         }
//     }
// }
