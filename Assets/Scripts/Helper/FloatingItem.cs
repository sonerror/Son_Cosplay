using DG.Tweening;
using UnityEngine;

public class FloatingItem1 : GameUnit
{
  [Header("Config Floating")]
  [SerializeField]
  protected SimpleCurve curveFloatingIdle = new SimpleCurve(
      new SimpleCurve.Keyframe(0, 0),
      new SimpleCurve.Keyframe(0.5f, 0.3f),
      new SimpleCurve.Keyframe(1, 0)
  );

  [SerializeField] protected float durationFloatingIdle = 2.41f;
  [SerializeField] protected float curveTimeOffSet = 0.25f;
  [SerializeField] protected float speedFloatingIdle = 0.4f;
  [SerializeField] protected bool isFloating;
  protected Vector3 floatingAnchor;
  protected float timeOffsetFloating;

  void Start()
  {
    UpdateFloatingAnchor();
  }

  public void StartFloating()
  {
    if (isFloating) return;
    isFloating = true;
    // UpdateFloatingAnchor();
  }

  public void StopFloating()
  {
    isFloating = false;
  }


  private void Update()
  {
    if (!isFloating) return;
    float timeProgress = Mathf.Repeat((Time.time + timeOffsetFloating) / durationFloatingIdle, 1f);
    Vector3 floatingPos = floatingAnchor + Vector3.up * curveFloatingIdle.Evaluate(timeProgress);
    Vector3 pos = Tf.position;
    pos = Vector3.Lerp(pos, floatingPos, speedFloatingIdle * Time.deltaTime * 50f);
    Tf.position = pos;

  }
  protected void UpdateFloatingAnchor()
  {
    floatingAnchor = Tf.position;
    timeOffsetFloating = (Mathf.Round(Time.time / durationFloatingIdle) + curveTimeOffSet) * durationFloatingIdle - Time.time;
  }

  public void Show()
  {
    gameObject.SetActive(true);
    Tf.DOScale(Vector3.one, 0.75f).From(Vector3.zero).SetDelay(0.5f).SetEase(Ease.OutBack)
    .OnComplete(() =>
    {
      StartFloating();
    });
  }

}
