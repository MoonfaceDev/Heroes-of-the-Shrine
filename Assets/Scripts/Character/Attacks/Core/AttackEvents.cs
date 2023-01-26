using System;
using ExtEvents;
using UnityEngine;

/// <summary>
/// Contains start and finish events for all attack phases
/// </summary>
[Serializable]
public class AttackEvents
{
    /// <value>
    /// Attack anticipation has started
    /// </value>
    [SerializeField] public ExtEvent onStartAnticipating;

    /// <value>
    /// Attack anticipation has finished, also fires if the attack was stopped while in anticipation
    /// </value>
    [SerializeField] public ExtEvent onFinishAnticipating;

    /// <value>
    /// Attack active phase has started
    /// </value>
    [SerializeField] public ExtEvent onStartActive;

    /// <value>
    /// Attack active phase has finished, also fires if the attack was stopped while in active phase
    /// </value>
    [SerializeField] public ExtEvent onFinishActive;

    /// <value>
    /// Attack recovery has started
    /// </value>
    [SerializeField] public ExtEvent onStartRecovery;

    /// <value>
    /// Attack recovery has finished, also fires if the attack was stopped while in recovery
    /// </value>
    [SerializeField] public ExtEvent onFinishRecovery;
}