using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Costopia.Level
{
  [CreateAssetMenu(fileName = "CostopiaLevelEvent", menuName = "Costopia/CostopiaLevelEvent", order = 0)]
  public class CostopiaLevelEventSO : ScriptableObject
  {
    private const float DEFAULT_TIME_LIMIT = 180f;
    private const int DEFAULT_MOOD_WRONG_DECREASE = 10;
    private const int DEFAULT_MOOD_CORRECT_INCREASE = 5;

    [Header("Event Info")]
    [SerializeField] private string id;
    public string Id => id;

    public string keyRemoteConfig;

    public string ModeNameTracking => id; // $"evt_{_id}";

    public string NameLocalizeKey => $"Title_{id}";

    [Header("Level Sequence")]
    private int[] _levelSequence;
    public int[] LevelSequence => _levelSequence;
    public int NumLevels => _levelSequence.Length;

    public int GetMappedLevel(int levelIndex)
    {
      return _levelMapping.GetValueOrDefault(levelIndex, -1);
    }

    public float GetTimeLimit(int mappedLevel)
    {
      return _levelTimeLimit.GetValueOrDefault(mappedLevel, DEFAULT_TIME_LIMIT);
    }

    public int GetMoodWrongDecrease(int mappedLevel)
    {
      return _levelMoodWrongDecrease.GetValueOrDefault(mappedLevel, DEFAULT_MOOD_WRONG_DECREASE);
    }

    public int GetMoodCorrectIncrease(int mappedLevel)
    {
      return _levelMoodCorrectIncrease.GetValueOrDefault(mappedLevel, DEFAULT_MOOD_CORRECT_INCREASE);
    }

    public int GetLevelIndex(int mappedLevel)
    {
      for (int i = 0; i < _levelSequence.Length; i++)
      {
        if (_levelSequence[i] == mappedLevel)
        {
          return i;
        }
      }
      return -1;
    }

    #region Runtime
    [Serializable]
    private class LevelConfig
    {
      public string lvSeq;
      public string timeLimit;
      public string moodWrongDecrease;
      public string moodCorrectIncrease;
    }
    [SerializeField] private LevelConfig levelConfig;

    private Dictionary<int, int> _levelMapping;
    private Dictionary<int, float> _levelTimeLimit;
    private Dictionary<int, int> _levelMoodWrongDecrease;
    private Dictionary<int, int> _levelMoodCorrectIncrease;





    #endregion

#if UNITY_EDITOR
    [Header("Editor Tool")]
    // [ValidateInput("IsGameEventIdValidEditor", "Duplicated id of some CosplayEventSO")]

    private bool _isGameEventIdValidEditor;
    private bool IsGameEventIdValidEditor() => _isGameEventIdValidEditor;

    private void OnValidate()
    {
      if (string.IsNullOrEmpty(id))
      {
        AssignNewId();
      }
      CheckUniqueIdAllAssets();
    }


    private void AssignNewId()
    {
      id = Guid.NewGuid().ToString();
      CheckUniqueIdAllAssets();
      UnityEditor.EditorUtility.SetDirty(this);
    }


    private void FixUniqueIdAllAssets()
    {
      if (UnityEditor.EditorUtility.DisplayDialog("Check Unique ID", "Are you sure to check all CosplayEventSO assets in project?", "YES!!!", "Nooo"))
      {
        CheckUniqueIdAllAssetsStatic(true);
        CheckUniqueIdAllAssets();
      }
    }


    private void CheckUniqueIdAllAssets()
    {
      _isGameEventIdValidEditor = CheckUniqueIdAllAssetsStatic();
    }

    private static bool CheckUniqueIdAllAssetsStatic(bool isFix = false)
    {
      bool isAnyDuplicate = false;
      Dictionary<string, CostopiaLevelEventSO> idToAsset = new Dictionary<string, CostopiaLevelEventSO>();
      string[] guids = UnityEditor.AssetDatabase.FindAssets("t:CosplayEventSO", new[] { "Assets/_GAME" });
      foreach (var guid in guids)
      {
        CostopiaLevelEventSO asset = UnityEditor.AssetDatabase.LoadAssetAtPath<CostopiaLevelEventSO>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid));
        if (asset)
        {
          if (!idToAsset.TryAdd(asset.Id, asset))
          {
            if (isFix)
            {
              // assign new id
              // H3Log.LogWarning($"Duplicate ID: {asset.Id} in {asset.name} and {idToAsset[asset.Id].name}");
              asset.AssignNewId();
              idToAsset.Add(asset.Id, asset);
              // H3Log.Log($"New ID: {asset.Id} in {asset.name}");
            }
            isAnyDuplicate = true;
          }
        }
      }
      return !isAnyDuplicate;
    }
#endif

#if UNITY_EDITOR
    //[Header("Editor Tool")]
    // [Button("Add Custom Level To Build Setting")]
    private void AddCustomLevelToBuildSettingEditor()
    {
      if (string.IsNullOrEmpty(levelConfig.lvSeq))
      {
        // H3Log.LogError("Custom Level Sequence is empty");
        return;
      }

      var tempScenes = UnityEditor.EditorBuildSettings.scenes.ToList();

      string[] splitStrs = levelConfig.lvSeq.Split(',');
      for (int i = 0; i < splitStrs.Length; i++)
      {
        string trimmed = splitStrs[i].Trim();
        if (string.IsNullOrEmpty(trimmed)) continue;

        int mappedLevel = int.Parse(trimmed);

        string sceneName = CostopiaLevelSO.LevelIdToSceneName(mappedLevel);

        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:scene " + sceneName);

        if (guids.Length == 0)
        {
          // H3Log.LogError("Scene " + sceneName + " does not exist in project");
          continue;
        }

        int countDuplicate = 0;
        foreach (string guid in guids)
        {
          string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
          string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
          if (fileName != sceneName) continue;

          countDuplicate += 1;
          if (countDuplicate > 1)
          {
            // H3Log.LogWarning("Duplicate scene name: " + sceneName);
          }

          bool isAlreadyHavePath = false;
          for (int j = 0; j < tempScenes.Count; j++)
          {
            if (tempScenes[j].path == path)
            {
              isAlreadyHavePath = true;
              break;
            }
          }

          if (isAlreadyHavePath)
          {
            // H3Log.Log("Scene already in build: " + path);
          }
          else
          {
            // H3Log.Log("Add scene to build: " + path);
            var newScene = new UnityEditor.EditorBuildSettingsScene(path, true);
            if (!tempScenes.Contains(newScene))
            {
              tempScenes.Add(newScene);
            }
          }
        }
      }

      UnityEditor.EditorBuildSettings.scenes = tempScenes.ToArray();
    }
#endif

#if UNITY_EDITOR
    [SerializeField] private string configEditor;
    // [Button("Load Config Editor")]
    private void LoadConfigEditor()
    {
      try
      {
        levelConfig = JsonUtility.FromJson<LevelConfig>(configEditor);
        UnityEditor.EditorUtility.SetDirty(this);
      }
      catch (Exception e)
      {
        Debug.LogException(e);
      }
    }
#endif
  }
}