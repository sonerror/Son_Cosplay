using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TapSelectItem : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onSelected;
    public UnityEvent onDeselected;

    private bool _isSelected;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isSelected) return;
        Select();
    }

    public void Select()
    {
        if (_isSelected) return;

        _isSelected = true;
        Debug.Log("Item Selected");
        onSelected?.Invoke();
    }

    public void Deselect()
    {
        if (!_isSelected) return;

        _isSelected = false;
        Debug.Log("Item Deselected");
        onDeselected?.Invoke();
    }

    public bool IsSelected => _isSelected;
}
