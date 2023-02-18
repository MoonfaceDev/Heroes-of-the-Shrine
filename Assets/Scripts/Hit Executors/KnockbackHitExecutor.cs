using System;
using UnityEngine;

/// <summary>
/// Hit executor that applies knockback on an hittable
/// </summary>
[Serializable]
public class KnockbackHitExecutor : IHitExecutor
{
    /// <value>
    /// Power of the knockback, affects its initial speed
    /// </value>
    public float knockbackPower;

    /// <value>
    /// Direction of the knockback in degrees, relative to X axis, in the direction of the hit
    /// </value>
    public float knockbackDirection;

    /// <value>
    /// If enemy is resistant to knockback, this value will be used.
    /// </value>
    public float stunTime = 0.5f;

    public void Execute(Hit hit)
    {
        var processedKnockbackPower =
            hit.source.AttackManager.knockbackPowerTranspiler.Transpile(hit.source, hit.victim, knockbackPower);

        var hitDirection =
            (int)Mathf.Sign(hit.victim.Character.Entity.WorldPosition.x - hit.source.Entity.WorldPosition.x);

        var knockbackBehaviour = hit.victim.Character.GetBehaviour<KnockbackBehaviour>();

        if (knockbackBehaviour)
        {
            knockbackBehaviour.Play(new KnockbackBehaviour.Command
            {
                power = processedKnockbackPower,
                angleDegrees = KnockbackBehaviour.GetRelativeDirection(knockbackDirection, hitDirection)
            });
        }
        else
        {
            hit.victim.Character.GetBehaviour<StunBehaviour>().Play(new StunBehaviour.Command { time = stunTime });
        }
    }
}