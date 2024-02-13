using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BrainModule), true)]
public class BrainModuleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var module = (BrainModule)target;
        GUI.enabled = false;
        EditorGUILayout.TextField("Parameters", FormatParameters(module.GetParameters()));
        GUI.enabled = true;
    }

    private static string FormatParameters(string[] parameters)
    {
        return string.Join(", ", parameters);
    }
}