using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class PhasedBehaviour<T> : PlayableBehaviour<T>, IControlledBehaviour
{
    /// <value>
    /// General phase events
    /// </value>
    [FormerlySerializedAs("attackEvents")] public PhaseEvents phaseEvents;
    
    /// <value>
    /// If true, the behaviour can be played while another interruptible behaviour is playing.
    /// </value>
    public bool instant;

    /// <value>
    /// If true, an instant behaviour can replace it while this behaviour is playing.
    /// </value>
    public bool interruptible = true;

    /// <value>
    /// If <c>true</c>, The behaviour cannot be interrupted while <see cref="Recovering"/> is true.
    /// </value>
    public bool hardRecovery;
    
    /// <value>
    /// Behaviour is anticipating.
    /// It also sets the animator parameter: <c>{MyPhasedBehaviour}-anticipating</c>.
    /// </value>
    public bool Anticipating
    {
        get => anticipating;
        private set
        {
            anticipating = value;
            Animator.SetBool(Name + "-anticipating", anticipating);
            (value ? phaseEvents.onStartAnticipating : phaseEvents.onFinishAnticipating).Invoke();
        }
    }

    /// <value>
    /// Behaviour is active.
    /// It also sets the animator parameter: <c>{MyPhasedBehaviour}-active</c>.
    /// </value>
    public bool Active
    {
        get => active;
        private set
        {
            active = value;
            Animator.SetBool(Name + "-active", active);
            (value ? phaseEvents.onStartActive : phaseEvents.onFinishActive).Invoke();
        }
    }

    /// <value>
    /// Behaviour is recovering.
    /// It also sets the animator parameter: <c>{MyPhasedBehaviour}-recovering</c>.
    /// </value>
    public bool Recovering
    {
        get => recovering;
        private set
        {
            recovering = value;
            Animator.SetBool(Name + "-recovering", recovering);
            (value ? phaseEvents.onStartRecovery : phaseEvents.onFinishRecovery).Invoke();
        }
    }
    
    public override bool Playing => Anticipating || Active || Recovering;
    
    private string Name => GetType().Name;
    private bool anticipating;
    private bool active;
    private bool recovering;
    private Coroutine flowCoroutine;
    
    /// <summary>
    /// Anticipation phase coroutine
    /// </summary>
    protected abstract IEnumerator AnticipationPhase();
    
    /// <summary>
    /// Active phase coroutine
    /// </summary>
    protected abstract IEnumerator ActivePhase();
    
    /// <summary>
    /// Recovery phase coroutine
    /// </summary>
    protected abstract IEnumerator RecoveryPhase();

    private IEnumerator FlowCoroutine()
    {
        Anticipating = true;
        yield return AnticipationPhase();
        Anticipating = false;
        Active = true;
        yield return ActivePhase();
        Active = false;
        Recovering = true;
        yield return RecoveryPhase();
        Stop();
    }
    
    /// <summary>
    /// Play the phases
    /// </summary>
    protected override void DoPlay(T command)
    {
        flowCoroutine = StartCoroutine(FlowCoroutine());
    }
    
    /// <summary>
    /// Stops the behaviour immediately in any of its phases.
    /// Can be safely called even if behaviour is not currently playing, as it will do nothing.
    /// </summary>
    protected override void DoStop()
    {
        StopCoroutine(flowCoroutine);

        if (Anticipating)
        {
            Anticipating = false;
            phaseEvents.onFinishAnticipating.Invoke();
        }

        if (Active)
        {
            Active = false;
            phaseEvents.onFinishActive.Invoke();
        }

        if (Recovering)
        {
            Recovering = false;
            phaseEvents.onFinishRecovery.Invoke();
        }
    }
}