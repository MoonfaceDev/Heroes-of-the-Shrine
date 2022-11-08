using System.Collections.Generic;
using UnityEngine;

public delegate void OnHit(float damage);
public delegate void OnKnockback(float damage, float power, float angleDegrees);
public delegate void OnStun(float damage, float time);

[RequireComponent(typeof(HealthSystem))]
public class HittableBehaviour : CharacterBehaviour
{
    public static float STUN_LAUNCH_POWER = 1;
    public static float STUN_LAUNCH_ANGEL = 90; // degrees

    public List<Hitbox> hitboxes;

    public event OnHit OnHit;
    public event OnKnockback OnKnockback;
    public event OnStun OnStun;

    private HealthSystem healthSystem;
    private KnockbackBehaviour knockbackBehaviour;
    private StunBehaviour stunBehaviour;

    public override void Awake()
    {
        base.Awake();
        healthSystem = GetComponent<HealthSystem>();
        knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        stunBehaviour = GetComponent<StunBehaviour>();
    }

    public bool CanGetHit()
    {
        return healthSystem.health > 0 && !(knockbackBehaviour && knockbackBehaviour.Recovering);
    }

    public virtual float ProcessDamage(float damage)
    {
        return damage;
    }

    public virtual bool Hit(float damage)
    {
        if (!CanGetHit())
        {
            return false;
        }
        float processedDamage = ProcessDamage(damage);
        healthSystem.health -= processedDamage;
        OnHit?.Invoke(processedDamage);
        return true;
    }

    protected virtual void DoKnockback(float damage, float power, float angleDegrees)
    {
        OnKnockback?.Invoke(damage, power, angleDegrees);
        if (knockbackBehaviour)
        {
            knockbackBehaviour.Play(power, angleDegrees);
        }
    }

    public virtual bool Knockback(float damage, float power, float angleDegrees)
    {
        if (!CanGetHit())
        {
            return false;
        }
        Hit(damage);
        DoKnockback(damage, power, angleDegrees);
        return true;
    }

    protected virtual void DoStun(float damage, float time)
    {
        OnStun?.Invoke(damage, time);
        if (stunBehaviour)
        {
            stunBehaviour.Play(time);
        }
    }

    public virtual bool Stun(float damage, float time)
    {
        if (!CanGetHit())
        {
            return false;
        }
        if (MovableObject.WorldPosition.y > 0)
        {
            Knockback(damage, STUN_LAUNCH_POWER, STUN_LAUNCH_ANGEL);
            return true;
        }
        Hit(damage);
        DoStun(damage, time);
        return true;
    }
}