using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseDownAction : MonoBehaviour
{
    public UnityEvent action;

    private void OnMouseDown()
    {
        action.Invoke();
    }
}
