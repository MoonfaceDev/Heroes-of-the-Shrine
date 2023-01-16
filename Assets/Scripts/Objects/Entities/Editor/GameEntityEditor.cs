using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameEntity), true)]
internal class GameEntityEditor : Editor
{
    private enum RotationType
    {
        Left,
        Right
    }

    private void DrawRotation()
    {
        var entity = (GameEntity)target;

        var selectedRotation = (RotationType)EditorGUILayout.EnumPopup("Rotation",
            entity.rotation == Rotation.Left ? RotationType.Left : RotationType.Right
        );
        entity.rotation = selectedRotation == RotationType.Left ? Rotation.Left : Rotation.Right;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("tags"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("parent"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("position"));
        DrawRotation();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("scale"));

        serializedObject.ApplyModifiedProperties();
    }
}