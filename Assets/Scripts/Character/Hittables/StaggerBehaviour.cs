using System;
using System.Collections.Generic;
using UnityEngine;

public class StaggerBehaviour : PlayableBehaviour<StaggerBehaviour.Command>
{
    public class Command
    {
    }

    [Serializable]
    public record BalanceReductionEntry
    {
        public BaseAttack attack;
        public float balanceReduction;
    }

    public float staggerDuration;
    public float defaultBalance = 1;
    public float balanceShockDuration;
    public float balanceRegenerationRate;
    public List<BalanceReductionEntry> balanceReductionSettings;

    private float balance;
    private string shockTimeout;
    private bool shocked;

    private bool Active
    {
        get => active;
        set
        {
            active = value;
            Animator.SetBool(GetType().Name, value);
        }
    }

    private bool active;
    [InjectBehaviour] private StunBehaviour stunBehaviour;

    public override bool Playing => Active;

    protected override void Awake()
    {
        base.Awake();
        balance = defaultBalance;
        
        eventManager.Register(() =>
        {
            if (!shocked)
            {
                var addition = Time.deltaTime * balanceRegenerationRate;
                balance = Mathf.Min(balance + addition, defaultBalance);
            }
        });

        foreach (var entry in balanceReductionSettings)
        {
            entry.attack.onBlock += () => ReduceBalance(entry.balanceReduction);
        }
    }

    private void Start()
    {
        stunBehaviour.PlayEvents.onStop += Stop;
    }

    private void ReduceBalance(float reduction)
    {
        eventManager.Cancel(shockTimeout);
        
        balance = Mathf.Max(balance - reduction, 0);
        
        if (balance == 0)
        {
            balance = defaultBalance;
            Play(new Command());
        }
        else
        {
            shocked = true;
            shockTimeout = eventManager.StartTimeout(() => shocked = false, balanceShockDuration);
        }
    }

    protected override void DoPlay(Command command)
    {
        Active = true;
        stunBehaviour.Play(new StunBehaviour.Command { time = staggerDuration });
    }

    protected override void DoStop()
    {
        stunBehaviour.Stop();
        Active = false;
    }
}