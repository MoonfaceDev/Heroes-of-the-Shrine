public class StaggerBehaviour : PlayableBehaviour<StaggerBehaviour.Command>
{
    public class Command
    {
    }

    public float staggerDuration;

    private bool Active
    {
        get => active;
        set
        {
            active = value;
            Animator.SetBool(GetType().Name, value);
        }
    }

    private bool active;
    private StunBehaviour stunBehaviour;

    public override bool Playing => Active;

    protected override void Awake()
    {
        base.Awake();
        stunBehaviour = GetBehaviour<StunBehaviour>();
    }

    private void Start()
    {
        AttackManager.onComboBlock += () => Play(new Command());
        stunBehaviour.PlayEvents.onStop += Stop;
    }

    protected override void DoPlay(Command command)
    {
        Active = true;
        stunBehaviour.Play(new StunBehaviour.Command { time = staggerDuration });
    }

    protected override void DoStop()
    {
        stunBehaviour.Stop();
        Active = false;
    }
}