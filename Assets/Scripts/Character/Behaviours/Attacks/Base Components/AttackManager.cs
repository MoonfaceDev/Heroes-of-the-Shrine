using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : CharacterBehaviour
{
    public float maxComboDelay;
    public List<string> hittableTags;
    [HideInInspector] public BaseAttack lastAttack;
    [HideInInspector] public float lastAttackTime;

    public override void Awake()
    {
        base.Awake();
        BaseAttack[] attackComponents = GetComponents<BaseAttack>();
        foreach (BaseAttack attack in attackComponents)
        {
            attack.onFinish += () =>
            {
                lastAttack = attack;
                lastAttackTime = Time.time;
            };
        }
        eventManager.Attach(() => true, () =>
        {
            if (lastAttack != null && Time.time - lastAttackTime > maxComboDelay)
            {
                lastAttack = null;
            }
        }, single: false);
    }

    private bool AnyAttack(Func<BaseAttack, bool> callback)
    {
        BaseAttack[] attackComponents = GetComponents<BaseAttack>();
        foreach (BaseAttack attack in attackComponents)
        {
            if (callback(attack))
            {
                return true;
            }
        }
        return false;
    }

    public bool anticipating
    {
        get => AnyAttack((attack) => attack.anticipating);
    }

    public bool active
    {
        get => AnyAttack((attack) => attack.active);
    }

    public bool recovering
    {
        get => AnyAttack((attack) => attack.recovering);
    }

    public bool attacking
    {
        get => AnyAttack((attack) => attack.attacking);
    }

    public bool CanWalk()
    {
        return !AnyAttack((attack) => attack.attacking && !attack.CanWalk());
    }

    public bool IsUninterruptable()
    {
        return AnyAttack((attack) => attack.active && !attack.interruptable);
    }

    public void Stop()
    {
        BaseAttack[] attackComponents = GetComponents<BaseAttack>();
        foreach (BaseAttack attack in attackComponents)
        {
            if (attack.attacking)
            {
                attack.Stop();
            }
        }
    }
}
