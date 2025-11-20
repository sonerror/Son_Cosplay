using UnityEngine;
using Utilities;

namespace HoangHH
{
    public class ScalingOnPick : H3MonoBehaviour
    {
        [SerializeField] private float scaleAmount = 1.1f;
        public BoolModifierWithRegisteredSource isPreventScale;
        
        private Vector3 _originalScale;
        
        private void Awake()
        {
            _originalScale = transform.localScale;
            isPreventScale = new BoolModifierWithRegisteredSource(OnChangedCanScale);
        }

        public void CancelScale()
        {
            OnMouseUp();
        }
        
        private void OnChangedCanScale()
        {
            if (isPreventScale.Value)
            {
                OnMouseUp();
            }
        }
        
        private void OnMouseDown()
        {
            if (isPreventScale.Value) return;
            Tf.localScale = _originalScale * scaleAmount;
        }
        
        private void OnMouseUp()
        {
            Tf.localScale = _originalScale;
        }
    }
}