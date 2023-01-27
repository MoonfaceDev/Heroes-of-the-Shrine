using System;
using SolidUtilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(Tags))]
public class TagsDrawer : PropertyDrawer
{
    private string name;
    private Tag[] tagOptions;
    private SerializedProperty tags;
    private ReorderableList list;

    private void Init(SerializedProperty property)
    {
        name = property.displayName;
        tagOptions = (Tag[])Enum.GetValues(typeof(Tag));
        tags = property.FindPropertyRelative("tags");
        list = new ReorderableList(property.serializedObject, tags, true, true, true, true);
        list.drawHeaderCallback += DrawHeader;
        list.drawElementCallback += DrawElement;
        list.onAddDropdownCallback += OnAddDropdown;
    }

    private void DrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, new GUIContent(name), EditorStyles.boldLabel);
    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        EditorGUI.LabelField(rect, Enum.GetNames(typeof(Tag))[element.enumValueIndex]);
    }

    private void OnAddDropdown(Rect buttonRect, ReorderableList list)
    {
        var menu = new GenericMenu();

        for (var i = 0; i < tagOptions.Length; i++)
        {
            var label = new GUIContent(tagOptions[i].ToString());

            // Don't allow duplicate tags to be added.
            if (PropertyContainsTag(tags, i))
            {
                menu.AddDisabledItem(label);
            }
            else
            {
                menu.AddItem(label, false, OnAddClickHandler, tagOptions[i].ToString());
            }
        }

        menu.ShowAsContext();
    }

    private static bool PropertyContainsTag(SerializedProperty property, int index)
    {
        if (!property.isArray)
        {
            return index == property.enumValueIndex;
        }

        for (var i = 0; i < property.arraySize; i++)
        {
            if (property.GetArrayElementAtIndex(i).enumValueIndex == index)
            {
                return true;
            }
        }

        return false;
    }

    private void OnAddClickHandler(object tag)
    {
        var index = list.serializedProperty.arraySize;
        list.serializedProperty.arraySize++;
        list.index = index;

        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        element.enumValueIndex = Enum.GetNames(typeof(Tag)).IndexOf(tag);
        tags.serializedObject.ApplyModifiedProperties();
    }

    private ReorderableList GetList(SerializedProperty property)
    {
        if (list == null)
        {
            Init(property);
        }

        return list;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GetList(property).DoList(position);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return GetList(property).GetHeight();
    }
}