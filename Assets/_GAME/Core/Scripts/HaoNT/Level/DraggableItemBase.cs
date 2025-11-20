using Costopia.Base;

using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Costopia.Level
{
  public class DraggableItemBase : ItemBase
  {

    [SerializeField]
    protected float timeCheckActiveInZone = 2f;


    [SerializeField]
    protected Rect triggerInteractZone = new Rect(-1, -1, 2, 2);


    [SerializeField]
    protected bool isTriggerZone;


    [SerializeField]
    private UnityEvent onTriggerZoneEvent, outTriggerZoneEvent;

    protected bool _isTriggerFaceZone = false;

    public virtual void OnMouseDown()
    {
    }

    public virtual void OnMouseDrag()
    {
    }

    public virtual void OnMouseUp()
    {
      _isTriggerFaceZone = false;
      _triggerZoneCheck = false;
      if (isTriggerZone)
      {
        outTriggerZoneEvent.Invoke();
      }
    }

    protected virtual void CheckZoneWhileMouseDragging()
    {
      _isTriggerFaceZone = triggerInteractZone.Contains(TF.position);
      TriggerZoneEvent();
    }

    private bool _triggerZoneCheck = false;
    protected float _timer;

    protected virtual void TriggerZoneEvent()
    {
      if (_isTriggerFaceZone)
      {
        if (InRightStep())
        {
          if (_triggerZoneCheck) return;
          _triggerZoneCheck = true;
          if (isTriggerZone)
          {
            //Debug.LogError("On Trigger Zone Event");
            onTriggerZoneEvent.Invoke();
          }
          return;
        }

        _timer += Time.deltaTime;
        if (!(_timer >= timeCheckActiveInZone)) return;
        OnMouseUp();
        _levelMakeupBase.OnWrongCurrentStep();
      }
      else
      {
        if (!_triggerZoneCheck) return;
        //Debug.LogError("Out Trigger Zone Event");
        _triggerZoneCheck = false;
        outTriggerZoneEvent.Invoke();
      }
    }

  }
}