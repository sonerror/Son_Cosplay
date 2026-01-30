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
    [SerializeField] private Canvas screenContainer;

    public Canvas ScreenContainer => screenContainer;
    [SerializeField] List<GamePlayScreen> screens = new List<GamePlayScreen>();
    public RectTransform[] canvasParent = new RectTransform[3];
    private GamePlayScreen[] uiActive = new GamePlayScreen[3];

    private Vector2 gameSize = new Vector2(1080, 1920);
    public Vector2 GameSize => gameSize;
    private bool isLandscape;
    public bool IsLandscape => isLandscape;
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(InitLunaOrientation());
    }

    private void Start()
    {
        // OpenUI(UIID.GameVideo);
        OpenUI(UIID.GamePlay);

    }
    private IEnumerator InitLunaOrientation()
    {
        yield return null;
        isLandscape = Screen.width > Screen.height;
        gameSize = isLandscape
            ? new Vector2(1920, 1080)
            : new Vector2(1080, 1920);
        resizeAllUI();
    }
    public bool IsOpenedUI(UIID ID)
    {
        return IsLoaded(ID) && uiActive[(int)ID].gameObject.activeInHierarchy;
    }

    public GamePlayScreen GetUI(UIID ID)
    {
        if (!IsLoaded(ID))
        {
            uiActive[(int)ID] =
                Instantiate(screens[(int)ID].gameObject, canvasParent[(int)ID]).GetComponent<GamePlayScreen>();
            uiActive[(int)ID].Resize(gameSize);
            uiActive[(int)ID].OnCreate();
        }
        return uiActive[(int)ID];
    }

    public GamePlayScreen OpenUI(UIID ID)
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
        isLandscape = Screen.width > Screen.height;
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