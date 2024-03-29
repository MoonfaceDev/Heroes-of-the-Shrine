using System.Collections.Generic;
using ExtEvents;
using UnityEngine;

/// <summary>
/// Behaviours responsible for processing hits from attacks
/// </summary>
public class HittableBehaviour : CharacterBehaviour, IHittable
{
    /// <value>
    /// Invoked when <see cref="ProcessHit"/> is called
    /// </value>
    [SerializeField] public ExtEvent onHit;

    public GameEntity RelatedEntity => Entity;
    public ExtEvent HitEvent => onHit;

    [InjectBehaviour] private HealthSystem healthSystem;
    [InjectBehaviour] private KnockbackBehaviour knockbackBehaviour;
    [InjectBehaviour] private ForcedWalkBehaviour forcedWalkBehaviour;
    private IEnumerable<IBlockBehaviour> blockBehaviours;

    protected override void Awake()
    {
        base.Awake();
        blockBehaviours = GetBehaviours<IBlockBehaviour>();
    }

    public bool CanGetHit()
    {
        return healthSystem.Alive
               && !(knockbackBehaviour && knockbackBehaviour.Recovering)
               && !(forcedWalkBehaviour && forcedWalkBehaviour.Playing);
    }

    public void Hit(ChainHitExecutor executor, Hit hit)
    {
        if (!CanGetHit())
        {
            return;
        }

        foreach (var blockBehaviour in blockBehaviours)
        {
            if (blockBehaviour.TryBlock(hit))
            {
                hit.Source.Block();
                return;
            }
        }

        hit.Victim.HitEvent.Invoke();
        executor.Execute(hit);
    }

    public void ProcessHit(IHitExecutor executor, Hit hit)
    {
        executor.Execute(hit);
    }
}