using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
  public bool disableHand = false;
  [SerializeField] float TimeHint = 5f;
  public bool enableCountTime = false;
  public float timeCountHint = 2f;
  [SerializeField] public HandCtrl handCtrl;
  [SerializeField] private GamePlayManager gamePlayManager;

  private int CountStepDone = 0;
  public void OnStepDone()
  {
    CountStepDone++;
  }

  public void IncreaseTimeHide()
  {
    TimeHint = 5f;
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

    var items = gamePlayManager.items;

    for (var index = 0; index < items.Count; index++)
    {
      if (items[index].IsReady)
      {
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
          default:
            return;
        }
      }

      if (index == items.Count - 1)
      {
        PlayTut7();
        return;
      }

    }

    handCtrl.gameObject.SetActive(false);
    resetTimeHint();
  }
  void PlayTut0()
  {
    var item = gamePlayManager.items[0] as FoundationBottle;
    handCtrl.ShowHandState2(item.Tf.position, item.getPosTargetActive());
  }

  void PlayTut1()
  {
    var item = gamePlayManager.items[1];
    handCtrl.ShowHandState2(item.Tf.position, tutorialNode[0].position);
  }

  void PlayTut2()
  {
    var item = gamePlayManager.items[2] as FoundationBottle;
    handCtrl.ShowHandState2(item.Tf.position, item.getPosTargetActive());
  }

  void PlayTut3()
  {
    var item = gamePlayManager.items[3] as Brush_Blush;
    if (item.CurrBrushBlushState == BrushBlushState.Drag)
    {
      handCtrl.ShowHandState2(item.Tf.position, item.hopPhanColliders.transform.position);
    }
    if (item.CurrBrushBlushState == BrushBlushState.DragWithPainter)
    {
      handCtrl.ShowHandState2(item.Tf.position, item.GetPosTargetActive());
    }

  }

  void PlayTut4()
  {
    var item = gamePlayManager.items[4] as FoundationBottle;
    handCtrl.ShowHandState2(item.Tf.position, item.getPosTargetActive());
  }
  void PlayTut5()
  {
    handCtrl.ShowHandAtPos(tutorialNode[2].position);
  }
  void PlayTut6()
  {
    var item = gamePlayManager.items[6] as Brush_Eye;

    if (item.CurrBrushEyeState == BrushEyeState.None)
    {
      handCtrl.ShowHandState2(item.Tf.position, tutorialNode[4].position);
    }

    if (item.CurrBrushEyeState == BrushEyeState.Blue)
    {
      if (!item.MaskGroupBlue.IsDone())
      {
        handCtrl.ShowHandState2(item.Tf.position, item.MaskGroupBlue.Tf.position);
      }
      else
      {
        handCtrl.ShowHandState2(item.Tf.position, tutorialNode[4].position);
      }
    }
    if (item.CurrBrushEyeState == BrushEyeState.Pink)
    {
      if (!item.MaskGroupPink.IsDone())
      {
        handCtrl.ShowHandState2(item.Tf.position, item.MaskGroupPink.Tf.position);
      }
      else
      {
        handCtrl.ShowHandState2(item.Tf.position, tutorialNode[3].position);
      }
    }
  }
  void PlayTut7()
  {
    var item = gamePlayManager.items[7];
    handCtrl.ShowHandState2(item.Tf.position, tutorialNode[1].position);
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
