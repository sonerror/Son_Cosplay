
using UnityEngine;

namespace HoangHH
{
  public class FullBoundScreenObject : H3MonoBehaviour
  {
    [SerializeField] private Camera cam;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private float marginVertical;
    [SerializeField] private float marginHorizontal;
    [SerializeField] private float paddingBottom;
    [SerializeField] private float zPos;

    [SerializeField] private Transform[] constantSizeChild;

    [SerializeField] private bool isScaleBaseOnWidth;
    [SerializeField] private bool isScaleBaseOnHeight;

    [Header("Control Size")]
    [SerializeField] private bool hasSizeBound;
    [SerializeField] private Vector2 maxSizeXY;

    [SerializeField] private Vector2 screenSize;
    [SerializeField] private Vector2 srSize;

    private void Awake()
    {
      CenterObject();
    }


    private void CenterObject()
    {
      Vector3 srScale = sr.transform.localScale;
      // make the sprite fit all the screen
      srSize.x = sr.sprite.bounds.size.x;
      srSize.y = sr.sprite.bounds.size.y;
      screenSize = new Vector2(Screen.width, Screen.height);
      float worldScreenHeight = cam.orthographicSize * 2;
      float worldScreenWidth = worldScreenHeight / screenSize.y * screenSize.x;
      if (isScaleBaseOnWidth && !isScaleBaseOnHeight)
      {
        sr.transform.localScale = new Vector3(worldScreenWidth / srSize.x - marginHorizontal, worldScreenWidth / srSize.x - marginHorizontal, 1);
      }
      else if (isScaleBaseOnHeight && !isScaleBaseOnWidth)
      {
        sr.transform.localScale = new Vector3(worldScreenHeight / srSize.y - marginVertical, worldScreenHeight / srSize.y - marginVertical, 1);
      }
      else
      {
        sr.transform.localScale = new Vector3(worldScreenWidth / srSize.x - marginHorizontal,
            worldScreenHeight / srSize.y - marginVertical, 1);
      }

      if (hasSizeBound)
      {
        if (sr.transform.localScale.x > maxSizeXY.x)
        {
          sr.transform.localScale = new Vector3(maxSizeXY.x, sr.transform.localScale.y, 1);
        }

        if (sr.transform.localScale.y > maxSizeXY.y)
        {
          sr.transform.localScale = new Vector3(sr.transform.localScale.x, maxSizeXY.y, 1);
        }
      }


      // center the sprite
      Vector3 newPos = cam.transform.position;
      newPos.z = zPos;
      newPos.y += paddingBottom;
      sr.transform.position = newPos;

      float xScale = sr.transform.localScale.x / srScale.x;
      float yScale = sr.transform.localScale.y / srScale.y;
      for (int index = 0; index < constantSizeChild.Length; index++)
      {
        Transform child = constantSizeChild[index];
        child.localScale = new Vector3(child.localScale.x * xScale, child.localScale.y * yScale, 1);
      }
    }
  }
}
