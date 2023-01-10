public interface IAttackFlow
{
    /// <value>
    /// <see cref="IAttackPhase"/> played while attack is anticipating
    /// </value>
    public IAttackPhase AnticipationPhase { get; }

    /// <value>
    /// <see cref="IAttackPhase"/> played while attack is active
    /// </value>
    public IAttackPhase ActivePhase { get; }

    /// <value>
    /// <see cref="IAttackPhase"/> played while attack is recovering
    /// </value>
    public IAttackPhase RecoveryPhase { get; }
}