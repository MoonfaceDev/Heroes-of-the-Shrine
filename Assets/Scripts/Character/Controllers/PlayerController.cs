using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Button
{
    Jump,
    Escape,
    Attack,
    Defense
}

public class BufferedAction
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

[Serializable]
public class AttackProperty
{
    public BaseAttack attack;
    public Button button;
}

public class PlayerController : CharacterController
{
    public AttackProperty[] attacks;
    public RuntimeAnimatorController[] animatorControllers;
    [HideInInspector] public int activeSuitIndex;

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

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        slideBehaviour = GetComponent<SlideBehaviour>();
        dodgeBehaviour = GetComponent<DodgeBehaviour>();

        bufferedActions = new List<BufferedAction>();
    }

    protected override void Update()
    {
        base.Update();
        
        // reduce duration of possessed effect
        if (possessedEffectTimeReducing.Any(button => Input.GetButtonDown(button.ToString())))
        {
            var possessedEffect = GetComponent<PossessedEffect>();
            if (possessedEffect)
            {
                possessedEffect.ReduceDuration(possessedEffectDurationReduction);
            }
        }

        // play the buffered action with highest priority
        ExecuteBufferedActions();

        //walking
        ExecuteWalk();
        //jumping
        ExecuteJump();
        //sliding
        ExecuteSlide();
        //dodging
        ExecuteDodge();
        //attacks
        ExecuteAttack();

        //change suits
        if (!Input.GetKeyDown(KeyCode.Alpha7)) return;
        activeSuitIndex = 1 - activeSuitIndex;
        Animator.runtimeAnimatorController = animatorControllers[activeSuitIndex];
    }

    private void ExecuteBufferedActions()
    {
        // clear expired buffered actions
        bufferedActions = bufferedActions.FindAll(action => !action.IsExpired());
        if (bufferedActions.Count == 0) return;
        foreach (var action in bufferedActions.OrderByDescending(action => action.Priority))
        {
            var isSuccess = action.TryExecute();
            if (isSuccess)
            {
                bufferedActions.Clear();
                return;
            }
        }
    }

    private void ExecuteWalk()
    {
        var horizontal = Direction(Input.GetAxisRaw("Horizontal"));
        var vertical = Direction(Input.GetAxisRaw("Vertical"));
        if (!walkBehaviour) return;
        if (horizontal != 0 || vertical != 0)
        {
            walkBehaviour.Play(new WalkCommand(horizontal, vertical));
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
            ExecutePlayable(jumpBehaviour, new JumpCommand(), jumpPriority);
        }
    }

    private void ExecuteSlide()
    {
        var horizontal = Direction(Input.GetAxisRaw("Horizontal"));
        if (slideBehaviour && Input.GetButtonDown(Button.Escape.ToString())) //pressed slide
        {
            ExecutePlayable(slideBehaviour, new SlideCommand(horizontal), slidePriority);
        }
    }

    private void ExecuteDodge()
    {
        var vertical = Direction(Input.GetAxisRaw("Vertical"));
        if (dodgeBehaviour && Input.GetButtonDown(Button.Escape.ToString())) //pressed dodge
        {
            ExecutePlayable(dodgeBehaviour, new DodgeCommand(vertical), dodgePriority);
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
        var command = new BaseAttackCommand();
        var nextAttack = GetNextAttack(command, downButtons);

        if (nextAttack == null)
        {
            return false;
        }

        nextAttack.Play(command);
        return true;
    }

    private BaseAttack GetNextAttack(BaseAttackCommand command, Button[] downButtons)
    {
        return (
            from property in attacks
            where property.attack.CanPlay(command) && downButtons.Any(button => button == property.button)
            select property.attack
        ).LastOrDefault();
    }

    private void ExecutePlayable<T>(PlayableBehaviour<T> behaviour, T command, int bufferingPriority) where T : ICommand
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

    private static bool TryExecutePlayable<T>(PlayableBehaviour<T> behaviour, T command) where T : ICommand
    {
        if (!behaviour.CanPlay(command))
        {
            return false;
        }

        behaviour.Play(command);
        return true;
    }

    private static int Direction(float number)
    {
        if (number > Mathf.Epsilon)
        {
            return 1;
        }

        if (number < -Mathf.Epsilon)
        {
            return -1;
        }

        return 0;
    }
}