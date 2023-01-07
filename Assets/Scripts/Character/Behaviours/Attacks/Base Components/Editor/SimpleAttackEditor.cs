using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleAttack), true)]
public class SimpleAttackEditor : Editor
{
    private SerializedProperty previousAttacks;
    private SerializedProperty anticipateDuration;
    private SerializedProperty activeDuration;
    private SerializedProperty recoveryDuration;
    private SerializedProperty hitDetector;
    private SerializedProperty hitDefinition;

    private const BindingFlags OverridableFlags =
        BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    private void OnEnable()
    {
        previousAttacks = serializedObject.FindProperty("previousAttacks");
        anticipateDuration = serializedObject.FindProperty("anticipateDuration");
        activeDuration = serializedObject.FindProperty("activeDuration");
        recoveryDuration = serializedObject.FindProperty("recoveryDuration");
        hitDetector = serializedObject.FindProperty("hitDetector");
        hitDefinition = serializedObject.FindProperty("hitDefinition");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "previousAttacks", "anticipateDuration", "activeDuration",
            "recoveryDuration", "hitDetector", "hitDefinition");
        var attack = (SimpleAttack)target;

        if (!HasMethod(attack, "ComboCondition"))
        {
            EditorGUILayout.PropertyField(previousAttacks);
            List<BaseAttack> previousAttacksValue = new();
            for (var i = 0; i < previousAttacks.arraySize; i++)
            {
                previousAttacksValue.Add((BaseAttack)previousAttacks.GetArrayElementAtIndex(i).objectReferenceValue);
            }

            attack.previousAttacks = previousAttacksValue;
        }

        if (!HasMethod(attack, "AnticipateCoroutine"))
        {
            EditorGUILayout.PropertyField(anticipateDuration);
            attack.anticipateDuration = anticipateDuration.floatValue;
        }

        if (!HasMethod(attack, "ActiveCoroutine"))
        {
            EditorGUILayout.PropertyField(activeDuration);
            attack.activeDuration = activeDuration.floatValue;
        }

        if (!HasMethod(attack, "RecoveryCoroutine"))
        {
            EditorGUILayout.PropertyField(recoveryDuration);
            attack.recoveryDuration = recoveryDuration.floatValue;
        }

        if (!HasMethod(attack, "ConfigureHitDetector"))
        {
            EditorGUILayout.PropertyField(hitDetector);
            attack.hitDetector = (BaseHitDetector)hitDetector.objectReferenceValue;
        }

        if (!HasMethod(attack, "HitCallable"))
        {
            EditorGUILayout.PropertyField(hitDefinition);
            attack.hitDefinition = (HitDefinition) hitDefinition.boxedValue;
        }

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(attack);
        }
    }

    private static bool HasMethod(SimpleAttack attack, string name)
    {
        var type = attack.GetType();
        while (type != typeof(SimpleAttack) && type != null)
        {
            var method = type.GetMethod(name, OverridableFlags);
            if (method != null)
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }
}