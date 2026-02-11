public interface ISelectableItem
{
    void OnSelected();
    void OnDeselected();
    bool CanSelect();
}
