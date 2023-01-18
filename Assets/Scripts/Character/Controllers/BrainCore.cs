using UnityEngine;

public class BrainCore : CharacterController
{
    public Animator StateMachine { get; private set; }

    public override void Awake()
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