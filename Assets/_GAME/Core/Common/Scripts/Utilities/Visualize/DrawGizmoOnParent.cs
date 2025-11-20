using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class DrawGizmoOnParent : MonoBehaviour
    {
#if UNITY_EDITOR
        public int backwardMessageDepth = 3;
        private void OnDrawGizmosSelected()
        {
            Transform parent = this.transform;
            for (int i = 0; i < backwardMessageDepth; i++)
            {
                parent = parent.parent;
                if (parent)
                {
                    parent.SendMessage("OnDrawGizmosSelected");
                }
                else
                {
                    break;
                }
            }
        }
#endif
    }
}