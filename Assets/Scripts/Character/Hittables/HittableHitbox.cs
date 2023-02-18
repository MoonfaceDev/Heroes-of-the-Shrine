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

    public Character Character => hittableBehaviour.Character;

    public ExtEvent HitEvent => onHit;

    private Material defaultMaterial;

    protected virtual void Awake()
    {
        Hitbox = GetBehaviour<Hitbox>();
        defaultMaterial = figure.material;
        onHit += hittableBehaviour.onHit.Invoke;
        onHit += Blink;
    }

    /// <value>
    /// Plays blink effect on the character's figure, to emphasize hits
    /// </value>
    private void Blink()
    {
        if (!figure) return;
        figure.material = blinkMaterial;
        StartTimeout(() => figure.material = defaultMaterial, blinkTime);
    }

    public bool CanGetHit()
    {
        return hittableBehaviour.CanGetHit();
    }

    public virtual bool Hit(IHitExecutor executor, Hit hit)
    {
        return hittableBehaviour.Hit(executor, hit);
    }
}