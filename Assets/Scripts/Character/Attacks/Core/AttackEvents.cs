using System;
using UnityEngine.Events;

/// <summary>
/// Contains start and finish events for all attack phases
/// </summary>
[Serializable]
public class AttackEvents
{
    /// <value>
    /// Attack anticipation has started
    /// </value>
    public UnityEvent onStartAnticipating;

    /// <value>
    /// Attack anticipation has finished, also fires if the attack was stopped while in anticipation
    /// </value>
    public UnityEvent onFinishAnticipating;

    /// <value>
    /// Attack active phase has started
    /// </value>
    public UnityEvent onStartActive;

    /// <value>
    /// Attack active phase has finished, also fires if the attack was stopped while in active phase
    /// </value>
    public UnityEvent onFinishActive;

    /// <value>
    /// Attack recovery has started
    /// </value>
    public UnityEvent onStartRecovery;

    /// <value>
    /// Attack recovery has finished, also fires if the attack was stopped while in recovery
    /// </value>
    public UnityEvent onFinishRecovery;
}