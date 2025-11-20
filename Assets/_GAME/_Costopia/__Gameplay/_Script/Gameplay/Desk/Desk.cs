using System.Collections.Generic;
using HoangHH;
using UnityEngine;

namespace Costopia.Gameplay
{
  public class Desk : H3MonoBehaviour
  {
    [SerializeField] private List<DeskSlot> deskSlots;
    [SerializeField] private float distancePerEachSlot = 10f;
    [SerializeField] private float slotZOffset = -0.1f; // Offset to prevent z-fighting

    [SerializeField] private float snapSpeed = 10f;
    [SerializeField] private float minDragDistanceToSnap = 0.5f;
    [SerializeField] private float maxSnapDuration = 0.3f; // seconds
    private DeskSlot _currentSlot;
    private float _dragStartTime;
    private bool _isDragging;

    private Dictionary<DeskSlot, int> _slotIndices;

    private Vector3 _startMousePosition;
    private Vector3 _startPosition;
    private float _targetX;

    public int SlotCount => deskSlots.Count;

    private void Start()
    {
      SetCachingIndex();
      DefineFirstSlot();
    }

    private void Update()
    {
      if (_isDragging) return;
      Vector3 pos = Tf.localPosition;
      pos.x = Mathf.Lerp(pos.x, _targetX, Time.deltaTime * snapSpeed);
      Tf.localPosition = pos;
    }

    private void OnMouseDown()
    {
      _isDragging = true;
      _startMousePosition = Input.mousePosition;
      _startPosition = Tf.localPosition;
      _dragStartTime = Time.time;
    }

    private void OnMouseDrag()
    {
      if (!_isDragging) return;

      Vector3 mouseDelta = Input.mousePosition - _startMousePosition;
      float dragDistance = mouseDelta.x * 0.01f; // sensitivity

      Vector3 newPosition = _startPosition + new Vector3(dragDistance, 0, 0);
      Tf.localPosition = newPosition;
    }

    private void OnMouseUp()
    {
      if (!_isDragging) return;
      _isDragging = false;

      float dragDuration = Time.time - _dragStartTime;
      float dragDelta = Tf.localPosition.x - _startPosition.x;
      float dragDistance = Mathf.Abs(dragDelta);

      // Check if it's a valid fast swipe
      bool isFastEnough = dragDuration <= maxSnapDuration;
      bool isFarEnough = dragDistance >= minDragDistanceToSnap;

      if (!isFastEnough || !isFarEnough)
      {
        // Too slow or too short → snap back
        _targetX = _startPosition.x;
        return;
      }

      bool isMovingRight = dragDelta < 0;
      _currentSlot = GetNextSlot(isMovingRight);
      SetTargetPosition(_currentSlot);
    }

    private void SetTargetPosition(DeskSlot slot, bool setIntermediate = false)
    {
      _targetX = -_currentSlot.Tf.localPosition.x;
      if (setIntermediate)
      {
        Tf.localPosition = new Vector3(_targetX, Tf.localPosition.y, Tf.localPosition.z);
      }
      else
      {
        // H3// AudioManager.PlaySFX(SystemSfx.Whoosh);
      }
    }

    private DeskSlot GetNextSlot(bool isMovingRight)
    {
      if (deskSlots is null || deskSlots.Count == 0) return null;

      // Determine the index of the current slot based on the direction of movement
      int currentIndex = _slotIndices[_currentSlot];
      int nextIndex = isMovingRight ? currentIndex + 1 : currentIndex - 1;

      // Ensure the index is within bounds
      if (nextIndex < 0) nextIndex = 0;
      if (nextIndex >= deskSlots.Count) nextIndex = deskSlots.Count - 1;

      return deskSlots[nextIndex];
    }

    private void DefineFirstSlot()
    {
      if (deskSlots is null || deskSlots.Count == 0) return;
      // Set the first slot as the current slot
      _currentSlot = deskSlots[0];
      // Set the initial position of the desk to the first slot
      _targetX = -_currentSlot.Tf.localPosition.x;
      Tf.localPosition = new Vector3(_targetX, Tf.localPosition.y, Tf.localPosition.z);
    }

    private void SetCachingIndex()
    {
      _slotIndices = new Dictionary<DeskSlot, int>();
      for (int i = 0; i < deskSlots.Count; i++) _slotIndices[deskSlots[i]] = i;
    }


    public void SetDeskPositionAtIndex(int index)
    {
      if (deskSlots is null || deskSlots.Count == 0) return;
      if (index < 0 || index >= deskSlots.Count) return;

      _currentSlot = deskSlots[index];

      SetTargetPosition(_currentSlot, !Application.isPlaying);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
      if (deskSlots is null || deskSlots.Count == 0) return;
      SortSlots(deskSlots[0].Tf.localPosition.y);
    }

    private void OnDrawGizmosSelected()
    {
      if (deskSlots == null || deskSlots.Count == 0) return;

      Gizmos.color = Color.cyan;

      foreach (DeskSlot slot in deskSlots)
      {
        if (slot == null) continue;

        Vector3 center = slot.Tf.position;

        float height = slot.Tf.localPosition.y * 2f;
        float width = distancePerEachSlot;
        float depth = 0.2f; // Adjust for visual thickness

        Vector3 size = new Vector3(width, height, depth);

        Gizmos.DrawWireCube(center, size);
      }
    }


    private void TrySortSlots(float yPosition)
    {
      // find all DeskSlot components in the children of this GameObject
      deskSlots = new List<DeskSlot>(GetComponentsInChildren<DeskSlot>());
      if (deskSlots is null || deskSlots.Count == 0) return;
      // add the DeskSlot components to the dictionary with their index
      SortSlots(yPosition);
      SetCachingIndex();
      DefineFirstSlot();
    }

    private void SortSlots(float yPosition)
    {
      int count = deskSlots.Count;
      if (count == 0) return;

      int startX = 0;
      // if (count % 2 == 0)
      //     // Even → start at -((count / 2) - 1)
      //     startX = -(count / 2 - 1);
      // else
      //     // Odd → start at -(count / 2)
      //     startX = -(count / 2);

      for (int i = 0; i < count; i++)
      {
        float x = (startX + i) * distancePerEachSlot;
        deskSlots[i].Tf.localPosition = new Vector3(x, yPosition, slotZOffset);
      }
    }


#endif
  }
}