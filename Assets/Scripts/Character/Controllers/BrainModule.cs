using UnityEngine;

[RequireComponent(typeof(BrainCore))]
public abstract class BrainModule : CharacterBehaviour
{
    private BrainCore brainCore;
    protected Animator StateMachine => brainCore.StateMachine;

    public override void Awake()
    {
        base.Awake();
        brainCore = GetComponent<BrainCore>();
    }

    public abstract string[] GetParameters();
}