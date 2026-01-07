using System.Collections.Generic;
using HoangHH;
using UnityEngine;

public class Scissors : ItemDragBase
{

  [SerializeField] private Animator anim;
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
    anim.SetTrigger("Play");
    // SoundManager.Ins.PlaySoundLoop(FxType.Cut);
  }

  public override void OnActionMouseDrag()
  {
    base.OnActionMouseDrag();
  }

  public override void OnActionMouseUp()
  {
    base.OnActionMouseUp();
    anim.SetTrigger("Idle");
    // SoundManager.Ins.StopSoundLoop(FxType.Cut);
    if (!IsReady) return;
  }

}
