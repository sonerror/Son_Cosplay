using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaskGroupCustom : MonoBehaviour
{
  [SerializeField] private List<GameObject> masks;

  private void Start()
  {
    foreach (var mask in masks)
    {
      mask.SetActive(false);
    }
  }

  public bool CheckMasks(Vector3 pos)
  {
    foreach (var mask in masks)
    {
      if (mask.activeSelf)
      {
        continue;
      }

      if (Vector2.Distance(pos, mask.transform.position) < 0.75f)
      {
        mask.SetActive(true);
      }
    }

    return masks.All(x => x.activeSelf);
  }

  public void ResetMask()
  {
    foreach (var mask in masks)
    {
      mask.SetActive(false);
    }
  }
}
