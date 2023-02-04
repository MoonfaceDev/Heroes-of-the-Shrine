using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Rotation))]
public class RotationDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property,
        GUIContent label)
    {
        var value = property.FindPropertyRelative("flipped");
        value.boolValue = EditorGUI.Toggle(position, property.displayName, value.boolValue);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}