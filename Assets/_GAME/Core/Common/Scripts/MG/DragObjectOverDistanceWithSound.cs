using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;


public class DragObjectOverDistanceWithSound : DragObjectOverDistanceAction
{
  [Header("DRAG SOUND")]
  public AudioSource audioSource;

  public float dragVolumeRatio = 0.001f;
  public float dragVolumeChangeSpeed = 2;
  public float dragVolumeDecaySpeed = 2;
  public float dragVolumeMax = 1;
  public float dragVolumeMin = 0;
  public bool volumeOnDragDownwardOnly = false; // only set volume if the object is moving downward relative to its local Y axis

  [Header("DRAG VIBRATION")]
  public bool useVibration = true;
  [Range(0, 1)]
  public float vibrationStrength = 0.1f;
  private float vibrationSharpness = 0.1f;
  private float vibrationIntensity = 0.1f; // Adjust this for stronger or weaker vibration
  public float vibrationVolThreshold = 0.001f;
  private const float vibrationDuration = 0.1f; // Duration of each vibration
  private float lastVibrationTime;

  private float targetVolume;

  // private WaitForSeconds w1 = WaitForSecondCache.Get(vibrationDuration);

  protected override void Update()
  {
    base.Update();

    if (audioSource != null)
    {
      if (LevelBase.Instance.isEndingGame)
      {
        targetVolume = 0;
        audioSource.volume = 0;
        if (audioSource.isPlaying)
        {
          audioSource.Stop();
        }
      }
      else
      {
        audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, Time.deltaTime * dragVolumeChangeSpeed);
      }
    }
  }

  /// <summary>
  /// While dragging
  /// </summary>
  /// <param name="direction">The direction of the drag, including the delta as the magnitude of the direction vector</param>
  public override void OnDrag(Vector3 direction)
  {
    base.OnDrag(direction);

    float target = Mathf.Clamp(direction.sqrMagnitude / dragVolumeRatio, dragVolumeMin, dragVolumeMax);

    if (volumeOnDragDownwardOnly && direction.y >= 0 && target > targetVolume) return;

    if (audioSource != null)
    {
      // increase faster than decrease
      if (target > targetVolume)
      {
        targetVolume = target;
        audioSource.volume = targetVolume;
      }
      else
      {
        targetVolume = Mathf.MoveTowards(targetVolume, target, Time.deltaTime * dragVolumeDecaySpeed);
      }
    }

    vibrationIntensity = target <= vibrationVolThreshold ? 0 : vibrationStrength;

    //// MMVibrationManager.UpdateContinuousHaptic(target, vibrationSharpness);

    if (lastVibrationTime + vibrationDuration < Time.time)
    {
      lastVibrationTime = Time.time;
    }
  }

  public override void OnDragStop()
  {
    base.OnDragStop();
    Debug.Log("Drag stop");
    targetVolume = 0;
    // MMVibrationManager.StopContinuousHaptic();
  }

  public override void OnDragStart()
  {
    base.OnDragStart();
    //vibrationIntensity = 0.1f;
    //Vibrate();
  }

  Coroutine corVibrate;


}
