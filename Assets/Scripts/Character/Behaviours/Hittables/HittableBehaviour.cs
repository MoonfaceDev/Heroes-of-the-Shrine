using System;
using UnityEngine;

public delegate void HitCallback(float damage);
public delegate void KnockbackCallback(float damage, float power, float angleDegrees);
public delegate void StunCallback(float damage, float time);

[RequireComponent(typeof(HealthSystem))]
public class HittableBehaviour : CharacterBehaviour, IHittable
{
    [ShowDebug] public float damageMultiplier;

    private const float StunLaunchPower = 1;
    private const float StunLaunchAngel = 90; // degrees
    public event HitCallback OnHit;
    public event KnockbackCallback OnKnockback;
    public event StunCallback OnStun;

    private HealthSystem healthSystem;
    private KnockbackBehaviour knockbackBehaviour;
    private StunBehaviour stunBehaviour;

    private EventListener blinkEvent;

    public override void Awake()
    {
        base.Awake();
        healthSystem = GetComponent<HealthSystem>();
        knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        stunBehaviour = GetComponent<StunBehaviour>();
        damageMultiplier = 1;
    }

    public bool CanGetHit()
    {
        return healthSystem.Alive && !(knockbackBehaviour && knockbackBehaviour.Recovering);
    }

    private float ProcessDamage(float damage)
    {
        return damage * damageMultiplier;
    }
    
    public void Hit(float damage)
    {
        if (!CanGetHit())
        {
            return;
        }
        var processedDamage = ProcessDamage(damage);
        healthSystem.health = Math.Max(healthSystem.health - processedDamage, 0);
        OnHit?.Invoke(processedDamage);
    }

    private void DoKnockback(float damage, float power, float angleDegrees)
    {
        OnKnockback?.Invoke(damage, power, angleDegrees);
        if (knockbackBehaviour)
        {
            knockbackBehaviour.Play(power, angleDegrees);
        }
    }

    public void Knockback(float damage, float power, float angleDegrees, float stunTime)
    {
        if (!CanGetHit())
        {
            return;
        }
        Hit(damage);
        DoKnockback(damage, power, angleDegrees);
    }

    private void DoStun(float damage, float time)
    {
        OnStun?.Invoke(damage, time);
        if (stunBehaviour)
        {
            stunBehaviour.Play(time);
        }
    }

    public void Stun(float damage, float time)
    {
        if (!CanGetHit())
        {
            return;
        }
        if (MovableObject.WorldPosition.y > 0)
        {
            Knockback(damage, StunLaunchPower, StunLaunchAngel, time);
            return;
        }
        Hit(damage);
        DoStun(damage, time);
    }

    private void OnDestroy()
    {
        EventManager.Instance.Detach(blinkEvent);
    }
}