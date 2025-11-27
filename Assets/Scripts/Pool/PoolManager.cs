using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
  SFX_Acne,
  SFX_Acne_Cream,
}

public class PoolManager : Singleton<PoolManager>
{
  public PoolAmount[] poolAmounts;

  [System.Serializable]
  public struct PoolAmount
  {
    public PoolType type;
    public int amount;
    public PoolMember poolMember;
  }

  private Dictionary<PoolType, Queue<PoolMember>> dict = new Dictionary<PoolType, Queue<PoolMember>>();

  protected override void Awake()
  {
    base.Awake();
    OnInit();
  }

  private void OnInit()
  {
    for (int i = 0; i < poolAmounts.Length; i++)
    {
      if (!dict.ContainsKey(poolAmounts[i].type))
      {
        dict[poolAmounts[i].type] = new Queue<PoolMember>();
      }

      for (int j = 0; j < poolAmounts[i].amount; j++)
      {
        PoolMember poolMember = Instantiate(poolAmounts[i].poolMember);
        poolMember.gameObject.SetActive(false);
        dict[poolAmounts[i].type].Enqueue(poolMember);
      }
    }
  }

  public PoolMember Spawn(PoolType poolType, Vector3 pos, Quaternion rot)
  {
    PoolMember poolMember = dict[poolType].Count > 0 ? dict[poolType].Dequeue() : Instantiate(GetPrefab(poolType));

    poolMember.Tf.SetPositionAndRotation(pos, rot);
    poolMember.gameObject.SetActive(true);
    poolMember.OnSpawn();
    return poolMember;
  }

  public T Spawn<T>(PoolType poolType, Vector3 pos, Quaternion rot) where T : PoolMember
  {
    return Spawn(poolType, pos, rot) as T;
  }

  public void Despawn(PoolMember poolMember)
  {
    poolMember.gameObject.SetActive(false);
    poolMember.OnDespawn();
    dict[poolMember.poolType].Enqueue(poolMember);
  }


  public PoolMember GetPrefab(PoolType poolType)
  {
    for (int i = 0; i < poolAmounts.Length; i++)
    {
      if (poolAmounts[i].type == poolType)
      {
        return poolAmounts[i].poolMember;
      }
    }

    return null;
  }
}