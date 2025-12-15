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

  [SerializeField] private TapOpener lidTapOpener;
  private GameObject clock;
  private int CountStepDone = 0;
  public void OnStepDone()
  {
    CountStepDone++;
  }


  void Start()
  {
    clock = GameManager.Ins.clockTimer.gameObject;
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
      // case 6:
      //   PlayTut6();
      //   return;
      // case 7:
      //   PlayTut7();
      //   return;
      default:
        handCtrl.gameObject.SetActive(false);
        resetTimeHint();
        return;
    }
  }
  void PlayTut0()
  {
    var item = gamePlayManager.items[0] as HairBang;
    handCtrl.ShowHandPosToPos(item.Tf.position, item.targetInfos.targetTf.position);
  }

  void PlayTut1()
  {
    var item = gamePlayManager.items[1] as TapJumpOpener;
    handCtrl.ShowHandAtPos(item.Tf.position);
  }

  void PlayTut2()
  {
    var item = gamePlayManager.items[2] as BrushBlushWithMaskGroup;
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

  void PlayTut3()
  {
    var item = gamePlayManager.items[3] as FaceTowelWithVfx;
    handCtrl.ShowHandPosToPos(item.Tf.position, item.MaskGroup.GetTranformOfMaskNotActive().position);
  }

  void PlayTut4()
  {
    var item = gamePlayManager.items[4] as FaceTowelWithVfx;
    handCtrl.ShowHandPosToPos(item.Tf.position, item.MaskGroup.GetTranformOfMaskNotActive().position);
  }
  void PlayTut5()
  {
    handCtrl.ShowHandPosToPos(tutorialNode[0].position, tutorialNode[1].position);
    AdsManager.Ins.ShowEndGame();
  }
  // void PlayTut6()
  // {
  //   var item = gamePlayManager.items[6] as FoundationBottleWithFx;
  //   handCtrl.ShowHandPosToPos(item.Tf.position, item.getPosTargetActive());

  // }
  // void PlayTut7()
  // {
  //   var item = gamePlayManager.items[7] as FoundationBottleWithFx;
  //   handCtrl.ShowHandPosToPos(item.Tf.position, item.getPosTargetActive());
  // }

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
