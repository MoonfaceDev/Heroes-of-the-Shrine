using UnityEditor;

[CustomEditor(typeof(AttackPattern))]
public class AttackPatternEditor : Editor
{
    SerializedProperty escapeAfterAttack;
    SerializedProperty escapeSpeedMultiplier;
    SerializedProperty escapeTime;

    void OnEnable()
    {
        escapeAfterAttack = serializedObject.FindProperty("escapeAfterAttack");
        escapeSpeedMultiplier = serializedObject.FindProperty("escapeSpeedMultiplier");
        escapeTime = serializedObject.FindProperty("escapeTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "escapeAfterAttack", "escapeSpeedMultiplier", "escapeTime");
        AttackPattern pattern = (AttackPattern)target;

        EditorGUILayout.PropertyField(escapeAfterAttack);
        pattern.escapeAfterAttack = escapeAfterAttack.boolValue;
        if (pattern.escapeAfterAttack)
        {
            EditorGUILayout.PropertyField(escapeSpeedMultiplier);
            pattern.escapeSpeedMultiplier = escapeSpeedMultiplier.floatValue;
            EditorGUILayout.PropertyField(escapeTime);
            pattern.escapeTime = escapeTime.floatValue;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
