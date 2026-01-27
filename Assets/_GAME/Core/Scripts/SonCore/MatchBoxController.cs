using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace sonnv
{
    public class MatchBoxController : SonMonoBehaviour
    {
        [Header("Button Components")]
        [SerializeField] private Collider2D col;
        [SerializeField] private bool blockInteractManually;
        [SerializeField] private bool isBlockTap = false;

        [Header("Audio")]
        [SerializeField] private AudioClip turnOnSound;
        [SerializeField] private AudioClip turnOffSound;

        [Header("Transform Movement")]
        [SerializeField] private Transform targetTransform;
        [SerializeField] private float onPositionX = 0f;
        [SerializeField] private float offPositionX = 0f;
        [SerializeField] private float onPositionY = 0;
        [SerializeField] private float offPositionY = 0f;


        [SerializeField] private float moveDuration = 0.3f;
        [SerializeField] private Ease moveEase = Ease.OutQuad;

        [Header("Events")]
        public UnityEvent onClickOn;
        public UnityEvent onClickOff;
        public System.Action onBlock;

        public UnityEvent onCompleteMoveOn;
        public UnityEvent onCompleteMoveOff;

        public bool IsOn { get; private set; }

        private bool _canInteract = true;
        private Vector3 _originalTargetLocalPosition;
        private bool _isMoving = false;

        private void Awake()
        {
            if (targetTransform != null)
            {
                _originalTargetLocalPosition = targetTransform.localPosition;
            }
        }

        private void Start()
        {
            if (col != null && isBlockTap == false)
            {
                col.enabled = _canInteract;
            }

            if (targetTransform != null)
            {
                targetTransform.localPosition = IsOn ?
                    new Vector3(onPositionX, onPositionY, _originalTargetLocalPosition.z) :
                    new Vector3(offPositionX, offPositionY, _originalTargetLocalPosition.z);
            }
        }



        public void SetBlockManually(bool block)
        {
            blockInteractManually = block;
            if (col != null)
            {
                col.enabled = !(blockInteractManually || !_canInteract || isBlockTap || _isMoving);
            }
        }

        private void OnBlockInteract()
        {
            if (col != null && isBlockTap == false)
            {
                col.enabled = _canInteract && !blockInteractManually && !_isMoving;
            }
        }
        private void OnMouseUpAsButton()
        {
            if (!CanClick())
            {
                onBlock?.Invoke();
                return;
            }

            ClickMatchBox();
        }

        private bool CanClick()
        {
            if (isBlockTap) return false;
            if (!_canInteract) return false;
            if (blockInteractManually) return false;
            if (_isMoving) return false;
            if (!IsMatchCondition()) return false;
            return true;
        }

        protected virtual bool IsMatchCondition()
        {
            return true;
        }

        public void ClickMatchBox()
        {
            if (_isMoving) return;

            IsOn = !IsOn;

            _isMoving = true;
            if (col != null) col.enabled = false;
            SoundManager.PlaySFX(IsOn ? turnOnSound : turnOffSound);
            if (targetTransform != null)
            {
                Vector3 targetLocalPosition = IsOn ?
                    new Vector3(onPositionX, onPositionY, _originalTargetLocalPosition.z) :
                    new Vector3(offPositionX, offPositionY, _originalTargetLocalPosition.z);

                targetTransform.DOLocalMove(targetLocalPosition, moveDuration).SetEase(moveEase).OnComplete(() =>
                {
                    _isMoving = false;
                    if (col != null && isBlockTap == false)
                    {
                        col.enabled = _canInteract && !blockInteractManually;
                    }

                    if (IsOn)
                    {
                        onCompleteMoveOn?.Invoke();
                    }
                    else
                    {
                        onCompleteMoveOff?.Invoke();
                    }
                });
            }
            else
            {
                _isMoving = false;
                if (col != null && isBlockTap == false)
                {
                    col.enabled = _canInteract && !blockInteractManually;
                }
                if (IsOn)
                {
                    onCompleteMoveOn?.Invoke();
                }
                else
                {
                    onCompleteMoveOff?.Invoke();
                }
            }

            if (IsOn)
            {
                onClickOn.Invoke();
            }
            else
            {
                onClickOff.Invoke();
            }
        }
        public void OnCloseMatchBox()
        {
            if (IsOn)
            {
                ClickMatchBox();
            }
        }
        public void SetCanBlockTap(bool value)
        {
            isBlockTap = value;
        }
    }
}