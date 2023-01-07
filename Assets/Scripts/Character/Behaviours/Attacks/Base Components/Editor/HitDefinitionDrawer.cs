using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(HitDefinition))]
public class HitDefinitionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property,
        GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var rectFoldout = new Rect(position.min.x, position.min.y, position.size.x, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(rectFoldout, property.isExpanded, label);
        var lines = 0;

        if (!property.isExpanded) return;
        EditorGUI.PropertyField(GetRect(position, ++lines), property.FindPropertyRelative("damage"));
        var hitTypeIndex = property.FindPropertyRelative("hitType");
        EditorGUI.PropertyField(GetRect(position, ++lines), hitTypeIndex);
        var hitTypeValue = (HitType)typeof(HitType).GetEnumValues().GetValue(hitTypeIndex.enumValueIndex);
        if (hitTypeValue == HitType.Knockback)
        {
            EditorGUI.PropertyField(GetRect(position, ++lines), property.FindPropertyRelative("knockbackPower"));
            EditorGUI.PropertyField(GetRect(position, ++lines), property.FindPropertyRelative("knockbackDirection"));
        }

        EditorGUI.PropertyField(GetRect(position, ++lines), property.FindPropertyRelative("stunTime"));
    }

    private static Rect GetRect(Rect position, int line)
    {
        return new Rect(position.min.x, position.min.y + line * EditorGUIUtility.singleLineHeight, position.size.x,
            EditorGUIUtility.singleLineHeight);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var hitTypeIndex = property.FindPropertyRelative("hitType");
        var hitTypeValue = (HitType)typeof(HitType).GetEnumValues().GetValue(hitTypeIndex.enumValueIndex);
        var totalLines = property.isExpanded ? (hitTypeValue == HitType.Knockback ? 6 : 4) : 1;
        return EditorGUIUtility.singleLineHeight * totalLines +
               EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
    }
}