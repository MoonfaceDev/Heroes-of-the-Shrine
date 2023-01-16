using System;
using UnityEngine;

/// <summary>
/// Simple implementation of <see cref="IHitExecutor"/> that deals damage and applies stun or knockback on an hittable
/// </summary>
[Serializable]
public class SimpleHitExecutor : IHitExecutor
{
    public HitDefinition hitDefinition;

    public void Execute(BaseAttack attack, IHittable hittable)
    {
        var processedDamage =
            attack.AttackManager.DamageTranspiler.TranspileDamage(attack, hittable, hitDefinition.damage);

        switch (hitDefinition.hitType)
        {
            case HitType.Knockback:
                var hitDirection =
                    (int)Mathf.Sign(hittable.Character.movableEntity.WorldPosition.x -
                                    attack.MovableEntity.WorldPosition.x);
                hittable.Knockback(processedDamage, hitDefinition.knockbackPower,
                    KnockbackBehaviour.GetRelativeDirection(hitDefinition.knockbackDirection, hitDirection),
                    hitDefinition.stunTime);
                break;
            case HitType.Stun:
                hittable.Stun(processedDamage, hitDefinition.stunTime);
                break;
        }
    }
}