
using UnityEngine;

namespace HoangHH
{
  public class TouchSpeedSoundController : H3MonoBehaviour
  {
    [SerializeField] private bool hasTouchColliderZone;
    [SerializeField] private Collider2D touchZone;

    [SerializeField] private AudioSource audioSource;
    public float speedMultiplier = 1f / 1000f;
    public AnimationCurve volumeMapCurve;
    [Range(0f, 1f)] public float lerpVolume = 0.1f;

    private Vector2 _previousMousePosition = Vector2.zero;
    private Camera _mainCam;

    private void Awake()
    {
      _mainCam = Camera.main;
    }

    private void OnDisable()
    {
      audioSource.volume = 0f;
    }

    private void OnEnable()
    {
      audioSource.volume = 0f;
    }

    private void Update()
    {
      if (Time.timeScale <= Mathf.Epsilon)
      {
        audioSource.volume = 0f;
        return;
      }

      float totalSpeed = 0.0f;

      if (Input.GetMouseButtonDown(0))
      {
        _previousMousePosition = Input.mousePosition;
      }
      else if (Input.GetMouseButton(0))
      {
        Vector2 mousePosition = Input.mousePosition;

        if (hasTouchColliderZone)
        {
          Vector2 mouseWorldPos = _mainCam.ScreenToWorldPoint(mousePosition);

          if (touchZone.OverlapPoint(new Vector2(mouseWorldPos.x, mouseWorldPos.y)))
          {
            Vector2 speed = new Vector2(
                Mathf.Abs(mousePosition.x - _previousMousePosition.x) / Screen.width,
                Mathf.Abs(mousePosition.y - _previousMousePosition.y) / Screen.width
            ) / Time.deltaTime;

            totalSpeed += speed.magnitude;
          }
        }
        else
        {
          Vector2 mouseWorldPos = _mainCam.ScreenToWorldPoint(mousePosition);

          Vector2 speed = new Vector2(
              Mathf.Abs(mousePosition.x - _previousMousePosition.x) / Screen.width,
              Mathf.Abs(mousePosition.y - _previousMousePosition.y) / Screen.width
          ) / Time.deltaTime;

          totalSpeed += speed.magnitude;
        }
        _previousMousePosition = mousePosition;
      }

      audioSource.volume = Mathf.Lerp(audioSource.volume, volumeMapCurve.Evaluate(totalSpeed * speedMultiplier), lerpVolume);
    }

    public void OnChangeSound(AudioClip clip)
    {
      audioSource.clip = clip;
      audioSource.Play();
      // play again after clip end
      audioSource.loop = true;
    }
  }
}