using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class BehaviourInjector
{
    private const BindingFlags InjectableBindingFlags =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    public static void Inject(EntityBehaviour behaviour)
    {
        var fields = GetTypeHierarchy(behaviour).SelectMany(type => type.GetFields(InjectableBindingFlags));
        foreach (var field in fields.Where(IsInjectable))
        {
            field.SetValue(behaviour, behaviour.GetBehaviour(field.FieldType));
        }
    }

    private static IEnumerable<Type> GetTypeHierarchy(EntityBehaviour behaviour)
    {
        var types = new HashSet<Type>();

        var type = behaviour.GetType();
        while (type != null)
        {
            types.Add(type);
            type = type.BaseType;
        }

        return types;
    }

    private static bool IsInjectable(MemberInfo member)
    {
        return member.GetCustomAttribute<InjectBehaviourAttribute>(false) != null;
    }
}