using System;
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
  [SerializeField] private float RatingDone = 0.85f;
  [SerializeField] private float CurrRating = 0f;

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

    CurrRating = (float)masks.Count(x => x.activeSelf == typeActive) / masks.Count;

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
        rs = true;
      }
    }
    if (rs)
    {
      SoundManager.Ins.PlayFxIfNotPlay(FxSound);
      CurrRating = (float)masks.Count(x => x.activeSelf == typeActive) / masks.Count;
    }
    return rs;
  }

  public bool IsDoneRating(bool ActiveIfDone = false)
  {
    if (CurrRating >= RatingDone)
    {
      if (ActiveIfDone)
      {
        DoneAllMask();
        SoundManager.Ins.PlayFxIfNotPlay(FxSound);
        CurrRating = 1f;
      }
      return true;
    }
    else return false;
  }

  public float GetPercentFill()
  {
    return CurrRating;
  }

  public void DoneAllMask()
  {
    foreach (var mask in masks)
    {
      mask.SetActive(typeActive);
    }
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
    return Mathf.Approximately(CurrRating, 1f);
  }

  public void ResetMask()
  {
    CurrRating = 0f;
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
