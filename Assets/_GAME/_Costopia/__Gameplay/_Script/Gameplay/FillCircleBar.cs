using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace HoangHH
{
  public class FillCircleBar : H3MonoBehaviour
  {
    [SerializeField] private Image fillImage;
    [SerializeField] private float showTime = 0.3f;

    private System.Action _onFillComplete;
    private Tween _showTween;

    private float _currentFill;
    private bool _isFilled;
    private bool _isShow;

    public bool IsShow => _isShow;

    public void SetUp(System.Action onFillComplete = null, bool showInstantly = true)
    {
      _isFilled = false;
      _currentFill = 0;
      fillImage.fillAmount = 0;
      _onFillComplete = onFillComplete;
      _isShow = false;
      if (showInstantly) Show();
    }

    public void ReFill()
    {
      _isFilled = false;
      _currentFill = 0;
      fillImage.fillAmount = 0;
    }

    public void Fill(float fillTo)
    {
      if (_isFilled) return;
      _currentFill = Mathf.Clamp01(fillTo);
      fillImage.fillAmount = _currentFill;
      if (!Mathf.Approximately(_currentFill, 1f)) return;
      _isFilled = true;
      _onFillComplete?.Invoke();
      _onFillComplete = null;
      Hide();
    }



    public void Show()
    {
      if (_isShow) return;
      _isShow = true;
      gameObject.SetActive(true);
      _showTween?.Kill();
      _showTween = Tf.DOScale(Vector3.one, showTime);
    }


    public void Hide()
    {
      if (!_isShow) return;
      _isShow = false;
      _showTween?.Kill();
      _showTween = Tf.DOScale(Vector3.zero, showTime)
          .OnComplete(() => gameObject.SetActive(false));
    }
  }
}