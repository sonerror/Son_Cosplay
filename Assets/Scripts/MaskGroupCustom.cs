using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaskGroupCustom : MonoBehaviour
{
  [SerializeField] private List<GameObject> masks;
  [SerializeField] private bool typeActive = true;
  [SerializeField] private float distanceCheck = 0.75f;
  [SerializeField] private FxType fxSound = FxType.None;

  private void Start()
  {
    ResetMask();
  }

  public bool CheckMasks(Vector3 pos)
  {
    foreach (var mask in masks)
    {
      if (mask.activeSelf == typeActive) continue;

      if (Vector2.Distance(pos, mask.transform.position) < distanceCheck)
      {
        mask.SetActive(typeActive);
        SoundManager.Ins.PlayFxIfNotPlay(fxSound);
      }
    }

    return masks.All(x => x.activeSelf == typeActive);
  }

  public bool IsDone()
  {
    return masks.All(x => x.activeSelf == typeActive);
  }

  public void ResetMask()
  {
    foreach (var mask in masks)
    {
      mask.SetActive(!typeActive);
    }
  }
}
