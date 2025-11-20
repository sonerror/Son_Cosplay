using System.Collections.Generic;
using DG.Tweening;

using UnityEngine;
using UnityEngine.Events;

namespace Costopia.Level
{
  /// DraggableMakeupItem is a class that allows
  public class DraggableMakeupItem : DraggableItemBase
  {

    [SerializeField]
    private Collider itemCollider2D;


    [SerializeField]
    private float rewindDuration = 1f;


    [SerializeField]
    private float deltaZMulti = 20f;


    [SerializeField]
    private float deltaZRotate = 20f, dragSlerpSpeed = 30f;


    [SerializeField]
    private float scale = 1.05f;


    [SerializeField]
    private AudioClip sfxPick;


    [SerializeField]
    private Vector2 offsetPick;


    [SerializeField]
    private bool
        isActive = true,
        isFrezee = false,
        isFlexSortingOrder = false,
        isRewind = true,
        isFlexRewindDurationByDistance = false,
        isScaleUp = true,
        isRotate = true,
        isRotateFollowFace = true,
        needChangeYWhileInZone = false,
        haveOffset = false,
        useToCheckEndPhase = false;




    [SerializeField]
    private int moveOrder, unmoveOrder;



    [SerializeField]
    private List<SpriteRenderer> render;


    [SerializeField]
    private UnityEvent startDragEvents, draggingEvents, endDragEvents, completeRewindEvents;


    private Vector2 startOffset;
    private Vector3 startPos;

    private bool canRemoveStep;
    private bool isDragging = false;
    private float startZ;
    private Vector3 iniScale;
    public bool IsDragging => isDragging;
    public UnityEvent OnStartDragEvent => startDragEvents;
    public UnityEvent OnStopDragEvent => endDragEvents;
    public UnityEvent CompleteRewindEvents => completeRewindEvents;

    public Collider itemCol2d => itemCollider2D;
    public Vector3 InitialPos => startPos;

    public bool CanRemoveUseInStep
    {
      set => canRemoveStep = value;
    }
    public bool IsActive
    {
      get => isActive;
      set => isActive = value;
    }

    public bool IsRewind
    {
      get => isRewind;
      set => isRewind = value;
    }

    private void RemoveStepUse(int step)
    {
      if (useInSteps.Contains(step))
      {
        useInSteps.Remove(step);
      }

    }
    public void AddUseInStep(int step)
    {
      if (useInSteps.Contains(step)) return;
      useInSteps.Add(step);
    }

    protected override void Awake()
    {
      base.Awake();
      if (_levelMakeupBase)
      {
        _levelMakeupBase.OnBlockPlayerInteractChanged += OnMouseUp;
      }
    }

    private void OnDestroy()
    {
      if (_levelMakeupBase)
      {
        _levelMakeupBase.OnBlockPlayerInteractChanged -= OnMouseUp;
      }
    }

    protected override void Start()
    {
      base.Start();
      startPos = TF.localPosition;
      startZ = TF.eulerAngles.z;
      iniScale = TF.localScale;
      SetUnmoveOrder();
    }

    public override void OnMouseDown()
    {
      base.OnMouseDown();

      if (!isActive || !_levelMakeupBase.IsAllowInteract || isFrezee) return;

      // AudioManager.PlaySFX(sfxPick);

      SetMoveOrder();

      TF.DOKill();

      if (isScaleUp)
      {
        TF.DOScale(Vector3.one * scale, 0.2f);
      }

      if (isRotate)
      {
        TF.DORotate(Vector3.forward * deltaZRotate, 0.2f);
      }

      _levelMakeupBase.Character.SetBodyCanMove(false);
      startOffset = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)TF.position;
      isDragging = true;
      startDragEvents?.Invoke();
    }

    public override void OnMouseDrag()
    {
      base.OnMouseDrag();

      if (!_levelMakeupBase.IsAllowInteract || isFrezee) return;

      if (!isDragging) return;
      if (!isActive)
      {
        CancelDragging();
      }

      if (Camera.main != null)
      {
        var pos2d = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - startOffset +
                    (haveOffset ? offsetPick : Vector2.zero);
        //TF.position = new Vector3(pos2d.x, pos2d.y, TF.position.z);
        TF.position = Vector3.Slerp(TF.position, pos2d, dragSlerpSpeed * Time.deltaTime);
      }

      RotateToTarget();
      CheckZoneWhileMouseDragging();
      draggingEvents?.Invoke();
    }

