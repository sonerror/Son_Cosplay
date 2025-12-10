using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class Item : MonoBehaviour
{
  private Transform _tf;
  public Transform Tf => _tf ? _tf : _tf = transform;
  public bool IsReady = false;
  [SerializeField] protected bool isBlocked = false;
  protected bool isDragging = false;
  protected Vector3 offSet;


  public bool BlockItem => isBlocked;
  public int step = 0;

  public UnityEvent OnWrong;

  public UnityEvent OnFinish;
  public UnityEvent OnPickItem;
  public UnityEvent OnDropItem;

  protected Camera mainCamera;
  protected virtual void Awake()
  {
    mainCamera = Camera.main;
  }

  public virtual void MouseDown(BaseEventData eventData)
  {
    if (IsReady) TutorialManager.Ins.OnCollectSuccess();
    else TutorialManager.Ins.OnCollectFail();
  }

  public virtual void MouseUp(BaseEventData eventData)
  {
    TutorialManager.Ins.resetTimeHint();
  }

  public virtual void MouseDrag(BaseEventData eventData) { }

  public void SetIsReady(bool ready)
  {
    IsReady = ready;
  }

  public void SetBlockItem(bool block)
  {
    isBlocked = block;
  }

#if UNITY_EDITOR

  [Button]
  protected void SetUpEventTrigger()
  {
    var eventTrigger = gameObject.AddComponent<EventTrigger>();
    AddEventTriggerEntry(EventTriggerType.PointerDown, MouseDown);
    AddEventTriggerEntry(EventTriggerType.PointerUp, MouseUp);
    AddEventTriggerEntry(EventTriggerType.Drag, MouseDrag);

    void AddEventTriggerEntry(EventTriggerType eventType, UnityAction<BaseEventData> action)
    {
      var entry = new EventTrigger.Entry
      {
        eventID = eventType
      };
      UnityEditor.Events.UnityEventTools.AddPersistentListener(entry.callback, action);
      eventTrigger.triggers.Add(entry);
    }
  }
#endif

  protected virtual Vector3 GetMouseWorldPos()
  {
    Vector3 mousePoint = Input.mousePosition;
    return mainCamera.ScreenToWorldPoint(mousePoint);
  }



}

[System.Serializable]
public class TargetInfo
{
  public Transform targetTf;
  public float distance;
  public bool hadChecked = false;
}
