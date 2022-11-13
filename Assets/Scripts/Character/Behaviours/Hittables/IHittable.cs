public interface IHittable
{
    public Character Character { get; }

    public void Hit(float damage);

    public void Knockback(float damage, float power, float angleDegrees);

    public void Stun(float damage, float time);
}