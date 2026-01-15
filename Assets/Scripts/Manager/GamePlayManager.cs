using System;
using System.Collections.Generic;
using System.Linq;
using AnhPD.Cook;
using DG.Tweening;
using HoangHH;
using Sirenix.OdinInspector;
// using Unity.VisualScripting;
using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
  public CharacterControl character;
  // public ShowObjectEffect Gr1, Gr2;
  public FloatingItem1 floatingItem;

  public List<Item> items = new List<Item>();

  // public void ChangeGroupItemStart()
  // {
  //   Debug.Log("ChangeGroupItemStart ");
  //   Gr1.Hide(0.5f);
  //   Gr2.Show(1f);
  // }

  [SerializeField] private int currentStep = 0;
  public int CurrentStep => currentStep;

  protected void TurnCharacterSlotAttachment(List<SlotAttachmentPair> slotDataList, bool attached)
  {
    for (int i = 0; i < slotDataList.Count; i++)
    {
      character.TurnSlotAttachment(slotDataList[i].slotName,
          attached ? slotDataList[i].attachmentName : null);
    }
  }

  void Start()
  {
    StartStep();
  }

  public void SetStep(int step)
  {
    currentStep = step;
    StartStep();
  }

  void DoneStep()
  {
    GameManager.Ins.PlayPositiveEmoji();
  }

  void TryNextStep()
  {
    currentStep++;
    Debug.LogWarning("nextto" + currentStep);
    StartStep();
  }

#if UNITY_EDITOR
  [Button]
#endif
  public void StartStep()
  {
    switch (currentStep)
    {
      case 0:
        OnStartStep0();
        break;
      case 1:
        OnStartStep1();
        break;
      case 2:
        OnStartStep2();
        break;
      case 3:
        OnStartStep3();
        break;
      case 4:
        OnStartStep4();
        break;
      case 5:
        OnStartStep5();
        break;
      case 6:
        OnStartStep6();
        break;
      case 7:
        OnStartStep7();
        break;
      default:
        Debug.LogWarning("No more steps!");
        break;
    }
  }

  #region Step0

  private void OnStartStep0()
  {
    items[0].SetReady();
    items[0].OnFinish.AddListener(OnEndStep0);
  }

  private void OnEndStep0()
  {
    DoneStep();
    TryNextStep();
    TutorialManager.Ins.IncreaseTimeHide();
    floatingItem.Show();
  }
  #endregion Step0

  #region Step1
  private void OnStartStep1()
  {
    var item = items[1];
    item.SetReady();
    item.OnFinish.AddListener(OnEndStep1);
  }

  private void OnEndStep1()
  {
    var item = items[1];
    item.OnFinish.RemoveListener(OnEndStep1);

    DOVirtual.DelayedCall(1f, () =>
    {
      GameManager.Ins.ChangeSceneToGamePlay2(() =>
      {
        DoneStep();
        TryNextStep();
      });
    });
  }

  #endregion Step1

  #region Step2

  [SerializeField] private List<SlotAttachmentPair> slotStep2 = new List<SlotAttachmentPair>();


  private void OnStartStep2()
  {
    var item = items[2];
    item.OnFinish.AddListener(OnEndStep2);
    item.SetReady();
  }

  private void OnEndStep2()
  {
    var item = items[2];
    item.OnFinish.RemoveListener(OnEndStep2);
    item.gameObject.SetActive(false);
    character.TurnOnSlotsAttachment(slotStep2);
    SoundManager.Ins.PlayFx(FxType.Drop);


    DoneStep();
    TryNextStep();
  }

  #endregion Step2

  #region Step3
  private void OnStartStep3()
  {
    Debug.Log("OnStartStep3");
    var item = items[3];
    item.OnFinish.AddListener(OnEndStep3);
    item.SetReady();
  }

  private void OnEndStep3()
  {
    var item = items[3];
    item.OnFinish.RemoveListener(OnEndStep3);
    item.IsReady = false;

    DoneStep();
    TryNextStep();
  }

  #endregion Step3

  #region Step4

  public void AddEyeL()
  {
    character.TurnSlotAttachment("Phase1-lensL", "Phase1/lens_Pupil");
    character.TurnSlotAttachment("Base_Pupil2", null);
  }

  public void AddEyeR()
  {
    character.TurnSlotAttachment("Base_Pupil", null);
    character.TurnSlotAttachment("Set-P1-lensR", "Phase1/lens_Pupil");
  }

  private void OnStartStep4()
  {
    Debug.Log("OnStartStep4");
    items[4].OnFinish.AddListener(CheckEndStep4);
    items[4].SetReady();
    items[5].OnFinish.AddListener(CheckEndStep4);
    items[5].SetReady();
  }

  void CheckEndStep4()
  {
    SoundManager.Ins.PlayFx(FxType.AddEye);
    if (!items[4].IsReady && !items[5].IsReady)
    {
      OnEndStep4();
    }
  }

  private void OnEndStep4()
  {
    DoneStep();
    TryNextStep();
  }

  #endregion Step4

  #region Step5

  private void OnStartStep5()
  {
    items[6].OnFinish.AddListener(OnEndStep5);
    items[6].SetReady();
  }

  private void OnEndStep5()
  {
    items[6].OnFinish.RemoveListener(OnEndStep5);
    items[6].IsReady = false;

    DoneStep();
    TryNextStep();
  }
  #endregion Step5

  #region Step6
  private void OnStartStep6()
  {
    items[7].OnFinish.AddListener(OnEndStep6);
    items[7].SetReady();
  }

  private void OnEndStep6()
  {
    items[7].OnFinish.RemoveListener(OnEndStep6);
    items[7].SetReady();

    DoneStep();
    TryNextStep();
  }
  #endregion Step6

  #region Step7
  private void OnStartStep7()
  {
    AdsManager.Ins.ShowEndGame();
  }

  #endregion Step7


}