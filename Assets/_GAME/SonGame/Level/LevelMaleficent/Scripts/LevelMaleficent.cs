using UnityEngine;
using DG.Tweening;
public class LevelMaleficent : GamePlayManager
{
    private bool hadClicked = false;
    [SerializeField] private bool isPlayingGame = false;
    [SerializeField] private UIManager uIManager;
    public bool IsPlayingGame => isPlayingGame;
    private void Update()
    {
        if (isPlayingGame && Input.GetMouseButtonDown(0))
        {
            EventManager.TriggerEvent("ShowBtnInstall");
        }

        if (!hadClicked && Input.GetMouseButtonDown(0))
        {
            hadClicked = true;
            StartGamePlay();
        }
    }
    private void StartGamePlay()
    {
        GamePlayScreen uiScreen = UIManager.Ins.GetUI(0);
        uiScreen.logoUI.SetActive(false);
        EventManager.TriggerEvent("ShowIconLv");
        TutorialManager.Ins.enableCountTime = true;
        SoundManager.Ins.PlayFx(FxType.StartGame);
        SoundManager.Ins.PlayBgm();
        DOVirtual.DelayedCall(0.2f, () =>
        {
            isPlayingGame = true;
        });

    }


}
