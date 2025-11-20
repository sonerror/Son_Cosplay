using HoangHH;
using UnityEditor;
using UnityEngine;

public class BoundTransformMoveOnTouch : H3MonoBehaviour
{
  [SerializeField] private Vector2 boundMin;
  [SerializeField] private Vector2 boundMax;
  [SerializeField] private float lerpSpeed = 5f;
  [SerializeField] private float worldBottomLimitY = -3f;
  [SerializeField] private bool inverseX;
  [SerializeField] private bool inverseY;
  [SerializeField] private float dragThreshold = 10f; // in pixels
  private Camera _camera;
  private bool _hasStartedDragging;
  private bool _isDraggingValid;
  private Vector3 _mouseStartPosition;
  private Vector3 _startLocalPosition;

  private void Start()
  {
    _camera = Camera.main;
    _startLocalPosition = Tf.localPosition;
  }

  private void LateUpdate()
  {
    if (Input.GetMouseButtonDown(0))
    {
      float worldLimitScreenY = _camera.WorldToScreenPoint(new Vector3(0f, worldBottomLimitY, 0f)).y;
      float mouseY = Input.mousePosition.y;

      _isDraggingValid = mouseY >= worldLimitScreenY;
      _mouseStartPosition = Input.mousePosition;
      _hasStartedDragging = false; // reset drag state
    }

    if (Input.GetMouseButton(0))
    {
      float worldLimitScreenY = _camera.WorldToScreenPoint(new Vector3(0f, worldBottomLimitY, 0f)).y;
      float mouseY = Input.mousePosition.y;

      if (mouseY < worldLimitScreenY && !_isDraggingValid)
        return;

      if (!_hasStartedDragging)
      {
        // Check drag distance
        if ((Input.mousePosition - _mouseStartPosition).sqrMagnitude >= dragThreshold * dragThreshold)
          _hasStartedDragging = true;
        else
          return; // not dragging yet
      }

      // --- Movement starts only here ---
      float normalizedX = Mathf.Clamp01(Input.mousePosition.x / Screen.width);
      float normalizedY = Mathf.Clamp01(mouseY / Screen.height);

      if (inverseX) normalizedX = 1f - normalizedX;
      if (inverseY) normalizedY = 1f - normalizedY;

      float targetX = Mathf.Lerp(boundMin.x, boundMax.x, normalizedX);
      float targetY = Mathf.Lerp(boundMin.y, boundMax.y, normalizedY);

      Vector3 targetLocalPosition = new Vector3(targetX, targetY, Tf.localPosition.z);
      Tf.localPosition = Vector3.Lerp(Tf.localPosition, targetLocalPosition, Time.deltaTime * lerpSpeed);
    }
    else if (Input.GetMouseButtonUp(0))
    {
      _isDraggingValid = false;
      _hasStartedDragging = false;
    }
    else
    {
      Tf.localPosition = Vector3.Lerp(Tf.localPosition, _startLocalPosition, Time.deltaTime * lerpSpeed);
    }
  }

#if UNITY_EDITOR
  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.green;

    Transform reference = Tf.parent ? Tf.parent : null;

    Vector3 bottomLeft = reference
        ? reference.TransformPoint(new Vector3(boundMin.x, boundMin.y, 0f))
        : new Vector3(boundMin.x, boundMin.y, 0f);
    Vector3 bottomRight = reference
        ? reference.TransformPoint(new Vector3(boundMax.x, boundMin.y, 0f))
        : new Vector3(boundMax.x, boundMin.y, 0f);
    Vector3 topRight = reference
        ? reference.TransformPoint(new Vector3(boundMax.x, boundMax.y, 0f))
        : new Vector3(boundMax.x, boundMax.y, 0f);
    Vector3 topLeft = reference
        ? reference.TransformPoint(new Vector3(boundMin.x, boundMax.y, 0f))
        : new Vector3(boundMin.x, boundMax.y, 0f);

    Gizmos.DrawLine(bottomLeft, bottomRight);
    Gizmos.DrawLine(bottomRight, topRight);
    Gizmos.DrawLine(topRight, topLeft);
    Gizmos.DrawLine(topLeft, bottomLeft);

    // ➕ Add this for worldBottomLimitY debug
    Gizmos.color = Color.red;
    float zRef = Tf != null ? Tf.position.z : 0f; // Match object's Z
    Vector3 left = new Vector3(boundMin.x - 1f, worldBottomLimitY, zRef);
    Vector3 right = new Vector3(boundMax.x + 1f, worldBottomLimitY, zRef);
    Gizmos.DrawLine(left, right);

    Handles.Label(left + Vector3.up * 0.1f, $"Bottom Limit Y = {worldBottomLimitY:F2}");
  }
#endif

  public void ResetToStartPos()
  {
    Tf.localPosition = _startLocalPosition;
  }
}