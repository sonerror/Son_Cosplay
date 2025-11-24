using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{
  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.P))
    {
      ScreenCapture.CaptureScreenshot("screenshot.png");
      Debug.Log("Screenshot taken");
    }
  }
}
