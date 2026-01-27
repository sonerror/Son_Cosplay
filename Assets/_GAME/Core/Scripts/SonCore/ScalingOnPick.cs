using UnityEngine;
using Utilities;

namespace sonnv
{
    public class ScalingOnPick : SonMonoBehaviour
    {
        [SerializeField] private float scaleAmount = 1.1f;

        private Vector3 _originalScale;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        public void CancelScale()
        {
            OnMouseUp();
        }

        private void OnChangedCanScale()
        {
            OnMouseUp();
        }

        private void OnMouseDown()
        {
            Tf.localScale = _originalScale * scaleAmount;
        }

        private void OnMouseUp()
        {
            Tf.localScale = _originalScale;
        }
    }
}