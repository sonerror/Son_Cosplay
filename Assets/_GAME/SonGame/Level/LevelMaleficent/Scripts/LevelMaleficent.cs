// using UnityEngine;
// using DG.Tweening;
// using System.Collections.Generic;
// using sonnv;
// using System.Collections;
// public class LevelMaleficent : GamePlayManager
// {
//     [SerializeField] private SlotAttachmentPairList slotOn;
//     [SerializeField] private SlotAttachmentPairList slotOff;
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

//     [SerializeField] private List<SonThrowObject> listThrowObject;
//     public List<SonThrowObject> ListThrowObject => listThrowObject;
//     private int countThrowObjDone = 0;
//     private void OnStartStep1()
//     {
//         for (int i = 0; i < listThrowObject.Count; i++)
//         {
//             SonThrowObject obj = listThrowObject[i];

//             obj.onRemoveItem.AddListener(() =>
//             {
//                 int removedIndex = listThrowObject.IndexOf(obj);
//                 if (removedIndex < 0) return;


//                 listThrowObject.RemoveAt(removedIndex);
//                 TutorialManager.Ins.TutorialNode.RemoveAt(removedIndex);

//                 if (listThrowObject.Count == 0)
//                 {
//                     DoneStep();
//                     TryNextStep();
//                 }
//             });
//         }
//     }

//     [SerializeField] private Camera cam;
//     [SerializeField] private float newCameraOrthoSizeStepWearing = 26f;
//     [SerializeField] private float newCameraPositionYStepWearing = -15f;
//     private void OnStartStep2()
//     {
//         Sequence sequence = DOTween.Sequence();
//         sequence.Append(cam.DOOrthoSize(newCameraOrthoSizeStepWearing, 0.75f));
//         sequence.Join(cam.transform.DOLocalMoveY(newCameraPositionYStepWearing, 0.75f));

//         sequence.OnComplete(() =>
//         {
//             SnapObjectScrollUIController.Instance.Show();
//             Debug.Log("show");
//             LoadUI();
//         });
//     }
//     [SerializeField] private List<SnapObjectUI.SnapObjectUIConfig> dressItemSnapConfig;
//     private void LoadUI()
//     {
//         for (int i = 0; i < dressItemSnapConfig.Count; i++)
//         {
//             SnapObjectScrollUIController.Instance.AddData(dressItemSnapConfig[i]);
//             SnapObjectScrollUIController.Instance.ManualSetOrder(GetSequence(dressItemSnapConfig.Count, false));
//         }
//         for (int i = 0; i < dressItemSnapConfig.Count; i++)
//         {
//             var item = dressItemSnapConfig[i];

//             item.onSnap += () => SnapDress(item);
//         }


//     }
//     private int countSnapUI = 0;
//     private void SnapDress(SnapObjectUI.SnapObjectUIConfig item)
//     {
//         if (dressItemSnapConfig.Remove(item))
//         {
//             countSnapUI++;
//             item.onSnap = null;
//             item.onRelease = null;
//             item.onStartDrag = null;
//             if (dressItemSnapConfig.Count == 0)
//             {
//                 DoneStep();
//                 TryNextStep();
//             }
//             if (countSnapUI >= 5)
//             {
//                 AdsManager.Ins.ShowEndGame();
//             }
//         }
//     }
//     private List<int> GetSequence(int count, bool random)
//     {
//         List<int> result = new List<int>(count);

//         for (int i = 0; i < count; i++)
//             result.Add(i);

//         if (!random)
//             return result;

//         for (int i = count - 1; i > 0; i--)
//         {
//             int randomIndex = UnityEngine.Random.Range(0, i + 1);
//             (result[i], result[randomIndex]) = (result[randomIndex], result[i]);
//         }

//         return result;
//     }

//     [SerializeField] private SonTransitionPhase transitionPhase;
//     [SerializeField] private float newCameraOrthoSizeP4 = 45f;
//     [SerializeField] private float newCameraPositionYP4 = 10;
//     private void OnStartStep3()
//     {
//         StartCoroutine(IE_OnTrasionPhase1Phase1());
//     }
//     IEnumerator IE_OnTrasionPhase1Phase1()
//     {
//         yield return new WaitForSeconds(1f);
//         transitionPhase.TransitionToPhase(0, 1, () =>
//             {
//                 Sequence sequence = DOTween.Sequence();
//                 sequence.Append(cam.DOOrthoSize(newCameraOrthoSizeP4, 0.25f));
//                 sequence.Join(cam.transform.DOLocalMoveY(newCameraPositionYP4, 0.25f));

//             });
//         transitionPhase.onComplete.AddListener(() =>
//         {
//             DoneStep();
//             TryNextStep();
//         });
//     }
// }
