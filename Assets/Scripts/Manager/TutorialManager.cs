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
    //step2


    [SerializeField] private Transform tfSprayBody;
    [SerializeField] private bool isTriggerBox = false;

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

                return;
            case 2:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfSprayBody.position, tfBody.position);
                // if (isTriggerBox == false)
                // {
                //     handCtrl.gameObject.SetActive(true);
                //     handCtrl.ShowHandPosToPos(tfBoxColor.position, tfBoxColor.position);
                //     return;
                // }
                // else
                // {
                //     handCtrl.gameObject.SetActive(true);
                //     handCtrl.ShowHandPosToPos(tfBoxColor.position, tfBoxColor.position);
                //     return;
                // }
                return;
            case 3:
                // handCtrl.gameObject.SetActive(true);
                // handCtrl.ShowHandPosToPosToPos(tfMacara.position, tfEyeL.position, tfEyeR.position);
                return;
            case 4:
                // handCtrl.gameObject.SetActive(true);
                // handCtrl.ShowHandPosToPos(tfLipStick.position, tfMouth.position);
                return;
            case 5:
                // handCtrl.gameObject.SetActive(true);
                // handCtrl.ShowHandPosToPos(tfHandBand.position, tfHandBandThrow.position);
                return;
            case 6:
                // handCtrl.gameObject.SetActive(true);
                // handCtrl.ShowHandPosToPos(tfCut.position, tfHairCut.position);
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