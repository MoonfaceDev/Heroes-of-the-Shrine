using UnityEngine;

/// <summary>
/// Enemies brain is designed with modular pieces called brain modules.
/// BrainCore class managing state machine and all brain modules.
/// Attach only the required brain modules for your state machine logic.
/// </summary>
[RequireComponent(typeof(Animator))]
public class BrainCore : CharacterController
{
    /// <value>
    /// Animator component that contains the state machine logic
    /// </value>
    public Animator StateMachine { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        StateMachine = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        StateMachine.enabled = true;
    }

    private void OnDisable()
    {
        StateMachine.enabled = false;
    }
}