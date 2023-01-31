using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Rotation))]
public class RotationDrawer : PropertyDrawer
{
    private static readonly Rotation.Value[] Values = { Rotation.Value.Left, Rotation.Value.Right };

    public override void OnGUI(Rect position, SerializedProperty property,
        GUIContent label)
    {
        var value = property.FindPropertyRelative("value");

        var newValue = (Rotation.Value)EditorGUI.EnumPopup(position, property.displayName,
            (Rotation.Value)Values.GetValue(value.enumValueIndex));

        value.enumValueIndex = Array.IndexOf(Values, newValue);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}