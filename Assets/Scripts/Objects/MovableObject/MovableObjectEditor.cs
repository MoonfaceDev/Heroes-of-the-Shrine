using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovableObject))]
class MovableObjectEditor : Editor
{
    SerializedProperty figureObject;
    SerializedProperty startPosition;

    void OnEnable()
    {
        figureObject = serializedObject.FindProperty("figureObject");
        startPosition = serializedObject.FindProperty("startPosition");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(figureObject);
        EditorGUILayout.PropertyField(startPosition);
        MovableObject movableObject = (MovableObject)target;
        FieldInfo figureObjectField = target.GetType().GetField(figureObject.propertyPath);
        if (figureObjectField != null)
        {
            movableObject.figureObject = (GameObject)figureObjectField.GetValue(target);
        }
        movableObject.startPosition = startPosition.vector3Value;
        Console.WriteLine(movableObject.startPosition);
        movableObject.transform.localPosition = MovableObject.GroundScreenCoordinates(movableObject.startPosition);
        if (movableObject.figureObject != null)
        {
            movableObject.figureObject.transform.localPosition = MovableObject.ScreenCoordinates(movableObject.startPosition.y * Vector3.up);
        }
        serializedObject.ApplyModifiedProperties();
    }
}