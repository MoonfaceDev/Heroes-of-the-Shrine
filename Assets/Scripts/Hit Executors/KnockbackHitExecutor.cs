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

    public void Execute(BaseAttack attack, IHittable hittable)
    {
        var processedKnockbackPower =
            attack.AttackManager.knockbackPowerTranspiler.Transpile(attack, hittable, knockbackPower);

        var hitDirection =
            (int)Mathf.Sign(hittable.Character.movableEntity.WorldPosition.x - attack.MovableEntity.WorldPosition.x);

        hittable.Knockback(
            processedKnockbackPower,
            KnockbackBehaviour.GetRelativeDirection(knockbackDirection, hitDirection),
            stunTime
        );
    }
}