using HoangHH.AdsIAP;
using UnityEngine;


namespace Costopia.Core
{
  public class InitCostopia : MonoBehaviour
  {
    private void Start()
    {
      // Target FPS
      int fps = PlayerPrefs.GetInt(CostopiaConstant.KEY_FPS, 60);
      Application.targetFrameRate = fps;
      Screen.sleepTimeout = SleepTimeout.NeverSleep;

      // Initialize Adjust SDK
    }

  }
}