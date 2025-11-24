using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
  /// <summary>
  /// Call getter Value return true if there any modifier inside, otherwise return false
  /// </summary>
  [System.Serializable]
  public class BoolModifierWithRegisteredSource
  {
    private List<int> _sources = new List<int>();
#if UNITY_EDITOR
    private List<Object> _sourceObjs = new List<Object>();
#endif
    private bool _value;
    public bool Value => _value;
    private System.Action _onChanged;

    public BoolModifierWithRegisteredSource() { }
    public BoolModifierWithRegisteredSource(System.Action onChanged)
    {
      _onChanged = onChanged;
    }

    public void AddModifier(Object @object)
    {
      AddModifier(@object.GetInstanceID());
#if UNITY_EDITOR
      if (!_sourceObjs.Contains(@object))
      {
        _sourceObjs.Add(@object);
      }
#endif
    }
    public void AddModifier(int id)
    {
      if (!_sources.Contains(id))
      {
        _sources.Add(id);
        _value = _sources.Count > 0;
        _onChanged?.Invoke();
      }
    }

    public void RemoveModifier(Object @object)
    {
      RemoveModifier(@object.GetInstanceID());
#if UNITY_EDITOR
      _sourceObjs.Remove(@object);
#endif
    }
    public void RemoveModifier(int id)
    {
      _sources.Remove(id);
      _value = _sources.Count > 0;
      _onChanged?.Invoke();
    }
    public void ClearAll()
    {
      _sources.Clear();
#if UNITY_EDITOR
      _sourceObjs.Clear();
#endif
      _value = false;
      _onChanged?.Invoke();
    }
  }

}