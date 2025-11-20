using System;
using UnityEngine;

namespace VinhLB.CustomControllers
{
    public class InputReader : ScriptableObject
    {
        public event Action<Vector2> Move;
        public event Action<Vector2, bool> Look;
        public event Action CameraControlEnabled;
        public event Action CameraControlDisabled;
        public event Action<bool> Jump; 
        public event Action<bool> Dash;
        public event Action Attack;
        public event Action<RaycastHit> Click;
        
        public Vector2 MoveDirection => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        public Vector2 LookDirection => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        public bool IsJumpKeyPressed => Input.GetKeyDown(KeyCode.Space);
    }
}