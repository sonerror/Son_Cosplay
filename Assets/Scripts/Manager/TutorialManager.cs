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
    [SerializeField] private bool isTapGel = false;
    public void ChangeStateIsTapGel(bool value)
    {
        isTapGel = value;
    }
    [SerializeField] private Transform tfComb;
    [SerializeField] private Transform tfBrush;

    [SerializeField] private Transform tfHairL;
    [SerializeField] private Transform tfHairR;


    [SerializeField] private Transform tfFace;
    private bool isOpenBox = false;
    public void SetIsOpenBox()
    {
        isOpenBox = true;
    }




    [SerializeField] private Transform tftfBox;
    [SerializeField] private Transform tftfBoxThrow;



    [SerializeField] private List<Transform> listTfClothes = new List<Transform>();
    [SerializeField] private List<Transform> listTfTargetClothes = new List<Transform>();
    public List<Transform> ListTfClothes => listTfClothes;
    public List<Transform> ListTfTargetClothes => listTfTargetClothes;
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
                handCtrl.ShowHandPosToPos(tfComb.position, tfFace.position);
                return;
            case 2:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfBrush.position, tfFace.position);
                return;
            case 3:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tfComb.position, tfFace.position);
                return;
            case 4:
                if (isOpenBox == false)
                {
                    handCtrl.gameObject.SetActive(true);
                    handCtrl.ShowHandPosToPos(tftfBox.position, tftfBoxThrow.position);
                }
                else
                {
                    handCtrl.gameObject.SetActive(true);
                    handCtrl.ShowHandPosToPos(tftfBox.position, tfHairR.position);
                }
                return;
            case 5:
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tftfBox.position, tfHairL.position);
                return;
            case 6:
                if (handCtrl == null) return;
                if (0 >= tfItem.Count || 0 >= tutorialNode.Count) return;
                handCtrl.gameObject.SetActive(true);
                handCtrl.ShowHandPosToPos(tutorialNode[0].position, tfItem[0].position);
                return;
            case 7:
                if (handCtrlMakeup == null) return;
                if (0 >= listTfClothes.Count || 0 >= listTfTargetClothes.Count) return;
                handCtrlMakeup.gameObject.SetActive(true);
                handCtrlMakeup.ShowHandPosToPos(listTfClothes[0].position, listTfTargetClothes[0].position);
                return;
            case 8:
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