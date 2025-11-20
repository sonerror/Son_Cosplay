using UnityEngine;

namespace HoangHH
{
  public class H3ForwardTriggerEvent : MonoBehaviour
  {
    [SerializeField] private Collider2D col;
    [SerializeField] private Rigidbody2D rb;

    public System.Action<Collider2D> onTriggerEnter;
    public System.Action<Collider2D> onTriggerStay;
    public System.Action<Collider2D> onTriggerExit;
    public Collider2D Collider => col;
    public Rigidbody2D Rigidbody => rb;

    private void OnTriggerEnter2D(Collider2D collision)
    {
      onTriggerEnter?.Invoke(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
      onTriggerStay?.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
      onTriggerExit?.Invoke(collision);
    }

#if UNITY_EDITOR


    private void GetRef()
    {
      col = GetComponent<Collider2D>();
      rb = GetComponent<Rigidbody2D>();
    }

#endif
  }
}