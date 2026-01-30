using System.Collections;
using System.Collections.Generic;
using HoangHH;
using UnityEngine;
using sonnv;
public class TutorialManager : Singleton<TutorialManager>
{
    public bool disableHand = false;
    [SerializeField] float TimeHint = 5f;
    public bool enableCountTime = false;
    public float timeCountHint = 2f;
    [SerializeField] public HandCtrl handCtrl;
    [SerializeField] private LevelMaleficent gamePlayManager;
    // [SerializeField] private GameObject clock;
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
    public List<Transform> TutorialNode => tutorialNode;
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
        // if (clock.activeSelf) return;
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
        int index = gamePlayManager.CurrentStep;

        switch (index)
        {
            case 0:
                handCtrl.gameObject.SetActive(true);
                Tutorial();
                return;
            case 1:
                return;
            default:
                handCtrl.gameObject.SetActive(false);
                resetTimeHint();
                return;
        }
    }
    private void Tutorial()
    {
        var obj = gamePlayManager.ListThrowObject[0];
        var node = tutorialNode[0];
        handCtrl.ShowHandPosToPos(obj.Tf.position, node.position);
        if (gamePlayManager.ListThrowObject.Count <= 0)
        {
            handCtrl.gameObject.SetActive(false);
            resetTimeHint();
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
