using UnityEditor;
using UnityEngine;

public class ReadonlyAttribute : PropertyAttribute
{
    public readonly bool playMode;

    public ReadonlyAttribute(bool playMode = false)
    {
        this.playMode = playMode;
    }
}

public class ShowDebugAttribute : ReadonlyAttribute
{
    public ShowDebugAttribute() : base(true)
    {
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadonlyAttribute), true)]
public class ReadonlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return ShouldDraw() ? EditorGUI.GetPropertyHeight(property, label, true) : 0;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!ShouldDraw()) return;
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }

    private bool ShouldDraw()
    {
        return !((ReadonlyAttribute)attribute).playMode || Application.isPlaying;
    }
}
#endif