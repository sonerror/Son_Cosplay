using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUnit : MonoBehaviour
{
    private RectTransform rectTf;
    public RectTransform RectTf => rectTf ? rectTf : rectTf = GetComponent<RectTransform>();
}
