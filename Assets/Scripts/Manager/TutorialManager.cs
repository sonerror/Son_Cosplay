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
    [SerializeField] public HandCtrl handCtrlMakeup;
    [SerializeField] private StepManager gamePlayManager;
    [SerializeField] private int countHintStep1 = 0;
    public int CountHintStep1
    {
        get { return countHintStep1; }
        set { countHintStep1 = value; }
    }
    public void IncreaseCountHintStep1()
    {
        countHintStep1++;
    }
    private int CountStepDone = 0;

    [SerializeField] private List<Transform> tutorialNode = new List<Transform>();
    [SerializeField] private List<Transform> tfItem = new List<Transform>();
    public List<Transform> TutorialNode => tutorialNode;
    public List<Transform> TfItem => tfItem;
    [SerializeField] private Transform tf1;
    [SerializeField] private Transform tf2;

    int countCollectFail = 0;
    private bool isTap = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HideHint();
            timeCountHint = countCollectFail >= 3 ? 1.5f : TimeHint;
            enableCountTime = true;
            return;
        }
        if (Input.GetMouseButton(0))
        {
            return;
        }
        if (!enableCountTime) return;
        if (handCtrl.gameObject.activeSelf) return;
        if (handCtrlMakeup.gameObject.activeSelf) return;
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
                return;
            case 1:
                TutorialStep1(0);
                return;
            case 2:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tf1.position, tf2.position);
                return;
            default:
                resetTimeHint();
                return;
        }
    }



    private void Tutorial(int indexStep)
    {

    }
    private void TutorialStep1(int indexStep)
    {
        if (handCtrl == null) return;
        if (indexStep >= tfItem.Count || indexStep >= tutorialNode.Count) return;
        handCtrl.gameObject.SetActive(true);
        var obj = tfItem[indexStep];
        var node = tutorialNode[indexStep];
        handCtrl.ShowHandPosToPos(node.position, obj.position);
    }
    public void SetStateIsTap(bool value)
    {
        isTap = value;
    }
    void HideHint()
    {
        handCtrl.gameObject.SetActive(false);
        handCtrlMakeup.gameObject.SetActive(false);
    }
    public void resetTimeHint()
    {
        HideHint();
        timeCountHint = countCollectFail >= 3 ? 1.5f : TimeHint;
        enableCountTime = true;
    }
    public void OnStepDone()
    {
        CountStepDone++;
    }
    public void IncreaseTimeHide()
    {
        TimeHint = 5f;
        resetTimeHint();
    }
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
    public void SetNewTime(float timer)
    {
        TimeHint = timer;
        timeCountHint = timer;
    }

}