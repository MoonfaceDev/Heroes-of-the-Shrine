using System;
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
    Heal
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
    public int jumpPriority;

    public int slidePriority;
    public int dodgePriority;
    public int attackPriority;

    [Header("Special inputs")] public List<Button> possessedEffectTimeReducing;
    public float possessedEffectDurationReduction;


    private WalkBehaviour walkBehaviour;
    private RunBehaviour runBehaviour;
    private JumpBehaviour jumpBehaviour;
    private SlideBehaviour slideBehaviour;
    private DodgeBehaviour dodgeBehaviour;
    private HealBehaviour healBehaviour;
    private FocusBlock focusBlock;
    private WideBlock wideBlock;

    private List<BufferedAction> bufferedActions;

    protected override void Awake()
    {
        base.Awake();
        walkBehaviour = GetBehaviour<WalkBehaviour>();
        runBehaviour = GetBehaviour<RunBehaviour>();
        jumpBehaviour = GetBehaviour<JumpBehaviour>();
        slideBehaviour = GetBehaviour<SlideBehaviour>();
        dodgeBehaviour = GetBehaviour<DodgeBehaviour>();
        healBehaviour = GetBehaviour<HealBehaviour>();
        focusBlock = GetBehaviour<FocusBlock>();
        wideBlock = GetBehaviour<WideBlock>();

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

        if (Input.GetButtonUp(Button.Heal.ToString()))
        {
            healBehaviour.Stop();
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
            where downButtons.Any(button => button == property.button) && property.attack.CanPlay(command)
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