
using UnityEngine;

namespace HoangHH
{
  public class TransformFallByGravity : H3MonoBehaviour
  {
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private Vector3 direction = Vector3.down;
    [SerializeField] private float initialVelocity;
    [SerializeField] private bool destroyAfterTime;
    [SerializeField]
    private float waitDestroy = 5f;

    private float velocity;
    private float _timeDestroyCounter;

    private void OnEnable()
    {
      velocity = initialVelocity;
    }

    private void LateUpdate()
    {
      velocity += gravity * Time.deltaTime;
      Tf.position += direction.normalized * (velocity * Time.deltaTime);
      if (destroyAfterTime)
      {
        _timeDestroyCounter += Time.deltaTime;
        if (_timeDestroyCounter > waitDestroy)
        {
          Destroy(gameObject);
        }
      }
    }
  }
}