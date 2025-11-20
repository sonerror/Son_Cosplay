using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class FontReplacer : EditorWindow
{
    private GameObject targetObject;
    private Font newUIFont;
    private TMP_FontAsset newTMPFont;

    [MenuItem("Tools/Font Replacer")]
    public static void ShowWindow()
    {
        GetWindow<FontReplacer>("Font Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Replace Fonts in Object Hierarchy", EditorStyles.boldLabel);

        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);

        EditorGUILayout.Space();

        newUIFont = (Font)EditorGUILayout.ObjectField("New UI Font (Text)", newUIFont, typeof(Font), false);
        newTMPFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New TMP Font (TextMeshPro)", newTMPFont, typeof(TMP_FontAsset), false);

        EditorGUILayout.Space();

        if (GUILayout.Button("Replace Fonts"))
        {
            if (targetObject == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a Target Object.", "OK");
                return;
            }

            ReplaceFonts(targetObject);
            EditorUtility.DisplayDialog("Done", "Fonts replaced successfully!", "OK");
        }
    }

    private void ReplaceFonts(GameObject obj)
    {
        // Replace legacy UI.Text fonts
        if (newUIFont != null)
        {
            Text[] uiTexts = obj.GetComponentsInChildren<Text>(true);
            foreach (var t in uiTexts)
            {
                Undo.RecordObject(t, "Replace Font");
                t.font = newUIFont;
                EditorUtility.SetDirty(t);
            }
        }

        // Replace TMP_Text fonts
        if (newTMPFont != null)
        {
            TMP_Text[] tmpTexts = obj.GetComponentsInChildren<TMP_Text>(true);
            foreach (var t in tmpTexts)
            {
                Undo.RecordObject(t, "Replace TMP Font");
                t.font = newTMPFont;
                EditorUtility.SetDirty(t);
            }
        }
    }
}
