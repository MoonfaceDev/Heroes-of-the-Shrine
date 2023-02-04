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
    Defense
}

/// <summary>
/// Character controller designed for a human player, that plays behaviours based on user input (keyboard, mouse, controller)
/// </summary>
public class PlayerController : CharacterController
{
    /// <summary>
    /// Pairing between attack and required button
    /// </summary>
    [Serializable]
    public class AttackProperty
    {
        public BaseAttack attack;
        public Button button;
    }

    /// <value>
    /// List of attacks that can be played using the controller
    /// </value>
    public AttackProperty[] attacks;

    /// <value>
    /// Window for input buffering, after that time is passed, that input is forgotten
    /// </value>
    [Header("Action buffering")] public float bufferingTime;

    [Header("Buffered actions priorities")]
    public int walkPriority;

    public int jumpPriority;
    public int slidePriority;
    public int dodgePriority;
    public int attackPriority;

    [Header("Special inputs")] public List<Button> possessedEffectTimeReducing;
    public float possessedEffectDurationReduction;


    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;
    private SlideBehaviour slideBehaviour;
    private DodgeBehaviour dodgeBehaviour;
    private List<BufferedAction> bufferedActions;

    protected override void Awake()
    {
        base.Awake();
        walkBehaviour = GetBehaviour<WalkBehaviour>();
        jumpBehaviour = GetBehaviour<JumpBehaviour>();
        slideBehaviour = GetBehaviour<SlideBehaviour>();
        dodgeBehaviour = GetBehaviour<DodgeBehaviour>();

        bufferedActions = new List<BufferedAction>();
    }

    protected override void Update()
    {
        base.Update();

        ReducePossessedEffectDuration();
        ExecuteBufferedActions();

        ExecuteWalk();
        ExecuteJump();
        ExecuteDodge();
        ExecuteSlide();
        ExecuteAttack();
    }

    private void ReducePossessedEffectDuration()
    {
        if (!possessedEffectTimeReducing.Any(button => Input.GetButtonDown(button.ToString()))) return;
        var possessedEffect = GetBehaviour<PossessedEffect>();
        if (possessedEffect)
        {
            possessedEffect.ReduceDuration(possessedEffectDurationReduction);
        }
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
            ExecutePlayable(slideBehaviour, new SlideBehaviour.Command(Math.Sign(horizontal)), slidePriority);
        }
    }

    private void ExecuteDodge()
    {
        var vertical = Input.GetAxisRaw("Vertical");
        if (dodgeBehaviour && Input.GetButtonDown(Button.Escape.ToString())) //pressed dodge
        {
            ExecutePlayable(dodgeBehaviour, new DodgeBehaviour.Command(Math.Sign(vertical)), dodgePriority);
        }
    }

    private void ExecuteAttack()
    {
        var downButtons = GetDownButtons();
        if (downButtons.Length == 0)
        {
            return;
        }

        if (!TryExecuteAttack(downButtons))
        {
            bufferedActions.Add(new BufferedAction(
                () => TryExecuteAttack(downButtons), attackPriority, Time.time + bufferingTime
            ));
        }
    }

    private static Button[] GetDownButtons()
    {
        return Enum.GetValues(typeof(Button)).Cast<Button>().Where(button => Input.GetButtonDown(button.ToString()))
            .ToArray();
    }

    private bool TryExecuteAttack(Button[] downButtons)
    {
        var command = new BaseAttack.Command();
        var nextAttack = GetNextAttack(command, downButtons);

        if (nextAttack == null)
        {
            return false;
        }

        nextAttack.Play(command);
        return true;
    }

    private BaseAttack GetNextAttack(BaseAttack.Command command, Button[] downButtons)
    {
        return (
            from property in attacks
            where property.attack.CanPlay(command) && downButtons.Any(button => button == property.button)
            select property.attack
        ).LastOrDefault();
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