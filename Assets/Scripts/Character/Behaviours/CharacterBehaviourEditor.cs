/*
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(CharacterBehaviour), true)]
public class CharacterBehaviourEditor : Editor
{
    SerializedProperty conditions;
    ReorderableList list;

    void OnEnable()
    {
        conditions = serializedObject.FindProperty("conditions");
        list = new ReorderableList(serializedObject, conditions, true, true, true, true)
        {
            drawElementCallback = DrawListItem,
            drawHeaderCallback = DrawHeader
        };
    }

    void DrawListItem(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, 160, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("behaviour"),
            GUIContent.none
        );

        CharacterBehaviour behaviour = (CharacterBehaviour) element.FindPropertyRelative("behaviour").objectReferenceValue;
        if (behaviour)
        {
            string[] properties = behaviour.GetType().GetProperties().Where(property => property.PropertyType == typeof(bool)).Select(property => property.Name).ToArray();
            int propertyIndex = Array.IndexOf(properties, (target as CharacterBehaviour).conditions[index].propertyName);

            if (propertyIndex < 0)
            {
                propertyIndex = 0;
            }

            propertyIndex = EditorGUI.Popup(new Rect(rect.x + 180, rect.y, 160, EditorGUIUtility.singleLineHeight), propertyIndex, properties);
            (target as CharacterBehaviour).conditions[index].propertyName = properties[propertyIndex];
        }
    }

    void DrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "Behaviour Conditions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "conditions");

        list.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
*/