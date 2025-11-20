using UnityEngine;

namespace VinhLB
{
    public abstract class VinhLBMonoBehaviour : MonoBehaviour
    {
        private bool _hasCheckedForRectTransform;
        
        private Transform _transform;
        private RectTransform _rectTransform;
        
        public new Transform transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = GetComponent<Transform>();
                }

                return _transform;
            }
        }

        public RectTransform RectTransform
        {
            get
            {
                if (!_hasCheckedForRectTransform)
                {
                    _hasCheckedForRectTransform = true;
                    _rectTransform = transform as RectTransform;
                }

                return _rectTransform;
            }
        }
    }
    #pragma warning restore IDE1006
}