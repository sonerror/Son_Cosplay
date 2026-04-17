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
    private void Start()
    {
        isOpenBox = false;
    }
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
    [SerializeField] private Transform tfFace;
    private bool isOpenBox = false;
    public bool IsOpenBox => isOpenBox;
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
                HintAuto(HintPos1, tfFace.position);
                return;
            case 2:
                HintAuto(HintPos1, tfFace.position);
                return;
            case 3:
                HintAuto(HintPos1, tfFace.position);
                return;
            case 4:
                if (isOpenBox == false)
                {
                    handCtrl.gameObject.SetActive(true);
                    handCtrl.ShowHandPosToPos(tftfBox.position, tftfBoxThrow.position);
                }
                else
                {
                    HintAuto(HintPos1, HintPos2);
                }
                return;
            case 5:
                HintAuto(HintPos1, HintPos2);
                return;
            case 6:
                TutorialStep7(0);
                return;
            case 7:
                TutorialStep(0);
                return;
            case 8:
                return;
            default:
                resetTimeHint();
                return;
        }
    }

    [SerializeField] private Vector3 HintPos1 { get; set; }
    [SerializeField] private Vector3 HintPos2 { get; set; }
    public void SetData(Vector3? pos1 = null, Vector3? pos2 = null)
    {
        if (pos1.HasValue) HintPos1 = pos1.Value;
        if (pos2.HasValue) HintPos2 = pos2.Value;
    }
    private void HintAuto(Vector3 pos1, Vector3 pos2)
    {
        handCtrl.gameObject.SetActive(true);
        handCtrl.ShowHandPosToPos(pos1, pos2);
    }

    private void HintAuto() => HintAuto(HintPos1, HintPos2);
    [Preserve]
    public void TutorialStep7(int indexStep)
    {
        if (handCtrl == null) return;
        if (indexStep >= tfItem.Count || indexStep >= tutorialNode.Count) return;
        handCtrl.gameObject.SetActive(true);
        var obj = tfItem[indexStep];
        var node = tutorialNode[indexStep];
        handCtrl.ShowHandPosToPos(node.position, obj.position);
    }
    [Preserve]
    public void TutorialStep(int indexStep)
    {
        if (handCtrlMakeup == null) return;
        if (indexStep >= listTfClothes.Count || indexStep >= listTfTargetClothes.Count) return;
        handCtrlMakeup.gameObject.SetActive(true);
        var obj = listTfTargetClothes[indexStep];
        var node = listTfClothes[indexStep];
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