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

    public void Hit(float damage)
    {
        if (!CanGetHit())
        {
            return;
        }
        healthSystem.health -= damage;
        onHit?.Invoke(damage);
    }

    public void Knockback(float damage, float power, float angleDegrees)
    {
        if (!CanGetHit())
        {
            return;
        }
        Hit(damage);
        onKnockback?.Invoke(damage, power, angleDegrees);
        if (!knockbackBehaviour)
        {
            return;
        }
        knockbackBehaviour.Knockback(power, angleDegrees);
    }

    public void Stun(float damage, float time)
    {
        if (!CanGetHit())
        {
            return;
        }
        if (movableObject.position.y > 0)
        {
            Knockback(damage, STUN_LAUNCH_POWER, STUN_LAUNCH_ANGEL);
            return;
        }
        Hit(damage);
        onStun?.Invoke(damage, time);
        if (!stunBehaviour)
        {
            return;
        }
        stunBehaviour.Stun(time);
    }
}
