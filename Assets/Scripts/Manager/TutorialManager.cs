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
    [SerializeField] private Animator animHand;
    // [SerializeField] private GameObject clock;
    private int CountStepDone = 0;
    public void OnStepDone()
    {
        CountStepDone++;
    }

    public void IncreaseTimeHide()
    {
        TimeHint = 5f;
        this.resetTimeHint();
    }
    [SerializeField] private List<Transform> tutorialNode = new List<Transform>();
    [SerializeField] private List<Transform> tfItem = new List<Transform>();
    public List<Transform> TutorialNode => tutorialNode;
    int countCollectFail = 0;

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
    private bool isTap = false;
    private void Update()
    {
        if (isTap) return;
        if (Input.GetMouseButtonDown(0))
        {
            if (animHand.gameObject.activeSelf)
            {
                HideHint();
            }
            return;
        }

        if (Input.GetMouseButton(0)) return;
        if (!enableCountTime) return;
        if (animHand.gameObject.activeSelf) return;
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
            default:
                animHand.gameObject.SetActive(false);
                resetTimeHint();
                return;
        }
    }

    private void Tutorial(int indexStep)
    {
        animHand.gameObject.SetActive(true);
        Debug.Log(animHand.GetBool("Por"));
        Debug.Log(animHand.GetCurrentAnimatorStateInfo(0).normalizedTime);

        animHand.SetBool("Por", true);
        // var obj = tfItem[indexStep];
        // var node = tutorialNode[indexStep];
        // handCtrl.ShowHandPosToPos(obj.position, node.position);
    }
    public void SetStateIsTap()
    {
        isTap = true;
    }
    void HideHint()
    {
        enableCountTime = true;
        animHand.gameObject.SetActive(false);

        // handCtrl.HideHand();
    }

    public void resetTimeHint()
    {
        HideHint();

        timeCountHint = countCollectFail >= 3 ? 1.5f : TimeHint;
    }
}
