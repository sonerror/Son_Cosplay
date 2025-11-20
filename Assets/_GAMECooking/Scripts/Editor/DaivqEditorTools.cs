using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class DaivqEditorTools : EditorWindow
{
    private string foldersInput = "Assets/";
    private bool isProcessing = false;
    private bool cancelRequested = false;

    [MenuItem("Tools/DaivqEditorTools")]
    public static void ShowWindow()
    {
        GetWindow<DaivqEditorTools>("Daivq Editor Tools");
    }

    private void OnGUI()
    {
        GUILayout.Label("Reset Android Texture Overrides", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        foldersInput = EditorGUILayout.TextField("Folders (comma-separated):", foldersInput);

        EditorGUILayout.Space();

        if (!isProcessing)
        {
            if (GUILayout.Button("Apply Android Settings To iOS"))
            {
                string[] folders = foldersInput
                    .Split(',')
                    .Select(f => f.Trim())
                    .Where(f => !string.IsNullOrEmpty(f))
                    .ToArray();

                cancelRequested = false;
                isProcessing = true;
                ApplyAndroidSettingsToiOS(folders);
            }

            // apply audio settings to iOS


            if (GUILayout.Button("Load From Selected Folders"))
            {
                var selectedFolders = Selection.assetGUIDs
                    .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                    .Where(path => Directory.Exists(path))
                    .ToArray();
                if (selectedFolders.Length > 0)
                {
                    foldersInput = string.Join(", ", selectedFolders);
                }
                else
                {
                    Debug.LogWarning("No valid folders selected.");
                }
            }

            // button count assets textures
            if (GUILayout.Button("Count Textures in Folders"))
            {
                string[] folders = foldersInput
                    .Split(',')
                    .Select(f => f.Trim())
                    .Where(f => !string.IsNullOrEmpty(f))
                    .ToArray();
                int textureCount = 0;
                foreach (string folder in folders)
                {
                    string[] guids = AssetDatabase.FindAssets("t:Texture", new[] { folder });
                    textureCount += guids.Length;
                }
                Debug.Log($"Total textures found: {textureCount}");
            }

            // button count audioclip
            if (GUILayout.Button("Count Audio Clips in Folders"))
            {
                string[] folders = foldersInput
                    .Split(',')
                    .Select(f => f.Trim())
                    .Where(f => !string.IsNullOrEmpty(f))
                    .ToArray();
                int audioClipCount = 0;
                foreach (string folder in folders)
                {
                    string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { folder });
                    audioClipCount += guids.Length;
                }
                Debug.Log($"Total audio clips found: {audioClipCount}");
            }
        }
        else
        {
            if (GUILayout.Button("Cancel"))
            {
                cancelRequested = true;
            }
        }
    }

    private void ApplyAndroidSettingsToiOS(string[] folders)
    {
        int count = 0;

        try
        {
            AssetDatabase.StartAssetEditing();

            foreach (string folder in folders)
            {
                string[] guids = AssetDatabase.FindAssets("t:Texture", new[] { folder });
                List<string> pathsToReimport = new List<string>();

                foreach (string guid in guids)
                {
                    if (cancelRequested)
                    {
                        Debug.LogWarning("Operation canceled by user.");
                        return;
                    }

                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

                    if (importer != null)
                    {
                        TextureImporterPlatformSettings android = importer.GetPlatformTextureSettings("Android");
                        TextureImporterPlatformSettings ios = importer.GetPlatformTextureSettings("iPhone");

                        ios.name = "iPhone";
                        ios.overridden = android.overridden;
                        ios.format = android.format;
                        ios.maxTextureSize = android.maxTextureSize;
                        ios.compressionQuality = android.compressionQuality;
                        ios.resizeAlgorithm = android.resizeAlgorithm;
                        ios.textureCompression = android.textureCompression;

                        importer.SetPlatformTextureSettings(ios);
                        EditorUtility.SetDirty(importer);
                        pathsToReimport.Add(path);
                        count++;
                    }
                }

                foreach (var path in pathsToReimport)
                {
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error during scripted asset import: {e.Message}");
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        isProcessing = false;
        Debug.Log($"Apply Android overrides on {count} texture(s).");
    }

    //private void ApplyAndroidSettingsToiOSAudioClip(string[] folders)
    //{
    //    int count = 0;

    //    try
    //    {
    //        AssetDatabase.StartAssetEditing();

    //        foreach (string folder in folders)
    //        {
    //            string[] guids = AssetDatabase.FindAssets("t:Texture", new[] { folder });

    //            foreach (string guid in guids)
    //            {
    //                if (cancelRequested)
    //                {
    //                    Debug.LogWarning("Operation canceled by user.");
    //                    return;
    //                }

    //                string path = AssetDatabase.GUIDToAssetPath(guid);
    //                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

    //                if (importer != null)
    //                {
    //                    TextureImporterPlatformSettings android = importer.GetPlatformTextureSettings("Android");
    //                    TextureImporterPlatformSettings ios = importer.GetPlatformTextureSettings("iPhone");

    //                    ios.name = "iPhone";
    //                    ios.overridden = true;
    //                    ios.format = android.format;
    //                    ios.maxTextureSize = android.maxTextureSize;
    //                    ios.compressionQuality = android.compressionQuality;
    //                    ios.resizeAlgorithm = android.resizeAlgorithm;
    //                    ios.textureCompression = android.textureCompression;

    //                    importer.SetPlatformTextureSettings(ios);
    //                    EditorUtility.SetDirty(importer);
    //                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    //                    count++;
    //                }
    //            }
    //        }
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.LogError($"Error during scripted asset import: {e.Message}");
    //    }
    //    finally
    //    {
    //        AssetDatabase.StopAssetEditing();
    //    }

    //    AssetDatabase.SaveAssets();
    //    AssetDatabase.Refresh();
    //    isProcessing = false;
    //    Debug.Log($"Apply Android overrides on {count} audio(s).");
    //}
}