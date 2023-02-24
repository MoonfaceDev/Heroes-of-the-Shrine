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

    public BlockDefinition[] blockableAttacks;
    public float anticipateTime;
    public float activeTime;
    public float recoveryTime;
    [SerializeField] public ExtEvent onBlock;

    private EnergySystem energySystem;

    protected override void Awake()
    {
        base.Awake();
        energySystem = GetBehaviour<EnergySystem>();
    }

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
        return Active && blockDefinition != null && Entity.WorldRotation == -hit.direction;
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