using System;
using ExtEvents;
using UnityEngine;

/// <summary>
/// Contains start and finish events for all phases
/// </summary>
[Serializable]
public class PhaseEvents
{
    /// <value>
    /// Anticipation has started
    /// </value>
    [SerializeField] public ExtEvent onStartAnticipating;

    /// <value>
    /// Anticipation has finished, also fires if behaviour was stopped while in anticipation
    /// </value>
    [SerializeField] public ExtEvent onFinishAnticipating;

    /// <value>
    /// Active phase has started
    /// </value>
    [SerializeField] public ExtEvent onStartActive;

    /// <value>
    /// Active phase has finished, also fires if behaviour was stopped while in active phase
    /// </value>
    [SerializeField] public ExtEvent onFinishActive;

    /// <value>
    /// Recovery has started
    /// </value>
    [SerializeField] public ExtEvent onStartRecovery;

    /// <value>
    /// Recovery has finished, also fires if behaviour was stopped while in recovery
    /// </value>
    [SerializeField] public ExtEvent onFinishRecovery;
}