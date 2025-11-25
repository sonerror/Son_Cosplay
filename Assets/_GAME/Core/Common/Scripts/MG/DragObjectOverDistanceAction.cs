using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class DragObjectOverDistanceAction : DragObject2D
{
  // [Tooltip("Invoked on drop")]
  public UnityEvent actionTrue;
  // [Tooltip("Invoked on drop")]
  public UnityEvent actionFalse;

  // [Tooltip("Invoked while dragging")]
  public UnityEvent actionTrueDrag;
  // [Tooltip("Invoked while dragging")]
  public UnityEvent actionFalseDrag;

  private bool isTrueDragInvoked;

  public Transform target;
  public Vector3 originalPosition;

  public bool checksGreater = true;
  public bool checksEqual = true;

  public float distance = 1;

  public bool checkDistance = true;
  public bool checkOnEnable = true;

  public bool useScale = true;
  public float scaleOnDrag = 1.1f;
  public float scaleOnDrop = 1;

  public AudioClip[] acOnDrag;
  public AudioClip[] acOnDropFalse;
  public AudioClip[] acOnDropTrue;

  public Collider2D col;
  public Rigidbody2D rb;
  public bool isKinematicOnDrag = true;
  public bool isDynamicOnDropTrue = true;
  public bool disableColOnDropTrue = true;

  [SerializeField] private Vector3 lastMousePosition; // For calculating throw velocity
  private Vector3 screenPoint;

  public bool useThrow = true;
  public float throwMultiplier = 2;

  private void OnEnable()
  {
    originalPosition = transform.position;
    if (checkOnEnable)
    {
      Check();
    }
  }

  public override void OnDragStart()
  {
    base.OnDragStart();

    if (acOnDrag != null)
    {
      // AudioManager.PlaySFX(acOnDrag);
    }
    if (col != null)
    {
      col.enabled = false;
    }
    if (rb != null && isKinematicOnDrag)
    {
      rb.isKinematic = true;
    }
    if (useScale)
    {
      //transform.localScale = Vector3.one * scaleOnDrag;
      DOTween.Kill(twnScale);

      twnScale = transform.DOScale(scaleOnDrag, 0.1f);
    }

    //// Additional for throw functionality
    //screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
    //offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.ySpeed, screenPoint.z));
  }

  public override void OnDrag(Vector3 direction)
  {
    base.OnDrag(direction);

    // Drag and throw functionality

    if (rb != null)
    {
      Vector2 vector = direction.normalized * throwMultiplier; // Adjust the speed as needed
      if (vector != Vector2.zero) lastMousePosition = vector;

      if (actionTrueDrag != null && !isTrueDragInvoked)
      {
        isTrueDragInvoked = Check(target, actionTrueDrag, actionFalseDrag);
      }
    }
  }

  Tween twnScale;
  public override void OnDragStop()
  {
    base.OnDragStop();

    bool check = Check(target, actionTrue, actionFalse);
    if (!check && acOnDropFalse != null)
    {
      // AudioManager.PlaySFX(acOnDropFalse);
    }
    if (col != null)
    {
      col.enabled = true;
    }
    if (check)
    {
      if (rb != null && isDynamicOnDropTrue)
      {
        rb.isKinematic = false;
        if (useThrow) rb.linearVelocity = lastMousePosition; // Apply last recorded velocity to simulate a throw
      }
      if (disableColOnDropTrue && col != null)
      {
        col.enabled = false;
      }
      if (acOnDropTrue != null)
      {
        // AudioManager.PlaySFX(acOnDropTrue);
      }
    }
    if (useScale)
    {
      //transform.localScale = Vector3.one * scaleOnDrop;
      DOTween.Kill(twnScale);
      twnScale = transform.DOScale(scaleOnDrop, 0.15f);
    }
  }

  public void Check()
  {
    Check(target, actionTrue, actionFalse);
  }

  public bool Check(Transform target, UnityEvent actionTrue, UnityEvent actionFalse)
  {
    if (!checkDistance) return true;
    bool result;
    float distanceQrt = distance * distance;
    if (target == null)
    {
      float sqrMagnitude = (originalPosition - transform.position).sqrMagnitude;
      if ((checksGreater && sqrMagnitude > distanceQrt)
                         || (checksEqual && sqrMagnitude == distanceQrt)
                                        || (!checksGreater && !checksEqual && sqrMagnitude < distanceQrt))
      {
        actionTrue?.Invoke();
        result = true;
      }
      else
      {
        actionFalse?.Invoke();
        result = false;
      }
    }
    else
    {
      float sqrMagnitude = (target.position - transform.position).sqrMagnitude;
      if ((checksGreater && sqrMagnitude > distanceQrt)
                         || (checksEqual && sqrMagnitude == distanceQrt)
                                        || (!checksGreater && !checksEqual && sqrMagnitude < distanceQrt))
      {
        actionTrue?.Invoke();
        result = true;
      }
      else
      {
        actionFalse?.Invoke();
        result = false;
      }
    }
    return result;
  }
}
