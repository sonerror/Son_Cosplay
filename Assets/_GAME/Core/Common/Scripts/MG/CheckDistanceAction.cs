using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckDistanceAction : MonoBehaviour
{
  public UnityEvent actionTrue;
  public UnityEvent actionFalse;

  // [Tooltip("If null, checks distance to local zero. If not null, checks world distance to target.")]
  public Transform target;

  public bool checksGreater = true;
  public bool checksEqual = true;

  public float distance = 1;

  public bool checkOnEnable = true;

  private void OnEnable()
  {
    if (checkOnEnable)
    {
      Check();
    }
  }

  public void Check()
  {
    float distanceQrt = distance * distance;
    if (target == null)
    {
      float sqrMagnitude = transform.localPosition.sqrMagnitude;
      if ((checksGreater && sqrMagnitude > distanceQrt)
          || (checksEqual && sqrMagnitude == distanceQrt)
          || (!checksGreater && !checksEqual && sqrMagnitude < distanceQrt))
      {
        actionTrue?.Invoke();
      }
      else
      {
        actionFalse?.Invoke();
      }
    }
    else
    {
      float sqrMagnitude = (target.position - transform.position).sqrMagnitude;
      if ((checksGreater && sqrMagnitude > distanceQrt)
          || (checksEqual && sqrMagnitude == distanceQrt)
          || (!checksGreater && !checksEqual && sqrMagnitude < distanceQrt))
      {
        actionTrue?.Invoke();
      }
      else
      {
        actionFalse?.Invoke();
      }
    }
  }
}
