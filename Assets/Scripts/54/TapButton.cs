
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AnhPD.Cook
{
  public class TapButton : Item
  {
    [SerializeField] private Collider2D coll2D;
    [SerializeField] private FxType sfxClick = FxType.None;

    public bool isAlwaysReady;

    public override void MouseDown(BaseEventData eventData)
    {
      base.MouseDown(eventData);
      if (isBlocked) return;
      if (!IsReady) return;
      if (!isAlwaysReady)
        IsReady = false;
      SoundManager.Ins.PlayFx(sfxClick);
      OnFinish?.Invoke();
    }

    public void EnableCollider(bool isEnable = true)
    {
      coll2D.enabled = isEnable;
    }

    public void OnReady()
    {
      IsReady = true;
    }
    [Button]
    private void AddBoxCollider()
    {
      if (!coll2D)
      {
        coll2D = gameObject.AddComponent<BoxCollider2D>();
        coll2D.isTrigger = true;
      }
    }
  }
}

