using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using sonnv;
public class ShowObjectEffectUI : SonMonoBehaviour
{
    [SerializeField] private Direction direction = Direction.Up;
    [SerializeField] private Direction hideDirection = Direction.Down;
    [SerializeField] private float yPositionShow = 2f;
    [SerializeField] private bool fadeImage = true;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float timeFade = 0.2f;
    [SerializeField] private float delayFadeOut = 0.3f;


    [SerializeField] private float timeShow = 0.5f;
    [SerializeField] private bool showOnEnable;
    [SerializeField] private Ease easeShow = Ease.OutBack;
    [SerializeField] private Ease easeHide = Ease.InBack;

    public UnityEvent onShow;
    public UnityEvent onHide;

    public UnityEvent onShowComplete;
    public UnityEvent onHideStart;

    public CanvasGroup CanvasGroup => canvasGroup;

    private bool _setStartPosition;
    private Vector3 _startPosition;

    private Sequence _showSeq;

    private bool _isValidate;

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

    public void ShowInstantly()
    {
        gameObject.SetActive(true);
        Tf.localPosition = GetStartPosition();
        canvasGroup.alpha = 1;
        onShow.Invoke();
    }

    public void HideInstantly()
    {
        gameObject.SetActive(false);
        Tf.localPosition = GetEndPosition(hideDirection);
        canvasGroup.alpha = 0;
        onHide.Invoke();
    }

    [Button]
    public void Show()
    {
        gameObject.SetActive(true);
        _showSeq?.Kill();
        onShow?.Invoke();
        Tf.localPosition = GetEndPosition(direction);
        canvasGroup.alpha = 0;
        _showSeq = DOTween.Sequence();
        _showSeq.Append(Tf.DOLocalMove(GetStartPosition(), timeShow).SetEase(easeShow));
        if (fadeImage)
        {
            _showSeq.Join(DOVirtual.Float(0, 1, timeFade, (x) => canvasGroup.alpha = x));
        }
        _showSeq.SetUpdate(true);
        _showSeq.OnComplete(() =>
        {
            onShowComplete?.Invoke();
        });
    }

    [Button]
    public void Hide()
    {
        onHideStart?.Invoke();
        _showSeq?.Kill();
        _showSeq = DOTween.Sequence();
        _showSeq.Append(Tf.DOLocalMove(GetEndPosition(hideDirection), timeShow).SetEase(easeHide));
        if (fadeImage)
        {
            _showSeq.Join(DOVirtual.Float(1, 0, timeFade, (x) => canvasGroup.alpha = x)).SetDelay(delayFadeOut);
        }
        _showSeq.SetUpdate(true);
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

    private void OnEnable()
    {
        if (showOnEnable)
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

}