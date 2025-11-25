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

    var items = gamePlayManager.items;
    enableCountTime = false;
    handCtrl.gameObject.SetActive(true);

    var i = gamePlayManager.CurrState;
    if (i == 0)
    {
      var item = items[i] as HairBang;
      handCtrl.ShowHandState2(item.Tf.position, item.targetInfos.targetTf.position);
      return;
    }
    if (i == 1)
    {
      var item = items[i];
      handCtrl.ShowHandState2(item.Tf.position, tutorialNode[0].position);
      return;
    }
    if (i == 2)
    {
      var item = items[i];
      handCtrl.ShowHandState2(item.Tf.position, tutorialNode[1].position);
      return;
    }
    if (i == 3)
    {
      var item = items[i] as Oil;
      for (int j = 0; j < item.targetInfos.Count; j++)
      {
        if (item.targetInfos[j].hadChecked) continue;
        handCtrl.ShowHandState2(item.Tf.position, item.targetInfos[j].targetTf.position);
        return;
      }
    }


    handCtrl.gameObject.SetActive(false);
    resetTimeHint();
  }

  void HideHint()
  {
    enableCountTime = true;
    handCtrl.HideHand();
  }

  public void resetTimeHint()
  {
    HideHint();

    timeCountHint = gamePlayManager.CurrState < 2 ? 1f : TimeHint;
  }
}
