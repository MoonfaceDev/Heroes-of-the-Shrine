using System;
using System.Collections;
using System.Linq;
using ExtEvents;
using TypeReferences;
using UnityEngine;

public class WideBlock : PhasedBehaviour<WideBlock.Command>, IBlockBehaviour
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
    /// It also sets the animator parameter: <c>WideBlock-invincible</c>.
    /// </value>
    public bool Invincible
    {
        get => invincible;
        private set
        {
            invincible = value;
            Animator.SetBool($"{typeof(WideBlock)}-invincible", value);
            (value ? onStartInvincible : onFinishInvincible).Invoke();
        }
    }

    public BlockDefinition[] blockableAttacks;
    public float activeTime;
    public float damageMultiplier = 1;
    public float blockWindowStartTime;
    public float blockWindowDuration;
    public float invincibleTime;
    [SerializeField] public ExtEvent onStartInvincible;
    [SerializeField] public ExtEvent onFinishInvincible;

    [InjectBehaviour] private HealthSystem healthSystem;
    [InjectBehaviour] private EnergySystem energySystem;
    private string blockWindowStartTimeout;
    private string blockWindowStopTimeout;
    private bool blockWindow;
    private bool invincible;

    protected override void Awake()
    {
        base.Awake();

        phaseEvents.onStartActive += () =>
        {
            healthSystem.damageMultiplier *= damageMultiplier;
            BlockBehaviours(typeof(IForcedBehaviour));
        };

        phaseEvents.onFinishActive += () =>
        {
            healthSystem.damageMultiplier /= damageMultiplier;
            UnblockBehaviours(typeof(IForcedBehaviour));
        };

        PlayEvents.onStop += () =>
        {
            eventManager.Cancel(blockWindowStartTimeout);
            eventManager.Cancel(blockWindowStopTimeout);
        };
    }
    public override bool Playing => base.Playing || Invincible;

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command) && !IsPlaying<JumpBehaviour>();
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
        blockWindow = false;
        UnblockBehaviours(typeof(IControlledBehaviour));
    }

    protected override IEnumerator ActivePhase()
    {
        blockWindowStartTimeout = eventManager.StartTimeout(() => blockWindow = true, blockWindowStartTime);
        blockWindowStopTimeout = eventManager.StartTimeout(
            () => blockWindow = false,
            blockWindowStartTime + blockWindowDuration
        );
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
        return (blockWindow || Invincible) && blockDefinition != null;
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