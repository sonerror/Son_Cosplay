using System;
using UnityEngine;
using UnityEngine.Events;

namespace HoangHH
{
    public class SnapObjectNoInteract : H3MonoBehaviour
    {
        public bool canSnap;
        [SerializeField] private SnapPoint snapPoint;
        [SerializeField] private float snapDistance = 0.09f;
        
        [SerializeField] protected UnityEvent onSnap;
        private void OnMouseUpAsButton()
        {
            TrySnap();
        }

        public void TrySnap()
        {
            if (!canSnap) return;
            if (snapPoint.isSnap) return;
            if (DistanceToInSqrVec2(snapPoint.Tf.position) < snapDistance)
            {
                snapPoint.isSnap = true;
                snapPoint.OnSnap();
                onSnap.Invoke();
            }
        }
    }
}