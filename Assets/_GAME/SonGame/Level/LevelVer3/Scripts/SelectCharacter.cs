using UnityEngine;

public class SelectCharacter : MonoBehaviour, ISelectableItem
{
    [SerializeField] private int requiredStep;
    public bool CanSelect()
    {
        return StepManager.Ins == null ||
               StepManager.Ins.CurrentStep == requiredStep;
    }
    public void OnSelected()
    {
        Debug.Log("Item Selected");
    }

    public void OnDeselected()
    {
        Debug.Log("Item Deselected");
    }
}
