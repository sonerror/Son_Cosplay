using UnityEngine;

public class GamePlayScreen : UIScreen
{
  public Transform btnPlay;
  public Transform Text;

  void Start()
  {
    EventManager.StartListening("ShowBtnInstall", ShowBtnInstall);
    EventManager.StartListening("ShowText", ShowText);

  }

  private bool hadShowText = false;
  public void ShowText()
  {
    if (hadShowText) return;
    hadShowText = true;
    Text.gameObject.SetActive(true);
  }

  public void ShowBtnInstall()
  {
    if (hadShowBtnInstall) return;
    hadShowBtnInstall = true;
    btnPlay.gameObject.SetActive(true);
    Text.gameObject.SetActive(false);
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
