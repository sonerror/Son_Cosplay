using System;
using System.Collections.Generic;
using Collider2DOptimization;
using UnityEngine;
using UnityEngine.Serialization;

namespace VinhLB
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class VinhLBPolygonColliderOptimizer : MonoBehaviour
    {
        [SerializeField]
        private double _tolerance;
        
        private PolygonCollider2D _collider;
        private List<List<Vector2>> _originalPaths = new List<List<Vector2>>();

        public double Tolerance
        {
            get => _tolerance;
            set
            {
                _tolerance = value;
                
                ResetPaths();
            }
        }

        public void Reset()
        {
            Initialize();
            
            ResetPaths();
        }

        private void OnValidate()
        {
            if(_collider == null)
            {
                Initialize();
            }

            ResetPaths();
        }
        
        private void Initialize()
        {
            //When first getting a reference to the collider save the paths
            //so that the optimization is redoable (by performing it on the original path
            //every time)
            _collider = GetComponent<PolygonCollider2D>();
            _originalPaths.Clear();
            for(int i = 0; i < _collider.pathCount; i++)
            {
                List<Vector2> path = new List<Vector2>(_collider.GetPath(i));
                _originalPaths.Add(path);
            }
        }

        private void ResetPaths()
        {
            //Reset the original paths
            if(Tolerance <= 0)
            {
                for(int i = 0; i < _originalPaths.Count; i++)
                {
                    List<Vector2> path = _originalPaths[i];
                    _collider.SetPath(i, path.ToArray());
                }
                return;
            }
            for(int i = 0; i < _originalPaths.Count; i++)
            {
                List<Vector2> path = _originalPaths[i];
                path = ShapeOptimizationHelper.DouglasPeuckerReduction(path, Tolerance);
                _collider.SetPath(i, path.ToArray());

            }
        }
    }
}