using System;
using UnityEngine;

namespace VinhLB
{
    public class DestroyOnCollision : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            Destroy(other.gameObject);
        }
    }
}