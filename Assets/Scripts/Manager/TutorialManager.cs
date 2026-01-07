using System.Collections;
using System.Collections.Generic;
using HoangHH;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
  public bool disableHand = false;
  [SerializeField] float TimeHint = 5f;
  public bool enableCountTime = false;
  public float timeCountHint = 2f;
  [SerializeField] public HandCtrl handCtrl;
  [SerializeField] private GamePlayManager gamePlayManager;
  [SerializeField] private GameObject clock;
  private int CountStepDone = 0;
  public void OnStepDone()
  {
    CountStepDone++;
  }

  public void IncreaseTimeHide()
  {
    TimeHint = 5f;
    this.resetTimeHint();
  }

  [SerializeField] private List<Transform> tutorialNode = new List<Transform>();

  int countCollectFail = 0;

  public void OnCollectFail()
  {
    countCollectFail++;
    if (countCollectFail >= 3)
    {
      timeCountHint = 1.5f;
    }
  }

  public void OnCollectSuccess()
  {
    countCollectFail = 0;
    resetTimeHint();
  }

  private void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      if (handCtrl.gameObject.activeSelf)
      {
        HideHint();
        timeCountHint = 1.5f;
      }
      return;
    }

    if (Input.GetMouseButton(0)) return;
    if (!enableCountTime) return;
    if (handCtrl.gameObject.activeSelf) return;
    if (clock.activeSelf) return;
    CalculateTimeHint();
  }
  private void CalculateTimeHint()
  {
    timeCountHint -= Time.deltaTime;
    if (timeCountHint <= 0)
    {
      ShowHint();
    }
  }

  void ShowHint()
  {
    if (disableHand) return;

    enableCountTime = false;
    handCtrl.gameObject.SetActive(true);

    int index = gamePlayManager.CurrentStep;

    switch (index)
    {
      case 0:
        PlayTut0();
        return;
      case 1:
        PlayTut1();
        return;
      case 2:
        PlayTut2();
        return;
      case 3:
        PlayTut3();
        return;
      case 4:
        PlayTut4();
        return;
      case 5:
        PlayTut5();
        return;
      case 6:
        PlayTut6();
        return;
      case 7:
        PlayTut7();
        return;
      case 8:
        PlayTut8();
        return;
      case 9:
        PlayTut9();
        return;
      case 10:
        PlayTut10();
        return;
      default:
        handCtrl.gameObject.SetActive(false);
        resetTimeHint();
        return;
    }
  }
  void PlayTut0()
  {
    handCtrl.ShowHandPosToPos(tutorialNode[0].position, tutorialNode[0].position + Vector3.right * 3f);
  }

  void PlayTut1()
  {
    var item = gamePlayManager.items[1];
    handCtrl.ShowHandAtPos(item.Tf.position);
  }

  void PlayTut2()
  {

    var item = gamePlayManager.items[2] as FoundationBottle;
    handCtrl.ShowHandPosToPos(item.Tf.position, item.getPosTargetActive());

  }

  void PlayTut3()
  {
    var item = gamePlayManager.items[3];
    handCtrl.ShowHandAtPos(item.Tf.position);
  }

  void PlayTut4()
  {
    var item = gamePlayManager.items[4] as FoundationBottle;
    handCtrl.ShowHandPosToPos(item.Tf.position, item.getPosTargetActive());
  }
  void PlayTut5()
  {
    var item = gamePlayManager.items[5] as BrushBlushWithMaskGroup;
    if (item.CurrBrushBlushState == BrushBlushState.Drag)
    {
      handCtrl.ShowHandPosToPos(item.Tf.position, item.BoxFlour.Tf.position);
      return;
    }
    if (item.CurrBrushBlushState == BrushBlushState.DragWithPainter)
    {
      handCtrl.ShowHandPosToPos(item.Tf.position, item.FlourFill.Tf.position);
      return;
    }
  }
  void PlayTut6()
  {
    var item = gamePlayManager.items[6] as FaceTowel;
    handCtrl.ShowHandPosToPos(item.Tf.position, item.maskGroup.GetTranformOfMaskNotActive().position);

  }
  void PlayTut7()
  {
    var item = gamePlayManager.items[7];
    handCtrl.ShowHandAtPos(item.Tf.position);
  }

  void PlayTut8()
  {
    var item = gamePlayManager.items[8] as FoundationBottle;
    handCtrl.ShowHandPosToPos(item.Tf.position, item.getPosTargetActive());
  }

  void PlayTut9()
  {
    var item = gamePlayManager.items[9] as HairBang;
    handCtrl.ShowHandPosToPos(item.Tf.position, item.targetInfos.targetTf.position);
  }

  void PlayTut10()
  {
    for (int index1 = 10; index1 < 16; index1++)
    {
      if (gamePlayManager.items[index1].IsReady)
      {
        var item = gamePlayManager.items[index1] as HairBang;
        handCtrl.ShowHandPosToPos(item.Tf.position, item.targetInfos.targetTf.position);
      }
      return;
    }
  }

  void HideHint()
  {
    enableCountTime = true;
    handCtrl.HideHand();
  }

  public void resetTimeHint()
  {
    HideHint();

    timeCountHint = countCollectFail >= 3 ? 1.5f : TimeHint;
  }
}
