using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace sonnv
{
    public class Pumper : SonMonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Anim")]
        [SerializeField] private string[] pumperAnimName;
        [SerializeField] private SkeletonAnimation pumperAnim;
        [Header("State")]
        [SerializeField] private bool isActive = true;

        [Header("Move")]
        [SerializeField] private float dragFollowSpeed = 35f;
        [SerializeField] private Vector2 movingYCurve;
        [SerializeField] private bool invertY = false;

        [Header("Reach Target Smooth")]
        [SerializeField] private float targetDistance = 0.03f;
        [SerializeField] private float reachTargetAnimTime = 0.08f;
        [SerializeField] private Ease reachTargetEase = Ease.OutQuad;
        [SerializeField] private float holdAtTargetTime = 0.04f;

        [Header("Return")]
        [SerializeField] private bool returnToStartOnRelease = true;
        [SerializeField] private bool returnToStartWhenReachTarget = true;
        [SerializeField] private float returnTime = 0.25f;
        [SerializeField] private Ease returnEase = Ease.OutQuad;

        [Header("Repeat")]
        [SerializeField] private bool repeatAfterReturn = true;

        [Header("Pump Count")]
        [SerializeField] private bool usePumpLimit = true;
        [SerializeField] private int targetCount = 0;
        [SerializeField] private int maxPumpCount = 3;

        [Header("References")]
        [SerializeField] private Collider2D pumpCol;
        public Collider2D Col => pumpCol;
        [SerializeField] private AudioSource pumpAs;

        [Header("Target")]
        [SerializeField] private bool haveTargetY = true;

        // Kéo xuống thì true, kéo lên thì false
        [SerializeField] private bool targetIsMinY = true;

        [Header("Events")]
        public UnityEvent OnReachTargetY;
        public UnityEvent OnReturnToStartComplete;
        public UnityEvent OnDone;

        public Collider2D PumpCol => pumpCol;
        public int TargetCount => targetCount;
        public int MaxPumpCount => maxPumpCount;

        private bool isDragging;
        private bool hasReachedTarget;
        private bool isReturning;
        private bool isFinishingTarget;
        private bool isDone;

        private Camera mainCamera;

        private Vector3 startLocalPos;
        private Vector3 startPointerLocalPos;
        private Vector3 originalLocalPos;

        private Tween currentTween;

        private float MinY => Mathf.Min(movingYCurve.x, movingYCurve.y);
        private float MaxY => Mathf.Max(movingYCurve.x, movingYCurve.y);
        private float TargetY => targetIsMinY ? MinY : MaxY;

        public void SetActive(bool active)
        {
            isActive = active;

            if (pumpCol != null)
                pumpCol.enabled = active && !isDone && !isReturning && !isFinishingTarget;
        }

        public void ResetPump()
        {
            currentTween?.Kill();

            targetCount = 0;
            isDone = false;
            isDragging = false;
            hasReachedTarget = false;
            isReturning = false;
            isFinishingTarget = false;
            isActive = true;

            transform.localPosition = originalLocalPos;

            if (pumpCol != null)
                pumpCol.enabled = true;
            Debug.Log("[Pumper] Reset Pump");
        }

        private void Awake()
        {
            mainCamera = Camera.main;
            originalLocalPos = transform.localPosition;
        }

        private void OnDestroy()
        {
            currentTween?.Kill();
        }

        private void Update()
        {
            if (!isDragging || !isActive) return;
            if (isReturning || isFinishingTarget || isDone) return;

            if (!Input.GetMouseButton(0) && Input.touchCount <= 0)
            {
                StopDragging(true);
                return;
            }

            Vector2 screenPos;

            if (Input.touchCount > 0)
                screenPos = Input.GetTouch(0).position;
            else
                screenPos = Input.mousePosition;

            DragByScreenPosition(screenPos);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isActive) return;
            if (isReturning) return;
            if (isFinishingTarget) return;
            if (isDone) return;

            if (IsPumpLimitReached())
            {
                FinishDone();
                return;
            }

            if (mainCamera == null)
                mainCamera = Camera.main;

            if (mainCamera == null)
            {
                Debug.LogError("[Pumper] No Main Camera");
                return;
            }
            if (pumpAs != null)
                pumpAs.Play();

            currentTween?.Kill();

            Vector3 worldPos = ScreenToWorld(eventData.position);
            Vector3 localPos = WorldToLocal(worldPos);

            startLocalPos = transform.localPosition;
            startPointerLocalPos = localPos;

            isDragging = true;
            hasReachedTarget = false;
            isReturning = false;
            isFinishingTarget = false;

           
            Debug.Log("[Pumper] Down");
            Debug.Log("[Pumper] Count: " + targetCount + "/" + pumperAnimName.Length);
            Debug.Log("[Pumper] StartY: " + startLocalPos.y + " | TargetY: " + TargetY + " | MinY: " + MinY + " | MaxY: " + MaxY);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StopDragging(true);
            Debug.Log("[Pumper] Up");
        }

        private void DragByScreenPosition(Vector2 screenPos)
        {
            Vector3 worldPos = ScreenToWorld(screenPos);
            Vector3 currentPointerLocalPos = WorldToLocal(worldPos);

            Vector3 delta = currentPointerLocalPos - startPointerLocalPos;

            if (invertY)
                delta.y *= -1f;

            Vector3 desiredPos = startLocalPos + delta;

            desiredPos.x = startLocalPos.x;
            desiredPos.z = startLocalPos.z;
            desiredPos.y = Mathf.Clamp(desiredPos.y, MinY, MaxY);

            if (haveTargetY && !hasReachedTarget)
            {
                if (IsReachTarget(desiredPos.y))
                {
                    BeginReachTarget();
                    return;
                }
            }

            float t = 1f - Mathf.Exp(-dragFollowSpeed * Time.deltaTime);

            Vector3 newPos = transform.localPosition;
            newPos.x = startLocalPos.x;
            newPos.z = startLocalPos.z;
            newPos.y = Mathf.Lerp(transform.localPosition.y, desiredPos.y, t);

            transform.localPosition = newPos;
        }

        private bool IsReachTarget(float currentY)
        {
            if (targetIsMinY)
            {
                return currentY <= TargetY + targetDistance;
            }
            else
            {
                return currentY >= TargetY - targetDistance;
            }
        }

        private void BeginReachTarget()
        {
            if (hasReachedTarget) return;
            if (isFinishingTarget) return;
            if (isDone) return;

            hasReachedTarget = true;
            isDragging = false;
            isFinishingTarget = true;

            if (pumpCol != null)
                pumpCol.enabled = false;

            currentTween?.Kill();

            currentTween = transform
                .DOLocalMoveY(TargetY, reachTargetAnimTime)
                .SetEase(reachTargetEase)
                .OnComplete(() =>
                {
                    pumperAnim.AnimationState.SetAnimation(0, pumperAnimName[targetCount], false);
                    targetCount++;
                    Debug.Log("[Pumper] Reach Target: " + targetCount + "/" + pumperAnimName.Length);

                    OnReachTargetY?.Invoke();

                    bool doneAfterThisPump = IsPumpLimitReached();

                    if (returnToStartWhenReachTarget)
                    {
                        if (doneAfterThisPump)
                        {
                            ReturnToStart(FinishDone);
                        }
                        else
                        {
                            ReturnToStart();
                        }
                    }
                    else
                    {
                        isFinishingTarget = false;

                        if (doneAfterThisPump)
                        {
                            FinishDone();
                        }
                        else
                        {
                            ResetForNextDrag();
                        }
                    }
                });
        }

        private bool IsPumpLimitReached()
        {
            if (!usePumpLimit) return false;
            if (pumperAnimName.Length <= 0) return false;

            return targetCount >= pumperAnimName.Length;
        }

        private void FinishDone()
        {
            if (isDone) return;

            isDone = true;
            isActive = false;
            isDragging = false;
            hasReachedTarget = true;
            isReturning = false;
            isFinishingTarget = false;
            if (pumpCol != null)
                pumpCol.enabled = false;

            Debug.Log("[Pumper] Done");

            OnDone?.Invoke();
        }

        private Vector3 ScreenToWorld(Vector2 screenPos)
        {
            Vector3 pos = screenPos;

            pos.z = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);

            Vector3 worldPos = mainCamera.ScreenToWorldPoint(pos);
            worldPos.z = transform.position.z;

            return worldPos;
        }

        private Vector3 WorldToLocal(Vector3 worldPos)
        {
            if (transform.parent != null)
                return transform.parent.InverseTransformPoint(worldPos);

            return worldPos;
        }

        private void StopDragging(bool canReturnToStart)
        {
            if (!isDragging) return;

            isDragging = false;
            if (canReturnToStart && returnToStartOnRelease && !hasReachedTarget)
            {
                ReturnToStart();
            }
        }

        private void ReturnToStart(UnityAction onComplete = null)
        {
            currentTween?.Kill();

            isDragging = false;
            isReturning = true;
            isFinishingTarget = false;

            if (pumpCol != null)
                pumpCol.enabled = false;

            currentTween = transform
                .DOLocalMove(originalLocalPos, returnTime)
                .SetEase(returnEase)
                .SetDelay(holdAtTargetTime)
                .OnComplete(() =>
                {
                    isReturning = false;

                    OnReturnToStartComplete?.Invoke();

                    Debug.Log("[Pumper] Return Complete");

                    if (onComplete != null)
                    {
                        onComplete.Invoke();
                        return;
                    }

                    if (repeatAfterReturn)
                    {
                        ResetForNextDrag();
                    }
                    else
                    {
                        SetActive(false);
                    }
                });
        }

        private void ResetForNextDrag()
        {
            if (IsPumpLimitReached())
            {
                FinishDone();
                return;
            }

            isDragging = false;
            hasReachedTarget = false;
            isReturning = false;
            isFinishingTarget = false;
            isActive = true;

            if (pumpCol != null)
                pumpCol.enabled = true;

            Debug.Log("[Pumper] Reset For Next Drag");
        }
    }
}