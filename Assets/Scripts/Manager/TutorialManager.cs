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
    // [SerializeField] private List<Transform> listTfClothes = new List<Transform>();
    // [SerializeField] private List<Transform> listTfTargetClothes = new List<Transform>();
    // public List<Transform> ListTfClothes => listTfClothes;
    // public List<Transform> ListTfTargetClothes => listTfTargetClothes;

    // [SerializeField] private Transform tfWigHair;
    // [SerializeField] private Transform tfHair;

    // [SerializeField] private Transform tfBody;
    // [SerializeField] private Transform tfSpay1;
    // [SerializeField] private Transform tfSpay2;


    // [SerializeField] private List<Transform> listTfSticker = new List<Transform>();
    // [SerializeField] private List<Transform> listTfTargetSticker = new List<Transform>();
    // public List<Transform> ListTfSticker => listTfSticker;
    // public List<Transform> ListTfTargetSticker => listTfTargetSticker;


    // [SerializeField] private Transform tfLens;
    // [SerializeField] private Transform tfEyes;
    [SerializeField] private Transform tfBoxColor;
    [SerializeField] private Transform tfBrush;
    [SerializeField] private Transform tfFace;
    [SerializeField] private Transform tfEyeL;
    [SerializeField] private Transform tfEyeR;

    [SerializeField] private Transform tfMacara;
    [SerializeField] private Transform tfLipStick;
    [SerializeField] private Transform tfMouth;

    [SerializeField] private Transform tfHandBand;
    [SerializeField] private Transform tfHandBandThrow;

    [SerializeField] private Transform tfCut;
    [SerializeField] private Transform tfHairCut;

    [SerializeField] private Transform tfSpray;
    [SerializeField] private Transform tfHair;

    [SerializeField] private Transform tfDecoHair;
    [SerializeField] private Transform tfDecoTargetHair;

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
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfBoxColor.position, tfBoxColor.position);
                return;
            case 2:
                if (isTriggerBox == false)
                {
                    handCtrl.gameObject.SetActive(true);
                    handCtrl.ShowHandPosToPosToPos(tfBrush.position, tfBoxColor.position, tfFace.position);
                }
                else
                {
                    handCtrl.gameObject.SetActive(true);
                    handCtrl.ShowHandPosToPosToPos(tfBrush.position, tfEyeL.position, tfEyeR.position);
                }
                return;
            case 3:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPosToPos(tfMacara.position, tfEyeL.position, tfEyeR.position);
                return;
            case 4:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfLipStick.position, tfMouth.position);
                return;
            case 5:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfHandBand.position, tfHandBandThrow.position);
                return;
            case 6:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfCut.position, tfHairCut.position);
                return;
            case 7:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfSpray.position, tfHair.position);
                return;
            case 8:
                TutorialStep8(0);
                return;
            case 9:
                TutorialStep9(0);
                return;
            case 10:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfDecoHair.position, tfDecoTargetHair.position);
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
    // private void TutorialStepCurrent(int indexStep)
    // {
    //     if (handCtrl == null) return;
    //     if (indexStep >= listTfClothes.Count || indexStep >= listTfTargetClothes.Count) return;
    //     handCtrl.gameObject.SetActive(true);
    //     var tf = listTfClothes[indexStep];
    //     var tfTarget = listTfTargetClothes[indexStep];
    //     handCtrl.ShowHandPosToPos(tf.gameObject.transform.position, tfTarget.transform.position);
    // }
    // private void TutorialStep6(int indexStep)
    // {
    //     if (handCtrl == null) return;
    //     if (indexStep >= listTfSticker.Count || indexStep >= listTfTargetSticker.Count) return;
    //     handCtrl.gameObject.SetActive(true);
    //     var tf = listTfSticker[indexStep];
    //     var tfTarget = listTfTargetSticker[indexStep];
    //     handCtrl.ShowHandPosToPos(tf.gameObject.transform.position, tfTarget.transform.position);
    // }

    [SerializeField] private List<Transform> listTfDragTie = new List<Transform>();
    [SerializeField] private List<Transform> listTfDragTargeTie = new List<Transform>();
    public List<Transform> ListTfDragTie => listTfDragTie;
    public List<Transform> ListTfDragTargetTie => listTfDragTargeTie;
    private void TutorialStep8(int indexStep)
    {
        if (handCtrl == null) return;
        if (indexStep >= listTfDragTie.Count || indexStep >= listTfDragTargeTie.Count) return;
        handCtrl.gameObject.SetActive(true);
        var tf = listTfDragTie[indexStep];
        var tfTarget = listTfDragTargeTie[indexStep];
        handCtrl.ShowHandPosToPos(tf.gameObject.transform.position, tfTarget.transform.position);
    }

    [SerializeField] private List<Transform> listTfDragAccessory = new List<Transform>();
    [SerializeField] private List<Transform> listTfDragTargerAccessory = new List<Transform>();
    public List<Transform> ListTfDragAccessory => listTfDragAccessory;
    public List<Transform> ListTfDragTargerAccessory => listTfDragTargerAccessory;
    private void TutorialStep9(int indexStep)
    {
        if (handCtrl == null) return;
        if (indexStep >= listTfDragAccessory.Count || indexStep >= listTfDragTargerAccessory.Count) return;
        handCtrl.gameObject.SetActive(true);
        var tf = listTfDragAccessory[indexStep];
        var tfTarget = listTfDragTargerAccessory[indexStep];
        handCtrl.ShowHandPosToPos(tf.gameObject.transform.position, tfTarget.transform.position);
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