using UnityEngine;

namespace HoangHH
{
    public class ChangeSpriteObjectByValue : ModifyByValue
    {
        [Header("Sprite Change")] 
        [SerializeField] private SpriteRenderer spriteRender;
        [SerializeField] private Sprite lowerSprite;
        [SerializeField] private Vector3 lowerSpritePosition;
        [SerializeField] private Sprite upperSprite;
        [SerializeField] private Vector3 upperSpritePosition;
        [SerializeField] private float changeValue;
        
        private Sprite _currentSprite;
        public bool IsUpperSprite => _currentSprite == upperSprite;
        public System.Action onSpriteChange;
        
        public void SetLowerSprite(Sprite sprite, Vector3 position)
        {
            lowerSprite = sprite;
            lowerSpritePosition = position;
            if (!(modifyValue < changeValue)) return;
            spriteRender.sprite = lowerSprite;
            spriteRender.transform.localPosition = lowerSpritePosition;
            _currentSprite = lowerSprite;
        }
        
        public void SetUpperSprite(Sprite sprite, Vector3 position)
        {
            upperSprite = sprite;
            upperSpritePosition = position;
            if (!(modifyValue >= changeValue)) return;
            spriteRender.sprite = upperSprite;
            spriteRender.transform.localPosition = upperSpritePosition;
            _currentSprite = upperSprite;
        }
        
        protected override void OnModify(float value)
        {
            if (value < changeValue)
            {
                if (_currentSprite == lowerSprite) return;
                spriteRender.sprite = lowerSprite;
                spriteRender.transform.localPosition = lowerSpritePosition;
                _currentSprite = lowerSprite;
                onSpriteChange?.Invoke();
            }
            else
            {
                if (_currentSprite == upperSprite) return;
                spriteRender.sprite = upperSprite;
                spriteRender.transform.localPosition = upperSpritePosition;
                _currentSprite = upperSprite;
                onSpriteChange?.Invoke();
            }
        }
    }
}