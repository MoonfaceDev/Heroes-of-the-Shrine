using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BasePattern), true)]
public class BasePatternEditor : Editor
{
    private SerializedProperty hasRandomExitTime;
    private SerializedProperty minTime;
    private SerializedProperty maxTime;

    private void OnEnable()
    {
        hasRandomExitTime = serializedObject.FindProperty("hasRandomExitTime");
        minTime = serializedObject.FindProperty("minTime");
        maxTime = serializedObject.FindProperty("maxTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "hasRandomExitTime", "minTime", "maxTime");
        var pattern = (BasePattern)target;

        EditorGUILayout.PropertyField(hasRandomExitTime);
        pattern.hasRandomExitTime = hasRandomExitTime.boolValue;
        if (pattern.hasRandomExitTime)
        {
            EditorGUILayout.PropertyField(minTime);
            pattern.minTime = minTime.floatValue;

            EditorGUILayout.PropertyField(maxTime);
            pattern.maxTime = maxTime.floatValue;
        }

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(pattern);
        }
    }
}
