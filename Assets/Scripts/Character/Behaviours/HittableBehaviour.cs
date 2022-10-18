using System;
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
    [Header("Death")]
    public float deathAnimationDuration;

    public event OnHit OnHit;
    public event OnKnockback OnKnockback;
    public event OnStun OnStun;
    public event Action OnDie;

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
        return !(knockbackBehaviour && knockbackBehaviour.Recovering);
    }


    public void Kill()
    {
        Type[] behavioursToStop = { typeof(BaseMovementBehaviour), typeof(AttackManager), typeof(StunBehaviour) };
        DisableBehaviours(behavioursToStop);
        StopBehaviours(behavioursToStop);
        OnDie?.Invoke();
        EventManager.Attach(() => !IsPlaying(typeof(KnockbackBehaviour)), () =>
        {
            Animator.SetBool("dead", true);
            DisableBehaviours(typeof(KnockbackBehaviour), typeof(BaseEffect));
            StopBehaviours(typeof(BaseEffect));
            Destroy(gameObject, deathAnimationDuration);
        });
    }

    public bool Hit(float damage)
    {
        if (!CanGetHit())
        {
            return false;
        }
        healthSystem.health -= damage;
        OnHit?.Invoke(damage);
        if (healthSystem.health <= 0)
        {
            Kill();
        }
        return true;
    }

    public bool Knockback(float damage, float power, float angleDegrees)
    {
        if (!CanGetHit())
        {
            return false;
        }
        Hit(damage);
        OnKnockback?.Invoke(damage, power, angleDegrees);
        if (knockbackBehaviour)
        {
            knockbackBehaviour.Play(power, angleDegrees);
        }
        return true;
    }

    public bool Stun(float damage, float time)
    {
        if (!CanGetHit())
        {
            return false;
        }
        if (MovableObject.position.y > 0)
        {
            Knockback(damage, STUN_LAUNCH_POWER, STUN_LAUNCH_ANGEL);
            return true;
        }
        Hit(damage);
        OnStun?.Invoke(damage, time);
        if (stunBehaviour)
        {
            stunBehaviour.Play(time);
        }
        return true;
    }
}
