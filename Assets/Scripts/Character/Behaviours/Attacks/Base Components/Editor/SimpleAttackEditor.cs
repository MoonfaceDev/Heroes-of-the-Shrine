using System.Reflection;
using UnityEditor;

[CustomEditor(typeof(SimpleAttack), true)]
public class SimpleAttackEditor : Editor
{
    private SerializedProperty previousAttacks;
    private SerializedProperty attackFlow;
    private SerializedProperty hitDetector;
    private SerializedProperty hitDefinition;

    private const BindingFlags OverridableFlags =
        BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    private void OnEnable()
    {
        previousAttacks = serializedObject.FindProperty("previousAttacks");
        attackFlow = serializedObject.FindProperty("attackFlow");
        hitDetector = serializedObject.FindProperty("hitDetector");
        hitDefinition = serializedObject.FindProperty("hitDefinition");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "previousAttacks", "attackFlow", "hitDetector", "hitDefinition");
        var attack = (SimpleAttack)target;

        if (!HasMethod(attack, "ComboCondition"))
        {
            EditorGUILayout.PropertyField(previousAttacks);
        }

        if (!HasProperty(attack, "AttackFlow"))
        {
            EditorGUILayout.PropertyField(attackFlow);
        }

        if (!HasMethod(attack, "ConfigureHitDetector"))
        {
            EditorGUILayout.PropertyField(hitDetector);
        }

        if (!HasMethod(attack, "HitCallable"))
        {
            EditorGUILayout.PropertyField(hitDefinition);
        }

        serializedObject.ApplyModifiedProperties();
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