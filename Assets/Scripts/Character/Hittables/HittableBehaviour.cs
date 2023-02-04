using System;

/// <summary>
/// Behaviours responsible for processing hits from attacks
/// </summary>
public class HittableBehaviour : CharacterBehaviour, IHittable
{
    /// <value>
    /// Multiplier for any damage that character is getting
    /// </value>
    [ShowDebug] public float damageMultiplier;

    private const float StunLaunchPower = 1;
    private const float StunLaunchAngel = 90; // degrees

    /// <value>
    /// Invoked when <see cref="Hit"/> is called
    /// </value>
    public event Action<float> OnHit;

    private HealthSystem healthSystem;
    private KnockbackBehaviour knockbackBehaviour;
    private StunBehaviour stunBehaviour;
    private ForcedWalkBehaviour forcedWalkBehaviour;

    protected override void Awake()
    {
        base.Awake();
        healthSystem = GetBehaviour<HealthSystem>();
        knockbackBehaviour = GetBehaviour<KnockbackBehaviour>();
        stunBehaviour = GetBehaviour<StunBehaviour>();
        damageMultiplier = 1;
    }

    public bool CanGetHit()
    {
        return healthSystem.Alive
               && !(knockbackBehaviour && knockbackBehaviour.Recovering)
               && !(forcedWalkBehaviour && forcedWalkBehaviour.Playing);
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

    public void Knockback(float power, float angleDegrees, float stunTime)
    {
        if (!CanGetHit())
        {
            return;
        }

        if (knockbackBehaviour)
        {
            knockbackBehaviour.Play(new KnockbackBehaviour.Command(power, angleDegrees));
        }
    }

    public void Stun(float time)
    {
        if (!CanGetHit())
        {
            return;
        }

        if (MovableEntity.WorldPosition.y > 0)
        {
            Knockback(StunLaunchPower, StunLaunchAngel, time);
            return;
        }

        if (stunBehaviour)
        {
            stunBehaviour.Play(new StunBehaviour.Command(time));
        }
    }
}