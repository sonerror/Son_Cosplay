using System;
using DG.Tweening;
using UnityEngine;

namespace VinhLB
{
    public class SpriteButton : MonoBehaviour
    {
        public event System.Action OnClick;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private Color _pressedColor;
        [SerializeField]
        private Color _disabledColor;
        [SerializeField]
        private bool _clickable = true;
        [SerializeField]
        private float _cooldown;

        private Color _originColor;
        private bool _inCooldown;
        private float _cooldownTimer;
        
        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public bool Clickable => _clickable;
        public float Cooldown => _cooldown;
        
        private void Awake()
        {
            if (_spriteRenderer == null)
            {
                return;
            }
            
            _originColor = _spriteRenderer.color;
        }

        private void Update()
        {
            if (!_clickable)
            {
                return;
            }

            if (_inCooldown)
            {
                _cooldownTimer += Time.deltaTime;
                if (_cooldownTimer >= _cooldown)
                {
                    _cooldownTimer = 0;
                    _inCooldown = false;
                }   
            }
        }

        private void OnMouseDown()
        {
            if (!_clickable)
            {
                return;
            }

            StartClickAnim();
        }

        private void OnMouseExit()
        {
            if (!_clickable)
            {
                return;
            }
            
            EndClickAnim();
        }

        private void OnMouseUpAsButton()
        {
            if (!_clickable)
            {
                return;
            }
            
            if (_inCooldown)
            {
                return;
            }
            
            OnClick?.Invoke();

            _inCooldown = true;
            
            if (!_clickable)
            {
                return;
            }
            
            EndClickAnim();
        }

        public void SetClickable(bool value)
        {
            _clickable = value;

            if (_spriteRenderer == null)
            {
                return;
            }
            
            if (_clickable)
            {
                _spriteRenderer.DOColor(_originColor, 0.1f);
            }
            else
            {
                _spriteRenderer.DOColor(_disabledColor, 0.1f);
            }
        }

        private void StartClickAnim()
        {
            if (_spriteRenderer == null)
            {
                return;
            }
            
            _spriteRenderer.DOColor(_pressedColor, 0.1f);
        }

        private void EndClickAnim()
        {
            if (_spriteRenderer == null)
            {
                return;
            }
            
            _spriteRenderer.DOColor(_originColor, 0.1f);
        }
    }   
}
