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
    public float anticipateTime;
    public float activeTime;
    public float invincibleTime;
    public float recoveryTime;
    [SerializeField] public ExtEvent onStartInvincible;
    [SerializeField] public ExtEvent onFinishInvincible;

    private EnergySystem energySystem;
    private bool invincible;

    protected override void Awake()
    {
        base.Awake();
        energySystem = GetBehaviour<EnergySystem>();
    }

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

    protected override IEnumerator AnticipationPhase()
    {
        yield return new WaitForSeconds(anticipateTime);
    }

    protected override IEnumerator ActivePhase()
    {
        yield return new WaitForSeconds(activeTime);
    }

    protected override IEnumerator RecoveryPhase()
    {
        yield return new WaitForSeconds(recoveryTime);
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

    private bool CanBlock(Hit hit)
    {
        var blockDefinition = GetBlockDefinition(hit.source);
        return (Active || Invincible) && blockDefinition != null && Entity.WorldRotation == -hit.direction;
    }

    private void SuccessfulBlock(Hit hit)
    {
        Stop();
        var blockDefinition = GetBlockDefinition(hit.source);
        energySystem.Energy += blockDefinition.energyReward;
        if (!Invincible)
        {
            TurnInvincible();
        }
    }

    private void TurnInvincible()
    {
        Invincible = true;
        BlockBehaviours(typeof(IMovementBehaviour), typeof(HealBehaviour));

        StartTimeout(() =>
        {
            Invincible = false;
            UnblockBehaviours(typeof(IMovementBehaviour), typeof(HealBehaviour));
        }, invincibleTime);
    }

    private BlockDefinition GetBlockDefinition(BaseAttack attack)
    {
        return blockableAttacks.SingleOrDefault(blockableAttack => attack.GetType() == blockableAttack.attackType.Type);
    }
}