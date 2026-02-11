// using UnityEngine;
// using DG.Tweening;
// using System.Collections.Generic;
// using sonnv;
// using System.Collections;
// using HoangHH;
// public class LevelSailorMoon : GamePlayManager
// {
//     private bool hadClicked = false;
//     [SerializeField] private bool isPlayingGame = false;
//     [SerializeField] private UIManager uIManager;
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
//     private void Awake()
//     {
//         slotMouth.TurnSlotState(false);

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
// #if UNITY_EDITOR

//   [Sirenix.OdinInspector.Button]
//    public void TurnOffSlotMouth()
//     {
//          slotMouth.TurnSlotState(false);
//     }

// #endif

//     [SerializeField] private SlotAttachmentPairList slotMouth;

//     [SerializeField] private SonThrowObject throwObjectGlasses;
//     [SerializeField] private SlotAttachmentPairList slotOffGlasses;
//     public void SetStateSlotGlasses(bool value)
//     {
//         slotOffGlasses.TurnSlotState(value);
//     }
//     private void OnStartStep1()
//     {
//         throwObjectGlasses.onRemoveItem.AddListener(() =>
//         {
//             DoneStep();
//             TryNextStep();
//         });
//     }
//     [SerializeField] private SonSnapObject headBandSnapObj;
//     [SerializeField] private SonSnapPoint headBandSnapPoint;
//     private void OnStartStep2()
//     {
//         headBandSnapPoint.ChangeCanSnap(true);
//         headBandSnapObj.OnSnap.AddListener(() =>
//         {
//             SetStateSlotHeadBand();
//             DoneStep();
//             TryNextStep();
//         });
//     }
//     [SerializeField] private SlotAttachmentPairList slotHeadBand;
//     [SerializeField] private SlotAttachmentPairList slotHair;

//     public void SetStateSlotHeadBand()
//     {
//         slotHeadBand.TurnSlotState(true);
//         slotHair.TurnSlotState(false);
//     }


//     //============================================================================================================
//     [SerializeField] private SonDragItemBase itemShowerDrag;
//     [SerializeField] private OnTransformGoToAffectZone showerTrigger;
//     [SerializeField] private float showerTime;
//     [SerializeField] private ParticleSystem showerParticle;
//     private void OnStartStep3()
//     {
//         itemShowerDrag.AddUseInStep(StepManager.Ins.CurrentStep);
//         itemShowerDrag.onDragStart.AddListener(EnableShowerTrigger);
//         itemShowerDrag.onDragStop.AddListener(DisableShowerTrigger);
//         showerTrigger.onEnterZone.AddListener(TryShower);
//         showerTrigger.onOutZone.AddListener(TryPausShower);
//     }
//     private void EnableShowerTrigger()
//     {
//         showerTrigger.enabled = true;
//     }

//     private void DisableShowerTrigger()
//     {
//         showerTrigger.enabled = false;
//     }

//     private Tween tweenShower;



//     private void TryShower()
//     {
//         fillCircleBar.Show();
//         if (tweenShower == null)
//         {
//             showerParticle.Play();
//             tweenShower = DOVirtual.Float(0, 1, showerTime,
//                     value =>
//                     {
//                         SetEmissionRate(showerParticle, (value * 10f));
//                         fillCircleBar.Fill(value);
//                     }).SetEase(Ease.Linear)
//                 .OnComplete(() =>
//                 {
//                     tweenShower = null;
//                     TryEndShower();
//                 });
//         }
//         else if (!tweenShower.IsPlaying())
//         {
//             tweenShower.Play();
//         }
//     }

//     private void TryPausShower()
//     {
//         fillCircleBar.Hide();

//         if (tweenShower != null && tweenShower.IsPlaying())
//         {
//             tweenShower.Pause();
//         }
//     }

//     private void TryEndShower()
//     {
//         fillCircleBar.Hide();
//         SetEmissionRate(showerParticle, 10);
//         showerParticle.Play();
//         DisableShowerTrigger();
//         itemShowerDrag.onDragStart.RemoveListener(EnableShowerTrigger);
//         itemShowerDrag.onDragStop.RemoveListener(DisableShowerTrigger);
//         showerTrigger.onEnterZone.RemoveListener(TryShower);
//         showerTrigger.onOutZone.RemoveListener(TryPausShower);
//         DoneStep();
//         //TryNextStep();
//     }

