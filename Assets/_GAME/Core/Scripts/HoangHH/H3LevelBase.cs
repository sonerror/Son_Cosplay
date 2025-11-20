using UnityEngine;

namespace HoangHH
{
    public abstract class H3LevelBase : LevelBase
    {
        private Camera cam;
        
        public Camera Camera => cam ? cam : cam = Camera.main;

#if UNITY_EDITOR
        public void DebugLog(string message)
        {
            if (Application.isEditor)
            {
                Debug.Log($"[H3LevelBase] {message}");
            }
        }
#endif
    }
}
