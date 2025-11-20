using UnityEngine;

public class FollowTargetScaled : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 movementScale = Vector3.one;

    private Vector3 _lastTargetPosition;

    void Start()
    {
        if (target != null)
            _lastTargetPosition = target.position;
    }

    void LateUpdate()
    {
        // if (target == null) return;

        Vector3 targetDelta = target.position - _lastTargetPosition;
        Vector3 scaledDelta = Vector3.Scale(targetDelta, movementScale);
        transform.position += scaledDelta;

        _lastTargetPosition = target.position;
    }
}