//     [SerializeField] private List<TriggerWithCertainCollider> listTriggerCleanser;
//     [SerializeField] private SonDragItemBase itemBoxCleanser;
//     [SerializeField] private AudioData vfxCleanser;
//     private int countTrigger = 0;
//     private void OnStartStep4()
//     {

//         itemBoxCleanser.AddUseInStep(StepManager.Ins.CurrentStep);
//         for (int i = 0; i < listTriggerCleanser.Count; i++)
//         {
//             TriggerWithCertainCollider cleanser = listTriggerCleanser[i];
//             cleanser.EnableCol(true);
//             cleanser.AddTriggerEvent(() =>
//             {
//                 SoundManager.PlaySFX(vfxCleanser.clip, 1);
//                 countTrigger++;
//                 if (countTrigger >= listTriggerCleanser.Count)
//                 {
//                     DoneStep();
//                     //TryNextStep();
//                 }
//             });
//         }
//     }
//     [SerializeField] private SonDragItemBase itemWashMachineDrag;
//     [SerializeField] private OnTransformGoToAffectZone washTrigger;
//     [SerializeField] private float washTime = 3;
//     [SerializeField] private List<SpriteRenderer> listSpriteCleanser;
//     [SerializeField] private SpriteRenderer spriteFoamCleanser;

//     private void ChangeAlphaCleanser(float value)
//     {
//         value = Mathf.Clamp01(value);

//         foreach (SpriteRenderer sprite in listSpriteCleanser)
//         {
//             if (sprite == null) continue;

//             Color c = sprite.color;
//             c.a = value;
//             sprite.color = c;
//         }
//     }

//     private void ChangeAlphaFoam(float value)
//     {
//         if (spriteFoamCleanser == null) return;

//         value = Mathf.Clamp01(value);

//         Color c = spriteFoamCleanser.color;
//         c.a = value;
//         spriteFoamCleanser.color = c;
//     }

//     private void OnStartStep5()
//     {
//         itemWashMachineDrag.AddUseInStep(StepManager.Ins.CurrentStep);
//         itemWashMachineDrag.onDragStart.AddListener(EnableWashTrigger);
//         itemWashMachineDrag.onDragStop.AddListener(DisableWashTrigger);
//         washTrigger.onEnterZone.AddListener(TryWash);
//         washTrigger.onOutZone.AddListener(TryPausWash);
//         fillCircleBar.ReFill();
//     }

//     private void EnableWashTrigger()
//     {
//         washTrigger.enabled = true;
//     }

//     private void DisableWashTrigger()
//     {
//         washTrigger.enabled = false;
//     }

//     private Tween tweenWash;


//     [SerializeField] private SlotAttachmentPairList slotfreckles;
//     private void TryWash()
//     {
//         fillCircleBar.Show();
//         if (tweenWash == null)
//         {
//             tweenWash = DOVirtual.Float(0, 1, washTime,
//                     value =>
//                     {

//                         fillCircleBar.Fill(value);
//                         ChangeAlphaCleanser(1 - value);
//                         ChangeAlphaFoam(value);
//                         SetStateChangeAnim(false);
//                     }).SetEase(Ease.Linear)
//                 .OnComplete(() =>
//                 {
//                     tweenWash = null;
//                     slotfreckles.TurnSlotState(false);

//                     TryEndWash();
//                 });
//         }
//         else if (!tweenWash.IsPlaying())
//         {
//             tweenWash.Play();
//         }
//     }

//     private void TryPausWash()
//     {
//         fillCircleBar.Hide();

//         if (tweenWash != null && tweenWash.IsPlaying())
//         {
//             tweenWash.Pause();
//         }
//     }

