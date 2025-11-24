using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolMember : GameUnit
{
  public PoolType poolType;

  public virtual void OnDespawn()
  {
  }

  public virtual void OnSpawn()
  {
  }
}
