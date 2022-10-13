using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class RunBehaviour : CharacterBehaviour
{
    public float timeToRun;
    public float runSpeedMultiplier;
    public ParticleSystem runParticles;

    public event Action onStart;
    public event Action onStop;

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
                Stop();
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
