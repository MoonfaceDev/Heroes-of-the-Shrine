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
            if (lastAttack != null && Time.time - lastAttackTime > maxComboDelay)
            {
                lastAttack = null;
            }
        }, single: false);
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
