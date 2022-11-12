﻿using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovableObject))]
internal class MovableObjectEditor : Editor
{
    private SerializedProperty figureObject;
    private SerializedProperty position;
    private SerializedProperty rotation;
    private SerializedProperty scale;

    private void OnEnable()
    {
        figureObject = serializedObject.FindProperty("figureObject");
        position = serializedObject.FindProperty("position");
        rotation = serializedObject.FindProperty("rotation");
        scale = serializedObject.FindProperty("scale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "figureObject", "position", "rotation", "scale");
        EditorGUILayout.PropertyField(figureObject);
        EditorGUILayout.PropertyField(position);
        EditorGUILayout.PropertyField(rotation);
        EditorGUILayout.PropertyField(scale);
        var movableObject = (MovableObject)target;
        var figureObjectField = target.GetType().GetField(figureObject.propertyPath);
        if (figureObjectField != null)
        {
            movableObject.figureObject = (Renderer)figureObjectField.GetValue(target);
        }
        movableObject.position = position.vector3Value;
        movableObject.transform.localPosition = MovableObject.GroundScreenCoordinates(movableObject.position);
        movableObject.rotation = rotation.quaternionValue;
        movableObject.transform.localRotation = movableObject.rotation;
        movableObject.scale = scale.vector3Value;
        movableObject.transform.localScale = movableObject.scale;
        if (movableObject.figureObject)
        {
            movableObject.figureObject.transform.localPosition = MovableObject.ScreenCoordinates(movableObject.position.y * Vector3.up);
        }
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(movableObject);
        }
    }
}