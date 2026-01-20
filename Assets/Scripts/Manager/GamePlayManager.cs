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
  // public FloatingItem1 floatingItem;

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
    Debug.Log("Fx step" + currentStep);
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
      case 8:
        OnStartStep8();
        break;
      default:
        Debug.LogWarning("No more steps!");
        break;
    }
  }

  #region Step0
  public List<RemoveItem> ClotherRemove = new List<RemoveItem>();
  private int countRemoveClother = 0;
  private void OnStartStep0()
  {
    ClotherRemove[0].SetReady();
    ClotherRemove[1].SetReady();
    ClotherRemove[2].SetReady();
    ClotherRemove[3].SetReady();
    for (int i = 0; i < ClotherRemove.Count; i++)
    {
      ClotherRemove[i].OnFinish.AddListener(OnRemoveClother);
    }
  }

  void OnRemoveClother()
  {
    countRemoveClother++;
    TutorialManager.Ins.IncreaseTimeHide();
    if (countRemoveClother >= ClotherRemove.Count)
    {
      OnEndStep0();
    }
  }

  private void OnEndStep0()
  {
    DoneStep();
    TryNextStep();
  }
  #endregion Step0

  #region Step1

  public List<ControlAlphaSlot> ControlAlphaSlot = new List<ControlAlphaSlot>();

  private void OnStartStep1()
  {
    items[1].gameObject.SetActive(true);
    items[1].SetReady();

    for (int i = 0; i < ControlAlphaSlot.Count; i++)
    {
      ControlAlphaSlot[i].OnFinish.AddListener(OnFinishShowSlot);
      ControlAlphaSlot[i].SetReady(character);
    }
  }

  private int countShowSlot = 0;
  void OnFinishShowSlot()
  {
    Debug.Log("OnFinishShowSlot" + countShowSlot);
    countShowSlot++;
    if (countShowSlot == ControlAlphaSlot.Count - 1)
    {
      OnEndStep1();
    }
  }

  private void OnEndStep1()
  {
    DoneStep();
    DOVirtual.DelayedCall(2f, () =>
    {
      GameManager.Ins.ChangeSceneToGamePlay2(() =>
      {
        TryNextStep();
      });
    });
  }

  #endregion Step1

  #region Step2

  private void OnStartStep2()
  {
    var item = items[2];
    item.OnFinish.AddListener(OnEndStep2);
    item.SetReady();
    item.SetBlockItem(false);
  }

  private void OnEndStep2()
  {
    var item = items[2];
    item.OnFinish.RemoveListener(OnEndStep2);
    item.SetBlockItem(true);

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

  private void OnStartStep4()
  {
    items[4].OnFinish.AddListener(OnEndStep4);
    items[4].SetReady();
  }
  private void OnEndStep4()
  {
    items[4].OnFinish.RemoveListener(OnEndStep4);
    items[4].IsReady = false;

    DOVirtual.DelayedCall(0.5f, () =>
    {
      items[4].Tf.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
      {
        items[4].gameObject.SetActive(false);
      });
    });


    DoneStep();
    TryNextStep();
  }

  #endregion Step4

  #region Step5

  private void OnStartStep5()
  {
    items[5].OnFinish.AddListener(OnEndStep5);
    items[5].SetReady();
  }

  private void OnEndStep5()
  {
    items[5].OnFinish.RemoveListener(OnEndStep5);
    items[5].IsReady = false;

    DoneStep();
    TryNextStep();
  }
  #endregion Step5

  #region Step6
  private void OnStartStep6()
  {
    items[6].OnFinish.AddListener(OnEndStep6);
    items[6].SetReady();
  }

  private void OnEndStep6()
  {
    items[6].OnFinish.RemoveListener(OnEndStep6);
    items[6].SetReady();

    DoneStep();
    TryNextStep();
  }
  #endregion Step6

  #region Step7
  private void OnStartStep7()
  {
    AdsManager.Ins.ShowEndGame();
  }

  private void OnEndStep7()
  {
    items[7].OnFinish.RemoveListener(OnEndStep7);
    items[7].IsReady = false;

    DoneStep();
    TryNextStep();
  }

  #endregion Step7


  #region Step8
  private void OnStartStep8()
  {
    AdsManager.Ins.ShowEndGame();
  }

  #endregion Step8


}