using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIID // id of UI
{
  GamePlay = 0,
  GameLoseScreen = 1,
}// remember set range of array in UIManager

public class UIManager : Singleton<UIManager>
{
  private Transform tf;
  public Transform Tf => tf ? tf : tf = transform;

  [SerializeField] List<UIScreen> screens = new List<UIScreen>();
  public RectTransform[] canvasParent = new RectTransform[3];
  private UIScreen[] uiActive = new UIScreen[3];

  private Vector2 gameSize = new Vector2(1080, 1920);
  public Vector2 GameSize => gameSize;

  protected override void Awake()
  {
    base.Awake();
  }

  private void Start()
  {
    // OpenUI(UIID.GameVideo);
    OpenUI(UIID.GamePlay);

  }

  public bool IsOpenedUI(UIID ID)
  {
    return IsLoaded(ID) && uiActive[(int)ID].gameObject.activeInHierarchy;
  }

  public UIScreen GetUI(UIID ID)
  {
    if (!IsLoaded(ID))
    {
      uiActive[(int)ID] =
          Instantiate(screens[(int)ID].gameObject, canvasParent[(int)ID]).GetComponent<UIScreen>();
      uiActive[(int)ID].Resize(gameSize);
      uiActive[(int)ID].OnCreate();
    }
    return uiActive[(int)ID];
  }

  public UIScreen OpenUI(UIID ID)
  {
    if (IsLoaded(ID))
    {
      if (uiActive[(int)ID].gameObject.activeInHierarchy) return uiActive[(int)ID];

      uiActive[(int)ID].gameObject.SetActive(true);
      uiActive[(int)ID].OnShow();
      return uiActive[(int)ID];
    }
    else
    {
      return GetUI(ID);
    }
  }

  public bool IsLoaded(UIID ID)
  {
    return uiActive[(int)ID] != null;
  }

  public void CloseUI(UIID ID)
  {
    if (IsLoaded(ID))
    {
      GetUI(ID).gameObject.SetActive(false);
    }
  }

  private void FixedUpdate()
  {
    float ratio = (float)Screen.width / (float)Screen.height;
    float heightTmp = (float)1080f / ratio;
    float widthTmp = (float)1920f * ratio;
    if (heightTmp <= 1920f)
    {
      gameSize.x = widthTmp;
      gameSize.y = 1920f;
    }

    if (widthTmp < 1080f)
    {
      gameSize.x = 1080f;
      gameSize.y = heightTmp;
    }
    resizeAllUI();
  }

  private void resizeAllUI()
  {
    for (int i = 0; i < uiActive.Length; i++)
    {
      if (!uiActive[i]) continue;
      uiActive[i].Resize(uiActive[i].fomatSize(gameSize));
    }
  }

}