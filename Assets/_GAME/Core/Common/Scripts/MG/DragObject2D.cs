using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

/// <summary>
/// Moves this 2D object as player drag their finger/mouse across the screen. Can be restricted to only change X or Y. Can be restricted within a certain rectangle, shows the rectangle in gizmos.
/// </summary>
public class DragObject2D : MonoBehaviour
{
  public UnityEvent onDragStart;
  public UnityEvent onDragStop;

  public bool zeroOnDragStart; // set position to cursor on drag start (doesn't change Z)

  public bool restrictX;
  public bool restrictY;
  public bool restrictWithinRect;
  public Rect rect;

  public bool returnOffScreen; // returns to inside screen if dragged off screen (on drop)
  private Tween twnReturn;
  /// <summary>
  /// Check if this object is off screen and return to nearest position inside screen if so
  /// </summary>
  public void CheckOffScreen()
  {
    if (returnOffScreen)
    {
      Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
      if (pos.x < 0 || pos.x > Screen.width || pos.y < 0 || pos.y > Screen.height)
      {
        // calculate nearest inside screen position to the current offscreen position
        float oldZ = transform.position.z;
        Vector3 nearestInsideScreenPos = pos;
        if (pos.x < 0) nearestInsideScreenPos.x = 0;
        if (pos.x > Screen.width) nearestInsideScreenPos.x = Screen.width;
        if (pos.y < 0) nearestInsideScreenPos.y = 0;
        if (pos.y > Screen.height) nearestInsideScreenPos.y = Screen.height;
        // convert back to world position
        nearestInsideScreenPos = Camera.main.ScreenToWorldPoint(nearestInsideScreenPos);
        nearestInsideScreenPos.z = oldZ;

        DOTween.Kill(twnReturn);
        twnReturn = transform.DOMove(nearestInsideScreenPos, 0.5f);
      }
    }
  }

  // [Tooltip("Snaps to center if greater than 0")]
  public float centerSnapDistance;
  protected float centerSnapDistanceSqr;

  protected Vector3 offset;
  protected bool dragging;
  public bool IsDragging => dragging;

  protected virtual void OnMouseDown()
  {
    if (enabled) OnDragStart();
  }

  protected virtual void Start()
  {

  }

  public virtual void OnDragStart()
  {
    DOTween.Kill(twnReturn);

    if (zeroOnDragStart)
    {
      transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
    centerSnapDistanceSqr = centerSnapDistance * centerSnapDistance;
    offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    dragging = true;
    onDragStart?.Invoke();
  }

  public virtual void OnDrag(Vector3 direction)
  {

  }

  protected virtual void OnMouseUp()
  {
    if (enabled) OnDragStop();
  }

  public virtual void OnDragStop()
  {
    dragging = false;
    onDragStop?.Invoke();

    if (returnOffScreen) CheckOffScreen();
  }

  protected virtual void Update()
  {
    if (dragging)
    {
      Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
      Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

      if (restrictWithinRect)
      {
        curPosition.x = Mathf.Clamp(curPosition.x, rect.xMin, rect.xMax);
        curPosition.y = Mathf.Clamp(curPosition.y, rect.yMin, rect.yMax);
      }
      if (restrictX)
      {
        curPosition.x = transform.position.x;
      }
      if (restrictY)
      {
        curPosition.y = transform.position.y;
      }

      if (centerSnapDistance > 0)
      {
        // snap to parent zero if close enough
        if ((curPosition - transform.parent.position).sqrMagnitude < centerSnapDistanceSqr)
        {
          curPosition = transform.parent.position;
        }
      }


      Vector3 oldPos = transform.position;
      transform.position = curPosition;

      OnDrag(curPosition - oldPos);
    }
  }

  protected virtual void OnDrawGizmos()
  {
    if (restrictWithinRect)
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireCube(rect.center, rect.size);
    }
  }
}
