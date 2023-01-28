using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class RunBehaviour : BaseMovementBehaviour<RunBehaviour.Command>
{
    public class Command
    {
    }

    public float timeToRun;
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
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        runParticlesMain = runParticles.main;
    }

    private void Start()
    {
        string startTimeout = null;

        walkBehaviour.PlayEvents.onPlay += () =>
        {
            if (!Run)
            {
                startTimeout ??= StartTimeout(() =>
                {
                    startTimeout = null;
                    Play(new Command());
                }, timeToRun);
            }
        };

        walkBehaviour.PlayEvents.onStop += () =>
        {
            Cancel(startTimeout);
            startTimeout = null;
        };

        walkBehaviour.PlayEvents.onStop += Stop;

        if (jumpBehaviour)
        {
            jumpBehaviour.onStartActive += () => runParticlesMain.gravityModifier = 1f;
            jumpBehaviour.onFinishActive += () => runParticlesMain.gravityModifier = 0;
        }
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