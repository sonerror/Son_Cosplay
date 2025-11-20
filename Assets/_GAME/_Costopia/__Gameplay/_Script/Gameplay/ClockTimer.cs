using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace HoangHH
{
  public class ClockTimer : H3MonoBehaviour
  {
    [SerializeField] private Image timerImage;
    [SerializeField] private float showTime = 0.3f;
    [SerializeField] private float timeOut = 3f;
    [SerializeField] private AudioSource sound;

    public System.Action OnTimeOut;
    private Tween _showTween;

    private bool _isTimeOut;
    private bool _isStartTimer;
    private LevelBase _level;

    private void Start()
    {
      _level = LevelBase.Instance;
      if (_level) _level.OnBlockPlayerInteractChanged += SetSoundOnChangeInteract;
    }

    private void SetSoundOnChangeInteract()
    {
      if (!_isStartTimer) return;
      if (!_level.IsAllowInteract) sound.Pause();
      else sound.Play();
    }

    private void Update()
    {
      if (!_isStartTimer) return;
      timerImage.fillAmount -= Time.deltaTime / timeOut;
      if (!(timerImage.fillAmount <= 0)) return;
      OnTimeOut?.Invoke();
      Hide();
    }

    public void Show(float time)
    {
      timeOut = time;
      Show();
    }


    private void Show()
    {
      if (_isStartTimer) return;
      gameObject.SetActive(true);
      Tf.localScale = Vector3.zero;
      timerImage.fillAmount = 1;
      _showTween?.Kill();
      _showTween = Tf.DOScale(Vector3.one, showTime);
      _isStartTimer = true;
      sound?.Play();
    }

    private void Hide()
    {
      _showTween?.Kill();
      _showTween = Tf.DOScale(Vector3.zero, showTime)
          .OnComplete(() => gameObject.SetActive(false));
      _isStartTimer = false;
      sound?.Stop();
    }
  }
}