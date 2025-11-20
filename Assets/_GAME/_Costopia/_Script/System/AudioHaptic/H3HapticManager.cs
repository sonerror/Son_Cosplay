
using UnityEngine;

namespace HoangHH.AudioHaptic
{
  public class H3HapticManager : H3Singleton<H3HapticManager>
  {
    public delegate void OnActiveValueChanged(bool isOn);

    public event OnActiveValueChanged onHapticConfigChanged;

    private const string KEY_HAPTIC_ACTIVE = "HapticActive";

    public bool IsHapticActive => PlayerPrefs.GetInt(KEY_HAPTIC_ACTIVE, 1) == 1;

    private void Start()
    {
      ApplyHapticConfig();
    }

    public void ToggleHaptic()
    {
      bool isActive = PlayerPrefs.GetInt(KEY_HAPTIC_ACTIVE, 1) == 1;
      SetHapticActive(!isActive);
    }

    private void SetHapticActive(bool isActive)
    {
      PlayerPrefs.SetInt(KEY_HAPTIC_ACTIVE, isActive ? 1 : 0);
      PlayerPrefs.Save();
      ApplyHapticConfig();
    }

    private void ApplyHapticConfig()
    {
      bool isHapticOn = PlayerPrefs.GetInt(KEY_HAPTIC_ACTIVE, 1) == 1;
      // MMVibrationManager.SetHapticsActive(isHapticOn);
      onHapticConfigChanged?.Invoke(isHapticOn);
    }
  }
}