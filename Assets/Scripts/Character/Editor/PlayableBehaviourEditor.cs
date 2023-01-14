using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayableBehaviour<>), true)]
[CanEditMultipleObjects]
public class PlayableBehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject);

        GUI.enabled = false;
        EditorGUILayout.Toggle("Playing", target is IPlayableBehaviour { Playing: true });
        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}