using System;
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
    private SerializedProperty hitbox;
    private SerializedProperty overrideDefaultHittableTags;
    private SerializedProperty hittableTags;
    private SerializedProperty damage;
    private SerializedProperty hitTypeIndex;
    private SerializedProperty knockbackPower;
    private SerializedProperty knockbackDirection;
    private SerializedProperty stunTime;

    private const BindingFlags OverridableFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    private void OnEnable()
    {
        previousAttacks = serializedObject.FindProperty("previousAttacks");
        anticipateDuration = serializedObject.FindProperty("anticipateDuration");
        activeDuration = serializedObject.FindProperty("activeDuration");
        recoveryDuration = serializedObject.FindProperty("recoveryDuration");
        hitbox = serializedObject.FindProperty("hitbox");
        overrideDefaultHittableTags = serializedObject.FindProperty("overrideDefaultHittableTags");
        hittableTags = serializedObject.FindProperty("hittableTags");
        damage = serializedObject.FindProperty("damage");
        hitTypeIndex = serializedObject.FindProperty("hitType");
        knockbackPower = serializedObject.FindProperty("knockbackPower");
        knockbackDirection = serializedObject.FindProperty("knockbackDirection");
        stunTime = serializedObject.FindProperty("stunTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "previousAttacks", "anticipateDuration", "activeDuration", "recoveryDuration", "hitbox", "overrideDefaultHittableTags", "hittableTags", "damage", "hitType", "knockbackPower", "knockbackDirection", "stunTime");
        var attack = (SimpleAttack)target;

        if (!HasMethod(attack, "ComboCondition"))
        {
            EditorGUILayout.PropertyField(previousAttacks);
            List<BaseAttack> previousAttacksValue = new();
            for (var i = 0; i < previousAttacks.arraySize; i++)
            {
                previousAttacksValue.Add((BaseAttack) previousAttacks.GetArrayElementAtIndex(i).objectReferenceValue);
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

        if (!HasMethod(attack, "CreateHitDetector"))
        {
            EditorGUILayout.PropertyField(hitbox);
            attack.hitbox = (Hitbox) hitbox.objectReferenceValue;
        }

        if (!HasProperty(attack, "HittableTags"))
        {
            EditorGUILayout.PropertyField(overrideDefaultHittableTags);
            attack.overrideDefaultHittableTags = overrideDefaultHittableTags.boolValue;
            if (attack.overrideDefaultHittableTags)
            {
                EditorGUILayout.PropertyField(hittableTags);
                List<string> hittableTagsValue = new();
                for (var i = 0; i < hittableTags.arraySize; i++)
                {
                    hittableTagsValue.Add(hittableTags.GetArrayElementAtIndex(i).stringValue);
                }
                attack.hittableTags = hittableTagsValue;
            }
        }

        if (!HasMethod(attack, "CalculateDamage"))
        {
            EditorGUILayout.PropertyField(damage);
            attack.damage = damage.floatValue;
        }

        if (!HasMethod(attack, "HitCallable"))
        {
            EditorGUILayout.PropertyField(hitTypeIndex);
            var hitTypeValue = (HitType)typeof(HitType).GetEnumValues().GetValue(hitTypeIndex.enumValueIndex);
            attack.hitType = hitTypeValue;
            switch (hitTypeValue)
            {
                case HitType.Knockback:
                    EditorGUILayout.PropertyField(knockbackPower);
                    EditorGUILayout.PropertyField(knockbackDirection);
                    attack.knockbackPower = knockbackPower.floatValue;
                    attack.knockbackDirection = knockbackDirection.floatValue;
                    break;
                case HitType.Stun:
                    EditorGUILayout.PropertyField(stunTime);
                    attack.stunTime = stunTime.floatValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) { 
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

    private static bool HasProperty(SimpleAttack attack, string name)
    {
        var type = attack.GetType();
        while (type != typeof(SimpleAttack) && type != null)
        {
            var property = type.GetProperty(name, OverridableFlags);
            if (property != null)
            {
                return true;
            }
            type = type.BaseType;
        }
        return false;
    }
}
