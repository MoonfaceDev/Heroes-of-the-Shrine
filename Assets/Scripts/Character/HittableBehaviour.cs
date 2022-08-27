using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class HittableBehaviour : CharacterBehaviour
{
    public static float STUN_FALL_POWER = 1;
    public static float STUN_FALL_ANGEL = 270; // degrees

    public delegate void OnHit(float damage);
    public delegate void OnKnockback(float damage, float power, float angleDegrees);
    public delegate void OnStun(float damage, float time);

    public OnHit onHit;
    public OnKnockback onKnockback;
    public OnStun onStun;

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

    public void Hit(float damage)
    {
        healthSystem.health -= damage;
        onHit(damage);
    }

    public bool Knockback(float damage, float power, float angleDegrees)
    {
        Hit(damage);
        onKnockback(damage, power, angleDegrees);
        if (!knockbackBehaviour)
        {
            return false;
        }
        knockbackBehaviour.Knockback(power, angleDegrees);
        return true;
    }

    public bool Stun(float damage, float time)
    {
        if (movableObject.position.y > 0)
        {
            return Knockback(damage, STUN_FALL_POWER, STUN_FALL_ANGEL);
        }
        Hit(damage);
        onStun(damage, time);
        if (!stunBehaviour)
        {
            return false;
        }
        stunBehaviour.Stun(time);
        return true;
    }
}
