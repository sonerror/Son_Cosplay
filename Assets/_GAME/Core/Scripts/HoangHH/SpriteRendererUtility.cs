using UnityEngine;

namespace HoangHH
{
  public class SpriteRendererUtility : H3MonoBehaviour
  {
    [SerializeField] private SpriteRenderer sr;

    public void SetMaskIndex(int index)
    {
      if (index < 0 || index > 3)
      {
        return;
      }
      // index to set mask
      sr.maskInteraction = (SpriteMaskInteraction)index;
    }
  }
}