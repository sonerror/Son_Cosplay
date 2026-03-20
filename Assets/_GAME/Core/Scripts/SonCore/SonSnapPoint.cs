using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
namespace sonnv
{
    public class SonSnapPoint : SonMonoBehaviour
    {
        public bool canSnap = true;
        public bool isSnap;
        [SerializeField] private UnityEvent onSnap;
        public void Show()
        {
            gameObject.SetActive(true);
            Vector3 scale = Tf.localScale;
            Tf.localScale = Vector3.zero;
            Tf.DOScale(scale, 0.3f);
        }

        public virtual void OnSnap()
        {
            onSnap?.Invoke();
        }

        public virtual void ForceChangeSnap(bool snap)
        {
            isSnap = snap;
        }

        public void ChangeCanSnap(bool snap)
        {
            canSnap = snap;
        }
    }

}
