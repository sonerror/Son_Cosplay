using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen : UIUnit
{
    private Transform tf;
    public Transform Tf => tf ? tf : tf = transform;
    public bool LimitX = false;
    public bool LimitY = false;

    public virtual void OnClose() { }
    public virtual void OnCreate() { }
    public virtual void OnShow() { }
    public virtual void OnHide() { }
    public virtual void Resize(Vector2 gameSize) { }

    public Vector2 fomatSize(Vector2 gameSize)
    {
        if (LimitX && gameSize.x > 1080)
            gameSize.x = 1080;
        if (LimitY && gameSize.y > 1920)
            gameSize.y = 1920;
        return gameSize;
    }
}
