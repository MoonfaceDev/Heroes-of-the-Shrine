using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Hitbox))]
internal class HitboxEditor : Editor
{
    private SerializedProperty size;

    private void OnEnable()
    {
        size = serializedObject.FindProperty("size");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(size);
        var hitbox = (Hitbox)target;
        hitbox.size = size.vector3Value;
        Vector3 worldPosition = new(hitbox.GetLeft(), hitbox.GetBottom(), hitbox.GetFar());
        hitbox.transform.position = MovableObject.ScreenCoordinates(worldPosition);
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(hitbox);
        }
    }
}