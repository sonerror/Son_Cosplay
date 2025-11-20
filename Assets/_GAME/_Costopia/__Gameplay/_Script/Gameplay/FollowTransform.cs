using UnityEngine;

namespace HoangHH
{
    public class FollowTransform : H3MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private bool followX;
        [SerializeField] private bool followY;
        [SerializeField] private float offsetX;
        [SerializeField] private float offsetY;

        private void LateUpdate()
        {
            Vector3 position = Tf.position;
            if (followX) position.x = target.position.x + offsetX;
            if (followY) position.y = target.position.y + offsetY;
            Tf.position = position;
        }
    }
}