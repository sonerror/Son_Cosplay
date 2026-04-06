using System.Collections;
using System.Collections.Generic;
using HoangHH;
using UnityEngine;
using sonnv;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private LevelControl levelBase;
    public bool disableHand = false;
    [SerializeField] float TimeHint = 5f;
    public bool enableCountTime = false;
    public float timeCountHint = 2f;
    [SerializeField] public HandCtrl handCtrl;
    [SerializeField] public HandCtrl handCtrlMakeup;
    [SerializeField] public HandCanvasCtrl handCtrlCanvas;
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
        if (handCtrlCanvas.gameObject.activeSelf) return;
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
    [SerializeField] private bool isTapGel = false;
    public void ChangeStateIsTapGel(bool value)
    {
        isTapGel = value;
    }
    [SerializeField] private Transform tfHair;

    [SerializeField] private Transform tfWaterFaucet;
    [SerializeField] private Transform tfShampoo;
    [SerializeField] private Transform tfBrush;
    [SerializeField] private Transform tfStand;


    [SerializeField] private Transform tfGel;
    [SerializeField] private Transform tfGelThrow;
    [SerializeField] private Transform tf1;
    [SerializeField] private Transform tf2;

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
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfWaterFaucet.position, tfHair.position);
                return;
            case 2:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfShampoo.position, tfHair.position);
                return;
            case 3:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfBrush.position, tfHair.position);
                return;
            case 4:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfWaterFaucet.position, tfHair.position);
                return;
            case 5:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfStand.position, tfHair.position);
                return;
            case 6:
                if (isTapGel == false)
                {
                    handCtrl.gameObject.SetActive(true);
                    handCtrl.ShowHandPosToPos(tfGel.position, tfGelThrow.position);
                }
                else
                {
                    handCtrl.gameObject.SetActive(true);
                    handCtrl.ShowHandPosToPos(tfGel.position, tfHair.position);
                }
                return;
            case 7:
                TutorialStep(0);
                return;
            case 8:
                ScrollObjectInfo info = SnapObjectScrollUIController.Instance
                    .GetFirstScrollObjectInfo();
                if (info == null) return;
                if (info.itemRT == null) return;
                if (info.snapPointTf == null) return;
                handCtrlCanvas.Show(info.itemRT, info.snapPointTf);
                return;
            default:
                resetTimeHint();
                return;
        }
    }

    private void TutorialStep(int indexStep)
    {
        if (handCtrlMakeup == null) return;
        if (indexStep >= tfItem.Count || indexStep >= tutorialNode.Count) return;
        handCtrlMakeup.gameObject.SetActive(true);
        var obj = tfItem[indexStep];
        var node = tutorialNode[indexStep];
        handCtrlMakeup.ShowHandPosToPos(node.position, obj.position);
    }
    public void SetStateIsTap(bool value)
    {
        isTap = value;
    }
    void HideHint()
    {
        handCtrl.gameObject.SetActive(false);
        handCtrlMakeup.gameObject.SetActive(false);
        handCtrlCanvas.gameObject.SetActive(false);
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