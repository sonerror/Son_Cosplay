using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HoangHH
{
  public enum Direction
  {
    Up,
    Down,
    Left,
    Right
  }
  public class ShowObjectEffect : H3MonoBehaviour
  {
    [SerializeField] private Direction direction = Direction.Up;
    [SerializeField] private Direction hideDirection = Direction.Up;
    [SerializeField] private float yPositionShow = 2f;
    [SerializeField] private bool fadeSprite = true;
    [SerializeField] private SpriteRenderer[] sprites;
    [SerializeField] private float timeFade = 0.4f;
    [SerializeField] private float delayFadeOut = 0.3f;

    [SerializeField]
    private bool targetAlphaToItsBaseValue;


    [SerializeField] private float timeShow = 0.7f;
    [SerializeField] private bool showOnStart;
    [SerializeField] private Ease easeShow = Ease.OutBack;
    [SerializeField] private Ease easeHide = Ease.InBack;

    public UnityEvent onShow;
    public UnityEvent onHide;

    public UnityEvent onShowComplete;
    public UnityEvent onHideStart;

    private bool _setStartPosition;
    private Vector3 _startPosition;

    private Sequence _showSeq;
    private List<float> _baseAlphaValues;

    private bool _isValidate;

    public float TimeAnim => timeShow;

    private List<float> BaseAlphaValues
    {
      get
      {
        if (_baseAlphaValues != null) return _baseAlphaValues;
        SetBaseAlpha();
        return _baseAlphaValues;
      }
    }

    public void SetPositionShow(float position)
    {
      yPositionShow = position;
    }

    public void ResetStartPosition()
    {
      _startPosition = transform.localPosition;
    }

    public void Show(float delay)
    {
      DOVirtual.DelayedCall(delay, Show);
    }

    [Button]
    public void Show()
    {
      if (!_isValidate)
      {
        _isValidate = true;
        ValidateCurrentSprite();
      }
      gameObject.SetActive(true);
      _showSeq?.Kill();
      onShow?.Invoke();
      Tf.localPosition = GetEndPosition(direction);
      if (targetAlphaToItsBaseValue && _baseAlphaValues == null)
      {
        SetBaseAlpha();
      }
      SetAlpha(0);
      _showSeq = DOTween.Sequence();
      _showSeq.Append(Tf.DOLocalMove(GetStartPosition(), timeShow).SetEase(easeShow));
      if (fadeSprite)
      {
        if (!targetAlphaToItsBaseValue)
        {
          _showSeq.Join(DOVirtual.Float(0, 1, timeFade, SetAlpha));
        }
        else
        {
          for (int i = 0; i < sprites.Length; i++)
          {
            int i1 = i;
            _showSeq.Join(DOVirtual.Float(0, BaseAlphaValues[i], timeFade,
                x => sprites[i1].color = sprites[i1].color.SetAlpha(x)));
          }
        }
      }
      _showSeq.OnComplete(() =>
      {
        onShowComplete?.Invoke();
      });
    }

    public void Hide()
    {
      if (!_isValidate)
      {
        _isValidate = true;
        ValidateCurrentSprite();
      }
      onHideStart?.Invoke();
      _showSeq?.Kill();
      _showSeq = DOTween.Sequence();
      _showSeq.Append(Tf.DOLocalMove(GetEndPosition(hideDirection), timeShow).SetEase(easeHide));
      if (fadeSprite)
      {
        _showSeq.Join(DOVirtual.Float(1, 0, timeFade, SetAlpha).SetDelay(delayFadeOut));
      }
      _showSeq.OnComplete(() =>
      {
        gameObject.SetActive(false);
        onHide?.Invoke();
      });
    }

    public void Hide(float delay)
    {
      this.WaitToDo(Hide, delay);
    }

    void Start()
    {
      if (showOnStart)
      {
        Show();
      }
    }

    private Vector3 GetStartPosition()
    {
      if (!_setStartPosition)
      {
        _startPosition = Tf.localPosition;
        _setStartPosition = true;
      }
      return _startPosition;
    }

    private Vector3 GetEndPosition(Direction direct)
    {
      Vector3 offset = Vector3.zero;
      switch (direct)
      {
        case Direction.Up:
          offset = new Vector3(0, yPositionShow, 0);
          break;
        case Direction.Down:
          offset = new Vector3(0, -yPositionShow, 0);
          break;
        case Direction.Left:
          offset = new Vector3(-yPositionShow, 0, 0);
          break;
        case Direction.Right:
          offset = new Vector3(yPositionShow, 0, 0);
          break;
      }
      return GetStartPosition() + offset;
    }

    private void SetAlpha(float alpha)
    {
      foreach (var sprite in sprites)
      {
        sprite.color = sprite.color.SetAlpha(alpha);
      }
    }

    private void FindSpriteRenderer(bool includeInactive = false)
    {
      var spriteList = GetComponentsInChildren<SpriteRenderer>(includeInactive).ToList();
      // to List 
      sprites = spriteList.Where(s => s.enabled && s.color.a > 0).ToArray();
    }

    private void ValidateCurrentSprite()
    {
      // check all sprite in list, if something missing or null, remove it
      sprites = sprites.Where(s => s != null).ToArray();
    }

    private void SetBaseAlpha()
    {
      _baseAlphaValues = new List<float>();
      foreach (var sprite in sprites)
      {
        _baseAlphaValues.Add(sprite.color.a);
      }
    }
  }
}