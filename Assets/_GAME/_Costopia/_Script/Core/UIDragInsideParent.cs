using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIDragInsideParent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
  [SerializeField] private Image dragHandle; // optional handle area

  [Header("Clamp Area")]
  // [Tooltip("Leave empty to use the immediate parent as the boundary.")]
  [SerializeField] private RectTransform boundary; // usually the parent

  [Header("Margins")]
  // [Tooltip("Margin (in UI pixels) kept from the boundary on each axis.")]
  [SerializeField] private Vector2 margin = new Vector2(16f, 16f);
  // [Tooltip("If true, margin is a percentage of boundary size (0..1).")]
  [SerializeField] private bool marginIsPercent = false;

  private RectTransform _rt;           // this object
  private RectTransform _parent;       // effective boundary
  private Canvas _canvas;
  private Vector2 _dragOffsetInParent; // offset from pointer to anchoredPosition (parent space)
  private readonly Vector3[] _parentWorld = new Vector3[4];
  private readonly Vector3[] _selfWorld = new Vector3[4];

  public System.Action onBeginDrag;
  public System.Action onEndDrag;

  public Image DragHandle => dragHandle;

  private void Awake()
  {
    _rt = GetComponent<RectTransform>();
    _parent = boundary ? boundary : _rt.parent as RectTransform;

    if (_parent == null)
      Debug.LogError($"{nameof(UIDragInsideParent)}: Needs a RectTransform parent/boundary.");

    _canvas = GetComponentInParent<Canvas>();
    if (_canvas == null)
      Debug.LogError($"{nameof(UIDragInsideParent)}: Must be under a Canvas.");
  }

  public void OnBeginDrag(PointerEventData eventData)
  {
    if (_parent == null) return;

    // Convert pointer to parent local space
    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parent, eventData.position, _canvas.worldCamera, out var localPoint))
    {
      _dragOffsetInParent = _rt.anchoredPosition - localPoint;
      onBeginDrag?.Invoke();
    }
  }

  public void OnDrag(PointerEventData eventData)
  {
    if (_parent == null) return;

    // Desired position in parent local space
    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parent, eventData.position, _canvas.worldCamera, out var localPoint))
    {
      var desired = localPoint + _dragOffsetInParent;

      // Set first, then correct using world-space corner tests (robust to anchors/pivots/scale)
      _rt.anchoredPosition = desired;
      KeepInsideParent();
    }
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    // no-op; hook for snap/animation if you want
    onEndDrag?.Invoke();
  }

  private void KeepInsideParent()
  {
    // Get boundary corners in world space
    _parent.GetWorldCorners(_parentWorld);

    // Compute pixel margins -> world offsets
    Vector2 actualMargin = marginIsPercent
        ? new Vector2(_parent.rect.width * margin.x, _parent.rect.height * margin.y)
        : margin;

    // Convert pixel margin to world distance along parent axes
    // (scale-safe: uses parent local->world scale)
    Vector3 marginWorldX = _parent.TransformVector(new Vector3(actualMargin.x, 0f, 0f));
    Vector3 marginWorldY = _parent.TransformVector(new Vector3(0f, actualMargin.y, 0f));

    // Boundary (shrunken) in world space
    float left = _parentWorld[0].x + Mathf.Abs(marginWorldX.x);
    float bottom = _parentWorld[0].y + Mathf.Abs(marginWorldY.y);
    float top = _parentWorld[1].y - Mathf.Abs(marginWorldY.y);
    float right = _parentWorld[2].x - Mathf.Abs(marginWorldX.x);

    // Get this rect's world corners at current tentative position
    _rt.GetWorldCorners(_selfWorld);

    float childLeft = _selfWorld[0].x;
    float childBottom = _selfWorld[0].y;
    float childTop = _selfWorld[1].y;
    float childRight = _selfWorld[2].x;

    // Compute world-space correction needed to keep fully inside boundary
    float dx = 0f, dy = 0f;
    if (childLeft < left) dx = left - childLeft;
    else if (childRight > right) dx = right - childRight;

    if (childBottom < bottom) dy = bottom - childBottom;
    else if (childTop > top) dy = top - childTop;

    if (dx != 0f || dy != 0f)
    {
      // Convert world delta to parent local delta, then apply to anchoredPosition
      Vector3 deltaLocal = _parent.InverseTransformVector(new Vector3(dx, dy, 0f));
      _rt.anchoredPosition += (Vector2)deltaLocal;
    }
  }
}
