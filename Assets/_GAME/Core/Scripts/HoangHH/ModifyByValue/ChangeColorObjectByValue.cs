using UnityEngine;

namespace HoangHH
{
    public class ChangeColorObjectByValue : ModifyByValue
    {
        [Header("Color Change")]
        [SerializeField] private SpriteRenderer spriteRender;
        [SerializeField] private Color lowerColor;
        [SerializeField] private Color upperColor;
        
        private Color _currentColor;
        
        protected override void OnModify(float value)
        {
            spriteRender.color = Color.Lerp(lowerColor, upperColor, value / 1f);
        }
    }
}