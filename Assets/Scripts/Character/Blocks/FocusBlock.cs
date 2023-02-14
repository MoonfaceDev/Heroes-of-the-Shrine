using System;
using System.Collections;
using System.Linq;
using ExtEvents;
using TypeReferences;
using UnityEngine;

public class FocusBlock : PhasedBehaviour<FocusBlock.Command>
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

    public bool TryBlock(BaseAttack attack)
    {
        if (!Active)
        {
            return false;
        }
        
        var blockDefinition = GetBlockDefinition(attack);
        if (blockDefinition == null)
        {
            return false;
        }

        SuccessfulBlock(blockDefinition);
        return true;
    }

    private void SuccessfulBlock(BlockDefinition blockDefinition)
    {
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