using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class RunBehaviour : BaseMovementBehaviour
{
    public float timeToRun;
    public float runSpeedMultiplier;
    public ParticleSystem runParticles;

    public bool run
    {
        get => _run;
        private set
        {
            _run = value;
            animator.SetBool("run", _run);
        }
    }

    public override bool Playing => run;

    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;
    private Coroutine startCoroutine;
    private ParticleSystem.MainModule runParticlesMain;
    private IModifier speedModifier;
    private bool _run;


    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        runParticlesMain = runParticles.main;
    }

    public void Start()
    {
        walkBehaviour.onPlay += () =>
        {
            if (CanPlay())
            {
                startCoroutine = StartCoroutine(RunAfter(timeToRun));
            }
        };
        walkBehaviour.onStop += () =>
        {
            Stop();
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
        Play();
    }

    public void Play()
    {
        if (!CanPlay())
        {
            return;
        }
        run = true;
        speedModifier = new MultiplierModifier(runSpeedMultiplier);
        walkBehaviour.speed.AddModifier(speedModifier);
        runParticles.Play();
        InvokeOnPlay();
    }

    public override void Stop()
    {
        if (run)
        {
            InvokeOnStop();
            run = false;
            walkBehaviour.speed.RemoveModifier(speedModifier);
            runParticles.Stop();
        }
        if (startCoroutine != null)
        {
            StopCoroutine(startCoroutine);
        }
    }
}
