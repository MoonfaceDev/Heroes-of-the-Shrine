using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovableObject))]
class MovableObjectEditor : Editor
{
    SerializedProperty figureObject;
    SerializedProperty position;

    void OnEnable()
    {
        figureObject = serializedObject.FindProperty("figureObject");
        position = serializedObject.FindProperty("position");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(figureObject);
        EditorGUILayout.PropertyField(position);
        MovableObject movableObject = (MovableObject)target;
        FieldInfo figureObjectField = target.GetType().GetField(figureObject.propertyPath);
        if (figureObjectField != null)
        {
            movableObject.figureObject = (Renderer)figureObjectField.GetValue(target);
        }
        movableObject.position = position.vector3Value;
        movableObject.transform.localPosition = MovableObject.GroundScreenCoordinates(movableObject.position);
        if (movableObject.figureObject != null)
        {
            movableObject.figureObject.transform.localPosition = MovableObject.ScreenCoordinates(movableObject.position.y * Vector3.up);
        }
        serializedObject.ApplyModifiedProperties();
    }
}