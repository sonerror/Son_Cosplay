using DG.Tweening;
using HoangHH;
using UnityEngine;

namespace Costopia.Gameplay
{
    public class PhaseBackground : H3MonoBehaviour
    {
        // TODO: In future, all setup of gameplay (include makeover and dress phase handle on this class)
        [SerializeField] private SpriteRendererGroup bgMakeUpGroup;
        [SerializeField] private float distanceXPerPhase;
        [SerializeField] private float timeMovePerPhase = 0.5f;

        private Tween _moveTween;
        private int _currentPhase;

        public System.Action<int> onEnterPhase;
        public System.Action<int> onExitPhase;
        
        public void SetPhase(int index)
        {
            if (index == _currentPhase) return;
            // if (index >= bgGroup.NumberSpriteRenderers) return;
            _moveTween?.Kill();
            bgMakeUpGroup.Tf.DOLocalMoveX(-distanceXPerPhase * index, timeMovePerPhase);
            onEnterPhase?.Invoke(index);
            onExitPhase?.Invoke(_currentPhase);
            _currentPhase = index;
        }
    }
}