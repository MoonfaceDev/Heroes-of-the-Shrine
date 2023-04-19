using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Particles))]
public class ParticlesEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var particles = (Particles)target;
        if (!particles.attachToCharacter) return;
        GUI.enabled = false;
        EditorGUILayout.TextArea("If attachToCharacter is true, the prefab needs to be a GameEntity!");
        GUI.enabled = true;
    }
}