using ExtEvents;
using UnityEngine;

/// <summary>
/// Behaviours responsible for processing hits from attacks
/// </summary>
public class HittableBehaviour : CharacterBehaviour, IHittable
{
    /// <value>
    /// Invoked when <see cref="Hit"/> is called
    /// </value>
    [SerializeField] public ExtEvent onHit;

    public ExtEvent HitEvent => onHit;

    private HealthSystem healthSystem;
    private KnockbackBehaviour knockbackBehaviour;
    private ForcedWalkBehaviour forcedWalkBehaviour;
    private FocusBlock focusBlock;

    protected override void Awake()
    {
        base.Awake();
        healthSystem = GetBehaviour<HealthSystem>();
        knockbackBehaviour = GetBehaviour<KnockbackBehaviour>();
    }

    public bool CanGetHit()
    {
        return healthSystem.Alive
               && !(knockbackBehaviour && knockbackBehaviour.Recovering)
               && !(forcedWalkBehaviour && forcedWalkBehaviour.Playing);
    }

    public bool Hit(IHitExecutor executor, Hit hit)
    {
        if (!CanGetHit())
        {
            return false;
        }

        if (focusBlock && focusBlock.TryBlock(hit))
        {
            return false;
        }

        executor.Execute(hit);

        return true;
    }
}