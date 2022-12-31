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
            Animator.SetBool(RunParameter, run);
        }
    }

    public override bool Playing => Run;

    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;
    private Coroutine startCoroutine;
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
        walkBehaviour.onPlay.AddListener(() =>
        {
            if (CanPlay())
            {
                startCoroutine = StartCoroutine(RunAfter(timeToRun));
            }
        });
        walkBehaviour.onStop.AddListener(Stop);
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
        walkBehaviour.speed *= runSpeedMultiplier;
        runParticles.Play();
        onPlay.Invoke();
    }

    public override void Stop()
    {
        if (Run)
        {
            onStop.Invoke();
            Run = false;
            walkBehaviour.speed /= runSpeedMultiplier;
            runParticles.Stop();
        }
        if (startCoroutine != null)
        {
            StopCoroutine(startCoroutine);
        }
    }
}
