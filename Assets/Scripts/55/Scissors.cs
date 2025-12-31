using System.Collections.Generic;
using HoangHH;
using UnityEngine;

public class Scissors : ItemDragBase
{

  public override void SetReady()
  {
    base.SetReady();
  }

  void Start()
  {

  }
  public override void OnActionMouseDown()
  {
    base.OnActionMouseDown();
    if (!IsReady) return;
    // anim.Play();
  }

  public override void OnActionMouseDrag()
  {
    base.OnActionMouseDrag();
  }

  public override void OnActionMouseUp()
  {
    base.OnActionMouseUp();

    if (!IsReady) return;
    // anim.Stop();
  }

}
