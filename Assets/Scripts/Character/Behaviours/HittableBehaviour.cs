using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class HittableBehaviour : CharacterBehaviour
{
    public static float STUN_LAUNCH_POWER = 1;
    public static float STUN_LAUNCH_ANGEL = 90; // degrees

    public List<Hitbox> hitboxes;

    public delegate void OnHit(float damage);
    public delegate void OnKnockback(float damage, float power, float angleDegrees);
    public delegate void OnStun(float damage, float time);

    public event OnHit onHit;
    public event OnKnockback onKnockback;
    public event OnStun onStun;

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
        return !(knockbackBehaviour && knockbackBehaviour.recovering);
    }

    public bool Hit(float damage)
    {
        if (!CanGetHit())
        {
            return false;
        }
        healthSystem.health -= damage;
        onHit?.Invoke(damage);
        return true;
    }

    public bool Knockback(float damage, float power, float angleDegrees)
    {
        if (!CanGetHit())
        {
            return false;
        }
        Hit(damage);
        onKnockback?.Invoke(damage, power, angleDegrees);
        if (knockbackBehaviour)
        {
            knockbackBehaviour.Knockback(power, angleDegrees);
        }
        return true;
    }

    public bool Stun(float damage, float time)
    {
        if (!CanGetHit())
        {
            return false;
        }
        if (movableObject.position.y > 0)
        {
            Knockback(damage, STUN_LAUNCH_POWER, STUN_LAUNCH_ANGEL);
            return true;
        }
        Hit(damage);
        onStun?.Invoke(damage, time);
        if (stunBehaviour)
        {
            stunBehaviour.Stun(time);
        }
        return true;
    }

    public override void Stop()
    {
        throw new NotImplementedException();
    }
}
