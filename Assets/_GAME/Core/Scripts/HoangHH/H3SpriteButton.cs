using DG.Tweening;
using UnityEngine.EventSystems;


namespace HoangHH
{
    /// <summary>
    /// A base class for sprite renderer button, need Camera has Ray-cast2D and a collider in button component to work
    /// </summary>
    public abstract class H3SpriteButton : H3MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private bool isLockBtn;
        private bool isPointerDown;

        private Tween tween;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (isLockBtn || !isPointerDown) return;
            isPointerDown = true;
            ScaleButton(0.9f, 0.3f);
            OnPointerDown();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            ScaleButton(1, 0.3f);
            OnPointerUp();
        }

        private void ScaleButton(float scale, float time)
        {
            tween.Kill(true);
            tween = Tf.DOScale(scale, time).SetEase(Ease.OutBounce);
        }
        
        public void LockButton(bool isLock)
        {
            isLockBtn = isLock;
            if (!isLock || !isPointerDown) return;
            isPointerDown = false;
            ScaleButton(1, 0.3f);
        }
        
        protected virtual void OnPointerDown() { }

        protected virtual void OnPointerUp() { }
    }
}
