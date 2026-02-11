using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SnapObjectUI : MonoBehaviour
{
    [Serializable]
    public class SnapObjectUIConfig
    {

        public Sprite sprite;

        public float customScale = 1f;

        public float detalScale = 3f;


        public List<SnapPointUI> snapPoint;


        public float distanceAcceptSnap = 2f;

        public AudioClip snapSound;

        // Runtime only
        public Action onStartDrag;
        public Action onRelease;
        public Action onSnap;
    }

    // [SerializeField] private SnapObjectUIMatConfig materialConfig;
    [SerializeField] private Image imgObject;
    [SerializeField] private Vector2 dragOffset = new Vector2(0f, 100f);
    [SerializeField] private float returnDuration = 0.35f;
    [SerializeField] private float enlargeDuration = 0.4f;
    [SerializeField] private float dragYOffset = 80f;

    private ObjectInScroll _objectInScroll;
    private RectTransform _rectTransform;
    private RectTransform _parentRT;
    private Camera _mainCamera;
    private readonly Vector3[] _corners = new Vector3[4];
    private CanvasScaler _mainScaler;

    private Vector2 _originalCenter;
    private Vector2 _originalSize;
    private Vector2 _targetSize;
    private Vector2 _dragOffsetFromCenter;
    private bool _isDragging;
    private Vector2 _targetPosition;
    private Vector2 _velocity; // cho SmoothDamp
    [SerializeField] private float followSmooth = 0.05f; // nhỏ = bám chặt, lớn = trễ hơn
    private bool _isSetMaterial;
    private Sequence _outlineSequence;

    private RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }


    public void SetUp(ObjectInScroll objectInScroll, CanvasScaler mainScaler, Camera mainCamera)
    {
        /*if (!_isSetMaterial)
        {
            _isSetMaterial = true;
            Material material = materialConfig.CreateInstanceMaterial();
            imgObject.material = material;
        }*/

        _mainScaler = mainScaler;
        _objectInScroll = objectInScroll;
        _parentRT = RectTransform.parent as RectTransform;
        _mainCamera = mainCamera;

        imgObject.sprite = objectInScroll.Image.sprite;
        imgObject.preserveAspect = objectInScroll.Image.preserveAspect;
        imgObject.color = objectInScroll.Image.color;

        RectTransform.anchorMin = RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        RectTransform.pivot = new Vector2(0.5f, 0.5f);
        RectTransform.localScale = Vector3.one;

        GetSourceRectOnCanvas(out _originalCenter, out _originalSize);
        RectTransform.anchoredPosition = _originalCenter;
        RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _originalSize.x);
        RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _originalSize.y);
        float _customScale = _objectInScroll.Config.customScale;
        if (UIManager.Ins.IsLandscape)
        {
            _customScale = _objectInScroll.Config.customScale * _objectInScroll.Config.detalScale;
        }

        _targetSize = CalculateWorldProjectedSize(_mainCamera, imgObject.sprite, _customScale);

        RectTransform.DOSizeDelta(_targetSize, enlargeDuration).SetEase(Ease.OutCubic);
        FadeOutline(true);
        _objectInScroll.Config.onStartDrag?.Invoke();
    }

    private void FadeOutline(bool fadeOut)
    {
        /*_outlineSequence?.Kill();
        _outlineSequence = DOTween.Sequence();
        _outlineSequence.Append(imgObject.material.DOFloat(fadeOut ? 0f : materialConfig.OutlineWidth,
            materialConfig.OutlineWidthID,
            fadeOut ? returnDuration : enlargeDuration));
        _outlineSequence.Join(imgObject.material.DOFloat(fadeOut ? 0f : materialConfig.FadeStrength,
            materialConfig.FadeStrengthID,
            fadeOut ? returnDuration : enlargeDuration));
        _outlineSequence.Join(imgObject.material.DOFloat(fadeOut ? 0f : materialConfig.OutlineUVOffset,
            materialConfig.OutlineUVOffsetID,
            fadeOut ? returnDuration : enlargeDuration));*/
    }

    public void BeginDrag(Vector2 pointerScreenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentRT, pointerScreenPos, null, out var localPointerPos);

        // store the offset between center and touch, including upward lift
        _dragOffsetFromCenter = RectTransform.anchoredPosition - localPointerPos + new Vector2(0f, dragYOffset);
        _targetPosition = RectTransform.anchoredPosition;
        _isDragging = true;
    }

    public void OnDragging(Vector2 pointerScreenPos)
    {
        if (!_isDragging) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentRT, pointerScreenPos, null, out var localPointerPos);

        // keep the Y offset during drag
        _targetPosition = localPointerPos + _dragOffsetFromCenter;
    }

    private void Update()
    {
        if (_isDragging)
        {
            // SmoothDamp giúp di chuyển bám tay nhưng vẫn mượt
            RectTransform.anchoredPosition = Vector2.SmoothDamp(
                RectTransform.anchoredPosition, _targetPosition,
                ref _velocity, followSmooth);
        }
    }

    public void OnRelease()
    {
        _isDragging = false;
        CheckSnap();
    }

    private void CheckSnap()
    {
        SnapPointUI snapPoint = TrySnapLogic(out bool isNearSnapPoint);
        if (snapPoint)
        {
            Vector3 worldSnapPos = snapPoint.transform.position;
            Vector2 screenSnapPos = RectTransformUtility.WorldToScreenPoint(_mainCamera, worldSnapPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentRT, screenSnapPos, null, out Vector2 localSnapPos);
            Vector2 startMovePos = RectTransform.anchoredPosition;
            DOTween.To(() => 0f, t =>
                {
                    RectTransform.anchoredPosition = Vector2.LerpUnclamped(startMovePos, localSnapPos, t);
                }, 1f, 0.1f)
                .SetEase(Ease.OutCubic)
                .OnComplete(Snap);
            return;
            void Snap()
            {
                SoundManager.PlaySFX(_objectInScroll.Config.snapSound);
                snapPoint.OnSnap(this);
                _objectInScroll.Config.onSnap?.Invoke();
                _objectInScroll.DespawnSelf();
                DespawnSelf();
                SnapObjectScrollUIController.Instance.SnapIncrease++;
                if (SnapObjectScrollUIController.Instance.CanShowVFX)
                {
                    ControllerDoneVFX.Instance.SpawnVFX(snapPoint.Tf);
                }
                else
                {
                    ControllerDoneVFX.Instance.SpawnSnapVFX(snapPoint.Tf);
                }
                LevelRumiAilen.Ins.PlayPositiveEmojiOnSnap();
            }
        }
        // --- NO SNAP ---
        _objectInScroll.Image.enabled = false;
        _objectInScroll.Restore();
        Vector2 startPos = RectTransform.anchoredPosition;
        Vector2 startSize = RectTransform.sizeDelta;
        FadeOutline(false);
        DOTween.To(() => 0f, t =>
        {
            GetSourceRectOnCanvas(out var destCenter, out var destSize);
            RectTransform.anchoredPosition = Vector2.LerpUnclamped(startPos, destCenter, t);
            RectTransform.sizeDelta = Vector2.LerpUnclamped(startSize, destSize, t);
        }, 1f, returnDuration)
        .SetEase(Ease.InOutCubic)
        .OnComplete(() =>
        {
            _objectInScroll.Image.enabled = true;
            DespawnSelf();
            _objectInScroll.Config.onRelease?.Invoke();
        });
    }

    private SnapPointUI TrySnapLogic(out bool isNearSnapPoint)
    {
        isNearSnapPoint = false;
        SnapPointUI nearest = null;
        float minDistance = float.MaxValue;

        foreach (var point in _objectInScroll.Config.snapPoint)
        {
            float dist = Vector2.Distance(point.transform.position, _mainCamera.ScreenToWorldPoint(RectTransform.position));
            if (dist < _objectInScroll.Config.distanceAcceptSnap)
            {
                isNearSnapPoint = true;
                if (dist < minDistance && point.canSnap && !point.isSnap)
                {
                    nearest = point;
                    minDistance = dist;
                }
            }

        }

        return nearest;
    }


    public void DespawnSelf()
    {
        SnapObjectScrollUIController.Instance.DespawnSnapObject(this);
    }

    private void GetSourceRectOnCanvas(out Vector2 center, out Vector2 size)
    {
        RectTransform srcRect = _objectInScroll.Image.rectTransform;
        srcRect.GetWorldCorners(_corners);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentRT, RectTransformUtility.WorldToScreenPoint(null, _corners[0]), null, out var bl);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentRT, RectTransformUtility.WorldToScreenPoint(null, _corners[2]), null, out var tr);
        size = tr - bl;
        center = (bl + tr) * 0.5f;
    }

    private Vector2 CalculateWorldProjectedSize(Camera cam, Sprite sprite, float worldScale = 1f)
    {
        Vector2 worldSize = sprite.bounds.size * worldScale;

        float pixelPerWorldUnit = UnityEngine.Screen.height / (cam.orthographicSize * 2f);
        Vector2 pixelSizeOnScreen = worldSize * pixelPerWorldUnit;

        CanvasScaler scaler = _mainScaler;
        if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            return pixelSizeOnScreen;

        Vector2 referenceResolution = scaler.referenceResolution;
        float logWidth = Mathf.Log(UnityEngine.Screen.width / referenceResolution.x, 2f);
        float logHeight = Mathf.Log(UnityEngine.Screen.height / referenceResolution.y, 2f);
        float lerp = Mathf.Lerp(logWidth, logHeight, scaler.matchWidthOrHeight);
        float scaleFactorCanvas = Mathf.Pow(2f, lerp);

        float canvasScale = 1f / scaleFactorCanvas;
        Vector2 pixelSizeOnCanvas = pixelSizeOnScreen * canvasScale;

        return pixelSizeOnCanvas;
    }

    public bool AutoSnap()
    {
        // 1️⃣ Find the nearest valid snap point
        SnapPointUI snapPoint = GetValidSnapPoint();
        if (!snapPoint)
        {
            return false;
        }

        Vector3 worldSnapPos = snapPoint.transform.position;
        Vector2 screenSnapPos = RectTransformUtility.WorldToScreenPoint(_mainCamera, worldSnapPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentRT, screenSnapPos, null, out Vector2 localSnapPos);

        Vector2 startMovePos = RectTransform.anchoredPosition;
        FadeOutline(false);
        _objectInScroll.Image.enabled = false;

        DOTween.Sequence()
            .Append(RectTransform.DOAnchorPos(localSnapPos, 0.5f).SetEase(Ease.OutBack))
            .OnComplete(() =>
            {
                SoundManager.PlaySFX(_objectInScroll.Config.snapSound);
                snapPoint.OnSnap(this);
                _objectInScroll.Config.onSnap?.Invoke();

                _objectInScroll.DespawnSelf();
                DespawnSelf();
                SnapObjectScrollUIController.Instance.SnapIncrease++;
            });
        return true;

        SnapPointUI GetValidSnapPoint()
        {
            for (int index = 0; index < _objectInScroll.Config.snapPoint.Count; index++)
            {
                SnapPointUI point = _objectInScroll.Config.snapPoint[index];
                if (point.canSnap && !point.isSnap) return point;
            }

            return null;
        }
    }

}
