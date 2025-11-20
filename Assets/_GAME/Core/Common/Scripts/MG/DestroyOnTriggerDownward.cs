using UnityEngine;
using UnityEngine.Events;

public class DestroyOnTriggerDownward : MonoBehaviour
{
  [Range(0, 1)]
  public float hitRate = 1;
  public bool checkDownward = true;
  public string targetObjectName;
  private Vector3 previousPosition;
  // [Tooltip("Set dynamic on trigger instead of destroying")]
  public bool setDynamicOnTrigger;

  public UnityEvent<Collider2D> onTrigger;

  private void Start()
  {
    previousPosition = transform.position;
  }

  private void Update()
  {
    // Update the previous position in each frame
    previousPosition = transform.position;
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    // Check if the object is moving downward relative to its local Y axis
    if ((!checkDownward || transform.InverseTransformDirection(transform.position - previousPosition).y < 0) && (hitRate >= 1 || Random.value <= hitRate))
    {
      // Check if the triggered object has the specified name
      if (other.gameObject.name == targetObjectName)
      {
        // Invoke the onTrigger event
        onTrigger?.Invoke(other);

        if (setDynamicOnTrigger)
        {
          // Set the triggered object's rigidbody to dynamic
          other.gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        }
        else
        {
          // Destroy the triggered object
          Destroy(other.gameObject);
        }
      }
    }
  }
}