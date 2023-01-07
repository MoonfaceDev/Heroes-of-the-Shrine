using UnityEngine;

public class RunCommand : ICommand
{
}

[RequireComponent(typeof(WalkBehaviour))]
public class RunBehaviour : BaseMovementBehaviour<RunCommand>
{
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


    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        runParticlesMain = runParticles.main;
    }

    public void Start()
    {
        string startTimeout = null;

        walkBehaviour.PlayEvents.onPlay.AddListener(() =>
        {
            if (!Run)
            {
                startTimeout ??= StartTimeout(() =>
                {
                    startTimeout = null;
                    Play(new RunCommand());
                }, timeToRun);
            }
        });

        walkBehaviour.PlayEvents.onStop.AddListener(() =>
        {
            Cancel(startTimeout);
            startTimeout = null;
        });

        walkBehaviour.PlayEvents.onStop.AddListener(Stop);

        if (jumpBehaviour)
        {
            jumpBehaviour.OnJump += () => { runParticlesMain.gravityModifier = 1f; };
            jumpBehaviour.OnLand += () => { runParticlesMain.gravityModifier = 0; };
        }
    }

    protected override void DoPlay(RunCommand command)
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