using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
  [System.Serializable]
  public class KeyframeData
  {
    public float time;
    public float value;

    public KeyframeData(float time, float value)
    {
      this.time = time;
      this.value = value;
    }
  }

  public class LoopAnimTransformFloating : MonoBehaviour
  {
    public float cycleDuration = 1f;
    public float positionCurveMul = 1f;
    public KeyframeData[] curveX = new KeyframeData[] { new KeyframeData(0, 0), new KeyframeData(1, 0) };
    public KeyframeData[] curveY = new KeyframeData[] {
      new KeyframeData(0, 0),
      new KeyframeData(0.3f, 0.7f),
      new KeyframeData(0.6f, 0.7f),
      new KeyframeData(1, 0)
    };
    public bool isUseScaleTime = false;
    public bool isRandomTimeOffset = false;
    public Vector2 timeOffset = Vector2.zero;

    private Transform _transform;
    private Vector3 _startPos;

    private void OnEnable()
    {
      ResetFloatingAnchor();
    }

    public void ResetFloatingAnchor()
    {
      _transform = transform;
      _startPos = _transform.localPosition;
      if (isRandomTimeOffset)
      {
        timeOffset = new Vector2(
            UnityEngine.Random.Range(0f, cycleDuration),
            UnityEngine.Random.Range(0f, cycleDuration)
            );
      }
    }

    private float EvaluateCurve(KeyframeData[] keyframes, float time)
    {
      if (keyframes == null || keyframes.Length == 0) return 0f;
      if (keyframes.Length == 1) return keyframes[0].value;

      // Tìm 2 keyframe để interpolate
      for (int i = 0; i < keyframes.Length - 1; i++)
      {
        if (time >= keyframes[i].time && time <= keyframes[i + 1].time)
        {
          float t = (time - keyframes[i].time) / (keyframes[i + 1].time - keyframes[i].time);
          return Mathf.Lerp(keyframes[i].value, keyframes[i + 1].value, t);
        }
      }

      // Nếu time ngoài range, trả về giá trị đầu hoặc cuối
      if (time < keyframes[0].time) return keyframes[0].value;
      return keyframes[keyframes.Length - 1].value;
    }

    // Update is called once per frame
    void Update()
    {
      if (isClose)
      {
        CloseEye();
        return;
      }

      float timeX = Mathf.Repeat(((isUseScaleTime ? Time.time : Time.unscaledTime) + timeOffset.x) / cycleDuration, 1f);
      float timeY = Mathf.Repeat(((isUseScaleTime ? Time.time : Time.unscaledTime) + timeOffset.y) / cycleDuration, 1f);

      _transform.localPosition = new Vector3(
          _startPos.x + positionCurveMul * EvaluateCurve(curveX, timeX),
          _startPos.y + positionCurveMul * EvaluateCurve(curveY, timeY),
          _startPos.z
          );
    }

    private bool isClose = false;
    public void CloseEye()
    {
      isClose = true;
      _transform.localPosition = new Vector3(
         _startPos.x + positionCurveMul * EvaluateCurve(curveX, 1f),
         _startPos.y + positionCurveMul * EvaluateCurve(curveY, 1f),
         _startPos.z
         );
    }

    private void OnDisable()
    {
      _transform.localPosition = _startPos;
    }
  }
}