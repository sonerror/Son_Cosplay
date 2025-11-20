using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEventHandler2D : MonoBehaviour
{
    public float startDelay = 0;
    private bool isEnabled = true;

    public string colEnterTag;
    public string colEnterName;
    public GameObject colEnterObj; // if not null, will be used to compare with the collided object
    public UnityEvent onCollisionEnter;

    public string colExitTag;
    public string colExitName;
    public GameObject colExitObj; // if not null, will be used to compare with the collided object
    public UnityEvent onCollisionExit;

    

    private void Start()
    {
        if (startDelay > 0)
        {
            StartCoroutine(_OnStart());
        }
    }

    private IEnumerator _OnStart()
    {
        isEnabled = false;
        yield return new WaitForSeconds(startDelay);
        isEnabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isEnabled) return;
        if (colEnterTag != "" && collision.gameObject.CompareTag(colEnterTag))
        {
            onCollisionEnter?.Invoke();
        }
        else if (colEnterName != "" && collision.gameObject.name == colEnterName)
        {
            onCollisionEnter?.Invoke();
        }
        else if (colEnterObj != null && collision.gameObject == colEnterObj)
        {
            onCollisionEnter?.Invoke();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!isEnabled) return;
        if (colExitTag != "" && collision.gameObject.CompareTag(colExitTag))
        {
            onCollisionExit?.Invoke();
        }
        else if (colExitName != "" && collision.gameObject.name == colExitName)
        {
            onCollisionExit?.Invoke();
        }
        else if (colExitObj != null && collision.gameObject == colExitObj)
        {
            onCollisionExit?.Invoke();
        }
    }
}