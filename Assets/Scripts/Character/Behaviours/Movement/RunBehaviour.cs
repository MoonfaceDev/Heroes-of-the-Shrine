using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class RunBehaviour : CharacterBehaviour
{
    public float timeToRun;
    public float timeToStopRun;
    public float runSpeedMultiplier;
    public ParticleSystem runParticles;

    public delegate void OnStart();
    public delegate void OnStop();

    public event OnStart onStart;
    public event OnStop onStop;

    public bool run
    {
        get => _run;
        private set
        {
            _run = value;
            animator.SetBool("run", _run);
        }
    }

    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;
    private Coroutine startCoroutine;
    private ParticleSystem.MainModule runParticlesMain;
    private bool _run;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        runParticlesMain = runParticles.main;

        walkBehaviour.onStart += () =>
        {
            startCoroutine = StartCoroutine(RunAfter(timeToRun));
        };
        walkBehaviour.onStop += () =>
        {
            if (startCoroutine != null)
            {
                StopCoroutine(startCoroutine);
            }
            if (run)
            {
                StartCoroutine(StopAfter(timeToStopRun));
            }
        };
        if (jumpBehaviour)
        {
            jumpBehaviour.onJump += () =>
            {
                runParticlesMain.gravityModifier = 1f;
            };
            jumpBehaviour.onLand += () =>
            {
                runParticlesMain.gravityModifier = 0;
            };
        }
    }

    private IEnumerator RunAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Run();
    }

    private IEnumerator StopAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Stop();
    }

    public void Run()
    {
        run = true;
        walkBehaviour.speed *= runSpeedMultiplier;
        runParticles.Play();
        onStart?.Invoke();
    }

    public void Stop()
    {
        run = false;
        walkBehaviour.speed = walkBehaviour.defaultSpeed;
        runParticles.Stop();
        onStop?.Invoke();
    }
}
