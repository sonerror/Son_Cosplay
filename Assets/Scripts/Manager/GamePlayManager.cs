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
  public CharacterControl You;
  public CharacterControl Rumi;

  public List<Transform> CollectSelected = new List<Transform>();
  public List<GameObject> IconSelected = new List<GameObject>();

  [SerializeField] private int currentStep = 0;
  public int CurrentStep => currentStep;

  void Start()
  {
    SetCollect(0);
  }

  void SetCollect(int id, bool usingTween = true)
  {
    currentStep = id;
    for (var i = 0; i < CollectSelected.Count; i++)
    {
      CollectSelected[i].gameObject.SetActive(i != id);
      IconSelected[i].SetActive(i != id);
    }

    if (usingTween)
    {
      var tfCollect = CollectSelected[id];
      tfCollect.localScale = Vector3.zero;
      tfCollect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
    else
    {
      CollectSelected[id].localScale = Vector3.one;
    }
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
      default:
        Debug.LogWarning("No more steps!");
        break;
    }
  }

  #region Step0

  private void OnStartStep0()
  {

  }

  private void OnEndStep0()
  {
    DoneStep();
    TryNextStep();
  }
  #endregion Step0

  #region Step1


  private void OnStartStep1()
  {

  }


  private void OnEndStep1()
  {

  }

  #endregion Step1

  #region Step2

  private void OnStartStep2()
  {

  }

  private void OnEndStep2()
  {

  }

  #endregion Step2

  #region Step3
  private void OnStartStep3()
  {
    AdsManager.Ins.ShowEndGame();
  }



  #endregion Step3
}