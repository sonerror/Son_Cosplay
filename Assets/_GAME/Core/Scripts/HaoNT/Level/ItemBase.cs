using Costopia.Level;
using System.Collections.Generic;

using UnityEngine;

namespace Costopia.Base
{
  public class ItemBase : MonoBehaviour
  {
    private Transform _tf;

    public Transform TF => _tf ? _tf : _tf = transform;

    [SerializeField]
    protected List<int> useInSteps;

    protected StepByStepMakeUp _levelMakeupBase => LevelBase.Instance as StepByStepMakeUp;

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
    }

    protected virtual bool InRightStep()
    {
      return useInSteps.Contains(_levelMakeupBase.CurrentStep);
      //return false;
    }
  }
}