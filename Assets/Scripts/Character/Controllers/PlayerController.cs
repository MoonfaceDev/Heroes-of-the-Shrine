﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Button options
/// </summary>
public enum Button
{
    Jump,
    Escape,
    Attack,
    Defense,
    Run,
    Heal,
    Spike,
    Thread,
}

/// <summary>
/// Character controller designed for a human player, that plays behaviours based on user input (keyboard, mouse, controller)
/// </summary>
public class PlayerController : CharacterController
{
    public PlayerAttackExecutor attackExecutor;

    /// <value>
    /// Window for input buffering, after that time is passed, that input is forgotten
    /// </value>
    [Header("Action buffering")] public float bufferingTime;

    [Header("Buffered actions priorities")]
    public int jumpPriority;

    public int slidePriority;
    public int dodgePriority;
    public int attackPriority;

    [Header("Special inputs")] public List<Button> possessedEffectTimeReducing;
    public float possessedEffectDurationReduction;


    [InjectBehaviour] private WalkBehaviour walkBehaviour;
    [InjectBehaviour] private RunBehaviour runBehaviour;
    [InjectBehaviour] private JumpBehaviour jumpBehaviour;
    [InjectBehaviour] private SlideBehaviour slideBehaviour;
    [InjectBehaviour] private DodgeBehaviour dodgeBehaviour;
    [InjectBehaviour] private HealBehaviour healBehaviour;
    [InjectBehaviour] private FocusBlock focusBlock;
    [InjectBehaviour] private WideBlock wideBlock;

    [InjectBehaviour] private PossessedEffect possessedEffect;

    private List<BufferedAction> bufferedActions;

    protected override void Awake()
    {
        base.Awake();
        bufferedActions = new List<BufferedAction>();
    }

    protected override void Update()
    {
        base.Update();

        ReducePossessedEffectDuration();
        ExecuteBufferedActions();

        ExecuteWalk();
        ExecuteRun();
        ExecuteJump();
        ExecuteDodge();
        ExecuteSlide();
        ExecuteHeal();
        ExecuteAttack();
        ExecuteFocusBlock();
        ExecuteWideBlock();
    }

    private void ReducePossessedEffectDuration()
    {
        if (!possessedEffectTimeReducing.Any(button => Input.GetButtonDown(button.ToString()))) return;
        possessedEffect.ReduceDuration(possessedEffectDurationReduction);
    }

    private void ExecuteBufferedActions()
    {
        // clear expired buffered actions
        bufferedActions = bufferedActions.FindAll(action => !action.IsExpired());
        foreach (var action in bufferedActions.OrderByDescending(action => action.Priority))
        {
            if (!action.TryExecute()) continue;
            bufferedActions.Clear();
            return;
        }
    }

    private void ExecuteWalk()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        if (!walkBehaviour) return;
        if (horizontal != 0 || vertical != 0)
        {
            walkBehaviour.Play(new WalkBehaviour.Command(new Vector2(horizontal, vertical)));
        }
        else
        {
            walkBehaviour.Stop();
        }
    }

    private void ExecuteRun()
    {
        if (!runBehaviour) return;
        if (Input.GetButton(Button.Run.ToString()))
        {
            if (!runBehaviour.Playing)
            {
                runBehaviour.Play(new RunBehaviour.Command());
            }
        }
        else
        {
            runBehaviour.Stop();
        }
    }

    private void ExecuteJump()
    {
        if (jumpBehaviour && Input.GetButtonDown(Button.Jump.ToString())) //pressed jump
        {
            ExecutePlayable(jumpBehaviour, new JumpBehaviour.Command(), jumpPriority);
        }
    }

    private void ExecuteSlide()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        if (slideBehaviour && Input.GetButtonDown(Button.Escape.ToString())) //pressed slide
        {
            ExecutePlayable(slideBehaviour, new SlideBehaviour.Command { direction = Math.Sign(horizontal) },
                slidePriority);
        }
    }

    private void ExecuteDodge()
    {
        var vertical = Input.GetAxisRaw("Vertical");
        if (dodgeBehaviour && Input.GetButtonDown(Button.Escape.ToString())) //pressed dodge
        {
            ExecutePlayable(dodgeBehaviour, new DodgeBehaviour.Command { direction = Math.Sign(vertical) },
                dodgePriority);
        }
    }

    private void ExecuteHeal()
    {
        if (!healBehaviour) return;
        if (Input.GetButtonDown(Button.Heal.ToString()))
        {
            healBehaviour.Play(new HealBehaviour.Command());
        }
    }

    private void ExecuteFocusBlock()
    {
        if (!focusBlock) return;
        if (Input.GetButtonDown(Button.Defense.ToString()))
        {
            focusBlock.Play(new FocusBlock.Command());
        }
    }

    private void ExecuteWideBlock()
    {
        if (!wideBlock) return;
        if (Input.GetButtonDown(Button.Escape.ToString()))
        {
            wideBlock.Play(new WideBlock.Command());
        }
    }

    private void ExecuteAttack()
    {
        var upButtons = GetUpButtons();
        if (upButtons.Length > 0)
        {
            attackExecutor.Release(upButtons);
        }

        var downButtons = GetDownButtons();
        if (downButtons.Length > 0 && !attackExecutor.Play(downButtons))
        {
            bufferedActions.Add(new BufferedAction(
                () => attackExecutor.Play(downButtons), attackPriority, Time.time + bufferingTime
            ));
        }
    }

    private static IEnumerable<Button> GetButtons()
    {
        return Enum.GetValues(typeof(Button)).Cast<Button>();
    }

    private static Button[] GetDownButtons()
    {
        return GetButtons().Where(button => Input.GetButtonDown(button.ToString())).ToArray();
    }

    private static Button[] GetUpButtons()
    {
        return GetButtons().Where(button => Input.GetButtonUp(button.ToString())).ToArray();
    }

    private void ExecutePlayable<T>(PlayableBehaviour<T> behaviour, T command, int bufferingPriority)
    {
        if (!TryExecutePlayable(behaviour, command))
        {
            bufferedActions.Add(new BufferedAction(
                () => TryExecutePlayable(behaviour, command),
                bufferingPriority,
                Time.time + bufferingTime
            ));
        }
    }

    private static bool TryExecutePlayable<T>(PlayableBehaviour<T> behaviour, T command)
    {
        if (!behaviour.CanPlay(command))
        {
            return false;
        }

        behaviour.Play(command);
        return true;
    }

    private class BufferedAction
    {
        private readonly Func<bool> execute;
        private readonly float expirationTime;

        public BufferedAction(Func<bool> execute, int priority, float expirationTime)
        {
            this.execute = execute;
            Priority = priority;
            this.expirationTime = expirationTime;
        }

        public int Priority { get; }

        public bool IsExpired()
        {
            return Time.time > expirationTime;
        }

        public bool TryExecute()
        {
            return execute();
        }
    }
}