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
        if (canBlockShowHint)
        {
            return;
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
    [SerializeField] private List<Transform> listTfClothes = new List<Transform>();
    [SerializeField] private List<Transform> listTfTargetClothes = new List<Transform>();
    public List<Transform> ListTfClothes => listTfClothes;
    public List<Transform> ListTfTargetClothes => listTfTargetClothes;

    [SerializeField] private Transform tfWigHair;
    [SerializeField] private Transform tfHair;

    [SerializeField] private Transform tfBody;
    [SerializeField] private Transform tfSpay1;
    [SerializeField] private Transform tfSpay2;


    [SerializeField] private List<Transform> listTfSticker = new List<Transform>();
    [SerializeField] private List<Transform> listTfTargetSticker = new List<Transform>();
    public List<Transform> ListTfSticker => listTfSticker;
    public List<Transform> ListTfTargetSticker => listTfTargetSticker;


    [SerializeField] private Transform tfLens;
    [SerializeField] private Transform tfEyes;


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
                TutorialStepCurrent(0);
                return;
            case 2:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfWigHair.position, tfHair.position);
                return;
            case 3:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfSpay1.position, tfBody.position);
                return;
            case 4:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfSpay2.position, tfBody.position);
                return;
            case 5:
                TutorialStep6(0);
                return;
            case 6:
                return;
            case 7:
                TutorialStep8(0);
                return;
            case 8:
                handCtrlMakeup.gameObject.SetActive(true);
                handCtrlMakeup.ShowHandPosToPos(tfLens.position, tfEyes.position);
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
    private void TutorialStepCurrent(int indexStep)
    {
        if (handCtrl == null) return;
        if (indexStep >= listTfClothes.Count || indexStep >= listTfTargetClothes.Count) return;
        handCtrl.gameObject.SetActive(true);
        var tf = listTfClothes[indexStep];
        var tfTarget = listTfTargetClothes[indexStep];
        handCtrl.ShowHandPosToPos(tf.gameObject.transform.position, tfTarget.transform.position);
    }
    private void TutorialStep6(int indexStep)
    {
        if (handCtrl == null) return;
        if (indexStep >= listTfSticker.Count || indexStep >= listTfTargetSticker.Count) return;
        handCtrl.gameObject.SetActive(true);
        var tf = listTfSticker[indexStep];
        var tfTarget = listTfTargetSticker[indexStep];
        handCtrl.ShowHandPosToPos(tf.gameObject.transform.position, tfTarget.transform.position);
    }

    [SerializeField] private List<Transform> listTfDragSticker = new List<Transform>();
    [SerializeField] private List<Transform> listTfDragTargetSticker = new List<Transform>();
    public List<Transform> ListTfDragSticker => listTfDragSticker;
    public List<Transform> ListTfDragTargetSticker => listTfDragTargetSticker;
    private void TutorialStep8(int indexStep)
    {
        if (handCtrl == null) return;
        if (indexStep >= listTfDragSticker.Count || indexStep >= listTfDragTargetSticker.Count) return;
        handCtrl.gameObject.SetActive(true);
        var tf = listTfDragSticker[indexStep];
        var tfTarget = listTfDragTargetSticker[indexStep];
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