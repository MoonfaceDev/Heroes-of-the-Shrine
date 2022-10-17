using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class RunBehaviour : BaseMovementBehaviour
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
            Animator.SetBool("run", run);
        }
    }

    public override bool Playing => Run;

    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;
    private Coroutine startCoroutine;
    private ParticleSystem.MainModule runParticlesMain;
    private IModifier speedModifier;
    private bool run;


    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        runParticlesMain = runParticles.main;
    }

    public void Start()
    {
        walkBehaviour.OnPlay += () =>
        {
            if (CanPlay())
            {
                startCoroutine = StartCoroutine(RunAfter(timeToRun));
            }
        };
        walkBehaviour.OnStop += () =>
        {
            Stop();
        };
        if (jumpBehaviour)
        {
            jumpBehaviour.OnJump += () =>
            {
                runParticlesMain.gravityModifier = 1f;
            };
            jumpBehaviour.OnLand += () =>
            {
                runParticlesMain.gravityModifier = 0;
            };
        }
    }

    private IEnumerator RunAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Play();
    }

    public void Play()
    {
        if (!CanPlay())
        {
            return;
        }
        Run = true;
        speedModifier = new MultiplierModifier(runSpeedMultiplier);
        walkBehaviour.speed.AddModifier(speedModifier);
        runParticles.Play();
        InvokeOnPlay();
    }

    public override void Stop()
    {
        if (Run)
        {
            InvokeOnStop();
            Run = false;
            walkBehaviour.speed.RemoveModifier(speedModifier);
            runParticles.Stop();
        }
        if (startCoroutine != null)
        {
            StopCoroutine(startCoroutine);
        }
    }
}
