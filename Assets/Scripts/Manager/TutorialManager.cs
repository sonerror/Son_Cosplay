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

  [SerializeField] private List<Transform> tutorialNode = new List<Transform>();

  int countCollectFail = 0;

  public void OnCollectFail()
  {
    countCollectFail++;
    if (countCollectFail >= 3)
    {
      timeCountHint = 1f;
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
          case 8:
            PlayTut8();
            return;
          default:
            return;
        }
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
    var item = gamePlayManager.items[1] as Pimple;
    for (var i = 0; i < item.acnePimpleColliders.Count; i++)
    {
      if (item.acnePimpleColliders[i].gameObject.activeSelf)
      {
        handCtrl.ShowHandState2(item.Tf.position, item.acnePimpleColliders[i].Tf.position);
        return;
      }
    }

  }

  void PlayTut2()
  {
    var item = gamePlayManager.items[2] as PimpleHeal;
    for (var i = 0; i < item.acnePimpleColliders.Count; i++)
    {
      if (item.acnePimpleColliders[i].gameObject.activeSelf)
      {
        handCtrl.ShowHandState2(item.Tf.position, item.acnePimpleColliders[i].Tf.position);
        return;
      }
    }
  }

  void PlayTut3()
  {
    handCtrl.ShowHandAtPos(gamePlayManager.items[3].Tf.position);
  }

  void PlayTut4()
  {
    var item = gamePlayManager.items[4] as ItemUseOnce;
    handCtrl.ShowHandState2(item.Tf.position, item.targetInfos.targetTf.position);
  }
  void PlayTut5()
  {
    var item = gamePlayManager.items[5];
    handCtrl.ShowHandState2(item.Tf.position, item.Tf.position + Vector3.right * 2f);
  }
  void PlayTut6()
  {
    handCtrl.ShowHandAtPos(gamePlayManager.items[6].Tf.position);
  }
  void PlayTut7()
  {
    var item = gamePlayManager.items[7] as ItemUseOnce;
    handCtrl.ShowHandState2(item.Tf.position, item.targetInfos.targetTf.position);
  }
  void PlayTut8()
  {
    var item = gamePlayManager.items[8];
    handCtrl.ShowHandState2(item.Tf.position, item.Tf.position + Vector3.right * 2f);
  }

  void HideHint()
  {
    enableCountTime = true;
    handCtrl.HideHand();
  }

  public void resetTimeHint()
  {
    HideHint();

    timeCountHint = TimeHint;
  }
}
