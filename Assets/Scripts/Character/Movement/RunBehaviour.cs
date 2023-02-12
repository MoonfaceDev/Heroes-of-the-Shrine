using UnityEngine;

public class RunBehaviour : PlayableBehaviour<RunBehaviour.Command>, IMovementBehaviour
{
    public class Command
    {
    }

    public float runSpeedMultiplier;
    public ParticleSystem runParticles;

    public bool Run
    {
        get => run;
        private set
        {
            run = value;
            Animator.SetBool(RunParameter, run);
        }
    }

    public override bool Playing => Run;

    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;
    private ParticleSystem.MainModule runParticlesMain;
    private bool run;

    private static readonly int RunParameter = Animator.StringToHash("run");

    protected override void Awake()
    {
        base.Awake();
        walkBehaviour = GetBehaviour<WalkBehaviour>();
        jumpBehaviour = GetBehaviour<JumpBehaviour>();
        runParticlesMain = runParticles.main;
    }

    private void Start()
    {
        if (jumpBehaviour)
        {
            jumpBehaviour.onStartActive += () => runParticlesMain.gravityModifier = 1f;
            jumpBehaviour.onFinishActive += () => runParticlesMain.gravityModifier = 0;
        }
    }

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command) && !walkBehaviour.Blocked;
    }

    protected override void DoPlay(Command command)
    {
        Run = true;
        walkBehaviour.speed *= runSpeedMultiplier;
        runParticles.Play();
    }

    protected override void DoStop()
    {
        Run = false;
        walkBehaviour.speed /= runSpeedMultiplier;
        runParticles.Stop();
    }
}