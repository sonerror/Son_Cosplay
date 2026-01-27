using UnityEngine;

public class GamePlayScreen : UIScreen
{
  public Transform btnPlay;
  public Transform Text1, Text2;
  public GameObject IconLv, logoUI;

  void Start()
  {
    EventManager.StartListening("ShowBtnInstall", ShowBtnInstall);
    EventManager.StartListening("ShowText2", ShowText2);
    EventManager.StartListening("ShowIconLv", ShowIconLv);
  }

  public void ShowBtnInstall()
  {
    if (hadShowBtnInstall) return;
    hadShowBtnInstall = true;
    btnPlay.gameObject.SetActive(true);
    Text2.gameObject.SetActive(false);
  }

  public void ShowIconLv()
  {
    IconLv.SetActive(true);
  }

  public void ShowText2()
  {
    Text1.gameObject.SetActive(false);
    Text2.gameObject.SetActive(true);
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
