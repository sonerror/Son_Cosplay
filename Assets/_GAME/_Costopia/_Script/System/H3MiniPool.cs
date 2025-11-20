using System.Collections.Generic;
using UnityEngine;

namespace HoangHH
{
  public class H3MiniPool<T> where T : Component
  {
    private Transform _parent;
    private T _prefab;

    public Queue<T> InactiveObjects { get; } = new Queue<T>();

    public List<T> ActiveObjects { get; } = new List<T>();

    public void OnInit(T prefab, int amount, Transform parent = null)
    {
      if (prefab == null)
      {
        Debug.LogError("HMiniPool: Prefab is null!");
        return;
      }

      _prefab = prefab;
      _parent = parent;

      for (int i = 0; i < amount; i++)
      {
        T obj = Object.Instantiate(prefab, parent);
        Despawn(obj);
      }
    }


    public T Spawn(Vector3 pos, Quaternion rot)
    {
      T go = Spawn();

      go.transform.SetPositionAndRotation(pos, rot);

      return go;
    }

    public T Spawn()
    {
      T go = InactiveObjects.Count > 0 ? InactiveObjects.Dequeue() :
          Object.Instantiate(_prefab, _parent);

      if (!ActiveObjects.Contains(go)) // Prevent duplicate tracking
      {
        ActiveObjects.Add(go);
      }

      go.gameObject.SetActive(true);
      return go;
    }


    public void Despawn(T obj)
    {
      if (!obj.gameObject.activeSelf || InactiveObjects.Contains(obj)) return;

      obj.gameObject.SetActive(false);
      InactiveObjects.Enqueue(obj);
      ActiveObjects.Remove(obj);
    }


    public void DespawnAll()
    {
      for (int i = ActiveObjects.Count - 1; i >= 0; i--)
      {
        Despawn(ActiveObjects[i]);
      }
      ActiveObjects.Clear();
    }

    public void DestroyAll()
    {
      foreach (var obj in ActiveObjects)
      {
        Object.Destroy(obj.gameObject);
      }
      ActiveObjects.Clear();

      while (InactiveObjects.Count > 0)
      {
        Object.Destroy(InactiveObjects.Dequeue().gameObject);
      }
    }

  }
}