using UnityEngine;

public class GamePlayScreen : UIScreen
{
  public Transform btnPlay;
  public Transform Text;

  public void ShowBtnInstall()
  {
    btnPlay.gameObject.SetActive(true);
    Text.gameObject.SetActive(false);
  }
  bool hadShowBtnInstall = false;
  void Update()
  {
    if (!hadShowBtnInstall && Input.GetMouseButtonDown(0))
    {
      hadShowBtnInstall = true;
      ShowBtnInstall();
    }
  }

  public void gotoStore()
  {
    GameManager.Ins.gotoStore();
  }

  public override void Resize(Vector2 gameSize)
  {
    base.Resize(gameSize);
    RectTf.sizeDelta = gameSize;
  }
}
