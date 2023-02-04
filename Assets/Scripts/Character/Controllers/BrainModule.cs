using UnityEngine;

/// <summary>
/// Modular piece of enemy brain. This is a base abstract class for any brain module
/// </summary>
public abstract class BrainModule : CharacterBehaviour
{
    private BrainCore brainCore;

    /// <value>
    /// Animator component that contains the state machine logic
    /// </value>
    protected Animator StateMachine => brainCore.StateMachine;

    protected override void Awake()
    {
        base.Awake();
        brainCore = GetBehaviour<BrainCore>();
    }

    /// <summary>
    /// The parameters provided by this method will be used to show the parameter names that should be added in state machine
    /// </summary>
    /// <returns>Parameter names provided by this module</returns>
    public abstract string[] GetParameters();
}