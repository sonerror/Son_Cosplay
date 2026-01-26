using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class GameVidScreen : UIScreen
{
  public void OnClick()
  {
    // GameManager.Ins.StartGamePlay();
    UIManager.Ins.OpenUI(UIID.GamePlay);
    // UIManager.Ins.CloseUI(UIID.GameVideo);
  }
}
