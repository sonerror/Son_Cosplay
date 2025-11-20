using UnityEngine;

namespace HoangHH
{
    public class TouchMoveDebug : H3MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        private Camera _camera;

        private bool _isDrag;
        
        private void Awake()
        {
            _camera = Camera.main;
            _isDrag = false;
            spriteRenderer.enabled = false;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
                // move to the position of the mouse
                pos.z = 0;
                Tf.position = pos;
                if (_isDrag) return;
                _isDrag = true;
                spriteRenderer.enabled = true;
            }
            else
            {
                if (!_isDrag) return;
                _isDrag = false;
                spriteRenderer.enabled = false;
            }
        }
    }
}
