using System;
using DG.Tweening;

using UnityEngine;
using UnityEngine.Events;

namespace HoangHH
{
  [Serializable]
  public struct SpriteSnap
  {
    public SpriteRenderer sprite;
    public int onDropOrderLayer;
    public int onDragOrderLayer;
    public int onSnapOrderLayer;
  }

  public enum SnapOrderLayer
  {
    OnDrop = 0,
    OnDrag = 1,
    OnSnap = 2,
  }

  public class SnapObjectMultipleSprite : H3MonoBehaviour
  {
    [Header("References")]
    [SerializeField] protected SnapPoint[] snapToPosition;
    [SerializeField] protected SpriteSnap[] spriteSnaps;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] protected Collider2D col;
    [SerializeField] private bool hasShadow;
    [SerializeField] private SpriteRenderer shadow;
    [SerializeField] private bool turnOffShadowOnDrag;
    [SerializeField] private bool turnOffShadowOnSnap;

    [Header("Properties")]
    [SerializeField] private bool ignoreRigidBody;
    [SerializeField] private bool attachToSnapPoint;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float snapDistance;
    [SerializeField] private float scaleOnDrag = 1;


    [Header("Event")]
    [SerializeField] private UnityEvent onSnap;

    private Camera _mainCam;
    private bool _isDragging;
    private Vector3 _initScale;

    public bool IsSnap { get; private set; }

    private void Awake()
    {
      _mainCam = Camera.main;
      offset.z += Tf.position.z;
      OnAwake();
    }

    private void Start()
    {
      _initScale = Tf.localScale;
      SortingOrder(SnapOrderLayer.OnDrop);
      if (ignoreRigidBody) rb.bodyType = RigidbodyType2D.Static;
    }

    protected virtual void OnAwake()
    {
      _initPosition = Tf.position;
    }

    protected virtual bool CanSnap()
    {
      return true;
    }

    #region Temporary
    private Vector3 _initPosition;

    private Tween _moveBackTween;

    protected void OnStartDrag()
    {
      _moveBackTween?.Kill();
    }

    protected void OnDrop()
    {
      _moveBackTween = Tf.DOMove(_initPosition, 0.5f);
    }

    #endregion

    private void SortingOrder(SnapOrderLayer snapOrderLayer)
    {
      for (int i = 0; i < spriteSnaps.Length; i++)
      {
        var spriteSnap = spriteSnaps[i];

        // Set Sorting Order
        switch (snapOrderLayer)
        {
          case SnapOrderLayer.OnDrop:
            spriteSnap.sprite.sortingOrder = spriteSnap.onDropOrderLayer;
            break;

          case SnapOrderLayer.OnDrag:
            spriteSnap.sprite.sortingOrder = spriteSnap.onDragOrderLayer;
            break;

          case SnapOrderLayer.OnSnap:
            spriteSnap.sprite.sortingOrder = spriteSnap.onSnapOrderLayer;
            break;

          default:
            // Giữ nguyên nếu không match
            break;
        }
      }

      // Handle Shadow chỉ 1 lần
      if (hasShadow)
      {
        bool disableShadow =
            (snapOrderLayer == SnapOrderLayer.OnDrag && turnOffShadowOnDrag) ||
            (snapOrderLayer == SnapOrderLayer.OnSnap && turnOffShadowOnSnap);

        shadow.enabled = !disableShadow;
      }
    }

    private void OnMouseDown()
    {
      if (_isDragging || IsSnap) return;
      _isDragging = true;
      SortingOrder(SnapOrderLayer.OnDrag);
      if (!ignoreRigidBody) rb.bodyType = RigidbodyType2D.Static;
      col.isTrigger = true;
      Vector3 mousePosition = _mainCam.ScreenToWorldPoint(Input.mousePosition);
      mousePosition += offset;
      mousePosition.z = offset.z;
      Tf.position = mousePosition;
      Tf.localRotation = Quaternion.identity;
      Tf.localScale = _initScale * scaleOnDrag;
      // AudioManager.PlaySFX(onDragAudio.clip, onDragAudio.volume);
      OnStartDrag();
    }

    private void OnMouseDrag()
    {
      if (!_isDragging || IsSnap) return;
      Vector3 mousePosition = _mainCam.ScreenToWorldPoint(Input.mousePosition);
      mousePosition += offset;
      mousePosition.z = offset.z;
      Tf.position = mousePosition;
    }

    private void OnMouseUp()
    {
      if (!_isDragging || IsSnap) return;
      _isDragging = false;
      SortingOrder(SnapOrderLayer.OnDrop);
      if (!ignoreRigidBody) rb.bodyType = RigidbodyType2D.Dynamic;
      col.isTrigger = false;
      Tf.localScale = _initScale;
      for (int i = 0; i < snapToPosition.Length; i++)
      {
        SnapPoint snapPoint = snapToPosition[i];
        if (snapPoint.Tf.gameObject.activeSelf && snapPoint.isSnap) continue;
        if (!(DistanceToInSqrVec2(snapPoint.Tf) < snapDistance)) continue;
        if (!CanSnap()) continue;
        if (!ignoreRigidBody) rb.bodyType = RigidbodyType2D.Static;
        if (attachToSnapPoint) Tf.SetParent(snapPoint.Tf);
        IsSnap = true;
        snapPoint.isSnap = true;
        snapPoint.OnSnap();
        SortingOrder(SnapOrderLayer.OnSnap);
        col.enabled = false;
        // AudioManager.PlaySFX(onSnapAudio.clip, onSnapAudio.volume);
        Tf.DOMove(snapPoint.Tf.position, 0.2f).OnComplete(() => onSnap.Invoke());
        return;
      }
      OnDrop();
    }


    private void GetReferences()
    {
      rb = GetComponentInChildren<Rigidbody2D>();
      col = GetComponentInChildren<Collider2D>();
    }
  }
}