    private bool _flipped;

    private void RotateToTarget()
    {
      if (!isRotateFollowFace) return;
      var direction = _levelMakeupBase.Character.transform.position - TF.position;
      // Sử dụng một hàm riêng để xử lý xoay
      float angleRad = Mathf.Atan2(direction.y, direction.x);
      float angleDeg = angleRad * Mathf.Rad2Deg * deltaZMulti;
      direction = TF.localRotation.eulerAngles;
      direction.z = angleDeg;
      TF.localRotation = Quaternion.Euler(direction);

      if (!needChangeYWhileInZone) return;
      if (TF.localPosition.x < 0)
      {
        if (_flipped) return;
        _flipped = true;
        var flipScale = TF.localScale;
        flipScale.y = -flipScale.y;
        TF.DOScale(flipScale, 0.2f);
      }
      else
      {
        _flipped = false;
        TF.DOScale(Vector3.one * 1.05f, 0.2f);
      }
    }

    private int CurrentStep => _levelMakeupBase.CurrentStep;

    private bool ContainDoneStep()
    {
      return useInSteps.Contains(CurrentStep) && _levelMakeupBase.IsDoneStep(CurrentStep);
    }

    public override void OnMouseUp()
    {
      base.OnMouseUp();

      if (!isActive || isFrezee) return;

      if (!isDragging) return;
      float rate = 1;
      _timer = 0;

      if (isFlexRewindDurationByDistance)
      {
        rate = (Vector2.Distance(TF.position, startPos) / 2f);
      }

      TF.DOScale(iniScale, 0.2f);

      if (isRewind)
      {
        TF.DOLocalMove(startPos, rewindDuration * rate).OnComplete(() =>
        {
          SetUnmoveOrder();
          completeRewindEvents?.Invoke();
        });
      }
      else
      {
        SetUnmoveOrder();
        completeRewindEvents?.Invoke();
      }

      isDragging = false;
      if (isRotate || isRotateFollowFace)
      {
        TF.DORotate(new Vector3(TF.eulerAngles.x, 0, startZ), .2f);
      }

      _levelMakeupBase.Character.SetBodyCanMove(true);
      endDragEvents?.Invoke();

      if (ContainDoneStep())
      {
        //Debug.LogError("done step r này");
        useInSteps.Remove(CurrentStep);
        if (useToCheckEndPhase) _levelMakeupBase.CheckStepRemain(0);
        _levelMakeupBase.TryNextStep();
      }
      else
      {
        if (!canRemoveStep) return;
        canRemoveStep = false;
        RemoveStepUse(CurrentStep);
      }
    }

    public void CancelDragging()
    {
      isDragging = true;
      OnMouseUp();
    }

    public void SetUnmoveOrder()
    {
      if (isFlexSortingOrder)
      {
        SetOrderLayer(unmoveOrder);
      }
    }

    public void SetMoveOrder()
    {
      if (isFlexSortingOrder)
      {
        SetOrderLayer(moveOrder);
      }
    }

    public void LockPosition()
    {
      isFrezee = true;
      isActive = false;
      TF.DOKill();
    }

    public void UnlockPosition()
    {
      isFrezee = false;
      isActive = true;
      CancelDragging();
    }

    public void BlockItemCollider()
    {
      itemCollider2D.enabled = false;
    }

    public void ActiveItemCollider()
    {
      itemCollider2D.enabled = true;
    }

    public void ChangeColliderEnabledGapTime(float time)
    {
      if (IsInvoking(nameof(ActiveItemCollider))) return;
      Invoke(nameof(ActiveItemCollider), time);
    }

    public void SetNewStart(Vector2 pos, float rotation)
    {
      startPos = pos;
      startZ = rotation;
    }

    public void SetOrderLayer(int order)
    {
      for (int i = 0; i < render.Count; i++)
      {
        render[i].sortingOrder = order;
      }
    }

    public void UpdateOffset()
    {
      startOffset = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)TF.position;
    }

  }
}