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
    [SerializeField] private StepManager gamePlayManager;
    [SerializeField] private LevelRumiAilen _level;
    [SerializeField] private Animator animHand;
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

    int countCollectFail = 0;
    private bool isTap = false;

    private void Update()
    {
        if (isTap) return;

        // Khi user vừa chạm xuống
        if (Input.GetMouseButtonDown(0))
        {
            HideHint();

            // reset lại thời gian
            timeCountHint = countCollectFail >= 3 ? 1.5f : TimeHint;
            enableCountTime = true;

            return;
        }

        // 🔥 Nếu đang giữ tay -> KHÔNG đếm thời gian
        if (Input.GetMouseButton(0))
        {
            return;
        }

        if (!enableCountTime) return;
        if (animHand.gameObject.activeSelf) return;
        if (handCtrl.gameObject.activeSelf) return;

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
                Tutorial(index);
                return;

            case 1:
                if (_level.CurrentCharacter == 1)
                {
                    TutorialStep1(countHintStep1);
                }
                return;
            case 2:
                if (_level.CurrentCharacter == 1)
                {
                    TutorialStep1(countHintStep1);
                }
                return;
            default:
                animHand.gameObject.SetActive(false);
                resetTimeHint();
                return;
        }
    }

    private void Tutorial(int indexStep)
    {
        if (animHand == null) return;

        animHand.gameObject.SetActive(true);
        animHand.SetBool("Por", true);
    }

    private void TutorialStep1(int indexStep)
    {
        if (handCtrl == null) return;
        if (indexStep >= tfItem.Count || indexStep >= tutorialNode.Count) return;

        handCtrl.gameObject.SetActive(true);
        var obj = tfItem[indexStep];
        var node = tutorialNode[indexStep];
        handCtrl.ShowHandPosToPos(obj.position, node.position);
    }

    public void SetStateIsTap(bool value)
    {
        isTap = value;
    }

    void HideHint()
    {
        animHand.gameObject.SetActive(false);
        handCtrl.gameObject.SetActive(false);
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
}
