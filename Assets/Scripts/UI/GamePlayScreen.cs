using DG.Tweening;
using UnityEngine;

public class GamePlayScreen : UIScreen
{
  public Transform btnPlay;
  public Transform Text;

  void Start()
  {
    EventManager.StartListening("ShowBtnInstall", ShowBtnInstall);
  }

  public void ShowBtnInstall()
  {
    if (hadShowBtnInstall) return;
    hadShowBtnInstall = true;
    Text.gameObject.SetActive(false);
    DOVirtual.DelayedCall(1f, () =>
    {
      btnPlay.gameObject.SetActive(true);
    });
  }
  bool hadShowBtnInstall = false;

  public void gotoStore()
  {
    AdsManager.Ins.gotoStore();
  }

  public override void Resize(Vector2 gameSize)
  {
    base.Resize(gameSize);
    RectTf.sizeDelta = gameSize;
  }
}
