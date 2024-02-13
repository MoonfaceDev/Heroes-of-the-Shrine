using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Transform))]
public class TransformEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        return ((Transform)target).GetComponent<GameEntity>() == null ? base.CreateInspectorGUI() : new VisualElement();
    }
}