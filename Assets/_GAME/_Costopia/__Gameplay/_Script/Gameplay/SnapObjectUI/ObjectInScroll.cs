using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;
public class ObjectInScroll : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Image imgObject;
    [SerializeField] private RectTransform rectTransform;
    //[SerializeField] private InGameHintSnapObjectUI hint;

    private ScrollRect _scrollRect;
    private SnapObjectUI _snapObjectUI;

    private Vector2 _startPointerPos;
    private bool _isDraggingSnap;
    private bool _isDraggingScroll;
    private bool _hasDeterminedDragType;
    private bool _scrollBeginSent;

    private float _initSizeDeltaX;

    private SnapObjectUI.SnapObjectUIConfig _snapConfig;
    public SnapObjectUI.SnapObjectUIConfig Config => _snapConfig;

    [Header("Config")]
    [SerializeField] private float dragThreshold = 8f;
    [SerializeField] private float minVerticalDeltaToSnap = 5f;

    private void Awake()
    {
        _initSizeDeltaX = rectTransform.sizeDelta.x;
    }

    public Image Image => imgObject;

    public void SetUp(ScrollRect scrollRect, SnapObjectUI.SnapObjectUIConfig config, bool setHint)
    {
        _scrollRect = scrollRect;
        _snapConfig = config;
        imgObject.sprite = _snapConfig.sprite;
        if (setHint)
        {
            //hint.SetStep(LevelBase.Instance.CurrentStep);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _startPointerPos = eventData.position;
        _isDraggingSnap = false;
        _isDraggingScroll = false;
        _hasDeterminedDragType = false;
        _scrollBeginSent = false;

        _scrollRect.OnInitializePotentialDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dragDelta = eventData.position - _startPointerPos;

        if (!_hasDeterminedDragType && dragDelta.magnitude > dragThreshold)
        {
            if (dragDelta.y > minVerticalDeltaToSnap)
            {
                StartSnapDrag();
                _isDraggingSnap = true;
            }
            else
            {
                _isDraggingScroll = true;

                if (!_scrollBeginSent)
                {
                    _scrollRect.OnBeginDrag(eventData);
                    _scrollBeginSent = true;
                }
            }

            _hasDeterminedDragType = true;
        }

        if (!_hasDeterminedDragType) return;

        if (_isDraggingSnap)
        {
            _snapObjectUI?.OnDragging(eventData.position);
        }
        else if (_isDraggingScroll)
        {
            _scrollRect.OnDrag(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isDraggingSnap)
        {
            _snapObjectUI?.OnRelease();
        }
        else if (_isDraggingScroll && _scrollBeginSent)
        {
            _scrollRect.OnEndDrag(eventData);
        }

        // Reset flags
        _isDraggingSnap = false;
        _isDraggingScroll = false;
        _hasDeterminedDragType = false;
        _scrollBeginSent = false;
    }

    private void StartSnapDrag()
    {
        _snapObjectUI = SnapObjectScrollUIController.Instance.SpawnSnapObject(this);
        _snapObjectUI.BeginDrag(_startPointerPos);
        imgObject.enabled = false;

        rectTransform.DOSizeDelta(new Vector2(0, rectTransform.sizeDelta.y), 0.25f)
            .SetEase(Ease.InBack);
    }

    [Sirenix.OdinInspector.Button]
    public void AutoSnap()
    {
        this.WaitToDo(() =>
        {
            _snapObjectUI = SnapObjectScrollUIController.Instance.SpawnSnapObject(this);
            if (_snapObjectUI.AutoSnap())
            {
                imgObject.enabled = false;
                rectTransform.DOSizeDelta(new Vector2(0, rectTransform.sizeDelta.y), 0.25f)
                    .SetEase(Ease.InBack);
            }
            else
            {
                _snapObjectUI.DespawnSelf();
            }
        }, 0.5f);
    }

    public void Restore()
    {
        rectTransform.DOSizeDelta(new Vector2(_initSizeDeltaX, rectTransform.sizeDelta.y), 0.25f)
            .SetEase(Ease.OutBack);
    }

    private void OnDisable()
    {
        _snapObjectUI = null;
        _isDraggingSnap = false;
        _isDraggingScroll = false;
        _hasDeterminedDragType = false;
        _scrollBeginSent = false;
        rectTransform.sizeDelta = new Vector2(_initSizeDeltaX, rectTransform.sizeDelta.y);
    }

    public void DespawnSelf()
    {
        SnapObjectScrollUIController.Instance.DespawnObjectInScroll(this);
    }
}
