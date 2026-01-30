using System.Collections.Generic;
using UnityEngine;

public class MiniPool<T> where T : Component
{
    private readonly Queue<T> queueActives = new Queue<T>();
    private Transform parent;
    private T prefab;

    public Queue<T> GetActiveQueue()
    {
        return queueActives;
    }

    public void OnInit(T prefab, int amount, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < amount; i++)
        {
            T obj = Object.Instantiate(prefab, parent);
            queueActives.Enqueue(obj);
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
        while (true)
        {
            T go = queueActives.Count > 0 ? queueActives.Dequeue() : null;

            if (go == null)
            {
                go = Object.Instantiate(prefab, parent);
                queueActives.Enqueue(go);
            }
            else if (go.gameObject.activeSelf) continue;

            go.gameObject.SetActive(true);

            return go;
        }
    }

    public void Despawn(T obj, bool isCollect = false)
    {
        if (!obj.gameObject.activeSelf) return;
        obj.gameObject.SetActive(false);
        if (!isCollect) queueActives.Enqueue(obj);
    }

    public void Collect(bool isCollect = true)
    {
        while (queueActives.Count > 0) Despawn(queueActives.Dequeue(), isCollect);
    }

    public void Release()
    {
        Collect();
        while (queueActives.Count > 0) Object.Destroy(queueActives.Dequeue().gameObject);
    }
}
