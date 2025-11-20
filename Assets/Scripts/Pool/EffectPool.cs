using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : GameUnit
{
    void OnEnable()
    {
        Invoke(nameof(DespawnEffect), 0.5f);
        try
        {
            var fx = tf.GetComponent<ParticleSystem>();
            fx.Play();
        }
        catch (Exception e)
        {
        }
    }

    void DespawnEffect()
    {
        PoolManager.Ins.Despawn( this);
    }
}