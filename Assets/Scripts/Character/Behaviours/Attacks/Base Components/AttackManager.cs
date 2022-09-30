using System;
using UnityEngine;

public class AttackManager : CharacterBehaviour
{
    public float maxComboDelay;
    [HideInInspector] public string lastAttack;
    [HideInInspector] public float lastAttackTime;

    public override void Awake()
    {
        base.Awake();
        BaseAttack[] attackComponents = GetComponents<BaseAttack>();
        foreach (BaseAttack attack in attackComponents)
        {
            attack.onFinish += () =>
            {
                lastAttack = attack.attackName;
                lastAttackTime = Time.time;
            };
        }
        eventManager.Attach(() => true, () =>
        {
            if (lastAttack != "" && Time.time - lastAttackTime > maxComboDelay)
            {
                lastAttack = "";
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
