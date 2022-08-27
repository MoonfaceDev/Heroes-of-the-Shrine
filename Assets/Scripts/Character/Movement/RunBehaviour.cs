using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class RunBehaviour : CharacterBehaviour
{
    public float timeToRun;
    public float timeToStopRun;
    public float runSpeedMultiplier;

    public delegate void OnStart();
    public delegate void OnStop();

    public OnStart onStart;
    public OnStop onStop;

    public bool run
    {
        get { return _run; }
        set
        {
            _run = value;
            animator.SetBool("run", _run); if (value)
            {
                onStart();
            }
            else
            {
                onStop();
            }
        }
    }

    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;
    private Coroutine startRunningCoroutine;
    private ParticleSystem runParticles;
    private ParticleSystem.MainModule runParticlesMain;
    private bool _run;

    private void Start()
    {
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        runParticlesMain = runParticles.main;

        walkBehaviour.onStart += () =>
        {
            startRunningCoroutine = StartCoroutine(StartRunningAfter(timeToRun));
            eventManager.Callback(() => !walkBehaviour.walk, () => StopCoroutine(startRunningCoroutine));
        };
        walkBehaviour.onStop += () =>
        {
            if (startRunningCoroutine != null)
            {
                StopCoroutine(startRunningCoroutine);
            }
            if (run)
            {
                StartCoroutine(StopRunningAfter(timeToStopRun));
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

    private IEnumerator StartRunningAfter(float time)
    {
        yield return new WaitForSeconds(time);
        StartRunning();
    }

    private IEnumerator StopRunningAfter(float time)
    {
        yield return new WaitForSeconds(time);
        StopRunning();
    }

    public void StartRunning()
    {
        run = true;
        walkBehaviour.speed *= runSpeedMultiplier;
        runParticles.Play();
    }

    public void StopRunning()
    {
        run = false;
        walkBehaviour.speed = walkBehaviour.defaultSpeed;
        runParticles.Stop();
    }
}
