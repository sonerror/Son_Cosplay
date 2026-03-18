using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using sonnv;

public class FillCircleBar : SonMonoBehaviour
{
  [SerializeField] private Image fillImage;
  [SerializeField] private float showTime = 0.3f;
  [SerializeField] private float scaleShow = 0.5f;

  private Tween _showTween;

  private float _currentFill;
  [SerializeField] private bool _isFilled;
  private bool _isShow;

  public bool IsShow => _isShow;

  public void SetUp(bool showInstantly = true)
  {
    _isFilled = false;
    _currentFill = 0;
    fillImage.fillAmount = 0f;
    _isShow = false;
    if (showInstantly) Show();
  }

  public void ReFill()
  {
    _isFilled = false;
    _currentFill = 0;
    fillImage.fillAmount = 0f;

  }

  public void Fill(float fillTo)
  {
    // Debug.LogWarning($"Fill to: {fillTo}");
    if (_isFilled) return;
    _currentFill = Mathf.Clamp01(fillTo);
    fillImage.fillAmount = _currentFill;
    if (!Mathf.Approximately(_currentFill, 1f)) return;
    // _isFilled = true;
    Hide();
  }


  [Sirenix.OdinInspector.Button]
  public void Show()
  {
    if (_isShow) return;
    _isShow = true;
    gameObject.SetActive(true);
    _showTween?.Kill();
    _showTween = Tf.DOScale(Vector3.one * scaleShow, showTime);
  }

  [Sirenix.OdinInspector.Button]
  public void Hide()
  {
    if (!_isShow) return;
    _isShow = false;
    _showTween?.Kill();
    _showTween = Tf.DOScale(Vector3.zero, showTime)
    .SetDelay(0.5f)
    .OnComplete(() => gameObject.SetActive(false));
  }
}