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
    //Step 1
    //[SerializeField] private Transform tfHair;
    //[SerializeField] private Transform tfHairL;
    //[SerializeField] private Transform tfHairR;
    //[SerializeField] private Transform tfClipper;
    ////step2
    //[SerializeField] private Transform tfBom;
    //[SerializeField] private Transform tfBall;
    ////step3
    //[SerializeField] private Transform tfhandBom;
    //[SerializeField] private Transform tfBomTarget;
    ////step4
    //[SerializeField] private bool isTriggerColor = false;
    //public void SetStateIsTriger()
    //{
    //    isTriggerColor = true;
    //}
    //[SerializeField] private Transform tfBrush;
    //[SerializeField] private Transform tfColor;
    //[SerializeField] private Transform tfItemStep3;
    ////step8
    //[SerializeField] private Transform tfItemInBody;
    //[SerializeField] private Transform tfItemInBodyTarget;

    //step1
    [SerializeField] private List<Transform> listClothes;
    public List<Transform> ListClothes => listClothes;
    [SerializeField] private List<Transform> listThrowClothes;
    public List<Transform> ListThrowClothes => listThrowClothes;
    //step2
    [SerializeField] private Transform tfSpray;
    [SerializeField] private Transform tfBody;
    //step3
    [SerializeField] private List<Transform> listObjEye;
    public List<Transform> ListObjEye => listObjEye;
    [SerializeField] private List<Transform> listPointEye;
    public List<Transform> ListPointEye => listPointEye;
    //step4
    [SerializeField] private Transform tffoundation;
    [SerializeField] private Transform tfHead;
    //step5
    [SerializeField] private Transform tfSponge;

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
                SetTut();
                return;
            case 2:
                handCtrlMakeup.gameObject.SetActive(true);
                handCtrlMakeup.ShowHandPosToPos(tfSpray.position, tfBody.position);

                return;
            case 3:
                if (listObjEye.Count >= 0 && listPointEye.Count >= 0)
                {
                    handCtrl.gameObject.SetActive(true);
                    handCtrl.ShowHandPosToPos(listObjEye[0].position, listPointEye[0].position);
                }
                return;

            case 4:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tffoundation.position, tfHead.position);
                return;
            case 5:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfSponge.position, tfHead.position);
                return;
            case 6:
                // handCtrlMakeup.gameObject.SetActive(true);
                // handCtrlMakeup.ShowHandPosToPos(tfBrush.position, tfItemStep3.position);
                return;
            case 7:
                // handCtrlMakeup.gameObject.SetActive(true);
                // handCtrlMakeup.ShowHandPosToPos(tfItemInBody.position, tfItemInBodyTarget.position);
                return;
            default:
                resetTimeHint();
                return;
        }
    }
    private void SetTut()
    {
        if (listClothes.Count >= 0 && listThrowClothes.Count >= 0)
        {
            handCtrlMakeup.gameObject.SetActive(true);
            handCtrlMakeup.ShowHandPosToPos(listClothes[0].position, listThrowClothes[0].position);
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