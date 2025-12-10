using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

public class MaskGroupCustom : GameUnit
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

#if UNITY_EDITOR
  [Button]
  public void LoadMasksFromChild()
  {
    masks = new List<GameObject>();
    for (var i = 0; i < Tf.childCount; i++)
    {
      masks.Add(Tf.GetChild(i).gameObject);
    }
  }
#endif
}
