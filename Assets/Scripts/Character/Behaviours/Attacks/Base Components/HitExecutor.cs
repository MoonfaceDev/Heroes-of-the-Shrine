using UnityEngine;

public abstract class HitExecutor<T> where T : HitDefinition
{
    protected abstract T HitDefinition { get; }

    public virtual void Execute(BaseAttack attack, IHittable hittable)
    {
        var processedDamage = attack.AttackManager.TranspileDamage(attack, hittable, HitDefinition.damage);
        switch (HitDefinition.hitType)
        {
            case HitType.Knockback:
                var hitDirection =
                    (int)Mathf.Sign(hittable.Character.movableObject.WorldPosition.x -
                                    attack.MovableObject.WorldPosition.x);
                hittable.Knockback(processedDamage, HitDefinition.knockbackPower,
                    KnockbackBehaviour.GetRelativeDirection(HitDefinition.knockbackDirection, hitDirection),
                    HitDefinition.stunTime);
                break;
            case HitType.Stun:
                hittable.Stun(processedDamage, HitDefinition.stunTime);
                break;
        }
    }
}