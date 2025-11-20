using UnityEngine;

public class LoopRotateCircle : MonoBehaviour
{
    [SerializeField] private bool clockwise;
    [SerializeField] private float speed = 1f;
    
    private void Update()
    {
        transform.Rotate(0, 0, (clockwise ? -1 : 1) * speed * Time.deltaTime);
    }
}
