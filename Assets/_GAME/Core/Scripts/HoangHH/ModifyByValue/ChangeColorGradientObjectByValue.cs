using UnityEngine;

namespace HoangHH
{
    public class ChangeColorGradientObjectByValue : ModifyByValue
    {
        [Header("Color Gradient Change")]
        [SerializeField] private SpriteRenderer spriteRender;
        [SerializeField] private Gradient gradient;
        
        protected override void OnModify(float value)
        {
            spriteRender.color = gradient.Evaluate(value);
        }
    }
}