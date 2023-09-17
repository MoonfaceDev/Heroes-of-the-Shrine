using System;
using System.Collections;
using System.Linq;
using ExtEvents;
using TypeReferences;
using UnityEngine;

public class FocusBlock : PhasedBehaviour<FocusBlock.Command>, IBlockBehaviour
{
    public class Command
    {
    }

    [Serializable]
    public class BlockDefinition
    {
        [Inherits(typeof(BaseAttack))] public TypeReference attackType;
        public float energyReward;
    }

    /// <value>
    /// Invincible to hits.
    /// It also sets the animator parameter: <c>FocusBlock-invincible</c>.
    /// </value>
    public bool Invincible
    {
        get => invincible;
        private set
        {
            invincible = value;
            Animator.SetBool($"{typeof(FocusBlock)}-invincible", value);
            (value ? onStartInvincible : onFinishInvincible).Invoke();
        }
    }

    public BlockDefinition[] blockableAttacks;
    public float activeTime;
    public float invincibleTime;
    [SerializeField] public ExtEvent onStartInvincible;
    [SerializeField] public ExtEvent onFinishInvincible;

    [InjectBehaviour] private EnergySystem energySystem;
    private bool invincible;

    public override bool Playing => base.Playing || Invincible;

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command) && !IsPlaying<JumpBehaviour>() && AttackManager.CanPlayAttack();
    }

    protected override void DoPlay(Command command)
    {
        StopBehaviours(typeof(IControlledBehaviour));
        BlockBehaviours(typeof(IControlledBehaviour));
        base.DoPlay(command);
    }

    protected override void DoStop()
    {
        base.DoStop();
        UnblockBehaviours(typeof(IControlledBehaviour));
    }

    protected override IEnumerator ActivePhase()
    {
        yield return new WaitForSeconds(activeTime);
    }
    
    public bool TryBlock(Hit hit)
    {
        if (!CanBlock(hit))
        {
            return false;
        }

        SuccessfulBlock(hit);
        return true;
    }

    private BlockDefinition GetBlockDefinition(BaseAttack attack)
    {
        return blockableAttacks.SingleOrDefault(blockableAttack => attack.GetType() == blockableAttack.attackType.Type);
    }

    private bool CanBlock(Hit hit)
    {
        var blockDefinition = GetBlockDefinition(hit.Source);
        return (Active || Invincible) && blockDefinition != null && Entity.WorldRotation == -hit.Direction;
    }

    private void SuccessfulBlock(Hit hit)
    {
        Stop();
        var blockDefinition = GetBlockDefinition(hit.Source);
        energySystem.AddEnergy(blockDefinition.energyReward);
        if (!Invincible)
        {
            TurnInvincible();
        }
    }

    private void TurnInvincible()
    {
        Invincible = true;
        BlockBehaviours(typeof(IMovementBehaviour), typeof(HealBehaviour));

        eventManager.StartTimeout(() =>
        {
            Invincible = false;
            UnblockBehaviours(typeof(IMovementBehaviour), typeof(HealBehaviour));
        }, invincibleTime);
    }
}