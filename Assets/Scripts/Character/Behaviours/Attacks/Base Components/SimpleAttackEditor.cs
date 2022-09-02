using UnityEditor;

[CustomEditor(typeof(SimpleAttack))]
public class SimpleAttackEditor : Editor
{
    SerializedProperty hitTypeIndex;
    SerializedProperty knockbackPower;
    SerializedProperty knockbackDirection;
    SerializedProperty stunTime;

    void OnEnable()
    {
        hitTypeIndex = serializedObject.FindProperty("hitType");
        knockbackPower = serializedObject.FindProperty("knockbackPower");
        knockbackDirection = serializedObject.FindProperty("knockbackDirection");
        stunTime = serializedObject.FindProperty("stunTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "hitType", "knockbackPower", "knockbackDirection", "stunTime");
        EditorGUILayout.PropertyField(hitTypeIndex);
        HitType hitTypeValue = (HitType) typeof(HitType).GetEnumValues().GetValue(hitTypeIndex.enumValueIndex);
        if (hitTypeValue == HitType.Knockback)
        {
            EditorGUILayout.PropertyField(knockbackPower);
            EditorGUILayout.PropertyField(knockbackDirection);
        } 
        else if (hitTypeValue == HitType.Stun)
        {
            EditorGUILayout.PropertyField(stunTime);
        }
        SimpleAttack attack = (SimpleAttack)target;
        attack.hitType = hitTypeValue;
        attack.knockbackPower = knockbackPower.floatValue;
        attack.knockbackDirection = knockbackDirection.floatValue;
        attack.stunTime = stunTime.floatValue;
        serializedObject.ApplyModifiedProperties();
    }
}
