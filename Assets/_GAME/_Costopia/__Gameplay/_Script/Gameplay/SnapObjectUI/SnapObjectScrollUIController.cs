using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
public class SnapObjectScrollUIController : MonoSingleton<SnapObjectScrollUIController>
{
    [Header("Prefab References")]
    [SerializeField] private ObjectInScroll objectInScrollPrefab;
    [SerializeField] private SnapObjectUI snapObjectUIPrefab;

    [Header("Parent References")]
    [SerializeField] private Transform scrollContentParent;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private ShowObjectEffectUI showFx;
    [SerializeField] private Text counter;

    [Header("Pool Config")]
    [SerializeField] private int initScrollObjectAmount = 10;
    [SerializeField] private int initSnapObjectAmount = 3;

    private MiniPool<ObjectInScroll> _scrollObjectPool;
    private MiniPool<SnapObjectUI> _snapObjectPool;

    private readonly List<ObjectInScroll> _activeScrollObjects = new();
    private readonly List<SnapObjectUI> _activeSnapObjects = new();

    private CanvasScaler _mainScaler;
    private CanvasScaler MainScaler => _mainScaler ??= UIManager.Ins.ScreenContainer.GetComponent<CanvasScaler>();
    private Camera _mainCamera;
    private Camera MainCamera => _mainCamera ??= Camera.main;
    private int _totalSpawn;

    #region Init

    private void Start()
    {
        showFx.CanvasGroup.alpha = 0f;
        InitPools();
        gameObject.SetActive(false);
        SnapIncrease = 0;
    }

#if UNITY_EDITOR
        [Sirenix.OdinInspector.Button]
#endif
    public void AddData(SnapObjectUI.SnapObjectUIConfig dataConfig, bool setHint = true)
    {
        SpawnObjectInScroll(dataConfig, setHint);
        _totalSpawn = _activeScrollObjects.Count;
        UpdateCounterText();
    }

    public void Show()
    {
        showFx.Show();
    }

    private void Hide()
    {
        showFx.Hide();
    }

    private void UpdateCounterText()
    {
        int totalLeft = _totalSpawn - _activeScrollObjects.Count;
        counter.text = $"{totalLeft}/{_totalSpawn}";
    }

    private void InitPools()
    {
        if (!objectInScrollPrefab || !snapObjectUIPrefab)
        {
            return;
        }

        _scrollObjectPool = new MiniPool<ObjectInScroll>();
        _scrollObjectPool.OnInit(objectInScrollPrefab, initScrollObjectAmount, scrollContentParent);

        _snapObjectPool = new MiniPool<SnapObjectUI>();
        _snapObjectPool.OnInit(snapObjectUIPrefab, initSnapObjectAmount, transform);
    }
    #endregion

    #region ObjectInScroll

    public void ManualSetOrder(List<int> objectInScrollOrder)
    {
        if (objectInScrollOrder == null)
        {
            return;
        }

        int count = _activeScrollObjects.Count;
        if (objectInScrollOrder.Count != count)
        {
            return;
        }

        var seen = new bool[count];
        var reordered = new List<ObjectInScroll>(count);

        for (int i = 0; i < objectInScrollOrder.Count; i++)
        {
            int idx = objectInScrollOrder[i];
            if (idx < 0 || idx >= count)
            {
                return;
            }

            if (seen[idx])
            {
                return;
            }

            seen[idx] = true;
            reordered.Add(_activeScrollObjects[idx]);
        }

        _activeScrollObjects.Clear();
        _activeScrollObjects.AddRange(reordered);

        for (int i = 0; i < _activeScrollObjects.Count; i++)
        {
            var obj = _activeScrollObjects[i];
            if (obj != null)
                obj.transform.SetSiblingIndex(i);
        }
    }



    private ObjectInScroll SpawnObjectInScroll(SnapObjectUI.SnapObjectUIConfig config, bool setHint)
    {
        ObjectInScroll obj = _scrollObjectPool.Spawn();
        obj.SetUp(scrollRect, config, setHint);
        obj.transform.SetParent(scrollContentParent, false);
        _activeScrollObjects.Add(obj);
        return obj;
    }

    public void DespawnObjectInScroll(ObjectInScroll obj)
    {
        if (obj == null) return;
        if (_activeScrollObjects.Contains(obj))
            _activeScrollObjects.Remove(obj);
        _scrollObjectPool.Despawn(obj);
        UpdateCounterText();
        if (_activeScrollObjects.Count == 0)
        {
            Hide();
        }
    }
    #endregion

    #region SnapObjectUI
    public SnapObjectUI SpawnSnapObject(ObjectInScroll source)
    {
        var snapObj = _snapObjectPool.Spawn();
        snapObj.transform.SetParent(transform, false);
        snapObj.SetUp(source, MainScaler, MainCamera);
        _activeSnapObjects.Add(snapObj);
        return snapObj;
    }

    public void DespawnSnapObject(SnapObjectUI snapObj)
    {
        if (snapObj == null) return;
        if (_activeSnapObjects.Contains(snapObj))
            _activeSnapObjects.Remove(snapObj);
        _snapObjectPool.Despawn(snapObj);
        snapObj.transform.SetParent(transform, false);
    }
    #endregion

    #region Helper
    public void ClearAll()
    {
        foreach (var obj in _activeScrollObjects)
            _scrollObjectPool.Despawn(obj);
        foreach (var snap in _activeSnapObjects)
            _snapObjectPool.Despawn(snap);
        _activeScrollObjects.Clear();
        _activeSnapObjects.Clear();
    }

    public void ReleaseAll()
    {
        _scrollObjectPool?.Release();
        _snapObjectPool?.Release();
        _activeScrollObjects.Clear();
        _activeSnapObjects.Clear();
    }

    public bool IsSnapObjectActive => _activeSnapObjects.Count > 0;
    public int SnapIncrease { get; set; }

    public bool CanShowVFX => SnapIncrease % 3 == 0;

    #endregion
}
