using System.Collections;
using System.Collections.Generic;
using HoangHH;
using UnityEngine;
using sonnv;
using UnityEngine.Scripting;
public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private bool canBlockShowHint = false;
    [SerializeField] private LevelControl levelBase;
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
    int countCollectFail = 0;
    private bool isTap = false;

    private void Update()
    {
        if (canBlockShowHint) return;
        if (Input.GetMouseButtonDown(0) && isTap == false)
        {
            isTap = true;
        }
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
    [SerializeField] private Transform tfBody;
    [SerializeField] private Transform tfHair;

    //step1
    [SerializeField] private List<Transform> listClothes = new List<Transform>();
    [SerializeField] private List<Transform> listThrowClothes = new List<Transform>();
    public List<Transform> ListClothes => listClothes;
    public List<Transform> ListThrowClothes => listThrowClothes;

    private void TutorialStep1(int indexStep)
    {
        if (handCtrl == null) return;
        if (indexStep >= listClothes.Count || indexStep >= listThrowClothes.Count) return;
        handCtrl.gameObject.SetActive(true);
        var tf = listClothes[indexStep];
        var tfTarget = listThrowClothes[indexStep];
        handCtrl.ShowHandPosToPos(tf.gameObject.transform.position, tfTarget.transform.position);
    }
    //step2


    [SerializeField] private Transform tfSprayBody;
    //step3
    [SerializeField] private bool isTriggerBox = false;
    [SerializeField] private Transform tfBox;
    [SerializeField] private Transform tfTagetBox;

    [SerializeField] private Transform tfHairInBox;
    [SerializeField] private Transform tfSprayHairGold;
    [SerializeField] private Transform tfHairColor;
    [SerializeField] private Transform tfGel;
    [SerializeField] private Transform tfEnd;
    [SerializeField] private Transform tfEye;


    public void SetIsTriggerBox()
    {
        isTriggerBox = true;
    }

    void ShowHint()
    {
        if (disableHand) return;
        enableCountTime = false;
        int index = gamePlayManager.CurrentStep;
        Debug.Log("index : " + index);
        switch (index)
        {
            case 0:
                return;
            case 1:
                TutorialStep1(0);
                return;
            case 2:
                handCtrlMakeup.gameObject.SetActive(true);
                handCtrlMakeup.ShowHandPosToPos(tfSprayBody.position, tfBody.position);

                return;
            case 3:
                if (isTriggerBox == false)
                {
                    handCtrlMakeup.gameObject.SetActive(true);
                    handCtrlMakeup.ShowHandPosToPos(tfBox.position, tfTagetBox.position);
                    return;
                }
                else
                {
                    handCtrlMakeup.gameObject.SetActive(true);
                    handCtrlMakeup.ShowHandPosToPos(tfHairInBox.position, tfHair.position);
                    return;
                }
                return;
            case 4:
                handCtrlMakeup.gameObject.SetActive(true);
                handCtrlMakeup.ShowHandPosToPos(tfSprayHairGold.position, tfHairColor.position);
                return;
            case 5:
                handCtrlMakeup.gameObject.SetActive(true);
                handCtrlMakeup.ShowHandPosToPos(tfGel.position, tfHairColor.position);
                return;
            case 6:
                handCtrlMakeup.gameObject.SetActive(true);
                handCtrlMakeup.ShowHandPosToPos(tfEnd.position, tfEye.position);
                return;
            case 7:
                // handCtrl.gameObject.SetActive(true);
                // handCtrl.ShowHandPosToPos(tfSpray.position, tfHair.position);
                return;
            default:
                resetTimeHint();
                return;
        }
    }
    public void SetStateIsTap(bool value)
    {
        isTap = value;
    }
    private void HideHint()
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