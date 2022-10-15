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
    private IModifier speedModifier;
    private bool _run;


    public bool CanRun()
    {
        ElectrifiedEffect electrifiedEffect = GetComponent<ElectrifiedEffect>();
        return
            !(electrifiedEffect && electrifiedEffect.active);
    }


    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        runParticlesMain = runParticles.main;

        walkBehaviour.onStart += () =>
        {
            if (CanRun())
            {
                startCoroutine = StartCoroutine(RunAfter(timeToRun));
            }
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
        speedModifier = new MultiplierModifier(runSpeedMultiplier);
        walkBehaviour.speed.AddModifier(speedModifier);
        runParticles.Play();
        onStart?.Invoke();
    }

    public override void Stop()
    {
        if (run)
        {
            onStop?.Invoke();
            run = false;
            walkBehaviour.speed.RemoveModifier(speedModifier);
            runParticles.Stop();
        }
    }
}