//     private void TryEndWash()
//     {
//         fillCircleBar.Hide();
//         ChangeAlphaCleanser(0);
//         DisableWashTrigger();
//         itemWashMachineDrag.onDragStart.RemoveListener(EnableWashTrigger);
//         itemWashMachineDrag.onDragStop.RemoveListener(DisableWashTrigger);
//         washTrigger.onEnterZone.RemoveListener(TryWash);
//         washTrigger.onOutZone.RemoveListener(TryPausWash);
//         DoneStep();
//         //TryNextStep();
//     }
//     private void OnStartStep6()
//     {
//         SetStateChangeAnim(true);
//         itemShowerDrag.AddUseInStep(StepManager.Ins.CurrentStep);
//         itemShowerDrag.onDragStart.AddListener(EnableShowerTrigger);
//         itemShowerDrag.onDragStop.AddListener(DisableShowerTrigger);
//         showerTrigger.onEnterZone.AddListener(TryShowerStep2);
//         showerTrigger.onOutZone.AddListener(TryPausShower);
//     }
//     private void TryShowerStep2()
//     {
//         fillCircleBar.Show();
//         if (tweenShower == null)
//         {
//             showerParticle.Play();
//             tweenShower = DOVirtual.Float(0, 1, showerTime,
//                     value =>
//                     {
//                         ChangeAlphaFoam(1 - value);

//                         fillCircleBar.Fill(value);
//                     }).SetEase(Ease.Linear)
//                 .OnComplete(() =>
//                 {
//                     tweenShower = null;
//                     TryEndShowerStep2();
//                 });
//         }
//         else if (!tweenShower.IsPlaying())
//         {
//             tweenShower.Play();
//         }
//     }


//     private void TryEndShowerStep2()
//     {
//         fillCircleBar.Hide();
//         DisableShowerTrigger();
//         itemShowerDrag.onDragStart.RemoveListener(EnableShowerTrigger);
//         itemShowerDrag.onDragStop.RemoveListener(DisableShowerTrigger);
//         showerTrigger.onEnterZone.RemoveListener(TryShowerStep2);
//         showerTrigger.onOutZone.RemoveListener(TryPausShower);
//         DoneStep();
//         // TryNextStep();
//     }
//     [SerializeField] private SonDragItemBase faceTowelDrag;
//     [SerializeField] private OnTransformGoToAffectZone faceToweTrigger;
//     [SerializeField] private float faceToweTime;


//     private void OnStartStep7()
//     {
//         faceTowelDrag.AddUseInStep(StepManager.Ins.CurrentStep);
//         faceTowelDrag.onDragStart.AddListener(EnableMixtureTrigger);
//         faceTowelDrag.onDragStop.AddListener(DisableMixtureTrigger);
//         faceToweTrigger.onEnterZone.AddListener(TryPouringResin);
//         faceToweTrigger.onOutZone.AddListener(TryPauseResin);
//         fillCircleBar.ReFill();
//     }

//     private void EnableMixtureTrigger()
//     {
//         faceToweTrigger.enabled = true;
//     }

//     private void DisableMixtureTrigger()
//     {
//         faceToweTrigger.enabled = false;
//     }

//     private Tween pouringResin;



//     private void TryPouringResin()
//     {
//         fillCircleBar.Show();

//         if (pouringResin == null)
//         {
//             pouringResin = DOVirtual.Float(0, 1, faceToweTime,
//                     value =>
//                     {
//                         SetEmissionRate(showerParticle, 10 - value);
//                         fillCircleBar.Fill(value);

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
//         fillCircleBar.Hide();

//         if (pouringResin != null && pouringResin.IsPlaying())
//         {
//             pouringResin.Pause();
//         }
//     }

//     private void TryEndPouringResin()
//     {
//         DisableMixtureTrigger();
//         fillCircleBar.Hide();
//         SetEmissionRate(showerParticle, 0);
//         showerParticle.Stop();
//         faceTowelDrag.onDragStart.RemoveListener(EnableMixtureTrigger);
//         faceTowelDrag.onDragStop.RemoveListener(DisableMixtureTrigger);
//         faceToweTrigger.onEnterZone.RemoveListener(TryPouringResin);
//         faceToweTrigger.onOutZone.RemoveListener(TryPauseResin);
//         DoneStep();
//         //TryNextStep();
//     }
//     [SerializeField] private ShowObjectEffect step1;
//     [SerializeField] private ShowObjectEffect step2;
//     private void OnStartStep8()
//     {
//         step1.Hide();
//         step2.Show(0.75f);
//         step2.onShowComplete.AddListener(() =>
//         {
//             AdsManager.Ins.ShowEndGame();
//         });
//     }
// }
