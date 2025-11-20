
using UnityEngine;

public class LoopSprite : MonoBehaviour
{
  [SerializeField] private SpriteRenderer spriteRenderer;
  [SerializeField] private Sprite[] sprites;

  [SerializeField]
  private float timeInterval;

  [SerializeField] private bool randomTimeIntervals;

  [SerializeField]
  private Vector2 timeIntervalRange;



  [SerializeField] private bool changeSpriteWhenDisable;
  [SerializeField]
  private Sprite disableSprite;

  private int _currentIndex;
  private float _time;
  private float _timeInterval;
  public SpriteRenderer SpriteRenderer => spriteRenderer;

  private void Start()
  {
    _timeInterval = GetTimeInterval;
  }

  private void Update()
  {
    _time += Time.deltaTime;
    if (_time < _timeInterval) return;
    _time = 0;
    _currentIndex = (_currentIndex + 1) % sprites.Length;
    spriteRenderer.sprite = sprites[_currentIndex];
    _timeInterval = GetTimeInterval;
  }

  private float GetTimeInterval => randomTimeIntervals ? Random.Range(timeIntervalRange.x, timeIntervalRange.y) : timeInterval;

  private void OnDisable()
  {
    if (changeSpriteWhenDisable)
    {
      spriteRenderer.sprite = disableSprite;
    }
  }

#if UNITY_EDITOR

  private void GetReferences()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
  }
#endif

}