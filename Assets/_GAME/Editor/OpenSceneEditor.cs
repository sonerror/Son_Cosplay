using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

public class OpenSceneEditor : MonoBehaviour
{
#if UNITY_EDITOR
    private static void OpenScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath + ".unity");
        }
    }

    // Kí tự & là biểu tượng cho phím Alt
    [MenuItem("Open Scene/Loading &1")]
    public static void OpenLoadingScene()
    {
        OpenScene("Assets/_GAME/_Costopia/_Scene/LoadingCostopia");
    }
    
    [MenuItem("Open Scene/Init &2")]
    public static void OpenInitScene()
    {
        OpenScene("Assets/_GAME/_Costopia/_Scene/InitCostopia");
    }
    
    [MenuItem("Open Scene/Home &3")]
    public static void OpenHomeScene()
    {
        OpenScene("Assets/_GAME/_Costopia/_Scene/HomeCostopia"); 
    }
    
    [MenuItem("Open Scene/Main Level Overlay &4")]
    public static void OpenMainLevelOverlayScene()
    {
        OpenScene("Assets/_GAME/_Costopia/_Scene/MainLevelOverlayCostopia"); 
    }
    
#endif
}
