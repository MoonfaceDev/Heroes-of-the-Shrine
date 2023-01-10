using System;

[Serializable]
public class TimedAttackFlow : IAttackFlow
{
    public TimedAttackPhase anticipationPhase;
    public TimedAttackPhase activePhase;
    public TimedAttackPhase recoveryPhase;

    public IAttackPhase AnticipationPhase => anticipationPhase;

    public IAttackPhase ActivePhase => activePhase;

    public IAttackPhase RecoveryPhase => recoveryPhase;
}