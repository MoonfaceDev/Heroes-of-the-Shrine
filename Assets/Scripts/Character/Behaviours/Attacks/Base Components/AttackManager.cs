using System.Collections.Generic;
using UnityEngine;

public class AttackManager : CharacterBehaviour
{
    [HideInInspector] public List<string> attackHistory;

    public override void Awake()
    {
        base.Awake();
        BaseAttack[] attackComponents = GetComponents<BaseAttack>();
        foreach (BaseAttack attack in attackComponents)
        {
            attack.onStart += () =>
            {
                attackHistory.Add(attack.attackName);
            };
        }
    }

    public string GetLastAttack()
    {
        if (attackHistory.Count == 0)
        {
            return null;
        }
        return attackHistory[^1];
    }

    public bool anticipating
    {
        get
        {
            BaseAttack[] attackComponents = GetComponents<BaseAttack>();
            foreach (BaseAttack attack in attackComponents)
            {
                if (attack.anticipating)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public bool active
    {
        get
        {
            BaseAttack[] attackComponents = GetComponents<BaseAttack>();
            foreach (BaseAttack attack in attackComponents)
            {
                if (attack.active)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public bool recovering
    {
        get
        {
            BaseAttack[] attackComponents = GetComponents<BaseAttack>();
            foreach (BaseAttack attack in attackComponents)
            {
                if (attack.recovering)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public bool attacking
    {
        get => anticipating || active || recovering;
    }

    public bool IsUninterruptable()
    {
        BaseAttack[] attackComponents = GetComponents<BaseAttack>();
        foreach (BaseAttack attack in attackComponents)
        {
            if (attack.active && !attack.interruptable)
            {
                return true;
            }
        }
        return false;
    }
}
