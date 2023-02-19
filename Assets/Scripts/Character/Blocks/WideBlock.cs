using System;
using System.Collections;
using System.Linq;
using ExtEvents;
using TypeReferences;
using UnityEngine;
using UnityEngine.Serialization;

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

    public BlockDefinition[] blockableAttacks;
    public float anticipateTime;
    public float activeTime;
    public float damageMultiplier = 1;
    public float blockWindowStartTime;
    public float blockWindowDuration;
    public float recoveryTime;
    [SerializeField] public ExtEvent onBlock;

    private HealthSystem healthSystem;
    private EnergySystem energySystem;
    private string blockWindowStartTimeout;
    private string blockWindowStopTimeout;
    private bool blockWindow;

    protected override void Awake()
    {
        base.Awake();

        healthSystem = GetBehaviour<HealthSystem>();
        energySystem = GetBehaviour<EnergySystem>();

        phaseEvents.onStartActive += () =>
        {
            healthSystem.damageMultiplier *= damageMultiplier;
            BlockBehaviours(typeof(IForcedBehaviour));
        };
        
        phaseEvents.onFinishActive += () =>
        {
            healthSystem.damageMultiplier /= damageMultiplier;
            UnblockBehaviours(typeof(KnockbackBehaviour));
        };

        PlayEvents.onStop += () =>
        {
            Cancel(blockWindowStartTimeout);
            Cancel(blockWindowStopTimeout);
        };
    }

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
        blockWindowStartTimeout = StartTimeout(() => blockWindow = true, blockWindowStartTime);
        blockWindowStopTimeout = StartTimeout(
            () => blockWindow = false,
            blockWindowStartTime + blockWindowDuration
        );
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
        return blockWindow && blockDefinition != null;
    }

    private void SuccessfulBlock(Hit hit)
    {
        var blockDefinition = GetBlockDefinition(hit.source);
        energySystem.Energy += blockDefinition.energyReward;
        Animator.SetTrigger($"{GetType().Name}-block");
        onBlock.Invoke();
        Stop();
    }

    private BlockDefinition GetBlockDefinition(BaseAttack attack)
    {
        return blockableAttacks.SingleOrDefault(blockableAttack => attack.GetType() == blockableAttack.attackType.Type);
    }
}