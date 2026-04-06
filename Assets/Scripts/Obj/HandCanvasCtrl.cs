using System.Collections;
using DG.Tweening;
using UnityEngine;

public class HandCanvasCtrl : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Vector2 startPosOffset = new Vector2(30f, -30f);
    [SerializeField] private Vector2 endPosOffset = new Vector2(30f, -30f);

    private RectTransform _rt;
    private RectTransform RT
    {
        get
        {
            if (_rt == null) _rt = GetComponent<RectTransform>();
            return _rt;
        }
    }

    private Vector2 _startPos;
    private Vector2 _endPos;
    private Coroutine _loopCoroutine;

    // ─── Public API ───────────────────────────────────────────────────────────

    public void Show(RectTransform startRT, Transform endWorldTf)
    {
        if (startRT == null || endWorldTf == null)
        {
            Debug.LogWarning("[HandCanvasCtrl] startRT hoặc endWorldTf null");
            return;
        }

        Vector2 startPos = CanvasLocalPos(startRT.position) + startPosOffset;
        Vector2 endPos = WorldToCanvasPos(endWorldTf.position) + endPosOffset;
        Show(startPos, endPos);
    }

    public void Show(Vector2 startPos, Vector2 endPos)
    {
        gameObject.SetActive(true);
        _startPos = startPos;
        _endPos = endPos;
        Play();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        StopLoop();
        RT.DOKill();
    }

    // ─── Private ──────────────────────────────────────────────────────────────

    private void Play()
    {
        StopLoop();
        RT.DOKill();

        RT.anchoredPosition = _startPos;
        animator.SetTrigger("HandDown");

        RT.DOAnchorPos(_endPos, 1f)
            .SetDelay(0.75f)
            .OnComplete(() => animator.SetTrigger("HandUp"));

        _loopCoroutine = StartCoroutine(IELoop());
    }

    private IEnumerator IELoop()
    {
        yield return DTPCache.GetWFS(3f);
        if (gameObject.activeSelf)
            Play();
    }

    private void StopLoop()
    {
        if (_loopCoroutine != null)
        {
            StopCoroutine(_loopCoroutine);
            _loopCoroutine = null;
        }
    }

    // ─── Convert position ─────────────────────────────────────────────────────

    private Vector2 CanvasLocalPos(Vector3 worldPos)
    {
        RectTransform parentRT = RT.parent as RectTransform;
        Camera cam = GetCanvasCamera();
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRT, screenPos, cam, out Vector2 localPos);
        return localPos;
    }

    private Vector2 WorldToCanvasPos(Vector3 worldPos)
    {
        RectTransform parentRT = RT.parent as RectTransform;
        Camera cam = Camera.main;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRT, screenPos, GetCanvasCamera(), out Vector2 localPos);
        return localPos;
    }

    private Camera GetCanvasCamera()
    {
        Canvas rootCanvas = RT.GetComponentInParent<Canvas>();
        if (rootCanvas == null) return null;
        rootCanvas = rootCanvas.rootCanvas;
        if (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
        return rootCanvas.worldCamera;
    }
}