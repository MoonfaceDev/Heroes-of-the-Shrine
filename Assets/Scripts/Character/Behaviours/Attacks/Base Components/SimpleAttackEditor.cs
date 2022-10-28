using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleAttack), true)]
public class SimpleAttackEditor : Editor
{
    SerializedProperty previousAttacks;
    SerializedProperty anticipateDuration;
    SerializedProperty activeDuration;
    SerializedProperty recoveryDuration;
    SerializedProperty hitbox;
    SerializedProperty overrideDefaultHittableTags;
    SerializedProperty hittableTags;
    SerializedProperty damage;
    SerializedProperty hitTypeIndex;
    SerializedProperty knockbackPower;
    SerializedProperty knockbackDirection;
    SerializedProperty stunTime;

    private readonly BindingFlags BINDING_FLAGS = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    void OnEnable()
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
        SimpleAttack attack = (SimpleAttack)target;

        if (!HasMethod(attack, "ComboCondition"))
        {
            EditorGUILayout.PropertyField(previousAttacks);
            List<BaseAttack> previousAttacksValue = new();
            for (int i = 0; i < previousAttacks.arraySize; i++)
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
                for (int i = 0; i < hittableTags.arraySize; i++)
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
            HitType hitTypeValue = (HitType)typeof(HitType).GetEnumValues().GetValue(hitTypeIndex.enumValueIndex);
            attack.hitType = hitTypeValue;
            if (hitTypeValue == HitType.Knockback)
            {
                EditorGUILayout.PropertyField(knockbackPower);
                EditorGUILayout.PropertyField(knockbackDirection);
                attack.knockbackPower = knockbackPower.floatValue;
                attack.knockbackDirection = knockbackDirection.floatValue;
            }
            else if (hitTypeValue == HitType.Stun)
            {
                EditorGUILayout.PropertyField(stunTime);
                attack.stunTime = stunTime.floatValue;
            }
        }

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) { 
            EditorUtility.SetDirty(attack);
        }
    }

    private bool HasMethod(SimpleAttack attack, string name)
    {
        Type type = attack.GetType();
        while (type != typeof(SimpleAttack))
        {
            MethodBase method = type.GetMethod(name, BINDING_FLAGS);
            if (method != null)
            {
                return true;
            }
            type = type.BaseType;
        }
        return false;
    }

    private bool HasProperty(SimpleAttack attack, string name)
    {
        Type type = attack.GetType();
        while (type != typeof(SimpleAttack))
        {
            PropertyInfo property = type.GetProperty(name, BINDING_FLAGS);
            if (property != null)
            {
                return true;
            }
            type = type.BaseType;
        }
        return false;
    }
}
