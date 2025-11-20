using System;
using UnityEngine;

namespace VinhLB.CustomControllers
{
    public class RaycastSensor
    {
        public enum CastDirection
        {
            Forward = 0,
            Right = 1,
            Up = 2,
            Backward = 3,
            Left = 4,
            Down = 5
        }
        
        public float CastLength = 1f;
        public LayerMask LayerMask = 255;
        
        private Vector3 _origin = Vector3.zero;
        private Transform _tr;

        private CastDirection _castDirection;
        private RaycastHit _hitInfo;

        public RaycastSensor(Transform playerTransform) => _tr = playerTransform;

        public void SetCastOrigin(Vector3 origin) => _origin = origin;
        
        public void SetCastDirection(CastDirection direction) => _castDirection = direction;

        public bool HasDetectedHit() => _hitInfo.collider != null;

        public float GetDistance() => _hitInfo.distance;
        
        public Vector3 GetNormal() => _hitInfo.normal;
        
        public Vector3 GetPosition() => _hitInfo.point;
        
        public Transform GetTransform() => _hitInfo.transform;

        public void Cast()
        {
            Vector3 worldOrigin = _tr.TransformPoint(_origin);
            Vector3 worldDirection = GetCastDirection();
            
            Physics.Raycast(worldOrigin, worldDirection, out _hitInfo, CastLength, LayerMask, QueryTriggerInteraction.Ignore);
        }

        private Vector3 GetCastDirection()
        {
            return _castDirection switch
            {
                CastDirection.Forward => _tr.forward,
                CastDirection.Right => _tr.right,
                CastDirection.Up => _tr.up,
                CastDirection.Backward => -_tr.forward,
                CastDirection.Left => -_tr.right,
                CastDirection.Down => -_tr.up,
                _ => Vector3.zero
            };
        }
    }
}