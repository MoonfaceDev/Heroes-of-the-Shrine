using ExtEvents;
using UnityEngine;

/// <summary>
/// Hittable attached to a hitbox, related to a <see cref="hittableBehaviour"/> of a character
/// </summary>
[RequireComponent(typeof(Hitbox))]
public class HittableHitbox : EntityBehaviour, IHittable
{
    /// <value>
    /// <see cref="hittableBehaviour"/> of the related character
    /// </value>
    public HittableBehaviour hittableBehaviour;
    
    /// <value>
    /// Invoked when <see cref="Hit"/> is called
    /// </value>
    [SerializeField] public ExtEvent onHit;

    /// <value>
    /// <see cref="SpriteRenderer"/> on which blink effect is played
    /// </value>
    [Header("Blink Effect")] public SpriteRenderer figure;
    
    /// <value>
    /// Material that <see cref="figure"/> changes to during blink effect
    /// </value>
    public Material blinkMaterial;
    
    /// <value>
    /// Duration of blink effect
    /// </value>
    public float blinkTime;

    /// <value>
    /// Related hitbox
    /// </value>
    public Hitbox Hitbox { get; private set; }

    private Material defaultMaterial;

    protected virtual void Awake()
    {
        Hitbox = GetBehaviour<Hitbox>();
        defaultMaterial = figure.material;
        onHit += () =>
        {
            if (figure && hittableBehaviour.CanGetHit())
            {
                Blink();
            }
        };
    }

    public Character Character => hittableBehaviour.Character;

    /// <value>
    /// Plays blink effect on the character's figure, to emphasize hits
    /// </value>
    protected void Blink()
    {
        if (!figure || !hittableBehaviour.CanGetHit()) return;

        figure.material = blinkMaterial;
        StartTimeout(() => figure.material = defaultMaterial, blinkTime);
    }

    public bool CanGetHit()
    {
        return hittableBehaviour.CanGetHit();
    }

    public virtual void Hit(float damage)
    {
        onHit.Invoke();
        hittableBehaviour.Hit(damage);
    }

    public virtual void Knockback(float power, float angleDegrees, float stunTime)
    {
        hittableBehaviour.Knockback(power, angleDegrees, stunTime);
    }

    public virtual void Stun(float time)
    {
        hittableBehaviour.Stun(time);
    }
}