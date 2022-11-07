using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShadowObject))]
class ShadowObjectEditor : Editor
{
    SerializedProperty shadowScale;
    SerializedProperty movableObject;

    void OnEnable()
    {
        shadowScale = serializedObject.FindProperty("shadowScale");
        movableObject = serializedObject.FindProperty("movableObject");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "shadowScale", "movableObject");
        EditorGUILayout.PropertyField(shadowScale);
        EditorGUILayout.PropertyField(movableObject);
        ShadowObject shadowObject = (ShadowObject)target;
        shadowObject.shadowScale = shadowScale.vector3Value;
        FieldInfo movableObjectField = target.GetType().GetField(movableObject.propertyPath);
        if (movableObjectField != null)
        {
            shadowObject.movableObject = (MovableObject)movableObjectField.GetValue(target);
        }
        if (shadowObject.movableObject != null)
        {
            float scale = 2 / (1 + Mathf.Exp(0.2f * shadowObject.movableObject.WorldPosition.y));
            shadowObject.transform.localScale = MovableObject.GroundScreenCoordinates(Vector3.Scale(shadowObject.shadowScale, scale * Vector3.one));
        }
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(shadowObject);
        }
    }
}