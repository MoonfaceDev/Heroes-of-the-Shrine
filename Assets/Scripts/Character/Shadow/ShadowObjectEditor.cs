using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShadowObject))]
class ShadowObjectEditor : Editor
{
    SerializedProperty shadowScale;
    SerializedProperty movableObject;
    SerializedProperty knockbackBehaviour;
    SerializedProperty recoveringFromKnockbackScale;

    void OnEnable()
    {
        shadowScale = serializedObject.FindProperty("shadowScale");
        movableObject = serializedObject.FindProperty("movableObject");
        knockbackBehaviour = serializedObject.FindProperty("knockbackBehaviour");
        recoveringFromKnockbackScale = serializedObject.FindProperty("recoveringFromKnockbackScale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
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
            float scale = 2 / (1 + Mathf.Exp(0.2f * shadowObject.movableObject.startPosition.y));
            shadowObject.transform.localScale = MovableObject.GroundScreenCoordinates(Vector3.Scale(shadowObject.shadowScale, scale * Vector3.one));
        }
        EditorGUILayout.PropertyField(knockbackBehaviour);
        shadowObject.knockbackBehaviour = (KnockbackBehaviour) knockbackBehaviour.objectReferenceValue;
        if (shadowObject.knockbackBehaviour)
        {
            EditorGUILayout.PropertyField(recoveringFromKnockbackScale);
            shadowObject.recoveringFromKnockbackScale = recoveringFromKnockbackScale.floatValue;
        }
        serializedObject.ApplyModifiedProperties();
    }
}