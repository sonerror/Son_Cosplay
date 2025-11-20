using UnityEngine;

namespace VinhLB
{
  public static class VinhLBLayer
  {
    public static readonly int Default = LayerMask.NameToLayer("Default");
    public static readonly int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
    public static readonly int UI = LayerMask.NameToLayer("UI");
    public static readonly int Draggable = LayerMask.NameToLayer("Draggable");
    public static readonly int Wall = LayerMask.NameToLayer("Wall");

  }
}