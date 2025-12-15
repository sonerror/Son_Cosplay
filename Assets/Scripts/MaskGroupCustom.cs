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
  [SerializeField] public FxType FxSound = FxType.None;

  private void Start()
  {
    ResetMask();
  }

  public bool CheckMasksColiderAndDone(Vector3 pos)
  {
    foreach (var mask in masks)
    {
      if (mask.activeSelf == typeActive) continue;

      if (Vector2.Distance(pos, mask.transform.position) < distanceCheck)
      {
        mask.SetActive(typeActive);
        SoundManager.Ins.PlayFxIfNotPlay(FxSound);
      }
    }

    return masks.All(x => x.activeSelf == typeActive);
  }

  public bool CheckMasksColider(Vector3 pos)
  {
    var rs = false;
    foreach (var mask in masks)
    {
      if (mask.activeSelf == typeActive) continue;

      if (Vector2.Distance(pos, mask.transform.position) < distanceCheck)
      {
        mask.SetActive(typeActive);
        SoundManager.Ins.PlayFxIfNotPlay(FxSound);
        rs = true;
      }
    }

    return rs;
  }

  public Transform GetTranformOfMaskNotActive()
  {
    for (int i = 0; i < masks.Count; i++)
    {
      if (masks[i].activeSelf != typeActive)
      {
        return masks[i].transform;
      }
    }

    return null;
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

  public void SetTypeActive(bool active)
  {
    typeActive = active;
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
