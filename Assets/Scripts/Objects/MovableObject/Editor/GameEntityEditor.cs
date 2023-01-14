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

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "rotation");
        var entity = (GameEntity)target;

        var selectedRotation = (RotationType)EditorGUILayout.EnumPopup(
            entity.rotation == Rotation.Left ? RotationType.Left : RotationType.Right
        );
        entity.rotation = selectedRotation == RotationType.Left ? Rotation.Left : Rotation.Right;

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(entity);
        }
    }
}