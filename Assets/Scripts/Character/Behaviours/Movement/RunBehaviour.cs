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

    public bool active
    {
        get => _active;
        private set
        {
            _active = value;
            animator.SetBool("run", _active);
        }
    }

    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;
    private Coroutine startCoroutine;
    private ParticleSystem.MainModule runParticlesMain;
    private bool _active;

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
            if (active)
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
        active = true;
        walkBehaviour.speed *= runSpeedMultiplier;
        runParticles.Play();
        onStart?.Invoke();
    }

    public void Stop()
    {
        active = false;
        walkBehaviour.speed = walkBehaviour.defaultSpeed;
        runParticles.Stop();
        onStop?.Invoke();
    }
}
