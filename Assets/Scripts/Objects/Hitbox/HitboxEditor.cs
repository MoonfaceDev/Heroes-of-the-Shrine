using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Hitbox))]
class HitboxEditor : Editor
{
    SerializedProperty position;
    SerializedProperty size;
    SerializedProperty parentObject;

    void OnEnable()
    {
        position = serializedObject.FindProperty("position");
        size = serializedObject.FindProperty("size");
        parentObject = serializedObject.FindProperty("parentObject");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(position);
        EditorGUILayout.PropertyField(size);
        EditorGUILayout.PropertyField(parentObject);
        Hitbox hitbox = (Hitbox)target;
        hitbox.position = position.vector3Value;
        hitbox.size = size.vector3Value;
        FieldInfo parentField = target.GetType().GetField(parentObject.propertyPath);
        if (parentField != null)
        {
            hitbox.parentObject = (MovableObject) parentField.GetValue(target);
        }
        if (hitbox.parentObject != null)
        {
            Vector3 worldPosition = new(hitbox.GetLeft(), hitbox.GetBottom(), hitbox.GetFar());
            hitbox.transform.position = MovableObject.ScreenCoordinates(worldPosition);
        }
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(hitbox);
        }
    }
}