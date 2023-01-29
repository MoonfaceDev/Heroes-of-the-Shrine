using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Rotation))]
public class RotationDrawer : PropertyDrawer
{
    private readonly string[] names = {"Left", "Right"};
    private readonly int[] values = {-1, 1};

    public override void OnGUI(Rect position, SerializedProperty property,
        GUIContent label)
    {
        var value = property.FindPropertyRelative("value");
        
        var newValue = EditorGUI.IntPopup(position, "Rotation", value.intValue, names, values);

        if (EditorGUI.EndChangeCheck())
        {
            value.intValue = newValue;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}