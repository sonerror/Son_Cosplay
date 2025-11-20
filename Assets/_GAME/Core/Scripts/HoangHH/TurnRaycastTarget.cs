
using UnityEngine;

namespace HoangHH
{
  public class TurnRaycastTarget : MonoBehaviour
  {

    private void TurnRaycastTargetOn(bool isOn)
    {
      // find all Image components in children
      var images = GetComponentsInChildren<UnityEngine.UI.Image>(true);
      foreach (var image in images)
      {
        image.raycastTarget = isOn;
      }
    }
  }
}