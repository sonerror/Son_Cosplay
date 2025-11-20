using System;
using System.Collections.Generic;

using UnityEngine;

namespace HoangHH
{
  [Serializable]
  public struct PairFloat
  {
    public float minValue;
    public float maxValue;
  }
  [Serializable]
  public struct ConstraintBox
  {
    public PairFloat x;
    public PairFloat y;
  }

  public class MoveConstraintCollider : H3MonoBehaviour
  {
    [SerializeField] private Camera mainCam;
    [SerializeField] private float lerpSpeed = 0.8f;

    [SerializeField] private Vector3 offset;
    [SerializeField] private bool isClampMove;
    [SerializeField] private bool isClampByX;
    [SerializeField] private bool isClampByY;

    private bool _canInput = true;
    public bool isLock;

    private float minX; // -2 
    private float maxX; // 2
    private float minY; // -4.4
    private float maxY; // -1.4

    [SerializeField] private List<ConstraintBox> constraintBoxes = new List<ConstraintBox>();


    private bool _isDrag;
    private Vector3 _dragPos;
    private Collider2D _collider2D;

    public void SetLock(bool value)
    {
      isLock = value;
      _collider2D.enabled = !isLock;
    }

    private void Awake()
    {
      _collider2D = GetComponent<Collider2D>();
    }

    private void Start()
    {
      if (constraintBoxes.Count == 0) return;
      minX = constraintBoxes[0].x.minValue;
      maxX = constraintBoxes[0].x.maxValue;
      minY = constraintBoxes[0].y.minValue;
      maxY = constraintBoxes[0].y.maxValue;
      for (int i = 1; i < constraintBoxes.Count; i++)
      {
        if (constraintBoxes[i].x.minValue < minX)
        {
          minX = constraintBoxes[i].x.minValue;
        }
        if (constraintBoxes[i].x.maxValue > maxX)
        {
          maxX = constraintBoxes[i].x.maxValue;
        }
        if (constraintBoxes[i].y.minValue < minY)
        {
          minY = constraintBoxes[i].y.minValue;
        }
        if (constraintBoxes[i].y.maxValue > maxY)
        {
          maxY = constraintBoxes[i].y.maxValue;
        }
      }
    }

    private void Update()
    {
      if (_isDrag && _canInput && !isLock)
      {
        // lerp the object to the drag position
        Tf.position = Vector3.Lerp(Tf.position, _dragPos, lerpSpeed);
      }
    }

    private void OnMouseDown()
    {
      if (_canInput)
      {
        SetDrag(true);
        // AudioManager.PlaySFX(clickSound.clip, clickSound.volume);
      }
    }

    private void OnMouseUp()
    {
      SetDrag(false);
    }

    private void OnMouseDrag()
    {
      if (_canInput && _isDrag)
      {
        Drag();
      }
    }

    public void CanInput(bool value)
    {
      _canInput = value;
      if (!_canInput) SetDrag(false);
    }

    private void SetDrag(bool p0)
    {
      _isDrag = p0;
      if (!_isDrag) return;
      if (_canInput) Drag();
    }

    private void Drag()
    {
      if (isClampMove) SetClampMove();
      else SetMove();
    }

    private void SetMove()
    {
      Vector3 pos = mainCam.ScreenToWorldPoint(Input.mousePosition);
      pos += offset;
      pos.z = 0;
      // check position in which constraint box
      for (int i = 0; i < constraintBoxes.Count; i++)
      {
        if (pos.x >= constraintBoxes[i].x.minValue && pos.x <= constraintBoxes[i].x.maxValue &&
            pos.y >= constraintBoxes[i].y.minValue && pos.y <= constraintBoxes[i].y.maxValue)
        {
          _dragPos = pos;
          return;
        }
      }
    }

    private void SetClampMove()
    {
      // move the object base on the distance between the init drag position and the current mouse position
      _dragPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
      _dragPos += offset;
      _dragPos.z = 0;
      if (isClampByX) // clamp by x 
      {
        _dragPos.x = Mathf.Clamp(_dragPos.x, minX, maxX);
      }
      else
      {
        // find the nearest x value in the constraint box
        for (int i = 0; i < constraintBoxes.Count; i++)
        {
          if (!(_dragPos.y >= constraintBoxes[i].y.minValue) ||
              !(_dragPos.y <= constraintBoxes[i].y.maxValue)) continue;
          _dragPos.x = Mathf.Clamp(_dragPos.x, constraintBoxes[i].x.minValue, constraintBoxes[i].x.maxValue);
          break;
        }
      }
      if (isClampByY)
      {
        _dragPos.y = Mathf.Clamp(_dragPos.y, minY, maxY);
      }
      else
      {
        // find the nearest y value in the constraint box
        for (int i = 0; i < constraintBoxes.Count; i++)
        {
          if (!(_dragPos.x >= constraintBoxes[i].x.minValue) ||
              !(_dragPos.x <= constraintBoxes[i].x.maxValue)) continue;
          _dragPos.y = Mathf.Clamp(_dragPos.y, constraintBoxes[i].y.minValue, constraintBoxes[i].y.maxValue);
          break;
        }
      }
    }

  }
}
