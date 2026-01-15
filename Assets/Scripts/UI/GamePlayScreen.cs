using UnityEngine;

public class GamePlayScreen : UIScreen
{
  public Transform btnPlay;
  public Transform Text1, Text2;

  void Start()
  {
    // EventManager.StartListening("ShowBtnInstall", ShowBtnInstall);
    // EventManager.StartListening("ShowText2", ShowText2);
  }
  private bool hadClick = false;
  void Update()
  {
    if (!hadClick && Input.GetMouseButtonDown(0))
    {
      hadClick = true;
      ShowBtnInstall();
    }
  }

  public void ShowBtnInstall()
  {
    if (hadShowBtnInstall) return;
    hadShowBtnInstall = true;
    btnPlay.gameObject.SetActive(true);
    Text2.gameObject.SetActive(false);
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
