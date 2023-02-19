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

    public ExtEvent HitEvent => onHit;

    private HealthSystem healthSystem;
    private KnockbackBehaviour knockbackBehaviour;
    private ForcedWalkBehaviour forcedWalkBehaviour;
    private IEnumerable<IBlockBehaviour> blockBehaviours;

    protected override void Awake()
    {
        base.Awake();
        healthSystem = GetBehaviour<HealthSystem>();
        knockbackBehaviour = GetBehaviour<KnockbackBehaviour>();
        forcedWalkBehaviour = GetBehaviour<ForcedWalkBehaviour>();
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
                hit.source.Stop();
                hit.source.AttackManager.onBlock.Invoke(hit);
                return;
            }
        }

        hit.victim.HitEvent.Invoke();
        executor.Execute(hit);
    }

    public void ProcessHit(IHitExecutor executor, Hit hit)
    {
        executor.Execute(hit);
    }
}