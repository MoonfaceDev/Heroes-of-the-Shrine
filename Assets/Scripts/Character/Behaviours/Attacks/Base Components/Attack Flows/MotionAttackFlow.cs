using System;
using UnityEngine;

[Serializable]
public class MotionAttackFlowDefinition
{
    public TimedAttackPhase anticipationPhase;
    public TimedAttackPhase recoveryPhase;
}

public class MotionAttackFlow : IAttackFlow
{
    public IAttackPhase AnticipationPhase => definition.anticipationPhase;

    public IAttackPhase ActivePhase => new UntilAttackPhase(ActiveCondition);

    public IAttackPhase RecoveryPhase => definition.recoveryPhase;

    private readonly MotionAttackFlowDefinition definition;
    private readonly MovableObject movableObject;
    private readonly int originalDirection;

    public MotionAttackFlow(MotionAttackFlowDefinition definition, MovableObject movableObject, int originalDirection)
    {
        this.definition = definition;
        this.movableObject = movableObject;
        this.originalDirection = originalDirection;
    }

    private bool ActiveCondition()
    {
        return Mathf.Approximately(movableObject.velocity.x, 0) ||
               !Mathf.Approximately(Mathf.Sign(movableObject.velocity.x), originalDirection);
    }
}