using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public class HittableHitbox : MonoBehaviour, IHittable
{
    public HittableBehaviour hittableBehaviour;

    public Hitbox Hitbox { get; private set; }

    private void Awake()
    {
        Hitbox = GetComponent<Hitbox>();
    }

    public Character Character => hittableBehaviour.Character;

    public void Hit(float damage)
    {
        hittableBehaviour.Hit(damage);
    }

    public void Knockback(float damage, float power, float angleDegrees)
    {
        hittableBehaviour.Knockback(damage, power, angleDegrees);
    }

    public void Stun(float damage, float time)
    {
        hittableBehaviour.Stun(damage, time);
    }
}