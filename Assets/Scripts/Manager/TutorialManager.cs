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



    //step1
    [SerializeField] private Transform tfGlass;
    [SerializeField] private Transform tfGlassThrow;
    //step2
    [SerializeField] private List<Transform> listObjEye;
    public List<Transform> ListObjEye => listObjEye;
    [SerializeField] private List<Transform> listPointEye;
    public List<Transform> ListPointEye => listPointEye;
    //step3
    [SerializeField] private Transform tfWig;
    [SerializeField] private Transform tfWigThrow;
    //step4
    [SerializeField] private Transform tfHead;
    [SerializeField] private Transform tfBrush;
    [SerializeField] private Transform tfBoxCont;
    //step5
    [SerializeField] private Transform tfBoxHighligh;
    //step6
    [SerializeField] private Transform tfBrush2;
    [SerializeField] private Transform tfBoxPowdwer;
    //step7
    [SerializeField] private Transform tfBrush3;
    [SerializeField] private Transform tfBoxPhan;
    //step8
    [SerializeField] private Transform tfEyeLiner;
    [SerializeField] private Transform tfEye;
    //step9
    [SerializeField] private List<Transform> listObjEyelash;
    public List<Transform> ListObjEyeLash => listObjEyelash;
    [SerializeField] private List<Transform> listPointEyeLash;
    public List<Transform> ListPointEyeLash => listPointEyeLash;
    //step10
    [SerializeField] private Transform tfBrushBrow;
    [SerializeField] private Transform tfBrow;

    //step11
    [SerializeField] private Transform tfPen;
    [SerializeField] private Transform tfPenTarget;
    //step13
    [SerializeField] private Transform tfLipStick;
    [SerializeField] private Transform tfMouth;
    //step14
    [SerializeField] private Transform tfHair;
    [SerializeField] private Transform tfSnapHair;
    //step14
    [SerializeField] private Transform tfShirt;
    [SerializeField] private Transform tfShirtSnap;
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
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfGlass.position, tfGlassThrow.position);
                return;
            case 2:
                if (listObjEye.Count >= 0 && listPointEye.Count >= 0)
                {
                    handCtrl.gameObject.SetActive(true);
                    handCtrl.ShowHandPosToPos(listObjEye[0].position, listPointEye[0].position);
                }
                return;
            case 3:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfWig.position, tfWigThrow.position);
                return;
            case 4:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPosToPos(tfBrush.position, tfBoxCont.position, tfHead.position);
                return;
            case 5:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPosToPos(tfBrush.position, tfBoxHighligh.position, tfHead.position);
                return;
            case 6:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPosToPos(tfBrush2.position, tfBoxPowdwer.position, tfHead.position);
                return;
            case 7:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPosToPos(tfBrush3.position, tfBoxPhan.position, tfHead.position);
                return;
            case 8:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfEyeLiner.position, tfEye.position);
                return;
            case 9:
                if (listObjEye.Count >= 0 && listPointEye.Count >= 0)
                {
                    handCtrl.gameObject.SetActive(true);
                    handCtrl.ShowHandPosToPos(listObjEyelash[0].position, listPointEyeLash[0].position);
                }
                return;
            case 10:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfBrushBrow.position, tfBrow.position);
                return;
            case 11:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfPen.position, tfPenTarget.position);
                return;
            case 12:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfLipStick.position, tfMouth.position);
                return;
            case 13:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfHair.position, tfSnapHair.position);
                return;
            case 14:
                handCtrlMakeup.gameObject.SetActive(true);
                handCtrlMakeup.ShowHandPosToPos(tfShirt.position, tfShirtSnap.position);
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